using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanServices;
        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDTO loan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation error occurred!");

                    return BadRequest(new ResBaseDTO<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var res = await _loanServices.CreateLoan(loan);
                return Ok(new ResBaseDTO<string>
                {
                    Success = true,
                    Message = "Success add loan data",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDTO<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLoan(ReqUpdateLoan updateLoan, string id)
        {
            try
            {
                var response = await _loanServices.UpdateLoan(updateLoan, id);
                return Ok(new ResBaseDTO<string>
                {
                    Success = true,
                    Message = "Succes Updating Loan",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Loan did not exist")
                {
                    return BadRequest(new ResBaseDTO<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDTO<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }
        }

        [HttpGet]
        public async Task<IActionResult> LoanList(string? status=null)
        {
            try
            {
                var res = await _loanServices.LoanList(status);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "Succes Getting Loan List",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDTO<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
