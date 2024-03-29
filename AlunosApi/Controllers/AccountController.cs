﻿using AlunosApi.Services;
using AlunosApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlunosApi.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")] 
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticate _authentication;

        public AccountController(IConfiguration configuration, IAuthenticate authentication)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));

            _authentication = authentication ??
                throw new ArgumentNullException(nameof(authentication));
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST api/Account/CreateUser
        ///     {
        ///         "email": "usuario@user.com",
        ///         "password": "User12#",
        ///         "confirmPassword": "User12#"
        ///     }
        /// </remarks>
        /// <param name="model">Objeto contendo email, senha e confirmação de senha.</param>
        /// <returns>ActionResult contendo Token de autenticação e validade do token.</returns>
        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmePassword", "As senhas não conferem");
                return BadRequest(ModelState);
            }

            var result = await _authentication.RegisterUser(model.Email, model.Password);

            if (result)
            {
                var userInfo = new LoginModel { Email = model.Email, Password = model.Password };
                return GenerateToken(userInfo);
            }
            else
            {
                ModelState.AddModelError("CreateUser", "Regsitro inválido.");
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Realiza o login do usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST api/Account/LoginUser
        ///     {
        ///         "email": "usuario@user.com",
        ///         "password": "User12#"
        ///     }
        /// </remarks>
        /// <param name="userInfo">Objeto contendo email e senha.</param>
        /// <returns>ActionResult contendo Token de autenticação e validade do token.</returns>
        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel userInfo)
        {
            var result = await _authentication.Authenticate(userInfo.Email, userInfo.Password);

            if (result)
            {
                return GenerateToken(userInfo);
            }
            else
            {
                ModelState.AddModelError("LoginUser", "Login inválido.");
                return BadRequest(ModelState);
            }
        }

        private ActionResult<UserToken> GenerateToken(LoginModel userInfo)
        {
            var claims = new[]
            {
                new Claim("email", userInfo.Email),
                new Claim("meu token", "token do pedro"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            var creeds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(10);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creeds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
            };
        }
    }
}
