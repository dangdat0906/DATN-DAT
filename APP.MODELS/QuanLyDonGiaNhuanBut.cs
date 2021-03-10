using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Unit_Price")]
    public class QuanLyDonGiaNhuanBut
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Value")]
        public decimal Value { get; set; }
        [Column("FromMonth")]
        public DateTime FromMonth { get; set; }
        [Column("ToMonth")]
        public DateTime ToMonth { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }

    }
}
