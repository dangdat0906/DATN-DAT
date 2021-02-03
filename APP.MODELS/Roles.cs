using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Note")]
        public String Note { get; set; }
        [Column("Status")]
        public byte Status { get; set; }
        [Column("CreatedDate")]      
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("CreatedBy")]
        public long? CreatedBy { get; set; }
        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }
        [NotMapped]
        public List<Role_Permissions> Role_Permissions { get; set; }

    }
}
