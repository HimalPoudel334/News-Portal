using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }

    public DateTime CommentedOn { get; set; } = DateTime.Now;

    public virtual User CommentedBy { get; set; }

    public virtual News News { get; set; }
    
    [ForeignKey("Comment")] 
    public int? CommentId { get; set; }
    
    public virtual ICollection<Comment> Replies { get; set; }

}