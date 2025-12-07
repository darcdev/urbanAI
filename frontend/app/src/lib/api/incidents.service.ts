import { API_BASE_URL } from '../environment';

export interface CreateIncidentRequest {
  Image: string;
  Latitude: number;
  Longitude: number;
  CitizenEmail: string;
  AdditionalComment: string;
}

export interface Incident {
  id: string;
  Image: string;
  Latitude: number;
  Longitude: number;
  CitizenEmail: string;
  AdditionalComment: string;
  createdAt?: string;
  updatedAt?: string;
}

class IncidentsService {
  private baseUrl = `${API_BASE_URL}/api/v1/incidents`;

  private getAuthHeaders(): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    return {
      'Content-Type': 'application/json',
      'ngrok-skip-browser-warning': 'true',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  async create(incidentData: CreateIncidentRequest): Promise<Incident> {
    try {
      console.log('📝 Creating incident with binary image');
      
      const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
      
      // Convert base64 to binary Blob
      const base64Data = incidentData.Image;
      const byteCharacters = atob(base64Data);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: 'image/jpeg' }); // or image/png
      
      // Create FormData with binary file
      const formData = new FormData();
      formData.append('Image', blob, 'incident.jpg');
      formData.append('Latitude', incidentData.Latitude.toString());
      formData.append('Longitude', incidentData.Longitude.toString());
      formData.append('CitizenEmail', incidentData.CitizenEmail);
      formData.append('AdditionalComment', incidentData.AdditionalComment);
      
      console.log('📦 FormData prepared with binary image, size:', blob.size, 'bytes');
      
      const response = await fetch(this.baseUrl, {
        method: 'POST',
        headers: {
          // Don't set Content-Type - browser will set it automatically with boundary for multipart/form-data
          ...(token && { 'Authorization': `Bearer ${token}` }),
        },
        body: formData,
      });

      console.log('📥 Response status:', response.status);
      console.log('📥 Response headers:', Object.fromEntries(response.headers.entries()));

      if (!response.ok) {
        let errorMessage = 'Error al crear el incidente';
        try {
          const errorText = await response.text();
          console.error('❌ Error response:', errorText);
          try {
            const error = JSON.parse(errorText);
            errorMessage = error.message || error.error || errorMessage;
          } catch {
            errorMessage = errorText || `Error ${response.status}: ${response.statusText}`;
          }
        } catch {
          errorMessage = `Error ${response.status}: ${response.statusText}`;
        }
        throw new Error(errorMessage);
      }

      const data = await response.json();
      console.log('✅ Incident created successfully:', data);
      return data;
    } catch (error) {
      console.error('❌ Error creating incident:', error);
      throw error;
    }
  }

  async getAll(): Promise<Incident[]> {
    try {
      console.log('📋 Fetching all incidents');
      
      const response = await fetch(this.baseUrl, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      console.log('✅ Incidents loaded:', data.length);
      return data;
    } catch (error) {
      console.error('❌ Error fetching incidents:', error);
      throw error;
    }
  }

  async getById(id: string): Promise<Incident> {
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
      console.error('❌ Error fetching incident:', error);
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

      console.log('✅ Incident deleted successfully');
    } catch (error) {
      console.error('❌ Error deleting incident:', error);
      throw error;
    }
  }
}

export const incidentsService = new IncidentsService();
