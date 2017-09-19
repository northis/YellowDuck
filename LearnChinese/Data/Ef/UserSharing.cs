using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace YellowDuck.LearnChinese.Data.Ef
{
    [Table("UserSharing")]
    public class UserSharing
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserSharing()
        {
        }

        public long Id { get; set; }

        public long IdOwner { get; set; }
        public long IdFriend { get; set; }


        public User UserFriend { get; set; }


        public User UserOwner { get; set; }
    }
}