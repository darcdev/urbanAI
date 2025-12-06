"use client"

import { useEffect, useState } from "react"
import { useSession, signOut } from "next-auth/react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { MapPin, Camera, Send, LogOut, AlertCircle } from "lucide-react"
import { Logo } from "@/components/logo"

interface Incident {
  id: string
  description: string
  latitude: number
  longitude: number
  photoUrl?: string
  createdAt: string
}

export default function DashboardPage() {
  const { data: session, status } = useSession()
  const router = useRouter()
  const [description, setDescription] = useState("")
  const [location, setLocation] = useState<{ lat: number; lng: number } | null>(null)
  const [photo, setPhoto] = useState<File | null>(null)
  const [photoPreview, setPhotoPreview] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [success, setSuccess] = useState("")
  const [incidents, setIncidents] = useState<Incident[]>([])


  useEffect(() => {
    fetchIncidents()
  }, [])

  useEffect(() => {
    // Obtener ubicación del usuario
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setLocation({
            lat: position.coords.latitude,
            lng: position.coords.longitude,
          })
        },
        (error) => {
          console.error("Error obteniendo ubicación:", error)
          setError("No se pudo obtener tu ubicación. Por favor, permite el acceso a la ubicación.")
        }
      )
    } else {
      setError("Tu navegador no soporta geolocalización")
    }
  }, [])

  const fetchIncidents = async () => {
    try {
      const response = await fetch("/api/incidents")
      if (response.ok) {
        const data = await response.json()
        setIncidents(data.incidents || [])
      }
    } catch (error) {
      console.error("Error obteniendo incidentes:", error)
    }
  }

  const handlePhotoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      setPhoto(file)
      const reader = new FileReader()
      reader.onloadend = () => {
        setPhotoPreview(reader.result as string)
      }
      reader.readAsDataURL(file)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError("")
    setSuccess("")
    setLoading(true)

    if (!location) {
      setError("Esperando ubicación...")
      setLoading(false)
      return
    }

    try {
      let photoUrl = ""
      
      // Si hay foto, subirla (simulado - en producción usar servicio de almacenamiento)
      if (photo) {
        // Por ahora, solo guardamos el nombre del archivo
        // En producción, subirías a Cloudinary, S3, etc.
        photoUrl = URL.createObjectURL(photo)
      }

      const response = await fetch("/api/incidents", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          description,
          latitude: location.lat,
          longitude: location.lng,
          photoUrl,
        }),
      })

      const data = await response.json()

      if (!response.ok) {
        setError(data.error || "Error al registrar incidente")
      } else {
        setSuccess("Incidente registrado exitosamente")
        setDescription("")
        setPhoto(null)
        setPhotoPreview(null)
        fetchIncidents()
      }
    } catch (error) {
      setError("Error al registrar incidente")
    } finally {
      setLoading(false)
    }
  }


  return (
    <div className="min-h-screen bg-white bg-pattern">
      <div className="container mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-8">
          <div>
            <Logo size="lg" />
            <p className="text-muted-foreground">
              {session?.user?.name ? `Bienvenido, ${session.user.name}` : "Bienvenido"}
            </p>
          </div>
          {session && (
            <Button
              variant="outline"
              onClick={() => signOut({ callbackUrl: "/login" })}
            >
              <LogOut className="mr-2 h-4 w-4" />
              Cerrar sesión
            </Button>
          )}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Formulario de registro */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertCircle className="h-5 w-5" />
                Registrar Incidente
              </CardTitle>
              <CardDescription>
                Reporta un incidente en tu ciudad con ubicación y foto
              </CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-4">
                {error && (
                  <div className="p-3 text-sm text-red-500 bg-red-500/10 border border-red-500/20 rounded-md">
                    {error}
                  </div>
                )}
                {success && (
                  <div className="p-3 text-sm text-green-500 bg-green-500/10 border border-green-500/20 rounded-md">
                    {success}
                  </div>
                )}

                <div className="space-y-2">
                  <Label htmlFor="description">Descripción del incidente</Label>
                  <Textarea
                    id="description"
                    placeholder="Describe el incidente que observaste..."
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                    rows={4}
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="location">Ubicación</Label>
                  {location ? (
                    <div className="flex items-center gap-2 p-3 bg-muted rounded-md">
                      <MapPin className="h-4 w-4" />
                      <span className="text-sm">
                        Lat: {location.lat.toFixed(6)}, Lng: {location.lng.toFixed(6)}
                      </span>
                    </div>
                  ) : (
                    <div className="p-3 bg-muted rounded-md text-sm text-muted-foreground">
                      Obteniendo ubicación...
                    </div>
                  )}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="photo">Foto (opcional)</Label>
                  <div className="flex items-center gap-4">
                    <Input
                      id="photo"
                      type="file"
                      accept="image/*"
                      onChange={handlePhotoChange}
                      className="cursor-pointer"
                    />
                    {photoPreview && (
                      <img
                        src={photoPreview}
                        alt="Preview"
                        className="w-20 h-20 object-cover rounded-md"
                      />
                    )}
                  </div>
                </div>

                <Button type="submit" className="w-full" disabled={loading || !location}>
                  <Send className="mr-2 h-4 w-4" />
                  {loading ? "Enviando..." : "Enviar Incidente"}
                </Button>
              </form>
            </CardContent>
          </Card>

          {/* Lista de incidentes */}
          <Card>
            <CardHeader>
              <CardTitle>Mis Incidentes</CardTitle>
              <CardDescription>
                Historial de incidentes reportados
              </CardDescription>
            </CardHeader>
            <CardContent>
              {incidents.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">
                  <AlertCircle className="h-12 w-12 mx-auto mb-4 opacity-50" />
                  <p>No has reportado incidentes aún</p>
                </div>
              ) : (
                <div className="space-y-4">
                  {incidents.map((incident) => (
                    <div
                      key={incident.id}
                      className="p-4 border rounded-md space-y-2"
                    >
                      <p className="text-sm">{incident.description}</p>
                      <div className="flex items-center gap-2 text-xs text-muted-foreground">
                        <MapPin className="h-3 w-3" />
                        <span>
                          {incident.latitude.toFixed(4)}, {incident.longitude.toFixed(4)}
                        </span>
                      </div>
                      {incident.photoUrl && (
                        <img
                          src={incident.photoUrl}
                          alt="Incidente"
                          className="w-full h-32 object-cover rounded-md mt-2"
                        />
                      )}
                      <p className="text-xs text-muted-foreground">
                        {new Date(incident.createdAt).toLocaleString()}
                      </p>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}

