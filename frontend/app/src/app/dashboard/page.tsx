"use client"

import { useEffect, useState, useRef, useCallback } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import {
  MapPin,
  Camera,
  Send,
  X,
  Filter,
  AlertCircle,
  Users,
  ChevronDown,
  Sparkles,
  CheckCircle2,
  Search,
} from "lucide-react"
import { Logo } from "@/components/logo"

type LeafletMap = {
  setView: (coords: [number, number], zoom?: number) => void
  remove: () => void
}

type LeafletMarker = {
  setLatLng: (coords: [number, number]) => void
}

interface Incident {
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

const categories = [
  { label: "Infraestructura", color: "bg-blue-500/10 text-blue-700 border-blue-200", icon: "🏗️" },
  { label: "Seguridad", color: "bg-red-500/10 text-red-700 border-red-200", icon: "🚨" },
  { label: "Medio Ambiente", color: "bg-green-500/10 text-green-700 border-green-200", icon: "🌱" },
  { label: "Tránsito", color: "bg-yellow-500/10 text-yellow-700 border-yellow-200", icon: "🚦" },
  { label: "Servicios Públicos", color: "bg-purple-500/10 text-purple-700 border-purple-200", icon: "🏛️" },
  { label: "Otro", color: "bg-gray-500/10 text-gray-700 border-gray-200", icon: "📋" },
]

export default function DashboardPage() {
  const [email, setEmail] = useState("")
  const [description, setDescription] = useState("")
  const [location, setLocation] = useState<{ lat: number; lng: number } | null>(null)
  const [photo, setPhoto] = useState<File | null>(null)
  const [photoPreview, setPhotoPreview] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")
  const [success, setSuccess] = useState("")
  const [incidents, setIncidents] = useState<Incident[]>([])
  const [selectedCategory, setSelectedCategory] = useState<string>("all")
  const [showFilters, setShowFilters] = useState(false)
  const [searchCode, setSearchCode] = useState("")
  const [page, setPage] = useState(1)
  const pageSize = 5
  const [showModal, setShowModal] = useState(false)
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [dragActive, setDragActive] = useState(false)
  const mapRef = useRef<HTMLDivElement>(null)
  const leafletRef = useRef<{
    map?: LeafletMap
    marker?: LeafletMarker
  }>({})

  const loadIncidents = useCallback(() => {
    const now = new Date().toISOString()
    const seed: Incident[] = [
      { id: "seed-1", code: "INC-1201", name: "Reporte fotográfico", description: "Hueco grande en la vía principal", latitude: 4.7109, longitude: -74.0721, createdAt: now },
      { id: "seed-2", code: "INC-1202", name: "Reporte fotográfico", description: "Luminaria dañada frente al parque", latitude: 4.7125, longitude: -74.065, createdAt: now },
      { id: "seed-3", code: "INC-1203", name: "Reporte fotográfico", description: "Árbol caído bloqueando andén", latitude: 4.7152, longitude: -74.07, createdAt: now },
      { id: "seed-4", code: "INC-1204", name: "Reporte fotográfico", description: "Semáforo fuera de servicio", latitude: 4.7088, longitude: -74.075, createdAt: now },
      { id: "seed-5", code: "INC-1205", name: "Reporte fotográfico", description: "Basura acumulada en la esquina", latitude: 4.7133, longitude: -74.069, createdAt: now },
      { id: "seed-6", code: "INC-1206", name: "Reporte fotográfico", description: "Rejilla de alcantarilla suelta", latitude: 4.7161, longitude: -74.067, createdAt: now },
      { id: "seed-7", code: "INC-1207", name: "Reporte fotográfico", description: "Poste inclinado cerca de la escuela", latitude: 4.7095, longitude: -74.064, createdAt: now },
      { id: "seed-8", code: "INC-1208", name: "Reporte fotográfico", description: "Fuga de agua en andén", latitude: 4.7118, longitude: -74.071, createdAt: now },
      { id: "seed-9", code: "INC-1209", name: "Reporte fotográfico", description: "Vehículo abandonado", latitude: 4.7144, longitude: -74.073, createdAt: now },
      { id: "seed-10", code: "INC-1210", name: "Reporte fotográfico", description: "Cableado colgando bajo", latitude: 4.7079, longitude: -74.069, createdAt: now },
      { id: "seed-11", code: "INC-1211", name: "Reporte fotográfico", description: "Parque sin iluminación", latitude: 4.7129, longitude: -74.074, createdAt: now },
      { id: "seed-12", code: "INC-1212", name: "Reporte fotográfico", description: "Grafiti en muro público", latitude: 4.7138, longitude: -74.066, createdAt: now },
      { id: "seed-13", code: "INC-1213", name: "Reporte fotográfico", description: "Bache frente a edificio", latitude: 4.7102, longitude: -74.068, createdAt: now },
      { id: "seed-14", code: "INC-1214", name: "Reporte fotográfico", description: "Tapa de registro faltante", latitude: 4.709, longitude: -74.0715, createdAt: now },
      { id: "seed-15", code: "INC-1215", name: "Reporte fotográfico", description: "Arqueta rota en la vía", latitude: 4.7159, longitude: -74.0725, createdAt: now },
    ]

    const stored = localStorage.getItem("incidents")
    if (stored) {
      try {
        const existing: Incident[] = JSON.parse(stored)
        const combined = [...existing]
        seed.forEach((s) => {
          if (!combined.some((i) => i.code === s.code)) {
            combined.push(s)
          }
        })
        if (combined.length < 15) {
          localStorage.setItem("incidents", JSON.stringify(combined))
        }
        setIncidents(combined)
        return
      } catch {
        // fallthrough to seed reset
      }
    }

    localStorage.setItem("incidents", JSON.stringify(seed))
    setIncidents(seed)
  }, [])

  useEffect(() => {
    loadIncidents()
  }, [loadIncidents])

  const saveIncident = (incident: Incident) => {
    const stored = localStorage.getItem("incidents")
    const list: Incident[] = stored ? JSON.parse(stored) : []
    list.unshift(incident)
    localStorage.setItem("incidents", JSON.stringify(list))
    setIncidents(list)
  }

  const generateCode = () => {
    const random = Math.floor(1000 + Math.random() * 9000)
    return `INC-${random}`
  }

  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true)
    } else if (e.type === "dragleave") {
      setDragActive(false)
    }
  }

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    setDragActive(false)

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFile(e.dataTransfer.files[0])
    }
  }

  const handleFile = (file: File) => {
    if (file.type.startsWith("image/")) {
      setPhoto(file)
      const reader = new FileReader()
      reader.onloadend = () => {
        setPhotoPreview(reader.result as string)
      }
      reader.readAsDataURL(file)
    } else {
      setError("Selecciona una imagen válida")
    }
  }

  const handlePhotoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      handleFile(file)
    }
  }

  const removePhoto = () => {
    setPhoto(null)
    setPhotoPreview(null)
    if (fileInputRef.current) {
      fileInputRef.current.value = ""
    }
  }

  useEffect(() => {
    const storedEmail = localStorage.getItem("userEmail")
    if (storedEmail) {
      setEmail(storedEmail)
    }
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError("")
    setSuccess("")
    setLoading(true)

    if (!photoPreview) {
      setError("La foto es obligatoria")
      setLoading(false)
      return
    }

    let finalLocation = location
    if (!finalLocation) {
      if (!navigator.geolocation) {
        setError("Tu navegador no soporta geolocalización")
        setLoading(false)
        return
      }
      try {
        const pos = await new Promise<GeolocationPosition>((resolve, reject) =>
          navigator.geolocation.getCurrentPosition(resolve, reject, { enableHighAccuracy: true })
        )
        finalLocation = {
          lat: pos.coords.latitude,
          lng: pos.coords.longitude,
        }
        setLocation(finalLocation)
      } catch {
        setError("Permite el acceso a la ubicación para enviar el reporte.")
        setLoading(false)
        return
      }
    }

    const photoUrl = photoPreview || undefined

    const newIncident: Incident = {
      id: `incident-${Date.now()}`,
      code: generateCode(),
      name: "Reporte fotográfico",
      description: description.trim() || undefined,
      latitude: finalLocation.lat,
      longitude: finalLocation.lng,
      photoUrl,
      createdAt: new Date().toISOString(),
    }

    saveIncident(newIncident)

    if (email.trim()) {
      localStorage.setItem("userEmail", email.trim())
    }

    setSuccess(`Incidente ${newIncident.code} reportado con éxito`)
    setEmail(email.trim())
    setDescription("")
    setLocation(null)
    setPhoto(null)
    setPhotoPreview(null)
    setLoading(false)
    setShowModal(false)

    setTimeout(() => setSuccess(""), 4000)
  }

  useEffect(() => {
    const refSnapshot = leafletRef.current
    async function initMap() {
      if (!mapRef.current || refSnapshot.map) return
      // @ts-expect-error CSS import for leaflet
      await import("leaflet/dist/leaflet.css").catch(() => {})
      const L = await import("leaflet")
      const map = L.map(mapRef.current).setView([4.710989, -74.07209], 12)
      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: "© OpenStreetMap contributors",
      }).addTo(map)
      const markerIcon = L.icon({
        iconUrl:
          "data:image/svg+xml,%3Csvg width='32' height='40' viewBox='0 0 32 40' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M16 0C8.832 0 3 5.832 3 13c0 9.75 11.314 20.834 12.02 21.53a1.4 1.4 0 0 0 1.96 0C17.686 33.834 29 22.75 29 13 29 5.832 23.168 0 16 0Z' fill='%23ff6b35'/%3E%3Ccircle cx='16' cy='13' r='6' fill='%23fff'/%3E%3C/svg%3E",
        iconSize: [32, 40],
        iconAnchor: [16, 40],
      })
      const marker = L.marker([4.710989, -74.07209], { icon: markerIcon }).addTo(map)
      refSnapshot.map = map
      refSnapshot.marker = marker
    }
    initMap()
    return () => {
      if (refSnapshot.map) {
        refSnapshot.map.remove()
      }
      refSnapshot.map = undefined
      refSnapshot.marker = undefined
    }
  }, [])

  const filteredIncidents = incidents.filter((incident) => {
      const matchCategory =
        selectedCategory === "all" || (incident.category || "Sin categoría") === selectedCategory
    const matchCode = searchCode.trim()
      ? incident.code.toLowerCase().includes(searchCode.trim().toLowerCase())
      : true
    return matchCategory && matchCode
  })

  useEffect(() => {
    setPage(1)
  }, [selectedCategory, searchCode, incidents])

  const totalPages = Math.max(1, Math.ceil(filteredIncidents.length / pageSize))
  const paginatedIncidents = filteredIncidents.slice((page - 1) * pageSize, page * pageSize)

  const flyToIncident = (incident: Incident) => {
    const refSnapshot = leafletRef.current
    if (!refSnapshot.map) return
    const coords: [number, number] = [incident.latitude, incident.longitude]
    refSnapshot.map.setView(coords, 15)
    if (refSnapshot.marker) {
      refSnapshot.marker.setLatLng(coords)
    }
  }

  const applyLocationToMap = (coords: { lat: number; lng: number }) => {
    const refSnapshot = leafletRef.current
    if (refSnapshot.map) {
      refSnapshot.map.setView([coords.lat, coords.lng], 15)
    }
    if (refSnapshot.marker) {
      refSnapshot.marker.setLatLng([coords.lat, coords.lng])
    }
  }

  useEffect(() => {
    if (!showModal) return
    if (!navigator.geolocation) {
      setError("Tu navegador no soporta geolocalización")
      return
    }
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        const coords = { lat: pos.coords.latitude, lng: pos.coords.longitude }
        setLocation(coords)
        applyLocationToMap(coords)
      },
      () => {
        setError("Permite el acceso a la ubicación para enviar el reporte.")
      },
      { enableHighAccuracy: true }
    )
  }, [showModal])

  useEffect(() => {
    if (!showModal) return
    if (!navigator.permissions || !navigator.permissions.query) return

    let permissionStatus: PermissionStatus | null = null

    const handlePermission = (status: PermissionStatus) => {
      if (status.state === "granted") {
        navigator.geolocation.getCurrentPosition(
          (pos) => {
            const coords = { lat: pos.coords.latitude, lng: pos.coords.longitude }
            setLocation(coords)
            applyLocationToMap(coords)
            setError("")
          },
          () => {
            setError("No se pudo obtener tu ubicación.")
          },
          { enableHighAccuracy: true }
        )
      } else {
        setLocation(null)
        setError("Permite el acceso a la ubicación para enviar el reporte.")
      }
    }

    navigator.permissions
      .query({ name: "geolocation" as PermissionName })
      .then((status) => {
        permissionStatus = status
        handlePermission(status)
        status.onchange = () => handlePermission(status)
      })
      .catch(() => {
        // ignore permission query errors
      })

    return () => {
      if (permissionStatus) {
        permissionStatus.onchange = null
      }
    }
  }, [showModal])

  return (
    <div className="relative h-screen w-screen md:overflow-hidden overflow-y-auto bg-gradient-to-br from-background via-background to-muted/20">
      <div className="absolute top-0 left-0 w-full z-50 bg-white/85 backdrop-blur-xl border-b border-border/40 shadow-sm">
        <div className="px-4 py-4 max-w-5xl mx-auto flex items-center justify-between">
          <Logo size="md" />
          <Button
            onClick={() => setShowModal(true)}
            className="rounded-full h-10 px-3 text-sm font-semibold"
          >
            Reportar incidente
          </Button>
        </div>
      </div>

      {success && (
        <div className="absolute top-20 left-1/2 -translate-x-1/2 w-[92vw] max-w-3xl z-[1200]">
          <div className="p-4 text-sm text-green-700 bg-green-50 border border-green-200 rounded-xl shadow-md">
            <div className="flex items-center gap-2">
              <CheckCircle2 className="h-5 w-5 flex-shrink-0" />
              <span>{success}</span>
            </div>
          </div>
        </div>
      )}

      <div className="md:absolute md:inset-0 md:top-[72px] relative flex flex-col">
        <div className="flex flex-1 flex-col md:flex-row overflow-hidden">
          <div className="w-full h-[60vh] md:h-full relative overflow-hidden z-0 shrink-0">
            <div
              ref={mapRef}
              className="absolute inset-0 z-0"
            />
          </div>

          <div className="w-full md:w-auto md:absolute md:top-6 md:right-6 md:bottom-6 md:max-w-[420px] md:min-w-[320px] bg-white/85 backdrop-blur-sm border border-border/60 shadow-lg rounded-xl z-20 mt-4 md:mt-0">
            <Card className="w-full h-full flex flex-col rounded-xl shadow-sm border border-border/60 bg-white/95 md:max-h-full">
              <CardHeader className="pb-3">
                <div className="flex items-center justify-between gap-3 flex-wrap">
                  <div className="flex items-center gap-3">
                    <div className="p-2 bg-primary/10 rounded-xl">
                      <Users className="h-5 w-5 text-primary" />
                    </div>
                  <div>
                    <CardTitle className="text-xl font-bold">Incidentes reportados</CardTitle>
                    <CardDescription>Listado de la comunidad</CardDescription>
                  </div>
                  </div>
                  <div className="px-3 py-1 bg-primary/10 text-primary rounded-full text-sm font-semibold">
                  {filteredIncidents.length} visibles
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-4 flex-1 overflow-y-auto md:h-auto">
                <div className="flex flex-col md:flex-row md:items-center gap-3 pt-6">
                  <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                    placeholder="Buscar por código (ej: INC-1234)"
                      value={searchCode}
                      onChange={(e) => setSearchCode(e.target.value)}
                      className="pl-10 rounded-xl"
                    />
                  </div>
                  <div className="flex items-center gap-2">
                    <Button
                      variant="outline"
                      onClick={() => setShowFilters(!showFilters)}
                      className="h-11 rounded-xl"
                    >
                      <Filter className="h-4 w-4 mr-2" />
                    Filtros
                      <ChevronDown
                        className={`h-4 w-4 ml-2 transition-transform ${showFilters ? "rotate-180" : ""}`}
                      />
                    </Button>
                    <Button
                      variant="ghost"
                      onClick={() => {
                        setSearchCode("")
                        setSelectedCategory("all")
                      }}
                      className="h-11 rounded-xl"
                    >
                    Limpiar
                    </Button>
                  </div>
                </div>

                {showFilters && (
                  <div className="p-4 bg-muted rounded-xl border space-y-3">
                    <div className="flex gap-2 flex-wrap">
                      <Button
                        variant={selectedCategory === "all" ? "default" : "outline"}
                        size="sm"
                        onClick={() => setSelectedCategory("all")}
                        className="rounded-lg"
                      >
                      Todas
                      </Button>
                      {categories.map((cat) => (
                        <Button
                          key={cat.label}
                          variant={selectedCategory === cat.label ? "default" : "outline"}
                          size="sm"
                          onClick={() => setSelectedCategory(cat.label)}
                          className="rounded-lg"
                        >
                          {cat.icon} {cat.label}
                        </Button>
                      ))}
                    </div>
                  </div>
                )}

                <div className="overflow-hidden rounded-xl border border-border/80 flex-1 min-h-0">
                  <div className="grid grid-cols-12 bg-muted/70 text-sm font-semibold text-muted-foreground px-3 py-2">
                    <div className="col-span-3">Código</div>
                    <div className="col-span-5">Nombre</div>
                    <div className="col-span-4 text-right">Fecha</div>
                  </div>
                  {filteredIncidents.length === 0 ? (
                    <div className="p-6 text-center text-muted-foreground text-sm">
                      No hay incidentes para los filtros actuales
                    </div>
                  ) : (
                    <div className="divide-y md:max-h-full md:overflow-y-auto">
                      {paginatedIncidents.map((incident) => (
                        <div
                          key={incident.id}
                          className="grid grid-cols-12 items-center px-3 py-3 transition-colors text-sm cursor-pointer hover:bg-muted/30"
                          onClick={() => flyToIncident(incident)}
                        >
                          <div className="col-span-3 font-semibold text-primary">{incident.code}</div>
                          <div className="col-span-5">{incident.name}</div>
                          <div className="col-span-4 text-right text-muted-foreground">
                            {new Date(incident.createdAt).toLocaleDateString("es-ES", {
                              day: "2-digit",
                              month: "2-digit",
                              year: "2-digit",
                            })}
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                <div className="flex items-center justify-between pt-3 px-3 pb-2">
                  <span className="text-xs text-muted-foreground">
                    Página {page} de {totalPages}
                  </span>
                  <div className="flex gap-2">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => setPage((p) => Math.max(1, p - 1))}
                      disabled={page === 1}
                      className="rounded-lg text-xs px-3"
                    >
                      Anterior
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                      disabled={page === totalPages}
                      className="rounded-lg text-xs px-3"
                    >
                      Siguiente
                    </Button>
                  </div>
                </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>

      {showModal && (
        <div className="fixed inset-0 z-[1300] flex items-center justify-center bg-black/60 backdrop-blur-sm px-4">
          <div className="w-full max-w-3xl">
            <Card className="shadow-2xl border border-border/60 bg-white overflow-hidden rounded-2xl max-h-[85vh] flex flex-col">
              <div className="h-1.5 bg-gradient-to-r from-primary via-primary/80 to-primary/60" />
              <CardHeader className="pb-4">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="p-2 bg-primary/10 rounded-xl">
                      <Sparkles className="h-5 w-5 text-primary" />
                    </div>
                    <div>
                  <CardTitle className="text-xl font-semibold text-foreground">Reporta un incidente</CardTitle>
                  <CardDescription className="text-xs mt-1 text-muted-foreground">
                    Completa el formulario, solicitaremos tu ubicación al enviar y adjunta una foto
                  </CardDescription>
                    </div>
                  </div>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => {
                      setShowModal(false)
                      setDescription("")
                      setLocation(null)
                      setPhoto(null)
                      setPhotoPreview(null)
                      setError("")
                    }}
                    className="h-9 w-9 rounded-full hover:bg-destructive/10 hover:text-destructive"
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-6 overflow-y-auto pr-2 pl-2 md:pr-3 md:pl-3 flex-1">
                <form onSubmit={handleSubmit} className="space-y-6">
                  {error && (
                    <div className="p-4 text-sm text-red-700 bg-red-50 border border-red-200 rounded-xl">
                      <div className="flex items-center gap-2">
                        <AlertCircle className="h-5 w-5 flex-shrink-0" />
                        <span>{error}</span>
                      </div>
                    </div>
                  )}

                  <div className="space-y-3">
                    <Label htmlFor="email-modal" className="text-sm font-semibold text-foreground">
                      Correo electrónico (opcional)
                    </Label>
                    <Input
                      id="email-modal"
                      type="email"
                      placeholder="tu@email.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="h-11 text-sm rounded-xl border-2 border-border focus:border-primary focus:ring-2 focus:ring-primary/20"
                    />
                  </div>

                  <div className="space-y-3">
                    <Label className="text-sm font-semibold">
                      Ubicación (se solicitará al enviar) *
                    </Label>
                    {location ? (
                      <div className="p-4 bg-primary/5 rounded-xl border border-primary/20">
                        <div className="flex items-start gap-3">
                          <div className="p-2 bg-primary/10 rounded-xl">
                            <MapPin className="h-5 w-5 text-primary" />
                          </div>
                          <div className="flex-1 min-w-0">
                            <p className="text-sm font-medium">Ubicación lista</p>
                            <p className="text-xs text-muted-foreground font-mono break-all">
                              {location.lat.toFixed(6)}, {location.lng.toFixed(6)}
                            </p>
                          </div>
                        </div>
                      </div>
                    ) : (
                      <div className="p-4 bg-muted rounded-xl border border-border text-sm text-muted-foreground">
                        Solicitaremos tu ubicación cuando envíes el reporte.
                      </div>
                    )}
                  </div>

                  <div className="space-y-3">
                    <Label className="text-sm font-semibold">
                      Foto del incidente (obligatoria)
                    </Label>
                    <div
                      className={`relative border-2 border-dashed rounded-2xl p-5 text-center transition-all ${
                        dragActive
                          ? "border-primary bg-primary/10"
                          : "border-muted-foreground/30 hover:border-primary/50 hover:bg-primary/5"
                      }`}
                      onDragEnter={handleDrag}
                      onDragLeave={handleDrag}
                      onDragOver={handleDrag}
                      onDrop={handleDrop}
                    >
                      <input
                        ref={fileInputRef}
                        type="file"
                        accept="image/*"
                        capture="environment"
                        onChange={handlePhotoChange}
                        className="hidden"
                        id="photo-upload-modal"
                      />
                      {photoPreview ? (
                        <div className="space-y-3">
                          <div className="relative inline-block">
                            <img
                              src={photoPreview || ""}
                              alt="Vista previa"
                              className="max-h-48 max-w-full rounded-xl shadow-lg"
                            />
                            <Button
                              type="button"
                              variant="destructive"
                              size="icon"
                              className="absolute -top-2 -right-2 h-8 w-8 rounded-full"
                              onClick={removePhoto}
                            >
                              <X className="h-4 w-4" />
                            </Button>
                          </div>
                          <p className="text-sm text-muted-foreground break-all font-medium">{photo?.name}</p>
                        </div>
                      ) : (
                        <div className="space-y-2">
                          <Camera className="h-9 w-9 mx-auto text-primary" />
                          <div>
                            <label
                              htmlFor="photo-upload-modal"
                              className="cursor-pointer text-primary hover:underline font-semibold"
                            >
                              Toca para subir
                            </label>
                            <p className="text-sm text-muted-foreground">o arrastra una imagen aquí</p>
                          </div>
                          <p className="text-xs text-muted-foreground">PNG, JPG hasta 10MB</p>
                        </div>
                      )}
                    </div>
                  </div>

                  <div className="space-y-3">
                    <Label htmlFor="description-modal" className="text-sm font-semibold text-foreground">
                      Contexto adicional (opcional)
                    </Label>
                    <Textarea
                      id="description-modal"
                      placeholder="Agrega una nota breve..."
                      value={description}
                      onChange={(e) => setDescription(e.target.value)}
                      rows={2}
                      className="text-sm rounded-xl border-2 border-border focus:border-primary focus:ring-2 focus:ring-primary/20"
                    />
                  </div>

                  <div className="flex flex-col sm:flex-row gap-3 pt-2">
                    <Button
                      type="submit"
                      disabled={loading || !photoPreview}
                      className="flex-1 h-11 text-sm font-semibold rounded-xl bg-primary text-primary-foreground hover:bg-primary/90"
                    >
                      <Send className="mr-2 h-5 w-5" />
                      {loading ? "Enviando..." : "Enviar incidente"}
                    </Button>
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => {
                        setShowModal(false)
                        setDescription("")
                        setLocation(null)
                        setPhoto(null)
                        setPhotoPreview(null)
                        setError("")
                      }}
                      className="h-11 text-sm rounded-xl"
                    >
                      Cancelar
                    </Button>
                  </div>
                </form>
              </CardContent>
            </Card>
          </div>
        </div>
      )}
    </div>
  )
}
