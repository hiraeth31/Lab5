using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization; // Для атрибута [Authorize]
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
// System для DateTime не нужен, если в PodcastViewModel нет дат, которые нужно форматировать

namespace Lab5.Pages
{
    // Предполагаем, что PodcastViewModel определен где-то (например, в Podcasts.cshtml.cs или в Models)
    // Если он не определен, раскомментируй и настрой его здесь или в отдельном файле:
    /*
    public class PodcastViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public bool IsFavorite { get; set; } // Хотя на этой странице они все будут IsFavorite = true
    }
    */

    [Authorize] // Доступ к сохраненным подкастам только для авторизованных пользователей
    public class SavedPodcastsModel : PageModel
    {
        public List<PodcastViewModel> SavedPodcastsList { get; set; } = new List<PodcastViewModel>();

        // Ключ сессии для хранения ID избранных подкастов (должен совпадать с ключом в Podcasts.cshtml.cs)
        private const string SessionKeyFavoritePodcasts = "FavoritePodcastIdsList";

        [TempData]
        public bool FavoritePodcastStateChangedOnSavedPage { get; set; } // Флаг для JS хака истории

        public void OnGet()
        {
            var favoriteIds = GetFavoritePodcastIdsFromSession(); // Получаем ID сохраненных подкастов

            if (favoriteIds.Any())
            {
                // Здесь должна быть логика получения ВСЕХ возможных подкастов,
                // чтобы потом отфильтровать по favoriteIds.
                // Поскольку у нас нет центрального сервиса данных, мы создадим
                // список всех подкастов "на лету" (это ЗАГЛУШКА).
                var allPossiblePodcasts = GetAllPossiblePodcasts(); // Метод-заглушка

                foreach (var id in favoriteIds)
                {
                    var podcast = allPossiblePodcasts.FirstOrDefault(p => p.Id == id);
                    if (podcast != null)
                    {
                        podcast.IsFavorite = true; // На этой странице они все по определению избранные
                        SavedPodcastsList.Add(podcast);
                    }
                }
            }
        }

        public IActionResult OnPostRemoveFavoritePodcast(int podcastId)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var favoriteIds = GetFavoritePodcastIdsFromSession();
            if (favoriteIds.Contains(podcastId))
            {
                favoriteIds.Remove(podcastId);
                SaveFavoritePodcastIdsToSession(favoriteIds);
            }

            FavoritePodcastStateChangedOnSavedPage = true;

            // Должен быть просто вызов RedirectToPage() без аргументов,
            // чтобы перезагрузить текущую страницу SavedPodcasts.
            return RedirectToPage();
        }

        // --- Вспомогательные методы для работы с сессией (идентичны тем, что в Podcasts.cshtml.cs) ---
        private List<int> GetFavoritePodcastIdsFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKeyFavoritePodcasts);
            if (string.IsNullOrEmpty(json))
            {
                return new List<int>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch (JsonException)
            {
                HttpContext.Session.Remove(SessionKeyFavoritePodcasts); // Очищаем поврежденные данные
                return new List<int>();
            }
        }

        private void SaveFavoritePodcastIdsToSession(List<int> ids)
        {
            HttpContext.Session.SetString(SessionKeyFavoritePodcasts, JsonSerializer.Serialize(ids));
        }

        // --- Метод-заглушка для получения всех возможных подкастов ---
        private List<PodcastViewModel> GetAllPossiblePodcasts()
        {
            // Эти данные должны соответствовать тем, что используются на странице Podcasts.cshtml
            return new List<PodcastViewModel>
            {
                new PodcastViewModel { Id = 101, Title = "Виды отдыха, которые нужно научиться различать", ImagePath = "/images/podcast_1.png", Duration = "5:13 мин" },
                new PodcastViewModel { Id = 102, Title = "Как побороть прокрастинацию: эффективные методы", ImagePath = "/images/podcast_1.png", Duration = "7:22 мин" }
                // Добавь сюда все подкасты, которые могут быть в избранном
            };
        }
    }
}