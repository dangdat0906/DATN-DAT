using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Content-TheLoai")]
    public class ContentTypes
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("ContentId")]
        public long ContentId { get; set; }
        [Column("TheLoaiId")]
        public long TheLoaiId { get; set; }
    }
}
