using System.ComponentModel.DataAnnotations;

namespace MVCTest1.Models
{
    public class GuestUserViewModel
    {
        [Required(ErrorMessage = "사원번호를 입력해주세요.")]
        public string Emp_Number { get; set; }

        [Required(ErrorMessage = "이름을 입력해주세요.")]
        public string NameString { get; set; }

        public string WebcamImage { get; set; }
    }
}
