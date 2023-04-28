using DatingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Services.Interfaces
{
    public interface ITokenService
    {

        Task<string> CreateToken(AppUser user);

    }
}
