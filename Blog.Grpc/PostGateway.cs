using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Grpc
{
    public class PostGateway : IDisposable
    {

        const string ConnectionString = "Server=database;Database=blog;Uid=user;Pwd=password;";

        private readonly MySqlConnection connection;

        public PostGateway()
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            connection?.Dispose();
        }

        public long Insert(string author, string title, string content)
        {
            var query = $@"INSERT INTO Post 
                           VALUES(null, @{nameof(author)}, @{nameof(title)}, @{nameof(content)}, null, null)";

            using var command = CreateCommand(query);

            command.Parameters.AddWithValue($"@{nameof(author)}", author);
            command.Parameters.AddWithValue($"@{nameof(title)}", title);
            command.Parameters.AddWithValue($"@{nameof(content)}", content);

            return command.ExecuteNonQuery();
        }

        private MySqlCommand CreateCommand(string query) => new(query, connection);

        public Post Select(int id)
        {
            var query = $@"SELECT Author, Title, Content, DatePosted, DateModified 
                           FROM Post
                           WHERE Id = @{nameof(id)}";

            using var command = CreateCommand(query);

            command.Parameters.AddWithValue($"@{nameof(id)}", id);

            var reader = command.ExecuteReader();
            if(reader.Read())
            {
                return new Post
                {
                    Id = id,
                    Author = reader.GetString(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    DatePosted = reader.GetDateTime(3).ToUniversalTime(),
                    DateModified = reader.GetDateTime(4).ToUniversalTime()
                };
            }

            return null;
        }
    }
}
