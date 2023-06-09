using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using NdfcAPIsMongoDB.Models;
using Microsoft.AspNetCore.Authorization;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMongoCollection<Commnet> _commentsCollection;

        public CommentController(IMongoDatabase database)
        {
            _commentsCollection = database.GetCollection<Commnet>("comments");
        }

        [HttpGet("{postId}")]
        public ActionResult<List<Commnet>> GetCommentsByPostId(string postId)
        {
            var objectId = ObjectId.Parse(postId);
            var filter = Builders<Commnet>.Filter.Eq(c =>c.PostId, objectId.ToString());
            var comments = _commentsCollection.Find(filter).ToList();
            return comments;
        }


        [HttpPost]
        [Authorize]
        public ActionResult<Commnet> CreateComment(CommentDTO commentDto)
        {
            // Tạo một đối tượng Comment từ CommentDto và lưu vào MongoDB
            var comment = new Commnet
            {
                CommentId = ObjectId.GenerateNewId().ToString(), // MongoDB tự tạo CommentId
                Content = commentDto.Content,
                PostId = commentDto.PostId,
                ParentCommentId = string.IsNullOrEmpty(commentDto.ParentCommentId) ? null : commentDto.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
                UserId = User.FindFirst("accountId")?.Value // Trích xuất UserId từ token
            };

            // Lưu comment vào MongoDB
            _commentsCollection.InsertOne(comment);

            return comment;
        }

    }
}
