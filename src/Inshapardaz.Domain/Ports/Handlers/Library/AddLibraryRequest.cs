﻿using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddLibraryRequest : RequestBase
    {
        public AddLibraryRequest(LibraryModel library)
        {
            Library = library;
        }

        public LibraryModel Library { get; }

        public LibraryModel Result { get; set; }
    }

    public class AddLibraryRequestHandler : RequestHandlerAsync<AddLibraryRequest>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly Settings _settings;

        public AddLibraryRequestHandler(ILibraryRepository libraryRepository, IAccountRepository accountRepository, IEmailService emailService, Settings settings)
        {
            _libraryRepository = libraryRepository;
            _accountRepository = accountRepository;
            _emailService = emailService;
            _settings = settings;
        }

        public override async Task<AddLibraryRequest> HandleAsync(AddLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _libraryRepository.AddLibrary(command.Library, cancellationToken);

            var account = await _accountRepository.GetAccountByEmail(command.Library.OwnerEmail, cancellationToken);
            if (account != null)
            {
                await _libraryRepository.AddAccountToLibrary(command.Result.Id, account.Id, Role.LibraryAdmin, cancellationToken);
                return await base.HandleAsync(command, cancellationToken);
            }

            var invitationCode = Guid.NewGuid().ToString("N");

            await _accountRepository.AddInvitedAccount(
                command.Library.Name,
                command.Library.OwnerEmail,
                Role.LibraryAdmin,
                invitationCode,
                DateTime.Today.AddDays(7),
                command.Result.Id,
                cancellationToken);

            await _emailService.SendAsync(command.Library.OwnerEmail,
                    $"Welcome to {command.Result.Name}",
                    EmailTemplateProvider.GetLibraryAdminInvitationEmail(command.Result.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.RegisterPagePath + invitationCode).ToString()),
                    _settings.EmailFrom,
                    cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
