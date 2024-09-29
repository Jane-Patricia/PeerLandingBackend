using DAL.DTO.Req;
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
    public class FundingServices : IFundingServices
    {
        private readonly PeerlandingContext _context;
        private readonly IUserServices _userServices;
        private readonly ILoanServices _loanServices;
        private readonly IRepaymentServices _repaymentServices;
        private readonly IRepaymentDetailServices _repaymentDetailServices;

        public FundingServices(PeerlandingContext peerLandingContext, IUserServices userServices, ILoanServices loanServices, IRepaymentServices repaymentServices, IRepaymentDetailServices repaymentDetailServices)
        {
            _context = peerLandingContext;
            _userServices = userServices;
            _loanServices = loanServices;
            _repaymentServices = repaymentServices;
            _repaymentDetailServices = repaymentDetailServices;
        }

        public async Task<string> CreateFunding(ReqFundingDto funding)
        {
            var findLender = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == funding.LenderId) ?? throw new Exception("Lender not found"); ;
            
            var findLoan = await _context.MstLoans.Include(e => e.User).SingleOrDefaultAsync(e => e.Id == funding.LoanId) ?? throw new Exception("Loan not found");
           
            var findBorrower = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == findLoan.User.Id) ?? throw new Exception("Borrower not found");
            await _loanServices.UpdateLoan(new ReqUpdateLoan
            {
                Status = "funded"
            }, findLoan.Id);

            await _userServices.UpdateBalance(new ReqUpdateBalanceDTO
            {
                Balance = findLender.Balance - findLoan.Amount ?? 0
            }, findLender.Id);

            await _userServices.UpdateBalance(new ReqUpdateBalanceDTO
            {
                Balance = findBorrower.Balance + findLoan.Amount ?? 0
            }, findBorrower.Id);

            var newFunding = new TrnFunding
            {
                LoanId = funding.LoanId,
                LenderId = funding.LenderId,
                Amount = findLoan.Amount
            };

            await _context.TrnFundings.AddAsync(newFunding);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Repayment - LoanId: {funding.LoanId}, Amount: {findLoan.Amount}");

            var repayment = await _repaymentServices.CreateRepayment(new ReqDetailRepaymentDTO
            {
                LoanId = funding.LoanId,
                Amount = findLoan.Amount,
                RepaidAmount = 0,
                BalanceAmount = findLoan.Amount,
                RepaidStatus = "on repay"
            });

            Console.WriteLine($"repayment: {repayment}");

            await _repaymentDetailServices.CreateDetailPayments(repayment);
     
            return findLender.Id;

        }
    }
}
