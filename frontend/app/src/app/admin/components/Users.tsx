"use client";

import { useState } from "react";
import { Search, Plus, Eye, Trash2 } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Modal } from "../shared/Modal";

// Datos de ejemplo
const mockUsers = [
  {
    id: 1,
    name: "Juan Pérez",
    email: "juan.perez@example.com",
    department: "Antioquia",
    municipality: "Medellín",
  },
  {
    id: 2,
    name: "María García",
    email: "maria.garcia@example.com",
    department: "Cundinamarca",
    municipality: "Bogotá",
  },
  {
    id: 3,
    name: "Carlos Rodríguez",
    email: "carlos.rodriguez@example.com",
    department: "Valle del Cauca",
    municipality: "Cali",
  },
  {
    id: 4,
    name: "Ana Martínez",
    email: "ana.martinez@example.com",
    department: "Atlántico",
    municipality: "Barranquilla",
  },
  {
    id: 5,
    name: "Pedro López",
    email: "pedro.lopez@example.com",
    department: "Santander",
    municipality: "Bucaramanga",
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

export function Users() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<typeof mockUsers[0] | null>(null);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    password: "",
    department: "",
    municipality: "",
  });

  const filteredUsers = mockUsers.filter(
    (user) =>
      user.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.department.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.municipality.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Nuevo usuario:", formData);
    // Aquí iría la lógica para crear el usuario
    setIsModalOpen(false);
    setFormData({
      name: "",
      email: "",
      password: "",
      department: "",
      municipality: "",
    });
  };

  const handleViewUser = (user: typeof mockUsers[0]) => {
    setSelectedUser(user);
    setIsViewModalOpen(true);
  };

  return (
    <>
      {/* Modal para ver detalles de usuario */}
      <Modal
        isOpen={isViewModalOpen}
        onClose={() => {
          setIsViewModalOpen(false);
          setSelectedUser(null);
        }}
        title="Detalles del Usuario"
      >
        {selectedUser && (
          <div className="space-y-4">
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Nombre completo
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedUser.name}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Correo electrónico
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedUser.email}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Departamento
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedUser.department}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Municipio
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedUser.municipality}
                </p>
              </div>
            </div>

            <div className="flex justify-end gap-3 pt-4 border-t">
              <button
                onClick={() => {
                  setIsViewModalOpen(false);
                  setSelectedUser(null);
                }}
                className="rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-accent"
              >
                Cerrar
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Modal para agregar usuario */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Agregar Usuario"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Nombre completo
            </label>
            <Input
              type="text"
              placeholder="Ingrese el nombre completo"
              value={formData.name}
              onChange={(e) =>
                setFormData({ ...formData, name: e.target.value })
              }
              required
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Correo electrónico
            </label>
            <Input
              type="email"
              placeholder="usuario@example.com"
              value={formData.email}
              onChange={(e) =>
                setFormData({ ...formData, email: e.target.value })
              }
              required
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Contraseña
            </label>
            <Input
              type="password"
              placeholder="Ingrese la contraseña"
              value={formData.password}
              onChange={(e) =>
                setFormData({ ...formData, password: e.target.value })
              }
              required
              minLength={6}
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Departamento
            </label>
            <select
              value={formData.department}
              onChange={(e) =>
                setFormData({ ...formData, department: e.target.value })
              }
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              required
            >
              <option value="">Seleccione un departamento</option>
              {departments.map((dept) => (
                <option key={dept} value={dept}>
                  {dept}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Municipio
            </label>
            <Input
              type="text"
              placeholder="Ingrese el municipio"
              value={formData.municipality}
              onChange={(e) =>
                setFormData({ ...formData, municipality: e.target.value })
              }
              required
            />
          </div>

          <div className="flex gap-3 pt-4">
            <button
              type="button"
              onClick={() => setIsModalOpen(false)}
              className="flex-1 rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-gray-200 transition-all cursor-pointer"
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="flex-1 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer"
            >
              Crear Usuario
            </button>
          </div>
        </form>
      </Modal>

      <div className="space-y-6">
        {/* Header */}
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">
              Usuarios
            </h1>
            <p className="text-muted-foreground">
              Gestión de usuarios del sistema
            </p>
          </div>
          <button
            onClick={() => setIsModalOpen(true)}
            className="inline-flex items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 cursor-pointer"
          >
            <Plus className="h-4 w-4" />
            Agregar Usuario
          </button>
        </div>

        {/* Search */}
        <div className="rounded-lg border border-border bg-card p-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              type="text"
              placeholder="Buscar usuarios..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-9"
            />
          </div>
        </div>

        {/* Table */}
        <div className="rounded-lg border border-border bg-card shadow-sm">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="border-b border-border bg-muted/50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                    Nombre
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                    Correo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                    Departamento
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                    Municipio
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-muted-foreground">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {filteredUsers.length > 0 ? (
                  filteredUsers.map((user) => (
                    <tr
                      key={user.id}
                      className="transition-colors hover:bg-muted/50"
                    >
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm font-medium text-card-foreground">
                          {user.name}
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm text-muted-foreground">
                          {user.email}
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm text-card-foreground">
                          {user.department}
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm text-card-foreground">
                          {user.municipality}
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="flex items-center gap-2">
                          <button
                            onClick={() => handleViewUser(user)}
                            className="bg-blue-100 text-blue-700 inline-flex items-center justify-center gap-2 rounded-md px-3 py-1.5 text-sm font-medium hover:bg-primary hover:text-accent-foreground transition-all cursor-pointer"
                          >
                            <Eye className="h-4 w-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={5} className="px-6 py-8 text-center">
                      <p className="text-sm text-muted-foreground">
                        No se encontraron usuarios
                      </p>
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </>
  );
}
