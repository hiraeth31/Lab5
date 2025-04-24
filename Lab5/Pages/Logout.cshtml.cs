using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab5.Pages
{
    public class LogoutModel : PageModel
    {
        // Этот метод можно оставить пустым, если страница просто для отображения
        public void OnGet()
        {
        }

        // Этот метод сработает при POST-запросе к /Logout
        public async Task<IActionResult> OnPostAsync()
        {
            // Выполняем выход пользователя - удаляем cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Перенаправляем на главную страницу
            return RedirectToPage("/Index");
        }
    }
}
