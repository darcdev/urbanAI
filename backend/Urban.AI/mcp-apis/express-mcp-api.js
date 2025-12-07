#!/usr/bin/env node
import "dotenv/config";
import cors from "cors";
import express from "express";
import fetch from "node-fetch";
import { spawn } from "child_process";
import { randomUUID } from "crypto";
import path from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const MCP_SERVER_PATH = path.join(__dirname, "postgres-mcp-server.js");

const {
  MCP_POSTGRES_URL = "postgres://postgres:postgres@172.20.10.7:5432/urbanai",
  LM_STUDIO_API_URL = "http://172.20.10.2:1234/v1",
  LM_STUDIO_MODEL = "gemma-3n-e4b",
  PORT = 4000,
} = process.env;

const SYSTEM_MESSAGE =
  "Eres el asistente de UrbanAI. Responde en español claro y en texto plano (sin markdown, listas ni código). Usa solo la base de datos Postgres disponible. Las tablas principales son: incidents (radicate_number/filing_number, image_path, location_latitude, location_longitude, citizen_email, additional_comment, caption, ai_description, category, severity, status, priority, created_at, attention_date, municipality_id, leader_id). No inventes otras tablas ni columnas. Si no tienes suficiente información, pide un dato adicional en lugar de ejecutar consultas inválidas.";

const formatUserMessage = (text = "") =>
  `User request:\n${text}\n\nReturn a clear, concise answer.`;

const parseToolRequest = (content) => {
  if (!content || typeof content !== "string") return null;
  let text = content.trim();
  const fenced = text.match(/```(?:json)?\s*([\s\S]*?)```/i);
  if (fenced?.[1]) {
    text = fenced[1].trim();
  }
  try {
    const parsed = JSON.parse(text);
    const payload = parsed.tool_request || parsed;
    if (payload?.name === "query" && payload.arguments) {
      return payload;
    }
  } catch {
    return null;
  }
  return null;
};

const toReadable = (content) => {
  let value = content;
  if (typeof content === "string") {
    try {
      value = JSON.parse(content);
    } catch {
      value = content;
    }
  }
  if (Array.isArray(value)) {
    if (value.length === 0) return "No rows.";
    const headers = Object.keys(value[0] || {});
    const lines = value.map((row) =>
      headers.map((h) => `${h}: ${row?.[h] ?? ""}`).join(" | ")
    );
    return lines.join("\n");
  }
  if (value && typeof value === "object") {
    return Object.entries(value)
      .map(([k, v]) => `${k}: ${v}`)
      .join("\n");
  }
  return String(value ?? "");
};

function createMcpClient() {
  const proc = spawn("node", [MCP_SERVER_PATH, MCP_POSTGRES_URL], {
    stdio: ["pipe", "pipe", "inherit"],
  });
  const pending = new Map();
  let buffer = "";

  proc.stdout.on("data", (chunk) => {
    buffer += chunk.toString();
    const parts = buffer.split("\n");
    buffer = parts.pop() ?? "";
    for (const line of parts) {
      if (!line.trim()) continue;
      let msg;
      try {
        msg = JSON.parse(line);
      } catch {
        continue;
      }
      if (msg.id && pending.has(msg.id)) {
        const { resolve, reject } = pending.get(msg.id);
        pending.delete(msg.id);
        if (msg.error) {
          reject(new Error(msg.error.message || "MCP error"));
        } else {
          resolve(msg.result);
        }
      }
    }
  });

  proc.on("exit", (code) => {
    for (const { reject } of pending.values()) {
      reject(new Error(`MCP exited with code ${code ?? "null"}`));
    }
    pending.clear();
  });

  const send = (method, params) => {
    const id = randomUUID();
    const payload = { jsonrpc: "2.0", id, method, params };
    return new Promise((resolve, reject) => {
      pending.set(id, { resolve, reject });
      proc.stdin.write(`${JSON.stringify(payload)}\n`, (err) => {
        if (err) {
          pending.delete(id);
          reject(err);
        }
      });
    });
  };

  const ready = send("initialize", {
    clientInfo: { name: "express-mcp-proxy", version: "0.1.0" },
    protocolVersion: "2024-11-05",
    capabilities: {},
  });

  return { send, ready, close: () => proc.kill("SIGTERM") };
}

const mcpClient = createMcpClient();

async function callMcpQuery(sql) {
  await mcpClient.ready;
  return mcpClient.send("tools/call", {
    name: "query",
    arguments: { sql },
  });
}

async function callLms(messages, tools) {
  const res = await fetch(`${LM_STUDIO_API_URL}/chat/completions`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      model: LM_STUDIO_MODEL,
      messages,
      tools,
    }),
  });
  if (!res.ok) {
    throw new Error(`LM Studio error ${res.status}: ${await res.text()}`);
  }
  return res.json();
}

const tools = [
  {
    type: "function",
    function: {
      name: "query",
      description: "Run a read-only SQL query via MCP Postgres",
      parameters: {
        type: "object",
        properties: { sql: { type: "string" } },
        required: ["sql"],
      },
    },
  },
];

const app = express();
app.use(
  cors({
    origin: "*",
  })
);
app.options("*", cors({ origin: "*" }));
app.use(express.json());

app.post("/chat", async (req, res) => {
  const userMessage = req.body?.message || "Hello";
  try {
    const completion = await callLms(
      [
        { role: "system", content: SYSTEM_MESSAGE },
        { role: "user", content: formatUserMessage(userMessage) },
      ],
      tools
    );

    const choice = completion.choices?.[0];
    const toolCall = choice?.message?.tool_calls?.[0];
    const parsedTool = parseToolRequest(choice?.message?.content);

    const activeToolCall = parsedTool
      ? {
          id: "content-tool-call",
          type: "function",
          function: {
            name: parsedTool.name,
            arguments: JSON.stringify(parsedTool.arguments),
          },
        }
      : toolCall;

    if (activeToolCall?.function?.name === "query") {
      const rawArgs = activeToolCall.function.arguments;
      let sql = "";
      if (typeof rawArgs === "string") {
        try {
          const parsed = JSON.parse(rawArgs);
          sql = parsed.sql || rawArgs;
        } catch {
          sql = rawArgs;
        }
      } else if (rawArgs?.sql) {
        sql = rawArgs.sql;
      }
      if (!sql) throw new Error("Missing sql from tool call");

      const queryResult = await callMcpQuery(sql);
      const readable = toReadable(queryResult?.content?.[0]?.text ?? queryResult);

      const second = await callLms([
        { role: "system", content: SYSTEM_MESSAGE },
        { role: "user", content: formatUserMessage(userMessage) },
        { role: "assistant", tool_calls: [activeToolCall] },
        {
          role: "tool",
          tool_call_id: activeToolCall.id || "call-1",
          content: readable,
        },
      ]);

      const reply =
        second.choices?.[0]?.message?.content ||
        "No response available right now.";
      res.json({ reply });
      return;
    }

    const reply = choice?.message?.content || "";
    res.json({ reply });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

app.listen(PORT, () => {
  console.log(`Express MCP bridge listening on ${PORT}`);
});

