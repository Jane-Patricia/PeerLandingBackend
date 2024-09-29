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
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDTO register);

        Task<List<ResUserDTO>> GetAllUsers();

        Task<ResLoginDTO> Login(ReqLoginDTO reqLogin);

        Task<string> Delete(string id);

        Task<string> Update(ReqUpdateDTO reqUpdate, string Id, string currUserRole);

        Task<ResGetUserByIdDTO> GetUser(string id);

        Task<ResUpdateBalanceDTO> UpdateBalance(ReqUpdateBalanceDTO reqUpdateBalance, string id);
    }
}
