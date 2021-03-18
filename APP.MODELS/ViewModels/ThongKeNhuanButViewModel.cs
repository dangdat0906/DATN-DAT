using System;
using System.Collections.Generic;
using System.Text;

namespace APP.MODELS.ViewModels
{
    public class ThongKeNhuanButViewModel
    {
        public string AuthorName { get; set; }
        public long AuthorId { get; set; }
        public List<TheLoai_HeSoViewModel> LoaiBaiViet { get; set; }
        public long DonGia { get; set; }
        public decimal Tongtien { get; set; }
        public string Thang { get; set; }
        public string TenCoQuan { get; set; }
        public decimal NhuanBut { get; set; }
        public float TongBaiVietHeSo { get; set; }
    }
    public class ThongKeNhuanButByAuthor
    {
        public string ContentName { get; set; }
        public long Dongia { get; set; }
        public decimal Tongtien { get; set; }
        public string LoaiBaiViet { get; set; }
        public double HeSo { get; set; }
        public string Thang { get; set; }
        public string CoQuan { get; set; }
        public string TacGia { get; set; }
        public decimal NhuanBut { get; set; }
    }

    public class TheLoai_HeSoViewModel : TheLoai_HeSo
    {
        public int? SoLuongBaiViet { get; set; }
    }
}
