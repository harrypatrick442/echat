using NodeAssignedIdRanges;

namespace NodeAssignedIdRangesCore.Interfaces
{
    public interface IIdRangesManager
    {
        //CHECKED
        IIdRangesManagerForIdType ForIdType(int idType);
    }
}