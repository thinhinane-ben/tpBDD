using System.Collections.Generic;
using System.Linq;

public class ArticleController
{
    private readonly string connectionString;
    private readonly ArticleRepository repository;

    public ArticleController(string connectionString)
    {
        this.connectionString = connectionString;
        this.repository = new ArticleRepository(connectionString);
    }

    public List<Article> GetAllArticles()
    {
        return repository.GetAll();
    }

    public void AddArticle(Article article)
    {
        repository.Add(article);
    }

    public void UpdateArticle(Article article)
    {
        repository.Update(article);
    }

    public void DeleteArticle(int id)
    {
        repository.Delete(id);
    }

    public List<Article> FilterArticles(string searchText, bool showInvalid)
    {
        var articles = repository.GetAll();

        if (!string.IsNullOrEmpty(searchText))
        {
            articles = articles.Where(a =>
                (a.Name?.ToLower().Contains(searchText) ?? false) ||
                (a.Description?.ToLower().Contains(searchText) ?? false) ||
                (a.Brand?.ToLower().Contains(searchText) ?? false) ||
                (a.Category?.ToLower().Contains(searchText) ?? false) ||
                (a.Code?.ToLower().Contains(searchText) ?? false)
            ).ToList();
        }

        return showInvalid ?
            articles.Where(a => !IsArticleValid(a)).ToList() :
            articles.Where(a => IsArticleValid(a)).ToList();
    }

    private bool IsArticleValid(Article article)
    {
        return !(string.IsNullOrWhiteSpace(article.Code) ||
                string.IsNullOrWhiteSpace(article.Name) ||
                string.IsNullOrWhiteSpace(article.Description) ||
                string.IsNullOrWhiteSpace(article.Brand) ||
                string.IsNullOrWhiteSpace(article.Category) ||
                article.Price <= 0);
    }
}