import { API_BASE_URL } from '../environment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  firstName: string;
  lastName: string;
  password: string;
}

export interface AuthResponse {
  id?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  user?: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
  };
  token?: string;
  accessToken?: string;
  message?: string;
  roles?: string[];
}

class AuthService {
  private baseUrl = `${API_BASE_URL}/api/v1/auth`;

  async login(credentials: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await fetch(`${this.baseUrl}/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
        mode: 'cors',
      });

      if (!response.ok) {
        let errorMessage = 'Error al iniciar sesión';
        try {
          const error = await response.json();
          errorMessage = error.message || error.error || errorMessage;
        } catch {
          errorMessage = `Error ${response.status}: ${response.statusText}`;
        }
        throw new Error(errorMessage);
      }

      const data = await response.json();
      const authToken = data.token || data.accessToken;
      
      if (authToken) {
        localStorage.setItem('token', authToken);
        localStorage.setItem('user', JSON.stringify(data.user || data));
      }

      return data;
    } catch (error) {
      if (error instanceof TypeError && error.message.includes('Failed to fetch')) {
        throw new Error('No se pudo conectar con el servidor. Verifica que el backend esté corriendo y que CORS esté habilitado.');
      }
      throw error;
    }
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await fetch(`${this.baseUrl}/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
        mode: 'cors',
      });

      if (!response.ok) {
        let errorMessage = 'Error al registrarse';
        try {
          const error = await response.json();
          errorMessage = error.message || error.error || errorMessage;
        } catch {
          errorMessage = `Error ${response.status}: ${response.statusText}`;
        }
        throw new Error(errorMessage);
      }

      const data = await response.json();

      const authToken = data.token || data.accessToken;

      if (authToken) {
        localStorage.setItem('token', authToken);
        localStorage.setItem('user', JSON.stringify(data.user || data));
      }

      return data;
    } catch (error) {
      if (error instanceof TypeError && error.message.includes('Failed to fetch')) {
        throw new Error('No se pudo conectar con el servidor. Verifica que el backend esté corriendo y que CORS esté habilitado.');
      }
      throw error;
    }
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  async whoami(): Promise<AuthResponse> {
    try {
      const token = this.getToken();
      if (!token) {
        throw new Error('No hay sesión activa');
      }

      const response = await fetch(`${this.baseUrl}/whoami`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        mode: 'cors',
      });

      if (!response.ok) {
        if (response.status === 401) {
          this.logout();
        }
        throw new Error('Sesión inválida');
      }

      const data = await response.json();
      
      // Guardar la información actualizada del usuario en localStorage
      if (data) {
        localStorage.setItem('user', JSON.stringify(data));
      }
      
      return data;
    } catch (error) {
      throw error;
    }
  }

  getToken(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('token');
    }
    return null;
  }

  getUser() {
    if (typeof window !== 'undefined') {
      const user = localStorage.getItem('user');
      return user ? JSON.parse(user) : null;
    }
    return null;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  async verifyToken(): Promise<boolean> {
    try {
      const token = this.getToken();
      if (!token) {
        return false;
      }
      await this.whoami();
      return true;
    } catch {
      return false;
    }
  }
}

export const authService = new AuthService();
