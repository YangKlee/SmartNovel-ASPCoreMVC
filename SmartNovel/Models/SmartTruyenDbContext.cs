using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartNovel.Models;

public partial class SmartTruyenDbContext : DbContext
{
    public SmartTruyenDbContext()
    {
    }

    public SmartTruyenDbContext(DbContextOptions<SmartTruyenDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<HistoryReader> HistoryReaders { get; set; }

    public virtual DbSet<MenuNav> MenuNavs { get; set; }

    public virtual DbSet<Novel> Novels { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<RecommendNovel> RecommendNovels { get; set; }

    public virtual DbSet<ReportTicket> ReportTickets { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasMany(d => d.Uids).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "BlockCategory",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlockCategory_User"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_BlockCategory_Category"),
                    j =>
                    {
                        j.HasKey("CategoryId", "Uid");
                        j.ToTable("BlockCategory");
                        j.IndexerProperty<string>("CategoryId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("CategoryID");
                        j.IndexerProperty<string>("Uid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("UID");
                    });
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.ToTable("Chapter");

            entity.Property(e => e.ChapterId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ChapterID");
            entity.Property(e => e.AllowComment).HasDefaultValue(true);
            entity.Property(e => e.ChapterFileUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ChapterTitle).HasMaxLength(255);
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NovelId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NovelID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SummaryChapter).HasMaxLength(1000);
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Novel).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.NovelId)
                .HasConstraintName("FK_Chapter_Novel");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.Property(e => e.CommentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("CommentID");
            entity.Property(e => e.ChapterId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ChapterID");
            entity.Property(e => e.ParentCommentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ParentCommentID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TimeCommeny)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Uid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UID");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_Comment_Chapter");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comment_Parent");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<HistoryReader>(entity =>
        {
            entity.HasKey(e => e.ReadSessionId);

            entity.ToTable("HistoryReader");

            entity.Property(e => e.ReadSessionId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ReadSessionID");
            entity.Property(e => e.ChapterId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ChapterID");
            entity.Property(e => e.NovelId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NovelID");
            entity.Property(e => e.TimeReader)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Uid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UID");

            entity.HasOne(d => d.Chapter).WithMany(p => p.HistoryReaders)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_HistoryReader_Chapter");

            entity.HasOne(d => d.Novel).WithMany(p => p.HistoryReaders)
                .HasForeignKey(d => d.NovelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistoryReader_Novel");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.HistoryReaders)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistoryReader_User");
        });

        modelBuilder.Entity<MenuNav>(entity =>
        {
            entity.ToTable("MenuNav");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.IconBootstrap)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.RoleId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.Slots).HasDefaultValue(0);
            entity.Property(e => e.UrlLink)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_MenuNav_Parent");

            entity.HasOne(d => d.Role).WithMany(p => p.MenuNavs)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_MenuNav_Role");
        });

        modelBuilder.Entity<Novel>(entity =>
        {
            entity.ToTable("Novel");

            entity.Property(e => e.NovelId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NovelID");
            entity.Property(e => e.AgeRating)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageBanerNovelUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageNovelUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.LikeCount).HasDefaultValue(0);
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Uid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UID");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.NovelsNavigation)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Novel_User");

            entity.HasMany(d => d.Categories).WithMany(p => p.Novels)
                .UsingEntity<Dictionary<string, object>>(
                    "NovelCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_NovelCategory_Category"),
                    l => l.HasOne<Novel>().WithMany()
                        .HasForeignKey("NovelId")
                        .HasConstraintName("FK_NovelCategory_Novel"),
                    j =>
                    {
                        j.HasKey("NovelId", "CategoryId");
                        j.ToTable("NovelCategory");
                        j.IndexerProperty<string>("NovelId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("NovelID");
                        j.IndexerProperty<string>("CategoryId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("CategoryID");
                    });

            entity.HasMany(d => d.Uids).WithMany(p => p.Novels)
                .UsingEntity<Dictionary<string, object>>(
                    "FollowNovel",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FollowNovel_User"),
                    l => l.HasOne<Novel>().WithMany()
                        .HasForeignKey("NovelId")
                        .HasConstraintName("FK_FollowNovel_Novel"),
                    j =>
                    {
                        j.HasKey("NovelId", "Uid");
                        j.ToTable("FollowNovel");
                        j.IndexerProperty<string>("NovelId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("NovelID");
                        j.IndexerProperty<string>("Uid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("UID");
                    });
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => new { e.Uid, e.NovelId });

            entity.ToTable("Rating");

            entity.Property(e => e.Uid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UID");
            entity.Property(e => e.NovelId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NovelID");

            entity.HasOne(d => d.Novel).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.NovelId)
                .HasConstraintName("FK_Rating_Novel");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rating_User");
        });

        modelBuilder.Entity<RecommendNovel>(entity =>
        {
            entity.HasKey(e => e.RecommendId);

            entity.ToTable("RecommendNovel");

            entity.Property(e => e.RecommendId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("RecommendID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.NovelId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NovelID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Uid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UID");

            entity.HasOne(d => d.Novel).WithMany(p => p.RecommendNovels)
                .HasForeignKey(d => d.NovelId)
                .HasConstraintName("FK_RecommendNovel_Novel");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.RecommendNovels)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecommendNovel_User");
        });

        modelBuilder.Entity<ReportTicket>(entity =>
        {
            entity.HasKey(e => e.TiketId);

            entity.ToTable("ReportTicket");

            entity.Property(e => e.TiketId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("TiketID");
            entity.Property(e => e.ChapterId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ChapterID");
            entity.Property(e => e.CommentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("CommentID");
            entity.Property(e => e.NovelId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("NovelID");
            entity.Property(e => e.RepoterUid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("RepoterUID");
            entity.Property(e => e.ResolvedUid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ResolvedUID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TargetUid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("TargetUID");
            entity.Property(e => e.TimeSend)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Chapter).WithMany(p => p.ReportTickets)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_ReportTicket_Chapter");

            entity.HasOne(d => d.Comment).WithMany(p => p.ReportTickets)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_ReportTicket_Comment");

            entity.HasOne(d => d.Novel).WithMany(p => p.ReportTickets)
                .HasForeignKey(d => d.NovelId)
                .HasConstraintName("FK_ReportTicket_Novel");

            entity.HasOne(d => d.RepoterU).WithMany(p => p.ReportTicketRepoterUs)
                .HasForeignKey(d => d.RepoterUid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReportTicket_RepoterUser");

            entity.HasOne(d => d.ResolvedU).WithMany(p => p.ReportTicketResolvedUs)
                .HasForeignKey(d => d.ResolvedUid)
                .HasConstraintName("FK_ReportTicket_ResolvedUser");

            entity.HasOne(d => d.TargetU).WithMany(p => p.ReportTicketTargetUs)
                .HasForeignKey(d => d.TargetUid)
                .HasConstraintName("FK_ReportTicket_TargetUser");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleDescription).HasMaxLength(1000);
            entity.Property(e => e.RoleDisplayName).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uid);

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E4A687A77E").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__User__5C7E359EB4EF3AA8").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534D11FF680").IsUnique();

            entity.Property(e => e.Uid)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UID");
            entity.Property(e => e.AvartarUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.BannedTime).HasColumnType("datetime");
            entity.Property(e => e.CreatorPoint).HasDefaultValue(0);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RoleId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TimeOutTime).HasColumnType("datetime");
            entity.Property(e => e.TimeOutType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");

            entity.HasMany(d => d.Authors).WithMany(p => p.Uids)
                .UsingEntity<Dictionary<string, object>>(
                    "BlockAuthor",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlockAuthor_Target"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlockAuthor_User"),
                    j =>
                    {
                        j.HasKey("AuthorId", "Uid");
                        j.ToTable("BlockAuthor");
                        j.IndexerProperty<string>("AuthorId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("AuthorID");
                        j.IndexerProperty<string>("Uid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("UID");
                    });

            entity.HasMany(d => d.FollowerUs).WithMany(p => p.UidsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "FollowAuthor",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("FollowerUid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FollowAuthor_Target"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FollowAuthor_User"),
                    j =>
                    {
                        j.HasKey("FollowerUid", "Uid");
                        j.ToTable("FollowAuthor");
                        j.IndexerProperty<string>("FollowerUid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("FollowerUID");
                        j.IndexerProperty<string>("Uid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("UID");
                    });

            entity.HasMany(d => d.Uids).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "BlockAuthor",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlockAuthor_User"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlockAuthor_Target"),
                    j =>
                    {
                        j.HasKey("AuthorId", "Uid");
                        j.ToTable("BlockAuthor");
                        j.IndexerProperty<string>("AuthorId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("AuthorID");
                        j.IndexerProperty<string>("Uid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("UID");
                    });

            entity.HasMany(d => d.UidsNavigation).WithMany(p => p.FollowerUs)
                .UsingEntity<Dictionary<string, object>>(
                    "FollowAuthor",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FollowAuthor_User"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("FollowerUid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FollowAuthor_Target"),
                    j =>
                    {
                        j.HasKey("FollowerUid", "Uid");
                        j.ToTable("FollowAuthor");
                        j.IndexerProperty<string>("FollowerUid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("FollowerUID");
                        j.IndexerProperty<string>("Uid")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .HasColumnName("UID");
                    });
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
