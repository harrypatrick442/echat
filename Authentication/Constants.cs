using Microsoft.AspNetCore.Mvc;
using Logging;

namespace Authentication
{
    public static class Constants
    {
        public const int PASSWORD_RESET_MANAGER_STAGED_DICTIONARY_N_STAGES = 3,
            PASSWORD_RESET_MANAGER_STAGED_DICTIONARY_SWITCH_INTERVAL = 19 * 60 * 1000;
    }
}