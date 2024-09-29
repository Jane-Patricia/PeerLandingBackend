using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/repaymentDetail/[action]")]
    [ApiController]
    public class RepaymentDetailController : Controller
    {
        private IRepaymentDetailServices _repaymentServices;

        public RepaymentDetailController(IRepaymentDetailServices repaymentServices)
        {
            _repaymentServices = repaymentServices;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDetailRepayment(string idRepayment)
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
                var res = await _repaymentServices.CreateDetailPayments(idRepayment);

                return Ok(new ResBaseDTO<string>
                {
                    Success = true,
                    Message = "Repayment detail created",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already exist")
                {
                    return BadRequest(new ResBaseDTO<object>
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
        [Authorize]
        public async Task<IActionResult> GetDetailRepaymentById(string repaymentId)
        {
            try
            {
                var response = await _repaymentServices.GetDetailRepaymentById(repaymentId);
                return Ok(new ResBaseDTO<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Payment"
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
