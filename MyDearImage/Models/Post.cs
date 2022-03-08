using MyDearImage.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MyDearImage.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public int LikeCount { get; set; }
        public string Description { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }
    }
}