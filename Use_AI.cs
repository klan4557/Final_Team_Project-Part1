using System;
using System.IO;
using System.Linq;
using DlibDotNet;
using FaceRecognitionDotNet;
using Google.Cloud.Vision.V1;
using System.Drawing;
using System.Diagnostics;


namespace Use_AI
{
    public class AI
    {
        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return new Bitmap(ms);
            }
        }
        private static void SaveBitmap(Bitmap bitmap, string filePath)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    ms.CopyTo(fs);
                }
            }
        }


        public static double recognize_face(string number)
        {
            byte[] image1_byte, image2_byte;
            double distance =0.0;
            if (number.Length > 4)
            {
                image1_byte = Decryption_dll.Decrypt.Decrypt_Byte_Data(Int32.Parse(number), "faceimg1");
                image2_byte = Decryption_dll.Decrypt.Decrypt_Byte_Data(Int32.Parse(number), "faceimg2");
            }
            else if (number.Length <= 3)
            {
                image1_byte = Decryption_dll.Decrypt.Decrypt_Byte_Data(Int32.Parse(number), "face_img1");
                image2_byte = Decryption_dll.Decrypt.Decrypt_Byte_Data(Int32.Parse(number), "face_img2");
            }
            else
            {
                throw new ArgumentException("Invalid number length.");
            }

            string tempImagePath1 = @"C:\JHG\c#\MVCTest1\wwwroot\compare_images\CompareImg1.jpg";
            string tempImagePath2 = @"C:\JHG\c#\MVCTest1\wwwroot\compare_images\CompareImg2.jpg";

            try
            {
                File.WriteAllBytes(tempImagePath1, image1_byte);
                File.WriteAllBytes(tempImagePath2, image2_byte);

                using var fr = FaceRecognition.Create(@"C:\\JHG\\c#\\MVCTest1\\Models\\model");
                var img1 = FaceRecognition.LoadImageFile(tempImagePath1);
                var img2 = FaceRecognition.LoadImageFile(tempImagePath2);

                // 각 이미지에서 얼굴 인코딩 추출
                var encoding1 = fr.FaceEncodings(img1).FirstOrDefault();
                var encoding2 = fr.FaceEncodings(img2).FirstOrDefault();
                if (encoding1 == null || encoding2 == null)
                {
                    return -1;
                }

                // 두 얼굴 인코딩 간의 거리를 계산하여 동일한 사람인지 판별
                distance = FaceRecognition.FaceDistance(encoding1, encoding2);

                Console.WriteLine(distance < 0.4 ? "Same Person" : "Different Persons");
                Console.WriteLine((1 - distance) * 100 + "%");

                // 리소스 정리
                encoding1.Dispose();
                encoding2.Dispose();
                img1.Dispose();
                img2.Dispose();

            }
            finally
            {
                // 임시 파일 삭제
                if (File.Exists(tempImagePath1))
                {
                    File.Delete(tempImagePath1);
                }

                if (File.Exists(tempImagePath2))
                {
                    File.Delete(tempImagePath2);
                }

            }
            return (1 - distance) * 100;
        }
        
    }
}
