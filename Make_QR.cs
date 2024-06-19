using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace QRmaker
{
    public class Make_QR
    {
        // 명함 QR png로 저장
        public static void make_QR_png(string inputText, string outputPath)
        {
            //string a = Xor.XorEncrypt("20241111");

            // 1. QR 만들어서 png로 저장하기
            // QR화하고 싶은 URL/TEXT를 inputText 매개변수로 받아야 함
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                // Generate the QR code image as byte array
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                // Convert byte array to bitmap
                using (MemoryStream ms = new MemoryStream(qrCodeImage))
                using (Bitmap bitmap = new Bitmap(ms))
                {
                    // Save the bitmap as a PNG file
                    bitmap.Save(outputPath, ImageFormat.Png);

                    // Print the result file path
                    Console.WriteLine($"QR code generated: {outputPath}");
                }
            }
        }

        // QR 비트맵으로 반환
        public static Bitmap make_QR_bmp(string inputText, int size)
        {
            
            
            // 2. QR 만들어서 bitmap으로 반환하기
            // QR화하고 싶은 URL/TEXT를 inputText 매개변수로 받아야 함
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                // Generate the QR code image as byte array
                byte[] qrCodeImage = qrCode.GetGraphic(size);

                // Convert byte array to bitmap
                using (MemoryStream ms = new MemoryStream(qrCodeImage))
                {
                    return new Bitmap(ms);
                }
            }
        }

        // QR 비트맵으로 반환
        public static Bitmap make_QR_bmp(byte[] input, int size)
        {
            // 2. QR 만들어서 bitmap으로 반환하기
            // QR화하고 싶은 URL/TEXT를 inputText 매개변수로 받아야 함
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                // Generate the QR code image as byte array
                byte[] qrCodeImage = qrCode.GetGraphic(size);

                // Convert byte array to bitmap
                using (MemoryStream ms = new MemoryStream(qrCodeImage))
                {
                    return new Bitmap(ms);
                }
            }
        }


        /* QR 테두리 및 배경 지우는 함수들 */
        // 주어진 열이 모두 흰색인지 확인합니다.
        private static bool IsColumnEmpty(Bitmap qrBitmap, int x)
        {
            for (int y = 0; y < qrBitmap.Height; y++)
            {
                if (qrBitmap.GetPixel(x, y).GetBrightness() < 0.5)
                {
                    return false;
                }
            }
            return true;
        }

        // 주어진 행이 모두 흰색인지 확인합니다.
        private static bool IsRowEmpty(Bitmap qrBitmap, int y)
        {
            for (int x = 0; x < qrBitmap.Width; x++)
            {
                if (qrBitmap.GetPixel(x, y).GetBrightness() < 0.5)
                {
                    return false;
                }
            }
            return true;
        }

        // QR 테두리 지우기
        public static Bitmap RemoveQRBorder(Bitmap qrBitmap)
        {
            // QR 코드 이미지의 크기를 결정합니다.
            int width = qrBitmap.Width;
            int height = qrBitmap.Height;

            // QR 코드의 가장자리 색상을 확인하고 검은색이 아닌 부분을 찾습니다.
            // 상하좌우 모서리부터 검사합니다.
            int left = 0, top = 0, right = width - 1, bottom = height - 1;

            // 좌우 확인
            for (int x = 0; x < width; x++)
            {
                if (left == 0 && !IsColumnEmpty(qrBitmap, x))
                {
                    left = x;
                }
                if (right == width - 1 && !IsColumnEmpty(qrBitmap, width - 1 - x))
                {
                    right = width - 1 - x;
                }
                if (left != 0 && right != width - 1)
                {
                    break;
                }
            }

            // 상하 확인
            for (int y = 0; y < height; y++)
            {
                if (top == 0 && !IsRowEmpty(qrBitmap, y))
                {
                    top = y;
                }
                if (bottom == height - 1 && !IsRowEmpty(qrBitmap, height - 1 - y))
                {
                    bottom = height - 1 - y;
                }
                if (top != 0 && bottom != height - 1)
                {
                    break;
                }
            }

            // QR 코드 이미지에서 테두리를 제거하여 새로운 비트맵을 생성합니다.
            Rectangle cropRect = new Rectangle(left, top, right - left + 1, bottom - top + 1);
            Bitmap croppedBitmap = qrBitmap.Clone(cropRect, qrBitmap.PixelFormat);

            return croppedBitmap;
        }

        // QR 배경 지우기
        public static Bitmap MakeQRBackgroundTransparent(Bitmap qrBitmap)
        {
            // 새로운 비트맵을 생성하고 원래 비트맵의 픽셀을 투명도로 설정
            Bitmap transparentBitmap = new Bitmap(qrBitmap.Width, qrBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int y = 0; y < qrBitmap.Height; y++)
            {
                for (int x = 0; x < qrBitmap.Width; x++)
                {
                    Color pixelColor = qrBitmap.GetPixel(x, y);

                    // 흰색 픽셀을 투명하게 설정
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                        transparentBitmap.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        transparentBitmap.SetPixel(x, y, pixelColor);
                    }
                }
            }

            return transparentBitmap;
        }
    }
}