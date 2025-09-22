using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Core.FileSystem;

namespace Snippets.Core.ImageProcessing
{
    public class TempSafeImage:SafeImage, IDisposable{
        private object _LockObjectDispose = new object();
        private bool _Disposed = false;
        private volatile bool _SafeCleanupDisabled = false;
        private TemporaryFile _TemporaryFile;
        public TemporaryFile TemporaryFile { get { return _TemporaryFile; } }
        protected TempSafeImage(TemporaryFile temporaryFile):base(temporaryFile.FilePath) {
            _TemporaryFile = temporaryFile;
            Directory.CreateDirectory(Path.GetDirectoryName(temporaryFile.FilePath));
        }
        public static TempSafeImage New()
        {
            TemporaryFile temporaryFile = new TemporaryFile(".png");
            return new TempSafeImage(temporaryFile);
        }
        /*
        public static TempSafeImage New(System.Dr awing.Image image)
        {
            TemporaryFile temporaryFile = new TemporaryFile(".png");
            image.Save(temporaryFile.FilePath);
            return new TempSafeImage(temporaryFile);
        }*/
        public static TempSafeImage New(Image<Rgba32> image)
        {
            TemporaryFile temporaryFile = new TemporaryFile(".png"); 
            image.Save(temporaryFile.FilePath);
            return new TempSafeImage(temporaryFile);
        }
        public void DisableSafeCleanup() {
            _SafeCleanupDisabled = true;
        }
        /*public new void UsingImage(Action<System.Draw ing.Bitmap, Action> callback)
        {
            lock (_LockObjectDispose)
            {
                CheckNotDisposed();
                base.UsingImage(callback);
            }
        }
        public new TReturn UsingImage<TReturn>(Func<System.Dr awing.Bitmap, Action, TReturn> callback)
        {
            lock (_LockObjectDispose)
            {
                CheckNotDisposed();
                return base.UsingImage(callback);
            }
        }*/
        public new void UsingImageSharp(Action<Image<Rgba32>, Action> callback)
        {
            lock (_LockObjectDispose)
            {
                CheckNotDisposed();
                base.UsingImageSharp(callback);
            }
        }
        public new TReturn UsingImageSharp<TReturn>(Func<Image<Rgba32>, Action, TReturn> callback)
        {
            lock (_LockObjectDispose)
            {
                CheckNotDisposed();
                return base.UsingImageSharp(callback);
            }
        }
        private void CheckNotDisposed() {
            if (_Disposed) throw new ObjectDisposedException(nameof(SafeImage));
        }
        public void SaveAs(string filePathSaveAs, bool overwrite = true) {
            File.Copy(_TemporaryFile.FilePath, filePathSaveAs, overwrite);
        }
        ~TempSafeImage() {
            Dispose();
        }
        public void Dispose() {
            lock (_LockObjectDispose) {
                if (_Disposed)
                    return;
                _Disposed = true;
                if (_SafeCleanupDisabled) return;
                _TemporaryFile?.Dispose();
            }
        }
    }
}