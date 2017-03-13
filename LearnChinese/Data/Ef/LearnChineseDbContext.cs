using System.Data.Entity;

namespace YellowDuck.LearnChinese.Data.Ef
{
    public class LearnChineseDbContext : DbContext
    {
        #region Constructors

        public LearnChineseDbContext()
            : base("name=LearnChineseDbContext")
        {
        }

        #endregion

        #region Properties

        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<UserSharing> UserSharings { get; set; }

        #endregion

        #region Methods


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Word>()
                .Property(e => e.OriginalWord)
                .IsUnicode(true);

            modelBuilder.Entity<Word>()
                .Property(e => e.Pronunciation)
                .IsUnicode(true);

            modelBuilder.Entity<Word>()
                .Property(e => e.Translation)
                .IsUnicode(true);
            
            modelBuilder.Entity<Word>()
                .Property(e => e.Usage)
                .IsUnicode(true);

            modelBuilder.Entity<Word>()
                .HasMany(e => e.Scores)
                .WithRequired(e => e.Word)
                .HasForeignKey(e => e.IdWord)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.Words)
                .WithRequired(e => e.UserOwner)
                .HasForeignKey(e => e.IdOwner)
                .WillCascadeOnDelete(true);


            modelBuilder.Entity<User>()
                .HasMany(e => e.FriendUserSharings)
                .WithRequired(e => e.UserFriend)
                .HasForeignKey(e => e.IdFriend)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.OwnerUserSharings)
                .WithRequired(e => e.UserOwner)
                .HasForeignKey(e => e.IdOwner)
                .WillCascadeOnDelete(true);
        }

        #endregion


    }
}
