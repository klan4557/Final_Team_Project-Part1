using System.ComponentModel.DataAnnotations;

namespace MVCTest1.Models
{
    public class Businesscardinfo
    {
        public int EmpNumber { get; set; }
        public byte[] Businessimg1 { get; set; }
        public byte[] Businessimg2 { get; set; }
    }
}