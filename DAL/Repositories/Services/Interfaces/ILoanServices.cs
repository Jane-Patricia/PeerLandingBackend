using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDTO loan);
        Task<string> UpdateLoan(ReqUpdateLoan updateLoan, string Id);
        Task<List<ResListLoanDTO>> LoanList(string? status=null);
    }
}
