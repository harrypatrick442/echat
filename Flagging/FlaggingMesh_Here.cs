using Flagging.DAL;
using Flagging.Messages.Requests;
using Logging;

namespace Flagging
{
    public partial class FlaggingMesh
    {
        public bool Flag_Here(FlagRequest flagRequest)
        {
            try
            {
                DalFlaggingLocal.Instance.Append(flagRequest);
                return true;
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                return false;
            }
        }
    }
}