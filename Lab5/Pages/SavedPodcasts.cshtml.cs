using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization; // ��� �������� [Authorize]
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
// System ��� DateTime �� �����, ���� � PodcastViewModel ��� ���, ������� ����� �������������

namespace Lab5.Pages
{
    // ������������, ��� PodcastViewModel ��������� ���-�� (��������, � Podcasts.cshtml.cs ��� � Models)
    // ���� �� �� ���������, �������������� � ������� ��� ����� ��� � ��������� �����:
    /*
    public class PodcastViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public bool IsFavorite { get; set; } // ���� �� ���� �������� ��� ��� ����� IsFavorite = true
    }
    */

    [Authorize] // ������ � ����������� ��������� ������ ��� �������������� �������������
    public class SavedPodcastsModel : PageModel
    {
        public List<PodcastViewModel> SavedPodcastsList { get; set; } = new List<PodcastViewModel>();

        // ���� ������ ��� �������� ID ��������� ��������� (������ ��������� � ������ � Podcasts.cshtml.cs)
        private const string SessionKeyFavoritePodcasts = "FavoritePodcastIdsList";

        [TempData]
        public bool FavoritePodcastStateChangedOnSavedPage { get; set; } // ���� ��� JS ���� �������

        public void OnGet()
        {
            var favoriteIds = GetFavoritePodcastIdsFromSession(); // �������� ID ����������� ���������

            if (favoriteIds.Any())
            {
                // ����� ������ ���� ������ ��������� ���� ��������� ���������,
                // ����� ����� ������������� �� favoriteIds.
                // ��������� � ��� ��� ������������ ������� ������, �� ��������
                // ������ ���� ��������� "�� ����" (��� ��������).
                var allPossiblePodcasts = GetAllPossiblePodcasts(); // �����-��������

                foreach (var id in favoriteIds)
                {
                    var podcast = allPossiblePodcasts.FirstOrDefault(p => p.Id == id);
                    if (podcast != null)
                    {
                        podcast.IsFavorite = true; // �� ���� �������� ��� ��� �� ����������� ���������
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

            // ������ ���� ������ ����� RedirectToPage() ��� ����������,
            // ����� ������������� ������� �������� SavedPodcasts.
            return RedirectToPage();
        }

        // --- ��������������� ������ ��� ������ � ������� (��������� ���, ��� � Podcasts.cshtml.cs) ---
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
                HttpContext.Session.Remove(SessionKeyFavoritePodcasts); // ������� ������������ ������
                return new List<int>();
            }
        }

        private void SaveFavoritePodcastIdsToSession(List<int> ids)
        {
            HttpContext.Session.SetString(SessionKeyFavoritePodcasts, JsonSerializer.Serialize(ids));
        }

        // --- �����-�������� ��� ��������� ���� ��������� ��������� ---
        private List<PodcastViewModel> GetAllPossiblePodcasts()
        {
            // ��� ������ ������ ��������������� ���, ��� ������������ �� �������� Podcasts.cshtml
            return new List<PodcastViewModel>
            {
                new PodcastViewModel { Id = 101, Title = "���� ������, ������� ����� ��������� ���������", ImagePath = "/images/podcast_1.png", Duration = "5:13 ���" },
                new PodcastViewModel { Id = 102, Title = "��� �������� ��������������: ����������� ������", ImagePath = "/images/podcast_1.png", Duration = "7:22 ���" }
                // ������ ���� ��� ��������, ������� ����� ���� � ���������
            };
        }
    }
}