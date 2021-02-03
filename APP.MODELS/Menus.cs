using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Menus")]
    public class Menus
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Name")] 
        public string Name { get; set; }
        [Column("Url")]      
        public string Url { get; set; }
        [Column("ParentId")]
        public long? ParentId { get; set; }
        [Column("Note")]
        public string Note { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("CreatedBy")]
        public long? CreatedBy { get; set; }
        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("IsMenu")]
        public bool IsMenu { get; set; }
        [Column("LangCode")]
        public string LangCode { get; set; }
        [Column("DisplayOrder")]
        public int? DisplayOrder { get; set; }
        [NotMapped]
        public List<Menus> ListChild { get; set; }
    }

    public class MenuViewModels : Menus
    {
        public string ParentName { get; set; }
        
    }
}
