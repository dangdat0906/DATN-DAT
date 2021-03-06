using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.MODELS
{
    [Table("Authors")]
    public class Authors
    {
        [Key]
        public long Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("NewsSourcesId")]
        public long NewsSourcesId { get; set; }
        [Column("Note")]
        public string Note { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public string NewsSourcesName { get; set; }
    }
}
