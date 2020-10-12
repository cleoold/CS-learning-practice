using Microsoft.AspNetCore.Mvc;

namespace TodoListApi.Controllers
{
    [Route("api/Ping")]
    [ApiController]
    public class PingController
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            return new { Alive = true };
        }
    }
}
