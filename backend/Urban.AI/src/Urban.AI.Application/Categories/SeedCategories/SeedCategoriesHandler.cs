namespace Urban.AI.Application.Categories.SeedCategories;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class SeedCategoriesHandler : ICommandHandler<SeedCategoriesCommand>
{
    #region Private Members
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubcategoryRepository _subcategoryRepository;
    #endregion

    public SeedCategoriesHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository,
        ISubcategoryRepository subcategoryRepository)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _subcategoryRepository = subcategoryRepository;
    }

    public async Task<Result> Handle(SeedCategoriesCommand request, CancellationToken cancellationToken)
    {
        var existingCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        if (existingCategories.Any())
        {
            return Result.Success();
        }

        var categories = CreateCategories();

        foreach (var category in categories)
        {
            _categoryRepository.Add(category);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private List<Category> CreateCategories()
    {
        var categories = new List<Category>();

        var categoryA = Category.Create(
            "A",
            "Infraestructura Vial y Peatonal",
            "Infraestructura relacionada con vías, andenes y acceso peatonal");

        var subcategoriesA = new List<(string Name, string Description)>
        {
            ("Deterioro de la malla vial (huecos, grietas profundas)", "Daño en superficies viales incluyendo huecos y grietas profundas"),
            ("Puentes peatonales o vehiculares con fallas estructurales", "Puentes con problemas o fallas estructurales"),
            ("Andenes/Veredas rotas o inexistentes (obstáculos para personas con discapacidad)", "Andenes en mal estado o inexistentes, afectando la accesibilidad"),
            ("Falta de tapas de alcantarillado (alto riesgo de accidente)", "Tapas de alcantarillado faltantes o dañadas que representan riesgos de seguridad")
        };

        foreach (var (name, description) in subcategoriesA)
        {
            var subcategory = Subcategory.Create(name, description, categoryA.Id);
            categoryA.AddSubcategory(subcategory);
        }

        categories.Add(categoryA);

        var categoryB = Category.Create(
            "B",
            "Servicios Públicos e Iluminación",
            "Problemas relacionados con servicios públicos e iluminación vial");

        var subcategoriesB = new List<(string Name, string Description)>
        {
            ("Luminarias fundidas o puntos ciegos de oscuridad (focos de inseguridad)", "Luces de calle no funcionales que crean problemas de seguridad"),
            ("Fugas de agua potable o desbordamiento de aguas negras", "Problemas de infraestructura de agua o alcantarillado"),
            ("Cables caídos o postes de energía inclinados/peligrosos", "Peligros de infraestructura eléctrica"),
            ("Acumulación de basuras o ineficiencia en la recolección", "Problemas con la gestión y recolección de residuos")
        };

        foreach (var (name, description) in subcategoriesB)
        {
            var subcategory = Subcategory.Create(name, description, categoryB.Id);
            categoryB.AddSubcategory(subcategory);
        }

        categories.Add(categoryB);

        var categoryC = Category.Create(
            "C",
            "Espacio Público y Mobiliario",
            "Problemas relacionados con espacios públicos, parques y mobiliario urbano");

        var subcategoriesC = new List<(string Name, string Description)>
        {
            ("Parques infantiles o gimnasios biosaludables en mal estado", "Instalaciones recreativas que necesitan mantenimiento o reparación"),
            ("Bancas rotas o señalética vandalizada", "Mobiliario urbano y señalización dañados"),
            ("Grafitis en zonas patrimoniales o edificios públicos", "Vandalismo que afecta sitios patrimoniales o propiedad pública"),
            ("Ocupación indebida del espacio público (vendedores sin permiso que bloquean paso, terrazas ilegales)", "Uso ilegal o inadecuado de espacios públicos")
        };

        foreach (var (name, description) in subcategoriesC)
        {
            var subcategory = Subcategory.Create(name, description, categoryC.Id);
            categoryC.AddSubcategory(subcategory);
        }

        categories.Add(categoryC);

        var categoryD = Category.Create(
            "D",
            "Medio Ambiente y Riesgo",
            "Peligros ambientales y situaciones de riesgo");

        var subcategoriesD = new List<(string Name, string Description)>
        {
            ("Árboles en riesgo de caída o que interfieren con redes eléctricas", "Árboles que representan riesgos de seguridad o afectan infraestructura"),
            ("Puntos de contaminación auditiva excesiva (zonas rosas fuera de control)", "Áreas con niveles de ruido problemáticos"),
            ("Vertimientos ilegales en cuerpos de agua", "Contaminación ambiental de fuentes de agua")
        };

        foreach (var (name, description) in subcategoriesD)
        {
            var subcategory = Subcategory.Create(name, description, categoryD.Id);
            categoryD.AddSubcategory(subcategory);
        }

        categories.Add(categoryD);

        var categoryE = Category.Create(
            "E",
            "Movilidad y Tránsito",
            "Problemas relacionados con tráfico y movilidad");

        var subcategoriesE = new List<(string Name, string Description)>
        {
            ("Semáforos averiados", "Señales de tráfico no funcionales"),
            ("Señales de tránsito ocultas por vegetación o caídas", "Señales de tránsito oscurecidas o dañadas"),
            ("Vehículos mal estacionados bloqueando rampas de acceso", "Violaciones de estacionamiento que afectan la accesibilidad")
        };

        foreach (var (name, description) in subcategoriesE)
        {
            var subcategory = Subcategory.Create(name, description, categoryE.Id);
            categoryE.AddSubcategory(subcategory);
        }

        categories.Add(categoryE);

        return categories;
    }
}
