using System.ComponentModel.DataAnnotations;

namespace MVCTest1.Models
{
    public class UserInfo
    {
        [Required(ErrorMessage = "사원번호를 입력해주세요.")]
        public string Emp_Number { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
        public string Password { get; set; }
    }
}
