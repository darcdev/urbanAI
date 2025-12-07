import { API_BASE_URL } from '../environment';

export interface CreateLeaderRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  departmentId: string;
  municipalityId: string;
  latitude: number;
  longitude: number;
}

export interface Leader {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  departmentId: string;
  municipalityId: string;
  latitude: number;
  longitude: number;
  createdAt?: string;
  updatedAt?: string;
}

class LeadersService {
  private baseUrl = `${API_BASE_URL}/api/v1/leaders`;

  private getAuthHeaders(): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  async create(leaderData: CreateLeaderRequest): Promise<Leader> {
    try {
      const response = await fetch(this.baseUrl, {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(leaderData),
        mode: 'cors',
      });
      if (!response.ok) {
        let errorMessage = 'Error al crear el líder';
        try {
          const error = await response.json();
          errorMessage = error.message || error.error || errorMessage;
        } catch {
          errorMessage = `Error ${response.status}: ${response.statusText}`;
        }
        throw new Error(errorMessage);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      throw error;
    }
  }

  async getAll(): Promise<Leader[]> {
    try {
      const response = await fetch(this.baseUrl, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      throw error;
    }
  }

  async getById(id: string): Promise<Leader> {
    try {
      const response = await fetch(`${this.baseUrl}/${id}`, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      throw error;
    }
  }

  async delete(id: string): Promise<void> {
    try {
      const response = await fetch(`${this.baseUrl}/${id}`, {
        method: 'DELETE',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }
    } catch (error) {
      throw error;
    }
  }
}

export const leadersService = new LeadersService();
