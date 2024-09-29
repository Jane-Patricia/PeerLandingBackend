using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userservices;
        public UserController(IUserServices userServices)
        {
            _userservices = userServices;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Register(ReqRegisterUserDTO register)
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
                var res = await _userservices.Register(register);

                return Ok(new ResBaseDTO<string>
                {
                    Success = true,
                    Message = "User registered",
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
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userservices.GetAllUsers();
                return Ok(new ResBaseDTO<List<ResUserDTO>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDTO<List<ResUserDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDTO LoginDTO)
        {
            try
            {
                var response = await _userservices.Login(LoginDTO);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "User login success",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid email or password")
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
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _userservices.Delete(id);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "Success Deleting User Account",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User did not exist")
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

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(ReqUpdateDTO updateDTO, string id)
        {
            try
            {
                var currUserRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                var response = await _userservices.Update(updateDTO, id, currUserRole);
                return Ok(new ResBaseDTO<string>
                {
                    Success = true,
                    Message = "Succes Updating User",
                    Data = response
                });

            }
            catch (Exception ex)
            {
                if(ex.Message == "User did not exist")
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
        [Authorize]
        public async Task<IActionResult> UserById(string id)
        {
            try
            {
                var userById = await _userservices.GetUser(id);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "Succes Selecting User By Id",
                    Data = userById
                });
            }
            
            catch (Exception ex)
            {
                if (ex.Message == "User did not exist")
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

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateSaldo(ReqUpdateBalanceDTO _updateBalance,string id)
        {
            try
            {
                var updateSal = await _userservices.UpdateBalance(_updateBalance, id);
                return Ok(new ResBaseDTO<object>
                {
                    Success = true,
                    Message = "Succes Updating Balance",
                    Data = updateSal
                });

            }
            catch (Exception ex)
            {
                if (ex.Message == "User did not exist")
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
    }
}
