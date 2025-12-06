// Simulación de base de datos en memoria
// En producción, reemplazar con Prisma o tu ORM preferido

export interface User {
  id: string
  email: string
  password: string
  name: string
}

export interface Incident {
  id: string
  userId: string
  description: string
  latitude: number
  longitude: number
  photoUrl?: string
  createdAt: Date
}

// Almacenamiento en memoria (se perderá al reiniciar el servidor)
export const users: User[] = [
  {
    id: "1",
    email: "demo@urbanai.com",
    password: "$2a$10$rOzJqZqZqZqZqZqZqZqZqO", // password: demo123 (hash simulado)
    name: "Usuario Demo",
  },
]

export const incidents: Incident[] = []

