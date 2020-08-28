using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthTest.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/user/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] NewRoleForm newRole)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);
            if(!await _roleManager.RoleExistsAsync(newRole.NewRole))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = newRole.NewRole });
            }

            await _userManager.AddToRoleAsync(user, newRole.NewRole);

            return Ok();
        }
    }


    public class NewRoleForm
    {
        public string NewRole { get; set; }
    }
}
