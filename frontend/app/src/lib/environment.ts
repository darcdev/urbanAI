export const environment = {
  apiUrl: process.env.NEXT_PUBLIC_API_URL || 'https://3jk7l7bp-44385.use2.devtunnels.ms',
  mcpApiUrl: process.env.NEXT_PUBLIC_MCP_API_URL || 'http://localhost:4000',
  production: process.env.NODE_ENV === 'production',
};

export const API_BASE_URL = environment.apiUrl;
export const MCP_API_BASE_URL = environment.mcpApiUrl;
