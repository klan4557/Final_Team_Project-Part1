using OpenCvSharp;
using System.Collections.Generic;

namespace MVCTest1.Services
{
    public interface idcard_detection_interface
    {
        List<OpenCvSharp.Point[]> FindIdCardContours(OpenCvSharp.Point[][] contours, Mat srcImage);
        string LastDetectedName { get; set; }
        string Script { get; set; }
        bool ocrPerformed { get; set; }
        void PerformOCR(Mat srcImage); // PerformOCR 메서드 정의 추가
    }
}