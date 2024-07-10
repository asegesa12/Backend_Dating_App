using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // /api/users
    [Authorize]
    public class UsersController(IUserRepository userRepository) : BaseControllerApi
    {
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() 
        {
            var users = await userRepository.GetMemberAsync();
           
            return(Ok(users));
        }
        
        [HttpGet("{username}")] // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberByNameAsync(username);
         
            if (user == null) return NotFound();

            return user;
        }
    }
}
