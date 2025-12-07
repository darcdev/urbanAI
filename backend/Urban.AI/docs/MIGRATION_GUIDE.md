# Database Migration Steps for Gemini AI Integration

## Prerequisites
- Ensure PostgreSQL database is running
- Update connection string in `appsettings.Development.json` if needed
- Install EF Core tools if not already installed:
  ```powershell
  dotnet tool install --global dotnet-ef
  ```

## Step 1: Create Migration

From the solution root directory, run:

```powershell
cd src/Urban.AI.Infrastructure
dotnet ef migrations add AddCategoriesSubcategoriesAndUpdateIncident --startup-project ../Urban.AI.WebApi --context ApplicationDbContext
```

This will create a new migration with:
- `categories` table
- `subcategories` table
- Updates to `incidents` table (removing old enum columns, adding FK to categories/subcategories)

## Step 2: Review Migration

Check the generated migration file in:
```
src/Urban.AI.Infrastructure/Database/Migrations/
```

The migration should include:
1. **CreateTable** for `categories` with columns:
   - `Id` (Guid, PK)
   - `Code` (string(10), unique index)
   - `Name` (string(200))
   - `Description` (string(500))

2. **CreateTable** for `subcategories` with columns:
   - `Id` (Guid, PK)
   - `CategoryId` (Guid, FK to categories)
   - `Name` (string(300))
   - `Description` (string(500))
   - Unique index on (CategoryId, Name)

3. **AlterTable** for `incidents`:
   - Drop columns: `Caption`, `Category`, `Severity` (old enum-based)
   - Add columns: `CategoryId` (Guid, nullable), `SubcategoryId` (Guid, nullable)
   - Add FK constraints to categories and subcategories

## Step 3: Apply Migration

```powershell
dotnet ef database update --startup-project ../Urban.AI.WebApi --context ApplicationDbContext
```

## Step 4: Seed Categories

After running migrations, you need to populate the categories table.

### Option A: Via API (Recommended for development)

1. Start the application:
   ```powershell
   cd ../Urban.AI.WebApi
   dotnet run
   ```

2. Authenticate and get a token (use Swagger or Postman)

3. Call the seed endpoint:
   ```http
   POST https://localhost:7149/api/v1/incidents/seed-categories
   Authorization: Bearer {your-token}
   ```

### Option B: Via SQL Script

Alternatively, you can create a SQL script to seed the data. Example:

```sql
-- Insert Categories
INSERT INTO categories (id, code, name, description) VALUES
('a1111111-1111-1111-1111-111111111111', 'A', 'Road and Pedestrian Infrastructure', 'Infrastructure related to roads, sidewalks, and pedestrian access'),
('b2222222-2222-2222-2222-222222222222', 'B', 'Public Services and Lighting', 'Issues related to public utilities and street lighting'),
('c3333333-3333-3333-3333-333333333333', 'C', 'Public Space and Furniture', 'Issues related to public spaces, parks, and urban furniture'),
('d4444444-4444-4444-4444-444444444444', 'D', 'Environment and Risk', 'Environmental hazards and risk situations'),
('e5555555-5555-5555-5555-555555555555', 'E', 'Mobility and Traffic', 'Traffic and mobility-related issues');

-- Insert Subcategories for Category A
INSERT INTO subcategories (id, category_id, name, description) VALUES
(gen_random_uuid(), 'a1111111-1111-1111-1111-111111111111', 'Road surface deterioration (potholes, deep cracks)', 'Damage to road surfaces including potholes and deep cracks'),
(gen_random_uuid(), 'a1111111-1111-1111-1111-111111111111', 'Pedestrian or vehicular bridges with structural failures', 'Bridges with structural problems or failures'),
(gen_random_uuid(), 'a1111111-1111-1111-1111-111111111111', 'Broken or non-existent sidewalks (obstacles for people with disabilities)', 'Sidewalks in poor condition or missing, affecting accessibility'),
(gen_random_uuid(), 'a1111111-1111-1111-1111-111111111111', 'Missing manhole covers (high accident risk)', 'Missing or damaged manhole covers posing safety risks');

-- Repeat for other categories...
```

## Step 5: Verify Data

Query the database to ensure categories were seeded correctly:

```sql
SELECT c.code, c.name, COUNT(s.id) as subcategory_count
FROM categories c
LEFT JOIN subcategories s ON c.id = s.category_id
GROUP BY c.id, c.code, c.name
ORDER BY c.code;
```

Expected result:
| Code | Name | Subcategory Count |
|------|------|------------------|
| A | Road and Pedestrian Infrastructure | 4 |
| B | Public Services and Lighting | 4 |
| C | Public Space and Furniture | 4 |
| D | Environment and Risk | 3 |
| E | Mobility and Traffic | 3 |

## Step 6: Test Incident Creation

Create a test incident to verify the integration works:

```http
POST https://localhost:7149/api/v1/incidents
Content-Type: multipart/form-data

{
  "image": <upload a file>,
  "latitude": 4.6097,
  "longitude": -74.0817,
  "citizenEmail": "test@urbanai.com",
  "additionalComment": "Test incident"
}
```

The response should include:
- `category` object with id, code, and name
- `subcategory` object with id and name
- `aiDescription` with Gemini's analysis

## Troubleshooting

### Migration Fails
- Check PostgreSQL connection string
- Ensure no existing data conflicts with new constraints
- Review migration file for issues

### Categories Not Seeding
- Check authentication token is valid
- Verify endpoint permissions
- Check application logs for errors

### Gemini API Not Working
- Verify API key in `appsettings.Development.json`
- Check internet connectivity
- Verify Gemini API quota is not exceeded
- Check application logs for detailed error messages

## Rollback

If you need to rollback the migration:

```powershell
dotnet ef database update <PreviousMigrationName> --startup-project ../Urban.AI.WebApi --context ApplicationDbContext
```

Then remove the migration file:

```powershell
dotnet ef migrations remove --startup-project ../Urban.AI.WebApi --context ApplicationDbContext
```

## Notes

- The migration will **not** delete existing incident data
- Old incidents will have `NULL` values for `CategoryId` and `SubcategoryId`
- The Gemini AI will automatically categorize new incidents
- You can manually update old incidents with categories later if needed
