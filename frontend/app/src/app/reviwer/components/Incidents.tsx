"use client";

import { useState } from "react";
import {
  MapPin,
  Calendar,
  Filter,
  Search,
  ChevronLeft,
  ChevronRight,
  Eye,
  User,
  Clock,
} from "lucide-react";
import { Input } from "@/components/ui/input";
import { Modal } from "../../../shared/Modal";

const allIncidents = [
  {
    id: 1,
    title: "Bache en Av. Principal",
    location: "Av. Principal #123",
    status: "pending",
    date: "2024-12-06",
    priority: "high",
    category: "Vías",
    description: "Bache profundo que representa peligro para vehículos",
    image: "https://images.unsplash.com/photo-1625047509168-a7026f36de04?w=800&q=80",
    reporter: "Juan Pérez",
    reportedAt: "08:30 AM",
  },
  {
    id: 2,
    title: "Semáforo dañado",
    location: "Calle 45 con 67",
    status: "in-progress",
    date: "2024-12-05",
    priority: "critical",
    category: "Señalización",
    description: "Semáforo sin funcionar en intersección principal",
    image: "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800&q=80",
    reporter: "María García",
    reportedAt: "2024-12-05 02:15 PM",
  },
  {
    id: 3,
    title: "Basura acumulada",
    location: "Parque Central",
    status: "resolved",
    date: "2024-12-04",
    priority: "medium",
    category: "Limpieza",
    description: "Acumulación de basura en área recreativa",
    image: "https://images.unsplash.com/photo-1530587191325-3db32d826c18?w=800&q=80",
    reporter: "Carlos Rodríguez",
    reportedAt: "2024-12-04 10:00 AM",
  },
  {
    id: 4,
    title: "Alumbrado público",
    location: "Calle 12 #34-56",
    status: "pending",
    date: "2024-12-03",
    priority: "low",
    category: "Servicios",
    description: "Postes de luz sin funcionar en vía principal",
    image: "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80",
    reporter: "Ana Martínez",
    reportedAt: "2024-12-03 07:45 PM",
  },
  {
    id: 5,
    title: "Árbol caído",
    location: "Carrera 8 #23-45",
    status: "in-progress",
    date: "2024-12-02",
    priority: "critical",
    category: "Emergencia",
    description: "Árbol obstruyendo vía vehicular",
    image: "https://images.unsplash.com/photo-1611273426858-450d8e3c9fce?w=800&q=80",
    reporter: "Pedro López",
    reportedAt: "2024-12-02 06:20 AM",
  },
  {
    id: 6,
    title: "Fuga de agua",
    location: "Calle 67 #89-12",
    status: "pending",
    date: "2024-12-01",
    priority: "high",
    category: "Servicios",
    description: "Fuga considerable en tubería principal",
    image: "https://images.unsplash.com/photo-1584646098378-0874589d76b1?w=800&q=80",
    reporter: "Laura Sánchez",
    reportedAt: "2024-12-01 11:30 AM",
  },
  {
    id: 7,
    title: "Grafiti en paredes",
    location: "Edificio Municipal",
    status: "resolved",
    date: "2024-11-30",
    priority: "low",
    category: "Vandalismo",
    description: "Grafitis en fachada de edificio público",
    image: "https://images.unsplash.com/photo-1569163139394-de4798aa62b6?w=800&q=80",
    reporter: "Jorge Díaz",
    reportedAt: "2024-11-30 03:00 PM",
  },
  {
    id: 8,
    title: "Hundimiento de vía",
    location: "Av. Circunvalar Km 4",
    status: "in-progress",
    date: "2024-11-29",
    priority: "critical",
    category: "Vías",
    description: "Hundimiento significativo en calzada",
    image: "https://images.unsplash.com/photo-1589939705384-5185137a7f0f?w=800&q=80",
    reporter: "Sofía Ramírez",
    reportedAt: "2024-11-29 09:15 AM",
  },
  {
    id: 9,
    title: "Señal de tránsito vandalizada",
    location: "Calle 34 con 56",
    status: "pending",
    date: "2024-11-28",
    priority: "medium",
    category: "Señalización",
    description: "Señal de pare destruida",
    image: "https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=800&q=80",
    reporter: "Miguel Torres",
    reportedAt: "2024-11-28 04:30 PM",
  },
  {
    id: 10,
    title: "Alcantarilla tapada",
    location: "Barrio Los Pinos",
    status: "resolved",
    date: "2024-11-27",
    priority: "high",
    category: "Servicios",
    description: "Alcantarilla obstruida causando inundaciones",
    image: "https://images.unsplash.com/photo-1580674285054-bed31e145f59?w=800&q=80",
    reporter: "Carmen Flores",
    reportedAt: "2024-11-27 12:00 PM",
  },
  {
    id: 11,
    title: "Parque en mal estado",
    location: "Parque Las Flores",
    status: "pending",
    date: "2024-11-26",
    priority: "medium",
    category: "Espacios Públicos",
    description: "Juegos infantiles deteriorados",
    image: "https://images.unsplash.com/photo-1593510987459-7bd9ba92e6f2?w=800&q=80",
    reporter: "Roberto Méndez",
    reportedAt: "2024-11-26 10:45 AM",
  },
  {
    id: 12,
    title: "Cables sueltos",
    location: "Calle 90 #12-34",
    status: "in-progress",
    date: "2024-11-25",
    priority: "critical",
    category: "Emergencia",
    description: "Cables eléctricos colgando a baja altura",
    image: "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?w=800&q=80",
    reporter: "Patricia Vargas",
    reportedAt: "2024-11-25 08:00 AM",
  },
];

const statusConfig = {
  pending: { label: "Pendiente", color: "bg-red-200 text-red-800" },
  "in-progress": { label: "En Progreso", color: "bg-yellow-200 text-yellow-800" },
  resolved: { label: "Resuelto", color: "bg-green-200 text-green-800" },
};

const priorityConfig = {
  critical: { label: "Crítica", color: "bg-red-200 text-red-800" },
  high: { label: "Alta", color: "bg-yellow-200 text-yellow-800" },
  medium: { label: "Media", color: "bg-blue-200 text-blue-800" },
  low: { label: "Baja", color: "bg-green-200 text-green-800" },
};

type Incident = {
  id: number;
  title: string;
  location: string;
  status: string;
  date: string;
  priority: string;
  category: string;
  description: string;
  image: string;
  reporter: string;
  reportedAt: string;
};

export function Incidents() {
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");
  const [priorityFilter, setPriorityFilter] = useState<string>("all");
  const [selectedIncident, setSelectedIncident] = useState<Incident | null>(null);
  const itemsPerPage = 10;

  // Filtrar incidentes
  const filteredIncidents = allIncidents.filter((incident) => {
    const matchesSearch =
      incident.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      incident.location.toLowerCase().includes(searchTerm.toLowerCase()) ||
      incident.category.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus =
      statusFilter === "all" || incident.status === statusFilter;

    const matchesPriority =
      priorityFilter === "all" || incident.priority === priorityFilter;

    return matchesSearch && matchesStatus && matchesPriority;
  });

  // Paginación
  const totalPages = Math.ceil(filteredIncidents.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentIncidents = filteredIncidents.slice(startIndex, endIndex);

  // Reset page cuando cambian los filtros
  const handleFilterChange = () => {
    setCurrentPage(1);
  };

  return (
    <>
      {/* Modal */}
      <Modal 
        isOpen={selectedIncident !== null}
        onClose={() => setSelectedIncident(null)}
        title="Detalles del Incidente"
      >
        {selectedIncident && (
          <div className="space-y-6">
            {/* Imagen */}
            <div className="rounded-lg overflow-hidden border border-border">
              <img
                src={selectedIncident.image}
                alt={selectedIncident.title}
                className="w-full h-64 object-cover"
              />
            </div>

            {/* Información Principal */}
            <div className="space-y-4">
              <div>
                <h3 className="text-xl font-semibold text-card-foreground">
                  {selectedIncident.title}
                </h3>
                <p className="text-muted-foreground mt-1">
                  {selectedIncident.description}
                </p>
              </div>

              {/* Grid de Información */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4 border-t border-border">
                <div className="flex items-start gap-3">
                  <MapPin className="h-5 w-5 text-muted-foreground mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Ubicación</p>
                    <p className="text-sm text-muted-foreground">{selectedIncident.location}</p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <Calendar className="h-5 w-5 text-muted-foreground mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Fecha</p>
                    <p className="text-sm text-muted-foreground">{selectedIncident.date}</p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <Clock className="h-5 w-5 text-muted-foreground mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Hora</p>
                    <p className="text-sm text-muted-foreground">{selectedIncident.reportedAt}</p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <div className="h-5 w-5 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Estado</p>
                    <span className={`inline-block px-2 py-1 text-xs font-medium rounded-md mt-1 ${
                      selectedIncident.status === 'pending' ? 'bg-red-200 text-red-800' :
                      selectedIncident.status === 'in-progress' ? 'bg-yellow-200 text-yellow-800' :
                      'bg-green-200 text-green-800'
                    }`}>
                      {selectedIncident.status === 'pending' ? 'Pendiente' :
                       selectedIncident.status === 'in-progress' ? 'En Progreso' :
                       'Resuelto'}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            {/* Botones de Acción */}
            <div className="flex gap-3 pt-4 border-t border-border">
              <button className="flex-1 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer">
                Asignar
              </button>
              <button className="flex-1 rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-gray-200 transition-all cursor-pointer">
                Cambiar Estado
              </button>
            </div>
          </div>
        )}
      </Modal>

      <div className="space-y-6">
        {/* Header */}
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">
            Incidentes
          </h1>
          <p className="text-muted-foreground">
            Gestión y seguimiento de todos los incidentes reportados
          </p>
        </div>
        {/* <button className="inline-flex items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90">
          <Download className="h-4 w-4" />
          Exportar
        </button> */}
      </div>

      {/* Filters Section */}
      <div className="rounded-lg border border-border bg-card p-4">
        <div className="grid gap-4 md:grid-cols-3">
          {/* Search */}
          <div className="relative md:col-span-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              type="text"
              placeholder="Buscar incidentes..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                handleFilterChange();
              }}
              className="pl-9"
            />
          </div>

          {/* Status Filter */}
          <div>
            <select
              value={statusFilter}
              onChange={(e) => {
                setStatusFilter(e.target.value);
                handleFilterChange();
              }}
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <option value="all">Todos los estados</option>
              <option value="pending">Pendientes</option>
              <option value="in-progress">En Progreso</option>
              <option value="resolved">Resueltos</option>
            </select>
          </div>

          {/* Priority Filter */}
          {/* <div>
            <select
              value={priorityFilter}
              onChange={(e) => {
                setPriorityFilter(e.target.value);
                handleFilterChange();
              }}
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <option value="all">Todas las prioridades</option>
              <option value="critical">Crítica</option>
              <option value="high">Alta</option>
              <option value="medium">Media</option>
              <option value="low">Baja</option>
            </select>
          </div> */}
        </div>

        {/* Results count */}
        <div className="mt-3 flex items-center gap-2 text-sm text-muted-foreground">
          <Filter className="h-4 w-4" />
          <span>
            Mostrando {currentIncidents.length} de {filteredIncidents.length}{" "}
            incidentes
          </span>
        </div>
      </div>

      {/* Table */}
      <div className="rounded-lg border border-border bg-card shadow-sm">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="border-b border-border bg-muted/50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Incidente
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Ubicación
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Categoría
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Estado
                </th>
                {/* <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Prioridad
                </th> */}
                <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Fecha
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Acciones
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-border">
              {currentIncidents.length > 0 ? (
                currentIncidents.map((incident) => (
                  <tr
                    key={incident.id}
                    className="transition-colors hover:bg-muted/50"
                  >
                    <td className="px-4 py-4">
                      <div className="max-w-xs">
                        <div className="text-sm font-medium text-card-foreground">
                          {incident.title}
                        </div>
                        <div className="text-xs text-muted-foreground line-clamp-1">
                          {incident.description}
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-4">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <MapPin className="h-4 w-4 shrink-0" />
                        <span className="line-clamp-1">{incident.location}</span>
                      </div>
                    </td>
                    <td className="whitespace-nowrap px-4 py-4">
                      <span className="text-sm text-card-foreground">
                        {incident.category}
                      </span>
                    </td>
                    <td className="whitespace-nowrap px-4 py-4">
                      <span
                        className={`inline-flex rounded-md px-2.5 py-0.5 text-xs font-medium ${
                          statusConfig[incident.status as keyof typeof statusConfig]
                            .color
                        }`}
                      >
                        {
                          statusConfig[incident.status as keyof typeof statusConfig]
                            .label
                        }
                      </span>
                    </td>
                    {/* <td className="whitespace-nowrap px-4 py-4">
                      <span
                        className={`inline-flex rounded-md px-2.5 py-0.5 text-xs font-medium ${
                          priorityConfig[
                            incident.priority as keyof typeof priorityConfig
                          ].color
                        }`}
                      >
                        {
                          priorityConfig[
                            incident.priority as keyof typeof priorityConfig
                          ].label
                        }
                      </span>
                    </td> */}
                    <td className="whitespace-nowrap px-4 py-4">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Calendar className="h-4 w-4" />
                        {incident.date}
                      </div>
                    </td>
                    <td className="whitespace-nowrap px-4 py-4">
                      <button 
                        onClick={() => setSelectedIncident(incident)}
                        className="bg-blue-100 text-blue-700 inline-flex items-center justify-center gap-2 rounded-md px-3 py-1.5 text-sm font-medium hover:bg-primary hover:text-accent-foreground transition-all cursor-pointer"
                      >
                        <Eye className="h-4 w-4" />
                        Ver
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={8} className="px-4 py-8 text-center">
                    <p className="text-sm text-muted-foreground">
                      No se encontraron incidentes con los filtros aplicados
                    </p>
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {filteredIncidents.length > 0 && (
          <div className="flex items-center justify-between border-t border-border px-4 py-3">
            <div className="text-sm text-muted-foreground">
              Página {currentPage} de {totalPages}
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
                disabled={currentPage === 1}
                className="inline-flex items-center justify-center gap-2 rounded-md border border-input bg-background px-3 py-1.5 text-sm font-medium hover:bg-accent hover:text-accent-foreground disabled:pointer-events-none disabled:opacity-50"
              >
                <ChevronLeft className="h-4 w-4" />
                Anterior
              </button>
              <button
                onClick={() =>
                  setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                }
                disabled={currentPage === totalPages}
                className="inline-flex items-center justify-center gap-2 rounded-md border border-input bg-background px-3 py-1.5 text-sm font-medium hover:bg-accent hover:text-accent-foreground disabled:pointer-events-none disabled:opacity-50"
              >
                Siguiente
                <ChevronRight className="h-4 w-4" />
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
    </>
  );
}
