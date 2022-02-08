using System;

namespace Blog.Grpc
{
    public class Post
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateModified { get; set; }
    }
}
