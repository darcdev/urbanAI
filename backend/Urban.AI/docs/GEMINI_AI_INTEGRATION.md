# Gemini AI Integration for Urban Incident Analysis

## Overview
This implementation integrates Google Gemini AI to automatically analyze incident images reported by citizens and classify them into predefined categories and subcategories.

## Changes Made

### 1. Domain Layer

#### New Entities
- **Category** (`Urban.AI.Domain.Incidents.Category`): Represents incident categories (A-E)
- **Subcategory** (`Urban.AI.Domain.Incidents.Subcategory`): Represents specific incident types within categories

#### Updated Entities
- **Incident**: Now includes `CategoryId`, `SubcategoryId`, and `AiDescription` instead of the old enum-based approach
- **IncidentAnalysis**: Value object updated to contain Category and Subcategory IDs plus AI-generated description

#### Repositories
- **ICategoryRepository**: Interface for Category operations
- **ISubcategoryRepository**: Interface for Subcategory operations

### 2. Infrastructure Layer

#### AI Service
- **GeminiIncidentAnalysisService**: Implements `IIncidentAnalysisService` using Google Gemini API
  - Uses `gemini-1.5-flash` model for fast and accurate image analysis
  - Sends structured prompts with category definitions
  - Parses JSON responses from Gemini
  - Maps AI results to database entities
  - Handles "Not Applicable" cases when images don't match categories

#### Configuration Classes
- **GeminiOptions**: Configuration for Gemini API
- **GeminiRequest/Response**: DTOs for API communication

#### Database
- **CategoryMapping**: EF Core configuration for Category table
- **SubcategoryMapping**: EF Core configuration for Subcategory table
- **CategoryRepository**: Implementation with EF Core
- **SubcategoryRepository**: Implementation with EF Core
- **IncidentMapping**: Updated to include Category and Subcategory foreign keys

### 3. Application Layer

#### Seed Categories UseCase
- **SeedCategoriesCommand**: Command to populate categories
- **SeedCategoriesHandler**: Creates all predefined categories and subcategories

Categories created:
- **A. Road and Pedestrian Infrastructure** (4 subcategories)
- **B. Public Services and Lighting** (4 subcategories)
- **C. Public Space and Furniture** (4 subcategories)
- **D. Environment and Risk** (3 subcategories)
- **E. Mobility and Traffic** (3 subcategories)

#### Updated DTOs
- **IncidentResponse**: Now includes `CategoryDto` and `SubcategoryDto` instead of string-based category
- **CategoryDto**: Response DTO for category information
- **SubcategoryDto**: Response DTO for subcategory information

### 4. WebAPI Layer

#### Configuration
- **appsettings.json**: Added Gemini configuration section
- **appsettings.Development.json**: Includes development API key

#### Controllers
- **IncidentsController**: Added `/api/v1/incidents/seed-categories` endpoint

## Setup Instructions

### 1. Update API Key
Replace the Gemini API key in `appsettings.Development.json` or use environment variables:

```json
{
  "Gemini": {
    "ApiKey": "YOUR_NEW_GEMINI_API_KEY_HERE"
  }
}
```

**⚠️ IMPORTANT**: The API key in the current configuration should be regenerated as it was exposed publicly.

### 2. Run Database Migration
Create and apply a new migration to add the Categories and Subcategories tables:

```powershell
# From the solution root
cd src/Urban.AI.Infrastructure
dotnet ef migrations add AddCategoriesAndSubcategories --startup-project ../Urban.AI.WebApi
dotnet ef database update --startup-project ../Urban.AI.WebApi
```

### 3. Seed Categories
After running migrations, call the seed endpoint to populate categories:

```http
POST http://localhost:5000/api/v1/incidents/seed-categories
Authorization: Bearer {your-token}
```

Or use the Swagger UI to execute the endpoint.

### 4. Test Incident Creation
Create an incident with an image to test the AI analysis:

```http
POST http://localhost:5000/api/v1/incidents
Content-Type: multipart/form-data

{
  "image": <file>,
  "latitude": 4.6097,
  "longitude": -74.0817,
  "citizenEmail": "test@example.com",
  "additionalComment": "Broken sidewalk"
}
```

The response will include the AI-analyzed category and subcategory.

## How It Works

1. **Citizen uploads image**: When a citizen reports an incident with an image
2. **Image sent to Gemini**: The image is converted to base64 and sent to Gemini API with a structured prompt
3. **AI analyzes**: Gemini analyzes the image and returns:
   - Category (A-E or "Not Applicable")
   - Subcategory (specific problem type)
   - Description (max 80 words in English)
4. **Mapping to database**: The service maps Gemini's text response to actual Category and Subcategory entities
5. **Incident stored**: The incident is saved with Category, Subcategory, and AI description

## Error Handling

- If Gemini API fails, the incident creation will fail with `AnalysisFailed` error
- If the image doesn't match any category, it's marked as "Not Applicable" (CategoryId and SubcategoryId = null)
- If Gemini returns a category/subcategory not in the database, it's also marked as "Not Applicable"

## Future Improvements

1. Add retry logic for Gemini API calls
2. Implement caching for frequently analyzed similar images
3. Add admin endpoint to view/edit categories and subcategories
4. Support multilingual descriptions
5. Add confidence score from AI analysis
6. Implement fallback to FakeIncidentAnalysisService if Gemini quota is exceeded

## API Response Example

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "radicateNumber": "INC-RA-A4B2",
  "imageUrl": "https://...",
  "latitude": 4.6097,
  "longitude": -74.0817,
  "citizenEmail": "citizen@example.com",
  "additionalComment": "Broken sidewalk",
  "aiDescription": "The image shows a damaged sidewalk with broken concrete and exposed rebar, creating a significant hazard for pedestrians.",
  "category": {
    "id": "...",
    "code": "A",
    "name": "Road and Pedestrian Infrastructure"
  },
  "subcategory": {
    "id": "...",
    "name": "Broken or non-existent sidewalks (obstacles for people with disabilities)"
  },
  "status": "Pending",
  "priority": "NotEstablished",
  "createdAt": "2025-12-07T10:30:00Z"
}
```

## Notes

- The Gemini model used is `gemini-1.5-flash` for optimal speed and cost
- Temperature is set to 0.4 for more deterministic outputs
- Max output tokens: 500 (sufficient for category + subcategory + description)
- Response format is JSON for easy parsing
