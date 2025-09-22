using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace Snippets.Core.ImageProcessing
{
    public class SafeImage
    {
        private string _FilePath;
        public string FilePath { get { return _FilePath; } }
        public SafeImage(string filePath)
        {
            _FilePath = filePath;
        }/*
        public void UsingImage(Action<Bitmap, Action> callback)
        {
            Action save = null;
            Bitmap image = null;
            try
            {
                GC.Collect();
                image = (Bitmap)Bitmap.FromFile(_FilePath);
                save = () =>
                {
                    image.Save(_FilePath);
                };
            }
            catch (FileNotFoundException) { }
            try
            {
                callback(image, save);
            }
            finally
            {
                image?.Dispose();
            }
        }
        public TReturn UsingImage<TReturn>(Func<Bitmap, Action, TReturn> callback)
        {
            Action save = null;
            Bitmap image = null;
            try
            {
                GC.Collect();
                image = (Bitmap)Bitmap.FromFile(_FilePath);
                save = () => image.Save(_FilePath); ;
            }
            catch (FileNotFoundException) { }
            try
            {
                return callback(image, save);
            }
            finally
            {
                image?.Dispose();
            }
        }*/
        public void UsingImageSharp(Action<Image<Rgba32>, Action> callback)
        {
            Action save = null;
            Image<Rgba32> image = null;
            try
            {
                GC.Collect();
                image = LoadImageSharp();
                save = () => image.Save(_FilePath); ;
            }
            catch (FileNotFoundException) { }
            try
            {
                callback(image, save);
            }
            finally
            {
                image?.Dispose();
            }
        }
        public TReturn UsingImageSharp<TReturn>(Func<Image<Rgba32>, Action, TReturn> callback)
        {
            Action save = null;
            Image<Rgba32> image = null;
            try
            {
                //GC.Collect();
                image = LoadImageSharp();
                save = () => image.Save(_FilePath); ;
            }
            catch (FileNotFoundException) { }
            try
            {
                return callback(image, save);
            }
            finally
            {
                image?.Dispose();
            }
        }
        private Image<Rgba32> LoadImageSharp() {
            return Image.Load<Rgba32>(File.ReadAllBytes(_FilePath));
        }
    }
}