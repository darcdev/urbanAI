"use client";

import { useState, useMemo, useCallback } from "react";
import dynamic from "next/dynamic";
import { Search, Calendar } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Modal } from "../shared/Modal";

// Type for HeatMap props
type HeatMapProps = {
  incidents: Array<{
    id: number;
    title: string;
    description: string;
    department: string;
    municipality: string;
    date: string;
    priority: string;
    status: string;
    coordinates: { lat: number; lng: number };
  }>;
  onBoundsChange?: (visibleIncidentIds: number[]) => void;
};

// Importar el mapa dinámicamente para evitar SSR issues  
const HeatMap = dynamic<HeatMapProps>(
  () => import("@/app/entity/components/HeatMap"),
  { ssr: false }
);

// Datos de ejemplo de incidentes
const mockIncidents = [
  {
    id: 1,
    title: "Bache en vía principal",
    description: "Bache profundo que representa peligro para vehículos",
    department: "Antioquia",
    municipality: "Medellín",
    date: "2024-12-06",
    priority: "high",
    status: "pending",
    coordinates: { lat: 6.2476, lng: -75.5658 },
  },
  {
    id: 2,
    title: "Semáforo dañado",
    description: "Semáforo sin funcionamiento en intersección importante",
    department: "Cundinamarca",
    municipality: "Bogotá",
    date: "2024-12-05",
    priority: "critical",
    status: "in-progress",
    coordinates: { lat: 4.7110, lng: -74.0721 },
  },
  {
    id: 3,
    title: "Basuras en esquina",
    description: "Acumulación de basuras sin recoger",
    department: "Valle del Cauca",
    municipality: "Cali",
    date: "2024-12-04",
    priority: "medium",
    status: "pending",
    coordinates: { lat: 3.4516, lng: -76.5320 },
  },
  {
    id: 4,
    title: "Alumbrado público dañado",
    description: "Postes de luz sin funcionamiento en zona residencial",
    department: "Atlántico",
    municipality: "Barranquilla",
    date: "2024-12-03",
    priority: "high",
    status: "pending",
    coordinates: { lat: 10.9685, lng: -74.7813 },
  },
  {
    id: 5,
    title: "Fuga de agua",
    description: "Fuga importante en tubería principal",
    department: "Santander",
    municipality: "Bucaramanga",
    date: "2024-12-06",
    priority: "critical",
    status: "resolved",
    coordinates: { lat: 7.1193, lng: -73.1227 },
  },
  {
    id: 6,
    title: "Árbol caído",
    description: "Árbol obstruye vía vehicular",
    department: "Antioquia",
    municipality: "Medellín",
    date: "2024-12-05",
    priority: "high",
    status: "in-progress",
    coordinates: { lat: 6.2500, lng: -75.5700 },
  },
  {
    id: 7,
    title: "Andén deteriorado",
    description: "Andén peatonal en mal estado",
    department: "Cundinamarca",
    municipality: "Bogotá",
    date: "2024-12-04",
    priority: "medium",
    status: "pending",
    coordinates: { lat: 4.7150, lng: -74.0750 },
  },
  {
    id: 8,
    title: "Grafiti en muro público",
    description: "Grafiti vandálico en edificio municipal",
    department: "Antioquia",
    municipality: "Medellín",
    date: "2024-12-03",
    priority: "low",
    status: "pending",
    coordinates: { lat: 6.2450, lng: -75.5600 },
  },
  {
    id: 9,
    title: "Alcantarilla sin tapa",
    description: "Alcantarilla destapada representa peligro",
    department: "Valle del Cauca",
    municipality: "Cali",
    date: "2024-12-06",
    priority: "critical",
    status: "pending",
    coordinates: { lat: 3.4500, lng: -76.5350 },
  },
  {
    id: 10,
    title: "Contaminación sonora",
    description: "Establecimiento comercial con ruido excesivo",
    department: "Atlántico",
    municipality: "Barranquilla",
    date: "2024-12-05",
    priority: "medium",
    status: "in-progress",
    coordinates: { lat: 10.9700, lng: -74.7850 },
  },
  {
    id: 11,
    title: "Parque sin mantenimiento",
    description: "Parque público con falta de mantenimiento",
    department: "Bolívar",
    municipality: "Cartagena",
    date: "2024-12-04",
    priority: "low",
    status: "pending",
    coordinates: { lat: 10.3910, lng: -75.4794 },
  },
  {
    id: 12,
    title: "Señalización faltante",
    description: "Falta señalización vial en zona escolar",
    department: "Caldas",
    municipality: "Manizales",
    date: "2024-12-03",
    priority: "high",
    status: "pending",
    coordinates: { lat: 5.0689, lng: -75.5174 },
  },
];

const departments = [
  "Antioquia",
  "Cundinamarca",
  "Valle del Cauca",
  "Atlántico",
  "Santander",
  "Bolívar",
  "Caldas",
  "Quindío",
  "Risaralda",
  "Tolima",
];

export function Home() {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedDepartment, setSelectedDepartment] = useState("");
  const [selectedMunicipality, setSelectedMunicipality] = useState("");
  const [selectedDate, setSelectedDate] = useState("");
  const [selectedIncident, setSelectedIncident] = useState<typeof mockIncidents[0] | null>(null);
  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [visibleIncidentIds, setVisibleIncidentIds] = useState<number[]>([]);

  // useCallback para evitar recrear la función en cada render
  const handleBoundsChange = useCallback((ids: number[]) => {
    setVisibleIncidentIds(ids);
  }, []);

  // Calcular municipios cuando cambia el departamento usando useMemo
  const municipalities = useMemo(() => {
    if (selectedDepartment) {
      return mockIncidents
        .filter((inc) => inc.department === selectedDepartment)
        .map((inc) => inc.municipality)
        .filter((value, index, self) => self.indexOf(value) === index);
    }
    return [];
  }, [selectedDepartment]);

  // Filtrar incidentes por búsqueda, departamento, municipio y fecha (memoizado)
  const searchFilteredIncidents = useMemo(() => {
    return mockIncidents.filter((incident) => {
      const matchesSearch =
        searchTerm === "" ||
        incident.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        incident.description.toLowerCase().includes(searchTerm.toLowerCase());

      const matchesDepartment =
        selectedDepartment === "" || incident.department === selectedDepartment;

      const matchesMunicipality =
        selectedMunicipality === "" || incident.municipality === selectedMunicipality;

      const matchesDate = selectedDate === "" || incident.date === selectedDate;

      return matchesSearch && matchesDepartment && matchesMunicipality && matchesDate;
    });
  }, [searchTerm, selectedDepartment, selectedMunicipality, selectedDate]);

  // Filtrar adicionalmente por lo que está visible en el mapa
  const filteredIncidents = useMemo(() => {
    if (visibleIncidentIds.length === 0) {
      // Si no hay viewport definido, mostrar todos los filtrados
      return searchFilteredIncidents;
    }
    // Mostrar solo los que están visibles en el mapa Y cumplen los filtros
    return searchFilteredIncidents.filter(inc => visibleIncidentIds.includes(inc.id));
  }, [searchFilteredIncidents, visibleIncidentIds]);

  const handleViewIncident = (incident: typeof mockIncidents[0]) => {
    setSelectedIncident(incident);
    setIsViewModalOpen(true);
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
            <div className="space-y-2">
              <h3 className="text-lg font-semibold text-foreground">
                {selectedIncident.title}
              </h3>
              <p className="text-sm text-muted-foreground">
                {selectedIncident.description}
              </p>
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Departamento
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedIncident.department}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Municipio
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedIncident.municipality}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">Fecha</label>
                <p className="text-base font-medium text-foreground">
                  {selectedIncident.date}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">Estado</label>
                <span
                  className={`inline-block ml-2 px-2 py-1 text-xs font-medium rounded-md ${
                    selectedIncident.status === "pending"
                      ? "bg-red-200 text-red-800"
                      : selectedIncident.status === "in-progress"
                      ? "bg-yellow-200 text-yellow-800"
                      : "bg-green-200 text-green-800"
                  }`}
                >
                  {selectedIncident.status === "pending"
                    ? "Pendiente"
                    : selectedIncident.status === "in-progress"
                    ? "En Progreso"
                    : "Resuelto"}
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

        {/* Panel de filtros - Flotante arriba a la izquierda */}
        <div className="absolute top-3 left-14 right-4 md:right-auto md:w-96 z-[600]">
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
                value={selectedDepartment}
                onChange={(e) => {
                  setSelectedDepartment(e.target.value);
                  setSelectedMunicipality("");
                }}
                className="flex h-9 w-full rounded-md border border-input bg-background px-3 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              >
                <option value="">Todos los deptos.</option>
                {departments.map((dept) => (
                  <option key={dept} value={dept}>
                    {dept}
                  </option>
                ))}
              </select>

              {/* Municipio */}
              <select
                value={selectedMunicipality}
                onChange={(e) => setSelectedMunicipality(e.target.value)}
                className="flex h-9 w-full rounded-md border border-input bg-background px-3 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                disabled={!selectedDepartment}
              >
                <option value="">Todos los munic.</option>
                {municipalities.map((mun) => (
                  <option key={mun} value={mun}>
                    {mun}
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
              {filteredIncidents.length > 0 ? (
                <div className="divide-y divide-border">
                  {filteredIncidents.map((incident) => (
                    <div
                      key={incident.id}
                      className="p-4 hover:bg-muted/50 cursor-pointer transition-colors"
                      onClick={() => handleViewIncident(incident)}
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
