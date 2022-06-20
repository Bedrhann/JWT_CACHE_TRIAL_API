using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hafta._4.Persistence.Security
{
    public  class TokenCreate
    {
        IConfiguration _configuration;
        public TokenCreate(IConfiguration configuration)
        {
        }

        public  JwtSecurityToken Create(List<Claim> claims)
        {
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(

                 signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256),
                 issuer: _configuration["JWT:Issuer"],
                 audience: _configuration["JWT:Audience"],
                 expires: DateTime.Now.AddDays(2),
                 claims: claims
                );

            return token;

        }
    }
}
