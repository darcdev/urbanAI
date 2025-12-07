export const environment = {
  apiUrl: process.env.NEXT_PUBLIC_API_URL || 'https://overbearing-addyson-unphlegmatical.ngrok-free.dev',
  mcpApiUrl: process.env.NEXT_PUBLIC_MCP_API_URL || 'http://147.93.184.134:4000',
  production: process.env.NODE_ENV === 'production',
};

export const API_BASE_URL = environment.apiUrl;
export const MCP_API_BASE_URL = environment.mcpApiUrl;
