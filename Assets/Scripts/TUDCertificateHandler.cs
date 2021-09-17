using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

namespace HistocachingII
{
    public class TUDCertificateHandler : CertificateHandler
    {
        // Encoded RSAPublicKey
        // private const string PUB_KEY = "CC845D224C312B18871A16739EEF699615DF8455071C2AEF83E63E589E60833506DE2BF8F7E06D1FD69BB7CC23D00FE5E08B595E55A07BC36B2AF47E8217EB8D5866493460219719DD3FA776AA07E659CBF5DA703F25AF78A66904F18543BF85FD2B60434196F146C3B2093B9FEC7C53CE3B97B5FEBC76CA391632896D10C38819B7B475A447EAF38001649F2D941135A9F912B2C7AF65D2457F63FEDAD12F24BB8B432361C631C69CA009F82E66C40EE0C5D5E75AECB549BEE8B4F344BEDE5A5955D6F5AE569E5BA952A2512749D366D9CCAE0324BBAAD8A0AC101AAC479717AA265B2F2026029733C47AC4988527AABB864B6E960CFF0899B3FE95824CB17275CB7110673BE5318B24AEFF01ACFFE216318A516345F0E87F07EDC22BE1C3B97406EBA0EB20033044651D25B628CA0AD51E6B49554100E487A4D2D67E3A5A6C2BEB38F2886F3CF0CEC024F485AC00B1BC6FDAF4AD57FD610A5C715F7E4DC8C72E6DE353D2A93C57F7DF2D5B086A82BEC7BDD1B04F9FD5F6F3612A755C08E5CBA225938AC3179B1C1C2D5DFBD3C0ED6095CE319BEFCDAE2A73C4A853B9D7A6E8EBD3EBADFE66EA94F2B8E1F4430F9AAA56AE705243216345B62A63573E94A7D54FCD127BE66D3A7949AD50A815FC5A0F7B23F96A3471B5AB7E96FE826E0E8D45781500851036330D2CE2C043B5A8D59CCADFB481B33A27ECA91FB2327D4351A7";

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // X509Certificate2 certificate = new X509Certificate2(certificateData);
            // string pk = certificate.GetPublicKeyString();

            // if (pk.ToLower().Equals(PUB_KEY.ToLower()))
            //     return true;

            // return false;

            return true;
        }
    }
}
