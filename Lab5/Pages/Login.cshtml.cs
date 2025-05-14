using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab5.Pages
{
    public class LoginModel : PageModel
    {

        // Ключ сессии для избранных статей (должен совпадать с ключом в других моделях)
        private const string SessionKeyFavoriteArticles = "FavoriteArticleIdsList";
        private const string SessionKeyFavoritePodcasts = "FavoritePodcastIdsList";

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl ?? "/Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "single_user_id"),
                new Claim(ClaimTypes.Name, "DefaultUser"), // Это имя останется в ClaimsPrincipal
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { AllowRefresh = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // ---> НАЧАЛО: Сохраняем данные пользователя в сессию <---
            HttpContext.Session.SetString("UserNickname", "СуперПользователь");
            HttpContext.Session.SetString("UserEmail", "user@example.com");
            // Сохраняем дату регистрации как строку (можно выбрать другой формат)
            HttpContext.Session.SetString("UserRegDate", DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss"));
            // ---> КОНЕЦ <---

            // ---> НАЧАЛО: Добавляем статью с ID=1 в избранное по умолчанию <---
            // Проверяем, есть ли уже что-то в избранном (маловероятно при первом входе, но на всякий случай)
            var favoriteArticleIdsJson = HttpContext.Session.GetString(SessionKeyFavoriteArticles);
            List<int> favoriteArticleIds;

            if (!string.IsNullOrEmpty(favoriteArticleIdsJson))
            {
                try
                {
                    favoriteArticleIds = JsonSerializer.Deserialize<List<int>>(favoriteArticleIdsJson) ?? new List<int>();
                }
                catch (JsonException)
                {
                    favoriteArticleIds = new List<int>(); // Если данные повреждены, начинаем с нуля
                }
            }
            else
            {
                favoriteArticleIds = new List<int>();
            }

            // Добавляем ID=1, если его еще нет
            if (!favoriteArticleIds.Contains(1))
            {
                favoriteArticleIds.Add(1);
            }

            // Сохраняем обновленный список в сессию
            HttpContext.Session.SetString(SessionKeyFavoriteArticles, JsonSerializer.Serialize(favoriteArticleIds));
            // ---> КОНЕЦ <---

            // ---> НАЧАЛО: Добавляем подкаст с ID=101 (или другим) в избранное по умолчанию <---
            var favoritePodcastIdsJson = HttpContext.Session.GetString(SessionKeyFavoritePodcasts);
            List<int> favoritePodcastIds;

            if (!string.IsNullOrEmpty(favoritePodcastIdsJson))
            {
                try
                {
                    favoritePodcastIds = JsonSerializer.Deserialize<List<int>>(favoritePodcastIdsJson) ?? new List<int>();
                }
                catch (JsonException)
                {
                    favoritePodcastIds = new List<int>(); // Если данные повреждены, начинаем с нуля
                }
            }
            else
            {
                favoritePodcastIds = new List<int>();
            }

            // Добавляем ID подкаста (например, 101), если его еще нет
            int defaultPodcastId = 101; // ЗАМЕНИ НА ID РЕАЛЬНОГО ПОДКАСТА
            if (!favoritePodcastIds.Contains(defaultPodcastId))
            {
                favoritePodcastIds.Add(defaultPodcastId);
            }

            // Сохраняем обновленный список в сессию
            HttpContext.Session.SetString(SessionKeyFavoritePodcasts, JsonSerializer.Serialize(favoritePodcastIds));
            // ---> КОНЕЦ <---

            // Заставляем сессию сохраниться немедленно (иногда полезно)
            await HttpContext.Session.CommitAsync();

            return LocalRedirect(returnUrl ?? "/Index");
        }
    }
}

