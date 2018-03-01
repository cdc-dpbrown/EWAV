/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       JPGUtil.cs
 *  Namespace:  WriteableBitmapScreenCapture    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace WriteableBitmapScreenCapture
{
    using System.Windows.Media.Imaging;
    using System.IO;
    using FluxJpeg.Core;
    using FluxJpeg.Core.Encoder;

    public static class JPGUtil
    {
        public static void EncodeJpg(WriteableBitmap bitmap, Stream destinationStream)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3; // RGB
            byte[][,] raster = new byte[bands][,];

            // initialise the bands
            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            // copy over the pixel data from the bitmap
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            // Use the Flux library to encode the JPG
            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);
            
            // encode it to the destination stream
            JpegEncoder encoder = new JpegEncoder(img, 90, destinationStream);
            encoder.Encode();
        }
    }
}