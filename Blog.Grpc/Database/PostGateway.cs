using MySqlConnector;
using System;

namespace Blog.Grpc
{
    public class PostGateway : IDisposable
    {
        public PostGateway()
        {
            InitializeConnection();
        }

        /// <summary>
        /// Insert blog post into database
        /// </summary>
        public bool Insert(string author, string title, string content)
        {
            var query = $@"INSERT INTO {PostTable} 
                           VALUES(NULL, @{nameof(author)}, @{nameof(title)}, @{nameof(content)}, NULL, NULL)";

            using var command = CreateCommand(query);

            command.Parameters.AddWithValue($"@{nameof(author)}", author);
            command.Parameters.AddWithValue($"@{nameof(title)}", title);
            command.Parameters.AddWithValue($"@{nameof(content)}", content);

            var linesInserted = command.ExecuteNonQuery();
            return linesInserted > 0;
        }

        /// <summary>
        /// Select blog post with specified id from database
        /// </summary>
        public Post Select(int id)
        {
            var query = $@"SELECT {AuthorColumn}, {TitleColumn}, {ContentColumn}, {DatePostedColumn}, {DateModifiedColumn} 
                           FROM {PostTable} 
                           WHERE {IdColumn} = @{nameof(id)}";

            using var command = CreateCommand(query);

            command.Parameters.AddWithValue($"@{nameof(id)}", id);

            var reader = command.ExecuteReader();
            if (reader.Read())
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

        /// <summary>
        /// Update blog post with specified id
        /// </summary>
        internal bool Update(int id, string author, string title, string content)
        {
            var query = $@"UPDATE {PostTable} 
                           SET {AuthorColumn}=@{nameof(author)}, {TitleColumn}=@{nameof(title)}, {ContentColumn}=@{nameof(content)}, {DateModifiedColumn}=NOW()
                           WHERE {IdColumn} = @{nameof(id)}";

            using var command = CreateCommand(query);

            command.Parameters.AddWithValue($"@{nameof(author)}", author);
            command.Parameters.AddWithValue($"@{nameof(title)}", title);
            command.Parameters.AddWithValue($"@{nameof(content)}", content);
            command.Parameters.AddWithValue($"@{nameof(id)}", id);

            var linesUpdated = command.ExecuteNonQuery();
            return linesUpdated > 0;
        }

        /// <summary>
        /// Delete blog post with specified id from database
        /// </summary>
        internal bool Delete(int id)
        {
            var query = $@"DELETE FROM {PostTable} 
                           WHERE {IdColumn} = @{nameof(id)}";

            using var command = CreateCommand(query);

            command.Parameters.AddWithValue($"@{nameof(id)}", id);

            var linesDeleted = command.ExecuteNonQuery();
            return linesDeleted > 0;
        }

        #region PrivateMembers

        private void InitializeConnection()
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
        }

        private MySqlCommand CreateCommand(string query) => new(query, connection);

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            connection?.Dispose();
        }

        private const string ConnectionString = "Server=database;Database=blog;Uid=user;Pwd=password;";
        private const string PostTable = "Post";
        private const string IdColumn = "Id";
        private const string AuthorColumn = "Author";
        private const string TitleColumn = "Title";
        private const string ContentColumn = "Content";
        private const string DatePostedColumn = "DatePosted";
        private const string DateModifiedColumn = "DateModified";
        private MySqlConnection connection;
        #endregion
    }
}
