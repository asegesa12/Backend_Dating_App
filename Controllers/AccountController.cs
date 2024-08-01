using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    
    public class AccountController(DataContext context, ITokenService tokenService) : BaseControllerApi
    {
        [HttpPost("register")] // account/register

        public async Task<ActionResult<UserDTO>> Regisster(RegisterDto registerDto)
        {
            //Summary: If username already exist on the database, it wont allow the user
            // to use it. However if the username doesnt exist, it will be able to register the user.
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            return Ok();
            //Summary: HMAC Help us out to generate random password in our App
            //using var hmac = new HMACSHA512();

            //var user = new AppUser
            //{
            //    UserName = registerDto.Username,
            //    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //    PasswordSalt = hmac.Key
            //};

            //context.Users.Add(user);
            //await context.SaveChangesAsync();

            //return new UserDTO
            //{
            //    Username = user.UserName,
            //    Token = tokenService.CreateToken(user)
            //};
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
        {
            //Summary: If the user is not found in our Db it will return an Unathorized
            var user = await context.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x =>
            x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            //Summary: On this scope we are going to compare if the password is the same
            // that user used in our app. 

            //However since our password are Hash and salt. We must HMAC to compare the 
            //Password are identical. So we used a loop to compare the password that is store
            //in our app and the one that was used to log in.

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDTO
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };

        }


        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}
