using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeUni.Data.Entities
{
    public class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public byte[] RowVersion { get; set; }
    }
}
