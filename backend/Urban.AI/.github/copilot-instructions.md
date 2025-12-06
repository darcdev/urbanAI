# Arquitectura de referencia para las capas de clean architecture en .NET 8

Variables para esta arquitectura:
    - [Database]: PostgreSQL

---

## General
1. **Tecnologías**
- Lenguaje: C#
- Framework: ASP .NET 8
- Patrón de diseño: CQRS y Mediator
- Base de datos: [Database]

2. **Reglas de arquitectura**
- Todo el código debe estar en inglés
- Usa namespace con ambito de archivo
- Evita el hardcoding, utiliza constantes dentro de la clase
- Usa la incidencia de patrones para las comparaciones de condicionales
- Usa recursos cuando sea necesario
- Siempre que uses un recurso valida el idioma EN, ES: [EntityName]Resources.resx o [EntityName]Resources.es.resx
- Redodea fragmentos de código con region para Usings, Constants, Private Members
- Las capas tiene las siguientes dependencias: WebApi -> Application -> Infrastructure -> Domain

---

## Domain Layer
- Dentro de la capa Company.NameApp.Domain en la carpeta de la entidad generar la entidad con una estructura como la siguiente:
    ```csharp
    public class [EntityName]: : Entity
    {
        region Constants
        ...
        endregion

        private [EntityName]() {} // This empty constructor is mandatory for EF in classes, but without this comment
    }
    ```

- Los objetos de valor deben seguir la siguiente estructura:
    ```csharp
    public class [NameValueObject]
    {
    }
    ```

- Consideraciones para la entidad y objetos de valor:
    - Todos los atributos de la entidad u objeto de valor deben tener el set privado
    - El constructor no debe tener mas que la inicialización de las listas necesarias
    - No debe crear los setters y los getters para los atributos, solo puedes crear los setters que sean estrictamente necesarios.
    - No colocar documentación dentro de la clase, solo sobre la entidad

- Crea los archivos de recursos en ingles y español para cada entidad dentro de su propia carpeta: /[EntityName]/Resources
- Para definir los errores, uso los archivos de recursos de la siguiente forma:
    ```csharp
    public static class [EntityName]Errors
    {
        public static readonly Error NoAvailable = new(
            nameof([EntityName]Resources.[EntityName]NoAvailable),
            [EntityName]Resources.[EntityName]NoAvailable);

        public static Error EmailAlreadyExists(string email) => new(
            nameof([EntityName]Resources.[EntityName]EmailAlreadyExists),
            string.Format([EntityName]Resources.[EntityName]EmailAlreadyExists, email));
        
        ...
    }
    ```

---

## Infrastructure Layer

## Si importar la base de datos debe cumplir con:
- Usar MongoDB.Driver
- Debe generar los mappings de los objetos de valor.
- Crea las clases mappings como internal sealed.
- Dentro de la capa Company.NameApp.Domain en la carpeta de la entidad genera la interfaz del repositorio.
- Nombre del archivo: I[EntityName]Repository.cs
- Hereda de la interfaz genérica Company.NameApp.Domain.Abstractions.IRepository que contiene los métodos genéricos del CRUD: IRepository<[EntityName]>
- Dentro de Company.NameApp.Infrastructure.Database /Repositories debe generar el repositorio siguiendo las siguientes convenciones: 
- Nombre del archivo: [EntityName]Repository.cs
- No cree los metodos CRUD. En su lugar hereda de Repository<[EntityName]>( IMongoDatabase database ) o Repository<[EntityName]>( ApplicationDbContext dbContext ) que contiene los metodos CRUD
- Hereda de I[EntityName]Repository.cs e implementa sus métodos de ser necesario.
- Inyecta el repositorio como AddTransient en DependencyInjection.cs en el metodo AddRepositories( this IServiceCollection services ) ubicado en el namespace Company.NameApp.Infrastructure.DependencyInjection.
- Crea los repositorios como internal sealed.
- Usa el CancellationToken para propagar a los métodos asíncronos.

### Si usa MongoDB
- Dentro de Company.NameApp.Infrastructure.Database /Mappings/[EntityNameFolder] debe generar el mapping de entidad de dominio hacia la DB siguiendo las siguientes convenciones:
- Nombre del archivo: [EntityName]Mapping.cs
    ```csharp
    namespace Company.NameApp.Infrastructure.MongoDB.Mappings;

    public class [EntityName]Mapping
    {
        public [EntityName]Mapping()
        {
            if( BsonClassMap.IsClassMapRegistered( typeof( [EntityName] ) ) )
            {
                return;
            }

            BsonClassMap.RegisterClassMap<[EntityName]>( map =>
            {
                map.MapMember( [EntityName] => [EntityName].Property1 ).SetElementName( "property1" );
                map.MapMember( [EntityName] => [EntityName].Property2 ).SetElementName( "property2" );
                ...

                map.SetIgnoreExtraElements( true );
            } );
        }
    }
    ```
- Para los enums usa el siguiente serializador: .SetSerializer( new EnumSerializer<EnumType>( BsonType.String ) )
    
### Si usa una base de datos relacional

- Dentro de la capa Company.NameApp.Infrastructure.Database /Mappings/[EntityNameFolder] debe generar el mapping de entidad de dominio hacia la DB siguiendo las siguientes convenciones:
- Nombre del archivo: [EntityName]Mapping.cs
- No uses el .HasColumnName() para configurar un nombre
    ```csharp
    namespace Company.NameApp.Infrastructure.Database.Mappings;

    public class [EntityName]Mapping : IEntityTypeConfiguration<[EntityName]>
    {
        region Constants
        private const string EntityNameTableName = "[EntityName]s";
        private const int PropertyName1MaxLength = 100;
        private const int PropertyName2MaxLength = 500;
        endregion

        public void Configure( EntityTypeBuilder<[EntityName]> builder )
        {
            builder.ToTable( EntityNameTableName );

            builder.HasKey( e => e.Id );

            builder.Property( e => e.PropertyName1 ).IsRequired();
            builder.Property( e => e.PropertyName2 ).IsRequired();

            ...
        }
    }
    ```

---

## Application Layer
- Usa los repositorios necesarios que estarán en Company.NameApp.Infrastructure.Database /Repositories llamando [EntityName]Repository.cs mediante la interfaz I[EntityName]Repository.cs que se encuentra en Company.NameApp.Domain en la carpeta de la entidad.
    - Usa camelCase para el nombramiento de las propiedades privadas
    - Nombre las propiedades privadas con nombres que hagan referencia de la misma, es decir que sean dicientes.

- Usa la siguiente estructura de carpetas para los casos de uso:
    ```
    ├───Company.NameApp.Application
    │   ├───[EntityName1]s
    │   │   ├───Dtos
    │   │   ├───Get...
    │   │   ├───Create...
    │   │   ├───Search...
    │   │   ├───Request...
    │   │   ├───Update...
    │   │   └───Delete...
    │   ├───[EntityName1]Extensions.cs
    │   │
    │   ├───[EntityName2]s
    ```

- Cada entidad que agrupa sus casos de uso tendrá su propia carpeta de Dtos.
- Cada caso de uso tendrá los siguientes archivos: `[UseCase]Handler.cs`, `[UseCase]Command.cs` o `[UseCase]Query.cs` y `[UseCase]InputValidation.cs` de ser necesario.
- Usa FluentValidation para las validaciones de entrada y hereda de : `AbstractValidator<[UseCase]Command>`, las cuales ya se ejecutan por medio de validación en pipeline.
- Todos los textos de validación deben estar en los archivos de recursos dentro de la carpeta Resources de la entidad correspondiente.
- No uses el sufijo "Dto" en los nombres de las clases, en su lugar usa request, response o si sufijo según corresponda.
- Cada caso de uso tendrá su propio clase estatica de extensiones, donde contendrá metodos como:
    ```csharp
    private static Dtos.[DtoName] ToDto(this [EntityName] entityName)
    private static [EntityName] ToDomain(this Dtos.[DtoName] dtoName)
    ```
    - Tendrá métodos de extensión para convertir entre Dtos, entidades de dominios o tambien puede usar nombres con referencia a su contexto, ejemplo: Complete...,  CreateTo..., GroupBy... 

- Para crear los commands o queries usa clase o records según convenga y hereda de la interfa IQuery o ICommand que viene de `Company.NameApp.Application.Common.Abstractions.CQRS`
    - Para un Query crealo de la siguiente manera: `public record [UseCase]Query(TypeParam param) : IQuery<TypeResponse>;`
    - Para un Command crealo de la siguiente manera: `public class [UseCase]Command(Dtos.[NameRequest] request) : ICommand<Guid>;` debería devolver el Id de la entidad creada o nada.

- El handler debe retornar un `Task<Result<TypeResponse>>`, este patron resultado nos permite manejar las respuestas de las siguiente manera:
    - Ingresar errores: `return Result.Failure<TypeResponse>(CircleErrors.UserNotMemberOfCircle);`
    - Ingresar resultados: `return Result.Success(DataReponse);`
    - Ingresar resultados vacios: `return Result.Success();`

- Todos los handler deben ser internal sealed.
- No usa try catch en los handlers
- Usa las validaciones minimas necesarias en los handlers
- Los casos de uso serán llamados desde el controlador por medio de sus Commands o Queries
- No solicite algunos datos de usuario en los Dtos de request, en su lugar inyecta el servicio `IUserContext` para cuando requieras acceder a datos de usuario como su ID, Email, etc. Este servicio ya trae eso datos del token.
- Usa el CancellationToken para propagar a los método asíncronos.
- Usa el patrón UnitOfWork ejecutando `await _unitOfWork.SaveChangesAsync(cancellationToken);` donde corresponda.
- Nunca coloques comentarios en el código para explicar qué hace el código, porque el código debe ser autoexplicativo.

---

## Web API Layer
- El controlador debe heredar de ControllerBase
- Debe usar los siguientes decoradores en el controller:
    - `[Authorize]` // Si requiere autenticación
    - `[ApiController]`
    - `[ApiVersion(ApiVersions.V1)]`
    - `[Route("api/v{version:apiVersion}/[NameController]")]`

- Inyecta el `ISender` en el controlador para enviar Commands y Queries.
- Los métodos del controller deben llevar su respectivo decorador de verbo
- Los metodos del controller deben retornar un Task<IActionResult>
- Cada controlador debería tener validar el patron resultado antes de retornar algo:
    ```csharp
    if (result.IsFailure)
    {
        return BadRequest(result.Error);
    }

    return Created(result.Value);
    ```

---

3. **Otras consideraciones**
- Realizar control de excepciones
- Utiliza Clean Code
- Utiliza SOLID
- Crea una API REST

## Genera el código en C#
- Genera todo el codigo requerido para usar en cada capa.