namespace MassAidVOne.Application.Interfaces
{
    public interface IPasswordManagerService
    {
        string HashedPassword(string Password);
        bool VerifyPassword(UserInformation User, string EnteredPassword);
    }
}
