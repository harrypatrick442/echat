namespace NodeAssignedIdRanges
{
    public interface IIdRangesManagerForIdType
    {
        //CHECKED
        int IdType { get; }

        IdRange AssignNextIdRangeToNode(int nodeId);
        NodeIdRanges[] GetIdRangesAssignedToNodes();
    }
}