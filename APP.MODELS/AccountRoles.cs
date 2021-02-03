using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("AccountRoles")]
    public class AccountRoles
    {
        [Key]
        [Column("Id")]
        public long Id { set; get; }
        [Column("AccountId")]
        public long AccountId { get; set; }
        [Column("RoleId")]
        public long RoleId { get; set; }
    }
}