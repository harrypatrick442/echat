using Core.Exceptions;
using Core.Maths;
using Nodes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

internal class NetworkPair
{
    public NetworkJSONEntry A { get; }
    public NetworkJSONEntry B { get; }
    private NetworkJSONEntry _Client, _Server;
    private Existing _Existing;
    public bool BSeenA { get; private set; }
    private string _Password, _Password2;
    private string _Hash, _Hash2;
    public NetworkPair(NetworkJSONEntry a, NetworkJSONEntry b, Existing existing) {
        A = a;
        B = b;
        _Existing = existing;
        if (!B.HasDomain)
        {
            if (A.HasDomain)
            {
                _Client = B;
                _Server = A;
            }
            else
                throw new ParseException($"Neither node {a.Id} or {b.Id} have a domain!");
        }
        else
        {
            _Client = A;
            _Server = B;
        }
    }
    public void BHasSeenA() {
        BSeenA = true;
    }
    public string GetHash(NetworkJSONEntry me)
    {
        GeneratePasswordHashIfNotAlready();
        return IAmClientElseServer(me) ? _Hash2 : _Hash;
    }
    public string GetPassword(NetworkJSONEntry me){
        GeneratePasswordHashIfNotAlready();
        return IAmClientElseServer(me) ? _Password : _Password2;
    }
    public string GetServerUrl(NetworkJSONEntry me)
    {
        return IAmClientElseServer(me) ? $"wss://{GlobalConstants.Nodes.FirstUniqueDomainForNode(_Server.Id)}:8443" : null;
    }
    public string GetExpectedIpAddressOfClient(NetworkJSONEntry me)
    {
        return null;
    }
    public bool IAmClientElseServer(NetworkJSONEntry me) {
        return me.Equals(_Client);
    }
    private void GeneratePasswordHashIfNotAlready() {
        if (_Password != null) return;
        InterserverConnection existingInterserverConnectionOnClientNode = _Existing.GetExistingInterserverConnection(_Client.Id, _Server.Id);
        InterserverConnection existingInterserverConnectionOnServerNode = _Existing.GetExistingInterserverConnection(_Server.Id, _Client.Id);
        if (existingInterserverConnectionOnClientNode?.Password != null && existingInterserverConnectionOnServerNode?.Hash != null
            && existingInterserverConnectionOnClientNode?.Hash != null && existingInterserverConnectionOnServerNode?.Password != null)
        {
            _Password = existingInterserverConnectionOnClientNode.Password;
            _Hash = existingInterserverConnectionOnServerNode.Hash;
            _Password2 = existingInterserverConnectionOnServerNode.Password;
            _Hash2 = existingInterserverConnectionOnClientNode.Hash;
            return;
        }
        int passwordLength = RandomHelper.NextInt(16, 24);
        _Password = RandomHelper.RandomString(passwordLength);
        _Hash = BCrypt.Net.BCrypt.HashPassword(_Password);
         passwordLength = RandomHelper.NextInt(16, 24);
        _Password2 = RandomHelper.RandomString(passwordLength);
        _Hash2 = BCrypt.Net.BCrypt.HashPassword(_Password2);
    }
}
