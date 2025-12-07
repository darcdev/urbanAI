# Filtrado de Incidentes por Geografía - Organizaciones

## Descripción
Este documento describe el caso de uso que permite a los usuarios de tipo **Organización/Entidad** listar reportes de incidencias filtrados por departamento o municipio.

## Endpoint

### GET `/api/v1/incidents/by-geography`

**Autorización:** Requiere autenticación con rol `Organization`

**Parámetros de consulta (Query Parameters):**
- `departmentId` (opcional): GUID del departamento para filtrar incidentes
- `municipalityId` (opcional): GUID del municipio para filtrar incidentes

**Respuesta exitosa:** `200 OK`
```json
[
  {
    "id": "guid",
    "radicateNumber": "INC-20231207-XXXX",
    "imageUrl": "https://...",
    "latitude": 4.6097,
    "longitude": -74.0817,
    "citizenEmail": "citizen@example.com",
    "additionalComment": "Descripción adicional",
    "aiDescription": "Descripción generada por IA",
    "category": {
      "id": "guid",
      "name": "Infraestructura"
    },
    "subcategory": {
      "id": "guid",
      "name": "Andén roto"
    },
    "leader": {
      "id": "guid",
      "fullName": "Juan Pérez",
      "email": "leader@example.com"
    },
    "status": "Accepted",
    "priority": "High",
    "createdAt": "2023-12-07T10:30:00Z"
  }
]
```

## Comportamiento del Filtrado

### 1. Sin filtros
Si no se proporciona ningún parámetro, se retornan **todos** los incidentes del sistema.

**Ejemplo:**
```http
GET /api/v1/incidents/by-geography
```

### 2. Filtrar por Departamento
Cuando se proporciona `departmentId`, se retornan todos los incidentes de los municipios que pertenecen a ese departamento.

**Ejemplo:**
```http
GET /api/v1/incidents/by-geography?departmentId=12345678-1234-1234-1234-123456789012
```

### 3. Filtrar por Municipio
Cuando se proporciona `municipalityId`, se retornan solo los incidentes de ese municipio específico.

**Ejemplo:**
```http
GET /api/v1/incidents/by-geography?municipalityId=87654321-4321-4321-4321-210987654321
```

### 4. Filtrar por Municipio dentro de Departamento
Si se proporcionan ambos parámetros, **tiene prioridad el `municipalityId`**, ignorando el `departmentId`.

**Ejemplo:**
```http
GET /api/v1/incidents/by-geography?departmentId=12345678-1234-1234-1234-123456789012&municipalityId=87654321-4321-4321-4321-210987654321
```

## Características del Endpoint

### ✅ Incluye todos los estados
El endpoint retorna incidentes en **todos los estados**:
- `Pending` (Pendiente)
- `Accepted` (Aceptado)
- `Rejected` (Rechazado)

### ✅ Ordenamiento
Los incidentes se ordenan por **fecha de creación descendente** (más recientes primero).

### ✅ Información completa
Cada incidente incluye:
- Datos básicos del incidente
- Categoría y subcategoría
- Información del municipio
- Información del líder asignado (si aplica)
- URL presignada de la imagen (válida por 24 horas)

### ✅ Sin paginación
El endpoint retorna la lista completa sin paginación.

## Códigos de Respuesta

- `200 OK` - Lista de incidentes retornada exitosamente
- `401 Unauthorized` - No autenticado
- `403 Forbidden` - Usuario autenticado pero sin rol de `Organization`

## Arquitectura Implementada

### Capa de Aplicación
- **Query:** `GetIncidentsByGeographyQuery`
- **Handler:** `GetIncidentsByGeographyHandler`

### Capa de Dominio
- **Interfaz:** `IIncidentRepository.GetByGeographyAsync()`

### Capa de Infraestructura
- **Implementación:** `IncidentRepository.GetByGeographyAsync()`

### Capa de WebApi
- **Controller:** `IncidentsController.GetIncidentsByGeography()`

## Ejemplos de Uso

### Usando cURL

**Obtener todos los incidentes:**
```bash
curl -X GET "https://api.urbanai.com/api/v1/incidents/by-geography" \
  -H "Authorization: Bearer {token}"
```

**Filtrar por departamento:**
```bash
curl -X GET "https://api.urbanai.com/api/v1/incidents/by-geography?departmentId=12345678-1234-1234-1234-123456789012" \
  -H "Authorization: Bearer {token}"
```

**Filtrar por municipio:**
```bash
curl -X GET "https://api.urbanai.com/api/v1/incidents/by-geography?municipalityId=87654321-4321-4321-4321-210987654321" \
  -H "Authorization: Bearer {token}"
```

### Usando JavaScript (Fetch)

```javascript
const token = 'your-jwt-token';
const departmentId = '12345678-1234-1234-1234-123456789012';

fetch(`https://api.urbanai.com/api/v1/incidents/by-geography?departmentId=${departmentId}`, {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
```

## Notas Importantes

1. **Autenticación requerida:** Solo usuarios con rol `Organization` pueden acceder a este endpoint.

2. **URLs de imágenes temporales:** Las URLs de las imágenes son presignadas y válidas por 24 horas.

3. **Filtros opcionales:** Ambos parámetros son opcionales, permitiendo flexibilidad en las consultas.

4. **Prioridad de filtros:** Si se envían ambos filtros, el `municipalityId` tiene prioridad sobre el `departmentId`.

5. **Performance:** Para grandes volúmenes de datos, considera implementar paginación en futuras versiones.
