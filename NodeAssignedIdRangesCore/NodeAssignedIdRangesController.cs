using Microsoft.AspNetCore.Mvc;
using Logging;
using JSON;

namespace NodeAssignedIdRanges
{
    public class NodeAssignedIdRangesController : ControllerBase
    {
        public NodeAssignedIdRangesController() : base()
        {

        }
        [HttpGet]
        [Route(Configurations.Routes.ID_SERVER_GET)]
        public IActionResult Get()
        {
            try
            {
                int nodeId = int.Parse(Request.Query["nodeId"]);
                string jsonString = Json.Serialize(IdRangesMesh.Instance.GetNodesIdRangesForAllAssociatedIdTypes_Here(nodeId));
                return new ContentResult() { Content = jsonString };
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                return StatusCode(406);
            }
        }
    }
}