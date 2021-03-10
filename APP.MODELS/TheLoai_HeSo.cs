using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Coefficient")]
    public class TheLoai_HeSo
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("TypeId")]
        public long? TypeId { get; set; }
        [Column("Coefficient")]
        public double Coefficient { get; set; }
        [Column("FromDate")]
        public DateTime FromDate { get; set; }
        [Column("ToDate")]
        public DateTime ToDate { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }
        [NotMapped]
        public string TypeName { get; set; }
    }

}
