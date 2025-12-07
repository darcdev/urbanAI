import { API_BASE_URL } from '../environment';

export interface Category {
  id: string;
  code: string;
  name: string;
}

export interface Subcategory {
  id: string;
  name: string;
}

export interface IncidentLeader {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
}

export interface ReviewerIncident {
  id: string;
  radicateNumber: string;
  imageUrl: string;
  latitude: number;
  longitude: number;
  citizenEmail: string | null;
  additionalComment: string;
  aiDescription: string;
  category: Category;
  subcategory: Subcategory;
  leader: IncidentLeader;
  status: 'Pending' | 'Accepted' | 'Rejected';
  priority: 'NotEstablished' | 'Low' | 'Medium' | 'High' | 'Critical';
  createdAt: string;
}

export interface MyIncidentsResponse {
  accepted: ReviewerIncident[];
  pending: ReviewerIncident[];
  rejected: ReviewerIncident[];
}

class ReviewerIncidentsService {
  private baseUrl = `${API_BASE_URL}/api/v1/incidents`;

  private getAuthHeaders(): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  async getMyIncidents(): Promise<MyIncidentsResponse> {
    try {
      console.log('📋 Fetching my incidents');
      
      const response = await fetch(`${this.baseUrl}/my-incidents`, {
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
      console.log('✅ My incidents fetched:', data);
      return data;
    } catch (error) {
      console.error('❌ Error fetching my incidents:', error);
      throw error;
    }
  }

  async acceptIncident(incidentId: string, priority: 'Low' | 'Medium' | 'High' | 'Critical'): Promise<void> {
    try {
      console.log(`✅ Accepting incident ${incidentId} with priority ${priority}`);
      
      const response = await fetch(`${this.baseUrl}/${incidentId}/status`, {
        method: 'PATCH',
        headers: this.getAuthHeaders(),
        mode: 'cors',
        body: JSON.stringify({
          status: 'Accepted',
          priority: priority,
        }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Error response:', errorText);
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const text = await response.text();
      if (text) {
        const data = JSON.parse(text);
        console.log('✅ Incident accepted:', data);
      } else {
        console.log('✅ Incident accepted (no response body)');
      }
    } catch (error) {
      console.error('❌ Error accepting incident:', error);
      throw error;
    }
  }

  async rejectIncident(incidentId: string): Promise<void> {
    try {
      console.log(`❌ Rejecting incident ${incidentId}`);
      
      const response = await fetch(`${this.baseUrl}/${incidentId}/status`, {
        method: 'PATCH',
        headers: this.getAuthHeaders(),
        mode: 'cors',
        body: JSON.stringify({
          status: 'Rejected',
        }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Error response:', errorText);
        throw new Error(`Error ${response.status}: ${response.statusText}`);
      }

      const text = await response.text();
      if (text) {
        const data = JSON.parse(text);
        console.log('✅ Incident rejected:', data);
      } else {
        console.log('✅ Incident rejected (no response body)');
      }
    } catch (error) {
      console.error('❌ Error rejecting incident:', error);
      throw error;
    }
  }
}

export const reviewerIncidentsService = new ReviewerIncidentsService();
