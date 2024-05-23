using Inshapardaz.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories;

public interface IAccountRepository
{
    Task<int> AddInvitedAccount(string name, string email, Role role, string invitationCode, DateTime invitationCodeExpiry, int? libraryId, CancellationToken cancellationToken);

    Task<Page<AccountModel>> GetAccounts(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<Page<AccountModel>> FindAccounts(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);


    Task<Page<AccountModel>> GetAccountsByLibrary(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<Page<AccountModel>> FindAccountsByLibrary(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<IEnumerable<AccountModel>> GetWriters(int libraryId, CancellationToken cancellationToken);

    Task<IEnumerable<AccountModel>> FindWriters(int libraryId, string query, CancellationToken cancellationToken);

    Task<AccountModel> GetAccountByInvitationCode(string invitationCode, CancellationToken cancellationToken);

    Task<AccountModel> GetAccountByEmail(string email, CancellationToken cancellationToken);

    Task UpdateInvitationCode(string email, string invitationCode, DateTime invitationCodeExpiry, CancellationToken cancellationToken);

    Task<Role> GetUserRole(int accountId, int libraryId, CancellationToken cancellationToken = default);

    Task<AccountModel> GetAccountById(int accountId, CancellationToken cancellationToken);

    Task<AccountModel> GetLibraryAccountById(int libraryId, int accountId, CancellationToken cancellationToken);

    Task<RefreshTokenModel> GetRefreshToken(string token, CancellationToken cancellationToken);

    Task AddRefreshToken(RefreshTokenModel refreshToken, int accountId, CancellationToken cancellationToken);

    Task RemoveOldRefreshTokens(AccountModel account, int tokenAgeInDays, CancellationToken cancellationToken);

    Task RevokeRefreshToken(string refreshToken, string ipAddress, string newRefreshToken, CancellationToken cancellationToken);

    Task<AccountModel> GetAccountByResetToken(string token, CancellationToken cancellationToken);

    Task UpdateAccount(AccountModel account, CancellationToken cancellationToken);

    #region for migration
    Task AddAccountToLibrary(int libraryId, int accountId, Role role, CancellationToken cancellationToken);
    Task<AccountModel> AddAccount(AccountModel account, CancellationToken cancellationToken);
    #endregion
}
