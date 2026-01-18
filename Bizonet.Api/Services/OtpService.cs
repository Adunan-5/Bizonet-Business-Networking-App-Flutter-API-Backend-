namespace Bizonet.Api.Services
{
    public interface IOtpService
    {
        string GenerateOtp(int length = 6);
    }

    public class OtpService : IOtpService
    {
        public string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = random.Next(0, (int)Math.Pow(10, length));
            return otp.ToString().PadLeft(length, '0');
        }
    }
}
