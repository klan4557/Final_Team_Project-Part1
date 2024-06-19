using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC ��Ʈ�ѷ��� �׼� �޼��带 ����ϱ� ���� ���ӽ����̽�
using MVCTest1.Models; // MVCTest1 ������Ʈ�� Models ������ �ִ� Ŭ������ ����ϱ� ���� ���ӽ����̽�
using System.Diagnostics; // �ý��� ���ܿ� �ʿ��� Ŭ������ ����ϱ� ���� ���ӽ����̽�
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
// MVCTest1 ������Ʈ�� Controllers ���� ���� HomeController Ŭ������ ����
using MVCTest1.TcpIp;
using Relate_GuestCard;
using System.Xml.Linq;
using MySQL_dll;
using Google.Protobuf;
using FaceRecognitionDotNet.Extensions;
using System.Drawing;

namespace MVCTest1.Controllers
{
    // Controller Ŭ������ ��ӹ޾� HomeController Ŭ������ ����
    public class HomeController : Controller
    {
        
        // ILogger �������̽��� ����Ͽ� �α� ����� ����. HomeController Ÿ���� �ΰ� �ν��Ͻ��� ����
        // �����ͺ��̽� ���ؽ�Ʈ ��ü�� ������ ������ ���� �ʵ��Դϴ�.
        // �����ڸ� ���� ApplicationDbContext Ÿ���� �ν��Ͻ��� �޾ƿͼ� _context �ʵ带 �ʱ�ȭ�մϴ�. �̴� DI(Dependency Injection) ������ ����մϴ�.
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly idcard_detection_interface _idcard_detection_implement;
        private readonly FaceDetection _faceDetection;

        //private static readonly Dictionary<string, List<LogSuccessData>> Logdatastore = new Dictionary<string, List<LogSuccessData>>();
        // ���� �����ڷ� ��� ������ ����
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

        // Index �������� ��ȯ�ϴ� �׼� �޼��� IActionResult
        public IActionResult Index()
        {
            _logger.LogInformation($"���� Role ���� ��: {HttpContext.Session.GetString("Role")}");
            return View(); // �⺻������ Views/Home/Index.cshtml �� ������ ã�Ƽ� ��ȯ
        }

        // Privacy �������� ��ȯ�ϴ� �׼� �޼���
        public IActionResult Privacy()
        {
            return View(); // �⺻������ Views/Home/Privacy.cshtml �� ������ ã�Ƽ� ��ȯ
        }

        // login �������� ��ȯ�ϴ� �׼� �޼���
        public IActionResult Login()
        {
            return View(); // �⺻������ Views/Home/Login.cshtml �� ������ ã�Ƽ� ��ȯ
        }

        public IActionResult Modify()
        {
            var emp_number = HttpContext.Session.GetString("emp_number");
            var userRole = HttpContext.Session.GetString("Role");

            ViewBag.UserRole = userRole;

            if (string.IsNullOrEmpty(emp_number) && userRole != "Admin")
            {
                ViewBag.emp_number = "";
                _logger.LogInformation("Modify ����: ����ڰ� �α������� �ʾҽ��ϴ�.");
                return RedirectToAction("Index", "Home");
            }

            if (userRole == "User")
            {
                ViewBag.emp_number = emp_number;
                _logger.LogInformation($"Modify ����: �α����� ����� �̸� - {emp_number}");
                int emp_number2 = int.Parse(emp_number);

                ViewBag.User_name = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "name");
                ViewBag.User_pos = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "pos");
                ViewBag.User_tel = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "tel");
                ViewBag.User_email = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "email");
                ViewBag.User_address = Decryption_dll.Decrypt.Decrypt_String_Data(emp_number2, "address");
            }
            else if (userRole == "Admin")
            {
                _logger.LogInformation("Admin ����: ��� ���� ��ȸ �� ����");
                // �ʱ⿡�� �ƹ� ������ ��ȸ���� ����
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
                ModelState.AddModelError("Emp_Number", "�����ȣ�� �Է����ּ���.");
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

                // Encryption �� �����ͺ��̽� ó�� ����
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

                return RedirectToAction("Index", "Home"); // ���� �� Ȩ���� ���𷺼�
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
                // empNumber�� ������� ��� ��� �α׸� ������
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
        //OCR�� ����� �̹��� ����  �� �����ȣ, �ӽ� ��й�ȣ �ִ� ����Ʈ

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
                Console.WriteLine("�����ͺ��̽� ������ �߻��߽��ϴ�: " + ex.Message);
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

            //ī��Ʈ �Ʒ��� ������ ��
            
            ViewBag.email = "moble@team5.com";
            ViewBag.address = "õ�Ƚ� ����� 255 3�� ���";
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
            //count�� 1�ڸ� �����̸� num�� 00�߰��ؼ� �ֱ�
            //count�� 2�ڸ� �����̸� num�� 0�߰��ؼ� �ֱ�

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

                // Base64 ���ڿ����� �̹��� ������ ����
                string base64Image = viewModel.WebcamImage;
                string base64Data = Regex.Match(base64Image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                byte[] face_img1 = Convert.FromBase64String(base64Data);

                // Encryption �� �����ͺ��̽� ó�� ����
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
                Console.WriteLine("�����ͺ��̽� ������ �߻��߽��ϴ�: " + ex.Message);
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

        //OCR�� ����� �̹��� ����  �� �����ȣ, �ӽ� ��й�ȣ �ִ� ����Ʈ

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
                ViewBag.ErrorMessage = "��� �����Ͱ� �̹� ä���� �ֽ��ϴ�.";
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

                // Base64 ���ڿ����� �̹��� ������ ����
                string base64Image = viewModel.WebcamImage;
                string base64Data = Regex.Match(base64Image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                byte[] face_img1 = Convert.FromBase64String(base64Data);

                // Encryption �� �����ͺ��̽� ó�� ����
                MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();

                // ID�� �ִ� �Լ� �ϸ� �ֱ�
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
            // Ŭ���̾�Ʈ�κ��� ���� �̹��� �����͸� ',' �������� ������ ���� �̹��� ������ �κ��� ����
            var data = model.Image.Split(',')[1];
            // Base64 ���ڿ��� ����Ʈ �迭�� ��ȯ
            var bytes = Convert.FromBase64String(data);

            // ����Ʈ �迭�� OpenCV �̹���(Mat ��ü)�� ��ȯ (�÷� �̹����� �б�)
            using var srcImage = OpenCvSharp.Mat.FromImageData(bytes, OpenCvSharp.ImreadModes.Color);

            // �̹����� �׷��̽����Ϸ� ��ȯ
            using var grayImage = srcImage.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2GRAY);
            // �׷��̽����� �̹����� ����þ� �� ó��
            grayImage.GaussianBlur(new OpenCvSharp.Size(9, 9), 0);
            // �� ó���� �̹����� ����ȭ(Threshold) ����
            var thresholdOutput = grayImage.Threshold(155, 255, OpenCvSharp.ThresholdTypes.Binary);

            // ����ȭ�� �̹������� �ܰ����� ã��
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;
            Cv2.FindContours(thresholdOutput, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxSimple);

            // �ܺ� �޼��带 ����Ͽ� �ź��� �ܰ����� ã��
            var idCardContours = _idcard_detection_implement.FindIdCardContours(contours, srcImage);

            // ã�� �ܰ����� ���� �簢���� �׷� ǥ��
            foreach (var contour in idCardContours)
            {
                var rect = Cv2.BoundingRect(contour);
                Cv2.Rectangle(srcImage, rect, new OpenCvSharp.Scalar(0, 255, 0), 2);
            }

            // ó���� �̹����� ����Ʈ �迭�� ��ȯ
            var processedImageBytes = srcImage.ToBytes(".png");
            // ����Ʈ �迭�� Base64 ���ڿ��� ��ȯ
            var processedImageBase64 = Convert.ToBase64String(processedImageBytes);
            // Base64 ���ڿ��� ������ URL �������� ��ȯ
            var processedImageDataUrl = $"data:image/png;base64,{processedImageBase64}";

            // �ܺ� �޼��忡�� ������ ��ũ��Ʈ�� ������
            string script = _idcard_detection_implement.Script;
            // �ܺ� �޼����� ��ũ��Ʈ �� ������ ������ �̸� �ʱ�ȭ
            _idcard_detection_implement.Script = string.Empty;
            _idcard_detection_implement.LastDetectedName = string.Empty;

            // ó���� �̹��� ������ URL�� ��ũ��Ʈ�� �����Ͽ� JSON ���� ��ȯ
            return Json(new { success = true, message = "�̹����� ���������� ó���Ǿ����ϴ�.", processedImage = processedImageDataUrl, script = script });
        }

        [HttpPost]
        public IActionResult UserResponse([FromBody] UserResponseModel model)
        {
            // ����ڰ� �̸��� Ȯ���� ���
            if (model.Confirmed)
            {
                // ����ڰ� Ȯ���� �̸��� ������ ����
                string userName = model.Name;
                _idcard_detection_implement.LastDetectedName = userName;
                Console.WriteLine($"User confirmed name: {userName}");
            }
            else
            {
                // ����ڰ� �̸��� Ȯ������ ���� ���
                _idcard_detection_implement.LastDetectedName = null;
                Console.WriteLine("User did not confirm the name.");
            }

            // HTTP 200 OK ���� ��ȯ
            return Ok();
        }


        public class ImageModel
        {
            // Ŭ���̾�Ʈ�κ��� ���� �̹��� �����͸� �����ϴ� �� Ŭ����
            public string Image { get; set; }
        }


        

        public IActionResult Logout()
        {
            // ���� Ŭ����
            HttpContext.Session.Clear();
            return View();
        }

       
        public IActionResult MyPage()
        {
            var username = HttpContext.Session.GetString("emp_number");
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Username = "";
                _logger.LogInformation("MyPage ����: ����ڰ� �α������� �ʾҽ��ϴ�.");
            }
            else
            {
                ViewBag.Username = username;
                _logger.LogInformation($"MyPage ����: �α����� ����� �̸� - {username}");
            }
            return View();
        }

        
        [HttpPost]
        public IActionResult Login(UserInfo model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"�α��� �õ�: Employee_Number = {model.Emp_Number}");

                // Admin credentials check
                if (model.Emp_Number == "12345678admin" && model.Password == "1234")
                {
                    _logger.LogInformation($"������ �α��� ����: {model.Emp_Number}");
                    // �����ȣ�� session �����ϱ�
                    HttpContext.Session.SetString("emp_number", model.Emp_Number); // or use the actual admin name
                    HttpContext.Session.SetString("Role", "Admin");
                    _logger.LogInformation($"���� Role ���� ��: {HttpContext.Session.GetString("Role")}");
                    return Json(new { success = true });
                }

                // Regular user login check
                if (Login_Member(model.Emp_Number, model.Password))
                {
                    _logger.LogInformation($"�α��� ����: {model.Emp_Number}");
                    HttpContext.Session.SetString("emp_number", model.Emp_Number);
                    HttpContext.Session.SetString("Role", "User");
                    _logger.LogInformation($"���� Role ���� ��: {HttpContext.Session.GetString("Role")}");
                    return Json(new { success = true });
                }

                _logger.LogWarning("�α��� ����: �ٽ� �Է����ּ���");
                return StatusCode(400, "�α��� ����: �ٽ� �Է����ּ���");
            }
            else
            {
                _logger.LogWarning("�α��� �õ� ����: �� ���°� ��ȿ���� �ʽ��ϴ�.");
                return StatusCode(400, "�α��� �õ� ����: �� ���°� ��ȿ���� �ʽ��ϴ�.");
            }
        }

        //general_user ���̺��� emp_number(���)�� �����ؼ� �ش� ���� face_img2 �÷��� ����� �޼���
        

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

            return Json(new { success = true, message = "���������� �������ϴ�." });
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

            // �� ����Ʈ�� �ʱ�ȭ�Ͽ� ��� ����
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
                // ��� �����ȣ ��������
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
                // Ư�� �����ȣ �˻�
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
                ViewBag.ValidationMessage = "�����ȣ�� �Է����ּ���.";
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
                Console.WriteLine("�����ͺ��̽� ������ �߻��߽��ϴ�: " + ex.Message);
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
                Console.WriteLine("�����ͺ��̽� ������ �߻��߽��ϴ�: " + ex.Message);
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
                // �湮�� �ݳ� ���� ����
                MySQL_dll.Handler mysqlhandler = new MySQL_dll.Handler();
                bool result = mysqlhandler.ClearGuestData(guestCardNumber);

                if (result)
                {
                    return Json(new { success = true, message = "�ݳ��� ���������� �Ϸ�Ǿ����ϴ�." });
                }
                else
                {
                    return Json(new { success = false, message = "�ݳ��� �����Ͽ����ϴ�." });
                }
            }
            catch (Exception ex)
            {
                // ���� ó��
                return Json(new { success = false, message = "������ �߻��Ͽ����ϴ�: " + ex.Message });
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
                                    Name = string.IsNullOrEmpty(name) ? "�������" : name
                                });
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("�����ͺ��̽� ������ �߻��߽��ϴ�: " + ex.Message);
            }

            return guestCards;
        }



        // ���� �������� ��ȯ�ϴ� �׼� �޼���. ĳ���� ��Ȱ��ȭ�ϴ� ��Ʈ����Ʈ�� �����
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // ErrorViewModel �ν��Ͻ��� �����ϰ�, ���� Activity�� Id �Ǵ� HttpContext�� TraceIdentifier�� RequestId�� �����Ͽ� �信 ����
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}