using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    public class Content_Categories
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }
        [Column("ContentId")]
        public long ContentId { get; set; }
        [Column("CategoryId")]
        public long CategoryId { get; set; }

    }
}
