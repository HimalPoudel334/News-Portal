using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Newsportal.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }

    public DateTime CommentedOn { get; set; } = DateTime.Now;

    public User CommentedBy { get; set; }

    public News News { get; set; }

    public ICollection<Comment> Replies { get; set; }

}