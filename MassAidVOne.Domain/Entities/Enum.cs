namespace MassAidVOne.Domain.Entities
{
    public static class Enum
    {
        public static class UserType
        {
            public const string Admin = "Admin";
            public const string MessManager = "MessManager";
            public const string User = "User";
        }

        public static class OtpActivityType
        {
            public const string VerifyEmail = "VerifyEmail";
            public const string ForgetPassword = "ForgetPassword";
        }

        public static class OtpStatus
        {
            public const string Active = "Active";
            public const string Used = "Used";
            public const string Expired = "Expired";
        }

        public static class FileUploadPath
        {
            public const string User = "User";

        }
    }
}
