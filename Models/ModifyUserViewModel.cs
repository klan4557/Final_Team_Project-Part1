using System.ComponentModel.DataAnnotations;

namespace MVCTest1.Models
{
    public class ModifyUserViewModel
    {
        [Required(ErrorMessage = "사원번호를 입력해주세요.")]
        public string Emp_Number { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
        public string Password { get; set; }

        public string NameString { get; set; }
        public string PositionString { get; set; }
        public string TelString { get; set; }
        public string EmailString { get; set; }
        public string AddressString { get; set; }
    }
}
