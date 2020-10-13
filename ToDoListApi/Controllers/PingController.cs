using Microsoft.AspNetCore.Mvc;

namespace TodoListApi.Controllers
{
    [Route("api/Ping")]
    [ApiController]
    public class PingController
    {
        [HttpGet]
        public ActionResult<object> GetPing()
        {
            return new { Alive = true };
        }
    }
}
