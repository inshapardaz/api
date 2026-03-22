using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.Options;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library;

public class AddLibraryRequest(LibraryModel library) : RequestBase
{
    public LibraryModel Library { get; } = library;

    public LibraryModel Result { get; set; }
}

public class AddLibraryRequestHandler(
    ILibraryRepository libraryRepository,
    IAccountRepository accountRepository,
    ISendEmail emailService,
    IOptions<Settings> settings)
    : RequestHandlerAsync<AddLibraryRequest>
{
    private readonly Settings _settings = settings.Value;

    [AuthorizeAdmin(1)]
    public override async Task<AddLibraryRequest> HandleAsync(AddLibraryRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await libraryRepository.AddLibrary(command.Library, cancellationToken);

        var account = await accountRepository.GetAccountByEmail(command.Library.OwnerEmail, cancellationToken);
        if (account != null)
        {
            await libraryRepository.AddAccountToLibrary(command.Result.Id, account.Id, Role.LibraryAdmin, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }

        var invitationCode = Guid.NewGuid().ToString("N");

        await accountRepository.AddInvitedAccount(
            command.Library.Name,
            command.Library.OwnerEmail,
            Role.LibraryAdmin,
            invitationCode,
            DateTime.Today.AddDays(7),
            command.Result.Id,
            cancellationToken);

        await emailService.SendAsync(command.Library.OwnerEmail,
                $"Welcome to {command.Result.Name}",
                EmailTemplateProvider.GetLibraryAdminInvitationEmail(command.Result.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.RegisterPagePath + invitationCode).ToString()),
                _settings.Email.EmailFrom,
                cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
