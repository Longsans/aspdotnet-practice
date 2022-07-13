using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UnitTests
{
    public static class ControllerExtensions
    {
        public static ControllerBase AddEmptyHttpContext(this ControllerBase c)
        {
            c.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            return c;
        }
    }
}
