using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentDetailServices
    {
        Task<string> CreateDetailPayments(string idRepayment);
        Task<List<ResDetailRepaymentDTO>> GetDetailRepaymentById(string idRepayment);
        //Task<string> UpdateDetailPayment(List<ReqUpdateDetailPaymentDTO> detailPaymentDTO);
    }
}
