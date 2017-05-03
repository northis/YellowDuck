using System.ComponentModel.DataAnnotations.Schema;
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

        public virtual DbSet<WordFileA> WordFileAs { get; set; }
        public virtual DbSet<WordFileO> WordFileOs { get; set; }
        public virtual DbSet<WordFileP> WordFilePs { get; set; }
        public virtual DbSet<WordFileT> WordFileTs { get; set; }

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

            modelBuilder.Entity<Word>()
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            modelBuilder.Entity<WordFileA>()
                .Property(a => a.CreateDate)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            modelBuilder.Entity<WordFileA>()
                .Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<WordFileO>()
                .Property(a => a.CreateDate)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            modelBuilder.Entity<WordFileO>()
                .Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<WordFileP>()
                .Property(a => a.CreateDate)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            modelBuilder.Entity<WordFileP>()
                .Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<WordFileT>()
                .Property(a => a.CreateDate)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            modelBuilder.Entity<WordFileT>()
                .Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<WordFileA>()
                .HasRequired(a => a.Word)
                .WithRequiredDependent(a => a.WordFileA)
                .WillCascadeOnDelete();

            modelBuilder.Entity<WordFileO>()
                .HasRequired(a => a.Word)
                .WithRequiredDependent(a => a.WordFileO)
                .WillCascadeOnDelete();

            modelBuilder.Entity<WordFileP>()
                .HasRequired(a => a.Word)
                .WithRequiredDependent(a => a.WordFileP)
                .WillCascadeOnDelete();

            modelBuilder.Entity<WordFileT>()
                .HasRequired(a => a.Word)
                .WithRequiredDependent(a => a.WordFileT)
                .WillCascadeOnDelete();

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
