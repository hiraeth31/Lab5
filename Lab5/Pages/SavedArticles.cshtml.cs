using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization; // Для [Authorize]
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System; // Для DateTime в ArticleData

namespace Lab5.Pages
{
    [Authorize] // Эта страница доступна только авторизованным пользователям
    public class SavedArticlesModel : PageModel
    {
        public List<ArticleData> SavedArticlesList { get; set; } = new List<ArticleData>();
        private const string SessionKeyFavoriteArticles = "FavoriteArticleIdsList"; // Тот же ключ, что и в ArticleDetailsModel

        public void OnGet()
        {
            var favoriteIds = GetFavoriteArticleIdsFromSession();
            if (favoriteIds.Any())
            {
                // Загружаем данные для каждой избранной статьи
                // ВАЖНО: Это упрощенная загрузка. В реальном приложении нужен сервис/репозиторий.
                foreach (var id in favoriteIds)
                {
                    var article = GetArticleDataById(id); // Метод для получения данных статьи по ID
                    if (article != null)
                    {
                        SavedArticlesList.Add(article);
                    }
                }
                // Можно отсортировать, если нужно, например, по дате добавления (но мы ее не храним)
                // или по дате публикации статьи
                SavedArticlesList = SavedArticlesList.OrderByDescending(a => a.PublishDate).ToList();
            }
        }

        public IActionResult OnPostRemoveFromFavorites(int articleId)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                // Теоретически, сюда не должны попасть неавторизованные, т.к. страница с [Authorize]
                return Forbid();
            }

            var favoriteIds = GetFavoriteArticleIdsFromSession();
            if (favoriteIds.Contains(articleId))
            {
                favoriteIds.Remove(articleId);
                SaveFavoriteArticleIdsToSession(favoriteIds);
            }

            // Перезагружаем страницу, чтобы обновить список отображаемых статей
            return RedirectToPage();
        }


        // --- Вспомогательные методы для работы с сессией (можно вынести в общий класс/сервис) ---
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

        // --- Вспомогательный метод для получения данных статьи (заглушка) ---
        // ВАЖНО: Замени это на реальную логику получения данных статьи из твоего источника
        // (например, вызов метода из ArticleDetailsModel или общего сервиса)
        private ArticleData? GetArticleDataById(int articleId)
        {
            // Это временная заглушка, имитирующая загрузку данных.
            // Тебе нужно будет адаптировать это под то, как у тебя реально хранятся
            // или генерируются данные для ArticleData.
            if (articleId == 1)
            {
                return new ArticleData { Id = 1, Title = "Вам трудно вести сложные разговоры?", ImagePath = "/images/article_1.png", PublishDate = new DateTime(2025, 3, 2), Content = "Краткое описание или пусто" };
            }
            else if (articleId == 2)
            {
                return new ArticleData { Id = 2, Title = "Что такое синдром самозванца: связан ли он с личностью?", ImagePath = "/images/article_2.png", PublishDate = new DateTime(2025, 3, 1), Content = "Краткое описание или пусто" };
            }
            // Добавь другие статьи, если они есть
            return null; // Если статья с таким ID не найдена
        }
    }

    // Класс ArticleData должен быть доступен здесь.
    // Если он определен в ArticleDetailsModel.cshtml.cs, убедись, что он public
    // или вынеси его в отдельный файл в папку Models.
    // public class ArticleData { ... }
}