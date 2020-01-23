﻿using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddCategoryRequest : RequestBase
    {
        public AddCategoryRequest(CategoryModel category)
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
            command.Result = await _categoryRepository.AddCategory(command.Category, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}