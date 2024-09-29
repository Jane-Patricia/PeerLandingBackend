using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlandingContext _context;
        public RepaymentServices(PeerlandingContext context)
        {
            _context = context;
        }

        public async Task<string> CreateRepayment(ReqDetailRepaymentDTO reqMonthlyRepay)
        {
            var newRepay = new TrnRepayment
            {
                LoanId = reqMonthlyRepay.LoanId,    
                Amount = reqMonthlyRepay.Amount,
                RepaidAmount = reqMonthlyRepay.RepaidAmount,
                BalanceAmount = reqMonthlyRepay.BalanceAmount,
                RepaidStatus = reqMonthlyRepay.RepaidStatus
            };

            await _context.AddAsync(newRepay);
            await _context.SaveChangesAsync();

            Console.WriteLine($"New Repayment ID: {newRepay.Id}");

            return newRepay.Id;
        }

        public async Task<ResGetRepaymentByIdDTO> GetRepaymentById(string id)
        {
            var repaymentById = await _context.TrnRepayments
            .Join(_context.MstLoans,
                  repayment => repayment.LoanId,
                  loan => loan.Id,
                  (repayment, loan) => new { repayment, loan })
            .Join(_context.TrnFundings,
                  combined => combined.loan.Id,
                  funding => funding.LoanId,
                  (combined, funding) => new { combined.repayment, combined.loan, funding })
            .Join(_context.MstUsers,
                  combined => combined.loan.BorrowerId,
                  user => user.Id,
                  (combined, user) => new { combined.repayment, combined.loan, combined.funding, user })
            .Where(r => r.repayment.Id == id)
            .Select(r => new ResGetRepaymentByIdDTO
            {
                Id = r.repayment.Id,
                LoanId = r.loan.Id,
                LenderId = r.funding.LenderId,
                BorrowerId = r.loan.BorrowerId,
                BorrowerName = r.user.Name,
                InterestRate = r.loan.InterestRate,
                Duration = r.loan.Duration,
                Amount = r.repayment.Amount,
                RepaidAmount = r.repayment.RepaidAmount,
                BalanceAmount = r.repayment.BalanceAmount,
                RepaidStatus = r.repayment.RepaidStatus,
                PaidAt = r.repayment.PaidAt
            })
            .FirstOrDefaultAsync();

            if(repaymentById == null)
            {
                throw new Exception("Repayment not found");
            }

            return repaymentById;
        }

        //public async Task<List<ResListRepaymentDTO>> ListRepayment(string status, string? idLender, string? borrowerId)
        //{
        //    var repayments = await _context.TrnRepayments
        //    .Where(r => (status == null || r.RepaidStatus == status) &&
        //                (borrowerId == null || r.Loans.BorrowerId == borrowerId) &&
        //                (idLender == null || _context.TrnFundings.Any(f => f.LoanId == r.LoanId && f.LenderId == idLender)))
        //    .Select(r => new ResListRepaymentDTO
        //    {
        //        Id = r.Id,
        //        LoanId = r.LoanId,
        //        LenderId = _context.TrnFundings
        //                    .Where(f => f.LoanId == r.LoanId && (idLender == null || f.LenderId == idLender))
        //                    .Select(f => f.LenderId)
        //                    .FirstOrDefault(),
        //        BorrowerId = r.Loans.BorrowerId,
        //        BorrowerName = r.Loans.User.Name,
        //        Amount = r.Amount,
        //        RepaidAmount = r.RepaidAmount,
        //        BalanceAmount = r.BalanceAmount,
        //        RepaidStatus = r.RepaidStatus,
        //        PaidAt = r.PaidAt
        //    }).ToListAsync();

        //    return repayments;
        //}

        public async Task<List<ResListRepaymentDTO>> ListRepayment(string idLender, string? status, string? borrowerId)
        {
            var repayments = await _context.TrnRepayments
            .Where(r => (status == null || r.RepaidStatus == status) &&
                        (borrowerId == null || r.Loans.BorrowerId == borrowerId) &&
                        (idLender == null || _context.TrnFundings.Any(f => f.LoanId == r.LoanId && f.LenderId == idLender)))
            .Select(r => new ResListRepaymentDTO
            {
                Id = r.Id,
                LoanId = r.LoanId,
                LenderId = _context.TrnFundings
                            .Where(f => f.LoanId == r.LoanId && (idLender == null || f.LenderId == idLender))
                            .Select(f => f.LenderId)
                            .FirstOrDefault(),
                BorrowerId = r.Loans.BorrowerId,
                BorrowerName = r.Loans.User.Name,
                Amount = r.Amount,
                RepaidAmount = r.RepaidAmount,
                BalanceAmount = r.BalanceAmount,
                RepaidStatus = r.RepaidStatus,
                PaidAt = r.PaidAt
            }).ToListAsync();

            return repayments;
        }
    }


    //public async Task<List<ResListRepaymentDTO>> ListRepayment(string idLender, string? status, string? borrowerId)
    //    {
    //        var repayments = await _context.TrnRepayments
    //    .Join(
    //        _context.MstLoans,
    //        repayment => repayment.LoanId,
    //        loan => loan.Id,
    //        (repayment, loan) => new { repayment, loan }
    //    )
    //    .Join(
    //        _context.TrnFundings,
    //        combined => combined.loan.Id,
    //        funding => funding.LoanId,
    //        (combined, funding) => new { combined.repayment, combined.loan, funding }
    //    )
    //    .Join(
    //        _context.MstUsers,
    //        combined => combined.loan.BorrowerId,
    //        user => user.Id,
    //        (combined, user) => new { combined.repayment, combined.loan, combined.funding, user }
    //    )
    //    .Where(r => (status == null || r.repayment.RepaidStatus == status) &&
    //                (idLender == null || r.funding.LenderId == idLender) &&
    //                (borrowerId == null || r.loan.BorrowerId == borrowerId))
    //    .Select(r => new ResListRepaymentDTO
    //    {
    //        Id = r.repayment.Id,
    //        LoanId = r.loan.Id,
    //        LenderId = r.funding.LenderId,
    //        BorrowerId = r.loan.BorrowerId,
    //        BorrowerName = r.user.Name,
    //        Amount = r.repayment.Amount,
    //        RepaidAmount = r.repayment.RepaidAmount,
    //        BalanceAmount = r.repayment.BalanceAmount,
    //        RepaidStatus = r.repayment.RepaidStatus,
    //        PaidAt = r.repayment.PaidAt
    //    })
    //    .ToListAsync();
    //        return repayments;
    //    }
    //}

    //public async Task<string> CreateRepayment(ReqLoanDTO loan)
    //{
    //    var newLoan = new MstLoans
    //    {
    //        BorrowerId = loan.BorrowerId,
    //        Amount = loan.Amount,
    //        InterestRate = loan.InterestRate,
    //    };

    //    await _peerlandingContext.AddAsync(newLoan);
    //    await _peerlandingContext.SaveChangesAsync();

    //    return newLoan.BorrowerId;
    //}
}
