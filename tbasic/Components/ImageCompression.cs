// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Tbasic.Components
{
    internal class Compress
    {
        public static MemoryStream DoIt(Image img, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100");


            // Encoder parameter for image quality 
            EncoderParameter qualityParam =
                new EncoderParameter(Encoder.Quality, quality);
            // Jpeg image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            MemoryStream stream = new MemoryStream();
            img.Save(stream, jpegCodec, encoderParams);
            return stream;
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++) {
                if (codecs[i].MimeType == mimeType) {
                    return codecs[i];
                }
            }
            return null;
        } 
    }
}
