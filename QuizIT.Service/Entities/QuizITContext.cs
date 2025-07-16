using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QuizIT.Service.Entities
{
    public partial class QuizITContext : DbContext
    {
        public QuizITContext()
        {
        }

        public QuizITContext(DbContextOptions<QuizITContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<ExamDetail> ExamDetail { get; set; }
        public virtual DbSet<History> History { get; set; }
        public virtual DbSet<HistoryDetail> HistoryDetail { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Rank> Rank { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer("workstation id=quiz111.mssql.somee.com;packet size=4096;user id=luongnl21_SQLLogin_1;pwd=krkanz3x8h;data source=quiz111.mssql.somee.com;persist security info=False;initial catalog=quiz111;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamName).HasMaxLength(255);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Exam)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Exam__CategoryId__3C69FB99");

                entity.HasOne(d => d.CreateByNavigation)
                    .WithMany(p => p.Exam)
                    .HasForeignKey(d => d.CreateBy)
                    .HasConstraintName("FK__Exam__CreateBy__3E52440B");
            });

            modelBuilder.Entity<ExamDetail>(entity =>
            {
                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamDetail)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExamDetai__ExamI__534D60F1");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ExamDetail)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExamDetai__Quest__5441852A");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.History)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__History__ExamId__4E88ABD4");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.History)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__History__UserId__4F7CD00D");
            });

            modelBuilder.Entity<HistoryDetail>(entity =>
            {
                entity.Property(e => e.AnswerSelect)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasOne(d => d.History)
                    .WithMany(p => p.HistoryDetail)
                    .HasForeignKey(d => d.HistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HistoryEx__Histo__5EBF139D");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.HistoryDetail)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HistoryEx__Quest__5FB337D6");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.AnswerA).IsRequired();

                entity.Property(e => e.AnswerB).IsRequired();

                entity.Property(e => e.AnswerC).IsRequired();

                entity.Property(e => e.AnswerCorrect)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AnswerD).IsRequired();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Question__Catego__49C3F6B7");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Question__Create__4BAC3F29");
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.Rank)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rank__ExamId__571DF1D5");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Rank)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rank__UserId__5812160E");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__RoleId__29572725");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
