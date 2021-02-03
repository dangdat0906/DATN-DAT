using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace APP.MODELS
{
    [Table("Accounts")]
    public class Accounts
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("UserName")]
        public string UserName { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Column("FullName")]
        public string FullName { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Token")]
        public string Token { get; set; }
        [Column("ExpiredToken")]
        public DateTime? ExpiredToken { get; set; }
        [Column("Status")]
        public byte Status { get; set; }
        [Column("CountLoginFail")]
        public int? CountLoginFail { get; set; }
        [Column("LastLogin")]
        public DateTime? LastLogin { get; set; }
        [Column("LastChangePass")]
        public DateTime? LastChangePass { get; set; }
        [Column("IsFirstLogin")]
        public bool? IsFirstLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("CreatedBy")]
        public long? CreatedBy { get; set; }
        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }
        public long? UnitsId { get; set; }
        public string PhoneNumber { get; set; }
        public long? PostionId { get; set; }
        [NotMapped]
        public string NewPass { get; set; }
        [NotMapped]
        public List<long> ListRoleId { get; set; }
        [NotMapped]
        public List<Role_Permissions> Role_Permissions { get; set; }
        [NotMapped]
        public List<Menus> ListMenu { get; set; }
        //[NotMapped]
        //public List<Contents> ListContent { get; set; }
        [NotMapped]
        public string tenDonVi { get; set; }
        [NotMapped]
        public bool? RememberPass { get; set; }
        [NotMapped]
        public string captcharesponse { get; set; }

    }

}
