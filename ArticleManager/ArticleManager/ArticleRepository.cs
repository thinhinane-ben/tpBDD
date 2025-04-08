using System.Collections.Generic;
using System.Data.SqlClient;
using System;

public class ArticleRepository
{
    private readonly string connectionString;

    public ArticleRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public List<Article> GetAll()
    {
        var articles = new List<Article>();
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("SELECT * FROM Articles", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        articles.Add(new Article
                        {
                            Id = (int)reader["Id"],
                            Code = reader["Code"].ToString(),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Brand = reader["Brand"].ToString(),
                            Category = reader["Category"].ToString(),
                            Price = (decimal)reader["Price"],
                            Image = reader["Image"] as byte[]
                        });
                    }
                }
            }
        }
        return articles;
    }

    public void Add(Article article)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var query = @"INSERT INTO Articles (Code, Name, Description, Brand, Category, Price, Image) 
                         VALUES (@Code, @Name, @Description, @Brand, @Category, @Price, @Image);
                         SELECT SCOPE_IDENTITY();";

            using (var cmd = new SqlCommand(query, conn))
            {
                AddParameters(cmd, article);
                article.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    public void Update(Article article)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var query = @"UPDATE Articles 
                         SET Code = @Code, Name = @Name, Description = @Description,
                         Brand = @Brand, Category = @Category, Price = @Price, Image = @Image 
                         WHERE Id = @Id";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", article.Id);
                AddParameters(cmd, article);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void Delete(int id)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("DELETE FROM Articles WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    private void AddParameters(SqlCommand cmd, Article article)
    {
        cmd.Parameters.AddWithValue("@Code", article.Code);
        cmd.Parameters.AddWithValue("@Name", article.Name);
        cmd.Parameters.AddWithValue("@Description", article.Description);
        cmd.Parameters.AddWithValue("@Brand", article.Brand);
        cmd.Parameters.AddWithValue("@Category", article.Category);
        cmd.Parameters.AddWithValue("@Price", article.Price);
        cmd.Parameters.AddWithValue("@Image", article.Image ?? (object)DBNull.Value);
    }
}