using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lab5.Pages
{
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl ?? "/Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "single_user_id"),
                new Claim(ClaimTypes.Name, "DefaultUser"), // ��� ��� ��������� � ClaimsPrincipal
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { AllowRefresh = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // ---> ������: ��������� ������ ������������ � ������ <---
            HttpContext.Session.SetString("UserNickname", "�����������������");
            HttpContext.Session.SetString("UserEmail", "user@example.com");
            // ��������� ���� ����������� ��� ������ (����� ������� ������ ������)
            HttpContext.Session.SetString("UserRegDate", DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss"));
            // ---> ����� <---

            // ���������� ������ ����������� ���������� (������ �������)
            await HttpContext.Session.CommitAsync();

            return LocalRedirect(returnUrl ?? "/Index");
        }
    }
}

