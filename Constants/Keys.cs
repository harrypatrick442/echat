using System;
using System.IO;

namespace GlobalConstants
{
    public class Keys
    {
        public static readonly  string KEY_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "credentials\\Keys\\node1_key2.ppk");
        public static readonly  string KEY_PATH_OPENSSH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "credentials\\Keys\\node1_key_openssh.ppk");

    }
}