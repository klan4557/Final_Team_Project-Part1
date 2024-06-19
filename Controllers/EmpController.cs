using Microsoft.AspNetCore.Mvc;
using MVCTest1.Models;
using System.Threading.Tasks;
using MVCTest1.Data;

public class EmpController : Controller
{
    [Route("/Home/Businesscard/{emp_number}")]
    public async Task<IActionResult> Businesscard(int emp_number)
    {
        Businesscardinfo businesscardinfo = new Businesscardinfo();
        businesscardinfo.EmpNumber = emp_number;
        businesscardinfo.Businessimg1 = Decryption_dll.Decrypt.Decrypt_Byte_Data(emp_number, "businessimg1");
        businesscardinfo.Businessimg2 = Decryption_dll.Decrypt.Decrypt_Byte_Data(emp_number, "businessimg2");

        // 뷰의 이름을 명시적으로 지정하여 Index.cshtml을 반환합니다.
        return View("~/Views/Home/businesscard.cshtml", businesscardinfo);

    }

}
