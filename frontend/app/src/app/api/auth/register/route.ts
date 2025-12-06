import { NextResponse } from "next/server"
import bcrypt from "bcryptjs"
import { users } from "@/lib/db"

export async function POST(request: Request) {
  try {
    const body = await request.json()
    const { email, password, name } = body

    if (!email || !password || !name) {
      return NextResponse.json(
        { error: "Todos los campos son requeridos" },
        { status: 400 }
      )
    }

    // Verificar si el usuario ya existe
    const existingUser = users.find((u) => u.email === email)
    if (existingUser) {
      return NextResponse.json(
        { error: "El usuario ya existe" },
        { status: 400 }
      )
    }

    // Hash de la contraseña
    const hashedPassword = await bcrypt.hash(password, 10)

    // Crear nuevo usuario
    const newUser = {
      id: String(users.length + 1),
      email,
      password: hashedPassword,
      name,
    }

    users.push(newUser)

    return NextResponse.json(
      { message: "Usuario registrado exitosamente", user: { id: newUser.id, email: newUser.email, name: newUser.name } },
      { status: 201 }
    )
  } catch (error) {
    return NextResponse.json(
      { error: "Error al registrar usuario" },
      { status: 500 }
    )
  }
}

