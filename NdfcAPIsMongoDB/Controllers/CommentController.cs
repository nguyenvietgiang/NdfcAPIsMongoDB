using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Models.DTO;
using System.Security.Claims;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class CommentController : BaseController
    {
        private readonly IMongoCollection<Commnet> _commentsCollection;

        public CommentController(IMongoDatabase database, IMemoryCache cache, ILogger<BaseController> logger)
        : base(cache, logger)
        {
            _commentsCollection = database.GetCollection<Commnet>("comments");
        }

        /// <summary>
        /// get all comment for a post by postID - no auth
        /// </summary>
        [HttpGet("{postId}")]
        public ActionResult<List<Commnet>> GetCommentsByPostId(string postId)
        {
            var objectId = ObjectId.Parse(postId);
            var filter = Builders<Commnet>.Filter.Eq(c => c.PostId, objectId.ToString());
            var comments = _commentsCollection.Find(filter).ToList();
            return comments;
        }

        /// <summary>
        /// current user create new comment
        /// </summary>
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
                UserId = User.FindFirstValue(ClaimTypes.Name) // Trích xuất UserName từ token
            };

            // Lưu comment vào MongoDB
            _commentsCollection.InsertOne(comment);

            return comment;
        }

        /// <summary>
        /// delete comment
        /// </summary>
        [HttpDelete]
        [Authorize]
        [Route("{commentId}")]
        public ActionResult DeleteComment(string commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            // Kiểm tra xem người dùng có quyền xóa comment hay không
            var comment = _commentsCollection.Find(x => x.CommentId == commentId && x.UserId == userId).FirstOrDefault();
            if (comment == null)
            {
                return NotFound(); // Trả về mã lỗi 404 nếu comment không tồn tại hoặc người dùng không có quyền xóa
            }

            // Xóa comment khỏi MongoDB
            _commentsCollection.DeleteOne(x => x.CommentId == commentId);

            return Ok(); // Trả về mã thành công 200 nếu xóa thành công
        }

    }
}
