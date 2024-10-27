using Bookify.Filters;
using Bookify.Models.Results;
using Bookify.Models.Requests;
using Bookify.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Bookify.Domain;
using Bookify.Identity.Services;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Controllers
{
    /// <summary>
    /// Представляет набор методов, отвечающих за работу авторизации и регистрации
    /// новых пользователей в системе бронирования
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(
        IUserRepository userRepository) : ControllerBase
    {
        /// <summary>
        /// Регистрация нового пользователя в системе
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST/ToDo
        ///     {
        ///         "firstName": "[имя клиента]",
        ///         "lastName": "[фамилия клиента]",
        ///         "email" : "[логин клиента (электронная почта)]",
        ///         "password" : "[пароль клиента]"
        ///         "confirmPassword" : "[повторный пароль клиента]"
        ///     }
        ///     
        /// </remarks>
        /// <param name="request">Набор параметров, необходимых для регистрации нового пользователя в системе</param>
        /// <returns>Результат регистрации нового пользователя</returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка API</response>
        [HttpPost("register", Name = "RegisterUser")]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResult), (int)HttpStatusCode.BadRequest)]
        [TypeFilter(typeof(ValidationFilterAttribute), Arguments = new object[] {-101})]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserRequest request)
        {
            var result = await userRepository.AddUser(request);

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.Error);
        }

        /// <summary>
        /// Авторизация пользователя в системе
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST/ToDo
        ///     {
        ///         "email" : "[логин клиента (электронная почта)]",
        ///         "password" : "[пароль клиента]"
        ///     }
        ///     
        /// </remarks>
        /// <param name="request">Набор параметров, необходимых для авторизации пользователя в системе</param>
        /// <returns>Результат авторизации пользователя - токен</returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка API</response>
        [HttpPost("login", Name = "LogIn")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResult), (int)HttpStatusCode.BadRequest)]
        [TypeFilter(typeof(ValidationFilterAttribute), Arguments = new object[] { -102 })]
        public async Task<ActionResult<string>> LogIn([FromBody] LoginRequest request)
        {
            var result = await userRepository.LogInUser(request);

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.Error);
        }
    }
}