using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace FrontEndApp.Pages.Account.LogoutModel
{
    public class Logout : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Remove("JwtToken");
            return RedirectToPage("/Index");
        }
    }
}
