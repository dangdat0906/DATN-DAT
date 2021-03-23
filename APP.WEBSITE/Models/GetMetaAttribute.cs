using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using APP.MODELS;
using APP.UTILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APP.WEBSITE.Models
{
    public class GetMetaAttribute : Controller
    {
        private readonly IConfiguration _config1;
        private readonly string _domain1;
        public GetMetaAttribute(IConfiguration config1)
        {
            this._config1 = config1;
            this._domain1 = _config1["APIDomain"].ToString(); 
        }
        public void GetMeta()
        {
            var data =  HttpHelper.GetData<Contacts>($"{_domain1}/api/contact/get-by-statuswebsite", "", "false").GetAwaiter().GetResult();
            ViewBag.Contacts = data;
            
        }

    }
}




