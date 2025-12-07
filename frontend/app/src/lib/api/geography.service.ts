import { API_BASE_URL } from '../environment';

export interface Department {
  id: string;
  departmentDaneCode: string;
  name: string;
  latitude: number;
  longitude: number;
}

export interface Municipality {
  id: string;
  municipalityDaneCode: string;
  name: string;
  departmentDaneCode: string;
  latitude: number;
  longitude: number;
}

class GeographyService {
  private baseUrl = `${API_BASE_URL}/api/v1/geography`;

  private getAuthHeaders(): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  async getDepartments(): Promise<Department[]> {
    try {
      const response = await fetch(`${this.baseUrl}/departments`, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        if (response.status === 401) {
          throw new Error('No autorizado. Por favor inicia sesión nuevamente.');
        }
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      throw error;
    }
  }

  async getMunicipalities(departmentDaneCode: string): Promise<Municipality[]> {
    try {
      const response = await fetch(`${this.baseUrl}/municipalities/${departmentDaneCode}`, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        if (response.status === 401) {
          throw new Error('No autorizado. Por favor inicia sesión nuevamente.');
        }
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      throw error;
    }
  }
}

export const geographyService = new GeographyService();
