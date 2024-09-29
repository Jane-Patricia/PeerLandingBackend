using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateRepaymentDTO
    {
        public decimal Amount { get; set; }
        public decimal repaidAmount { get; set; }
        public decimal balanceAmount { get; set; }
        public string repaidStatus { get; set; }
    }
}
