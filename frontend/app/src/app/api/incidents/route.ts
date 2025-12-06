import { NextResponse } from "next/server"
import { auth } from "@/app/api/auth/[...nextauth]/route"
import { incidents } from "@/lib/db"

export async function POST(request: Request) {
  try {
    const session = await auth()
    
    if (!session?.user?.id) {
      return NextResponse.json(
        { error: "No autorizado" },
        { status: 401 }
      )
    }

    const body = await request.json()
    const { description, latitude, longitude, photoUrl } = body

    if (!description || latitude === undefined || longitude === undefined) {
      return NextResponse.json(
        { error: "Descripción y ubicación son requeridos" },
        { status: 400 }
      )
    }

    const newIncident = {
      id: String(incidents.length + 1),
      userId: session.user.id,
      description,
      latitude,
      longitude,
      photoUrl,
      createdAt: new Date(),
    }

    incidents.push(newIncident)

    return NextResponse.json(
      { message: "Incidente registrado exitosamente", incident: newIncident },
      { status: 201 }
    )
  } catch (error) {
    return NextResponse.json(
      { error: "Error al registrar incidente" },
      { status: 500 }
    )
  }
}

export async function GET(request: Request) {
  try {
    const session = await auth()
    
    if (!session?.user?.id) {
      return NextResponse.json(
        { error: "No autorizado" },
        { status: 401 }
      )
    }

    const userIncidents = incidents.filter(
      (incident) => incident.userId === session.user.id
    )

    return NextResponse.json({ incidents: userIncidents }, { status: 200 })
  } catch (error) {
    return NextResponse.json(
      { error: "Error al obtener incidentes" },
      { status: 500 }
    )
  }
}

