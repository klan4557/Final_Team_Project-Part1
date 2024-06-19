using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC 컨트롤러와 액션 메서드를 사용하기 위한 네임스페이스
using MVCTest1.Models; // MVCTest1 프로젝트의 Models 폴더에 있는 클래스를 사용하기 위한 네임스페이스
using System.Diagnostics; // 시스템 진단에 필요한 클래스를 사용하기 위한 네임스페이스
using System;
using MVCTest1.Data;
using MySql.Data.MySqlClient;
using OpenCvSharp;
using Google.Cloud.Vision.V1;
using System.Collections.Generic;
using MVCTest1.Services;
using System.Text;
using QRmaker;
using Relate_EmployeeCard;
using Relate_BusinessCard;
using System.Text.RegularExpressions;
// MVCTest1 프로젝트의 Controllers 폴더 내에 HomeController 클래스를 정의
using MVCTest1.TcpIp;
using Relate_GuestCard;
using System.Xml.Linq;
using MySQL_dll;
using Google.Protobuf;
using FaceRecognitionDotNet.Extensions;
using System.Drawing;

namespace MVCTest1.Controllers
{
    // Controller 클래스를 상속받아 HomeController 클래스를 정의
    public class HomeController : Controller
    {
        
        // ILogger 인터페이스를 사용하여 로깅 기능을 구현. HomeController 타입의 로거 인스턴스를 선언
        // 데이터베이스 컨텍스트 객체의 의존성 주입을 위한 필드입니다.
        // 생성자를 통해 ApplicationDbContext 타입의 인스턴스를 받아와서 _context 필드를 초기화합니다. 이는 DI(Dependency Injection) 패턴을 사용합니다.
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly idcard_detection_interface _idcard_detection_implement;
        private readonly FaceDetection _faceDetection;

        //private static readonly Dictionary<string, List<LogSuccessData>> Logdatastore = new Dictionary<string, List<LogSuccessData>>();
        // 단일 생성자로 모든 의존성 주입
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, idcard_detection_interface idcard_detection_implement, FaceDetection faceDetection)
        {
            _logger = logger;
            _context = context;
            _idcard_detection_implement = idcard_detection_implement;
            _faceDetection = faceDetection;

            //if (!TcpIp.Tcp_ip.Logdatastore.ContainsKey("20241111"))
            //{
            //    TcpIp.Tcp_ip.Logdatastore["20241111"] = new List<LogSuccessData>
            //    {
            //        new LogSuccessData { Id = "20241111", Time = "2023-05-25 T 10:30:00", Commute = "1" },
            //        new LogSuccessData { Id = "20241111", Time = "2023-05-26 T 11:30:00", Commute = "2" },
            //        new LogSuccessData { Id = "20241111", Time = "2023-05-27 T 10:30:00", Commute = "3" }
            //    };
            //}
            //if (!Logdatastore.ContainsKey("1"))
            //{
            //    Logdatastore["1"] = new List<LogSuccessData>
            //    {
            //        new LogSuccessData { Id = "1", Time = "2023-05-25 T 10:30:00", Commute = "1" },
            //        new LogSuccessData { Id = "1", Time = "2023-05-26 T 11:30:00", Commute = "2" },
            //        new LogSuccessData { Id = "1", Time = "2023-05-27 T 10:30:00", Commute = "1" }
            //    };
            //}
        }

        // Index 페이지를 반환하는 액션 메서드 IActionResult
        public IActionResult Index()
        {
            _logger.LogInformation($"세션 Role 설정 후: {HttpContext.Session.GetString("Role")}");
            return View(); // 기본적으로 Views/Home/Index.cshtml 뷰 파일을 찾아서 반환
        }

        // Privacy 페이지를 반환하는 액션 메서드
        public IActionResult Privacy()
        {
            return View(); // 기본적으로 Views/Home/Privacy.cshtml 뷰 파일을 찾아서 반환
        }

        // login 페이지를 반환하는 액션 메서드
        public IActionResult Login()
        {
            return View(); // 기본적으로 Views/Home/Login.cshtml 뷰 파일을 찾아서 반환
        }

        public IActionResult Modify()
        {
            var emp_number = HttpContext.Session.GetString("emp_number");
            var userRole = HttpContext.Session.GetString("Role");

            ViewBag.UserRole = userRole;

            if (string.IsNullOrEmpty(emp_number) && userRole != "Admin")
            {
                ViewBag.emp_number = "";
                _logger.LogInformation("Modify 접근: 사용자가 로그인하지 않았습니다.");
                return RedirectToAction("Index", "Home");
            }

            if (userRole == "User")
            {
                ViewBag.emp_number = emp_number;
                _logger.LogInformation($"Modify 접근: 로그인한 사용자 이름 - {emp_number}");
                int emp_number2 = int.Parse(emp_number);

                ViewBag.User_name = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "name");
                ViewBag.User_pos = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "pos");
                ViewBag.User_tel = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "tel");
                ViewBag.User_email = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "email");
                ViewBag.User_address = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "address");
            }
            else if (userRole == "Admin")
            {
                _logger.LogInformation("Admin 접근: 사원 정보 조회 및 수정");
                // 초기에는 아무 정보도 조회하지 않음
            }
            else
            {
                _logger.LogWarning("Access denied. Invalid role trying to access Modify");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult SearchEmployee(string Emp_Number)
        {
            var userRole = HttpContext.Session.GetString("Role");
            ViewBag.UserRole = userRole;
            if (userRole != "Admin")
            {
                _logger.LogWarning("Unauthorized access attempt by non-admin user.");
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(Emp_Number))
            {
                ModelState.AddModelError("Emp_Number", "사원번호를 입력해주세요.");
                return View("Modify");
            }

            int emp_number2 = int.Parse(Emp_Number);
            ViewBag.emp_number = Emp_Number;
            ViewBag.User_name = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "name");
            ViewBag.User_pos = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "pos");
            ViewBag.User_tel = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "tel");
            ViewBag.User_email = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "email");
            ViewBag.User_address = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "address");

            return View("Modify");
        }

        [HttpPost]
        public IActionResult Modify(ModifyUserViewModel model)
        {
            var userRole = HttpContext.Session.GetString("Role");
            ViewBag.UserRole = userRole;
            if (userRole != "User" && userRole != "Admin")
            {
                _logger.LogWarning("Unauthorized access attempt.");
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Proceeding to save data.");

                int empNumber = int.Parse(model.Emp_Number);

                // Encryption 및 데이터베이스 처리 로직
                MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();
                mysqlhandler.Insert_UserIDPW(model.Emp_Number, model.Password);

                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, model.NameString, "name");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, model.PositionString, "pos");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, model.TelString, "tel");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, model.EmailString, "email");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, model.AddressString, "address");

                var getname = Decryption_dll.Decrypt.Decrypt_String_Data(empNumber, "name");
                Console.WriteLine(getname);
                _logger.LogInformation("Data encrypted and saved successfully.");

                return RedirectToAction("Index", "Home"); // 수정 후 홈으로 리디렉션
            }
            else
            {
                _logger.LogWarning("Model is invalid.");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }
        }

        public IActionResult Log(string empNumber)
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole == "User")
            {
                empNumber = HttpContext.Session.GetString("emp_number");
            }

            ViewData["EmpNumber"] = empNumber;
            ViewData["UserRole"] = userRole;

            List<LogSuccessData> logs = new List<LogSuccessData>();
            if (!string.IsNullOrEmpty(empNumber) && TcpIp.Tcp_ip.Logdatastore.ContainsKey(empNumber))
            {
                logs = TcpIp.Tcp_ip.Logdatastore[empNumber]
                    .OrderByDescending(log =>
                    {
                        DateTime.TryParseExact(log.Time, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate);
                        return parsedDate;
                    })
                    .ToList();
            }



            if (!logs.Any())
            {
                Console.WriteLine($"No logs found for {empNumber}");
            }

            return View(logs);
        }

        
        [HttpPost]
        public IActionResult Search(string empNumber, DateTime? startDate, DateTime? endDate)
        {
            List<LogSuccessData> logs = new List<LogSuccessData>();

            if (string.IsNullOrEmpty(empNumber))
            {
                // empNumber가 비어있을 경우 모든 로그를 가져옴
                foreach (var logList in TcpIp.Tcp_ip.Logdatastore.Values)
                {
                    logs.AddRange(logList);
                }
            }
            else if (TcpIp.Tcp_ip.Logdatastore.ContainsKey(empNumber))
            {
                logs = TcpIp.Tcp_ip.Logdatastore[empNumber];
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime startDateOnly = startDate.Value.Date;
                DateTime endDateOnly = endDate.Value.Date;
                logs = logs.Where(log =>
                {
                    DateTime.TryParseExact(log.Time, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate);
                    return parsedDate >= startDateOnly && parsedDate <= endDateOnly;
                }).ToList();
            }

            logs = logs.OrderByDescending(log =>
            {
                DateTime.TryParseExact(log.Time, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate);
                return parsedDate;
            }).ToList();

            ViewData["EmpNumber"] = empNumber;
            ViewData["UserRole"] = HttpContext.Session.GetString("Role");

            if (!logs.Any())
            {
                Console.WriteLine($"No logs found for {empNumber}");
            }

            return View("Log", logs);
        }




        public IActionResult Emp_id_create()
        {
            var userRole = HttpContext.Session.GetString("Role");
            _logger.LogInformation($"Emp_id_create accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Emp_id_create.");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        //OCR후 사원증 이미지 제작  및 사원번호, 임시 비밀번호 주는 사이트

        public int CountEmpNumber(string connectionString)
        {
            int count = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(emp_number) FROM General_user";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        count = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }

            return count;
        }


        [HttpGet]
        public IActionResult Emp_id_create2(string name)
        {
            var userRole = HttpContext.Session.GetString("Role");
            string text = "192.168.0.63";
            string text2 = "3306";
            string text3 = "miss_db";
            string text4 = "user1";
            string text5 = "1234";
            string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            //카운트 아래에 넣으면 됨
            
            ViewBag.email = "moble@team5.com";
            ViewBag.address = "천안시 대흥로 255 3층 모블";
            ViewBag.password = "1234";
            int count = CountEmpNumber(connectionString) + 1;
            string num;
            if (count < 10)
            {
                num = "00";
                ViewBag.emp_number = "24204" + num + count.ToString();
            }
            else if (count < 100)
            {
                num = "0";
                ViewBag.emp_number = "24204" + num + count.ToString();
            }
            else
            {
                num = "0";
                ViewBag.emp_number = "24204" + count.ToString();
            }
            //count가 1자리 숫자이면 num에 00추가해서 넣기
            //count가 2자리 숫자이면 num에 0추가해서 넣기

            ViewBag.name = name;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DetectFace()
        {
            using var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);
            using var bitmap = new Bitmap(memoryStream);
            var faceRect = _faceDetection.DetectFace(bitmap);

            if (faceRect.HasValue)
            {
                return Ok(new { left = faceRect.Value.Left, top = faceRect.Value.Top, width = faceRect.Value.Width, height = faceRect.Value.Height });
            }

            return NoContent();
        }


        [HttpPost]
        public IActionResult Emp_id_create2(GeneralUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Proceeding to save data.");

                int empNumber = int.Parse(viewModel.Emp_Number);

                // Base64 문자열에서 이미지 데이터 추출
                string base64Image = viewModel.WebcamImage;
                string base64Data = Regex.Match(base64Image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                byte[] face_img1 = Convert.FromBase64String(base64Data);

                // Encryption 및 데이터베이스 처리 로직
                MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();
                mysqlhandler.Insert_UserIDPW(viewModel.Emp_Number, viewModel.Password);

                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, viewModel.NameString, "name");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, viewModel.PositionString, "pos");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, viewModel.TelString, "tel");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, viewModel.EmailString, "email");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, viewModel.AddressString, "address");
                Encryption_dll.Encrypt.Encrypt_ToDB(empNumber, face_img1, "faceimg1");

                BusinessCard.make_BC(viewModel.Emp_Number);
                bool EC_state = EmployeeCard.make_EC(viewModel.Emp_Number);

                if (EC_state)
                {
                    byte[] employee_image1 = Decryption_dll.Decrypt.Decrypt_Byte_Data(empNumber, "employeeimg1");
                    byte[] employee_image2 = Decryption_dll.Decrypt.Decrypt_Byte_Data(empNumber, "employeeimg2");

                    if (employee_image1 != null && employee_image2 != null)
                    {
                        string base64Image1 = Convert.ToBase64String(employee_image1);
                        string base64Image2 = Convert.ToBase64String(employee_image2);
                        return Json(new { success = true, imageData1 = base64Image1, imageData2 = base64Image2 });
                    }
                    else
                    {
                        _logger.LogWarning("Decrypted image data is null.");
                        return Json(new { success = false, message = "Image data could not be retrieved." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Image data could not be retrieved." });
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                _logger.LogWarning("Model is invalid.");
                return Json(new { success = false, message = "Model is invalid." });
            }
        }


        [HttpPost]
        public IActionResult SaveImage(string imageData1, string imageData2)
        {
            bool success = true;

            if (!string.IsNullOrEmpty(imageData1))
            {
                string filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employee1.jpg");
                byte[] imageBytes1 = Convert.FromBase64String(imageData1);
                System.IO.File.WriteAllBytes(filePath1, imageBytes1);
                _logger.LogInformation("Image 1 saved successfully.");
            }
            else
            {
                _logger.LogWarning("Image data 1 is null or empty.");
                success = false;
            }

            if (!string.IsNullOrEmpty(imageData2))
            {
                string filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employee2.jpg");
                byte[] imageBytes2 = Convert.FromBase64String(imageData2);
                System.IO.File.WriteAllBytes(filePath2, imageBytes2);
                _logger.LogInformation("Image 2 saved successfully.");
            }
            else
            {
                _logger.LogWarning("Image data 2 is null or empty.");
                success = false;
            }

            return Json(new { success });
        }

        [HttpGet]
        public IActionResult Guest_Card_Create3()
        {
            var userRole = HttpContext.Session.GetString("Role");
            _logger.LogInformation($"Guest_Card_Create accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Guest_Card_Create_menu");
                return RedirectToAction("Index", "Home");
            }
            string text = "192.168.0.63";
            string text2 = "3306";
            string text3 = "miss_db";
            string text4 = "user1";
            string text5 = "1234";
            string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";

            int guestCount = CountGuestNumber(connectionString) + 1;
            ViewBag.Guest_num = guestCount;
            return View();
        }

        public int CountGuestNumber(string connectionString)
        {
            int count = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(id) FROM guest";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        count = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }

            return count;
        }

        [HttpPost]
        public IActionResult Guest_Card_Create3(GuestUserViewModelNum Viewmodel)
        {
            var userRole = HttpContext.Session.GetString("Role");
            _logger.LogInformation($"Guest_Card_Create accessed. Role: {userRole}");
            string text = "192.168.0.63";
            string text2 = "3306";
            string text3 = "miss_db";
            string text4 = "user1";
            string text5 = "1234";
            string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";
            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Guest_Card_Create_menu");
                return RedirectToAction("Index", "Home");
            }

            string guest_number = Viewmodel.Guest_Number;
            ViewBag.Guest_num = guest_number;
            int iempnumber = int.Parse(guest_number);


            bool GC_state = GuestCard.make_GC(Viewmodel.Guest_Number);
            if (GC_state)
            {
                byte[] visit_img1 = Decryption_dll.Decrypt.Decrypt_Byte_Data(iempnumber, "visit_img1");
                byte[] visit_img2 = Decryption_dll.Decrypt.Decrypt_Byte_Data(iempnumber, "visit_img2");

                if (visit_img1 != null && visit_img2 != null)
                {
                    ViewBag.VisitImg1 = Convert.ToBase64String(visit_img1);
                    ViewBag.VisitImg2 = Convert.ToBase64String(visit_img2);
                }
                else
                {
                    _logger.LogWarning("Decrypted image data is null.");
                }
            }
            else
            {
                _logger.LogWarning("Decrypted image data is null.");
            }

            return View();
        }


        public IActionResult Guest_Card_Create()
        {
            var userRole = HttpContext.Session.GetString("Role");
            _logger.LogInformation($"Guest_Card_Create accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Guest_Card_Create.");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //OCR후 사원증 이미지 제작  및 사원번호, 임시 비밀번호 주는 사이트

        public List<string> Get_IdNullValue(string connectionString)
        {
            List<string> id = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT id FROM guest WHERE name IS NULL AND face_img1 IS NULL AND face_img2 IS NULL";   //visit_img1 IS NULL AND visit_img2 IS NULL AND

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id.Add(reader.GetString("id"));
                        }
                    }
                }
            }
            return id;
        }

        [HttpGet]
        public IActionResult Guest_Card_Create2(string name)
        {
            var userRole = HttpContext.Session.GetString("Role");
            string text = "192.168.0.63";
            string text2 = "3306";
            string text3 = "miss_db";
            string text4 = "user1";
            string text5 = "1234";
            string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            List<string> ids = Get_IdNullValue(connectionString);

            if (ids.Count == 0)
            {
                ViewBag.ErrorMessage = "모든 데이터가 이미 채워져 있습니다.";
                return View("guest_card_create2_error");
            }

            ViewBag.Guest_num = ids[0];
            ViewBag.name = name;
            return View();
        }



       
        [HttpPost]
        public IActionResult Guest_Card_Create2(GuestUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model is valid. Proceeding to save data.");

                int empNumber = int.Parse(viewModel.Emp_Number);

                // Base64 문자열에서 이미지 데이터 추출
                string base64Image = viewModel.WebcamImage;
                string base64Data = Regex.Match(base64Image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                byte[] face_img1 = Convert.FromBase64String(base64Data);

                // Encryption 및 데이터베이스 처리 로직
                MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();

                // ID만 넣는 함수 하면 넣기
                // mysqlhandler.Insert_UserIDPW(viewModel.Emp_Number);

                bool GC_state = GuestCard.register_GC(viewModel.Emp_Number, viewModel.NameString, face_img1);
                if (GC_state)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Image data could not be retrieved." });
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                _logger.LogWarning("Model is invalid.");
                return Json(new { success = false, message = "Model is invalid." });
            }
        }



        [HttpPost]
        public IActionResult PerformOCR([FromBody] ImageModel model)
        {
            try
            {
                var data = model.Image.Split(',')[1];
                var bytes = Convert.FromBase64String(data);

                using var srcImage = OpenCvSharp.Mat.FromImageData(bytes, OpenCvSharp.ImreadModes.Color);

                // Perform OCR directly on the captured image
                _idcard_detection_implement.PerformOCR(srcImage);

                string name = _idcard_detection_implement.LastDetectedName;
                string script = _idcard_detection_implement.Script;
                _idcard_detection_implement.Script = string.Empty;
                _idcard_detection_implement.LastDetectedName = string.Empty;

                return Json(new { success = true, message = "OCR performed successfully.", name = name, script = script });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error performing OCR: {ex.Message}" });
            }
        }
        [HttpPost]
        public IActionResult PerformOCR_Guest([FromBody] ImageModel model)
        {
            try
            {
                var data = model.Image.Split(',')[1];
                var bytes = Convert.FromBase64String(data);

                using var srcImage = OpenCvSharp.Mat.FromImageData(bytes, OpenCvSharp.ImreadModes.Color);

                // Perform OCR directly on the captured image
                _idcard_detection_implement.PerformOCR(srcImage);

                string name = _idcard_detection_implement.LastDetectedName;
                string script = _idcard_detection_implement.Script;
                _idcard_detection_implement.Script = string.Empty;
                _idcard_detection_implement.LastDetectedName = string.Empty;

                return Json(new { success = true, message = "OCR performed successfully.", name = name, script = script });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error performing OCR: {ex.Message}" });
            }
        }


        [HttpPost]
        public IActionResult Capture([FromBody] ImageModel model)
        {
            // 클라이언트로부터 받은 이미지 데이터를 ',' 기준으로 나누어 실제 이미지 데이터 부분을 추출
            var data = model.Image.Split(',')[1];
            // Base64 문자열을 바이트 배열로 변환
            var bytes = Convert.FromBase64String(data);

            // 바이트 배열을 OpenCV 이미지(Mat 객체)로 변환 (컬러 이미지로 읽기)
            using var srcImage = OpenCvSharp.Mat.FromImageData(bytes, OpenCvSharp.ImreadModes.Color);

            // 이미지를 그레이스케일로 변환
            using var grayImage = srcImage.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2GRAY);
            // 그레이스케일 이미지를 가우시안 블러 처리
            grayImage.GaussianBlur(new OpenCvSharp.Size(9, 9), 0);
            // 블러 처리된 이미지에 이진화(Threshold) 적용
            var thresholdOutput = grayImage.Threshold(155, 255, OpenCvSharp.ThresholdTypes.Binary);

            // 이진화된 이미지에서 외곽선을 찾음
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;
            Cv2.FindContours(thresholdOutput, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxSimple);

            // 외부 메서드를 사용하여 신분증 외곽선을 찾음
            var idCardContours = _idcard_detection_implement.FindIdCardContours(contours, srcImage);

            // 찾은 외곽선에 대해 사각형을 그려 표시
            foreach (var contour in idCardContours)
            {
                var rect = Cv2.BoundingRect(contour);
                Cv2.Rectangle(srcImage, rect, new OpenCvSharp.Scalar(0, 255, 0), 2);
            }

            // 처리된 이미지를 바이트 배열로 변환
            var processedImageBytes = srcImage.ToBytes(".png");
            // 바이트 배열을 Base64 문자열로 변환
            var processedImageBase64 = Convert.ToBase64String(processedImageBytes);
            // Base64 문자열을 데이터 URL 형식으로 변환
            var processedImageDataUrl = $"data:image/png;base64,{processedImageBase64}";

            // 외부 메서드에서 생성된 스크립트를 가져옴
            string script = _idcard_detection_implement.Script;
            // 외부 메서드의 스크립트 및 마지막 감지된 이름 초기화
            _idcard_detection_implement.Script = string.Empty;
            _idcard_detection_implement.LastDetectedName = string.Empty;

            // 처리된 이미지 데이터 URL과 스크립트를 포함하여 JSON 응답 반환
            return Json(new { success = true, message = "이미지가 성공적으로 처리되었습니다.", processedImage = processedImageDataUrl, script = script });
        }

        [HttpPost]
        public IActionResult UserResponse([FromBody] UserResponseModel model)
        {
            // 사용자가 이름을 확인한 경우
            if (model.Confirmed)
            {
                // 사용자가 확인한 이름을 변수에 저장
                string userName = model.Name;
                _idcard_detection_implement.LastDetectedName = userName;
                Console.WriteLine($"User confirmed name: {userName}");
            }
            else
            {
                // 사용자가 이름을 확인하지 않은 경우
                _idcard_detection_implement.LastDetectedName = null;
                Console.WriteLine("User did not confirm the name.");
            }

            // HTTP 200 OK 응답 반환
            return Ok();
        }


        public class ImageModel
        {
            // 클라이언트로부터 받은 이미지 데이터를 저장하는 모델 클래스
            public string Image { get; set; }
        }


        

        public IActionResult Logout()
        {
            // 세션 클리어
            HttpContext.Session.Clear();
            return View();
        }

       
        public IActionResult MyPage()
        {
            var username = HttpContext.Session.GetString("emp_number");
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Username = "";
                _logger.LogInformation("MyPage 접근: 사용자가 로그인하지 않았습니다.");
            }
            else
            {
                ViewBag.Username = username;
                _logger.LogInformation($"MyPage 접근: 로그인한 사용자 이름 - {username}");
            }
            return View();
        }

        
        [HttpPost]
        public IActionResult Login(UserInfo model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"로그인 시도: Employee_Number = {model.Emp_Number}");

                // Admin credentials check
                if (model.Emp_Number == "12345678admin" && model.Password == "1234")
                {
                    _logger.LogInformation($"관리자 로그인 성공: {model.Emp_Number}");
                    // 사원번호로 session 저장하기
                    HttpContext.Session.SetString("emp_number", model.Emp_Number); // or use the actual admin name
                    HttpContext.Session.SetString("Role", "Admin");
                    _logger.LogInformation($"세션 Role 설정 후: {HttpContext.Session.GetString("Role")}");
                    return Json(new { success = true });
                }

                // Regular user login check
                if (Login_Member(model.Emp_Number, model.Password))
                {
                    _logger.LogInformation($"로그인 성공: {model.Emp_Number}");
                    HttpContext.Session.SetString("emp_number", model.Emp_Number);
                    HttpContext.Session.SetString("Role", "User");
                    _logger.LogInformation($"세션 Role 설정 후: {HttpContext.Session.GetString("Role")}");
                    return Json(new { success = true });
                }

                _logger.LogWarning("로그인 실패: 다시 입력해주세요");
                return StatusCode(400, "로그인 실패: 다시 입력해주세요");
            }
            else
            {
                _logger.LogWarning("로그인 시도 실패: 모델 상태가 유효하지 않습니다.");
                return StatusCode(400, "로그인 시도 실패: 모델 상태가 유효하지 않습니다.");
            }
        }

        //general_user 테이블의 emp_number(사번)에 접근해서 해당 열의 face_img2 컬럼을 지우는 메서드
        

        public IActionResult Guest_Card_Receive()
        {
            var userRole = HttpContext.Session.GetString("Role");
            
            _logger.LogInformation($"Guest_Card_Create accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Guest_Card_Create.");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public IActionResult Guest_Card_Receive(string emp_number)
        {
            var userRole = HttpContext.Session.GetString("Role");
            

            _logger.LogInformation($"Guest_Card_Receive accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Guest_Card_Receive.");
                return Json(new { success = false, message = "Access denied." });
            }

            MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();
            mysqlhandler.ClearGuestData(emp_number);

            //ClearFaceImgData(emp_number, connectionString);

            return Json(new { success = true, message = "성공적으로 지웠습니다." });
        }



        private bool Login_Member(string emp_number, string password)
        {
            string connectionString = "Server=192.168.0.63;Port=3306;Database=miss_db;Uid=user1;Pwd=1234;";
            using MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            string cmdText = "SELECT COUNT(*) FROM general_user WHERE EMP_NUMBER = @emp_number AND PASSWORD = @password";
            using MySqlCommand mySqlCommand = new MySqlCommand(cmdText, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@emp_number", emp_number);
            mySqlCommand.Parameters.AddWithValue("@password", password);
            int num = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            return num == 1;
        }

        public IActionResult Emp_id_check()
        {
            var userRole = HttpContext.Session.GetString("Role");
            _logger.LogInformation($"Emp_id_check accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Emp_id_check.");
                return RedirectToAction("Index", "Home");
            }

            // 빈 리스트로 초기화하여 뷰로 전달
            return View(new List<Employee_CheckModel>());
        }

        [HttpPost]
        public IActionResult SearchEmployee_Check(string Emp_Number)
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Admin")
            {
                _logger.LogWarning("Unauthorized access attempt by non-admin user.");
                return RedirectToAction("Index", "Home");
            }

            List<Employee_CheckModel> employees = new List<Employee_CheckModel>();

            string text = "192.168.0.63";
            string text2 = "3306";
            string text3 = "miss_db";
            string text4 = "user1";
            string text5 = "1234";
            string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";

            if (string.IsNullOrEmpty(Emp_Number))
            {
                // 모든 사원번호 가져오기
                var empNumbers = GetAllEmpNumbers(connectionString);

                foreach (var empNumber in empNumbers)
                {
                    try
                    {
                        int emp_number2 = int.Parse(empNumber);
                        var employee = new Employee_CheckModel
                        {
                            EmpNumber = empNumber,
                            Name = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "name"),
                            Image1 = Convert.ToBase64String(Decryption_dll.Decrypt.Decrypt_Byte_Data(emp_number2, "employeeimg1")),
                            Image2 = Convert.ToBase64String(Decryption_dll.Decrypt.Decrypt_Byte_Data(emp_number2, "employeeimg2"))
                        };
                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while decrypting employee data for employee number {empNumber}.");
                    }
                }
            }
            else
            {
                // 특정 사원번호 검색
                try
                {
                    int emp_number2 = int.Parse(Emp_Number);
                    var employee = new Employee_CheckModel
                    {
                        EmpNumber = Emp_Number,
                        Name = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "name"),
                        Image1 = Convert.ToBase64String(Decryption_dll.Decrypt.Decrypt_Byte_Data(emp_number2, "employeeimg1")),
                        Image2 = Convert.ToBase64String(Decryption_dll.Decrypt.Decrypt_Byte_Data(emp_number2, "employeeimg2"))
                    };
                    employees.Add(employee);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while decrypting employee data.");
                    ViewBag.Message = "Error occurred while retrieving employee data.";
                }
            }

            if (employees.Count == 0)
            {
                ViewBag.ValidationMessage = "사원번호를 입력해주세요.";
            }

            return View("Emp_id_check", employees);
        }


        public List<string> GetAllEmpNumbers(string connectionString)
        {
            List<string> empNumbers = new List<string>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT emp_number FROM General_user";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                empNumbers.Add(reader["emp_number"].ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }

            return empNumbers;
        }

        public List<string> GetAllGuestNumbers(string connectionString)
        {
            List<string> GuestNumbers = new List<string>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id FROM guest";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GuestNumbers.Add(reader["id"].ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }

            return GuestNumbers;
        }


        public IActionResult Guest_id_check()
        {
            var userRole = HttpContext.Session.GetString("Role");
            _logger.LogInformation($"Guest_Card_Create accessed. Role: {userRole}");

            if (userRole != "Admin")
            {
                _logger.LogWarning("Access denied. Non-admin user trying to access Guest_id_check");
                return RedirectToAction("Index", "Home");
            }
            string text = "192.168.0.63";
            string text2 = "3306";
            string text3 = "miss_db";
            string text4 = "user1";
            string text5 = "1234";
            string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";
            // Get guest cards from database
            List<GuestCheckModel> guestCards = GetGuestCards(connectionString);

            return View(guestCards);
        }

        //[HttpPost]
        //public IActionResult ReturnGuestCard(string guestCardNumber)
        //{
        //    var userRole = HttpContext.Session.GetString("Role");
        //    if (userRole != "Admin")
        //    {
        //        _logger.LogWarning("Unauthorized access attempt by non-admin user.");
        //        return RedirectToAction("Index", "Home");
        //    }

        //    try
        //    {
        //        MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();
        //        mysqlhandler.ClearGuestData(guestCardNumber);

        //        TempData["Message"] = "Guest card returned successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while returning guest card.");
        //        TempData["Message"] = "Error returning guest card.";
        //    }

        //    return RedirectToAction("Guest_id_check");
        //}

        [HttpPost]
        public IActionResult ReturnGuestCard(string guestCardNumber)
        {
            try
            {
                // 방문증 반납 로직 수행
                MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();
                bool result = mysqlhandler.ClearGuestData(guestCardNumber);

                if (result)
                {
                    return Json(new { success = true, message = "반납이 성공적으로 완료되었습니다." });
                }
                else
                {
                    return Json(new { success = false, message = "반납에 실패하였습니다." });
                }
            }
            catch (Exception ex)
            {
                // 에러 처리
                return Json(new { success = false, message = "오류가 발생하였습니다: " + ex.Message });
            }
        }


        private List<GuestCheckModel> GetGuestCards(string connectionString)
        {
            List<GuestCheckModel> guestCards = new List<GuestCheckModel>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id FROM guest";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string guestNumber = reader["id"].ToString();
                                string name = string.Empty;
                                try
                                {
                                    name = Decryption_dll.Decrypt.Decrypt_String_Data(int.Parse(guestNumber), "name");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error while decrypting name for guest number {guestNumber}");
                                }

                                guestCards.Add(new GuestCheckModel
                                {
                                    Guest_Number = guestNumber,
                                    Name = string.IsNullOrEmpty(name) ? "비어있음" : name
                                });
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }

            return guestCards;
        }



        // 에러 페이지를 반환하는 액션 메서드. 캐싱을 비활성화하는 어트리뷰트가 적용됨
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // ErrorViewModel 인스턴스를 생성하고, 현재 Activity의 Id 또는 HttpContext의 TraceIdentifier를 RequestId로 설정하여 뷰에 전달
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}