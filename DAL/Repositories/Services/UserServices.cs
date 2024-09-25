using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _context;
        private readonly IConfiguration _configuration;
        public UserServices(PeerlandingContext context, IConfiguration configuration) 
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<ResUserDTO>> GetAllUsers()
        {
            return await _context.MstUsers
                .Where(user => user.Role != "Admin")
                .Select(user => new ResUserDTO
                {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role,
                        Balance = user.Balance,
                }).ToListAsync();
        }

        public async Task<ResLoginDTO> Login(ReqLoginDTO reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if(!isPasswordValid)
            {
                throw new Exception("Invalid Email or Password");
            }

            var token = GenerateJwtToken(user);

            var loginResponse = new ResLoginDTO
            {
                Token = token,
            };

            return loginResponse;
        }

        private string GenerateJwtToken(MstUser user) 
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: Claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> Register(ReqRegisterUserDTO register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);
            if (isAnyEmail != null) 
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance,
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }

        public async Task<string> DeleteUser(string Id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == Id);
            if(user == null)
            {
                throw new Exception("User did not exist");
            }

            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();

            return user.Name;
        }

        public async Task<string> Delete(string Id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == Id);
            if (user == null)
            {
                throw new Exception("User did not exist");
            }

            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();

            return user.Name;
        }

        public async Task<string> Update(ReqUpdateDTO reqUpdate, string Id, string currUserRole)
        {
            var findUser = await _context.MstUsers.SingleOrDefaultAsync(e => e.Id == Id);  
            if(findUser == null)
            {
                throw new Exception("User did not exist");
            }

            if(currUserRole == "admin")
            {
                findUser.Name = reqUpdate.Name;
                findUser.Password = BCrypt.Net.BCrypt.HashPassword(reqUpdate.Password);
                findUser.Role = reqUpdate.Role;
                findUser.Balance = reqUpdate.Balance;
            }
            else
            {
                findUser.Name = reqUpdate.Name;
                findUser.Email = reqUpdate.Email;
                findUser.Password = BCrypt.Net.BCrypt.HashPassword(reqUpdate.Password);
                findUser.Balance = reqUpdate.Balance;
            }
            
            await _context.SaveChangesAsync();

            return reqUpdate.Name;
        }
    }
}
