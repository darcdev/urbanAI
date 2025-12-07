# Organization Creation Use Case - Setup Instructions

## Overview
This implementation allows administrators to create organization users in the UrbanAI system. Organizations represent organizations or institutions that can view statistical reports and analytics without geographic assignment or report management capabilities.

## What Was Created

### Domain Layer (`Urban.AI.Domain`)
1. **Organization.cs** - Main entity with properties:
   - UserId
   - OrganizationName
   - IsActive, CreatedAt
   - Relation with User

2. **IOrganizationRepository.cs** - Repository interface with methods:
   - GetByUserIdAsync
   - GetByIdWithDetailsAsync
   - GetAllActiveEntitiesAsync

3. **OrganizationErrors.cs** - Business rule errors
4. **OrganizationResources.resx/.es.resx** - Bilingual resources (EN/ES)
5. **OrganizationCreatedDomainEvent.cs** - Domain event

### Infrastructure Layer (`Urban.AI.Infrastructure`)
1. **OrganizationRepository.cs** - Implementation with:
   - CRUD methods and specific queries
   - Located in `Database/Repositories/OrganizationUser/` folder

2. **OrganizationMapping.cs** - EF Core configuration:
   - Table "organizations"
   - Relation with User
   - Indexes for optimization

3. **EmailService.cs** - Updated to include:
   - SendOrganizationCredentialsEmailAsync method
   - HTML template for organization credentials email

4. **DependencyInjection.cs** - Updated service registration

### Application Layer (`Urban.AI.Application`)
1. **CreateOrganizationCommand.cs** - CQRS command
2. **CreateOrganizationHandler.cs** - Business logic that:
   - Validates unique email
   - Creates user in local DB and Keycloak
   - Assigns "Organization" role in Keycloak
   - Creates Organization
   - Sends email with credentials

3. **CreateOrganizationInputValidation.cs** - FluentValidation rules:
   - Valid email
   - Password (8-100 characters)
   - Organization name required (max 200 chars)
   - Required IDs

4. **DTOs** - CreateOrganizationRequest and OrganizationResponse
5. **OrganizationExtensions.cs** - Mapping methods

### Web API Layer (`Urban.AI.WebApi`)
1. **OrganizationsController.cs** - REST endpoint:
   - `POST /api/v1/organizations`
   - Requires Admin authorization
   - Returns 201 Created with organization ID

## Database Migration

To create and apply the database migration for the Organizations table:

```powershell
# Navigate to the Infrastructure project
cd src/Urban.AI.Infrastructure

# Create the migration
dotnet ef migrations add AddOrganizationsTable --startup-project ../Urban.AI.WebApi --context ApplicationDbContext

# Apply the migration
dotnet ef database update --startup-project ../Urban.AI.WebApi --context ApplicationDbContext
```

## API Usage

### Create Entity Endpoint
**POST** `/api/v1/entities`

**Authorization**: Requires Admin role

**Request Body**:
```json
{
  "firstName": "María",
  "lastName": "González",
  "email": "maria.gonzalez@alcaldia.gov.co",
  "password": "SecurePassword123!",
  "organizationName": "Alcaldía Municipal de Bogotá"
}
```

**Response** (201 Created):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa9"
}
```

## Business Rules Implemented

1. ✅ Only administrators can create organizations
2. ✅ Unique email per user
3. ✅ User creation in both local database and Keycloak
4. ✅ Automatic "Organization" role assignment in Keycloak
5. ✅ Email notification with credentials
6. ✅ User-Organization association (1:1 relationship)
7. ✅ Organization name required
8. ✅ No geographic location required

## Key Differences from Leader

| Feature | Leader | Organization |
|---------|--------|--------|
| **Purpose** | Manage urban reports/incidents | View statistical reports only |
| **Geographic Location** | Required (Department, Municipality, Coordinates) | Not required |
| **Organization Info** | No | Required (Organization Name) |
| **Report Management** | Yes | No (read-only) |
| **Nearest Calculation** | Yes (Haversine distance) | No |

## Features

### No Geographic Restrictions
Unlike leaders, organizations are not tied to specific geographic locations. They can view aggregate data across all regions.

### Organization-Based Access
each organization is associated with an organization name, which identifies the institution they represent.

### Statistical Reports Only
organization users have view-only access to statistical reports and analytics, without the ability to manage or resolve incidents.

## Testing

To test the endpoint, you need:
1. An authenticated user with the "Admin" role
2. Configured email settings in appsettings.json

Example using curl:
```bash
curl -X POST "https://localhost:5001/api/v1/entities" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Carlos",
    "lastName": "Ramírez",
    "email": "carlos.ramirez@gobernacion.gov.co",
    "password": "SecurePass123!",
    "organizationName": "Gobernación del Valle del Cauca"
  }'
```

## Email Template

The system sends a customized email to organization users with:
- Welcome message including organization name
- Login credentials (email and temporary password)
- Information about their role as entity user
- Reminder to change password on first login
- Security notice

## Notes

- The Entity has a one-to-one relationship with User
- Entities can be activated/deactivated using the `IsActive` property
- The system prevents creating duplicate entities for the same user
- Organization name can be updated later if needed
- Email sending failure returns an error (consider adding compensation logic if needed)
- The "Organization" role must exist in Keycloak before creating organization users

## Next Steps

1. **Create the "Organization" role in Keycloak** if it doesn't exist
2. **Run the database migration** to create the Organizations table
3. **Configure email settings** in appsettings.json
4. **Test the endpoint** with an Admin user

## Security Considerations

- Only Admin users can create organizations
- Passwords are securely managed by Keycloak
- Email credentials are sent once during creation
- organization users have restricted access compared to administrators
