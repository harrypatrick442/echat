using Core.Exceptions;
using Core.Timing;
using Logging;
using Shutdown;
using System.IO;
using System.Timers;
using Timer = System.Timers.Timer;
namespace MultimediaServerCore
{
    public sealed class MultimediaDeletesProcessor
    {
        private const int MAX_N_ENTRIES_PER_BATCH = 50;
        private static MultimediaDeletesProcessor? _Instance;
        public static MultimediaDeletesProcessor Initialize()
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(MultimediaDeletesProcessor));
            _Instance = new MultimediaDeletesProcessor();
            return _Instance;
        }
        public static MultimediaDeletesProcessor Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(MultimediaDeletesProcessor));
                return _Instance;
            }
        }
        private DalMultimediaDeletes _DalMultimediaDeletes;
        private Timer _Timer;
        private MultimediaDeletesProcessor() {
            _DalMultimediaDeletes = DalMultimediaDeletes.Instance;
            _Timer = new Timer();
            _Timer.Interval = GlobalConstants.Intervals.PROCESS_DELETE_MULTIMEDIAS;
            _Timer.AutoReset = true;
            _Timer.Elapsed += DoDeletes;
            _Timer.Enabled = true;
            _Timer.Start();
            ShutdownManager.Instance.Add(_Timer.Stop, ShutdownOrder.MultimediaServerMesh);
        }
        public void Add(MultimediaToken multimediaToken)
        {
            string[] filePaths = MultimediaTokenHelper.GetFilePaths(multimediaToken);
            long now = TimeHelper.MillisecondsNow;
            foreach (string filePath in filePaths) {
                DalMultimediaDeletes.Instance.AddPending(new PendingMultimediaDelete(filePath, now));
            }
        }
        private void DoDeletes(object sender, ElapsedEventArgs e) {
            while (true)
            {
                if (ShutdownManager.Instance.CancellationToken.IsCancellationRequested) return;
                PendingMultimediaDelete[] pendingMultimediaDeletes = _DalMultimediaDeletes.GetNextPendings(MAX_N_ENTRIES_PER_BATCH);
                if (pendingMultimediaDeletes.Length <= 0)
                {
                    return;
                }
                foreach (PendingMultimediaDelete pendingMultimediaDelete in pendingMultimediaDeletes)
                {
                    try
                    {
                        string filePath = pendingMultimediaDelete.FilePath;
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        string parentDirectoryPath = Path.GetDirectoryName(filePath);
                        var folder = new DirectoryInfo(parentDirectoryPath);
                        if (DeleteFolderIfEmpty(folder)) {
                            DeleteFolderIfEmpty(new DirectoryInfo(Path.GetDirectoryName(parentDirectoryPath)));
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            _DalMultimediaDeletes.AddFailed(pendingMultimediaDelete);
                        }
                        catch (Exception ex2)
                        {
                            Logs.Default.Error(ex2);
                            _Timer.Stop();
                            return;
                        }
                    }
                    _DalMultimediaDeletes.DeletePending(pendingMultimediaDelete.Id);
                }
            }
        }
        private bool DeleteFolderIfEmpty(DirectoryInfo folder) {
            if (!folder.Exists) 
                return false;
            if (folder.GetFileSystemInfos().Length > 0)
                return false;
            try 
            { 
                folder.Delete();
            }
            catch { }
            return true;
        }
    }
}