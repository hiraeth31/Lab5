using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization; // ��� [Authorize]
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System; // ��� DateTime � ArticleData

namespace Lab5.Pages
{
    [Authorize] // ��� �������� �������� ������ �������������� �������������
    public class SavedArticlesModel : PageModel
    {
        public List<ArticleData> SavedArticlesList { get; set; } = new List<ArticleData>();
        private const string SessionKeyFavoriteArticles = "FavoriteArticleIdsList"; // ��� �� ����, ��� � � ArticleDetailsModel

        public void OnGet()
        {
            var favoriteIds = GetFavoriteArticleIdsFromSession();
            if (favoriteIds.Any())
            {
                // ��������� ������ ��� ������ ��������� ������
                // �����: ��� ���������� ��������. � �������� ���������� ����� ������/�����������.
                foreach (var id in favoriteIds)
                {
                    var article = GetArticleDataById(id); // ����� ��� ��������� ������ ������ �� ID
                    if (article != null)
                    {
                        SavedArticlesList.Add(article);
                    }
                }
                // ����� �������������, ���� �����, ��������, �� ���� ���������� (�� �� �� �� ������)
                // ��� �� ���� ���������� ������
                SavedArticlesList = SavedArticlesList.OrderByDescending(a => a.PublishDate).ToList();
            }
        }

        public IActionResult OnPostRemoveFromFavorites(int articleId)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                // ������������, ���� �� ������ ������� ����������������, �.�. �������� � [Authorize]
                return Forbid();
            }

            var favoriteIds = GetFavoriteArticleIdsFromSession();
            if (favoriteIds.Contains(articleId))
            {
                favoriteIds.Remove(articleId);
                SaveFavoriteArticleIdsToSession(favoriteIds);
            }

            // ������������� ��������, ����� �������� ������ ������������ ������
            return RedirectToPage();
        }


        // --- ��������������� ������ ��� ������ � ������� (����� ������� � ����� �����/������) ---
        private List<int> GetFavoriteArticleIdsFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKeyFavoriteArticles);
            if (string.IsNullOrEmpty(json))
            {
                return new List<int>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch (JsonException) { return new List<int>(); }
        }

        private void SaveFavoriteArticleIdsToSession(List<int> ids)
        {
            HttpContext.Session.SetString(SessionKeyFavoriteArticles, JsonSerializer.Serialize(ids));
        }

        // --- ��������������� ����� ��� ��������� ������ ������ (��������) ---
        // �����: ������ ��� �� �������� ������ ��������� ������ ������ �� ������ ���������
        // (��������, ����� ������ �� ArticleDetailsModel ��� ������ �������)
        private ArticleData? GetArticleDataById(int articleId)
        {
            // ��� ��������� ��������, ����������� �������� ������.
            // ���� ����� ����� ������������ ��� ��� ��, ��� � ���� ������� ��������
            // ��� ������������ ������ ��� ArticleData.
            if (articleId == 1)
            {
                return new ArticleData { Id = 1, Title = "��� ������ ����� ������� ���������?", ImagePath = "/images/article_1.png", PublishDate = new DateTime(2025, 3, 2), Content = "������� �������� ��� �����" };
            }
            else if (articleId == 2)
            {
                return new ArticleData { Id = 2, Title = "��� ����� ������� ����������: ������ �� �� � ���������?", ImagePath = "/images/article_2.png", PublishDate = new DateTime(2025, 3, 1), Content = "������� �������� ��� �����" };
            }
            // ������ ������ ������, ���� ��� ����
            return null; // ���� ������ � ����� ID �� �������
        }
    }

    // ����� ArticleData ������ ���� �������� �����.
    // ���� �� ��������� � ArticleDetailsModel.cshtml.cs, �������, ��� �� public
    // ��� ������ ��� � ��������� ���� � ����� Models.
    // public class ArticleData { ... }
}