using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab5.Pages
{
    [Authorize]
    public class BookmarksModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
