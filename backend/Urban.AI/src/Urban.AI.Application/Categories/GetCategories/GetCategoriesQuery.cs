namespace Urban.AI.Application.Categories.GetCategories;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Categories.Dtos;
#endregion

public record GetCategoriesQuery : IQuery<List<CategoryResponse>>;
