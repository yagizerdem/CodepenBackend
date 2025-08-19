using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Entity;
using Models.ResponseTypes;
using Service;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private  readonly ApplicationUserService _applicationUserService;
        public AuthController(
            ApplicationDbContext db,
            UserManager<ApplicationUserEntity> userManager,
            ApplicationUserService applicationUserService)
        {
            _db = db;
            _userManager = userManager;
            _applicationUserService = applicationUserService;   
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _applicationUserService.CreateUser(dto);

            return Ok(ApiResponse<ApplicationUserEntity>.SuccessResponse(
                data: result,
                message: "user created successfully",
                statusCode: System.Net.HttpStatusCode.Created));
        } 


    }
}
