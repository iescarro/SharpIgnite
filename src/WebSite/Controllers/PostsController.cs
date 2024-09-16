using SharpIgnite;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class PostsController : Controller
    {
        public PostsController()
        {
        }

        public IActionResult Index()
        {
            var posts = Post.All();
            var data = Array.New()
                .Add("posts", posts)
                .Add("info", Session.FlashData("info"));
            return View("Posts/Index", data);
        }

        public IActionResult Show()
        {
            int id = Input.Get<int>("id");
            var post = Post.Read(new { Id = id });
            var data = Array.New()
                .Add("post", post);
            return View("Posts/Show", data);
        }

        public IActionResult Create()
        {
            if (Input.Post()) {
                var post = new Post {
                    Title = Input.Post("Title"),
                    Content = Input.Post("Content"),
                };
                post.Save();
                Session.FlashData("Info", "Post successfully saved.");
                return Redirect("Posts");
            }
            var data = Array.New();
            return View("Posts/Create", data);
        }

        public IActionResult Edit()
        {
            int id = Input.Get<int>("Id");
            var post = Post.Read(new { Id = id });
            if (Input.Post()) {
                post.Title = Input.Post("Title");
                post.Content = Input.Post("Content");
                post.Update();
                Session.FlashData("Info", "Post successfully updated.");
                return Redirect("Posts");
            }
            var data = Array.New()
                .Add("Id", id)
                .Add("Post", post)
                .Add("Info", Session.FlashData("Info"));
            return View("Posts/Edit", data);
        }

        public IActionResult Delete()
        {
            int id = Input.Get<int>("Id");
            var post = Post.Read(new { Id = id });
            post.Delete();
            Session.FlashData("Info", "Post successfully deleted.");
            return Redirect("Posts");
        }
    }
}