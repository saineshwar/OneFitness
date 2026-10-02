using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OneFitness.API.Extensions
{
    public static class ControllerExtensions
    {
        public static int? GetCurrentUserId(this ControllerBase controller)
        {
            var value = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
