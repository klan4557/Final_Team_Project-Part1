
using OpenCvSharp;
using Google.Cloud.Vision.V1;
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections.Generic;

namespace MVCTest1.Services
{
    public class idcard_detection_implement : idcard_detection_interface
    {

        // 마지막으로 검출된 시간을 추적하는 변수
        private DateTime lastDetectedTime = DateTime.MinValue;
        // 현재 컨투어(윤곽선)를 저장하는 변수
        private OpenCvSharp.Point[] currentContour;
        // OCR 결과가 저장될 경로
        private const string ocrResultPath = "C:\\JHG\\c#\\MVCTest1\\wwwroot\\ocr_results\\";
        // 컨투어 일치 여부를 판단하기 위한 유클리드 거리 임계값
        private const double contourMatchThreshold = 15.0;
        // 컨투어 영역 차이를 판단하기 위한 임계값
        private const double contourAreaThreshold = 150.0;
        // OCR이 수행되었는지 여부를 추적하는 속성
        public bool ocrPerformed { get; set; }
        // 마지막으로 검출된 이름을 저장하는 속성
        public string LastDetectedName { get; set; }
        // JavaScript 스크립트를 저장하는 속성
        public string Script { get; set; }

        // ID 카드 컨투어를 찾는 메서드
        public List<OpenCvSharp.Point[]> FindIdCardContours(OpenCvSharp.Point[][] contours, Mat srcImage)
        {
            var idCardContours = new List<OpenCvSharp.Point[]>();

            // 만약 컨투어가 없다면 빈 리스트를 반환
            if (contours.Length == 0)
            {
                return idCardContours;
            }

            // 각 컨투어에 대해 반복
            foreach (var contour in contours)
            {
                // 컨투어의 둘레 길이 계산
                double perimeter = Cv2.ArcLength(contour, true);
                // 컨투어를 근사화하여 단순화
                OpenCvSharp.Point[] approx = Cv2.ApproxPolyDP(contour, 0.02 * perimeter, true);
                // 만약 근사화된 컨투어가 사각형이라면
                if (approx.Length == 4)
                {
                    // 컨투어의 면적 계산
                    double area = Cv2.ContourArea(contour);
                    // 면적이 특정 범위 내에 있는지 확인
                    if (2000 < area && area < 80000)
                    {
                        // 컨투어의 바운딩 박스(최소 사각형 영역) 계산
                        OpenCvSharp.Rect boundingRect = Cv2.BoundingRect(approx);
                        // 바운딩 박스의 가로 세로 비율 계산
                        double aspectRatio = (double)boundingRect.Width / boundingRect.Height;
                        // 가로 세로 비율이 특정 범위 내에 있는지 확인
                        if (1.15 < aspectRatio && aspectRatio < 2.0)
                        {
                            // 이전 컨투어와 유사한지 확인
                            if (currentContour == null || !ContoursAreSimilar(currentContour, approx))
                            {
                                // 새로운 컨투어가 감지되면 현재 시간으로 갱신하고 OCR 수행 플래그 재설정
                                lastDetectedTime = DateTime.Now;
                                currentContour = approx;
                                ocrPerformed = false;
                                Console.WriteLine("New contour detected, starting timer.");
                            }
                            // 이전에 수행된 OCR이 없고 경과 시간이 1초 이상인 경우 OCR 수행
                            else if ((DateTime.Now - lastDetectedTime).TotalSeconds >= 1.0 && !ocrPerformed)
                            {
                                ocrPerformed = true;
                                PerformOCR(srcImage);
                                Console.WriteLine("Contour OCR performed after 1 second.");
                            }
                            // ID 카드 컨투어 목록에 현재 컨투어 추가
                            idCardContours.Add(contour);
                        }
                    }
                }
            }
            
            return idCardContours;
        }

        // 두 개의 컨투어가 유사한지 확인하는 메서드
        private bool ContoursAreSimilar(OpenCvSharp.Point[] contour1, OpenCvSharp.Point[] contour2)
        {
            // 컨투어의 점의 개수가 다르면 유사하지 않음
            if (contour1.Length != contour2.Length)
                return false;

            double totalDistance = 0;
            // 각 점 간의 거리를 계산하여 총 거리 계산
            for (int i = 0; i < contour1.Length; i++)
            {
                double distance = Math.Sqrt(Math.Pow(contour1[i].X - contour2[i].X, 2) + Math.Pow(contour1[i].Y - contour2[i].Y, 2));
                totalDistance += distance;
            }

            // 평균 거리 계산
            double averageDistance = totalDistance / contour1.Length;
            // 각 컨투어의 면적 계산
            double area1 = Cv2.ContourArea(contour1);
            double area2 = Cv2.ContourArea(contour2);
            // 두 컨투어의 면적 차이 계산
            double areaDifference = Math.Abs(area1 - area2);

            // 평균 거리와 면적 차이가 모두 임계값 이내인 경우 유사함
            return averageDistance <= contourMatchThreshold && areaDifference <= contourAreaThreshold;
        }

        public void PerformOCR(Mat srcImage)
        {
            try
            {
                // Use the whole image for OCR, without requiring contours
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    srcImage.WriteToStream(ms);
                    imageBytes = ms.ToArray();
                }

                var client = ImageAnnotatorClient.Create();
                var image = Google.Cloud.Vision.V1.Image.FromBytes(imageBytes);
                var response = client.DetectDocumentText(image);

                string name = ExtractName(response.Text);
                //SaveOcrResult(response.Text, name);
                NotifyUser(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error performing OCR: {ex.Message}");
            }
        }


        // OCR 결과에서 이름을 추출하는 메서드
        private string ExtractName(string ocrText)
        {
            var match = Regex.Match(ocrText, @"\b([가-힣]+)\(");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }

        // ocr 결과 저장하는 메서드 현재는 그냥 이름을 알리는 용도로만 사용중
        private void SaveOcrResult(string text, string name)
        {
            try
            {
                // 사용자에게 이름 알림
                NotifyUser(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving OCR result: {ex.Message}");
            }
        }

        // 사용자에게 이름을 알리는 메서드
        private void NotifyUser(string name)
        {
            try
            {
                // JavaScript 스크립트를 생성하여 사용자에게 이름 알림
                string script = $"notifyUser('{name}');";
                Script = script;
                LastDetectedName = name;
                Console.WriteLine($"User notified with name: {name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notifying user: {ex.Message}");
            }
        }

    }
}

