using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqRepaymentByIdDTO
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string? LenderId { get; set; }
        public string? BorrowerId { get; set; }
    }
}
