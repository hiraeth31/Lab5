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
        // Свойства остаются с BindProperty для привязки к форме в модальном окне
        [BindProperty]
        [Required(ErrorMessage = "Псевдоним обязателен")]
        [Display(Name = "Псевдоним")]
        public string Nickname { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Дата регистрации")]
        public string RegistrationDate { get; private set; } = string.Empty;

        [Display(Name = "Прочитано статей")]
        public int ArticlesRead { get; set; }
        [Display(Name = "Прослушано подкастов")]
        public int PodcastsListened { get; set; }
        [Display(Name = "Проведено времени на сайте")]
        public string TimeOnSite { get; set; } = string.Empty;
        [Display(Name = "Дней подряд")]
        public int DaysInRow { get; set; }

        // ---> НАЧАЛО: Восстанавливаем TempData для сообщений <---
        //[TempData]
        //public string? StatusMessage { get; set; }
        // ---> КОНЕЦ <---

        public IActionResult OnGet()
        {
            LoadUserData();
            ArticlesRead = 2;
            PodcastsListened = 2;
            TimeOnSite = "4 часа";
            DaysInRow = 2;
            return Page();
        }

        // ---> НАЧАЛО: Раскомментируем и дорабатываем OnPostEditAsync <---
        public async Task<IActionResult> OnPostEditAsync()
        {
            RegistrationDate = HttpContext.Session.GetString("UserRegDate") ?? "Неизвестно";
            ArticlesRead = 2; PodcastsListened = 2; TimeOnSite = "4 часа"; DaysInRow = 2;

            if (!ModelState.IsValid)
            {
                // StatusMessage = "Ошибка валидации. Проверьте введенные данные."; // <-- Закомментировано или удалено
                return Page();
            }

            HttpContext.Session.SetString("UserNickname", Nickname);
            HttpContext.Session.SetString("UserEmail", Email);
            await HttpContext.Session.CommitAsync();

            // StatusMessage = "Профиль успешно обновлен!"; // <-- Закомментировано или удалено

            return RedirectToPage();
        }
        // ---> КОНЕЦ <---


        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // ... (остается без изменений)
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
                Nickname = HttpContext.Session.GetString("UserNickname") ?? "Неизвестный";
            }
            if (string.IsNullOrEmpty(Email))
            {
                Email = HttpContext.Session.GetString("UserEmail") ?? "Неизвестно";
            }
            RegistrationDate = HttpContext.Session.GetString("UserRegDate") ?? "Неизвестно";
        }
    }
}