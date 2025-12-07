FROM node:20-alpine AS base
WORKDIR /app
ENV NODE_ENV=production

FROM base AS deps
RUN apk add --no-cache libc6-compat
COPY package.json package-lock.json ./
RUN npm ci

FROM base AS builder
# Build-time args for public envs (these must start with NEXT_PUBLIC_)
ARG NEXT_PUBLIC_API_URL
ARG NEXT_PUBLIC_MCP_API_URL
ENV NEXT_PUBLIC_API_URL=${NEXT_PUBLIC_API_URL}
ENV NEXT_PUBLIC_MCP_API_URL=${NEXT_PUBLIC_MCP_API_URL}

COPY --from=deps /app/node_modules ./node_modules
COPY . .
ENV NEXT_TELEMETRY_DISABLED=1
RUN npm run build

FROM base AS runner
WORKDIR /app
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1
# NextAuth Configuration - Trust all hosts
ENV AUTH_TRUST_HOST=1
ENV NEXTAUTH_URL_INTERNAL=http://localhost:3000
# Runtime envs (e.g., NEXTAUTH_SECRET, NEXTAUTH_URL) can be injected with docker run -e/--env-file
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public
EXPOSE 3000
CMD ["node", "server.js"]

