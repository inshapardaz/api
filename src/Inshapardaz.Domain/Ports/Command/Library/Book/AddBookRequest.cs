﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class AddBookRequest : LibraryBaseCommand
{
    public AddBookRequest(int libraryId, int? accountId, BookModel book)
    : base(libraryId)
    {
        AccountId = accountId;
        Book = book;
    }

    public int? AccountId { get; }
    public BookModel Book { get; }

    public BookModel Result { get; set; }
}

public class AddBookRequestHandler : RequestHandlerAsync<AddBookRequest>
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ISeriesRepository _seriesRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AddBookRequestHandler(IBookRepository bookRepository, IAuthorRepository authorRepository,
                                 ISeriesRepository seriesRepository, ICategoryRepository categoryRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _seriesRepository = seriesRepository;
        _categoryRepository = categoryRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddBookRequest> HandleAsync(AddBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        IEnumerable<AuthorModel> authors = null;
        if (command.Book.Authors != null && command.Book.Authors.Any())
        {
            authors = await _authorRepository.GetAuthorByIds(command.LibraryId, command.Book.Authors.Select(a => a.Id), cancellationToken);
            if (authors.Count() != command.Book.Authors.Count())
            {
                throw new BadRequestException();
            }
        }

        if (authors == null || authors.FirstOrDefault() == null)
        {
            throw new BadRequestException();
        }

        SeriesModel series = null;
        if (command.Book.SeriesId.HasValue)
        {
            series = await _seriesRepository.GetSeriesById(command.LibraryId, command.Book.SeriesId.Value, cancellationToken);
            if (series == null)
            {
                throw new BadRequestException();
            }
        }

        IEnumerable<CategoryModel> categories = null;
        if (command.Book.Categories != null && command.Book.Categories.Any())
        {
            categories = await _categoryRepository.GetCategoriesByIds(command.LibraryId, command.Book.Categories.Select(c => c.Id), cancellationToken);
            if (categories.Count() != command.Book.Categories.Count())
            {
                throw new BadRequestException();
            }
        }

        command.Result = await _bookRepository.AddBook(command.LibraryId, command.Book, command.AccountId, cancellationToken);

        command.Result.SeriesName = series?.Name;
        command.Result.Authors = authors?.ToList();
        command.Result.Categories = categories?.ToList();

        return await base.HandleAsync(command, cancellationToken);
    }
}
