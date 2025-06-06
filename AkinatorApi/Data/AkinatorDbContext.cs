using Microsoft.EntityFrameworkCore;
using AkinatorApi.Models;

namespace AkinatorApi.Data
{
    public class AkinatorDbContext : DbContext
    {
        public AkinatorDbContext(DbContextOptions<AkinatorDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<SessionAnswer> SessionAnswers { get; set; }
        public DbSet<QuestionInfo> Questions { get; set; }  // ������ DbSet ��� ��������
        public DbSet<MatchCountLog> MatchCountLogs { get; set; } = null!; // �������� ��� ������

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id).HasName("pk_users");
                entity.Property(e => e.Id).HasColumnName("user_id");
                entity.Property(e => e.Username).HasColumnName("username").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.ToTable("sessions");
                entity.HasKey(e => e.Id).HasName("pk_sessions");
                entity.Property(e => e.Id).HasColumnName("session_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.StartedAt).HasColumnName("started_at");
                entity.Property(e => e.EndedAt).HasColumnName("ended_at");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.GameSessions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("fk_sessions_users");
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.ToTable("characters");
                entity.HasKey(e => e.Id).HasName("pk_characters");
                entity.Property(e => e.Id).HasColumnName("character_id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.AddedByUserId).HasColumnName("added_by_user_id");
                entity.Property(e => e.AddedAt).HasColumnName("created_at");

                entity.HasOne(e => e.AddedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.AddedByUserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("fk_characters_users");
            });

            modelBuilder.Entity<SessionAnswer>(entity =>
            {
                entity.ToTable("session_answers");
                entity.HasKey(e => e.Id).HasName("pk_session_answers");
                entity.Property(e => e.Id).HasColumnName("answer_id");
                entity.Property(e => e.GameSessionId).HasColumnName("session_id"); // ���� � ������� ������� ���������� session_id
                entity.Property(e => e.QuestionId).HasColumnName("question_id");
                entity.Property(e => e.Answer).HasColumnName("answer");
                entity.Property(e => e.AddedByUserId).HasColumnName("added_by_user_id");
                entity.Property(e => e.AddedAt).HasColumnName("added_at");

                entity.HasOne(e => e.GameSession)
                      .WithMany(s => s.SessionAnswers)
                      .HasForeignKey(e => e.GameSessionId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("fk_session_answers_sessions");

                entity.HasOne(e => e.AddedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.AddedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QuestionInfo>(entity =>
            {
                entity.ToTable("questions");
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Id).HasColumnName("question_id");
                entity.Property(q => q.Text).HasColumnName("text").IsRequired();
                entity.Property(q => q.AddedByUserId).HasColumnName("added_by_user_id");
                entity.Property(q => q.AddedAt).HasColumnName("added_at");

                entity.HasOne(q => q.AddedByUser)
                      .WithMany()
                      .HasForeignKey(q => q.AddedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MatchCountLog>(entity =>
            {
                entity.ToTable("match_count_logs");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).HasColumnName("id");
                entity.Property(m => m.UserId).HasColumnName("user_id");
                entity.Property(m => m.RequestedAt).HasColumnName("requested_at");
                entity.Property(m => m.AnswersJson).HasColumnName("answers_json");
                entity.Property(m => m.MatchesCount).HasColumnName("matches_count");

                entity.HasOne(m => m.User)
                      .WithMany()
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
