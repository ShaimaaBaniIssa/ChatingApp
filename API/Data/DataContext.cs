﻿using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserLike>() // represent the primary key
        .HasKey(k => new { k.SourceUserId, k.TargetUserId });
        modelBuilder.Entity<UserLike>()
        .HasOne(s => s.SourceUser)
        .WithMany(t => t.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
        .HasOne(s => s.TargetUser)
        .WithMany(t => t.LikedByUsers)
        .HasForeignKey(s => s.TargetUserId)
        .OnDelete(DeleteBehavior.NoAction);
        ////////////////////////////////////////////////////////////////////////////


        modelBuilder.Entity<Message>()
        .HasOne(s => s.Sender)
        .WithMany(m => m.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
        .HasOne(r => r.Recipient)
        .WithMany(m => m.MessagesReceived)
        .OnDelete(DeleteBehavior.NoAction);
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }




}
