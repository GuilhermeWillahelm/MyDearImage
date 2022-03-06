using MyDearImage.Areas.Identity.Data;

namespace MyDearImage.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
       
    }
}
