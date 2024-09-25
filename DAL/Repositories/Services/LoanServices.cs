using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerlandingContext;
        public LoanServices(PeerlandingContext peerLandingContext)
        {
            _peerlandingContext = peerLandingContext;
        }
        public async Task<string> CreateLoan(ReqLoanDTO loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerlandingContext.AddAsync(newLoan);
            await _peerlandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public async Task<List<ResListLoanDTO>> LoanList(string? status=null)
        {
            return await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .OrderByDescending(Reqloans => Reqloans.CreatedAt)
                .Where(Reqloans => status == null || Reqloans.Status == status)
                .Select(Reqloans => new ResListLoanDTO
                {
                    LoanId = Reqloans.Id,
                    BorrowerName = Reqloans.User.Name,
                    Amount = Reqloans.Amount,
                    InterestRate = Reqloans.InterestRate,
                    Duration = Reqloans.Duration,
                    Status = Reqloans.Status,
                    CreatedAt = Reqloans.CreatedAt,
                    UpdatedAt = Reqloans.UpdatedAt,
                }).ToListAsync();
        }

        public async Task<string> UpdateLoan(ReqUpdateLoan updateLoan, string Id)
        {
            var findLoan = await _peerlandingContext.MstLoans.SingleOrDefaultAsync(e => e.Id == Id);
            if (findLoan == null)
            {
                throw new Exception("Loan did not exist");
            }

            findLoan.Status = updateLoan.Status;
            findLoan.UpdatedAt = DateTime.UtcNow;

            await _peerlandingContext.SaveChangesAsync();

            return updateLoan.Status;
        }
    }
}
