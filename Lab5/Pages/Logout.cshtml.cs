using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab5.Pages
{
    public class LogoutModel : PageModel
    {
        // ���� ����� ����� �������� ������, ���� �������� ������ ��� �����������
        public void OnGet()
        {
        }

        // ���� ����� ��������� ��� POST-������� � /Logout
        public async Task<IActionResult> OnPostAsync()
        {
            // ��������� ����� ������������ - ������� cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // �������������� �� ������� ��������
            return RedirectToPage("/Index");
        }
    }
}
