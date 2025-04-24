using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Lab5.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        // �������� �������� � BindProperty ��� �������� � ����� � ��������� ����
        [BindProperty]
        [Required(ErrorMessage = "��������� ����������")]
        [Display(Name = "���������")]
        public string Nickname { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email ����������")]
        [EmailAddress(ErrorMessage = "������������ ������ email")]
        [Display(Name = "����������� �����")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "���� �����������")]
        public string RegistrationDate { get; private set; } = string.Empty;

        [Display(Name = "��������� ������")]
        public int ArticlesRead { get; set; }
        [Display(Name = "���������� ���������")]
        public int PodcastsListened { get; set; }
        [Display(Name = "��������� ������� �� �����")]
        public string TimeOnSite { get; set; } = string.Empty;
        [Display(Name = "���� ������")]
        public int DaysInRow { get; set; }

        // ---> ������: ��������������� TempData ��� ��������� <---
        //[TempData]
        //public string? StatusMessage { get; set; }
        // ---> ����� <---

        public IActionResult OnGet()
        {
            LoadUserData();
            ArticlesRead = 2;
            PodcastsListened = 2;
            TimeOnSite = "4 ����";
            DaysInRow = 2;
            return Page();
        }

        // ---> ������: ��������������� � ������������ OnPostEditAsync <---
        public async Task<IActionResult> OnPostEditAsync()
        {
            RegistrationDate = HttpContext.Session.GetString("UserRegDate") ?? "����������";
            ArticlesRead = 2; PodcastsListened = 2; TimeOnSite = "4 ����"; DaysInRow = 2;

            if (!ModelState.IsValid)
            {
                // StatusMessage = "������ ���������. ��������� ��������� ������."; // <-- ���������������� ��� �������
                return Page();
            }

            HttpContext.Session.SetString("UserNickname", Nickname);
            HttpContext.Session.SetString("UserEmail", Email);
            await HttpContext.Session.CommitAsync();

            // StatusMessage = "������� ������� ��������!"; // <-- ���������������� ��� �������

            return RedirectToPage();
        }
        // ---> ����� <---


        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // ... (�������� ��� ���������)
            HttpContext.Session.Remove("UserNickname");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserRegDate");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }

        private void LoadUserData()
        {
            if (string.IsNullOrEmpty(Nickname))
            {
                Nickname = HttpContext.Session.GetString("UserNickname") ?? "�����������";
            }
            if (string.IsNullOrEmpty(Email))
            {
                Email = HttpContext.Session.GetString("UserEmail") ?? "����������";
            }
            RegistrationDate = HttpContext.Session.GetString("UserRegDate") ?? "����������";
        }
    }
}