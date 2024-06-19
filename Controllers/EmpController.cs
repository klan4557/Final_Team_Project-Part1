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

        // ���� �̸��� ��������� �����Ͽ� Index.cshtml�� ��ȯ�մϴ�.
        return View("~/Views/Home/businesscard.cshtml", businesscardinfo);

    }

}
