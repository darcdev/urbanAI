# Leader Creation Use Case - Setup Instructions

## Overview
This implementation allows administrators to create leaders in the UrbanAI system. Leaders are users with special permissions to manage urban reports in their assigned geographic areas.

## What Was Created

### Domain Layer (`Urban.AI.Domain`)
- **Leader Entity** (`Leaders/Leader.cs`): Main entity with properties for User, Department, Municipality, and coordinates
- **Leader Repository Interface** (`Leaders/ILeaderRepository.cs`): Contract for data access
- **Leader Errors** (`Leaders/LeaderErrors.cs`): Error definitions for business rules
- **Leader Resources** (`Leaders/Resources/`): Localized error messages (EN/ES)
- **Leader Events** (`Leaders/Events/LeaderCreatedDomainEvent.cs`): Domain event when a leader is created

### Infrastructure Layer (`Urban.AI.Infrastructure`)
- **Leader Repository** (`Database/Repositories/Leader/LeaderRepository.cs`): EF Core implementation with Haversine distance calculation for finding nearest leader
- **Leader Mapping** (`Database/Mappings/Leader/LeaderMapping.cs`): EF Core entity configuration
- **Email Service** (`Email/EmailService.cs`): Gmail SMTP implementation for sending emails
- **Email Options** (`Email/EmailOptions.cs`): Email configuration model

### Application Layer (`Urban.AI.Application`)
- **CreateLeaderCommand** (`Leaders/CreateLeader/CreateLeaderCommand.cs`): CQRS command
- **CreateLeaderHandler** (`Leaders/CreateLeader/CreateLeaderHandler.cs`): Business logic for creating leaders
- **CreateLeaderInputValidation** (`Leaders/CreateLeader/CreateLeaderInputValidation.cs`): FluentValidation rules
- **DTOs** (`Leaders/Dtos/`): Request/Response models
- **Leader Extensions** (`Leaders/LeaderExtensions.cs`): Extension methods for mapping

### Web API Layer (`Urban.AI.WebApi`)
- **LeadersController** (`Controllers/Leaders/LeadersController.cs`): REST API endpoint with Admin authorization

## Database Migration

To create and apply the database migration for the Leaders table:

```powershell
# Navigate to the Infrastructure project
cd src/Urban.AI.Infrastructure

# Create the migration
dotnet ef migrations add AddLeadersTable --startup-project ../Urban.AI.WebApi --context ApplicationDbContext

# Apply the migration
dotnet ef database update --startup-project ../Urban.AI.WebApi --context ApplicationDbContext
```

## Email Configuration

Before using the leader creation feature, configure the email settings in `appsettings.json`:

```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderName": "UrbanAI System",
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "EnableSsl": true
}
```

### Gmail App Password Setup
1. Enable 2-factor authentication on your Gmail account
2. Go to Google Account Settings > Security > 2-Step Verification > App passwords
3. Generate a new app password for "Mail"
4. Use this password in the configuration above

## API Usage

### Create Leader Endpoint
**POST** `/api/v1/leaders`

**Authorization**: Requires Admin role

**Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "municipalityId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "latitude": 4.6097,
  "longitude": -74.0817
}
```

**Response** (201 Created):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa8"
}
```

## Business Rules Implemented

1. **User Creation**: Creates a user account in both the local database and Keycloak
2. **Role Assignment**: Automatically assigns the "Leader" role in Keycloak
3. **Geographic Association**: Links the leader to a specific department and municipality
4. **Coordinate Management**: Stores coordinates for calculating the nearest leader to reports
5. **Email Notification**: Sends credentials via email to the new leader
6. **Validation**: 
   - Email uniqueness
   - Department existence
   - Municipality existence
   - Coordinate ranges (-90 to 90 for latitude, -180 to 180 for longitude)
   - Password requirements (minimum 8 characters)

## Features

### Haversine Distance Calculation
The repository includes a method to find the nearest leader based on geographic coordinates using the Haversine formula. This is used when assigning reports to leaders based on location.

### Localization
Error messages are available in both English and Spanish, automatically selected based on the request culture.

### Security
- Only users with the "Admin" role can create leaders
- Passwords are securely handled by Keycloak
- Email credentials are sent once during creation

## Testing

To test the endpoint, you need:
1. An authenticated user with the "Admin" role
2. Valid Department and Municipality IDs from the database
3. Configured email settings

Example using curl:
```bash
curl -X POST "https://localhost:5001/api/v1/leaders" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@example.com",
    "password": "SecurePass123!",
    "departmentId": "dept-guid-here",
    "municipalityId": "muni-guid-here",
    "latitude": 4.6097,
    "longitude": -74.0817
  }'
```

## Notes

- The Leader entity has a one-to-one relationship with User
- Leaders can be activated/deactivated using the `IsActive` property
- The system prevents creating duplicate leaders for the same user
- Coordinates can be updated later if needed
- The email sending failure doesn't rollback the leader creation (consider adding compensation logic if needed)
