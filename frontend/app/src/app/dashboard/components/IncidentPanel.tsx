import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Filter, ChevronDown, Search, Users } from "lucide-react"
import { categories } from "../constants"
import { Incident } from "../types"

interface IncidentPanelProps {
  filteredIncidents: Incident[]
  selectedCategory: string
  setSelectedCategory: (c: string) => void
  searchCode: string
  setSearchCode: (s: string) => void
  showFilters: boolean
  setShowFilters: (v: boolean) => void
}

export function IncidentPanel({
  filteredIncidents,
  selectedCategory,
  setSelectedCategory,
  searchCode,
  setSearchCode,
  showFilters,
  setShowFilters,
}: IncidentPanelProps) {
  return (
    <div className="w-full md:w-[40%] h-[40vh] md:h-full bg-background/80 backdrop-blur-sm border-l border-border/60">
      <Card className="h-full w-full rounded-none md:rounded-l-2xl shadow-2xl border-0">
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
            <div className="px-3 py-1 bg-primary/10 text-primary rounded-md text-sm font-semibold">
              {filteredIncidents.length} visibles
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-4 h-[calc(100%-64px)] flex flex-col">
          <div className="flex flex-col md:flex-row md:items-center gap-3">
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
                No hay incidentes para los filtros aplicados
              </div>
            ) : (
              <div className="divide-y max-h-full overflow-y-auto">
                {filteredIncidents.map((incident) => (
                  <div
                    key={incident.id}
                    className="grid grid-cols-12 items-center px-3 py-3 hover:bg-muted/50 transition-colors text-sm"
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
          </div>
        </CardContent>
      </Card>
    </div>
  )
}


