"use client";

import { useState } from "react";
import { 
  AlertCircle, 
  CheckCircle, 
  Clock, 
  TrendingUp,
  MapPin,
  Calendar,
  Eye,
  User
} from "lucide-react";
import { Modal } from "../../../shared/Modal";

const stats = [
  {
    title: "Incidentes Totales",
    value: "1,234",
    change: "+12.3%",
    trend: "up",
    icon: AlertCircle,
    color: "text-info",
    bgColor: "bg-info/10",
  },
  {
    title: "Pendientes",
    value: "342",
    change: "-5.2%",
    trend: "down",
    icon: Clock,
    color: "text-warning",
    bgColor: "bg-warning/10",
  },
  {
    title: "Resueltos",
    value: "892",
    change: "+18.1%",
    trend: "up",
    icon: CheckCircle,
    color: "text-success",
    bgColor: "bg-success/10",
  },
  {
    title: "En Progreso",
    value: "156",
    change: "+3.4%",
    trend: "up",
    icon: TrendingUp,
    color: "text-primary",
    bgColor: "bg-primary/10",
  },
];

const recentIncidents = [
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
    reportedAt: "2024-12-06 08:30 AM",
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
];

const statusConfig = {
  pending: { label: "Pendiente", color: "bg-red-200 text-red-800" },
  "in-progress": { label: "En Progreso", color: "bg-yellow-200 text-yellow-800" },
  resolved: { label: "Resuelto", color: "bg-green-200 text-green-800" },
};

const priorityConfig = {
  critical: { label: "Crítica", color: "bg-destructive text-destructive-foreground" },
  high: { label: "Alta", color: "bg-orange-500 text-white" },
  medium: { label: "Media", color: "bg-warning text-warning-foreground" },
  low: { label: "Baja", color: "bg-muted text-muted-foreground" },
};

export function Home() {
  const [selectedIncident, setSelectedIncident] = useState<typeof recentIncidents[0] | null>(null);

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
                  <div>
                    <p className="text-sm font-medium text-card-foreground">Categoria</p>
                    <p className="text-sm text-muted-foreground">{selectedIncident.category}</p>
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
            <div className="flex gap-3 pt-4 border-t border-border justify-end">
              <button className="rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer">
                Aceptar
              </button>
              <button className="rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-gray-200 transition-all cursor-pointer">
                Rechazar
              </button>
            </div>
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
        {stats.map((stat) => {
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
                    {stat.change} del mes anterior
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
              {recentIncidents.map((incident) => (
                <tr
                  key={incident.id}
                  className="transition-colors hover:bg-muted/50"
                >
                  <td className="whitespace-nowrap px-6 py-4">
                    <div className="text-sm font-medium text-card-foreground">
                      {incident.title}
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-2 text-sm text-muted-foreground">
                      <MapPin className="h-4 w-4" />
                      {incident.location}
                    </div>
                  </td>
                  <td className="whitespace-nowrap px-6 py-4">
                    <span
                      className={`inline-flex rounded-md px-2.5 py-0.5 text-xs font-medium ${
                        statusConfig[incident.status as keyof typeof statusConfig].color
                      }`}
                    >
                      {statusConfig[incident.status as keyof typeof statusConfig].label}
                    </span>
                  </td>
                  {/* <td className="whitespace-nowrap px-6 py-4">
                    <span
                      className={`inline-flex rounded-md px-2.5 py-0.5 text-xs font-medium ${
                        priorityConfig[incident.priority as keyof typeof priorityConfig].color
                      }`}
                    >
                      {priorityConfig[incident.priority as keyof typeof priorityConfig].label}
                    </span>
                  </td> */}
                  <td className="whitespace-nowrap px-6 py-4">
                    <div className="flex items-center gap-2 text-sm text-muted-foreground">
                      <Calendar className="h-4 w-4" />
                      {incident.date}
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
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
    </>
  );
}
