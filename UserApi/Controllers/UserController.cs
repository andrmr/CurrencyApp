using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) =>
            _userService = userService;

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            var result = await _userService.Authenticate(request);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
        {
            var result = await _userService.Register(request);
            return Ok(result);
        }

        [HttpGet("get")]
        [Authorize]
        public async Task<ActionResult<GetDataResponse>> GetData(GetDataRequest request)
        {
            var result = await _userService.GetData(request);
            return Ok(result);
        }

        [HttpPut("set")]
        [Authorize]
        public async Task<ActionResult<SetDataResponse>> SetData(SetDataRequest request)
        {
            var result = await _userService.SetData(request);
            return Ok(result);
        }
    }
}
