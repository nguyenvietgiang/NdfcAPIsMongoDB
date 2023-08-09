namespace NdfcAPIsMongoDB.Common.EmailService
{
    public interface IEmailService
    {
        void SendEmail(string mail, string bodyString);
    }
}
