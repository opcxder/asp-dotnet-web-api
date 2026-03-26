using System.Security.Cryptography;

namespace Basic.Service
{
    public class AuthService
    {
       
      public static RSA LoadRsaKey()
        {
            string  rsaKeyPath = "Keys/private_key.pen";
            var rsa = RSA.Create();

            if (!File.Exists(rsaKeyPath))
            {
                throw new FileNotFoundException("Keys file not found ", rsaKeyPath);
            }
            var pemContent = File.ReadAllText(rsaKeyPath);
            rsa.ImportFromPem(pemContent.ToCharArray());

            return rsa;

        }



    }
}
