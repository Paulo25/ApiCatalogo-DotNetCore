using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    //[ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    //[Route("api/v{v:apiVersion}/teste")]
    [Route("api/teste")]
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV1Controller - GET V 1.0</h2></html></body>", "text/html");
        }

        /*[HttpGet, MapToApiVersion("2.0")]
        public IActionResult GetVersao2()
        {
            return Content("<html><body><h2>TesteV2Controller - GET V 2.0x</h2></html></body>", "text/html");
        */
        
    }
}
