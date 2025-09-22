using JSON;
using Logging;
using System;

namespace Organiser.WebsocketServers
{
    public class IdServerSnippetsWebsocketServer
    {
        static IdServerSnippetsWebsocketServer()
        {
            //AlarmDatabase.Instance.OnSnippetAlarm += OnSnippetAlarm;
        }
        private static TObject DeserializeContent<TObject>(string content) where TObject:class {
            if (string.IsNullOrEmpty(content)) return null;
            try
            {
                return Json.Deserialize<TObject>(content);
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                return null;
            }
        }
    }
}
