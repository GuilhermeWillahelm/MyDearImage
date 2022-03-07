using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyDearImage.Models;

namespace MyDearImage.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        this.Posts = new HashSet<Post>();
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Post> Posts { get; set; }
}

