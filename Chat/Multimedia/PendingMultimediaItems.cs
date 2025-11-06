using MultimediaCore;
using MultimediaServerCore.Enums;
namespace MultimediaServerCore
{
    public partial class PendingMultimediaItems
    {
        private bool _Disposed = false;
        private List<UserMultimediaItem> _PendingUserMultimediaItems;
        public PendingMultimediaItems()
        {

        }
        public void Delete(string multimediaToken)
        {
            lock (this)
            {
                if (_PendingUserMultimediaItems == null) return;
                UserMultimediaItem item = _PendingUserMultimediaItems
                    .Where(i => i.MultimediaToken == multimediaToken)
                    .FirstOrDefault();
                if (item == null)
                    return;
                _PendingUserMultimediaItems.Remove(item);
                MultimediaServerMesh.Instance.Delete(multimediaToken);
            }
        }
        public void AddPendingMultimediaItemAndOverflow(UserMultimediaItem userMultimediaItem)
        {
            lock (this)
            {
                if (_Disposed)
                {
                    MultimediaServerMesh.Instance.Delete(userMultimediaItem.MultimediaToken);
                    return;
                }
                if (_PendingUserMultimediaItems == null)
                {
                    _PendingUserMultimediaItems = new List<UserMultimediaItem> { userMultimediaItem! };
                    return;
                }
                _PendingUserMultimediaItems.Add(userMultimediaItem!);
                int nPendingToRemove = _PendingUserMultimediaItems.Count() - Configurations.Lengths.MAX_N_MULTIMEDIA_ITEMS_PER_MESSAGE;
                if (nPendingToRemove > 0)
                {
                    foreach (UserMultimediaItem toRemove in _PendingUserMultimediaItems.Take(nPendingToRemove))
                    {
                        MultimediaServerMesh.Instance.Delete(toRemove.MultimediaToken);
                    }
                    _PendingUserMultimediaItems.RemoveRange(0, nPendingToRemove);
                }
            }
        }
        public void UpdateMultimediaItemStatus(string multimediaToken, MultimediaItemStatus status) {
            lock (this) {
                if (_PendingUserMultimediaItems == null) return;
                UserMultimediaItem matching = _PendingUserMultimediaItems
                    .Where(p => p.MultimediaToken == multimediaToken)
                    .FirstOrDefault();
                if (matching == null) return;
                matching.Status = status;
            }
        }
        public UserMultimediaItem[] GetPendingUserMultimediaItems(UserMultimediaItem[] fromRequests)
        {
            lock (this)
            {
                if (_PendingUserMultimediaItems == null) return null;
                if (fromRequests == null)
                {
                    DeleteAllPending();
                    return null;
                }
                List<UserMultimediaItem> matches = new List<UserMultimediaItem>();
                foreach (UserMultimediaItem fromRequest in fromRequests)
                {
                    UserMultimediaItem matching = _PendingUserMultimediaItems
                        .Where(p => p.MultimediaToken == fromRequest.MultimediaToken&&p.Status.Equals(MultimediaItemStatus.Live)    )
                        .FirstOrDefault();
                    if (matching == null)
                        continue;
                    string description = fromRequest.Description;
                    if (description.Length > Configurations.Lengths.MAX_USER_MULTIMEDIA_DESCRIPTION_LENGTH)
                        description = description
                            .Substring(0, Configurations.Lengths.MAX_USER_MULTIMEDIA_DESCRIPTION_LENGTH);
                    matching.Description = description;
                    matches.Add(matching);
                    _PendingUserMultimediaItems.Remove(matching);
                }
                DeleteAllPending();
                return matches.ToArray();
            }
        }
        private void DeleteAllPending()
        {
            if (_PendingUserMultimediaItems == null) return;
            foreach (UserMultimediaItem toRemove in _PendingUserMultimediaItems)
            {
                MultimediaServerMesh.Instance.Delete(toRemove.MultimediaToken);
            }
            _PendingUserMultimediaItems.Clear();
        }
        public void Dispose()
        {
            lock (this)
            {
                if (_Disposed) return;
                _Disposed = true;
                DeleteAllPending();
            }
        }
    }
}