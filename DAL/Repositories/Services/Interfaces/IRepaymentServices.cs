using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentServices
    {
        Task<string> CreateRepayment(ReqDetailRepaymentDTO reqMonthlyRepay);
        Task<List<ResListRepaymentDTO>> ListRepayment(string idLender, string? status, string? borrowerId);
        Task<ResGetRepaymentByIdDTO> GetRepaymentById(string id);
    }
}
