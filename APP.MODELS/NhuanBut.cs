using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("NhuanBut")]
    public class NhuanBut
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("ContentName")]
        public string ContentName { get; set; }
        [Column("ContentType")]
        public string ContentType { get; set; }
        [Column("Coefficient")]
        public double? Coefficient { get; set; }
        [Column("BaseSalary")]
        public decimal? BaseSalary { get; set; }
        [Column("UserId")]
        public long? UserId { get; set; }
        [Column("ApprovedDate")]
        public DateTime? ApprovedDate { get; set; }
    }
}
