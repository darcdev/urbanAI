import { ChangeEvent, FormEvent } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"
import { AlertCircle, Camera, CheckCircle2, MapPin, Sparkles, Send, X } from "lucide-react"

interface ReportModalProps {
  open: boolean
  name: string
  description: string
  location: { lat: number; lng: number } | null
  locationLoading: boolean
  photoPreview: string | null
  loading: boolean
  error: string
  onClose: () => void
  onSubmit: (e: FormEvent) => void
  onNameChange: (value: string) => void
  onDescriptionChange: (value: string) => void
  onLocate: () => void
  onPhotoChange: (e: ChangeEvent<HTMLInputElement>) => void
  onRemovePhoto: () => void
  dragActive: boolean
  onDrag: (e: React.DragEvent) => void
  onDrop: (e: React.DragEvent) => void
}

export function ReportModal({
  open,
  name,
  description,
  location,
  locationLoading,
  photoPreview,
  loading,
  error,
  onClose,
  onSubmit,
  onNameChange,
  onDescriptionChange,
  onLocate,
  onPhotoChange,
  onRemovePhoto,
  dragActive,
  onDrag,
  onDrop,
}: ReportModalProps) {
  if (!open) return null

  return (
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
                  <CardTitle className="text-2xl font-bold text-foreground">Reporta un incidente</CardTitle>
                  <CardDescription className="text-sm mt-1 text-muted-foreground">
                    Completa el formulario, obtén tu ubicación y adjunta una imagen
                  </CardDescription>
                </div>
              </div>
              <Button
                variant="ghost"
                size="icon"
                onClick={onClose}
                className="h-9 w-9 rounded-full hover:bg-destructive/10 hover:text-destructive"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </CardHeader>
          <CardContent className="space-y-6 overflow-y-auto pr-1 flex-1">
            <form onSubmit={onSubmit} className="space-y-6">
              {error && (
                <div className="p-4 text-sm text-red-700 bg-red-50 border border-red-200 rounded-xl">
                  <div className="flex items-center gap-2">
                    <AlertCircle className="h-5 w-5 flex-shrink-0" />
                    <span>{error}</span>
                  </div>
                </div>
              )}

              <div className="space-y-3">
                <Label htmlFor="name-modal" className="text-base font-semibold text-foreground">
                  Nombre del reporte *
                </Label>
                <Input
                  id="name-modal"
                  placeholder="Ej: Bache en la calle principal"
                  value={name}
                  onChange={(e) => onNameChange(e.target.value)}
                  required
                  className="h-12 text-base rounded-xl border-2 border-border focus:border-primary focus:ring-2 focus:ring-primary/20"
                />
              </div>

              <div className="space-y-3">
                <Label className="text-base font-semibold">
                  Ubicación *
                </Label>
                {location ? (
                  <div className="p-4 bg-primary/5 rounded-xl border border-primary/20">
                    <div className="flex items-start gap-3">
                      <div className="p-2 bg-primary/10 rounded-xl">
                        <MapPin className="h-5 w-5 text-primary" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium">Ubicación obtenida</p>
                        <p className="text-xs text-muted-foreground font-mono break-all">
                          {location.lat.toFixed(6)}, {location.lng.toFixed(6)}
                        </p>
                      </div>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={onLocate}
                        className="rounded-lg"
                      >
                        Actualizar
                      </Button>
                    </div>
                  </div>
                ) : (
                  <Button
                    type="button"
                    variant="outline"
                    onClick={onLocate}
                    disabled={locationLoading}
                    className="w-full h-12 text-base rounded-xl"
                  >
                    <MapPin className="mr-2 h-5 w-5" />
                    {locationLoading ? "Obteniendo ubicación..." : "Localizarme"}
                  </Button>
                )}
              </div>

              <div className="space-y-3">
                <Label className="text-base font-semibold">
                  Foto del incidente (opcional)
                </Label>
                <div
                  className={`relative border-2 border-dashed rounded-2xl p-5 text-center transition-all ${
                    dragActive
                      ? "border-primary bg-primary/10"
                      : "border-muted-foreground/30 hover:border-primary/50 hover:bg-primary/5"
                  }`}
                  onDragEnter={onDrag}
                  onDragLeave={onDrag}
                  onDragOver={onDrag}
                  onDrop={onDrop}
                >
                  <input
                    type="file"
                    accept="image/*"
                    capture="environment"
                    onChange={onPhotoChange}
                    className="hidden"
                    id="photo-upload-modal"
                  />
                  {photoPreview ? (
                    <div className="space-y-3">
                      <div className="relative inline-block">
                        <img
                          src={photoPreview || ""}
                          alt="Preview"
                          className="max-h-48 max-w-full rounded-xl shadow-lg"
                        />
                        <Button
                          type="button"
                          variant="destructive"
                          size="icon"
                          className="absolute -top-2 -right-2 h-8 w-8 rounded-full"
                          onClick={onRemovePhoto}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                      <p className="text-sm text-muted-foreground break-all font-medium">Imagen seleccionada</p>
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
                <Label htmlFor="description-modal" className="text-base font-semibold text-foreground">
                  Descripción (opcional)
                </Label>
                <Textarea
                  id="description-modal"
                  placeholder="Agrega un comentario adicional..."
                  value={description}
                  onChange={(e) => onDescriptionChange(e.target.value)}
                  rows={2}
                  className="text-base rounded-xl border-2 border-border focus:border-primary focus:ring-2 focus:ring-primary/20"
                />
              </div>

              <div className="flex flex-col sm:flex-row gap-3 pt-2">
                <Button
                  type="submit"
                  disabled={loading || !location || !name.trim()}
                  className="flex-1 h-12 text-base font-semibold rounded-xl bg-primary text-primary-foreground hover:bg-primary/90"
                >
                  <Send className="mr-2 h-5 w-5" />
                  {loading ? "Enviando..." : "Reportar incidente"}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={onClose}
                  className="h-12 text-base rounded-xl"
                >
                  Cancelar
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}