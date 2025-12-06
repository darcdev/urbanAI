# Módulo de Gestión de Usuarios, Roles y Permisos

## Descripción
Módulo completo para gestionar usuarios, roles y permisos del sistema mediante Keycloak como proveedor de identidad. Incluye operaciones CRUD completas para cada entidad.

## Casos de Uso

### Usuarios
1. **GetUsers**: Obtener listado de usuarios con filtros y paginación
2. **GetUserById**: Obtener detalles de un usuario específico
3. **UpdateUser**: Actualizar información de un usuario
4. **DeleteUser**: Eliminar un usuario del sistema
5. **AssignRolesToUser**: Asignar roles a un usuario

### Roles
1. **GetRoles**: Obtener listado de roles
2. **GetRoleById**: Obtener detalles de un rol específico
3. **CreateRole**: Crear un nuevo rol
4. **UpdateRole**: Actualizar información de un rol
5. **DeleteRole**: Eliminar un rol del sistema
6. **AssignPermissionsToRole**: Asignar permisos a un rol

### Permisos
1. **GetPermissions**: Obtener listado de permisos
2. **GetPermissionById**: Obtener detalles de un permiso específico
3. **CreatePermission**: Crear un nuevo permiso
4. **UpdatePermission**: Actualizar información de un permiso
5. **DeletePermission**: Eliminar un permiso del sistema

## Tecnologías
- ASP.NET 8
- Keycloak (proveedor de identidad)
- PostgreSQL
- CQRS + Mediator
- FluentValidation

## Estructura de Carpetas
```
Application/
├── Users/
│   ├── GetUsers/
│   ├── GetUserById/
│   ├── UpdateUser/
│   ├── DeleteUser/
│   ├── AssignRolesToUser/
├── Roles/
│   ├── GetRoles/
│   ├── GetRoleById/
│   ├── CreateRole/
│   ├── UpdateRole/
│   ├── DeleteRole/
│   ├── AssignPermissionsToRole/
├── Permissions/
│   ├── GetPermissions/
│   ├── GetPermissionById/
│   ├── CreatePermission/
│   ├── UpdatePermission/
│   ├── DeletePermission/

WebApi/
├── Controllers/
│   ├── UsersController.cs
│   ├── RolesController.cs
│   ├── PermissionsController.cs
```

## Notas de Implementación
- Todos los endpoints requieren autenticación y autorización
- Las operaciones de gestión requieren permisos específicos
- Sincronización bidireccional entre base de datos local y Keycloak
- Validaciones usando FluentValidation
- Manejo de errores mediante Result Pattern
