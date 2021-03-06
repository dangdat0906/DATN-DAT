using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APP.MODELS
{
    [Table("Groups")]
    public class Groups
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Column("Note")]
        [MaxLength(250)]
        public string Note { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }

    }
}
