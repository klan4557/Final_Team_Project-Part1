//using Google.Cloud.Vision.V1;
//using System;
//using System.Collections.Generic;

//class Use_OCR
//{
//    static void print_ocr(string[] args)
//    {
//        // 인증 정보 파일 경로를 환경 변수에 설정합니다.
//        // 이 경로는 Google Cloud에서 생성한 서비스 계정 키 파일의 위치입니다.
//        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\JHG\\c#\\MVCTest1\\ocr-test-project-416305-80f44c1d7331.json");

//        // 클라이언트 생성
//        var client = ImageAnnotatorClient.Create();

//        // 이미지에 대한 경로를 지정합니다. 로컬 파일 또는 웹 URL일 수 있습니다.
//        var image = Image.FromFile("C:\\JHG\\c#\\MVCTest1\\imagetest\\idcard1.png");
//        // var image = Image.FetchFromUri("http://your/image/url.jpg");

//        // 텍스트 감지 요청
//        var response = client.DetectDocumentText(image);

//        // 전체 텍스트를 출력
//        Console.WriteLine(response.Text);
//    }


//}
