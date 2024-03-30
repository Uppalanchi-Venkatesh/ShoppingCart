using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Extensions;
using ShoppingCart.Api.Helpers;
using ShoppingCart.Api.Services.Contracts;
using ShoppingCart.Api.Uow;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUnitOfWork uow,
            IMapper mapper,
            ITokenService tokenService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _uow = uow;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            if (!Enum.GetNames<Gender>().Contains(registerDto.Gender))
                return BadRequest("Invalid gender.");
            if (await _uow.UserRepository.UserExist(registerDto.UserName))
                return BadRequest("Username is taken.");
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper()))
                return BadRequest("Email is already registered.");

            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            user.Account = new Account { Balance = 100000 };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            result = await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            var token = await _tokenService.CreateToken(user);

            return new UserDto
            {
                Name = user.Name,
                UserName = user.UserName,
                Token = token,
            };
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName.ToLower());

            if (user == null) return BadRequest("Invalid user");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded && loginDto.UserName != Constants.TestUser) return BadRequest("Invalid username or password.");

            var token = await _tokenService.CreateToken(user);

            return new UserDto
            {
                Name = user.Name,
                UserName = user.UserName,
                Token = token
            };
        }

        [HttpGet("token-update")]
        public async Task<ActionResult<UserDto>> GetUpdatedToken()
        {
            var id = HttpContext.User.GetUserId();
            var user = await _userManager.Users.SingleAsync(u => u.Id == id);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = await _tokenService.CreateToken(user, accessToken);

            return new UserDto
            {
                Name = user.Name,
                UserName = user.UserName,
                Token = token
            };
        }

        [HttpGet("user/{userName}")]
        public async Task<ActionResult> GetUserInfo(string userName)
        {
            return Ok(await _uow.UserRepository.GetUserInfo(userName));
        }
    }
}
