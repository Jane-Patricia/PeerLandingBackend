using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateMonthlyPaymentDTO
    {
        public string RepaymentId { get; set; }
        public string status { get; set; } 
    }
}
