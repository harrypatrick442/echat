using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalConstants
{
    public class Parameters
    {
        [ExportToJavaScript]
        public const string MUST_ESCAPE_WEBVIEW = "f";
        [ExportToJavaScript]
        public const string SECRET = "s";
        [ExportToJavaScript]
        public const string TOKEN = "t";
        [ExportToJavaScript]
        public const string CONVERSATION_ID = "r";
        [ExportToJavaScript]
        public const string USER_ID = "u";
        [ExportToJavaScript]
        public const string ROOM_PASSWORD = "p";
        [ExportToJavaScript]
        public const string SESSION_ID = "si";
        [ExportToJavaScript]
        public const string INTERSERVER_AUTHENTICATION_TOKEN = "iat";
        [ExportToJavaScript]
        public const string MULTIMEDIA_TOKEN = "m";
    }
}
