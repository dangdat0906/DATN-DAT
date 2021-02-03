using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Role_Permissions")]
    public class Role_Permissions
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("RoleId")]
        public long RoleId { get; set; }
        [Column("MenuId")]
        public long MenuId { get; set; }
        [Column("ActionCode")]
        public string ActionCode { get; set; }
        [NotMapped]
        public string MenuUrl { get; set; }
    }
}
