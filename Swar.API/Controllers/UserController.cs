using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Services;
using Swar.API.Models;
using Swar.API.Models.DTOs;

namespace Swar.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDTO">User login details.</param>
        /// <returns>Returns the login result.</returns>
        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LoginResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
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
            catch (ExternalServiceLoginException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status400BadRequest, Message = ex.Message });
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
        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDTO)
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
        [HttpPost("refresh-token")]
        [Authorize(AuthenticationSchemes = "RefreshTokenScheme")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccessTokenDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int userId)
                    throw new EntityNotFoundException("User does not exist");

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
        /// Gets all the users.
        /// </summary>
        /// <returns>Returns the list of users.</returns>
        [HttpGet("GetAllUsers")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<RegisteredUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                return Ok(users);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Gets the current user's details.
        /// </summary>
        /// <returns>Returns the user details.</returns>
        [HttpGet("me")]
        [Authorize(Roles = "Admin, User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int id)
                    throw new EntityNotFoundException("User does not exist");

                var user = await _userService.GetUserById(id);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Updates the current user's details.
        /// </summary>
        /// <param name="userUpdateDTO">User update details.</param>
        /// <returns>Returns the updated user details.</returns>
        [HttpPatch("me")]
        [Authorize(Roles = "Admin, User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserUpdateDTO userUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int id)
                    throw new EntityNotFoundException("User does not exist");
                var user = await _userService.UpdateUser(id, userUpdateDTO);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
            }
            catch (EntityAlreadyExistsException ex)
            {
                return Conflict(new ErrorModel { Status = StatusCodes.Status409Conflict, Message = ex.Message });
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
        /// Updates the current user's password.
        /// </summary>
        /// <param name="userPasswordUpdateDTO">Password update details.</param>
        /// <returns>Returns the updated user details.</returns>
        [HttpPatch("me/password")]
        [Authorize(Roles = "Admin, User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCurrentUserPassword([FromBody] UserPasswordUpdateDTO userPasswordUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int id)
                    throw new EntityNotFoundException("User does not exist");
                var user = await _userService.UpdateUserPassword(id, userPasswordUpdateDTO);
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
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(new ErrorModel { Status = StatusCodes.Status401Unauthorized, Message = ex.Message });
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
        /// Deletes the current user's account.
        /// </summary>
        /// <returns>Returns the deletion confirmation.</returns>
        [HttpDelete("me")]
        [Authorize(Roles = "Admin, User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int id)
                    throw new EntityNotFoundException("User does not exist");
                var user = await _userService.DeleteUser(id);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new ErrorModel { Status = StatusCodes.Status404NotFound, Message = ex.Message });
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
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int adminId)
                    throw new EntityNotFoundException("User does not exist");
                var user = await _userService.DeactivateUser(id, adminId);
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
        /// Activates a user account.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>Returns the deactivated user details.</returns>
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisteredUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not int adminId)
                    throw new EntityNotFoundException("User does not exist");
                var user = await _userService.ActivateUser(id, adminId);
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
        /// Verify the user's access token.
        /// </summary>
        [HttpGet("verify-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyToken()
        {
            return Ok();
        }

    }
}
