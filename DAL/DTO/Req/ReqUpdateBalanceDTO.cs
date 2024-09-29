using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateBalanceDTO
    {
        [Required(ErrorMessage = "balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "balance must be a poitive value")]
        public decimal Balance { get; set; }
    }
}
