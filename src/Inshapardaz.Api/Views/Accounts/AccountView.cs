using System;

namespace Inshapardaz.Api.Views.Accounts
{
    public class AccountView : ViewWithLinks
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
    }
}
