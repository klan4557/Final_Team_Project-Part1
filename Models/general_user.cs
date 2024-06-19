using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCTest1.Models
{
    public class general_user
    {
        [Key]
        public string emp_number { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
        public string password { get; set; }

        [Required(ErrorMessage = "이름을 입력해주세요.")]
        public byte[] name { get; set; }

        [Required(ErrorMessage = "직위를 입력해주세요.")]
        public byte[] position { get; set; }

        [Required(ErrorMessage = "전화번호를 입력해주세요.")]
        public byte[] tel { get; set; }

        [Required(ErrorMessage = "이메일을 입력해주세요.")]
        public byte[] email { get; set; }

        [Required(ErrorMessage = "주소를 입력해주세요.")]
        public byte[] address { get; set; }

        [NotMapped]
        public string emp_numberString { get; set; }

        [NotMapped]
        public string nameString { get; set; }

        [NotMapped]
        public string positionString { get; set; }

        [NotMapped]
        public string telString { get; set; }

        [NotMapped]
        public string emailString { get; set; }

        [NotMapped]
        public string addressString { get; set; }
    }
}

