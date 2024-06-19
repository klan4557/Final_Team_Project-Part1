using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FaceRecognitionDotNet;
using MySql.Data.MySqlClient;
using MySQL_dll;
using QRmaker;

namespace Relate_GuestCard
{
    public class GuestCard
    {
        public static bool CheckGuestExists(string guest_number)
        {
            try
            {
                string text = "192.168.0.63";
                string text2 = "3306";
                string text3 = "miss_db";
                string text4 = "user1";
                string text5 = "1234";
                string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";
                using MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
                mySqlConnection.Open();
                string cmdText = "SELECT EXISTS(SELECT 1 FROM Guest WHERE id = @id)";
                using MySqlCommand mySqlCommand = new MySqlCommand(cmdText, mySqlConnection);
                mySqlCommand.Parameters.AddWithValue("@id", guest_number);
                return Convert.ToBoolean(mySqlCommand.ExecuteScalar());
            }
            catch (MySqlException arg)
            {
                Console.WriteLine($"Error: {arg}");
                return false;
            }
        }

        public static FaceRecognition _faceRecognition = FaceRecognition.Create(@"C:\\JHG\\c#\\MVCTest1\\Models\\model"); // 모델 경로 설정

        public static Bitmap ConvertToNonIndexedImage(System.Drawing.Image img)
        {
            Bitmap newBmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(newBmp))
            {
                gr.DrawImage(img, new Rectangle(0, 0, newBmp.Width, newBmp.Height));
            }

            return newBmp;
        }

        public static bool make_GC(string Guest_number)
        {
            bool check_guestnumber = CheckGuestExists(Guest_number);
            if (!check_guestnumber)
            {
                // 컬러 지정
                string hexColor = "#000000";
                int color_r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
                int color_g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
                int color_b = Convert.ToInt32(hexColor.Substring(5, 2), 16);
                Color font_color = Color.FromArgb(color_r, color_g, color_b);

                // 기본 출입증 앞, 뒷면 이미지 로드
                string filePath1 = @"C:\JHG\c#\MVCTest1\GuestCard_pre1.jpg";
                string filePath2 = @"C:\JHG\c#\MVCTest1\GuestCard_pre2.jpg";
                byte[] imagedata1 = File.ReadAllBytes(filePath1);
                byte[] imagedata2 = File.ReadAllBytes(filePath2);
                Bitmap bmp_EC_front;
                Bitmap bmp_EC_backside;
                using (MemoryStream ms1 = new MemoryStream(imagedata1))
                {
                    Bitmap tempBmp = new Bitmap(ms1);
                    bmp_EC_front = ConvertToNonIndexedImage(tempBmp); // 변환
                }
                using (MemoryStream ms2 = new MemoryStream(imagedata2))
                {
                    Bitmap tempBmp = new Bitmap(ms2);
                    bmp_EC_backside = ConvertToNonIndexedImage(tempBmp); // 변환
                }

                // 앞면 QR 이미지 로드
                string xor_empnumber = Encryption_dll.Xor.XorEncrypt(Guest_number);
                Bitmap qrBitmap1 = Make_QR.make_QR_bmp(xor_empnumber, 15);
                Bitmap qrWithoutBorder1 = Make_QR.RemoveQRBorder(qrBitmap1);
                Bitmap transparentQR1 = Make_QR.MakeQRBackgroundTransparent(qrWithoutBorder1);

                // Guest_number를 3자리 숫자로 포맷
                int guestNumberInt = int.Parse(Guest_number);
                string formattedGuestNumber = guestNumberInt.ToString("D3");

                //사원증 앞면 그래픽 객체 생성
                using (Graphics graphics = Graphics.FromImage(bmp_EC_front))
                {
                    // 폰트와 브러시 설정
                    Font font1 = new Font("맑은 고딕", 35, FontStyle.Bold);
                    Font font2 = new Font("맑은 고딕", 25, FontStyle.Bold);
                    SolidBrush brush = new SolidBrush(font_color);

                    // 텍스트 렌더링
                    // graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(Guest_number), "name").ToString(), font, brush, new PointF(220, 455));
                    graphics.DrawString("방문증", font1, brush, new PointF(215, 460));
                    graphics.DrawString("No." + formattedGuestNumber, font2, brush, new PointF(230, 520));

                    // QR 이미지 렌더링
                    graphics.DrawImage(transparentQR1, 138, 610);

                    // DB에 저장하기 위해 비트맵을 바이트 배열로 변환
                    byte[] bmp_EC_front_bytes;
                    using (MemoryStream ms4 = new MemoryStream())
                    {
                        bmp_EC_front.Save(ms4, System.Drawing.Imaging.ImageFormat.Jpeg);
                        bmp_EC_front_bytes = ms4.ToArray();
                    }

                    // 변경 사항 저장
                    bmp_EC_front.Save("ex_image1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(Guest_number), bmp_EC_front_bytes, "visit_img1");
                }

                // 방문증 뒷면 저장
                //bmp_EC_backside.Save("ex_image2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(Guest_number), imagedata2, "visit_img2");
                return true;
            }
            else
            {
                Console.WriteLine("이미 존재하는 ID입니다.");
                return false;
            }
        }

        public static bool register_GC(string Guest_number, string name, byte[] faceImageData)
        {
            bool check_guestnumber = CheckGuestExists(Guest_number);
            if (check_guestnumber)
            {
                // 3자리 아닐 시 예외 처리
                if (Guest_number.Length > 4)
                {
                    Console.WriteLine("사원 번호가 3자리 이하가 아닙니다. 다시 확인해주세요.");
                    return false;
                }
                // 이미지 자체가 없을 시 예외 처리
                if (faceImageData == null)
                {
                    Console.WriteLine("제공된 이미지가 없습니다. 다시 확인해주세요.");
                    return false;
                }

                // 얼굴 이미지 확인하기 위한 준비
                Bitmap faceBitmap;
                using (MemoryStream ms = new MemoryStream(faceImageData))
                {
                    faceBitmap = new Bitmap(ms);
                }
                // Bitmap을 FaceRecognitionDotNet 이미지로 변환
                FaceRecognitionDotNet.Image faceImage = FaceRecognition.LoadImage(faceBitmap);
                var faceLocations = _faceRecognition.FaceLocations(faceImage).ToArray();

                if (faceLocations.Length > 0)
                {
                    Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(Guest_number), name, "name");
                    Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(Guest_number), faceImageData, "face_img1");
                    Console.WriteLine($"방문증 No.{Guest_number}에 사용자 등록 완료.");
                    return true;
                }
                else
                {
                    Console.WriteLine("제공된 이미지에서 얼굴이 인식되지 않았습니다. 다시 시도해주세요.");
                    return false;
                }
            }
            
            else return false;
        }
    }
}
