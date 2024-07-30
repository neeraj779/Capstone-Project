using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Helpers;
using Swar.API.Interfaces.Services;
using Swar.API.Models;
using Swar.API.Models.DTOs;

namespace Swar.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDTO">User login details.</param>
        /// <returns>Returns the login result.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var loginReturn = await _userService.Login(loginDTO);
                return Ok(loginReturn);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (InactiveAccountException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">User registration details.</param>
        /// <returns>Returns the registered user details.</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var registeredUser = await _userService.Register(userDTO);
                return Ok(registeredUser);
            }
            catch (EntityAlreadyExistsException ex)
            {
                return Conflict(new ErrorModel { Status = StatusCodes.Status409Conflict, Message = ex.Message });
            }
            catch (WeakPasswordException ex)
            {
                return BadRequest(new ErrorModel { Status = StatusCodes.Status400BadRequest, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Refreshes the user's access token.
        /// </summary>
        /// <returns>Returns the new access token.</returns>
        [Authorize(AuthenticationSchemes = "RefreshTokenScheme")]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                int userId = UserHelper.GetUserId(User);

                var accessToken = await _userService.RefreshToken(userId);
                return Ok(accessToken);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InactiveAccountException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Gets all registered users.
        /// </summary>
        /// <returns>Returns a list of all registered users.</returns>
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<RegisteredUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                return Ok(users);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status200OK, new ErrorModel { Status = StatusCodes.Status200OK, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Activates a user account.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>Returns the activated user details.</returns>
        [HttpPost("ActivateUser")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var user = await _userService.ActivateUser(id);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InactiveAccountException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>Returns the deactivated user details.</returns>
        [HttpPost("DeactivateUser")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var user = await _userService.DeactivateUser(id);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (InactiveAccountException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
