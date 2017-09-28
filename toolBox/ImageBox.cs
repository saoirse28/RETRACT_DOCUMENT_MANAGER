using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ToolBox
{
    public class ImageBox
    {
        private static float m_dpi = 300;

        public static Bitmap ImageArchiver(Bitmap forsaving, string filename)
        {
            //string filename = GetMatchValue(@"ExamType = (?<after>\w+)", input) + "-" + GetMatchValue(@"Ocr3 = (?<after>\w+)", input);
            //string filename = GetMatchValue(@"txn_id = (?<after>\w+)", input);
            Bitmap saveImage = new Bitmap(forsaving);

            ImageCodecInfo m_ImageCodecInfo;
            Encoder m_Encoder;
            EncoderParameter m_EncoderParameter;
            EncoderParameters m_EncoderParameters;

            m_ImageCodecInfo = GetEncoderInfo("image/tiff");
            m_Encoder = Encoder.Compression;


            m_EncoderParameters = new EncoderParameters(1);
            m_EncoderParameter = new EncoderParameter(
                m_Encoder,
                (long)EncoderValue.CompressionCCITT4);
            m_EncoderParameters.Param[0] = m_EncoderParameter;

            if (saveImage.PixelFormat != PixelFormat.Format1bppIndexed)
            {
                Bitmap resizedbmp = convertToBitonal(saveImage);
                resizedbmp.SetResolution(m_dpi, m_dpi);
                //resizedbmp.Save(cp.Archive + @"\" + filename + ".tif", m_ImageCodecInfo, m_EncoderParameters);
                resizedbmp.Save(filename, m_ImageCodecInfo, m_EncoderParameters);
            }
            else
            {
                saveImage.SetResolution(m_dpi, m_dpi);
                //saveImage.Save(cp.Archive + @"\" + filename + ".tif", m_ImageCodecInfo, m_EncoderParameters);
                saveImage.Save(filename, m_ImageCodecInfo, m_EncoderParameters);
            }

            return saveImage;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private static Bitmap convertToBitonal(Bitmap original)
        {
            int sourceStride;
            byte[] sourceBuffer = extractBytes(original, out sourceStride);

            Bitmap destination = new Bitmap(original.Width, original.Height, PixelFormat.Format1bppIndexed);

            destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            int imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = destination.Height;
            int width = destination.Width;
            int threshold = 500;

            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceStride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                for (int x = 0; x < width; x++)
                {
                    pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];

                    if (pixelTotal > threshold)
                        destinationValue += (byte)pixelValue;

                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }

                    sourceIndex += 4;
                }

                if (pixelValue != 128)
                    destinationBuffer[destinationIndex] = destinationValue;
            }

            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);
            destination.UnlockBits(destinationData);
            return destination;
        }

        private static byte[] extractBytes(Bitmap original, out int stride)
        {
            Bitmap source = null;

            try
            {
                if (original.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                    source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                    using (Graphics g = Graphics.FromImage(source))
                    {
                        g.DrawImageUnscaled(original, 0, 0);
                    }
                }
                else
                {
                    source = original;
                }

                BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                int imageSize = sourceData.Stride * sourceData.Height;
                byte[] sourceBuffer = new byte[imageSize];
                Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

                source.UnlockBits(sourceData);

                stride = sourceData.Stride;
                return sourceBuffer;
            }
            finally
            {
                if (source != original)
                    source.Dispose();
            }
        }
    }
}
