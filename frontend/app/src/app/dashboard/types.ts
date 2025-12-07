export interface Incident {
  id: string
  code: string
  name: string
  description?: string
  latitude: number
  longitude: number
  photoUrl?: string
  category?: string
  createdAt: string
}

