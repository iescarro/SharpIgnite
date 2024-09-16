using SharpIgnite;

namespace WebSite.Models
{
    public class Post : Model<Post>
    {
        [Column("Id", true, true)]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
    }
}