using System;
using System.Threading.Tasks;
using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostRepository postRepository,
            IUnitOfWork unitOfWork,
            ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var posts = await _postRepository.GetAllPostsAsync();

            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var newPost = new Post
            {
                Author = "admin",
                Body = "1231321312312321312321321",
                Title = "Title A",
                LastModified = DateTime.Now
            };

            _postRepository.AddPost(newPost);

            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
