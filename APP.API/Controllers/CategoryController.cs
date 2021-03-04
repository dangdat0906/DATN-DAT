using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APP.MANAGER;
using APP.MODELS;
using APP.UTILS;

namespace Portal.API.Controllers
{
    [ApiController]
    [Route("api/chuyen-muc")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryManager _categoryManager;
        //private readonly IConfigCategoryManager _configManager;
        public CategoryController(ICategoryManager categoryManager/*, IConfigCategoryManager configManager*/)
        {
            this._categoryManager = categoryManager;
            //this._configManager = configManager;
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(string name = "", long parentId = 0, int status = -1, string langcode="",int pageSize = 10, int pageNumber = 0)
        {
            try
            {

                var data = await _categoryManager.GetList(name, parentId, status,langcode,pageSize, pageNumber);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-parent")]
        public async Task<IActionResult> GetListParent(string langCode = "VIE")
        {
            try
            {

                var data = await _categoryManager.GetListParent(langCode);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Categories inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên chuyên mục {MessageConst.NOT_EMPTY_INPUT}");
                }
                var exist = await _categoryManager.FindByCode(inputModel.Code);
                if (exist != null){
                        throw new Exception($"Mã(Code) chuyên mục {MessageConst.EXIST}");
                }
                //var exist = await _categoryManager.FindByName(inputModel.Name.ToLower().Trim());
                //if (exist != null)
                //{
                //    throw new Exception($"Tên chuyên mục {MessageConst.EXIST}");
                //}
                //var dataDisplay = await _categoryManager.CheckDisplayOrder(inputModel.DisplayOrder);
                //if (dataDisplay != null)
                //{
                //    throw new Exception("Thứ tự hiển thị đã dùng cho chuyên mục mục khác");
                //}
                inputModel.MenuDisplay = inputModel.MenuDisplay.Trim().ToUpper();
                inputModel.GroupDisplay = inputModel.GroupDisplay.Trim().ToUpper();
                inputModel.ListContentType = inputModel.ListContentType == null ? 1 : inputModel.ListContentType;
                inputModel.CreatedDate = DateTime.Now;
                var result = await _categoryManager.Create(inputModel);
              
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-category-menu-published")]
        public async Task<IActionResult> Get_List_Category_Published(string langCode = "VIE")
        {
            try
            {
                var data = await _categoryManager.GetListChild(langCode);
                List<Categories> result = new List<Categories>();
                foreach (var item in data)
                {
                    if (item.IsMenu == true && item.Status == (int)StatusEnum.Active && item.LangCode.Equals(langCode))
                    {
                        result.Add(item);
                    }
                }
                result = result.OrderBy(c => c.DisplayOrder).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-category-onhome-published")]
        public async Task<IActionResult> Get_List_Category_Onhome_Published()
        {
            try
            {
                var data = await _categoryManager.GetListChild();
                List<Categories> result = new List<Categories>();
                foreach (var item in data)
                {
                    if (item.OnHome == true && item.Status == (int)StatusEnum.Active && item.ParentId == 0)
                    {
                        result.Add(item);
                    }
                }
                result = result.OrderBy(c => c.DisplayOrder).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-category-onhome-english")]
        public async Task<IActionResult> Get_List_Category_Onhome_English(int take = 2)
        {
            try
            {
                var data = (await _categoryManager.GetListHomeEng()).Take(take).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //private async void CreateConfig(List<ConfigCategories> inputModel, long categoryid)
        //{
        //    await _configManager.Delete(categoryid);
        //    await _configManager.CreateList(inputModel);
        //}
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Categories inputModel)
        {
            try
            {
                if (string.IsNullOrEmpty(inputModel.Name))
                {
                    throw new Exception($"Tên chuyên mục {MessageConst.NOT_EMPTY_INPUT}");
                }
                var data = await _categoryManager.FindByIdUpdate(inputModel.Id);
                if (data == null)
                {
                    throw new Exception(MessageConst.DATA_NOT_FOUND);
                }
                //var dataDisplay = await _categoryManager.CheckDisplayOrder(inputModel.DisplayOrder);
                //if (dataDisplay != null && dataDisplay.Id != inputModel.Id)
                //{
                //    throw new Exception($"Thứ tự hiển thị đã dùng chuyên mục khác");
                //}
                inputModel.Code = inputModel.Code.Trim().Replace(" ", "");
                inputModel.CreatedDate = data.CreatedDate;
                inputModel.UpdatedDate = DateTime.Now;
                await _categoryManager.Update(inputModel);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("look-up")]
        public async Task<IActionResult> LookUp(string langCode = "VIE")
        {
            try
            {
                var data = await _categoryManager.LookUp(langCode);
                return Ok(data.Select(x => new { Value = x.Id, Title = x.Name }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-list-child")]
        public async Task<IActionResult> Get_List_Child(string langCode = "VIE")
        {
            try
            {
                var data = await _categoryManager.GetListChild(langCode);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-id")]
        public async Task<IActionResult> FindById(long id)
        {
            try
            {
                var data = await _categoryManager.FindByIdUpdate(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("find-by-code")]
        public async Task<IActionResult> FindByCode(string code)
        {
            try
            {
                var data = await _categoryManager.FindByCode(code);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] Categories inputModel)
        {
            try
            {
                var data = await _categoryManager.FindByIdUpdate(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                data.Status = inputModel.Status;
                await  _categoryManager.Update(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] Categories inputModel)
        {
            try
            {
                var data = await _categoryManager.FindByIdUpdate(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _categoryManager.Delete(inputModel.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("delete-not-update")]
        public async Task<IActionResult> DeleteNotUpDate([FromBody] Categories inputModel)
        {
            try
            {
                var data = await _categoryManager.FindByIdUpdate(inputModel.Id);
                if (data == null)
                {
                    throw new Exception($"{MessageConst.DATA_NOT_FOUND}");
                }
                await _categoryManager.Delete(inputModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("get-statistical")]
        public async Task<IActionResult> Get_statistical(string langcode)
        {
            try
            {
                var data = await _categoryManager.GetStatistical(langcode);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("thong-ke-bai-viet-theo-danh-muc")]
        public async Task<IActionResult> GetCategoryContent(string tuNgay = "", string denNgay = "")
        {
            try
            {
                var data = await _categoryManager.CategoryContentTotal(tuNgay,denNgay);
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
