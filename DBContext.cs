using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using TypicalTools.Models;

namespace DataAccess
{
    public class DBContext
    {
        private readonly IConfiguration config;

        public DBContext(IConfiguration configuration)
        {
            config = configuration;
        }

        #region Comments Methods

        public List<Comment> GetCommentsForProduct(int id)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"SELECT * FROM comments WHERE comments.product_id = {id}";
                return connection.Query<Comment>(sql).AsList();
            }
        }

        public Comment GetSingleComment(int id)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"SELECT * FROM comments WHERE comments.id = {id}";
                return connection.QuerySingle<Comment>(sql);
            }
        }

        public void AddComment(Comment comment)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "INSERT INTO comments (text, product_id, session_id, created_date) " +
                             "VALUES (@text, @product_id, @session_id, @created_date)";
                connection.Execute(sql, comment);
            }
        }

        public void DeleteComment(int id)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"DELETE FROM comments WHERE comments.id = {id}";
                connection.Execute(sql);
            }
        }

        public void EditComment(Comment comment)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "UPDATE comments " +
                             "SET text = @text " +
                             "WHERE comments.id = @id";
                connection.Execute(sql, comment);
            }
        }


        #endregion


        #region Products Methods

        public List<Product> GetProducts()
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"SELECT * FROM products";
                return connection.Query<Product>(sql).AsList();
            }
        }

        public Product GetSingleProduct(int id)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"SELECT * FROM products WHERE products.id = {id}";
                return connection.QuerySingle<Product>(sql);
            }
        }

        public void AddProduct(Product product)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "INSERT INTO products (name, price, description) " +
                             "VALUES (@name, @price, @description)";
                connection.Execute(sql, product);
            }
        }

        public void EditProduct(Product product)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "UPDATE products " +
                             "SET price = @price, " +
                             "updated_date = @updated_date " +
                             "WHERE products.id = @id";
                connection.Execute(sql, product);
            }
        }

        #endregion


        #region User Login

        public User CheckLogin(User account)
        {
            try
            {
                using (var connection = new SqlConnection(config.GetConnectionString("Default")))
                {
                    string sql = "SELECT * FROM users WHERE username = @username";
                    User user = connection.QuerySingle<User>(sql, account);

                    if (user == null)
                    {
                        return null;
                    }

                    string passwordCheck = PasswordHasher.ConvertStringToHash(account.password + user.salt);

                    if (passwordCheck.Equals(user.password))
                    {
                        return user;
                    }
                    return null;
                }
            }

            catch (Exception e)
            {
                return null;
            }            
        }

        public bool CreateAccount(User user)
        {
            try
            {
                using (var connection = new SqlConnection(config.GetConnectionString("Default")))
                {
                    string sql = "SELECT COUNT(*) FROM users WHERE username = @username";
                    int count = connection.QuerySingle<int>(sql, user);

                    if(count > 0)
                    {
                        return false;
                    }

                    user.salt = PasswordHasher.GenerateSalt();
                    user.password = PasswordHasher.ConvertStringToHash(user.password + user.salt);

                    sql = "INSERT INTO users (username, password, salt, role) " +
                        "VALUES (@username, @password, @salt, @role)";
                    connection.Execute(sql, user);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


        #endregion


    }
}
