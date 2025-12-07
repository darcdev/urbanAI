import { API_BASE_URL } from '../environment';

export interface GeoCategory {
  id: string;
  code: string;
  name: string;
}

export interface GeoSubcategory {
  id: string;
  name: string;
}

export interface GeoLeader {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
}

export interface GeographyIncident {
  id: string;
  radicateNumber: string;
  imageUrl: string;
  latitude: number;
  longitude: number;
  citizenEmail: string | null;
  additionalComment: string;
  aiDescription: string;
  category: GeoCategory;
  subcategory: GeoSubcategory;
  leader: GeoLeader;
  status: string;
  priority: string;
  createdAt: string;
}

export interface GeographyIncidentsParams {
  department?: string;
  municipality?: string;
}

class GeographyIncidentsService {
  private baseUrl = `${API_BASE_URL}/api/v1/incidents`;

  private getAuthHeaders(): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    return {
      'Content-Type': 'application/json',
      'ngrok-skip-browser-warning': 'true',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  async getIncidentsByGeography(params?: GeographyIncidentsParams): Promise<GeographyIncident[]> {
    try {
      const queryParams = new URLSearchParams();
      
      if (params?.department) {
        queryParams.append('department', params.department);
      }
      
      if (params?.municipality) {
        queryParams.append('municipality', params.municipality);
      }

      const url = `${this.baseUrl}/by-geography${queryParams.toString() ? `?${queryParams.toString()}` : ''}`;
      
      console.log('🗺️ Fetching incidents by geography:', url);
      
      const response = await fetch(url, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Error response:', errorText);
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      console.log('✅ Incidents fetched:', data);
      return data;
    } catch (error) {
      console.error('❌ Error fetching incidents by geography:', error);
      throw error;
    }
  }
}

export const geographyIncidentsService = new GeographyIncidentsService();
