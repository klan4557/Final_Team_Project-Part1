//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using System.Text.Json;
//using MVCTest1.Models;

//namespace MVCTest1.TcpIp
//{
//    public class Tcp_ip
//    {
//        private readonly int _port;
//        // log 관리를 위한 변수 앞에 string은 emp_number 뒤에는 로그 내용
//        private static readonly Dictionary<string, List<QRSuccessData>> QRdatastore = new Dictionary<string, List<QRSuccessData>>();
//        public static readonly Dictionary<string, List<LogSuccessData>> Logdatastore = new Dictionary<string, List<LogSuccessData>>();

//        // 내가 받아서 저장하기 위한 클래스
//        private class QRSuccessData
//        {
//            public string QR { get; set; }
//            public string Success { get; set; }
//        }

//        private class ReceivedData
//        {
//            public string Id { get; set; }
//            public string QR { get; set; }
//            public string Success { get; set; }
//            public string Time { get; set; }
//            public string Commute { get; set; }
//        }

//        private static bool IsEightDigitNumber(string message)
//        {
//            return message.Length == 8 && long.TryParse(message, out _);
//        }

//        public Tcp_ip(int port)
//        {
//            _port = port;
//        }

//        public async Task StartAsync()
//        {
//            TcpListener server = null;
//            try
//            {
//                server = new TcpListener(IPAddress.Any, _port);
//                server.Start();
//                Console.WriteLine($"Server started on port {_port}");

//                while (true)
//                {
//                    TcpClient client = await server.AcceptTcpClientAsync();
//                    _ = HandleClientAsync(client);
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine($"Exception: {e}");
//            }
//            finally
//            {
//                server?.Stop();
//            }
//        }

//        private async Task HandleClientAsync(TcpClient client)
//        {

//            try
//            {


//                // 소켓 프로그래밍을 할 때 NetworkStream을 사용
//                NetworkStream stream = client.GetStream();
//                byte[] buffer = new byte[256];
//                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
//                bool face_state = false;

//                // Json 통신 예상해서 만든 코드
//                var receivedData = JsonSerializer.Deserialize<ReceivedData>(message);

//                if (receivedData != null && IsEightDigitNumber(receivedData.Id))
//                {
//                    if (!QRdatastore.ContainsKey(receivedData.Id))
//                    {
//                        QRdatastore[receivedData.Id] = new List<QRSuccessData>();
//                    }

//                    if (!string.IsNullOrEmpty(receivedData.Success))
//                    {
//                        QRdatastore[receivedData.Id].Add(new QRSuccessData { Success = receivedData.Success});
//                        Console.WriteLine($"Added '({receivedData.Success})' to list for key {receivedData.Id}");

//                        // Print all values for the given key
//                        Console.WriteLine($"Values for key {receivedData.Id}:");

//                        //사실상 여기서 이제 얼굴 비교 하고 라파에 결과 송신 코드를 넣어야 함

//                        byte[] response = Encoding.ASCII.GetBytes("Face True");        //결과 송신
//                        await stream.WriteAsync(response, 0, response.Length);
//                        QRdatastore.Remove(receivedData.Id);
//                        QRdatastore.TrimExcess();
//                    }
//                }

//                if (receivedData != null && IsEightDigitNumber(receivedData.Id))
//                {
//                    if (!Logdatastore.ContainsKey(receivedData.Id))
//                    {
//                        Logdatastore[receivedData.Id] = new List<LogSuccessData>();
//                    }

//                    if (!string.IsNullOrEmpty(receivedData.Time))
//                    {
//                        Logdatastore[receivedData.Id].Add(new LogSuccessData { Id = receivedData.Id, Time = receivedData.Time, Commute = receivedData.Commute });
//                        Console.WriteLine($"Added '({receivedData.Time})' '({receivedData.Commute})' to list for key {receivedData.Id}");

//                        // Print all values for the given key
//                        Console.WriteLine($"Values for key {receivedData.Id}:");
//                        foreach (var value in Logdatastore[receivedData.Id])
//                        {
//                            Console.WriteLine($"(Time: {value.Time})");
//                        }
//                    }
//                }

//                //if (message == "success")
//                //{
//                //    Console.WriteLine("success");
//                //    Thread.Sleep(5000);
//                //    //비교하고 face_img2 지우기
//                //    byte[] response = Encoding.ASCII.GetBytes("ok");
//                //    await stream.WriteAsync(response, 0, response.Length);

//                //}
//                //if (message == "fuck")
//                //{
//                //    Console.WriteLine("fuck");
//                //    Thread.Sleep(5000);
//                //    byte[] response = Encoding.ASCII.GetBytes("ok");
//                //    await stream.WriteAsync(response, 0, response.Length);

//                //}

//                // 메시지가 시간 형식으로 올 것으로 기대
//                //if (DateTime.TryParse(message, out DateTime receivedTime))
//                //{
//                //    Console.WriteLine($"Received time: {receivedTime}");
//                //    //byte[] response = Encoding.ASCII.GetBytes("2");
//                //    //await stream.WriteAsync(response, 0, response.Length);
//                //}
//                //else
//                //{
//                //    byte[] response = Encoding.ASCII.GetBytes("Invalid time format");
//                //    await stream.WriteAsync(response, 0, response.Length);
//                //}

//                stream.Close();
//                client.Close();
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine($"Exception: {e}");
//            }
//        }
//    }
//}
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MVCTest1.Models;
using System.Windows;
using Use_AI;
using MySQL_dll;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.SignalR;
namespace MVCTest1.TcpIp
{
    public class Tcp_ip
    {
        private readonly int _port;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<Tcp_ip> _logger;

        private static readonly Dictionary<string, List<QRSuccessData>> QRdatastore = new Dictionary<string, List<QRSuccessData>>();
        public static readonly Dictionary<string, List<LogSuccessData>> Logdatastore = new Dictionary<string, List<LogSuccessData>>();

        private class QRSuccessData
        {
            public string QR { get; set; }
            public string QRSuccess { get; set; }

        }


        private class ReceivedData
        {
            public string Id { get; set; }
            public string QR { get; set; }
            public string QRSuccess { get; set; }
            public string Time { get; set; }
            public string Commute { get; set; }
            public string Notice { get; set; }


        }

        private class NoticeOnlyData
        {
            public string Notice { get; set; }
        }


        //private static bool IsEightDigitNumber(string message)
        //{
        //    return message.Length >= 1 && long.TryParse(message, out _);
        //}

        private static bool IsEightDigitNumber(string message)
        {
            return !string.IsNullOrEmpty(message) && message.Length >= 1 && message.Length <= 8 && long.TryParse(message, out _);
        }

        private static bool IsThreeDigitNumber(string message)
        {
            return message.Length <= 7 && long.TryParse(message, out _);
        }

        public Tcp_ip(int port, IHubContext<NotificationHub> hubContext, ILogger<Tcp_ip> logger)
        {
            _port = port;
            _hubContext = hubContext;
            _logger = logger;
        }


        //public bool ClearFaceImgData(string number, string connectionString)
        //{
        //    try
        //    {
        //        using (MySqlConnection connection = new MySqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            if (number.Length >= 4)
        //            {
        //                string query = @"UPDATE general_user SET face_img2 = NULL WHERE emp_number = @emp_number";

        //                using (MySqlCommand command = new MySqlCommand(query, connection))
        //                {
        //                    command.Parameters.AddWithValue("@emp_number", number);

        //                    int rowsAffected = command.ExecuteNonQuery();

        //                    return rowsAffected > 0;
        //                }
        //            }
        //            if (number.Length < 4)
        //            {
        //                string query = @"UPDATE guest SET face_img2 = NULL WHERE id = @id";

        //                using (MySqlCommand command = new MySqlCommand(query, connection))
        //                {
        //                    command.Parameters.AddWithValue("@id", number);

        //                    int rowsAffected = command.ExecuteNonQuery();

        //                    return rowsAffected > 0;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }


        //        }
        //    }
        //    catch (MySqlException ex)
        //    {
        //        Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
        //        return false;
        //    }
        //}
        public bool ClearFaceImgData(string number, string connectionString)
        {
            if (string.IsNullOrEmpty(number))
            {
                Console.WriteLine("Error: 'number' is null or empty.");
                return false;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query;

                    if (number.Length >= 4)
                    {
                        query = @"UPDATE general_user SET face_img2 = NULL WHERE emp_number = @emp_number";
                    }
                    else
                    {
                        query = @"UPDATE guest SET face_img2 = NULL WHERE id = @id";
                    }

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (number.Length >= 4)
                        {
                            command.Parameters.AddWithValue("@emp_number", number);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@id", number);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
                return false;
            }
        }



        public async Task StartAsync()
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Any, _port);
                server.Start();
                Console.WriteLine($"Server started on port {_port}");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                server?.Stop();
            }
        }



        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[256];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                bool face_state = false;

                var receivedData = JsonSerializer.Deserialize<ReceivedData>(message);
                var noticeOnlyData = JsonSerializer.Deserialize<NoticeOnlyData>(message);

                string text = "192.168.0.63";
                string text2 = "3306";
                string text3 = "miss_db";
                string text4 = "user1";
                string text5 = "1234";
                string connectionString = "Server=" + text + ";Port=" + text2 + ";Database=" + text3 + ";Uid=" + text4 + ";Pwd=" + text5 + ";";



                if (receivedData != null && !string.IsNullOrEmpty(receivedData.Notice))
                {
                    _logger.LogInformation($"Received notice from receivedData: {receivedData.Notice}");
                    // 클라이언트에게 알림을 보냄
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", receivedData.Notice);
                }
                else if (noticeOnlyData != null && !string.IsNullOrEmpty(noticeOnlyData.Notice))
                {
                    _logger.LogInformation($"Received notice from noticeOnlyData: {noticeOnlyData.Notice}");
                    // 클라이언트에게 알림을 보냄
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", noticeOnlyData.Notice);
                }




                if (receivedData != null && IsEightDigitNumber(receivedData.Id))
                {
                    byte[] face_img2;
                    if (!Logdatastore.ContainsKey(receivedData.Id))
                    {
                        Logdatastore[receivedData.Id] = new List<LogSuccessData>();
                    }

                    if (!string.IsNullOrEmpty(receivedData.QRSuccess))
                    {
                        double result = AI.recognize_face(receivedData.Id);
                        if (result > 60.0)
                        {
                            byte[] response = Encoding.ASCII.GetBytes("Face True");
                            Console.WriteLine("Face True");
                            await stream.WriteAsync(response, 0, response.Length);
                        }
                        else if (result == -1)
                        {
                            byte[] response = Encoding.ASCII.GetBytes("None Face");
                            Console.WriteLine("None Face");
                            await stream.WriteAsync(response, 0, response.Length);
                        }
                        else
                        {
                            byte[] response = Encoding.ASCII.GetBytes("Face False");
                            await stream.WriteAsync(response, 0, response.Length);

                            int Id = int.Parse(receivedData.Id);
                            if(receivedData.Id.Length <4)
                            {
                                face_img2 = Decryption_dll.Decrypt.Decrypt_Byte_Data(Id, "face_img2");
                                //faceBase64 = Convert.ToBase64String(face_img2);
                            }
                            else
                            {
                                face_img2 = Decryption_dll.Decrypt.Decrypt_Byte_Data(Id, "faceimg2");
                                //faceBase64 = Convert.ToBase64String(face_img2);
                            }
                            

                            if (face_img2 != null)
                            {
                                
                                Console.WriteLine($"Face image Base64: {face_img2}");
                            }
                            else
                            {
                                Console.WriteLine("Face image is null");
                            }
                            // 여기서 count로 일반사원 방문자 번호 구분해서 해야될듯
                            string FalseTime = DateTime.Now.ToString("yyyy:MM:dd T HH:mm:ss");
                            Logdatastore[receivedData.Id].Add(new LogSuccessData { Id = receivedData.Id, Time = FalseTime, Commute = "3", Face2 = face_img2 });
                            foreach (var value in Logdatastore[receivedData.Id])
                            {
                                Console.WriteLine($"(Time: {value.Time})");
                            }
                        }

                        QRdatastore.Remove(receivedData.Id);
                        QRdatastore.TrimExcess();
                    }
                }


                if (receivedData != null && IsEightDigitNumber(receivedData.Id))
                {
                    if (!Logdatastore.ContainsKey(receivedData.Id))
                    {
                        Logdatastore[receivedData.Id] = new List<LogSuccessData>();
                    }

                    if (!string.IsNullOrEmpty(receivedData.Time) && receivedData.Commute !="3")
                    {
                        // Convert time to ISO 8601 format (yyyy-MM-ddTHH:mm:ss)
                        int id = int.Parse(receivedData.Id);
                        Logdatastore[receivedData.Id].Add(new LogSuccessData { Id = receivedData.Id, Time = receivedData.Time, Commute = receivedData.Commute});
                        Console.WriteLine($"Added '({receivedData.Time})' '({receivedData.Commute})' to list for key {receivedData.Id}");

                        byte[] response = Encoding.ASCII.GetBytes("DoorOK");
                        await stream.WriteAsync(response, 0, response.Length);

                        foreach (var value in Logdatastore[receivedData.Id])
                        {
                            Console.WriteLine($"(Time: {value.Time})");
                        }

                        
                    }
                }


                if (!string.IsNullOrEmpty(receivedData?.Id))
                {
                    ClearFaceImgData(receivedData.Id, connectionString);
                }
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                _logger.LogError($"Exception: {e}");

            }
        }
    }
}

