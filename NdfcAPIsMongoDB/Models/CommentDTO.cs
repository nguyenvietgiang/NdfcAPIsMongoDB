﻿namespace NdfcAPIsMongoDB.Models
{
    public class CommentDTO
    {
         public string Content { get; set; }
            public string PostId { get; set; }
            public string ParentCommentId { get; set; }
        }
}
