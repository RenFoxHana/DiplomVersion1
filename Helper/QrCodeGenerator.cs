using QRCoder;
using System.Drawing;
using System.IO;

namespace DiplomVersion1.Helper
{
    class QrCodeGenerator
    {
        public static string GenerateQrCode(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            return Convert.ToBase64String(qrCodeBytes);
        }

        public static Bitmap GenerateQrCodeBitmap(string content, int sizeInPixels)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new BitmapByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(sizeInPixels / 10);
            using var ms = new MemoryStream(qrCodeBytes);
            return new Bitmap(ms);
        }
    }
}
