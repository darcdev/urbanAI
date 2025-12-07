"use client";

import { useState, useEffect } from "react";
import { Search, Plus, Eye } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Modal } from "../../../shared/Modal";
import { geographyService, leadersService, Department, Municipality, Leader } from "@/lib/api";

export function Users() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<Leader | null>(null);
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    department: "",
    departmentId: "",
    departmentDaneCode: "",
    municipality: "",
    municipalityId: "",
    latitude: 0,
    longitude: 0,
  });

  const [leaders, setLeaders] = useState<Leader[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [municipalities, setMunicipalities] = useState<Municipality[]>([]);
  const [loadingLeaders, setLoadingLeaders] = useState(false);
  const [loadingDepartments, setLoadingDepartments] = useState(false);
  const [loadingMunicipalities, setLoadingMunicipalities] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadDepartments();
    loadLeaders();
  }, []);

  useEffect(() => {
    if (formData.departmentDaneCode) {
      loadMunicipalities(formData.departmentDaneCode);
    } else {
      setMunicipalities([]);
    }
  }, [formData.departmentDaneCode]);

  const loadLeaders = async () => {
    try {
      setLoadingLeaders(true);
      const data = await leadersService.getAll();
      setLeaders(data);
    } catch {
      setLeaders([]);
    } finally {
      setLoadingLeaders(false);
    }
  };

  const loadDepartments = async () => {
    try {
      setLoadingDepartments(true);
      const data = await geographyService.getDepartments();
      setDepartments(data);
    } catch {
      setDepartments([]);
    } finally {
      setLoadingDepartments(false);
    }
  };

  const loadMunicipalities = async (departmentDaneCode: string) => {
    try {
      setLoadingMunicipalities(true);
      const data = await geographyService.getMunicipalities(departmentDaneCode);
      setMunicipalities(data);
    } catch {
      setMunicipalities([]);
    } finally {
      setLoadingMunicipalities(false);
    }
  };

  const filteredUsers = leaders.filter(
    (leader) =>
      `${leader.firstName} ${leader.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
      leader.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      setSubmitting(true);
      
      await leadersService.create({
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        password: formData.password,
        departmentId: formData.departmentId,
        municipalityId: formData.municipalityId,
        latitude: formData.latitude,
        longitude: formData.longitude,
      });
      await loadLeaders();
      
      setIsModalOpen(false);
      setFormData({
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        department: "",
        departmentId: "",
        departmentDaneCode: "",
        municipality: "",
        municipalityId: "",
        latitude: 0,
        longitude: 0,
      });
    } finally {
      setSubmitting(false);
    }
  };

  const handleViewUser = (leader: Leader) => {
    setSelectedUser(leader);
    setIsViewModalOpen(true);
  };

  return (
    <>
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
                  {selectedUser.firstName} {selectedUser.lastName}
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
                  ID Departamento
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedUser.departmentId}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  ID Municipio
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedUser.municipalityId}
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

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Agregar Usuario"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Nombre
            </label>
            <Input
              type="text"
              placeholder="Ingrese el nombre"
              value={formData.firstName}
              onChange={(e) =>
                setFormData({ ...formData, firstName: e.target.value })
              }
              required
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Apellido
            </label>
            <Input
              type="text"
              placeholder="Ingrese el apellido"
              value={formData.lastName}
              onChange={(e) =>
                setFormData({ ...formData, lastName: e.target.value })
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
              value={formData.departmentDaneCode}
              onChange={(e) => {
                const selectedDept = departments.find(d => d.departmentDaneCode === e.target.value);
                setFormData({ 
                  ...formData, 
                  departmentDaneCode: e.target.value,
                  departmentId: selectedDept?.id || "",
                  department: selectedDept?.name || "",
                  municipality: "",
                  municipalityId: "",
                  latitude: 0,
                  longitude: 0,
                });
              }}
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              required
              disabled={loadingDepartments}
            >
              <option value="">
                {loadingDepartments ? "Cargando departamentos..." : "Seleccione un departamento"}
              </option>
              {departments.map((dept) => (
                <option key={dept.id} value={dept.departmentDaneCode}>
                  {dept.name}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Municipio
            </label>
            <select
              value={formData.municipalityId}
              onChange={(e) => {
                const selectedMun = municipalities.find(m => m.id === e.target.value);
                setFormData({ 
                  ...formData, 
                  municipalityId: e.target.value,
                  municipality: selectedMun?.name || "",
                  latitude: selectedMun?.latitude || 0,
                  longitude: selectedMun?.longitude || 0,
                });
              }}
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              required
              disabled={!formData.departmentDaneCode || loadingMunicipalities}
            >
              <option value="">
                {!formData.departmentDaneCode 
                  ? "Primero seleccione un departamento" 
                  : loadingMunicipalities 
                  ? "Cargando municipios..." 
                  : "Seleccione un municipio"}
              </option>
              {municipalities.map((mun) => (
                <option key={mun.id} value={mun.id}>
                  {mun.name}
                </option>
              ))}
            </select>
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
              disabled={submitting}
              className="flex-1 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {submitting ? "Creando..." : "Crear Líder"}
            </button>
          </div>
        </form>
      </Modal>

      <div className="space-y-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-foreground">
              Líderes Comunitarios
            </h1>
            <p className="text-muted-foreground">
              Gestión de líderes del sistema
            </p>
          </div>
          <button
            onClick={() => setIsModalOpen(true)}
            className="inline-flex items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 cursor-pointer"
          >
            <Plus className="h-4 w-4" />
            Agregar Líder
          </button>
        </div>

        <div className="rounded-lg border border-border bg-card p-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              type="text"
              placeholder="Buscar líderes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-9"
            />
          </div>
        </div>

        {loadingLeaders && (
          <div className="text-center py-8">
            <p className="text-muted-foreground">Cargando líderes...</p>
          </div>
        )}

        {!loadingLeaders && (
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
                          {user.firstName} {user.lastName}
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm text-muted-foreground">
                          {user.email}
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm text-card-foreground">
                          {user.departmentId.substring(0, 8)}...
                        </div>
                      </td>
                      <td className="whitespace-nowrap px-6 py-4">
                        <div className="text-sm text-card-foreground">
                          {user.municipalityId.substring(0, 8)}...
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
                        No se encontraron líderes
                      </p>
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
        )}
      </div>
    </>
  );
}
