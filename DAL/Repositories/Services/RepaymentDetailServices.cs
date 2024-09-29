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
    public class RepaymentDetailServices : IRepaymentDetailServices
    {
        private readonly PeerlandingContext _context;
        private readonly IRepaymentServices _repaymentServices;
        private readonly ILoanServices _loanServices;
        private readonly IUserServices _userServices;
        public RepaymentDetailServices(PeerlandingContext context, IRepaymentServices repaymentServices, ILoanServices loanServices, IUserServices userServices)
        {
            _context = context;
            _repaymentServices = repaymentServices;
            _loanServices = loanServices;
            _userServices = userServices;
        }

        public async Task<string> CreateDetailPayments(string idRepayment)
        {
            var repayment = await _repaymentServices.GetRepaymentById(idRepayment) ?? throw new Exception("Repayment not found");

            for(int i = 1; i <= 12; i++)
            {
                var newRepayment = new TrnRepaymentDetail
                {
                    RepaymentId = idRepayment,
                    Amount = calculateMonthlyInterest(repayment.Amount, repayment.InterestRate),
                    Status = "false"
                };

                await _context.AddAsync(newRepayment);
                await _context.SaveChangesAsync();
            }

            return repayment.Id;
        }

        public Task<List<ResDetailRepaymentDTO>> GetDetailRepaymentById(string idRepayment)
        {
            var repayment = _context.TrnRepaymentDetails
                .Select(p => new ResDetailRepaymentDTO
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    RepaymentId = p.RepaymentId,
                    status = p.Status
                })
                .Where(p => p.RepaymentId == idRepayment)
                .OrderBy(p => p.Id)
                .ToListAsync();

            return repayment;
        }

        //public async Task<string> UpdateDetailPayment(List<ReqUpdateDetailPaymentDTO> detailPaymentDTO)
        //{
        //    foreach(var detailPayment in detailPaymentDTO)
        //    {
        //        var payment = await _context.TrnRepaymentDetails
        //            .FirstOrDefaultAsync(p => p.Id == detailPayment.PaymentId) ?? throw new Exception("Detail repayment not found");
        //        payment.Status = "true";

        //        _context.TrnRepaymentDetails.Update(payment);

        //        //var lenderId = await _userServices.GetUser()

        //        var getRepayment = await _repaymentServices.GetRepaymentById(payment.RepaymentId);
        //         //get loan by id

        //        var updateRepay = await _context.TrnRepayments
        //            .SingleOrDefaultAsync(u => u.Id == payment.RepaymentId) ?? throw new Exception("Repayment not found");

        //        updateRepay.RepaidAmount += payment.Amount;
        //        updateRepay.BalanceAmount = 


        //    }
        //}

        public decimal calculateMonthlyInterest(decimal amount, decimal interest)
        {
            decimal monthlyRates = interest / 100 / 12;
            decimal denominator = 1 - (decimal)Math.Pow((double)(1 + monthlyRates), -12);

            if (denominator == 0)
            {
                throw new DivideByZeroException("The denominator in monthly payment cannot be zero.");
            }

            return amount * (monthlyRates / denominator);
        }
    }
}
