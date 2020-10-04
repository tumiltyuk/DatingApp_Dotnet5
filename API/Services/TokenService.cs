using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            // Adding claims
            var claims = new List<Claim>
            {
                // assign username as one of the claims
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // create credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // describe what token will look like
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            // create token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // return the token
            return tokenHandler.WriteToken(token);
        }
    }
}