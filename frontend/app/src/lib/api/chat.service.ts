import { MCP_API_BASE_URL } from '../environment';

class ChatService {
  private baseUrl = `${MCP_API_BASE_URL}/chat`;

  async sendMessage(message: string): Promise<string> {
    const response = await fetch(this.baseUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message }),
      mode: 'cors',
    });

    if (!response.ok) {
      let errorMessage = 'No se pudo obtener la respuesta del asistente';
      try {
        const error = await response.json();
        errorMessage = error.error || error.message || errorMessage;
      } catch {
        errorMessage = `Error ${response.status}: ${response.statusText}`;
      }
      throw new Error(errorMessage);
    }

    const data = await response.json();
    if (data?.reply) {
      return data.reply as string;
    }
    throw new Error('Respuesta vacía del asistente');
  }
}

export const chatService = new ChatService();

