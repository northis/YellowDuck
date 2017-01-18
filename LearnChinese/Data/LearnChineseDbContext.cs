using System.Data.Entity;
using YellowDuck.LearnChinese.Data.Ef;

namespace YellowDuck.LearnChinese.Data
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
        }

        #endregion


    }
}
