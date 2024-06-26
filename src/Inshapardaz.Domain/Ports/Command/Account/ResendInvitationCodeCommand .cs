﻿using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Microsoft.Extensions.Options;
using Paramore.Brighter;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class ResendInvitationCodeCommand : RequestBase
{
    public ResendInvitationCodeCommand(string email)
    {
        Email = email;
    }

    public string Email { get; }
}

public class ResendInvitationCodeCommandHandler : RequestHandlerAsync<ResendInvitationCodeCommand>

{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ISendEmail _emailService;
    private readonly Settings _settings;

    public ResendInvitationCodeCommandHandler(ILibraryRepository libraryRepository,
        IAccountRepository accountRepository,
        ISendEmail emailService,
        IOptions<Settings> settings)
    {
        _libraryRepository = libraryRepository;
        _accountRepository = accountRepository;
        _emailService = emailService;
        _settings = settings.Value;
    }

    public override async Task<ResendInvitationCodeCommand> HandleAsync(ResendInvitationCodeCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetAccountByEmail(command.Email, cancellationToken);

        if (account != null && !string.IsNullOrWhiteSpace(account.InvitationCode))
        {
            var library = await _libraryRepository.GetLibrariesByAccountId(account.Id, cancellationToken);

            // TODO : Read app name from settings
            var libraryName = library.FirstOrDefault()?.Name ?? "Nawishta";

            var invitationCode = Guid.NewGuid();

            await _accountRepository.UpdateInvitationCode(
                command.Email,
                invitationCode.ToString("N"),
                DateTime.Today.AddDays(+7),
                cancellationToken);

            await _emailService.SendAsync(account.Email,
                $"Welcome to {libraryName}",
                EmailTemplateProvider.GetLibraryUserInvitationEmail(account.Name, libraryName, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath).ToString()),
                _settings.Email.EmailFrom,
                cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
