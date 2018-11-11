using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ZXing.QrCode;

namespace GenerateQRCodeBarCode.Helpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("barcode")]
    public class BarCodeTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = context.AllAttributes["content"].Value.ToString();
            var width = context.AllAttributes["width"].Value.ToString();
            var height = context.AllAttributes["height"].Value.ToString();

            var barcodeWriterPixelData = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new QrCodeEncodingOptions
                {
                    Height = Convert.ToInt32(height),
                    Width = Convert.ToInt32(width),
                    Margin = 0
                }
            };

            var pixelData = barcodeWriterPixelData.Write(content);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    try
                    {
                        Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    bitmap.Save(memoryStream, ImageFormat.Png);
                    output.TagName = "img";
                    output.Attributes.Clear();
                    output.Attributes.Add("width", width);
                    output.Attributes.Add("height", height);
                    output.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(memoryStream.ToArray())));
                }
            }
        }
    }
}
