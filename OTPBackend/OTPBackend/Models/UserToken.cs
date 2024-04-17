using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OTPBackend.Models
{
    public class UserToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiredAt { get; set;} = default!;
    }
}
