"use client";

import { useState, useMemo, useCallback, useEffect } from "react";
import dynamic from "next/dynamic";
import Image from "next/image";
import { geographyIncidentsService, GeographyIncident, geographyService, Department, Municipality } from "@/lib/api";
import { Search, Calendar, Bot } from "lucide-react";
import { Input } from "@/components/ui/input";
import { ChatbotWidget } from "./ChatbotWidget";
import { Modal } from "../../../shared/Modal";

// Type for HeatMap props
type HeatMapProps = {
  incidents: Array<{
    id: number | string;
    title: string;
    description: string;
    department: string;
    municipality: string;
    date: string;
    priority: string;
    status: string;
    coordinates: { lat: number; lng: number };
  }>;
  onBoundsChange?: (visibleIncidentIds: (number | string)[]) => void;
};

// Importar el mapa dinámicamente para evitar SSR issues  
const HeatMap = dynamic<HeatMapProps>(
  () => import("@/app/entity/components/HeatMap"),
  { ssr: false }
);

export function Home() {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedDepartment, setSelectedDepartment] = useState<Department | null>(null);
  const [selectedMunicipality, setSelectedMunicipality] = useState<Municipality | null>(null);
  const [selectedDate, setSelectedDate] = useState("");
  const [selectedIncident, setSelectedIncident] = useState<GeographyIncident | null>(null);
  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [visibleIncidentIds, setVisibleIncidentIds] = useState<(number | string)[]>([]);
  const [incidents, setIncidents] = useState<GeographyIncident[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [municipalities, setMunicipalities] = useState<Municipality[]>([]);
  const [loading, setLoading] = useState(true);
  const [isChatOpen, setIsChatOpen] = useState(false);

  // Cargar departamentos al montar
  useEffect(() => {
    loadDepartments();
  }, []);

  // Cargar municipios cuando cambia el departamento
  useEffect(() => {
    if (selectedDepartment) {
      loadMunicipalities(selectedDepartment.departmentDaneCode);
    } else {
      setMunicipalities([]);
      setSelectedMunicipality(null);
    }
  }, [selectedDepartment]);

  const loadDepartments = async () => {
    try {
      const data = await geographyService.getDepartments();
      setDepartments(data);
    } catch (error) {
      console.error('Error loading departments:', error);
      setDepartments([]);
    }
  };

  const loadMunicipalities = async (departmentDaneCode: string) => {
    try {
      const data = await geographyService.getMunicipalities(departmentDaneCode);
      setMunicipalities(data);
    } catch (error) {
      console.error('Error loading municipalities:', error);
      setMunicipalities([]);
    }
  };

  const loadIncidents = useCallback(async () => {
    try {
      setLoading(true);
      // Cargar todos los incidentes sin filtros (el filtrado se hará por coordenadas)
      const data = await geographyIncidentsService.getIncidentsByGeography();
      setIncidents(data);
    } catch (error) {
      console.error('Error loading incidents:', error);
      setIncidents([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadIncidents();
  }, [loadIncidents]);

  // useCallback para evitar recrear la función en cada render
  const handleBoundsChange = useCallback((ids: (number | string)[]) => {
    setVisibleIncidentIds(ids);
  }, []);

  // Función para calcular distancia entre dos coordenadas (fórmula de Haversine)
  const calculateDistance = (lat1: number, lon1: number, lat2: number, lon2: number): number => {
    const R = 6371; // Radio de la Tierra en km
    const dLat = (lat2 - lat1) * Math.PI / 180;
    const dLon = (lon2 - lon1) * Math.PI / 180;
    const a = 
      Math.sin(dLat/2) * Math.sin(dLat/2) +
      Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
      Math.sin(dLon/2) * Math.sin(dLon/2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
    return R * c;
  };

  // Transformar incidentes del API al formato del mapa
  const transformedIncidents = useMemo(() => {
    return incidents.map(incident => {
      // Encontrar el departamento y municipio más cercano basado en coordenadas
      let departmentName = 'N/A';
      let municipalityName = 'N/A';

      if (selectedDepartment) {
        departmentName = selectedDepartment.name;
        if (selectedMunicipality) {
          municipalityName = selectedMunicipality.name;
        }
      }

      return {
        id: incident.id,
        title: `#${incident.radicateNumber}`,
        description: incident.aiDescription,
        department: departmentName,
        municipality: municipalityName,
        date: new Date(incident.createdAt).toLocaleDateString(),
        priority: incident.priority.toLowerCase(),
        status: incident.status.toLowerCase(),
        coordinates: { lat: incident.latitude, lng: incident.longitude },
      };
    });
  }, [incidents, selectedDepartment, selectedMunicipality]);

  // Filtrar incidentes por búsqueda, coordenadas y fecha (memoizado)
  const searchFilteredIncidents = useMemo(() => {
    return transformedIncidents.filter((incident) => {
      const matchesSearch =
        searchTerm === "" ||
        incident.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        incident.description.toLowerCase().includes(searchTerm.toLowerCase());

      // Filtrar por área geográfica usando distancia a las coordenadas del departamento/municipio
      let matchesGeography = true;
      
      if (selectedMunicipality) {
        // Filtrar por proximidad al municipio (radio de ~50km)
        const distance = calculateDistance(
          incident.coordinates.lat,
          incident.coordinates.lng,
          selectedMunicipality.latitude,
          selectedMunicipality.longitude
        );
        matchesGeography = distance <= 50; // 50 km de radio
      } else if (selectedDepartment) {
        // Filtrar por proximidad al departamento (radio de ~200km)
        const distance = calculateDistance(
          incident.coordinates.lat,
          incident.coordinates.lng,
          selectedDepartment.latitude,
          selectedDepartment.longitude
        );
        matchesGeography = distance <= 200; // 200 km de radio
      }

      const matchesDate = selectedDate === "" || incident.date === selectedDate;

      return matchesSearch && matchesGeography && matchesDate;
    });
  }, [searchTerm, selectedDate, transformedIncidents, selectedDepartment, selectedMunicipality]);

  // Filtrar adicionalmente por lo que está visible en el mapa
  const filteredIncidents = useMemo(() => {
    if (visibleIncidentIds.length === 0) {
      // Si no hay viewport definido, mostrar todos los filtrados
      return searchFilteredIncidents;
    }
    // Mostrar solo los que están visibles en el mapa Y cumplen los filtros
    return searchFilteredIncidents.filter(inc => visibleIncidentIds.includes(inc.id));
  }, [searchFilteredIncidents, visibleIncidentIds]);

  const handleViewIncident = (incident: GeographyIncident) => {
    setSelectedIncident(incident);
    setIsViewModalOpen(true);
  };

  const handleIncidentClick = (transformedIncident: typeof transformedIncidents[0]) => {
    const originalIncident = incidents.find(inc => inc.id === transformedIncident.id);
    if (originalIncident) {
      handleViewIncident(originalIncident);
    }
  };

  return (
    <>
      {/* Modal para ver detalles de incidente */}
      <Modal
        isOpen={isViewModalOpen}
        onClose={() => {
          setIsViewModalOpen(false);
          setSelectedIncident(null);
        }}
        title="Detalles del Incidente"
      >
        {selectedIncident && (
          <div className="space-y-4">
            {/* Imagen */}
            <div className="rounded-lg overflow-hidden border border-border relative h-64">
              <Image
                src={selectedIncident.imageUrl}
                alt={`Incidente #${selectedIncident.radicateNumber}`}
                fill
                className="object-cover"
              />
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-semibold text-foreground">
                Incidente #{selectedIncident.radicateNumber}
              </h3>
              <p className="text-sm text-muted-foreground">
                {selectedIncident.aiDescription}
              </p>
              {selectedIncident.additionalComment && (
                <p className="text-xs text-muted-foreground italic">
                  Comentario: {selectedIncident.additionalComment}
                </p>
              )}
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Ubicación
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedIncident.latitude.toFixed(6)}, {selectedIncident.longitude.toFixed(6)}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Categoría
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedIncident.category.name}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">Subcategoría</label>
                <p className="text-base font-medium text-foreground">
                  {selectedIncident.subcategory.name}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">Fecha</label>
                <p className="text-base font-medium text-foreground">
                  {new Date(selectedIncident.createdAt).toLocaleDateString()}
                </p>
              </div>

              {selectedIncident.status === 'Accepted' && (
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground">Prioridad</label>
                  <span
                    className={`inline-block px-2 py-1 text-xs font-medium rounded-md ${
                      selectedIncident.priority === "High" || selectedIncident.priority === "Critical"
                        ? "bg-red-100 text-red-800"
                        : selectedIncident.priority === "Medium"
                        ? "bg-orange-100 text-orange-800"
                        : "bg-blue-100 text-blue-800"
                    }`}
                  >
                    {selectedIncident.priority}
                  </span>
                </div>
              )}


              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">Estado</label>
                <span
                  className={`inline-block px-2 py-1 text-xs font-medium rounded-md ${
                    selectedIncident.status === "Pending"
                      ? "bg-yellow-200 text-yellow-800"
                      : selectedIncident.status === "Accepted"
                      ? "bg-green-200 text-green-800"
                      : "bg-red-200 text-red-800"
                  }`}
                >
                  {selectedIncident.status}
                </span>
              </div>
            </div>

            <div className="flex justify-end gap-3 pt-4 border-t">
              <button
                onClick={() => {
                  setIsViewModalOpen(false);
                  setSelectedIncident(null);
                }}
                className="rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-gray-200 transition-all cursor-pointer"
              >
                Cerrar
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Contenedor estilo Google Maps: Mapa de fondo completo */}
      <div className="relative w-full h-full">
        {/* Mapa de calor - Ocupa toda la pantalla */}
        <div className="absolute inset-0">
          <HeatMap 
            incidents={searchFilteredIncidents} 
            onBoundsChange={handleBoundsChange}
          />
        </div>

        {/* Botón del asistente */}
        <div className="absolute bottom-4 left-14 z-[750]">
          <button
            onClick={() => setIsChatOpen((v) => !v)}
            aria-label={isChatOpen ? "Cerrar asistente" : "Abrir asistente"}
            className="inline-flex h-11 w-11 items-center justify-center rounded-md bg-primary text-primary-foreground shadow-elevated hover:opacity-90 transition"
          >
            <Bot className="h-4 w-4" />
          </button>
        </div>

        {/* Panel de filtros - Flotante arriba a la izquierda */}
        <div className="absolute top-0 left-14 right-4 md:right-auto md:w-96 z-[600]">
          <div className="bg-white rounded-lg shadow-xl border border-border p-4 space-y-4">
            {/* Búsqueda */}
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="text"
                placeholder="Buscar incidente..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-9"
              />
            </div>

            {/* Filtros en grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              {/* Departamento */}
              <select
                value={selectedDepartment?.id || ""}
                onChange={(e) => {
                  const dept = departments.find(d => d.id === e.target.value);
                  setSelectedDepartment(dept || null);
                  setSelectedMunicipality(null);
                }}
                className="flex h-9 w-full rounded-md border border-input bg-background px-3 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              >
                <option value="">Todos los deptos.</option>
                {departments.map((dept) => (
                  <option key={dept.id} value={dept.id}>
                    {dept.name}
                  </option>
                ))}
              </select>

              {/* Municipio */}
              <select
                value={selectedMunicipality?.id || ""}
                onChange={(e) => {
                  const mun = municipalities.find(m => m.id === e.target.value);
                  setSelectedMunicipality(mun || null);
                }}
                className="flex h-9 w-full rounded-md border border-input bg-background px-3 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                disabled={!selectedDepartment}
              >
                <option value="">Todos los munic.</option>
                {municipalities.map((mun) => (
                  <option key={mun.id} value={mun.id}>
                    {mun.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Fecha */}
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="date"
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                className="pl-9 h-9"
              />
            </div>

            {/* Contador */}
            <div className="text-xs text-muted-foreground border-t pt-3">
              <span className="font-semibold text-foreground">{filteredIncidents.length}</span> incidentes en el área visible
              {visibleIncidentIds.length > 0 && searchFilteredIncidents.length !== filteredIncidents.length && (
                <span className="ml-2 text-primary">
                  ({searchFilteredIncidents.length} total)
                </span>
              )}
            </div>
          </div>
        </div>

        {/* Chatbot flotante */}
        {isChatOpen && (
          <div className="absolute bottom-20 left-14 z-[760]">
            <ChatbotWidget />
          </div>
        )}

        {/* Panel lateral derecho - Lista de incidentes flotante (estilo Google Maps) */}
        <div className="absolute top-11 right-4 bottom-4 w-80 md:w-96 z-[600] hidden lg:block">
          <div className="h-full rounded-lg shadow-xl border border-border bg-white overflow-hidden flex flex-col">
            {/* Header del panel */}
            <div className="px-4 py-3 border-b border-border bg-muted/30">
              <h3 className="font-semibold text-foreground">Incidentes</h3>
              <p className="text-xs text-muted-foreground mt-0.5">
                {filteredIncidents.length} resultado{filteredIncidents.length !== 1 ? 's' : ''}
              </p>
            </div>

            {/* Lista de incidentes */}
            <div className="flex-1 overflow-y-auto">
              {loading ? (
                <div className="flex items-center justify-center h-full p-8">
                  <p className="text-sm text-muted-foreground">Cargando incidentes...</p>
                </div>
              ) : filteredIncidents.length > 0 ? (
                <div className="divide-y divide-border">
                  {filteredIncidents.map((incident) => (
                    <div
                      key={incident.id}
                      className="p-4 hover:bg-muted/50 cursor-pointer transition-colors"
                      onClick={() => handleIncidentClick(incident)}
                    >
                      <div className="space-y-2">
                        <h4 className="font-medium text-sm text-foreground line-clamp-1">
                          {incident.title}
                        </h4>
                        <p className="text-xs text-muted-foreground line-clamp-2">
                          {incident.description}
                        </p>
                        <div className="flex items-center gap-2 text-xs">
                          <span className="text-muted-foreground">
                            {incident.municipality}, {incident.department}
                          </span>
                        </div>
                        <div className="flex items-center gap-2">
                          <span
                            className={`inline-block px-2 py-1 text-xs font-medium rounded-md ${
                            incident.status === "pending"
                            ? "bg-red-200 text-red-800"
                            : incident.status === "in-progress"
                            ? "bg-yellow-200 text-yellow-800"
                            : "bg-green-200 text-green-800"
                            }`}
                            >
                            {incident.status === "pending"
                                ? "Pendiente"
                                : incident.status === "in-progress"
                                ? "En Progreso"
                                : "Resuelto"}
                          </span>
                          <span className="text-xs text-muted-foreground">
                            {incident.date}
                          </span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="flex items-center justify-center h-full p-8">
                  <p className="text-sm text-muted-foreground text-center">
                    No se encontraron incidentes en esta área
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
