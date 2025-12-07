namespace Urban.AI.Application.Categories.GetCategories;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Categories.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class GetCategoriesHandler : IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>
{
    #region Private Members
    private readonly ICategoryRepository _categoryRepository;
    #endregion

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryResponse>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllWithSubcategoriesAsync(cancellationToken);

        var categoryResponses = categories
            .Select(c => c.ToResponse())
            .ToList();

        return Result.Success(categoryResponses);
    }
}
