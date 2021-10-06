namespace Inshapardaz.Domain.Common
{
    public static class EmailTemplateProvider
    {
        public static string GetLibraryAdminInvitationEmail(string libraryName, string invitationLink)
        {
            return string.Format($"Hi, Welcome to your library {libraryName}. Please click on this <a href={invitationLink}>link to activate your account</a>.");
        }

        public static string GetLibraryUserInvitationEmail(string name, string libraryName, string invitationLink)
        {
            return string.Format($"Hi {name}, You are invited to join {libraryName}. Please click on this <a href={invitationLink}>link to activate your account</a>.");
        }

        public static string GetSuperAdminInvitationEmail(string name, string invitationLink)
        {
            return string.Format($"Hi {name}, You are invited to join team of our administrators. Please click on this <a href={invitationLink}>link to activate your account</a>.");
        }

        internal static string GetResetPasswordEmail(string name, string invitationLink)
        {
            return string.Format($"Hi {name}, You have recently requested to reset your password. Please click on this <a href={invitationLink}>link to reset your account</a>. This link is valid for one day.");
        }
    }
}
