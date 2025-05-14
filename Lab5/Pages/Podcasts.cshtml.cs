using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // Для List
using System.Linq; // Для .Any()
using System.Text.Json; // Для JsonSerializer
// using System.Threading.Tasks; // Не нужен, если OnPostToggleFavoritePodcast не async

namespace Lab5.Pages
{
    // Допустим, у нас есть простая модель для данных подкаста на этой странице
    public class PodcastViewModel // Или как у тебя называется модель для подкаста
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public bool IsFavorite { get; set; } // Флаг для отображения иконки
    }

    public class PodcastsModel : PageModel
    {
        public List<PodcastViewModel> Podcasts { get; set; } = new List<PodcastViewModel>();

        private const string SessionKeyFavoritePodcasts = "FavoritePodcastIdsList"; // Новый ключ для подкастов

        [TempData]
        public bool FavoritePodcastStateChanged { get; set; }


        public void OnGet()
        {
            // Загружаем/инициализируем список подкастов
            // ВАЖНО: Это заглушка. У тебя здесь будет твоя логика получения списка подкастов.
            LoadPodcasts();

            // Проверяем для каждого подкаста, в избранном ли он
            var favoritePodcastIds = GetFavoritePodcastIdsFromSession();
            foreach (var podcast in Podcasts)
            {
                podcast.IsFavorite = favoritePodcastIds.Contains(podcast.Id);
            }
        }

        public IActionResult OnPostToggleFavoritePodcast(int podcastId, string? returnUrl)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login", new { ReturnUrl = returnUrl ?? Url.Page("/Podcasts") });
            }

            var favoriteIds = GetFavoritePodcastIdsFromSession();
            if (favoriteIds.Contains(podcastId))
            {
                favoriteIds.Remove(podcastId);
            }
            else
            {
                favoriteIds.Add(podcastId);
            }
            SaveFavoritePodcastIdsToSession(favoriteIds);

            FavoritePodcastStateChanged = true; // Устанавливаем флаг

            return Redirect(returnUrl ?? Url.Page("/Podcasts")); // Возвращаемся на страницу списка
        }

        private void LoadPodcasts()
        {
            // ЗАГЛУШКА: Замени это реальной загрузкой данных подкастов
            Podcasts = new List<PodcastViewModel>
            {
                new PodcastViewModel { Id = 101, Title = "Виды отдыха, которые нужно научиться различать", ImagePath = "/images/podcast_1.png", Duration = "5:13 мин" },
                new PodcastViewModel { Id = 102, Title = "Как побороть прокрастинацию: эффективные методы", ImagePath = "/images/podcast_1.png", Duration = "7:22 мин" }
                // Добавь другие подкасты
            };
        }

        // --- Методы для работы с избранными ПОДКАСТАМИ в сессии ---
        private List<int> GetFavoritePodcastIdsFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKeyFavoritePodcasts);
            if (string.IsNullOrEmpty(json)) return new List<int>();
            try { return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>(); }
            catch (JsonException) { HttpContext.Session.Remove(SessionKeyFavoritePodcasts); return new List<int>(); }
        }

        private void SaveFavoritePodcastIdsToSession(List<int> ids)
        {
            HttpContext.Session.SetString(SessionKeyFavoritePodcasts, JsonSerializer.Serialize(ids));
        }
    }
}