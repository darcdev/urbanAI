# .NET 8 Clean Architecture Template

🚀 **Template moderno de .NET 8** implementando **Clean Architecture** con patrones de diseño avanzados, soporte para bases de datos relacionales y no relacionales, y almacenamiento distribuido con MinIO.

---

## 🛠️ **Guía de Instalación y Configuración del Template**

### **Prerrequisitos**
- .NET 8 SDK
- Docker Desktop (para SQL Server y MinIO)
- Visual Studio 2022 or VS Code (preferiblemente con C# Extensions)

### **Sigue estos 12 pasos para configurar un nuevo proyecto**

1. **Clona el repositorio**
```bash
git clone https://github.com/Yuberley/Template-Clean-Architecture-Net.git
```

2. **Instala el template**
```bash
dotnet new install .\Template-Clean-Architecture-Net
```

3. **Crear un nuevo proyecto apartir del template**
Aquí es obligatorio usar el nombre de tu compañía y proyecto.
```bash
dotnet new cleanarchitecture --name YourNameCompany.YourProjectName
cd YourNameCompany.YourProjectName
```

4. **Restaura las dependencias**
```bash
dotnet restore
```

5. **Compila el proyecto**
```bash
dotnet build
```

6. **Abrelo con VS Code**
```bash
code .
```

7. **Configura del realm en Keycloak**
Personaliza el realm de Keycloak para tu proyecto.
- En el archivo `./infrastructure/resources/application-realm-export.json`, sistituye en todas las partes que diga `enterprisetemplate` por el nombre del proyecto (myprojectname) en minúsculas y sin espacios.

8. **Configura el `appsettings.json`**

- Para la base de datos, el template soporta (Si no se especifica, por defecto usa PostgreSQL):
  - SQL Server
  - PostgreSQL
  - MongoDB

En el archivo `appsettings.json` dentro del proyecto `WebApi`, actualiza la cadena de conexión `DefaultConnection` según la base de datos que estés utilizando.

Ejemplos de cadenas de conexión:
- SQL Server:
  `"Server=localhost,1433;Database=myprojectname;User ID=sa;Password=SQLserverPass*;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"`
- PostgreSQL:
  `"Host=localhost;Port=5432;Database=myprojectname;Username=postgres;Password=postgres;"`
- MongoDB:
  `"mongodb://root:userroot@localhost:27017"`
  
En caso de SQL Server o PostgreSQL, reemplaza Urban.AI por el nombre de tu proyecto.

```json
{
  "ConnectionStrings": {
    "DefaultConnectionDb": TU_CADENA_DE_CONEXION_AQUI
  },
  ...
}
```

- Para Keycloak, actualiza los valores reemplazando `enterprisetemplate` por el nombre de tu proyecto en minúsculas y sin espacios, el mismo que usaste para el realm.

```json
{
  "Keycloak": {
    "BaseUrl": "http://localhost:18080",
    "AdminUrl": "http://localhost:18080/admin/realms/enterprisetemplate/",
    "TokenUrl": "http://localhost:18080/realms/enterprisetemplate/protocol/openid-connect/token",
    "AdminClientId": "enterprisetemplate-admin-client",
    "AdminClientSecret": "UZDmbNxWmV4TlpaCRcju6pMRsyuV3er1",
    "AuthClientId": "enterprisetemplate-auth-client",
    "AuthClientSecret": "3E3yvXaYppoYBF3Ir6DgtEzADKKzSurZ"
  }
}
```

9. **Reemplaza en el archivo .env**
- PROJECT_NAME: nombre del proyecto en minúsculas y sin espacios.
- PATH_PROJECT_NAME: nombre del proyecto con el formato Urban.AI (usado en namespaces y rutas).
```env
PROJECT_NAME=proyectname
PATH_PROJECT_NAME=Urban.AI
```

10. **Configurar servicios de infraestructura**
```bash
# Postgres solo (default)
docker compose \
  -f docker-compose.yml \
  -f docker-compose.postgres.yml \
  up

# SQL Server
docker compose \
  -f docker-compose.yml \
  -f docker-compose.sqlserver.yml \
  --profile sqlserver \
  up

# Postgres + MinIO
docker compose \
  -f docker-compose.yml \
  -f docker-compose.postgres.yml \
  -f docker-compose.minio.yml \
  --profile postgres \
  --profile minio \
  up

# SQL Server + MinIO
docker compose \
  -f docker-compose.yml \
  -f docker-compose.sqlserver.yml \
  -f docker-compose.minio.yml \
  --profile sqlserver \
  --profile minio \
  up

# Postgres + Mongo + MinIO (si tu API requiere los tres)
docker compose \
  -f docker-compose.yml \
  -f docker-compose.postgres.yml \
  -f docker-compose.mongo.yml \
  -f docker-compose.minio.yml \
  --profile postgres \
  --profile mongo \
  --profile minio \
  up
```

11. **Si es DB relacional, aplicar migraciones y crear la base de datos**
```bash
dotnet ef database update --project .\src\Urban.AI.Infrastructure\ --startup-project .\src\Urban.AI.WebApi\
```

12. **Ejecutar la API**
```bash
dotnet run --project .\src\Urban.AI.WebApi\
```

13. **Acceder a la API**
- API: `https://localhost:44385`
- Swagger UI: `https://localhost:44385/swagger`
- MinIO Console: `http://localhost:9001` (admin/admin123)
- PostgreSQL: `localhost:5432` (postgres/postgres)
- SQL Server: `localhost,1433` (sa/YourStrong@Passw0rd)
- MongoDB: `mongodb://root:userroot@localhost:27017`


---

## ✨ **Características del Template**

### �️ **Arquitectura Robusta**
- ✅ **Clean Architecture** - Estructura en capas bien definidas
- ✅ **CQRS + Mediator** - Separación de comandos y consultas
- ✅ **Domain Driven Design** - Modelado rico del dominio
- ✅ **Repository Pattern** - Abstracción de acceso a datos
- ✅ **Unit of Work** - Gestión transaccional consistente

### 🗄️ **Soporte Multi-Base de Datos**
- ✅ **Bases de Datos Relacionales** - SQL Server, PostgreSQL
- ✅ **Bases de Datos NoSQL** - MongoDB, Redis para caching
- ✅ **Entity Framework Core** - ORM con migraciones automáticas
- ✅ **Configuración Flexible** - Múltiples proveedores de datos

### ☁️ **Almacenamiento Distribuido**
- ✅ **MinIO Storage** - Almacenamiento de objetos compatible con S3
- ✅ **Gestión de Archivos** - Subida, descarga y gestión de documentos
- ✅ **Configuración Docker** - Despliegue simplificado con contenedores

### 🎯 **Patrones de Diseño Implementados**
- ✅ **SOLID Principles** - Código mantenible y extensible
- ✅ **Domain Events** - Comunicación desacoplada
- ✅ **Value Objects** - Tipos seguros para primitivos
- ✅ **Specification Pattern** - Consultas complejas reutilizables
- ✅ **Options Pattern** - Configuración tipada y validada

## 🏗️ **Estructura del Template**

Implementación completa de **Clean Architecture** siguiendo las mejores prácticas:

```
┌──────────────────┐
│   WebApi Layer   │  Controllers, Auth, Middleware, API Versioning
├──────────────────┤
│   Application    │  CQRS Handlers, DTOs, Validations, Services  
├──────────────────┤
│  Infrastructure  │  EF Core, Repositories, External Services
├──────────────────┤
│   Domain Layer   │  Entities, Value Objects, Domain Events
└──────────────────┘
```

### **Estructura de Archivos del Template**
```
📁 src/
├── 📁 Domain/           # Entidades, Value Objects, Domain Events
├── 📁 Application/      # Use Cases, DTOs, Validators
├── 📁 Infrastructure/   # Data Access, External Services
└── 📁 WebApi/          # Controllers, Configuration
📁 tests/
├── 📁 UnitTests/       # Pruebas unitarias
├── 📁 IntegrationTests/ # Pruebas de integración
📁 infrastructure/
└── 📄 docker-compose.yml # Servicios de infraestructura
```

---

## ➕ **Comandos extras que podrías usar**

**Configurar Base de Datos**
```bash
# Aplicar migraciones iniciales
dotnet ef database update --project src/Urban.AI.Infrastructure --startup-project src/Urban.AI.WebApi

# Crear migración
dotnet ef migrations add NameMigration --project .\src\Urban.AI.Infrastructure\ --startup-project .\src\Urban.AI.WebApi\ -o Database\Migrations

# Eliminar última migración
dotnet ef migrations remove --project .\src\Urban.AI.Infrastructure\ --startup-project .\src\Urban.AI.WebApi\

# Opcional: Seed con datos de prueba
dotnet run --project src/Urban.AI.WebApi -- --seed-data
```

### **🔧 Configuración Personalizada**

#### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnectionDb": "TU_CADENA_DE_CONEXION_AQUI"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "admin",
    "SecretKey": "admin123",
    "BucketName": "documents"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-super-segura-de-al-menos-32-caracteres",
    "Issuer": "Urban.AI",
    "Audience": "Urban.AI-Users",
    "ExpirationMinutes": 60
  }
}
```

---

## 📚 **Documentación y Recursos**

### **Patrones y Conceptos Implementados**
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)


---

**🎯 Template desarrollado con ❤️ usando .NET 8 y Clean Architecture**