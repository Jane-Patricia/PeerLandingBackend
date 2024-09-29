using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/repayment/[action]")]
    [ApiController]
    public class RepaymentController : Controller
    {
        private readonly IRepaymentServices _repayServices;
        public RepaymentController(IRepaymentServices repayServices)
        {
            _repayServices = repayServices;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewRepayment(ReqDetailRepaymentDTO repay)
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

                var res = await _repayServices.CreateRepayment(repay);
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListRepayment(string idLender, string? status, string? borrowerId)
        {
            try
            {
                var res = await _repayServices.ListRepayment(idLender, status, borrowerId);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "List of users",
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RepaymentById(string id)
        {
            try
            {
                var res = await _repayServices.GetRepaymentById(id);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "List of users",
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
