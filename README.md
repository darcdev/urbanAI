# 🏙️ UrbanAI - Plataforma Integral de Gobernanza Urbana Inteligente

<div align="center">

![UrbanAI Logo](https://via.placeholder.com/200x200/4A90E2/FFFFFF?text=UrbanAI)

**Infraestructura crítica de gobernanza que cierra la brecha entre ciudadanía y Estado**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Keycloak](https://img.shields.io/badge/Keycloak-Auth-blue?logo=keycloak)](https://www.keycloak.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

---

## 📖 Índice

- [Descripción](#-descripción)
- [Características Principales](#-características-principales)
- [Arquitectura Tecnológica](#️-arquitectura-tecnológica)
- [Prerrequisitos](#-prerrequisitos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Casos de Uso Principales](#-casos-de-uso-principales)
- [API Endpoints](#-api-endpoints)
- [Integraciones](#-integraciones)
- [Seguridad y Autenticación](#-seguridad-y-autenticación)
- [Contribución](#-contribución)
- [Licencia](#-licencia)

---

## 🎯 Descripción

**UrbanAI** es una plataforma integral de gestión urbana inteligente que cierra la brecha entre la ciudadanía y el Estado. A diferencia de las aplicaciones de reportes tradicionales, UrbanAI se constituye como una **infraestructura crítica de gobernanza** diseñada bajo un modelo de **"Compliance-First" (Cumplimiento Normativo)**.

### ¿Qué nos hace diferentes?

La solución combina **Inteligencia Artificial** para la detección técnica de incidentes con una **capa de validación humana (Ediles)**, garantizando datos estructurados y veraces. Operativamente, se integra de forma nativa con los ecosistemas del Estado (X-Road, SECOP II, Autenticación Digital), permitiendo a las administraciones pasar de una **gestión reactiva a una Gobernanza Anticipatoria**, mientras habilita mecanismos de financiación colaborativa para micro-intervenciones comunitarias.

### Modelo de Gobernanza

- **Compliance-First**: Diseñado desde el principio para cumplir con normativas estatales
- **Validación Dual**: IA + Validación humana (Ediles)
- **Gobernanza Anticipatoria**: De la reactividad a la previsión inteligente
- **Financiación Colaborativa**: Micro-intervenciones comunitarias habilitadas

---

## ✨ Características Principales

### 🤖 Inteligencia Artificial Integrada
- **Análisis automático de incidentes** mediante Google Gemini AI
- **Clasificación inteligente** en 5 categorías principales (A-E) con 18 subcategorías
- **Detección técnica** de problemas urbanos mediante análisis de imágenes
- **Generación de descripciones** técnicas automáticas

### 👥 Gestión de Actores
- **Ciudadanos**: Reporte de incidentes con geolocalización
- **Ediles (Líderes)**: Validación y gestión territorial de incidentes
- **Organizaciones**: Visualización de analytics y estadísticas
- **Administradores**: Gestión completa del sistema

### 🗺️ Geolocalización Avanzada
- **Estructura territorial completa**: Departamentos → Municipios → Corregimientos
- **Filtrado geográfico** de incidentes por niveles administrativos
- **Asignación territorial** de ediles
- **Visualización en mapas** con coordenadas precisas

### 📊 Categorización Estructurada
- **A. Infraestructura Vial y Peatonal**: Vías, aceras, pasos peatonales, ciclovías
- **B. Servicios Públicos e Iluminación**: Alumbrado, servicios, señalización
- **C. Espacio Público y Mobiliario**: Parques, mobiliario, zonas verdes
- **D. Medio Ambiente y Riesgo**: Residuos, contaminación, estructuras en riesgo
- **E. Movilidad y Tráfico**: Semaforización, transporte, congestión

### 🔐 Seguridad Enterprise
- **Keycloak** para autenticación y autorización
- **JWT Tokens** con validación robusta
- **Roles y permisos granulares**
- **Integración con autenticación digital del Estado**

### 📁 Almacenamiento Distribuido
- **MinIO** para gestión de imágenes y documentos
- **Almacenamiento escalable** y de alta disponibilidad
- **URLs presignadas** para acceso seguro a recursos

### 🔗 Integraciones Estatales
- **SECOP II**: Consulta de contratos públicos en tiempo real
- **Geografía oficial**: Datos estructurados de división territorial
- **X-Road Ready**: Preparado para interoperabilidad con sistemas del Estado

---

## 🏗️ Arquitectura Tecnológica

### Backend (.NET 8)

UrbanAI está construido siguiendo los principios de **Clean Architecture** con patrones avanzados:

```
┌─────────────────────────────────────────────────┐
│           WebAPI Layer (Controllers)            │
│  ┌───────────────────────────────────────────┐  │
│  │     Application Layer (CQRS + Mediator)   │  │
│  │  ┌─────────────────────────────────────┐  │  │
│  │  │  Infrastructure Layer (Repositories) │  │  │
│  │  │  ┌───────────────────────────────┐   │  │  │
│  │  │  │   Domain Layer (Entities)     │   │  │  │
│  │  │  └───────────────────────────────┘   │  │  │
│  │  └─────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

#### Stack Tecnológico

**Core Framework**
- .NET 8 SDK
- ASP.NET Core Web API
- Entity Framework Core 8.0

**Patrones de Diseño**
- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Mediator Pattern
- Repository Pattern
- Unit of Work

**Base de Datos**
- PostgreSQL 17 (Base de datos principal)
- Redis (Caché distribuido)
- Entity Framework Core con Code-First Migrations

**Autenticación y Seguridad**
- Keycloak (Identity & Access Management)
- JWT Bearer Tokens
- OAuth 2.0 / OpenID Connect

**Almacenamiento**
- MinIO (Object Storage S3-compatible)
- Almacenamiento distribuido de imágenes y documentos

**Inteligencia Artificial**
- Google Gemini AI (gemini-1.5-flash)
- Análisis de imágenes y clasificación automática

**Validación y Testing**
- FluentValidation
- xUnit (Unit Testing)

**Infraestructura**
- Docker & Docker Compose
- Contenedores para todos los servicios

---

## 📋 Prerrequisitos

Antes de comenzar, asegúrate de tener instalados:

### Software Requerido
- **.NET 8 SDK** - [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop** - [Descargar](https://www.docker.com/products/docker-desktop)
- **PostgreSQL Client** (opcional, para gestión de BD)
- **Git** - [Descargar](https://git-scm.com/)

### IDEs Recomendados
- **Visual Studio 2022** (Community o superior)
- **JetBrains Rider**
- **VS Code** con extensiones de C#

### Puertos Necesarios
Asegúrate de que los siguientes puertos estén disponibles:
- `5000` - API Backend (HTTP)
- `5001` - API Backend (HTTPS)
- `5432` - PostgreSQL
- `18080` - Keycloak
- `9000` - MinIO Console
- `9001` - MinIO API
- `6379` - Redis

---

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone https://github.com/darcdev/urbanAI.git
cd urbanAI
```

### 2. Configurar Servicios de Infraestructura

Navega al directorio de infraestructura y levanta los servicios con Docker Compose:

```bash
cd backend/Urban.AI/infrastructure
docker-compose up -d
```

Esto levantará:
- PostgreSQL (Base de datos principal)
- Keycloak (Autenticación)
- MinIO (Almacenamiento de objetos)
- Redis (Caché)

### 3. Configurar Keycloak

#### Importar el Realm

1. Accede a Keycloak: `http://localhost:18080`
2. Credenciales por defecto:
   - Usuario: `admin`
   - Password: `admin`
3. Importa el realm desde: `infrastructure/resources/application-realm-export.json`

#### Configurar Clientes

El realm `urbanai` ya viene preconfigurado con:
- `urbanai-auth-client`: Cliente para autenticación de usuarios
- `urbanai-admin-client`: Cliente administrativo para gestión

### 4. Configurar Variables de Entorno

Edita los archivos de configuración según tu entorno:

**appsettings.Development.json** (Backend)
```json
{
  "ConnectionStrings": {
    "DefaultConnectionDb": "Host=localhost;Port=5432;Database=urbanai;Username=postgres;Password=postgres;",
    "RedisConnection": "localhost:6379"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE"
  },
  "Minio": {
    "Host": "localhost:9000",
    "Username": "rootadmin",
    "Password": "rootadmin",
    "IsSecureSSL": false
  }
}
```

### 5. Ejecutar Migraciones de Base de Datos

Desde el directorio raíz del backend:

```bash
cd backend/Urban.AI/src/Urban.AI.Infrastructure

# Crear la migración inicial
dotnet ef migrations add InitialCreate --startup-project ../Urban.AI.WebApi

# Aplicar migraciones
dotnet ef database update --startup-project ../Urban.AI.WebApi
```

### 6. Ejecutar el Backend

```bash
cd backend/Urban.AI/src/Urban.AI.WebApi
dotnet restore
dotnet build
dotnet run
```

El API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

### 7. Semillar Datos Iniciales

Ejecuta los siguientes endpoints para poblar datos iniciales:

```bash
# Semillar categorías de incidentes
POST http://localhost:5000/api/v1/incidents/seed-categories
Authorization: Bearer {admin-token}

# Semillar datos geográficos (Departamentos, Municipios, Corregimientos)
POST http://localhost:5000/api/v1/geography/seed
Authorization: Bearer {admin-token}
```

---

## 📁 Estructura del Proyecto

```
urbanAI/
├── backend/
│   └── Urban.AI/
│       ├── src/
│       │   ├── Urban.AI.Domain/              # Entidades, Value Objects, Interfaces
│       │   │   ├── Users/
│       │   │   ├── Incidents/
│       │   │   ├── Leaders/
│       │   │   ├── Organizations/
│       │   │   ├── Geography/
│       │   │   └── Common/
│       │   │
│       │   ├── Urban.AI.Application/         # Casos de Uso (CQRS)
│       │   │   ├── Auth/                     # Login, Register, WhoAmI
│       │   │   ├── Incidents/                # CRUD + AI Analysis
│       │   │   ├── Leaders/                  # Gestión de Ediles
│       │   │   ├── Organizations/            # Gestión de Organizaciones
│       │   │   ├── Geography/                # Datos Territoriales
│       │   │   ├── Categories/               # Categorías de Incidentes
│       │   │   ├── Secop/                    # Integración SECOP
│       │   │   └── Common/                   # Abstractions (ICommand, IQuery)
│       │   │
│       │   ├── Urban.AI.Infrastructure/      # Implementaciones
│       │   │   ├── Database/
│       │   │   │   ├── Mappings/            # EF Core Configurations
│       │   │   │   └── Repositories/        # Implementación de Repositorios
│       │   │   ├── Auth/                    # Keycloak Integration
│       │   │   ├── AI/                      # Gemini AI Service
│       │   │   ├── Storage/                 # MinIO Service
│       │   │   ├── Email/                   # Email Service
│       │   │   └── ExternalServices/        # SECOP, etc.
│       │   │
│       │   └── Urban.AI.WebApi/             # Controllers, Configurations
│       │       ├── Controllers/
│       │       ├── Configurations/
│       │       └── Program.cs
│       │
│       ├── infrastructure/
│       │   ├── docker-compose.yml           # Servicios de infraestructura
│       │   ├── keycloak.env
│       │   ├── postgres.env
│       │   ├── minio.env
│       │   └── resources/
│       │       └── application-realm-export.json
│       │
│       ├── docs/                            # Documentación técnica
│       │   ├── GEMINI_AI_INTEGRATION.md
│       │   ├── SECOP_INTEGRATION.md
│       │   ├── ORGANIZATION_CREATION_SETUP.md
│       │   └── MIGRATION_GUIDE.md
│       │
│       └── tests/
│           └── Urban.AI.UnitTests/
│
└── frontend/                                # (Frontend - TBD)
```

---

## 💼 Casos de Uso Principales

### 🔐 Autenticación y Usuarios

#### Registro de Usuario Ciudadano
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan.perez@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

#### Login
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "juan.perez@example.com",
  "password": "SecurePass123!"
}
```

### 📢 Gestión de Incidentes

#### Crear Incidente (con IA)
```http
POST /api/v1/incidents
Authorization: Bearer {token}
Content-Type: multipart/form-data

{
  "title": "Hueco en vía principal",
  "description": "Hueco grande en la Calle 72 con Carrera 15",
  "latitude": 4.6533,
  "longitude": -74.0836,
  "departmentId": "uuid",
  "municipalityId": "uuid",
  "image": (binary file)
}
```

**El sistema automáticamente:**
1. Analiza la imagen con Gemini AI
2. Clasifica en categoría y subcategoría
3. Genera descripción técnica
4. Asigna al edil territorial correspondiente

#### Obtener Incidentes por Geografía
```http
GET /api/v1/incidents/geography?departmentId={uuid}&municipalityId={uuid}
Authorization: Bearer {token}
```

### 👤 Gestión de Ediles (Líderes)

#### Crear Edil
```http
POST /api/v1/leaders
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "firstName": "María",
  "lastName": "González",
  "email": "maria.gonzalez@alcaldia.gov.co",
  "password": "SecurePass123!",
  "phoneNumber": "+573001234567",
  "departmentId": "uuid",
  "municipalityId": "uuid",
  "townshipId": "uuid"
}
```

### 🏢 Gestión de Organizaciones

#### Crear Organización
```http
POST /api/v1/organizations
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "firstName": "Carlos",
  "lastName": "Ramírez",
  "email": "carlos.ramirez@organizacion.org",
  "password": "SecurePass123!",
  "organizationName": "Fundación Desarrollo Urbano"
}
```

### 🔍 Integración SECOP

#### Buscar Contratos por Código
```http
GET /api/v1/secop/search/code/{contractCode}
Authorization: Bearer {token}
```

#### Buscar Contratos por Rango de Fechas
```http
GET /api/v1/secop/search/date-range?startDate=2024-01-01&endDate=2024-12-31
Authorization: Bearer {token}
```

---

## 🔌 API Endpoints

### Documentación Interactiva

Una vez el servidor esté ejecutándose, accede a:
- **Swagger UI**: `http://localhost:5000/swagger`
- **ReDoc**: `http://localhost:5000/redoc` (si está habilitado)

### Endpoints Principales

| Módulo | Método | Endpoint | Descripción |
|--------|--------|----------|-------------|
| **Auth** | POST | `/api/v1/auth/register` | Registro de ciudadano |
| **Auth** | POST | `/api/v1/auth/login` | Autenticación |
| **Auth** | GET | `/api/v1/auth/whoami` | Información del usuario actual |
| **Incidents** | POST | `/api/v1/incidents` | Crear incidente |
| **Incidents** | GET | `/api/v1/incidents` | Listar todos los incidentes |
| **Incidents** | GET | `/api/v1/incidents/geography` | Filtrar por geografía |
| **Incidents** | GET | `/api/v1/incidents/leader` | Incidentes del edil |
| **Incidents** | PATCH | `/api/v1/incidents/{id}/status` | Actualizar estado |
| **Leaders** | POST | `/api/v1/leaders` | Crear edil (Admin) |
| **Leaders** | GET | `/api/v1/leaders` | Listar ediles |
| **Leaders** | PUT | `/api/v1/leaders/{id}` | Actualizar edil |
| **Organizations** | POST | `/api/v1/organizations` | Crear organización (Admin) |
| **Categories** | GET | `/api/v1/categories` | Listar categorías |
| **Categories** | POST | `/api/v1/categories/seed` | Semillar categorías |
| **Geography** | GET | `/api/v1/geography/departments` | Listar departamentos |
| **Geography** | GET | `/api/v1/geography/municipalities/{deptId}` | Municipios por departamento |
| **Geography** | GET | `/api/v1/geography/townships/{munId}` | Corregimientos por municipio |
| **Geography** | POST | `/api/v1/geography/seed` | Semillar datos geográficos |
| **Secop** | GET | `/api/v1/secop/search/code/{code}` | Buscar contrato |
| **Secop** | GET | `/api/v1/secop/search/date-range` | Buscar por fechas |

---

## 🔗 Integraciones

### 🤖 Google Gemini AI

**Modelo**: `gemini-1.5-flash`

**Funcionalidad**: Análisis automático de imágenes de incidentes urbanos

**Proceso**:
1. Usuario sube imagen del incidente
2. Imagen se envía a Gemini AI con prompt estructurado
3. IA clasifica en categoría y subcategoría
4. Genera descripción técnica detallada
5. Sistema almacena análisis en BD

**Configuración**: 
```json
{
  "Gemini": {
    "ApiKey": "YOUR_API_KEY",
    "Model": "gemini-1.5-flash",
    "TimeoutSeconds": 30
  }
}
```

### 📜 SECOP II (Sistema Electrónico de Contratación Pública)

**API**: `licitaciones.info`

**Funcionalidad**: Consulta de contratos públicos en tiempo real

**Capacidades**:
- Búsqueda por código de contrato
- Búsqueda por rango de fechas
- Información detallada de contratos
- Cronograma de eventos

**Casos de Uso**:
- Transparencia en contratación pública
- Seguimiento de obras relacionadas con incidentes
- Analytics de inversión pública por territorio

### 🗺️ Datos Geográficos Oficiales

**Estructura**:
- Departamentos (32 departamentos)
- Municipios (~1,100 municipios)
- Corregimientos/Veredas

**Fuente**: Datos oficiales de división territorial de Colombia

### 🔐 Keycloak (Identity & Access Management)

**Realm**: `urbanai`

**Clientes configurados**:
- `urbanai-auth-client`: Autenticación de usuarios
- `urbanai-admin-client`: Gestión administrativa

**Roles**:
- `Admin`: Administrador del sistema
- `Leader`: Edil territorial
- `Organization`: Organización/Entidad
- `User`: Ciudadano

### 📁 MinIO (Object Storage)

**Buckets**:
- `incidents`: Imágenes de incidentes
- `documents`: Documentos generales

**Funcionalidades**:
- Generación de URLs presignadas
- Almacenamiento escalable
- Compatibilidad S3

---

## 🔐 Seguridad y Autenticación

### Modelo de Seguridad

UrbanAI implementa un modelo de seguridad robusto basado en:

#### OAuth 2.0 + OpenID Connect
- Tokens JWT emitidos por Keycloak
- Validación de tokens en cada request
- Refresh tokens para sesiones prolongadas

#### Roles y Permisos

| Rol | Permisos |
|-----|----------|
| **Admin** | Gestión completa del sistema, creación de ediles y organizaciones |
| **Leader** | Validación de incidentes en su territorio, actualización de estados |
| **Organization** | Visualización de analytics y reportes |
| **User** | Creación de incidentes, consulta de sus reportes |

#### Políticas de Autorización

```csharp
[Authorize(Roles = "Admin")]          // Solo administradores
[Authorize(Roles = "Leader,Admin")]   // Ediles y administradores
[Authorize]                           // Cualquier usuario autenticado
```

#### Seguridad de Datos

- Contraseñas hasheadas con bcrypt
- Conexiones HTTPS en producción
- Validación de entrada con FluentValidation
- Protección contra SQL Injection (EF Core parametrizado)
- CORS configurado por dominio

---

## 🌍 Modelo de Gobernanza

### Compliance-First (Cumplimiento Normativo)

UrbanAI está diseñado desde el principio para cumplir con:
- Normativas de protección de datos personales
- Estándares de interoperabilidad estatal
- Protocolos de transparencia y rendición de cuentas

### Validación Dual: IA + Humano

```
Ciudadano reporta → IA analiza y clasifica → Edil valida → Estado actúa
```

**Beneficios**:
- Datos estructurados y confiables
- Reducción de reportes falsos
- Clasificación técnica precisa
- Responsabilidad territorial clara

### De Reactiva a Anticipatoria

**Antes**: Esperar quejas → Investigar → Actuar

**Con UrbanAI**: 
- Detección temprana mediante IA
- Analytics predictivo de zonas críticas
- Asignación proactiva de recursos
- Planificación basada en datos reales

---

## 🛠️ Desarrollo

### Ejecutar en Modo Desarrollo

```bash
# Backend
cd backend/Urban.AI/src/Urban.AI.WebApi
dotnet watch run

# Con hot reload
dotnet watch --no-hot-reload run
```

### Ejecutar Tests

```bash
cd backend/Urban.AI/tests/Urban.AI.UnitTests
dotnet test
```

### Crear Nueva Migración

```bash
cd backend/Urban.AI/src/Urban.AI.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Urban.AI.WebApi
dotnet ef database update --startup-project ../Urban.AI.WebApi
```

### Code Style

El proyecto sigue los principios de **Clean Code** y **SOLID**:
- Nombres descriptivos en inglés
- Métodos pequeños y enfocados
- Separación de responsabilidades
- Uso extensivo de patrones de diseño

---

## 📚 Documentación Adicional

Consulta la carpeta `/docs` para documentación técnica detallada:

- [**GEMINI_AI_INTEGRATION.md**](backend/Urban.AI/docs/GEMINI_AI_INTEGRATION.md) - Integración de IA
- [**SECOP_INTEGRATION.md**](backend/Urban.AI/docs/SECOP_INTEGRATION.md) - Integración SECOP
- [**ORGANIZATION_CREATION_SETUP.md**](backend/Urban.AI/docs/ORGANIZATION_CREATION_SETUP.md) - Creación de organizaciones
- [**LEADER_CREATION_SETUP.md**](backend/Urban.AI/docs/LEADER_CREATION_SETUP.md) - Creación de ediles
- [**MIGRATION_GUIDE.md**](backend/Urban.AI/docs/MIGRATION_GUIDE.md) - Guía de migraciones

---

## 🤝 Contribución

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

### Guías de Contribución

- Sigue la arquitectura Clean Architecture existente
- Escribe tests para nuevas funcionalidades
- Documenta cambios importantes
- Usa conventional commits
- Código en inglés, documentación en español

---

## 👥 Autores

- **Yuberley** - *Arquitectura y Desarrollo Backend* - [@Yuberley](https://github.com/Yuberley)
- **darcdev** - *Desarrollo y Documentación* - [@darcdev](https://github.com/darcdev)

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

---

## 🙏 Agradecimientos

- Comunidad .NET
- Google Gemini AI
- Keycloak Team
- PostgreSQL Community
- Todos los contribuidores y usuarios de UrbanAI

---

## 📞 Contacto y Soporte

¿Tienes preguntas o necesitas ayuda?

- **Issues**: [GitHub Issues](https://github.com/darcdev/urbanAI/issues)
- **Email**: urbanai.notifications@gmail.com

---

<div align="center">

**UrbanAI** - Cerrando la brecha entre ciudadanía y Estado 🏙️

Hecho con ❤️ en Colombia

</div>
