using System.ComponentModel.DataAnnotations;

namespace MVCTest1.Models
{
    public class GeneralUserViewModel
    {
        [Required(ErrorMessage = "사원번호를 입력해주세요.")]
        public string Emp_Number { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "이름을 입력해주세요.")]
        public string NameString { get; set; }

        //[Required(ErrorMessage = "직위를 입력해주세요.")]
        public string PositionString { get; set; }

        //[Required(ErrorMessage = "전화번호를 입력해주세요.")]
        public string TelString { get; set; }

        //[Required(ErrorMessage = "이메일을 입력해주세요.")]
        public string EmailString { get; set; }

        //[Required(ErrorMessage = "주소를 입력해주세요.")]
        public string AddressString { get; set; }

        public string WebcamImage { get; set; }
    }
}
