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

        // ���� ������ ��� ��������� ������ (������ ��������� � ������ � ������ �������)
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

            // ---> ������: ��������� ������ � ID=1 � ��������� �� ��������� <---
            // ���������, ���� �� ��� ���-�� � ��������� (������������ ��� ������ �����, �� �� ������ ������)
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
                    favoriteArticleIds = new List<int>(); // ���� ������ ����������, �������� � ����
                }
            }
            else
            {
                favoriteArticleIds = new List<int>();
            }

            // ��������� ID=1, ���� ��� ��� ���
            if (!favoriteArticleIds.Contains(1))
            {
                favoriteArticleIds.Add(1);
            }

            // ��������� ����������� ������ � ������
            HttpContext.Session.SetString(SessionKeyFavoriteArticles, JsonSerializer.Serialize(favoriteArticleIds));
            // ---> ����� <---

            // ---> ������: ��������� ������� � ID=101 (��� ������) � ��������� �� ��������� <---
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
                    favoritePodcastIds = new List<int>(); // ���� ������ ����������, �������� � ����
                }
            }
            else
            {
                favoritePodcastIds = new List<int>();
            }

            // ��������� ID �������� (��������, 101), ���� ��� ��� ���
            int defaultPodcastId = 101; // ������ �� ID ��������� ��������
            if (!favoritePodcastIds.Contains(defaultPodcastId))
            {
                favoritePodcastIds.Add(defaultPodcastId);
            }

            // ��������� ����������� ������ � ������
            HttpContext.Session.SetString(SessionKeyFavoritePodcasts, JsonSerializer.Serialize(favoritePodcastIds));
            // ---> ����� <---

            // ���������� ������ ����������� ���������� (������ �������)
            await HttpContext.Session.CommitAsync();

            return LocalRedirect(returnUrl ?? "/Index");
        }
    }
}

