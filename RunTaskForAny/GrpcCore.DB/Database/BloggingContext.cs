using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrpcCore.DB.Database
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source = 127.0.0.1;Initial Catalog = Blogging; User ID = sa;Password =123456;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        //    modelBuilder.Entity<Blog>();
        //    modelBuilder.Entity<Post>();
            base.OnModelCreating(modelBuilder);
        }
    }

    [Table(nameof(Blog))]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }
        [MaxLength(500), Required]
        public string Url { get; set; }
        public int Rating { get; set; } = 100;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
        public DateTime CreateDate { get; set; } = DateTime.Now.Date;

        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public List<Post> Posts { get; set; } = new List<Post>();
    }

    [Table(nameof(Post))]
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        [MaxLength(50), Required]
        public string Title { get; set; }
        [MaxLength(0)]
        public string Content { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}