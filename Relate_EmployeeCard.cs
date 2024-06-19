using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FaceRecognitionDotNet;
using QRmaker;
using Relate_GuestCard;

namespace Relate_EmployeeCard
{
    public class EmployeeCard
    {
        private static FaceRecognition _faceRecognition = FaceRecognition.Create(@"C:\\JHG\\c#\\MVCTest1\\Models\\model"); // 모델 경로 설정

        public static bool make_EC(string emp_number)
        {
            bool check_empnumber = GuestCard.CheckGuestExists(emp_number);
            if(!check_empnumber)
            {
                // 컬러 지정
                string hexColor = "#000000";
                int color_r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
                int color_g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
                int color_b = Convert.ToInt32(hexColor.Substring(5, 2), 16);
                Color font_color = Color.FromArgb(color_r, color_g, color_b);

                // 기본 사원증 앞면 이미지 로드
                byte[] imagedata1 = Decryption_UsefulAdmindll.Decrypt.Decrypt_Byte_Data("use_employeeimg1");
                byte[] imagedata2 = Decryption_UsefulAdmindll.Decrypt.Decrypt_Byte_Data("use_employeeimg2");

                Bitmap bmp_EC_front;
                Bitmap bmp_EC_backside;
                using (MemoryStream ms1 = new MemoryStream(imagedata1))
                {
                    bmp_EC_front = new Bitmap(ms1);
                }
                using (MemoryStream ms2 = new MemoryStream(imagedata2))
                {
                    bmp_EC_backside = new Bitmap(ms2);
                }

                // 앞면 QR 이미지 로드
                string xor_empnumber = Encryption_dll.Xor.XorEncrypt(emp_number);
                Bitmap qrBitmap1 = Make_QR.make_QR_bmp(xor_empnumber, 9);
                Bitmap qrWithoutBorder1 = Make_QR.RemoveQRBorder(qrBitmap1);
                Bitmap transparentQR1 = Make_QR.MakeQRBackgroundTransparent(qrWithoutBorder1);

                // 뒷면 QR 이미지 로드
                Bitmap qrBitmap2 = Make_QR.make_QR_bmp("https://192.168.0.38:7097/Home/Businesscard/" + emp_number, 6);
                Bitmap qrWithoutBorder2 = Make_QR.RemoveQRBorder(qrBitmap2);
                Bitmap transparentQR2 = Make_QR.MakeQRBackgroundTransparent(qrWithoutBorder2);

                // 얼굴 이미지를 DB에서 복호화하여 로드
                byte[] faceImageData = Decryption_dll.Decrypt.Decrypt_Byte_Data(Int32.Parse(emp_number), "faceimg1");
                if (faceImageData == null)
                {
                    throw new ArgumentNullException("faceImageData", "Face image data is null.");
                }

                Bitmap faceBitmap;
                using (MemoryStream ms3 = new MemoryStream(faceImageData))
                {
                    faceBitmap = new Bitmap(ms3);
                }

                // Bitmap을 FaceRecognitionDotNet 이미지로 변환
                FaceRecognitionDotNet.Image faceImage = FaceRecognition.LoadImage(faceBitmap);
                var faceLocations = _faceRecognition.FaceLocations(faceImage).ToArray();

                if (faceLocations.Length > 0)
                {
                    var faceLocation = faceLocations.FirstOrDefault(); // 첫 번째 얼굴만 사용
                    int top = Math.Max(faceLocation.Top - 600, 0); // 얼굴 위쪽에 더 여유를 줌
                    int bottom = Math.Min(faceLocation.Bottom + 600, faceBitmap.Height); // 얼굴 아래쪽에 가슴 정도까지 여유를 줌
                    int left = Math.Max(faceLocation.Left - 100, 0); // 얼굴 좌측에 여유를 줌
                    int right = Math.Min(faceLocation.Right + 100, faceBitmap.Width); // 얼굴 우측에 여유를 줌

                    // 얼굴 위치를 조정하여 적절히 크롭
                    Rectangle cropRect = new Rectangle(left, top, right - left, bottom - top);
                    Bitmap croppedFaceBitmap = new Bitmap(cropRect.Width, cropRect.Height);

                    using (Graphics g = Graphics.FromImage(croppedFaceBitmap))
                    {
                        g.DrawImage(faceBitmap, new Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel);
                    }

                    // 크롭된 얼굴 이미지를 원 안에 맞추기 위해 크기 조정
                    Bitmap resizedFaceBitmap = new Bitmap(croppedFaceBitmap, new Size(284, 284));

                    // 동그라미 마스킹 적용
                    Bitmap maskedFaceBitmap = new Bitmap(284, 284);
                    using (Graphics graphic = Graphics.FromImage(maskedFaceBitmap))
                    {
                        using (TextureBrush brush = new TextureBrush(resizedFaceBitmap))
                        {
                            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            graphic.FillEllipse(brush, new Rectangle(0, 0, 284, 284));
                        }
                    }

                    // 사원증 앞면 그래픽 객체 생성
                    using (Graphics graphics = Graphics.FromImage(bmp_EC_front))
                    {
                        // 얼굴 이미지를 동그라미 안에 삽입, 위치 조정
                        graphics.DrawImage(maskedFaceBitmap, new Rectangle(152, 156, 284, 284));

                        // 폰트와 브러시 설정
                        Font font1 = new Font("맑은 고딕", 35, FontStyle.Bold);
                        Font font2 = new Font("맑은 고딕", 19, FontStyle.Italic);
                        Font font3 = new Font("맑은 고딕", 19);
                        SolidBrush brush = new SolidBrush(font_color);

                        // 텍스트 렌더링
                        graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "name").ToString(), font1, brush, new PointF(220, 455));
                        graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "pos").ToString(), font3, brush, new PointF(270, 520));
                        graphics.DrawString(emp_number, font3, brush, new PointF(197, 572));
                        graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "email").ToString(), font3, brush, new PointF(197, 617));
                        graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "tel").ToString(), font3, brush, new PointF(197, 664));

                        // QR 이미지 렌더링
                        graphics.DrawImage(transparentQR1, 182, 735);

                        // DB에 저장하기 위해 비트맵을 바이트 배열로 변환
                        byte[] bmp_EC_front_bytes;
                        using (MemoryStream ms4 = new MemoryStream())
                        {
                            bmp_EC_front.Save(ms4, System.Drawing.Imaging.ImageFormat.Jpeg);
                            bmp_EC_front_bytes = ms4.ToArray();
                        }

                        // 변경 사항 저장
                        // bmp_EC_front.Save("ex_image1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(emp_number), bmp_EC_front_bytes, "employeeimg1");
                    }

                    // 사원증 뒷면 그래픽 객체 생성
                    using (Graphics graphics = Graphics.FromImage(bmp_EC_backside))
                    {
                        // QR 이미지 렌더링
                        graphics.DrawImage(transparentQR2, 175, 730);

                        // 비트맵을 바이트 배열로 변환
                        byte[] bmp_EC_backside_bytes;
                        using (MemoryStream ms5 = new MemoryStream())
                        {
                            bmp_EC_backside.Save(ms5, System.Drawing.Imaging.ImageFormat.Jpeg);
                            bmp_EC_backside_bytes = ms5.ToArray();
                        }

                        // 변경 사항 DB에 저장
                        // bmp_EC_backside.Save("ex_image2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(emp_number), bmp_EC_backside_bytes, "employeeimg2");
                    }
                    return true;
                }
                else { return false; }
            }

            else
            {
                return false;
            }

        }
    }
}
