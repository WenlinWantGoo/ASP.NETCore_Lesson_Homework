using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversityDemo.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}