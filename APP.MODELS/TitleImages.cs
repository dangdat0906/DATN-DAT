using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("TitleImages")]
    public class TitleImages
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("ContentId")]
        public long ContentId { get; set; }
        [Column("Url")]
        public string Url { get; set; }
        [Column("Width")]
        public int? Width { get; set; }
        [Column("Height")]
        public int? Height { get; set; }
    }
}
