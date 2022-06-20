using Hafta._4.Domain.Entities;
using Hafta._4.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hafta._4.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole , int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendshipConfirmation> FriendshipConfirmations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageHistory> MessageHistories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostHistory> PostHistories { get; set; }

    }
}