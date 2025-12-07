# Obtener Categorías - Endpoint Público

## Descripción
Este documento describe el endpoint público que permite obtener todas las categorías de incidentes con sus respectivas subcategorías.

## Endpoint

### GET `/api/v1/categories`

**Autorización:** No requiere autenticación (público)

**Parámetros:** Ninguno

**Respuesta exitosa:** `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "code": "INFRA",
    "name": "Infraestructura",
    "description": "Problemas relacionados con infraestructura urbana",
    "subcategories": [
      {
        "id": "1fa85f64-5717-4562-b3fc-2c963f66afa7",
        "name": "Andén roto",
        "description": "Andenes en mal estado o con daños"
      },
      {
        "id": "2fa85f64-5717-4562-b3fc-2c963f66afa8",
        "name": "Escaleras",
        "description": "Problemas con escaleras en espacios públicos"
      },
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
        "name": "Falta de rampa",
        "description": "Ausencia de rampas de accesibilidad"
      }
    ]
  },
  {
    "id": "4fa85f64-5717-4562-b3fc-2c963f66afaa",
    "code": "SERV",
    "name": "Servicios Públicos",
    "description": "Problemas con servicios públicos",
    "subcategories": [
      {
        "id": "5fa85f64-5717-4562-b3fc-2c963f66afab",
        "name": "Fuga de agua",
        "description": "Fugas en tuberías o red de acueducto"
      },
      {
        "id": "6fa85f64-5717-4562-b3fc-2c963f66afac",
        "name": "Falta de luz/energía",
        "description": "Problemas con alumbrado público o energía eléctrica"
      }
    ]
  }
]
```

## Características del Endpoint

### ✅ Acceso público
- **No requiere autenticación:** Cualquier usuario puede consultar las categorías
- Útil para que los ciudadanos conozcan las categorías disponibles antes de reportar un incidente

### ✅ Información completa
Cada categoría incluye:
- **id**: GUID único de la categoría
- **code**: Código corto de la categoría (máx. 10 caracteres)
- **name**: Nombre de la categoría (máx. 200 caracteres)
- **description**: Descripción detallada de la categoría (máx. 500 caracteres)
- **subcategories**: Lista de subcategorías asociadas

Cada subcategoría incluye:
- **id**: GUID único de la subcategoría
- **name**: Nombre de la subcategoría (máx. 300 caracteres)
- **description**: Descripción detallada de la subcategoría (máx. 500 caracteres)

### ✅ Estructura jerárquica
- Las categorías incluyen sus subcategorías en una estructura anidada
- Facilita la visualización y selección en interfaces de usuario

## Uso del Endpoint

### Casos de uso típicos:

1. **Aplicación móvil de ciudadanos:**
   - Obtener las categorías disponibles para mostrarlas en un formulario de reporte
   - Permitir que el usuario seleccione la categoría y subcategoría apropiada
   - Puede complementar la categorización automática por IA

2. **Dashboard de organizaciones:**
   - Mostrar filtros por categoría para análisis estadísticos
   - Crear visualizaciones agrupadas por categoría

3. **Panel de líderes:**
   - Filtrar incidentes por tipo de categoría
   - Priorizar según el tipo de problema reportado

## Códigos de Respuesta

- `200 OK` - Lista de categorías retornada exitosamente
- `500 Internal Server Error` - Error del servidor

## Arquitectura Implementada

### Capa de Aplicación
- **DTOs:**
  - `CategoryResponse`: DTO con información completa de la categoría
  - `SubcategoryResponse`: DTO con información de la subcategoría
- **Query:** `GetCategoriesQuery`
- **Handler:** `GetCategoriesHandler`
- **Extensions:** `CategoryExtensions` con métodos `ToResponse()`

### Capa de Dominio
- **Interfaz:** `ICategoryRepository.GetAllWithSubcategoriesAsync()`
- **Entidades:** `Category`, `Subcategory`

### Capa de WebApi
- **Controller:** `CategoriesController.GetCategories()`

## Ejemplos de Uso

### Usando cURL

```bash
curl -X GET "https://api.urbanai.com/api/v1/categories"
```

### Usando JavaScript (Fetch)

```javascript
fetch('https://api.urbanai.com/api/v1/categories', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(categories => {
  console.log('Categorías disponibles:', categories);
  
  // Ejemplo: Mostrar en un select
  categories.forEach(category => {
    console.log(`Categoría: ${category.name}`);
    category.subcategories.forEach(sub => {
      console.log(`  - ${sub.name}`);
    });
  });
})
.catch(error => console.error('Error:', error));
```

### Usando C# (HttpClient)

```csharp
using System.Net.Http;
using System.Net.Http.Json;

var client = new HttpClient();
var response = await client.GetAsync("https://api.urbanai.com/api/v1/categories");

if (response.IsSuccessStatusCode)
{
    var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
    
    foreach (var category in categories)
    {
        Console.WriteLine($"Categoría: {category.Name}");
        foreach (var subcategory in category.Subcategories)
        {
            Console.WriteLine($"  - {subcategory.Name}");
        }
    }
}
```

## Integración con el Flujo de Reportes

Este endpoint se integra con el flujo de creación de incidentes:

1. **Usuario obtiene categorías:** `GET /api/v1/categories`
2. **Usuario selecciona categoría y subcategoría en la UI**
3. **Usuario toma fotografía y envía reporte**
4. **IA de Gemini analiza y confirma/corrige la categoría**
5. **Sistema guarda el incidente con la categoría final**

## Notas Importantes

1. **Endpoint público:** No requiere autenticación, facilitando la integración desde aplicaciones móviles y web.

2. **Datos maestros:** Las categorías y subcategorías son datos maestros del sistema que se crean mediante el endpoint de seed.

3. **Categorización por IA:** Aunque el usuario puede ver y sugerir categorías, el sistema de IA de Gemini tiene la decisión final sobre la categorización del incidente.

4. **Cache recomendado:** Se recomienda implementar cache en el lado del cliente, ya que las categorías no cambian frecuentemente.

5. **Estructura fija:** La respuesta siempre incluye todas las categorías con sus subcategorías. No hay filtrado ni paginación.

## Relación con Otros Endpoints

- **POST /api/v1/incidents** - Usa las categorías al crear un incidente
- **POST /api/v1/incidents/seed-categories** - Crea las categorías iniciales (requiere autenticación)
- **GET /api/v1/incidents** - Retorna incidentes con su categoría asignada
