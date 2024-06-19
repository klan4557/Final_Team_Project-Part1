using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
using Relate_GuestCard;


namespace Relate_BusinessCard
{
    public class BusinessCard
    {
        //// 명함 QR png로 저장
        //public static void make_BC(string emp_number)
        //{
        //    bool check_empnumber = GuestCard.CheckGuestExists(emp_number);
        //    if (!check_empnumber)
        //    {
        //        string hexColor = "#313866";

        //        // 색상 코드를 RGB 성분으로 분리
        //        int r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
        //        int g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
        //        int b = Convert.ToInt32(hexColor.Substring(5, 2), 16);

        //        // Color 객체 생성
        //        Color font_color = Color.FromArgb(r, g, b);

        //        // 기본 이미지 로드(앞면, 뒷면)
        //        byte[] imagedata1 = Decryption_UsefulAdmindll.Decrypt.Decrypt_Byte_Data("use_businessimg1");
        //        byte[] imagedata2 = Decryption_UsefulAdmindll.Decrypt.Decrypt_Byte_Data("use_businessimg2");
        //        Bitmap frontbmp;
        //        Bitmap backbmp;

        //        using (MemoryStream ms = new MemoryStream(imagedata1))
        //        {
        //            frontbmp = new Bitmap(ms);
        //        }
        //        using (MemoryStream ms2 = new MemoryStream(imagedata2))
        //        {
        //            backbmp = new Bitmap(ms2);

        //        }

        //        // 1. 명함 앞면 그래픽 객체 생성 및 작업 후 DB 저장
        //        using (Graphics graphics = Graphics.FromImage(frontbmp))
        //        {
        //            // 폰트와 브러시 설정
        //            Font font1 = new Font("맑은 고딕", 12, System.Drawing.FontStyle.Bold);
        //            Font font2 = new Font("맑은 고딕", 7);
        //            Font font3 = new Font("맑은 고딕", 6);
        //            SolidBrush brush = new SolidBrush(font_color);


        //            // 텍스트 렌더링
        //            graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "name").ToString(), font1, brush, new PointF(125, 225));
        //            graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "pos").ToString(), font2, brush, new PointF(125, 300));
        //            graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "tel").ToString(), font2, brush, new PointF(125, 385));
        //            graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "email").ToString(), font2, brush, new PointF(125, 427));
        //            graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "address").ToString(), font3, brush, new PointF(125, 476));

        //            // 비트맵을 바이트 배열로 변환
        //            byte[] frontbmp_bytes;
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                frontbmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                frontbmp_bytes = ms.ToArray();
        //            }

        //            // 변경 사항 저장
        //            // frontbmp.Save("ex_image123.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        //            Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(emp_number), frontbmp_bytes, "businessimg1");
        //        }

        //        // 2. 명함 뒷면 DB 저장

        //        Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(emp_number), imagedata2, "businessimg2");

        //    }


        //    // 컬러 지정
        //    // 16진수 색상 코드

        //}
        public static void make_BC(string emp_number)
        {
            // 사용 가능 폰트 확인
            //foreach (FontFamily font in System.Drawing.FontFamily.Families)
            //{
            //    Console.WriteLine(font.Name);
            //}

            // 컬러 지정
            // 16진수 색상 코드
            string hexColor = "#313866";

            // 색상 코드를 RGB 성분으로 분리
            int r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(5, 2), 16);

            // Color 객체 생성
            Color font_color = Color.FromArgb(r, g, b);

            // id 있는지 탐색
            bool check_empnumber = GuestCard.CheckGuestExists(emp_number);
            if (!check_empnumber)
            {
                // 기본 이미지 로드(앞면, 뒷면)
                byte[] imagedata1 = Decryption_UsefulAdmindll.Decrypt.Decrypt_Byte_Data("use_businessimg1");
                byte[] imagedata2 = Decryption_UsefulAdmindll.Decrypt.Decrypt_Byte_Data("use_businessimg2");
                Bitmap frontbmp;
                Bitmap backbmp;

                using (MemoryStream ms = new MemoryStream(imagedata1))
                {
                    frontbmp = new Bitmap(ms);
                }
                using (MemoryStream ms2 = new MemoryStream(imagedata2))
                {
                    backbmp = new Bitmap(ms2);

                }

                // 1. 명함 앞면 그래픽 객체 생성 및 작업 후 DB 저장
                using (Graphics graphics = Graphics.FromImage(frontbmp))
                {
                    // 폰트와 브러시 설정
                    Font font1 = new Font("맑은 고딕", 40, System.Drawing.FontStyle.Bold);
                    Font font2 = new Font("맑은 고딕", 23);
                    Font font3 = new Font("맑은 고딕", 19);
                    SolidBrush brush = new SolidBrush(font_color);


                    // 텍스트 렌더링
                    graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "name").ToString(), font1, brush, new PointF(120, 225));
                    graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "pos").ToString(), font2, brush, new PointF(125, 300));
                    graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "tel").ToString(), font2, brush, new PointF(125, 385));
                    graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "email").ToString(), font2, brush, new PointF(125, 427));
                    graphics.DrawString(Decryption_dll.Decrypt.Decrypt_String_Data(Int32.Parse(emp_number), "address").ToString(), font3, brush, new PointF(125, 476));

                    // 비트맵을 바이트 배열로 변환
                    byte[] frontbmp_bytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        frontbmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        frontbmp_bytes = ms.ToArray();
                    }

                    // 변경 사항 저장
                    //frontbmp.Save("ex_image123.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(emp_number), frontbmp_bytes, "businessimg1");
                }

                // 2. 명함 뒷면 DB 저장
                //backbmp.Save("ex_image1234.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                Encryption_dll.Encrypt.Encrypt_ToDB(Int32.Parse(emp_number), imagedata2, "businessimg2");
            }
            else Console.WriteLine("이미 존재하는 사원번호입니다.");
        }
    }
}