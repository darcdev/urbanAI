namespace Urban.AI.Application.Categories.Dtos;

public sealed record CategoryResponse(
    Guid Id,
    string Code,
    string Name,
    string Description,
    List<SubcategoryResponse> Subcategories);
