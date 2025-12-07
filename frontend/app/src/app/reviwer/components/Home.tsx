"use client";

import { useState, useEffect } from "react";
import Image from "next/image";
import Swal from 'sweetalert2';
import { 
  AlertCircle, 
  CheckCircle, 
  Clock, 
  XCircle,
  MapPin,
  Calendar,
  Eye
} from "lucide-react";
import { Modal } from "../../../shared/Modal";
import { reviewerIncidentsService, ReviewerIncident } from "@/lib/api";

const stats = [
  {
    title: "Incidentes Totales",
    value: "0",
    trend: "up",
    icon: AlertCircle,
    color: "text-info",
    bgColor: "bg-info/10",
  },
  {
    title: "Pendientes",
    value: "0",
    trend: "down",
    icon: Clock,
    color: "text-warning",
    bgColor: "bg-warning/10",
  },
  {
    title: "Aceptados",
    value: "0",
    trend: "up",
    icon: CheckCircle,
    color: "text-success",
    bgColor: "bg-success/10",
  },
  {
    title: "Rechazados",
    value: "0",
    trend: "up",
    icon: XCircle,
    color: "text-destructive",
    bgColor: "bg-destructive/10",
  },
];

export function Home() {
  const [selectedIncident, setSelectedIncident] = useState<ReviewerIncident | null>(null);
  const [incidents, setIncidents] = useState<ReviewerIncident[]>([]);
  const [loading, setLoading] = useState(true);
  const [statsData, setStatsData] = useState(stats);

  useEffect(() => {
    loadIncidents();
  }, []);

  const loadIncidents = async () => {
    try {
      setLoading(true);
      const data = await reviewerIncidentsService.getMyIncidents();
      
      // Combinar todos los incidentes
      const allIncidents = [...data.pending, ...data.accepted, ...data.rejected];
      setIncidents(allIncidents);

      // Actualizar estadísticas
      const total = allIncidents.length;
      const pending = data.pending.length;
      const accepted = data.accepted.length;
      const rejected = data.rejected.length;

      setStatsData([
        { ...stats[0], value: total.toString() },
        { ...stats[1], value: pending.toString() },
        { ...stats[2], value: accepted.toString() },
        { ...stats[3], value: rejected.toString() },
      ]);
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

  return (
    <>
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
      <div className="space-y-2">
        <h1 className="text-3xl font-bold tracking-tight text-foreground">
          Dashboard
        </h1>
        <p className="text-muted-foreground">
          Resumen general de incidentes urbanos
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {statsData.map((stat) => {
          const Icon = stat.icon;
          return (
            <div
              key={stat.title}
              className="rounded-lg border border-border bg-card p-6 shadow-sm transition-shadow hover:shadow-md"
            >
              <div className="flex items-center justify-between">
                <div className="space-y-1">
                  <p className="text-sm font-medium text-muted-foreground">
                    {stat.title}
                  </p>
                  <p className="text-2xl font-bold text-card-foreground">
                    {stat.value}
                  </p>
                  <p
                    className={`text-xs font-medium ${
                      stat.trend === "up" ? "text-success" : "text-destructive"
                    }`}
                  >
                  </p>
                </div>
                <div className={`rounded-md p-3 ${stat.bgColor}`}>
                  <Icon className={`h-6 w-6 ${stat.color}`} />
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Recent Incidents Table */}
      <div className="rounded-lg border border-border bg-card shadow-sm">
        <div className="border-b border-border p-6">
          <h2 className="text-xl font-semibold text-card-foreground">
            Incidentes Recientes
          </h2>
          <p className="mt-1 text-sm text-muted-foreground">
            Últimos reportes registrados en el sistema
          </p>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="border-b border-border bg-muted/50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Incidente
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Ubicación
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Estado
                </th>
                {/* <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Prioridad
                </th> */}
                <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Fecha
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  Acciones
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-border">
              {loading ? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-muted-foreground">
                    Cargando incidentes...
                  </td>
                </tr>
              ) : incidents.length === 0 ? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-muted-foreground">
                    No hay incidentes asignados
                  </td>
                </tr>
              ) : (
                incidents.slice(0, 10).map((incident) => (
                  <tr
                    key={incident.id}
                    className="transition-colors hover:bg-muted/50"
                  >
                    <td className="whitespace-nowrap px-6 py-4">
                      <div className="text-sm font-medium text-card-foreground">
                        #{incident.radicateNumber}
                      </div>
                      <div className="text-xs text-muted-foreground line-clamp-1 mt-1">
                        {incident.category.name}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <MapPin className="h-4 w-4" />
                        {incident.latitude.toFixed(4)}, {incident.longitude.toFixed(4)}
                      </div>
                    </td>
                    <td className="whitespace-nowrap px-6 py-4">
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
                    <td className="whitespace-nowrap px-6 py-4">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Calendar className="h-4 w-4" />
                        {new Date(incident.createdAt).toLocaleDateString()}
                      </div>
                    </td>
                    <td className="whitespace-nowrap px-6 py-4">
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
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
    </>
  );
}
