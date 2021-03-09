using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Contents")]
    public class Contents
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("Title")]
        public string Title { get; set; }
        [Column("Url")]
        public string Url { get; set; }
        [Column("Summary")]
        public string Summary { get; set; }
        //[Column("LangCode")]
        //public string LangCode { get; set; }
        [Column("Content")]
        public string Content { get; set; }
        //[Column("Source")]
        //public string Source { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }

        [Column("PublishDate")]
        public DateTime? PublishDate { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }
       
        //[Column("Tags")]
        //public string Tags { get; set; }
        [Column("TotalView")]
        public long? TotalView { get; set; }
        [Column("TitleImage")]
        public string TitleImage { get; set; }
        //[Column("TopHot")]
        //public DateTime? TopHot { get; set; }
        [Column("ShowOnTop")]
        public bool? ShowOnTop { get; set; }
        //[Column("ShowOnRightTop")]
        //public bool? ShowOnRightTop { get; set; }
        [Column("ContentType")]
        public byte? ContentType { get; set; }
        //[Column("Reasons")]
        //public string Reasons { get; set; }
        //[Column("RemovedDate")]
        //public DateTime? RemovedDate { get; set; }
        [Column("AuthorId")]
        public long? AuthorId { get; set; }
        public long? NewsSource { get; set; }
        [NotMapped]
        public List<long> GroupID { get; set; }
        [NotMapped]
        public List<long> CategoryId { get; set; }
        [NotMapped]
        public int? TitleImgWidth { get; set; }
        [NotMapped]
        public int? TitleImgHeight { get; set; }
        [NotMapped]
        public Categories CategoryParent { get; set; }
        [NotMapped]
        public Categories Categories { get; set; }
        [NotMapped]
        public bool? PheDuyet { get; set; }
    }
}