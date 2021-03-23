using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Contact")]
    public class Contacts
    {
        [Key]
        public long Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Phone")]
        public string Phone { get; set; }
        [Column("Fax")]
        public string Fax { get; set; }
        [Column("Adress")]
        public string Adress { get; set; }
        [Column("TitleFooter")]
        public string TitleFooter { get; set; }
        [Column("Description")]
        public string Description { get; set; }
        [Column("Url")]
        public string Url { get; set; }
        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("TitleWeb")]
        public string TitleWeb { get; set; }
        [Column("Banner")]
        public string Banner { get; set; }
        [Column("BaseSalary")]
        public decimal? BaseSalary { get; set; }
        [Column("UnitPrice")]
        public decimal? UnitPrice { get; set; }
        [Column("Meta")]
        public string Meta { get; set; }
        [Column("Logo")]
        public string  Logo { get; set; }
        [Column("StatusWebsite")]
        public bool StatusWebsite { get; set; }
        [Column("Editor")]
        public string Editor { get; set; }
        [Column("LangCode")]
        public string LangCode { get; set; }
        [Column("Online")]
        public long Online { get; set; }
        [Column("OnlineTotal")]
        public long OnlineTotal { get; set; }
        [Column("OnlineOnMonth")]
        public long OnlineOnMonth { get; set; }
        [Column("Day")]
        public int? Day { get; set; }
        [Column("Month")]
        public int? Month { get; set; }
        [Column("ContentFooter")] 
        public string ContentFooter { get; set; }
        public string MetaDescription { get; set; }
        public string MetaAuthor { get; set; }
        public string MetaLanguage { get; set; }
        public string MetaAbstract { get; set; }
    }
}
