using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Medias")]
    public class Medias
    {
        [Column("Id")]
        [Key]
        public long Id { get; set; }
        [Column("Url")]
        public string Url { get; set; }
        [Column("VideoType")]
        public bool? VideoType { get; set; }
        [Column("Status")]
        public byte? Status { get; set; }
        [Column("Type")]
        public byte? Type { get; set; }
        [Column("Folder")]
        public string Folder { get; set; }
        [Column("Size")]
        public long Size { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
       
        [NotMapped]
        public string DetailAlbumName { get; set; }
    }
}