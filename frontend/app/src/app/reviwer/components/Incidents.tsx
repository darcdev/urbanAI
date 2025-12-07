"use client";

import { useState, useEffect } from "react";
import Image from "next/image";
import Swal from 'sweetalert2';
import {
  MapPin,
  Calendar,
  Filter,
  Search,
  ChevronLeft,
  ChevronRight,
  Eye,
  Clock,
} from "lucide-react";
import { Input } from "@/components/ui/input";
import { Modal } from "../../../shared/Modal";
import { reviewerIncidentsService, ReviewerIncident } from "@/lib/api";

export function Incidents() {
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");
  const [selectedIncident, setSelectedIncident] = useState<ReviewerIncident | null>(null);
  const [incidents, setIncidents] = useState<ReviewerIncident[]>([]);
  const [loading, setLoading] = useState(true);
  const itemsPerPage = 10;

  useEffect(() => {
    loadIncidents();
  }, []);

  const loadIncidents = async () => {
    try {
      setLoading(true);
      const data = await reviewerIncidentsService.getMyIncidents();
      const allIncidents = [...data.pending, ...data.accepted, ...data.rejected];
      setIncidents(allIncidents);
    } catch (error) {
      console.error('Error loading incidents:', error);
      setIncidents([]);
    } finally {
      setLoading(false);
    }
  };

  const handleAccept = async () => {
    if (!selectedIncident) return;

    const { value: priority } = await Swal.fire({
      title: 'Seleccionar Prioridad',
      input: 'select',
      inputOptions: {
        'Low': 'Baja',
        'Medium': 'Media',
        'High': 'Alta'
      },
      inputPlaceholder: 'Selecciona una prioridad',
      showCancelButton: true,
      confirmButtonText: 'Aceptar',
      cancelButtonText: 'Cancelar',
      inputValidator: (value) => {
        if (!value) {
          return 'Debes seleccionar una prioridad';
        }
      }
    });

    if (priority) {
      try {
        await reviewerIncidentsService.acceptIncident(selectedIncident.id, priority as 'Low' | 'Medium' | 'High' | 'Critical');
        await Swal.fire({
          icon: 'success',
          title: '¡Éxito!',
          text: 'Incidente aceptado exitosamente',
        });
        setSelectedIncident(null);
        loadIncidents();
      } catch (error) {
        await Swal.fire({
          icon: 'error',
          title: 'Error',
          text: 'No se pudo aceptar el incidente',
        });
      }
    }
  };

  const handleReject = async () => {
    if (!selectedIncident) return;

    const result = await Swal.fire({
      title: '¿Estás seguro?',
      text: '¿Deseas rechazar este incidente?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sí, rechazar',
      cancelButtonText: 'Cancelar'
    });

    if (result.isConfirmed) {
      try {
        await reviewerIncidentsService.rejectIncident(selectedIncident.id);
        await Swal.fire({
          icon: 'success',
          title: '¡Éxito!',
          text: 'Incidente rechazado exitosamente',
        });
        setSelectedIncident(null);
        loadIncidents();
      } catch (error) {
        await Swal.fire({
          icon: 'error',
          title: 'Error',
          text: 'No se pudo rechazar el incidente',
        });
      }
    }
  };

  // Filtrar incidentes
  const filteredIncidents = incidents.filter((incident) => {
    const matchesSearch =
      incident.radicateNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
      incident.aiDescription.toLowerCase().includes(searchTerm.toLowerCase()) ||
      incident.category.name.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus =
      statusFilter === "all" || incident.status.toLowerCase() === statusFilter;

    return matchesSearch && matchesStatus;
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
            <div className="rounded-lg overflow-hidden border border-border relative h-64">
              <Image
                src={selectedIncident.imageUrl}
                alt={`Incidente #${selectedIncident.radicateNumber}`}
                fill
                className="object-cover"
              />
            </div>

            {/* Información Principal */}
            <div className="space-y-4">
              <div>
                <h3 className="text-xl font-semibold text-card-foreground">
                  Incidente #{selectedIncident.radicateNumber}
                </h3>
                <p className="text-muted-foreground mt-1">
                  {selectedIncident.aiDescription}
                </p>
                {selectedIncident.additionalComment && (
                  <p className="text-sm text-muted-foreground mt-2 italic">
                    Comentario: {selectedIncident.additionalComment}
                  </p>
                )}
              </div>

              {/* Grid de Información */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4 border-t border-border">
                <div className="flex items-start gap-3">
                  <MapPin className="h-5 w-5 text-muted-foreground mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Ubicación</p>
                    <p className="text-sm text-muted-foreground">
                      {selectedIncident.latitude.toFixed(6)}, {selectedIncident.longitude.toFixed(6)}
                    </p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <Calendar className="h-5 w-5 text-muted-foreground mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Fecha</p>
                    <p className="text-sm text-muted-foreground">
                      {new Date(selectedIncident.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <Clock className="h-5 w-5 text-muted-foreground mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Hora</p>
                    <p className="text-sm text-muted-foreground">
                      {new Date(selectedIncident.createdAt).toLocaleTimeString()}
                    </p>
                  </div>
                </div>

                <div className="flex items-start gap-3">
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Categoría</p>
                    <p className="text-sm text-muted-foreground">
                      {selectedIncident.category.name} - {selectedIncident.subcategory.name}
                    </p>
                  </div>
                </div>

                {selectedIncident.status === 'Accepted' && (
                  <div className="flex items-start gap-3">
                    <div>
                      <p className="text-sm font-medium text-card-foreground">Prioridad</p>
                      <span className={`inline-block px-2 py-1 text-xs font-medium rounded-md mt-1 ${
                        selectedIncident.priority === 'High' ? 'bg-red-100 text-red-800' :
                        selectedIncident.priority === 'Medium' ? 'bg-orange-100 text-orange-800' :
                        'bg-blue-100 text-blue-800'
                      }`}>
                        {selectedIncident.priority === 'High' ? 'Alta' :
                        selectedIncident.priority === 'Medium' ? 'Media' :
                        'Baja'}
                      </span>
                    </div>
                  </div>
                )}

                <div className="flex items-start gap-3">
                  <div className="h-5 w-5 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Estado</p>
                    <span className={`inline-block px-2 py-1 text-xs font-medium rounded-md mt-1 ${
                      selectedIncident.status === 'Pending' ? 'bg-yellow-200 text-yellow-800' :
                      selectedIncident.status === 'Accepted' ? 'bg-green-200 text-green-800' :
                      'bg-red-200 text-red-800'
                    }`}>
                      {selectedIncident.status === 'Pending' ? 'Pendiente' :
                       selectedIncident.status === 'Accepted' ? 'Aceptado' :
                       'Rechazado'}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            {/* Botones de Acción */}
            {selectedIncident.status === 'Pending' && (
              <div className="flex gap-3 pt-4 border-t border-border justify-end">
                <button 
                  onClick={handleAccept}
                  className="rounded-md bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700 transition-all cursor-pointer"
                >
                  Aceptar
                </button>
                <button 
                  onClick={handleReject}
                  className="rounded-md bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 transition-all cursor-pointer"
                >
                  Rechazar
                </button>
              </div>
            )}
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
              <option value="accepted">Aceptados</option>
              <option value="rejected">Rechazados</option>
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
              {loading ? (
                <tr>
                  <td colSpan={6} className="px-4 py-8 text-center">
                    <p className="text-sm text-muted-foreground">
                      Cargando incidentes...
                    </p>
                  </td>
                </tr>
              ) : currentIncidents.length > 0 ? (
                currentIncidents.map((incident) => (
                  <tr
                    key={incident.id}
                    className="transition-colors hover:bg-muted/50"
                  >
                    <td className="px-4 py-4">
                      <div className="max-w-xs">
                        <div className="text-sm font-medium text-card-foreground">
                          #{incident.radicateNumber}
                        </div>
                        <div className="text-xs text-muted-foreground line-clamp-1">
                          {incident.aiDescription}
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-4">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <MapPin className="h-4 w-4 shrink-0" />
                        <span className="line-clamp-1">
                          {incident.latitude.toFixed(4)}, {incident.longitude.toFixed(4)}
                        </span>
                      </div>
                    </td>
                    <td className="whitespace-nowrap px-4 py-4">
                      <span className="text-sm text-card-foreground">
                        {incident.category.name}
                      </span>
                    </td>
                    <td className="whitespace-nowrap px-4 py-4">
                      <span
                        className={`inline-flex rounded-md px-2.5 py-0.5 text-xs font-medium ${
                          incident.status === 'Pending' ? 'bg-yellow-200 text-yellow-800' :
                          incident.status === 'Accepted' ? 'bg-green-200 text-green-800' :
                          'bg-red-200 text-red-800'
                        }`}
                      >
                        {incident.status === 'Pending' ? 'Pendiente' :
                         incident.status === 'Accepted' ? 'Aceptado' :
                         'Rechazado'}
                      </span>
                    </td>
                    <td className="whitespace-nowrap px-4 py-4">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Calendar className="h-4 w-4" />
                        {new Date(incident.createdAt).toLocaleDateString()}
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
                  <td colSpan={6} className="px-4 py-8 text-center">
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
