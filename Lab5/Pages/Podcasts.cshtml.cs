using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // ��� List
using System.Linq; // ��� .Any()
using System.Text.Json; // ��� JsonSerializer
// using System.Threading.Tasks; // �� �����, ���� OnPostToggleFavoritePodcast �� async

namespace Lab5.Pages
{
    // ��������, � ��� ���� ������� ������ ��� ������ �������� �� ���� ��������
    public class PodcastViewModel // ��� ��� � ���� ���������� ������ ��� ��������
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public bool IsFavorite { get; set; } // ���� ��� ����������� ������
    }

    public class PodcastsModel : PageModel
    {
        public List<PodcastViewModel> Podcasts { get; set; } = new List<PodcastViewModel>();

        private const string SessionKeyFavoritePodcasts = "FavoritePodcastIdsList"; // ����� ���� ��� ���������

        [TempData]
        public bool FavoritePodcastStateChanged { get; set; }


        public void OnGet()
        {
            // ���������/�������������� ������ ���������
            // �����: ��� ��������. � ���� ����� ����� ���� ������ ��������� ������ ���������.
            LoadPodcasts();

            // ��������� ��� ������� ��������, � ��������� �� ��
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

            FavoritePodcastStateChanged = true; // ������������� ����

            return Redirect(returnUrl ?? Url.Page("/Podcasts")); // ������������ �� �������� ������
        }

        private void LoadPodcasts()
        {
            // ��������: ������ ��� �������� ��������� ������ ���������
            Podcasts = new List<PodcastViewModel>
            {
                new PodcastViewModel { Id = 101, Title = "���� ������, ������� ����� ��������� ���������", ImagePath = "/images/podcast_1.png", Duration = "5:13 ���" },
                new PodcastViewModel { Id = 102, Title = "��� �������� ��������������: ����������� ������", ImagePath = "/images/podcast_1.png", Duration = "7:22 ���" }
                // ������ ������ ��������
            };
        }

        // --- ������ ��� ������ � ���������� ���������� � ������ ---
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