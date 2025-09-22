using Core.Events;
using MultimediaServerCore.Messages;

namespace MultimediaServerCore
{
    public static class MultimediaUploadsEventSource
    {
        private static List<EventHandler<ItemEventArgs<MultimediaStatusUpdate>>> 
            _MultimediaStatusChanged = new List<EventHandler<ItemEventArgs<MultimediaStatusUpdate>>>();
        public static event EventHandler<ItemEventArgs<MultimediaStatusUpdate>> MultimediaStatusChanged
        {
            add
            {
                lock (_MultimediaStatusChanged)
                {
                    _MultimediaStatusChanged.Add(value);
                }
            }
            remove
            {
                lock (_MultimediaStatusChanged)
                {
                    _MultimediaStatusChanged.Remove(value);
                }
            }
        }
        public static void DispatchStatusUpdate(MultimediaStatusUpdate multimediaUploadUpdate) {
            ItemEventArgs<MultimediaStatusUpdate> e = new ItemEventArgs<MultimediaStatusUpdate>(multimediaUploadUpdate);
            EventHandler<ItemEventArgs<MultimediaStatusUpdate>>[] handlers;
            lock (_MultimediaStatusChanged) {
                handlers = _MultimediaStatusChanged.ToArray();
            }
            foreach (EventHandler<ItemEventArgs<MultimediaStatusUpdate>> handler in handlers) {
                handler.Invoke(null, e);
            }
        }
    }
}