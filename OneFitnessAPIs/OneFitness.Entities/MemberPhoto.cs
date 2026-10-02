using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    // Kept in its own table (1:1 with MemberRegistration) so member list queries never load image bytes.
    [Table("MemberPhoto")]
    public class MemberPhoto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MemberId { get; set; }

        [Required]
        public byte[] PhotoData { get; set; } = Array.Empty<byte>();

        [Required, MaxLength(50)]
        public string ContentType { get; set; } = "image/jpeg";

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }
    }
}
