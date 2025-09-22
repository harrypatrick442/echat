using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagement.Constants
{
    public static class TLS
    {
        public const string EMAIL = "admin@onestonesolutions.com";
        public const string CERTIFICATES_DIRECTORY_PATH = "/etc/letsencrypt/live/onestonesolutions/";
        public const string FULL_CHAIN_PATH = CERTIFICATES_DIRECTORY_PATH + "fullchain.pem";
        public const string PRIV_KEY_PATH = CERTIFICATES_DIRECTORY_PATH + "privkey.pem";
    }
}
