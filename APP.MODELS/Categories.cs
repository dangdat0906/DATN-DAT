using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{

    [Table("Categories")]
    public class Categories
    {
        [Key]
        //[Required]
        [Column("Id")]
        public long Id { get; set; }
        //[Required]
        [Column("Name")]
        //[MaxLength(50)]
        public string Name { get; set; }
        //[Required]
        [Column("Code")]
        //[MaxLength(250)]
        public string Code { get; set; }
        [Column("Note")]
        //[MaxLength(250)]
        public string Note { get; set; }
        [Column("ParentId")]
        public long? ParentId { get; set; }
        //[Required]
        //[MaxLength(50)]
        //[Required]
        [Column("Status")]
        public byte? Status { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("MenuDisplay")]
        //[MaxLength(250)]
        public string MenuDisplay { get; set; }
        [Column("GroupDisplay")]
        //[MaxLength(250)]
        public string GroupDisplay { get; set; }
        [Column("IsMenu")]
        public bool? IsMenu { get; set; }
        [Column("DisplayOrder")]
        public int DisplayOrder { get; set; }
        [Column("OnHome")]
        public bool? OnHome { get; set; }
        [Column("DisplayOnHomeType")]
        public byte? DisplayOnHomeType { get; set; }
        public byte? ListContentType { get; set; }
        [NotMapped]
        //public List<ConfigCategories> Config { get; set; }
        //[NotMapped]
        public List<Categories> ListChild { get; set; }
        [NotMapped]
        public List<Contents> Contents { get; set; }
        
    }
    public class CategoriesViewModel : Categories
    {
        public string ParentName { get; set; }
        public Categories ParentCategory { get; set; }
        public List<Contents> ListContents { get; set; }
        public long ContentCategoryCount { get; set; }
    }
}
