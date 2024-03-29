﻿using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Category
{
    public class AddCategoryRequest : LibraryBaseCommand
    {
        public AddCategoryRequest(int libraryId, CategoryModel category)
            : base(libraryId)
        {
            Category = category;
        }

        public CategoryModel Category { get; }
        public CategoryModel Result { get; set; }
    }

    public class AddCategoryRequestHandler : RequestHandlerAsync<AddCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository;

        public AddCategoryRequestHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<AddCategoryRequest> HandleAsync(AddCategoryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _categoryRepository.AddCategory(command.LibraryId, command.Category, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
