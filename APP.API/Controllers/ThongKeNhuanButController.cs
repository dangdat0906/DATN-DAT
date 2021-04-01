using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APP.MANAGER;
using APP.MODELS;
using APP.MODELS.ViewModels;


namespace APP.API.Controllers
{
    [Route("api/thong-ke-nhuan-but")]
    [ApiController]
    public class ThongKeNhuanButController : ControllerBase
    {
        private readonly IThongKeNhuanButManager _thongKeNhuanButManager;
        private readonly IAccountsManager _accountsManager;
        private readonly ITypesManager _typesManager;
        private readonly ITheLoai_HeSoManager _theLoai_HeSoManager;
        private readonly IAuthorManager _authorManager;
        public ThongKeNhuanButController(IAuthorManager authorManager,IThongKeNhuanButManager thongKeNhuanButManager, IAccountsManager accountsManager, ITypesManager typesManager, ITheLoai_HeSoManager theLoai_HeSoManager)
        {
            this._thongKeNhuanButManager = thongKeNhuanButManager;
            this._accountsManager = accountsManager;
            this._typesManager = typesManager;
            this._theLoai_HeSoManager = theLoai_HeSoManager;
            this._authorManager = authorManager;
        }
        //[HttpGet("get-list")]
        //public async Task<List<ThongKeNhuanButViewModel>> Get_List(string month)
        //{
        //    try
        //    {
        //        var listUser = await _thongKeNhuanButManager.Get_List_User();
        //        var listType = await _thongKeNhuanButManager.Get_List_Type();
        //        var listHeSoTheLoai = await _thongKeNhuanButManager.Get_List_Type(month);
        //        var donGia = await _thongKeNhuanButManager.Get_Don_Gia(month);
        //        var result = new List<ThongKeNhuanButViewModel>();
        //        foreach (var user in listUser) //11
        //        {
        //            var listTypeByUser = new List<TypesViewModel>();
        //            foreach (var type in listType) //5
        //            {
        //                var soLuongBaiViet = await _thongKeNhuanButManager.Get_Content_Number_By_Type((byte)type.Id, user.Id, month);
        //                var typeByUser = new TypesViewModel();
        //                typeByUser.Coefficient = (await _theLoai_HeSoManager.Find_By_Type_Id(type.Id, month)) != null ? (await _theLoai_HeSoManager.Find_By_Type_Id(type.Id, month)).Coefficient : 0;
        //                typeByUser.Name = type.Name;
        //                typeByUser.SoLuongBaiViet = soLuongBaiViet;
        //                listTypeByUser.Add(typeByUser);
        //            }
        //            var model = new ThongKeNhuanButViewModel();
        //            model.TenNguoiDang = user.FullName;
        //            model.UserName = user.UserName;
        //            model.LoaiBaiViet = listTypeByUser;
        //            model.Thang = month;
        //            model.UserId = user.Id;
        //            model.TenPhongBan = user.tenDonVi;
        //            model.DonGia = (int)(donGia == null ? 0 : donGia.Value);
        //            result.Add(model);
        //        }
        //        foreach (var item in result)
        //        {
        //            float tongBaiVietHeSo = 0;
        //            foreach (var type in item.LoaiBaiViet)
        //            {
        //                tongBaiVietHeSo += (float)(type.SoLuongBaiViet.Value * type.Coefficient);
        //            }
        //            item.Tongtien = (int)(tongBaiVietHeSo * item.DonGia);
        //            item.NhuanBut = item.Tongtien * 40 / 100;
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        [HttpGet("get-list")]
        public async Task<List<ThongKeNhuanButViewModel>> Get_List(string month)
        {
            try
            {
                //Danh sach tac gia
                //Danh sach loai bai viet + he so
                //Danh sach tong so bai viet trong thang
                //Don gia nhuan but
                var listAuthor = await _thongKeNhuanButManager.Get_List_Author();
                var listType = await _thongKeNhuanButManager.Get_List_Type(month);
                var listContentByMonth = (await _thongKeNhuanButManager.Get_Content_By_Month(month)).ToList();
                var donGia = (await _thongKeNhuanButManager.Get_Don_Gia(month)) == null? 0 : (await _thongKeNhuanButManager.Get_Don_Gia(month)).Value;
                var result = new List<ThongKeNhuanButViewModel>();
                foreach (var i in listAuthor)
                {
                    var listTypeByUser = new List<TheLoai_HeSoViewModel>();
                    foreach(var t in listType)
                    {
                        var soLuongBaiViet = listContentByMonth.Where(c => c.ContentType == t.TypeId && c.AuthorId == i.Id).ToList().Count();
                        var typeByAuthor = new TheLoai_HeSoViewModel()
                        {
                            TypeName = t.TypeName,
                            Coefficient = t.Coefficient,
                            SoLuongBaiViet = soLuongBaiViet
                        };
                        listTypeByUser.Add(typeByAuthor);
                    }
                    var model = new ThongKeNhuanButViewModel()
                    {
                        AuthorName = i.Name,
                        AuthorId = i.Id,
                        LoaiBaiViet = listTypeByUser,
                        DonGia = (long)donGia,
                        Thang = month,
                        TenCoQuan = i.NewsSourcesName,
                    };
                    result.Add(model);
                }
                foreach (var item in result)
                {
                    float tongBaiVietHeSo = 0;
                    foreach (var type in item.LoaiBaiViet)
                    {
                        tongBaiVietHeSo += (float)(type.SoLuongBaiViet.Value * type.Coefficient);
                    }
                    item.Tongtien = (decimal)(tongBaiVietHeSo * item.DonGia);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("detail")]
        public async Task<List<ThongKeNhuanButByAuthor>> Detail(long authorId, string month)
        {
            var author = await _authorManager.Find_By_Id(authorId);
            var listContentByAuthor = await _thongKeNhuanButManager.Get_Content_By_Author_Month(authorId, month);
            var listType = await _thongKeNhuanButManager.Get_List_Type(month);
            var donGia = (await _thongKeNhuanButManager.Get_Don_Gia(month)) == null ? 0 : (await _thongKeNhuanButManager.Get_Don_Gia(month)).Value;
            var listResult = new List<ThongKeNhuanButByAuthor>();
            foreach (var item in listContentByAuthor)
            {
                var loaiBv = listType.Where(c => c.TypeId == Convert.ToInt64(item.ContentType.Value)).Select(c => new TheLoai_HeSo { TypeName = c.TypeName,Coefficient = c.Coefficient }).FirstOrDefault(); 
                ThongKeNhuanButByAuthor model = new ThongKeNhuanButByAuthor()
                {
                    ContentName = item.Title,
                    Dongia = (long)donGia,
                    LoaiBaiViet = loaiBv == null ? "" : loaiBv.TypeName,
                    HeSo = loaiBv == null ? 0 : loaiBv.Coefficient,
                    Thang = month,
                    TacGia = author == null ? "" : author.Name,
                };
                listResult.Add(model);
            }
            foreach (var item in listResult)
            {
                item.Tongtien = (decimal)(item.HeSo * item.Dongia);
            }
            listResult = listResult.OrderBy(c => c.LoaiBaiViet).ToList();
            return listResult;
        }

        [HttpGet("get-list-type")]
        public async Task<List<TheLoai_HeSo>> Get_List_Type(string month)
        {
            try
            {
                return await _thongKeNhuanButManager.Get_List_Type(month);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
