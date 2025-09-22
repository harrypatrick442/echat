using System;
using System.Linq;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using GlobalConstants;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace Snippets.Core.ImageProcessing
{
    public static class ImageProcessing
    {/*
        public static TempSafeImage SubtractWithAlpha(SafeImage safeImage, SafeImage safeImageToSubtract) {
            return safeImage.UsingImageSharp((image, ignore) => {
                return safeImageToSubtract.UsingImageSharp((imageToSubtract, ignore2) => {
                    TempSafeImage tempSafeImageResult = TempSafeImage.New();
                    byte[] pixelBuffer;
                    BitmapData bitmapData = GetSourceDataFromBitmap(image, out pixelBuffer);
                    byte[] pixelBufferToSubtract;
                    BitmapData bitmapDataToSubtract = GetSourceDataFromBitmap(imageToSubtract, out pixelBufferToSubtract);
                    if (bitmapData.Width != bitmapDataToSubtract.Width)
                        throw new InvalidOperationException($"The images had different widths. The first image had width {bitmapData.Width} and the second image had width {bitmapDataToSubtract.Width}");
                    if (bitmapData.Height != bitmapDataToSubtract.Height)
                        throw new InvalidOperationException($"The images had different widths. The first image had width {bitmapData.Height} and the second image had width {bitmapDataToSubtract.Height}");
                    for (int i = 3; i < pixelBuffer.Length; i+=4) {
                      int value =  pixelBuffer[i] - pixelBufferToSubtract[i];
                        if (value < 0) value = 0;
                        pixelBuffer[i] = (byte)value;
                    }
                    Image<Rgba32> result = Image.LoadPixelData<Rgba32>(pixelBuffer, bitmapData.Width, bitmapData.Height);
                    result.Save(tempSafeImageResult.FilePath);
                    return tempSafeImageResult;
                });
            });
        }*/
        public static TempSafeImage ScaleImage(SafeImage safeImage, int? newWidth = null, int? newHeight = null) {
            if (newWidth == null && newHeight == null)
                throw new ArgumentException($"You must provide either a {nameof(newWidth)} or a {nameof(newHeight)} not both. They were both null");
            if(newWidth!=null&&newHeight!=null)
                throw new ArgumentException($"You provided both {nameof(newWidth)} and {nameof(newHeight)}. Only one can be provided");
            return safeImage.UsingImageSharp((image, save) =>
            {
                if (newWidth != null)
                    return ResizeImage(image, (int)newWidth, (int)((int)newWidth / image.Width) * image.Height);
                return ResizeImage(image, (int)((int)newHeight / image.Height) * image.Width, (int)newHeight);
            });
        }
        public static void ResizeImageToFile(string filePath, int width, int height, string filePathTo)
        {
            new SafeImage(filePath).UsingImageSharp((image, save) =>
            {
                image.Mutate(
                        i => i.Resize(width, height));
                image.Save(filePathTo);
            });
        }
        public static TempSafeImage ResizeImage(string filePath, int width, int height)
        {
            return new SafeImage(filePath).UsingImageSharp((image, save) =>
            {
                return ResizeImage(image, width, height);
            });
        }
        public static TempSafeImage ResizeImage(SafeImage safeImage, int width, int height)
        {
            return safeImage.UsingImageSharp((image, save) =>
            {
                return ResizeImage(image, width, height);
            });
        }
        public static void ResizeImageToFile(Image<Rgba32> image, int width, int height, string filePathTo)
        {
            using (Image<Rgba32> destImage = image.Clone())
            {
                destImage.Mutate(
                    i => i.Resize(width, height));
                //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                destImage.Save(filePathTo);
            }
        }
        public static TempSafeImage ResizeImage(Image<Rgba32> image, int width, int height)
        {
            using (Image<Rgba32> destImage = image.Clone())
            {
                destImage.Mutate(
                    i => i.Resize(width, height));
                //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                return TempSafeImage.New(destImage);
            }
        }
        private static Dictionary<int, float[]> _MapSizeToCircleMask = new Dictionary<int, float[]>();

        public static void ClipToCircle(string filePath, int size, string outputFilePath)
        {
            int nBytes = size * size * 4;
            byte[] pixelBytes = new byte[nBytes];
            float[]? mask;
            using (Image<Rgba32> image = Image<Rgba32>.Load<Rgba32>(File.ReadAllBytes(filePath)))
            {
                image.Mutate(
                i => i.Resize(size, size));
                image.CopyPixelDataTo(pixelBytes);
            }
            lock (_MapSizeToCircleMask)
            {
                if (!_MapSizeToCircleMask.TryGetValue(size, out mask))
                {
                    mask = GenerateCircleMask(size);
                    _MapSizeToCircleMask[size] = mask;
                }
            }
            int index = 0;
            int maskIndex = 0;
            while (index < nBytes)
            {
                float alpha = mask[maskIndex++];
                if (alpha == 0)
                {
                    pixelBytes[index++] = 0;
                    pixelBytes[index++] = 0;
                    pixelBytes[index++] = 0;
                    pixelBytes[index++] = 0;
                }
                else
                {
                    index += 3;
                    pixelBytes[index] = (byte)(pixelBytes[index] * alpha);
                    index ++;
                }
            }
            using (Image<Rgba32> final = Image<Rgba32>.LoadPixelData<Rgba32>(pixelBytes, size, size))
            {
                final.Save(outputFilePath);
            }
        }
        private static readonly double ROOT_TWO = Math.Sqrt(2);
        private static readonly double HALF_ROOT_TWO = ROOT_TWO / 2;
        private static float[] GenerateCircleMask(int size)
        {
            float[] mask = new float[size * size];
            float center = size / 2f;
            float radius = center;
            int maskIndex = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    double distanceFromMiddle = Math.Sqrt(Math.Pow(x - center, 2) + Math.Pow(y - center, 2));
                    /*
                    double value;
                    double p = distanceFromMiddle - radius;
                    if (p > 0.0) {
                        if (p < ROOT_TWO)
                        {
                            if (p < HALF_ROOT_TWO)
                            {
                                value = p * p;
                            }
                            else {
                                value = 0.5 +
                                    (0.5 - Math.Pow(
                                        ROOT_TWO - p, 2));
                            }

                        }
                        else
                            value = 0;
                    }
                    else value = 1;
                    if (value > 1) { 
                    
                    }*/
                    float distance = (float)Math.Sqrt(Math.Pow(x - center, 2) + Math.Pow(y - center, 2));

                    // Set alpha value based on distance from center
                    float alpha = 1f;
                    if (distance > radius)
                    {
                        // Vary alpha near the edges to approximate a circle
                        alpha = 1f - (distance - radius) / (size * 0.1f); // Adjust the divisor to control the edge smoothness
                        alpha = Math.Max(alpha, 0); // Ensure alpha doesn't go below 0
                    }
                    mask[maskIndex++] = alpha;
                }
            }
            return mask;
        }

        /*
        public static Tuple<int, int> GetImageSize(string imageFilePath) {
            using (Image<Rgba32> image = Image<Rgba32>.Load(File.ReadAllBytes(imageFilePath)))
            {
                return new Tuple<int, int>(image.Width, image.Height);
            }
        }*/
        public static TempSafeImage OverlayImages(SafeImage[] images) {
            if (images == null || images.Length < 1) throw new ArgumentException("No images provided");
            SafeImage firstSafeImage = images[0];
            SafeImage[] otherSafeImages = images.Skip(1).ToArray();
            return firstSafeImage.UsingImageSharp((firstImage, save) =>
            {
                using (Image<Rgba32> image = new Image<Rgba32>(firstImage.Width, firstImage.Height))
                {
                    //float horizontalResolution = firstImage.HorizontalResolution;
                    //float verticalResolution = firstImage.VerticalResolution;
                    //if (firstImage.HorizontalResolution <= 0)
                   //     horizontalResolution = 72f;
                   // if (firstImage.VerticalResolution <= 0)
                  //      verticalResolution = 72f;
                   // image.SetResolution(horizontalResolution, verticalResolution);
                        image.Mutate(o => o.DrawImage(firstImage, new Point(0, 0), 1f));
                       // firstImage.SetResolution(horizontalResolution, verticalResolution);
                        foreach (SafeImage safeImageToOverlay in otherSafeImages)
                        {
                            safeImageToOverlay.UsingImageSharp((imageToOverlay, ignore) =>
                            {
                                image.Mutate(o => o.DrawImage(imageToOverlay, new Point(0, 0), 1f));
                                //imageToOverlay.SetResolution(horizontalResolution, verticalResolution);
                            });
                        }
                    return TempSafeImage.New(image);
                }
            });
        }
        /*
    @staticmethod
    def overlay_images_from_image_urls_to_image_file_path(urls, image_file_path):
        images=[]
        print(image_file_path)
        for url in urls:
            response = requests.get(url)
            image = Image.open(BytesIO(response.content))
            images.append(image)
        image = ImageHelper.overlay_images(images)
        image.save(image_file_path)
        *//*
        public static TempSafeImage DilateAndErodeFilter(SafeImage safeImage, int matrixSize, MorphologyType morphType,
                           bool applyBlue = true, bool applyGreen = true, bool applyRed = true, bool applyAlpha = true)
        {
            return safeImage.UsingImageSharp((sourceBitmap, save) =>
            {
                return DilateAndErodeFilter(sourceBitmap, matrixSize, morphType, applyBlue, applyGreen, applyRed, applyAlpha);
            });
        }
        public static TempSafeImage DilateAndErodeFilter(Image<Rgba32> sourceBitmap, int matrixSize, MorphologyType morphType,
                           bool applyBlue = true, bool applyGreen = true, bool applyRed = true, bool applyAlpha = true)
        {
                byte[] pixelBuffer;
                BitmapData sourceData = GetSourceDataFromBitmap(sourceBitmap, out pixelBuffer);
                byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];
                int filterOffset = (matrixSize - 1) / 2;
                byte morphResetValue = (morphType == MorphologyType.Erosion) ? (byte)255 : (byte)0;
                FilterDelegate filter = GetFilter(morphType, pixelBuffer, sourceData, morphResetValue);
                ApplyFilterAccrossImage(filterOffset, sourceData, filter, pixelBuffer,
                    resultBuffer, applyBlue, applyGreen, applyRed, applyAlpha);
                using (Image<Rgba32> newBitmap =CopyResultBufferToNewBitmap(sourceBitmap.Width, sourceBitmap.Height, resultBuffer))
                {
                    return TempSafeImage.New(newBitmap);
                }
        }
        public static void Blur(Image<Rgba32> image) {
            return;
        }
        public static BitmapData GetSourceDataFromBitmap(Image<Rgba32> sourceBitmap, out byte[] pixelBuffer)
        {
            var memoryGroup = sourceBitmap.GetPixelMemoryGroup();
            var memoryGroupArray = memoryGroup.ToArray()[0];
            BitmapData sourceData = new BitmapData(sourceBitmap, 
                new SixLabors.ImageSharp.Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height));
            pixelBuffer = MemoryMarshal.AsBytes(memoryGroupArray.Span).ToArray();
            return sourceData;
        }/*
        private static Image<Rgba32> CopyResultBufferToNewBitmap(int width, int height, byte[] resultBuffer) {

            return Image.LoadPixelData<Rgba32>(resultBuffer, width, height);
        }
        private static void ApplyFilterAccrossImage(int filterOffset, BitmapData sourceData, FilterDelegate filter, byte[] pixelBuffer,
            byte[] resultBuffer, bool applyBlue, bool applyGreen, bool applyRed, bool applyAlpha) {


            for (int offsetY = filterOffset; offsetY < sourceData.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceData.Width - filterOffset; offsetX++)
                {
                    int byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    RGBABytes rgbaBytes = filter(filterOffset, byteOffset);

                    if (!applyBlue)
                        rgbaBytes.B = pixelBuffer[byteOffset];

                    if (!applyGreen)
                        rgbaBytes.G = pixelBuffer[byteOffset + 1];

                    if (!applyRed )
                        rgbaBytes.R = pixelBuffer[byteOffset + 2];

                    if (!applyAlpha)
                        rgbaBytes.A = pixelBuffer[byteOffset + 3];

                    resultBuffer[byteOffset] = rgbaBytes.B;
                    resultBuffer[byteOffset + 1] = rgbaBytes.G;
                    resultBuffer[byteOffset + 2] = rgbaBytes.R;
                    resultBuffer[byteOffset + 3] = rgbaBytes.A;
                }
            }
        }
        private static FilterDelegate GetFilter(MorphologyType morphologyType, byte[] pixelBuffer, BitmapData sourceData, byte morphResetValue, bool average = false) {
            switch (morphologyType)
            {
                case MorphologyType.Dilation:
                    return Get_DilationLexicalClosure(pixelBuffer, sourceData, morphResetValue, average);
                case MorphologyType.Erosion:
                    return Get_ErosionLexicalClosure(pixelBuffer, sourceData, morphResetValue, average);
                default:
                    throw new NotImplementedException($"The {nameof(MorphologyType)} {Enum.GetName(typeof(MorphologyType), morphologyType)} is not implemented");
            }
        }
        private static FilterDelegate Get_DilationLexicalClosure(byte[] pixelBuffer, BitmapData sourceData, byte morphResetValue, bool average)
        {
            return average 
                ?Get_FilterAverageLexicalClosure(pixelBuffer, sourceData, morphResetValue, DilationComparison)
                :Get_FilterLexicalClosure(pixelBuffer, sourceData, morphResetValue, DilationComparison);
        }
        private static FilterDelegate Get_ErosionLexicalClosure(byte[] pixelBuffer, BitmapData sourceData, byte morphResetValue, bool average)
        {
            return average 
                ?Get_FilterAverageLexicalClosure(pixelBuffer, sourceData, morphResetValue, ErosionComparison)
                : Get_FilterLexicalClosure(pixelBuffer, sourceData, morphResetValue, ErosionComparison);
        }
        private static bool DilationComparison(byte pixelBufferByte, byte rgbaBufferByte)
        {
            return pixelBufferByte > rgbaBufferByte;
        }
        private static bool ErosionComparison(byte pixelBufferByte, byte rgbaBufferByte)
        {
            return pixelBufferByte < rgbaBufferByte;
        }
        private static FilterDelegate Get_FilterLexicalClosure(byte[] pixelBuffer, BitmapData sourceData, byte morphResetValue,
            Func<byte, byte, bool> comparisonOperator)
        {
            return (filterOffset, byteOffset) =>
            {
                RGBABytes rgbaBytes = RGBABytes.ForDefault(morphResetValue);
                for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                {
                    for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                    {
                        int calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                        if (comparisonOperator(pixelBuffer[calcOffset], rgbaBytes.B))
                            rgbaBytes.B = pixelBuffer[calcOffset];

                        if (comparisonOperator(pixelBuffer[calcOffset + 1], rgbaBytes.G))
                            rgbaBytes.G = pixelBuffer[calcOffset + 1];

                        if (comparisonOperator(pixelBuffer[calcOffset + 2], rgbaBytes.R))
                            rgbaBytes.R = pixelBuffer[calcOffset + 2];

                        if (comparisonOperator(pixelBuffer[calcOffset + 3], rgbaBytes.A))
                            rgbaBytes.A = pixelBuffer[calcOffset + 3];
                    }
                }
                return rgbaBytes;
            };
        }
        private static FilterDelegate Get_FilterAverageLexicalClosure(byte[] pixelBuffer, BitmapData sourceData, byte morphResetValue,
            Func<byte, byte, bool> comparisonOperator)
        {
            return (filterOffset, byteOffset) =>
            {
                int r = 0, g = 0, b = 0, a = 0;
                int nR=0, nG=0, nB=0, nA= 0;
                RGBABytes rgbaBytes = RGBABytes.ForDefault(morphResetValue);
                for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                {
                    for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                    {
                        int calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                        if (comparisonOperator(pixelBuffer[calcOffset], rgbaBytes.B))
                        {
                            nB +=1;
                            r += pixelBuffer[calcOffset];
                        }

                        if (comparisonOperator(pixelBuffer[calcOffset + 1], rgbaBytes.G))
                        {
                            nG += 1;
                            g+= pixelBuffer[calcOffset + 1];
                        }

                        if (comparisonOperator(pixelBuffer[calcOffset + 2], rgbaBytes.R))
                        {
                            nR += 1;
                            b += pixelBuffer[calcOffset + 2];
                        }

                        if (comparisonOperator(pixelBuffer[calcOffset + 3], rgbaBytes.A))
                        {
                            nA += 1;
                            a += pixelBuffer[calcOffset + 3];
                        }
                    }
                }
                int nFilterPixels = (int)Math.Pow((filterOffset * 2), 2);
                if (nR>0)
                {
                    rgbaBytes.R = (byte)(r / nR);
                }
                if (nG>0)
                {
                    rgbaBytes.G = (byte)(g / nG);
                }
                if (nB>0)
                {
                    rgbaBytes.B = (byte)(b / nB);
                }
                if (nA>0)
                {
                    rgbaBytes.A = (byte)(a / nA);
                }
                return rgbaBytes;
            };
        }
        public static SafeImage OpenMorphologyFilter(SafeImage sourceBitmap, int matrixSize, bool applyBlue = true,
                            bool applyGreen = true, bool applyRed = true)
        {
            SafeImage resultBitmap = DilateAndErodeFilter(sourceBitmap, matrixSize, MorphologyType.Erosion,
                                applyBlue, applyGreen, applyRed);
            resultBitmap = DilateAndErodeFilter(resultBitmap, matrixSize, MorphologyType.Dilation,
                               applyBlue, applyGreen, applyRed);
            return resultBitmap;
        }


        public static SafeImage CloseMorphologyFilter(SafeImage sourceBitmap, int matrixSize, bool applyBlue = true,
              bool applyGreen = true, bool applyRed = true)
        {
            SafeImage resultBitmap =  DilateAndErodeFilter(sourceBitmap, matrixSize, MorphologyType.Dilation,
                                applyBlue, applyGreen, applyRed);
            resultBitmap = DilateAndErodeFilter(resultBitmap, matrixSize, MorphologyType.Erosion,
                               applyBlue, applyGreen, applyRed);
            return resultBitmap;
        }*/


        public static void CompressToJpg(SafeImage safeImage, string filePathOut, int qualityPercent)
        {
            safeImage.UsingImageSharp((image, save) =>
            {
                var imageEncoder = new JpegEncoder
                {
                    Quality = qualityPercent
                };
                image.Save(filePathOut, imageEncoder);
            });
        }
        public static void CompressToPng(SafeImage safeImage, string filePathOut, int qualityPercent)
        {

            safeImage.UsingImageSharp((image, save) =>
            {
                var imageEncoder = new PngEncoder
                {
                    CompressionLevel = GetPngCompressionLevelFromQualityPercent(qualityPercent)
                };
                image.Save(filePathOut, imageEncoder);
            });
        }
        private static PngCompressionLevel GetPngCompressionLevelFromQualityPercent(int qualityPercent)
        {
            if (qualityPercent > 90) return PngCompressionLevel.Level0;
            if (qualityPercent > 80) return PngCompressionLevel.Level1;
            if (qualityPercent > 70) return PngCompressionLevel.Level2;
            if (qualityPercent > 60) return PngCompressionLevel.Level3;
            if (qualityPercent > 50) return PngCompressionLevel.Level4;
            if (qualityPercent > 40) return PngCompressionLevel.Level5;
            if (qualityPercent > 30) return PngCompressionLevel.Level6;
            if (qualityPercent > 20) return PngCompressionLevel.Level7;
            if (qualityPercent > 10) return PngCompressionLevel.Level8;
            return PngCompressionLevel.Level9;
        }
    }
}
