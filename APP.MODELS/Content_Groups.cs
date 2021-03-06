using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    public class Content_Groups
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }
        [Column("ContentId")]
        public long ContentId { get; set; }
        [Column("GroupId")]
        public long GroupId { get; set; }

    }
}
