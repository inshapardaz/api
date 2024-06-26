﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Category;

public class GetCategoriesQuery : LibraryBaseQuery<IEnumerable<CategoryModel>>
{
    public GetCategoriesQuery(int libraryId)
        : base(libraryId)
    {
    }
}

public class GetCategoryQueryHandler : QueryHandlerAsync<GetCategoriesQuery, IEnumerable<CategoryModel>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<IEnumerable<CategoryModel>> ExecuteAsync(GetCategoriesQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        return await _categoryRepository.GetCategories(command.LibraryId, cancellationToken);
    }
}
