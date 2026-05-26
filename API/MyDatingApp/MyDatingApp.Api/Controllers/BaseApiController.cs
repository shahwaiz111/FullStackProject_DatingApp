using Microsoft.AspNetCore.Mvc;
using MyDatingApp.Api.Helpers;

namespace MyDatingApp.Api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
