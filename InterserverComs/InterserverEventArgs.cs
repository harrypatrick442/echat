using Core.Interfaces;
using JSON;

namespace InterserverComs
{
    public sealed class InterserverMessageEventArgs : EventArgs,
        ITypedMessage
    {
        public bool Handled { get; set; }
        private INodeEndpoint _EndpointFrom;
        public INodeEndpoint EndpointFrom { get { return _EndpointFrom; } }
        private string _Type;
        public string Type { get{return _Type; } }
        private string _JsonString;
        public string JsonString { get { return _JsonString; } }

        public TPayload Deserialize<TPayload>() {
            return Json.Deserialize<TPayload>(_JsonString);
        }
        public InterserverMessageEventArgs(
            INodeEndpoint endpointFrom,
            string type,
            string jsonString) {
            _EndpointFrom = endpointFrom;
            _Type = type;
            _JsonString = jsonString;
        }
    }
}