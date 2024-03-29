﻿using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class GetAccountByIdQuery : IQuery<AccountModel>
    {
        public GetAccountByIdQuery(int id)
        {
            Id = id;
        }

        public int? LibraryId { get; set; }
        public int Id { get; private set; }
    }

    public class GetAccountByIdQueryHandler : QueryHandlerAsync<GetAccountByIdQuery, AccountModel>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public override async Task<AccountModel> ExecuteAsync(GetAccountByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            if (query.LibraryId.HasValue) {
                return await _accountRepository.GetLibraryAccountById(query.LibraryId.Value, query.Id, cancellationToken);    
            }
            return await _accountRepository.GetAccountById(query.Id, cancellationToken);
        }
    }
}
