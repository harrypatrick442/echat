using NodeAssignedIdRangesCore.Requests;

namespace UserIgnore
{
    public sealed partial class UserIgnoresMesh
    {
        private UserIgnores GetUserIgnores_Here(long userId)
        {
            return DalUserIgnoresLocal.Instance.GetUserIgnores(userId);
        }
        private void AddUserIgnore_Here(long userIdIgnoring, long userIdBeingIgnored)
        {
            DalUserIgnoresLocal.Instance.AddUserIgnore(userIdIgnoring, userIdBeingIgnored);
        }
        private void RemoveUserIgnore_Here(long userIdUnignoring, long userIdBeingUnignored)
        {
            DalUserIgnoresLocal.Instance.RemoveUserIgnore(userIdUnignoring, userIdBeingUnignored);
        }
        private void AddBeingIgnoredBy_Here(long userIdIgnoring, long userIdBeingIgnored)
        {
            DalUserIgnoresLocal.Instance.AddBeingIgnoredBy(userIdIgnoring, userIdBeingIgnored);
        }
        private void RemoveBeingIgnoredBy_Here(long userIdUnignoring, long userIdBeingUnignored)
        {
            DalUserIgnoresLocal.Instance.RemoveBeingIgnoredBy(userIdUnignoring, userIdBeingUnignored);
        }
    }
}