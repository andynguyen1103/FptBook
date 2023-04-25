using FptBook.Areas.Admin.Models;
using FptBook.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FptBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role/[action]")]
    [Authorize]
    // [Authorize(Roles = RoleNames.Administrator)]
    public class RoleController : Controller
    {
        private readonly FptBookIdentityDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        [TempData] // Sử dụng Session lưu thông báo
        public string StatusMessage { get; set; }
        
        public RoleController(FptBookIdentityDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        // [HttpGet("/Admin/Role", Name = "RoleManageDefault")]
        [HttpGet("/Admin/Role")]
        public IActionResult Index()
        {
            var roleList = _roleManager.Roles.ToList();
            return View(roleList);
        }
        
        
        // [HttpPost("/Admin/Role/Create", Name = "RoleManageCreatePost")]
        public async Task<ActionResult> Create([FromBody]RoleInput roleInput)
        {
            if (!ModelState.IsValid) return BadRequest();
            Console.WriteLine(roleInput.name);
            
            var roleExist = _roleManager.RoleExistsAsync(roleInput.name).Result;
            if (!roleExist)
            {
                //create the roles and seed them to the database: Question 2
                await _roleManager.CreateAsync(new IdentityRole(roleInput.name));
                StatusMessage = "Role Created Successfully";
                return PartialView("_RoleTablePartial",_roleManager.Roles.ToList());
            }

            StatusMessage = "Role Existed";
            return PartialView("_RoleTablePartial",_roleManager.Roles.ToList());

            // return View();
        }
        
        
        // [HttpPost("/Admin/Role/Update")]
        public async Task<ActionResult> Update([FromBody]RoleInput roleInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Console.WriteLine(roleInput.name+roleInput.id);
            var role = _roleManager.FindByIdAsync(roleInput.id).Result;
            if (role == null)
            {
                
            }
            if (roleInput.name != role.Name)
            {
                role.Name = roleInput.name;
                await _roleManager.UpdateAsync(role);
            }

            return PartialView("_RoleTablePartial",_roleManager.Roles.ToList());
        }

        // [HttpPost("/Admin/Role/Delete", Name = "RoleManageDeletePost")]
        public async Task<ActionResult> Delete([FromBody] RoleInput roleInput)
        {
            if (!ModelState.IsValid) return BadRequest();
            Console.WriteLine(roleInput.id);
            
            var role = await _roleManager.FindByIdAsync(roleInput.id);
            if (role == null)
            {
                StatusMessage = "Role Not Existed";
                return PartialView("_RoleTablePartial",_roleManager.Roles.ToList());
            }

            await _roleManager.DeleteAsync(role);
            
            return PartialView("_RoleTablePartial",_roleManager.Roles.ToList());
        }
    }
}