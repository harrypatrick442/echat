using ConfigurationCore;
using DependencyManagement;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[DataContract]
internal class NetworkJSONEntry {

    [JsonPropertyName("id")]
    [JsonInclude]
    [DataMember(Name = "id")]
    public int Id { get; protected set; }
    [JsonPropertyName("to")]
    [JsonInclude]
    [DataMember(Name = "to")]
    public int[] To{ get; protected set; }
    public bool HasDomain { 
        get {
            return DependencyManager.Get<INodesConfiguration>().FirstUniqueDomainForNode(Id) != null;
        }
    }
    protected NetworkJSONEntry() { }
}
