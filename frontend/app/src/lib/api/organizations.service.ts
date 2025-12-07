import { API_BASE_URL } from '../environment';

export interface CreateOrganizationRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  organizationName: string;
}

export interface Organization {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  organizationName: string;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}

class OrganizationsService {
  private baseUrl = `${API_BASE_URL}/api/v1/organizations`;

  private getAuthHeaders(): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  async create(organizationData: CreateOrganizationRequest): Promise<Organization> {
    try {
      const response = await fetch(this.baseUrl, {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(organizationData),
        mode: 'cors',
      });

      if (!response.ok) {
        let errorMessage = 'Error al crear la organización';
        try {
          const errorText = await response.text();
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
      console.log('✅ Organization created successfully:', data);
      return data;
    } catch (error) {
      console.error('❌ Error creating organization:', error);
      throw error;
    }
  }

  async getAll(): Promise<Organization[]> {
    try {
      const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
      console.log('📋 Fetching all organizations');
      console.log('🔑 Token present:', !!token);
      console.log('🔗 URL:', this.baseUrl);
      
      const response = await fetch(this.baseUrl, {
        method: 'GET',
        headers: this.getAuthHeaders(),
        mode: 'cors',
      });

      console.log('📥 Response status:', response.status);

      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Error response:', errorText);
        throw new Error(`Error ${response.status}: ${response.statusText} - ${errorText}`);
      }

      const data = await response.json();
      console.log('✅ Organizations response:', data);
      // API returns { organizations: [...], totalCount, pageNumber, pageSize, totalPages }
      return data.organizations || [];
    } catch (error) {
      console.error('❌ Error fetching organizations:', error);
      throw error;
    }
  }

  async update(id: string, updateData: { firstName: string; lastName: string }): Promise<Organization> {
    try {
      console.log('✏️ Updating organization:', id, updateData);
      
      const response = await fetch(`${this.baseUrl}/${id}`, {
        method: 'PUT',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(updateData),
        mode: 'cors',
      });

      if (!response.ok) {
        let errorMessage = 'Error al actualizar la organización';
        try {
          const errorText = await response.text();
          console.error('❌ Error response text:', errorText);
          try {
            const error = JSON.parse(errorText);
            console.error('❌ Error parsed:', error);
            errorMessage = error.message || error.error || errorMessage;
          } catch {
            errorMessage = errorText || `Error ${response.status}: ${response.statusText}`;
          }
        } catch {
          errorMessage = `Error ${response.status}: ${response.statusText}`;
        }
        throw new Error(errorMessage);
      }

      // Check if response has content
      const text = await response.text();
      if (text) {
        const data = JSON.parse(text);
        console.log('✅ Organization updated successfully:', data);
        return data;
      }
      
      // If no content, return success with empty object
      console.log('✅ Organization updated successfully (no content)');
      return {} as Organization;
    } catch (error) {
      console.error('❌ Error updating organization:', error);
      throw error;
    }
  }
}

export const organizationsService = new OrganizationsService();
