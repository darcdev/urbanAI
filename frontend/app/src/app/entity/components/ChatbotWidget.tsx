"use client";

import dynamic from "next/dynamic";
import { useMemo } from "react";
import { Bot } from "lucide-react";
import { createChatBotMessage } from "react-chatbot-kit";
import "react-chatbot-kit/build/main.css";

const Chatbot = dynamic(() => import("react-chatbot-kit").then((mod) => mod.default), {
  ssr: false,
});

const ChatbotTyped = Chatbot as unknown as React.ComponentType<{
  config: unknown;
  messageParser: unknown;
  actionProvider: unknown;
}>;

type CreateMessage = typeof createChatBotMessage;
type SetChatState = React.Dispatch<React.SetStateAction<{ messages: unknown[] }>>;

class ActionProvider {
  createChatBotMessage: CreateMessage;
  setStateFunc: SetChatState;

  constructor(createChatBotMessage: CreateMessage, setStateFunc: SetChatState) {
    this.createChatBotMessage = createChatBotMessage;
    this.setStateFunc = setStateFunc;
  }

  greet() {
    this.addMessage("Hola, ¿cómo puedo ayudarte hoy?");
  }

  defaultResponse() {
    this.addMessage("Puedo darte tips sobre los incidentes visibles en el panel.");
  }

  addMessage(text: string) {
    const message = this.createChatBotMessage(text, {});
    this.setStateFunc((prev) => ({
      ...prev,
      messages: [...prev.messages, message],
    }));
  }
}

class MessageParser {
  actionProvider: ActionProvider;

  constructor(actionProvider: ActionProvider) {
    this.actionProvider = actionProvider;
  }

  parse(message: string) {
    const normalized = message.toLowerCase();
    if (normalized.includes("hola") || normalized.includes("buen")) {
      this.actionProvider.greet();
      return;
    }
    this.actionProvider.defaultResponse();
  }
}

export function ChatbotWidget() {
  const config = useMemo(
    () => ({
      botName: "Asistente Urbano",
      initialMessages: [
        createChatBotMessage("Hola, soy tu asistente urbano.", {}),
        createChatBotMessage("Puedo ayudarte con preguntas sobre los incidentes.", {}),
      ],
      placeholderText: "Escribe tu pregunta...",
    }),
    []
  );

  return (
    <div className="w-full max-w-sm sm:max-w-md md:w-[340px] lg:w-[400px] h-[70vh] sm:h-[72vh] md:h-[520px] max-h-[82vh] min-h-[360px] rounded-xl border border-border bg-card text-card-foreground shadow-elevated overflow-hidden flex flex-col">
      <div className="flex items-center gap-3 px-4 py-3 bg-primary text-primary-foreground">
        <div className="flex h-9 w-9 items-center justify-center rounded-full bg-primary-foreground/15 text-primary-foreground">
          <Bot className="h-5 w-5" />
        </div>
        <div className="leading-tight">
          <p className="text-sm font-semibold">Asistente urbano</p>
          <p className="text-xs opacity-80">En línea</p>
        </div>
      </div>
      <div className="flex-1 chatbot-shell">
        <ChatbotTyped config={config} messageParser={MessageParser} actionProvider={ActionProvider} />
      </div>
      <style jsx global>{`
        .chatbot-shell {
          display: flex;
          flex-direction: column;
          height: 100%;
          min-height: 0;
        }
        .chatbot-shell .react-chatbot-kit-chat-container,
        .chatbot-shell .react-chatbot-kit-chat-inner-container {
          display: flex;
          flex-direction: column;
          height: 100% !important;
          width: 100% !important;
          min-height: 0 !important;
        }
        .chatbot-shell .react-chatbot-kit-chat-message-container {
          flex: 1 !important;
          min-height: 0 !important;
          padding: 14px 14px 18px !important;
          overflow-y: auto !important;
          overflow-x: hidden !important;
          display: flex;
          flex-direction: column;
          gap: 10px;
          background: #f8fafc;
          scrollbar-width: thin;
          scrollbar-color: hsl(var(--primary)) transparent;
        }
        .chatbot-shell .react-chatbot-kit-chat-message-container::-webkit-scrollbar {
          width: 8px;
        }
        .chatbot-shell .react-chatbot-kit-chat-message-container::-webkit-scrollbar-thumb {
          background: hsl(var(--primary));
          border-radius: 9999px;
        }
        .chatbot-shell .react-chatbot-kit-chat-bot-message,
        .chatbot-shell .react-chatbot-kit-user-chat-message {
          max-width: 100%;
          max-height: 200px;
          overflow-y: auto !important;
          overflow-x: hidden !important;
          word-break: break-word;
          border-radius: 12px;
          padding: 10px 12px !important;
          box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
        }
        .chatbot-shell .react-chatbot-kit-chat-bot-message {
          background: #0f172a !important;
          color: #e5e7eb !important;
        }
        .chatbot-shell .react-chatbot-kit-chat-user-message {
          background: #ffffff !important;
          color: hsl(var(--foreground)) !important;
          border: 1px solid hsl(var(--border));
        }
        .chatbot-shell .react-chatbot-kit-chat-input-container {
          width: 100%;
          border-top: 1px solid hsl(var(--border));
          background: #f8fafc;
          margin-top: 0;
          box-shadow: 0 -1px 6px rgba(0, 0, 0, 0.04);
        }

        .chatbot-shell .react-chatbot-kit-chat-input-container:focus-within {
          outline: none;
        }
        .chatbot-shell .react-chatbot-kit-chat-input-container:focus-visible {
          outline: none;
        }
        .chatbot-shell .react-chatbot-kit-chat-input {
          background: #ffffff !important;
          color: hsl(var(--foreground)) !important;
          width: 100% !important;
          border: 1px solid hsl(var(--border));
        }
      `}</style>
    </div>
  );
}

