using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace MySQL
{
    // MySQLHandler 클래스: MySQL 데이터베이스와 상호 작용하기 위한 클래스
    public class MySQLHandler
    {
        // 데이터베이스 연결 문자열을 저장하는 필드
        private string connectionString;

        // MySQLHandler 생성자: 데이터베이스 연결을 위한 초기화 작업을 수행함
        public MySQLHandler()
        {
            // 데이터베이스 연결 정보 설정
            string server = "192.168.0.63";
            string port = "3306";
            string database = "miss_db";
            string id = "user1";
            string pw = "1234";
            // 연결 문자열 설정
            connectionString = $"Server={server};Port={port};Database={database};Uid={id};Pwd={pw};";
        }


        //GeneralUser 테이블에 접근하여 id,pw 값 전달하는 메서드
        public bool Insert_UserIDPW(string emp_number, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                //수정한 부분 변수 query 요청 하는 부분을 변경하였음 
                string query = "INSERT INTO General_user (EMP_NUMBER, password)" +
                               "VALUES(@emp_number, @password)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "PASSWORD = VALUES(PASSWORD)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@password", password);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 이름 값 전달하는 메서드
        public bool Insert_UserName(string emp_number, byte[] name)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, NAME)" +
                               "VALUES(@emp_number, @name)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "NAME = VALUES(NAME)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@name", name);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 직책 값 전달하는 메서드
        public bool Insert_UserPosition(string emp_number, byte[] position)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, POSITION)" +
                               "VALUES(@emp_number, @position)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "POSITION = VALUES(POSITION)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@position", position);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 전화번호 전달하는 메서드
        public bool Insert_UserTel(string emp_number, byte[] tel)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, TEL)" +
                               "VALUES(@emp_number, @tel)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "tel = VALUES(TEL)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@tel", tel);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 이메일 전달하는 메서드
        public bool Insert_UserEmail(string emp_number, byte[] email)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, EMAIL)" +
                               "VALUES(@emp_number, @email)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "email = VALUES(EMAIL)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@email", email);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 이메일 전달하는 메서드
        public bool Insert_Address(string emp_number, byte[] address)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, ADDRESS)" +
                               "VALUES(@emp_number, @address)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "address = VALUES(ADDRESS)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@address", address);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 사원의 사진 전달하는 메서드
        public bool Insert_FaceImg1(string emp_number, byte[] face_img1)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, FACE_IMG1)" +
                               "VALUES(@emp_number, @face_img1)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "face_img1 = VALUES(FACE_IMG1)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@face_img1", face_img1);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 웹서버에서 찍은 사원의 비교 사진 전달 메서드
        public bool Insert_FaceImg2(string emp_number, byte[] face_img2)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, FACE_IMG2)" +
                               "VALUES(@emp_number, @face_img2)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "face_img2 = VALUES(FACE_IMG2)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@face_img2", face_img2);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 사원증 앞면 이미지 전달하는 메서드
        public bool Insert_EmpImg1(string emp_number, byte[] employee_img1)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, EMPLOYEE_IMG1)" +
                               "VALUES(@emp_number, @employee_img1)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "employee_img1 = VALUES(EMPLOYEE_IMG1)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@employee_img1", employee_img1);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 사원증 뒷면 이미지 전달하는 메서드
        public bool Insert_EmpImg2(string emp_number, byte[] employee_img2)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, EMPLOYEE_IMG2)" +
                               "VALUES(@emp_number, @employee_img2)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "employee_img2 = VALUES(EMPLOYEE_IMG2)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@employee_img2", employee_img2);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 완성된 명함 앞면 사진 전달하는 메서드
        public bool Insert_BusinessImg1(string emp_number, byte[] business_img1)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, BUSINESS_IMG1)" +
                               "VALUES(@emp_number, @business_img1)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "business_img1 = VALUES(BUSINESS_IMG1)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@business_img1", business_img1);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 완성된 명함 뒷면 사진 전달하는 메서드
        public bool Insert_BusinessImg2(string emp_number, byte[] business_img2)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, BUSINESS_IMG2)" +
                               "VALUES(@emp_number, @business_img2)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "business_img2 = VALUES(BUSINESS_IMG2)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@business_img2", business_img2);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //GeneralUser 테이블의 emp_number에 접근하여 해당 사원정보의 암호화 key, iv 값 전달하는 메서드
        public bool Insert_UserKeyIv(string emp_number, byte[] user_key, byte[] user_iv)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, USER_KEY, USER_IV)" +
                               "VALUES(@emp_number, @user_key, @user_iv)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "USER_KEY = VALUES(USER_KEY), USER_IV = VALUES(USER_IV)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@user_key", user_key);
                    command.Parameters.AddWithValue("@user_iv", user_iv);


                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //유저 정보 로그를 전송하는 메서드
        public bool Insert_UserLog(string emp_number, byte[] user_log)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO General_user (EMP_NUMBER, USER_LOG)" +
                               "VALUES(@emp_number, @user_log)" +
                               "ON DUPLICATE KEY UPDATE " +
                               "USER_LOG = VALUES(USER_LOG)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@user_log", user_log);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        //로그인 후 회원정보 수정을 위한 메서드
        public bool UpdateUserPassword(string emp_number, string newpw)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE General_user SET PASSWORD = @newPassword WHERE EMP_NUMBER = @emp_number";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@emp_number", emp_number);
                        command.Parameters.AddWithValue("@newPassword", newpw);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
                return false;
            }
        }

        //로그인 - 정보 조회
        public bool Login_Member(string emp_number, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM general_user WHERE EMP_NUMBER = @emp_number AND PASSWORD = @password";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@emp_number", emp_number);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count == 1;

                }
            }
        }

        //General_user 테이블에서 key, iv값 가져오는 메서드
        //emp_number(사원번호)는 일반적으로 다른 정보를 가져올때 emp_number에 접근해서 가져오기 때문에 굳이 가져오지 않아도 읽어올 수 있어서
        //따로 정의하지 않음. pw는 varchar형으로 선언되어있기때문에 암호화 항목에 들어가지 않아 쓰임새가 따로 없다고 생각하기 떄문에 만들지 않음
        public (byte[], byte[]) Get_KeyIv(string emp_number)
        {
            //위와 같은 형태로 메서드를 선언한 이유 : 메서드가 두 개의 byte[] 값을 반환하도록 하기 위해서
            //이 방식을 사용하면 튜플(tuple)을 이용해 두 개 이상의 값을 한 번에 반환할 수 있음.
            //튜플(tuple)은 간단히 말하자면 여러 값을 묶어서 하나의 값으로 반환하는 자료구조임.
            // 사용자 키와 IV를 저장할 변수를 선언
            byte[] user_key = null;
            byte[] user_iv = null;

            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // 데이터베이스 연결을 엶

                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 USER_KEY와 USER_IV를 선택
                    string query = "SELECT USER_KEY, USER_IV FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // 결과가 있으면
                            if (reader.Read())
                            {   // USER_KEY와 USER_IV 값을 읽어 변수에 저장
                                if (!reader.IsDBNull(reader.GetOrdinal("USER_KEY")))
                                {
                                    user_key = (byte[])reader["USER_KEY"];
                                }
                                if (!reader.IsDBNull(reader.GetOrdinal("USER_IV")))
                                {
                                    user_iv = (byte[])reader["USER_IV"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }

            // 사용자 키와 IV 값을 반환 (튜플 형태로)
            return (user_key, user_iv);
        }
        //General_user 테이블에서 user의 name값 가져오는 메서드
        //이렇게 하나의 값을 가져오는 경우에는 굳이 튜플의 형태로 받아올 필요가 없기 때문에 받아오는 데이터의 자료형인 byte[]로 선언하여 메서드를 정의
        //값을 가져오는 메서드를 사용했을때 에러가 뜰 수 있는데 아마 받아오려는 값이 null값일떄 나오는 오류일테니 당황하지 말기
        public byte[] Get_UserName(string emp_number)
        {
            byte[] name = null;

            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 name를 선택
                    string query = "SELECT NAME FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("NAME")))
                                {
                                    name = (byte[])reader["NAME"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // NAME 값을 반환
            return (name);

            //반환받은 name 값은 우리가 사용할때 아래와 같이 변수를 선언해주면 사용이 가능.
            //string emp_number = "1";
            //byte[] name = handler.Get_UserName(emp_number);
            //Convert.ToBase64String은 이진 데이터를 텍스트 형식으로 변환하는 데 사용
            //Console.WriteLine($"emp_number: {emp_number}, name: {Convert.ToBase64String(name)}");
        }

        //General_user 테이블에서 user의 position값 가져오는 메서드
        //name, position, img의 정보는 기본적으로 하나의 값만을 가져오기 때문에 가져오려는 데이터의 자료형을 사용하여
        //변수를 선언해서 값을 받고, 반환해줌.
        //사용하는 것도 동일하게 가져오는 값의 자료형에 맞게 변수를 선언후에 메서드를 사용하여 값을 받은 뒤 사용이 가능하다,
        //사용방법은 name 값을 가져오는 방법과 동일.
        public byte[] Get_UserPosition(string emp_number)
        {
            byte[] position = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 position를 선택
                    string query = "SELECT POSITION FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("POSITION")))
                                {
                                    position = (byte[])reader["POSITION"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // POSITION 값을 반환
            return (position);
        }


        //General_user 테이블에서 user의 tel값 가져오는 메서드
        public byte[] Get_UserTel(string emp_number)
        {
            byte[] tel = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 tel을 선택
                    string query = "SELECT TEL FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("TEL")))
                                {
                                    tel = (byte[])reader["TEL"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // POSITION 값을 반환
            return (tel);
        }


        //General_user 테이블에서 user의 email값 가져오는 메서드
        public byte[] Get_UserEmail(string emp_number)
        {
            byte[] email = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 email를 선택
                    string query = "SELECT EMAIL FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("EMAIL")))
                                {
                                    email = (byte[])reader["EMAIL"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // POSITION 값을 반환
            return (email);
        }

        //General_user 테이블에서 회사 주소값 가져오는 메서드
        public byte[] Get_Address(string emp_number)
        {
            byte[] address = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 address를 선택
                    string query = "SELECT ADDRESS FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("ADDRESS")))
                                {
                                    address = (byte[])reader["ADDRESS"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // POSITION 값을 반환
            return (address);
        }

        //General_user 테이블에서 해당 사원의 이미지를 가져오는 메서드
        //받아오는 방식은 name, position 등과 동일함
        public byte[] Get_FaceImg1(string emp_number)
        {
            byte[] face_img1 = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    string query = "SELECT FACE_IMG1 FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("FAEC_IMG1")))
                                {
                                    face_img1 = (byte[])reader["FACE_IMG1"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (face_img1);
        }

        //General_user 테이블에서 사원의 사진과 비교하는 이미지를 가져오는 메서드
        //받아오는 방식은 name, position 등과 동일함
        public byte[] Get_FaceImg2(string emp_number)
        {
            byte[] face_img2 = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    string query = "SELECT FACE_IMG2 FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("FAEC_IMG2")))
                                {
                                    face_img2 = (byte[])reader["FACE_IMG2"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (face_img2);
        }

        //DB에 저장된 사원증의 앞면 사진을 가져오는 메서드 
        public byte[] Get_EmpImg1(string emp_number)
        {
            byte[] employee_img1 = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    string query = "SELECT EMPLOYEE_IMG1 FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("EMPLOYEE_IMG1")))
                                {
                                    employee_img1 = (byte[])reader["EMPLOYEE_IMG1"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (employee_img1);
        }

        //DB에 저장된 사원증의 뒷면 사진을 가져오는 메서드
        public byte[] Get_EmpImg2(string emp_number)
        {
            byte[] employee_img2 = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    //string query = "SELECT EMPLOYEE_IMG2 FROM General_user WHERE EMP_NUMBER = @emp_number";
                    string query = "SELECT EMPLOYEE_IMG2 FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("EMPLOYEE_IMG2")))
                                {
                                    employee_img2 = (byte[])reader["EMPLOYEE_IMG2"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (employee_img2);
        }


        //General_user 테이블에서 해당 사원의 명함 앞면 사진을 가져오는 메서드
        //받아오는 방식은 name, position 등과 동일함
        public byte[] Get_BusinessImg1(string emp_number)
        {
            byte[] business_img1 = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    string query = "SELECT BUSINESS_IMG1 FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("BUSINESS_IMG1")))
                                {
                                    business_img1 = (byte[])reader["BUSINESS_IMG1"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (business_img1);
        }

        //General_user 테이블에서 해당 사원의 명함 뒷면 사진을 가져오는 메서드
        public byte[] Get_BusinessImg2(string emp_number)
        {
            byte[] business_img2 = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    string query = "SELECT BUSINESS_IMG2 FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("BUSINESS_IMG2")))
                                {
                                    business_img2 = (byte[])reader["BUSINESS_IMG2"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (business_img2);
        }

        //유저 로그 정보 받아오는 메서드
        public byte[] Get_UserLog(string emp_number)
        {
            byte[] user_log = null;
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL 쿼리를 정의하여 특정 EMP_NUMBER에 대한 가져올 컬럼를 선택
                    string query = "SELECT USER_LOG FROM General_user WHERE EMP_NUMBER = @emp_number";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // 쿼리를 실행하고 결과를 읽기 위해 MySqlDataReader 객체를 사용
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            //결과가 있다면
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("USER_LOG")))
                                {
                                    user_log = (byte[])reader["USER_LOG"];
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 오류가 발생한 경우 오류 메시지를 콘솔에 출력
                Console.WriteLine("데이터베이스 오류가 발생했습니다: " + ex.Message);
            }
            // IMG 값을 반환
            return (user_log);
        }

        //테이블이름 동적으로 받아와서 테이블 정보 삭제하기
        //단 일반 유저 => General_user 테이블의 변수명은 emp_number, 관리자 계정 manage_user 테이블의 변수명은 id이므로
        //삭제할 테이블의 접근하는 id의 변수명을 참고할 것
        public bool DeleteInfo(string tableName, string emp_number)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM " + tableName + " WHERE EMPID = @emp_number";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@emp_number", emp_number);
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

        //general_user 테이블에서 내가 입력하는 사번이 존재하는지 조회하는 메서드
        public bool CheckIdExists(string emp_number)
        {
            //사용하려면 받아올 변수를 bool로 선언한 다음
            //bool a = handler.CheckIdExists(emp_number); 이런 식으로 결과 값을 받아오면 됨.
            try
            {
                // MySQL 데이터베이스 연결을 생성하고 연결 문자열을 사용하여 연결을 엶.
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // 데이터베이스 연결을 엶.

                    // SQL 쿼리를 정의하여 주어진 사원 번호가 테이블에 존재하는지 확인.
                    string query = "SELECT EXISTS(SELECT 1 FROM General_user WHERE emp_number = @emp_number)";

                    // MySQLCommand 객체를 생성하고 쿼리 및 연결 객체를 설정.
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // 쿼리 매개변수에 emp_number 값을 설정.
                        command.Parameters.AddWithValue("@emp_number", emp_number);

                        // ExecuteScalar로부터 반환되는 값은 bool 형식으로 존재 여부를 나타냄.
                        // MySQL의 EXISTS 함수는 존재 여부를 1 또는 0으로 반환함.
                        // 이 값을 bool로 직접 변환.
                        bool exists = Convert.ToBoolean(command.ExecuteScalar());

                        // 결과를 반환.
                        return exists;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // 데이터베이스 연결이나 쿼리 실행 중에 오류가 발생한 경우 콘솔에 오류 메시지를 출력하고 false를 반환.
                Console.WriteLine($"Error: {ex}");
                return false;
            }
        }
    }
}