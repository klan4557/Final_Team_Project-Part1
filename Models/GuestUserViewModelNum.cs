using System.ComponentModel.DataAnnotations;

namespace MVCTest1.Models
{
    public class GuestUserViewModelNum
    {
        [Required(ErrorMessage = "사원번호를 입력해주세요.")]
        public string Guest_Number { get; set; }

    }
}
