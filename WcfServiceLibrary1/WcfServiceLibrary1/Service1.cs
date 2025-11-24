using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace WcfServiceLibrary1
{
    public class Service1 : IService1
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public Service1()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";

            EnsureDatabase();
        }

        // --- Публичные методы контракта ---

        public List<UserDto> GetUsers()
        {
            var users = new List<UserDto>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT Email, Password FROM Users";

                using (var cmd = new SQLiteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserDto
                        {
                            Email = reader.GetString(0),
                            Password = reader.GetString(1)
                        });
                    }
                }
            }

            return users;
        }

        public void AddUser(string email, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "INSERT INTO Users (Email, Password) VALUES (@Email, @Password)";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- Внутренняя инициализация БД ---

        private void EnsureDatabase()
        {
            bool needSeed = !File.Exists(_dbPath);

            if (needSeed)
            {
                SQLiteConnection.CreateFile(_dbPath);

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string createTableSql = @"
                        CREATE TABLE Users (
                            Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                            Email    TEXT NOT NULL,
                            Password TEXT NOT NULL
                        );
                    ";

                    using (var cmd = new SQLiteCommand(createTableSql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string insertSql = @"
                        INSERT INTO Users (Email, Password) VALUES ('user1@example.com', 'pass1');
                        INSERT INTO Users (Email, Password) VALUES ('user2@example.com', 'pass2');
                    ";

                    using (var cmd = new SQLiteCommand(insertSql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
