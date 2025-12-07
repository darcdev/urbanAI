"use client";

import { useState, useEffect } from "react";
import { Search, Plus, Eye, Edit } from "lucide-react";
import { Input } from "@/components/ui/input";
import Swal from 'sweetalert2';
import { Modal } from "../../../shared/Modal";
import { organizationsService, Organization } from "@/lib/api";

export function Organizations() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedOrganization, setSelectedOrganization] = useState<Organization | null>(null);
  const [editData, setEditData] = useState({
    firstName: "",
    lastName: "",
  });
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    organizationName: "",
  });

  const [organizations, setOrganizations] = useState<Organization[]>([]);
  const [loadingOrganizations, setLoadingOrganizations] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadOrganizations();
  }, []);

  const loadOrganizations = async () => {
    try {
      setLoadingOrganizations(true);
      const data = await organizationsService.getAll();
      setOrganizations(Array.isArray(data) ? data : []);
    } finally {
      setLoadingOrganizations(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.firstName.trim() || !formData.lastName.trim() || 
        !formData.email.trim() || !formData.password.trim() || 
        !formData.organizationName.trim()) {
      alert("Todos los campos son obligatorios");
      return;
    }

    try {
      setSubmitting(true);
      await organizationsService.create(formData);
      
      // Reload organizations
      await loadOrganizations();
      
      // Reset form
      setFormData({
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        organizationName: "",
      });
      setIsModalOpen(false);
      
      await Swal.fire({
        icon: 'success',
        title: '¡Éxito!',
        text: 'Organización creada exitosamente',
        confirmButtonColor: '#3085d6',
      });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "Error al crear la organización";
      await Swal.fire({
        icon: 'error',
        title: 'Error',
        text: errorMessage,
        confirmButtonColor: '#d33',
      });
    } finally {
      setSubmitting(false);
    }
  };

  const handleEdit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!selectedOrganization) return;
    
    try {
      setSubmitting(true);
      await organizationsService.update(selectedOrganization.id, {
        firstName: editData.firstName,
        lastName: editData.lastName,
      });
      
      await loadOrganizations();
      setIsEditModalOpen(false);
      setSelectedOrganization(null);
      setEditData({ firstName: "", lastName: "" });
      
      await Swal.fire({
        icon: 'success',
        title: '¡Éxito!',
        text: 'Organización actualizada exitosamente',
        confirmButtonColor: '#3085d6',
      });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "Error al actualizar la organización";
      await Swal.fire({
        icon: 'error',
        title: 'Error',
        text: errorMessage,
        confirmButtonColor: '#d33',
      });
    } finally {
      setSubmitting(false);
    }
  };

  const filteredOrganizations = organizations.filter(
    (org) =>
      org.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      org.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      org.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
      org.organizationName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Organizaciones</h1>
          <p className="text-muted-foreground mt-1">
            Gestiona las organizaciones del sistema
          </p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors cursor-pointer"
        >
          <Plus className="h-4 w-4" />
          Nueva Organización
        </button>
      </div>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-muted-foreground" />
        <Input
          type="text"
          placeholder="Buscar por nombre, apellido, email u organización..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="pl-10"
        />
      </div>

      <div className="bg-white rounded-lg border border-border overflow-hidden">
        {loadingOrganizations ? (
          <div className="p-8 text-center text-muted-foreground">
            Cargando organizaciones...
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-muted/50 border-b border-border">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Nombre
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Apellido
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Email
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Organización
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {filteredOrganizations.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-6 py-8 text-center text-muted-foreground">
                      {searchTerm ? "No se encontraron resultados" : "No hay organizaciones registradas"}
                    </td>
                  </tr>
                ) : (
                  filteredOrganizations.map((org) => (
                    <tr
                      key={org.id}
                      className="hover:bg-muted/30 transition-colors"
                    >
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-foreground">
                        {org.firstName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">
                        {org.lastName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">
                        {org.email}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">
                        {org.organizationName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <div className="flex items-center gap-2">
                          <button
                            onClick={() => {
                              setSelectedOrganization(org);
                              setEditData({
                                firstName: org.firstName,
                                lastName: org.lastName,
                              });
                              setIsEditModalOpen(true);
                            }}
                            className="bg-green-100 text-green-700 inline-flex items-center justify-center gap-2 rounded-md px-3 py-1.5 text-sm font-medium hover:bg-green-200 transition-all cursor-pointer"
                            title="Editar"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => {
                              setSelectedOrganization(org);
                              setIsViewModalOpen(true);
                            }}
                            className="bg-blue-100 text-blue-700 inline-flex items-center justify-center gap-2 rounded-md px-3 py-1.5 text-sm font-medium hover:bg-primary hover:text-accent-foreground transition-all cursor-pointer"
                            title="Ver detalles"
                          >
                            <Eye className="h-4 w-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Nueva Organización"
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
              Nombre de la Organización
            </label>
            <Input
              type="text"
              placeholder="Ingrese el nombre de la organización"
              value={formData.organizationName}
              onChange={(e) =>
                setFormData({ ...formData, organizationName: e.target.value })
              }
              required
            />
          </div>

          <div className="flex gap-3 pt-4 justify-end">
            <button
              type="submit"
              disabled={submitting}
              className="px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors disabled:opacity-50 cursor-pointer"
            >
              {submitting ? "Creando..." : "Crear Organización"}
            </button>
            <button
              type="button"
              onClick={() => setIsModalOpen(false)}
              className="px-4 py-2 bg-muted text-foreground rounded-lg hover:bg-gray-200 transition-colors cursor-pointer"
            >
              Cancelar
            </button>
          </div>
        </form>
      </Modal>

      <Modal
        isOpen={isViewModalOpen}
        onClose={() => {
          setIsViewModalOpen(false);
          setSelectedOrganization(null);
        }}
        title="Detalles de la Organización"
      >
        {selectedOrganization && (
          <div className="space-y-4">
                        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Nombre completo
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedOrganization.firstName} {selectedOrganization.lastName}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  Correo electrónico
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedOrganization.email}
                </p>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-muted-foreground">
                  ID Departamento
                </label>
                <p className="text-base font-medium text-foreground">
                  {selectedOrganization.organizationName}
                </p>
              </div>

            </div>
            <div className="flex justify-end gap-3 pt-4 border-t">
              <button
                onClick={() => {
                  setIsViewModalOpen(false);
                  setSelectedOrganization(null);
                }}
                className="rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-gray-200 cursor-pointer transition-all"
              >
                Cerrar
              </button>
            </div>
          </div>
        )}
      </Modal>

      <Modal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedOrganization(null);
          setEditData({ firstName: "", lastName: "" });
        }}
        title="Editar Organización"
      >
        <form onSubmit={handleEdit} className="space-y-4">
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">
              Nombre
            </label>
            <Input
              type="text"
              placeholder="Ingrese el nombre"
              value={editData.firstName}
              onChange={(e) =>
                setEditData({ ...editData, firstName: e.target.value })
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
              value={editData.lastName}
              onChange={(e) =>
                setEditData({ ...editData, lastName: e.target.value })
              }
              required
            />
          </div>

          <div className="flex gap-3 pt-4 justify-end">
            <button
              type="submit"
              disabled={submitting}
              className="px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors disabled:opacity-50 cursor-pointer"
            >
              {submitting ? "Actualizando..." : "Actualizar"}
            </button>
            <button
              type="button"
              onClick={() => {
                setIsEditModalOpen(false);
                setSelectedOrganization(null);
                setEditData({ firstName: "", lastName: "" });
              }}
              className="px-4 py-2 bg-muted text-foreground rounded-lg hover:bg-gray-200 transition-colors cursor-pointer"
            >
              Cancelar
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
