﻿using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace APP.UTILS
{
    public class HttpHelper
    {

        private static IHttpContextAccessor httpContextAccessor;
        public static void SetHttpContextAccessor(IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
        }


        public async static Task<T> PostData<T>(T model, string url, string checktoken = "true")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                   /* httpClient.DefaultRequestHeaders.Add("checktoken", checktoken);

                    var account = SessionExtensions.Get<Accounts>(httpContextAccessor.HttpContext.Session, SessionExtensions.SessionAccount);
                    if (account != null)
                    {
                        httpClient.DefaultRequestHeaders.Add("token", account.Token);
                    }*/
                    string body = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

                    using (var response = httpClient.PostAsync(url, content))
                    {
                        //if (response.Result.IsSuccessStatusCode)
                        //{
                        string apiResponse = await response.Result.Content.ReadAsStringAsync();

                        if (!response.Result.IsSuccessStatusCode)
                        {
                            if((int)response.Result.StatusCode == (int)StatusCodes.Status401Unauthorized)
                            {
                                // httpContextAccessor.HttpContext.Response.Redirect($"/Error/Error401");
                                httpContextAccessor.HttpContext.Response.Redirect("/Error/Error401",true);
                                await httpContextAccessor.HttpContext.Response.WriteAsync($"<script> location.href = '/Error/Error401';</script>");
                                throw new Exception("401");
                            }
                            else
                            {

                                throw new Exception(response.Result.ReasonPhrase);
                            }
                        }
                        else
                        {
                            var data = JsonConvert.DeserializeObject<T>(apiResponse);
                            return data;
                        }

                        //var data = JsonConvert.DeserializeObject<ResponseModel>(apiResponse);
                        //{
                        //    ResponseMessage message = JsonConvert.DeserializeObject<ResponseMessage>(data.Data);
                        //    throw new Exception(message.Message);
                        //}
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }
            return default(T);
        }
        public static async Task<T> GetData<T>(string url, string request = "", string checktoken = "false")
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                   /* httpClient.DefaultRequestHeaders.Add("checktoken", checktoken);

                    var account = SessionExtensions.Get<Accounts>(httpContextAccessor.HttpContext.Session, SessionExtensions.SessionAccount);
                    if (account != null)
                    {
                        httpClient.DefaultRequestHeaders.Add("token", account.Token);
                    }*/
                    StringContent content = new StringContent(request, Encoding.UTF8, "application/json");
                    using (var response = await httpClient.GetAsync(string.Format("{0}?{1}", url, request)))
                    {
                        //if(response.StatusCode == (int) StatusCodes.Status401Unauthorized)
                        //{

                        //}
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var data = JsonConvert.DeserializeObject<T>(apiResponse);
                            return data;
                        }
                        else
                        {
                            if ((int)response.StatusCode == (int)StatusCodes.Status401Unauthorized)
                            {
                                await httpContextAccessor.HttpContext.Response.WriteAsync($"<script> location.href = '/Error/Error401';</script>");
                                //httpContextAccessor.HttpContext.Response.Redirect("/Error/Error401",true);
                                throw new Exception("401");
                            }
                            else
                            {

                                throw new Exception(response.ReasonPhrase);
                            }
                        }
                        //if (data.Result == "200")
                        //{
                        //    return JsonConvert.DeserializeObject<T>(data.Data);
                        //}
                        //else
                        //{
                        //    ResponseMessage message = JsonConvert.DeserializeObject<ResponseMessage>(data.Data);
                        //    throw new Exception(message.Message);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class ResponseMessage
    {
        public string Message { get; set; }
    }
    public class ResponseModel
    {
        public string Result { get; set; }
        public string Data { get; set; }
    }
    partial class Accounts
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public DateTime? ExpiredToken { get; set; }

        public byte Status { get; set; }

        public int? CountLoginFail { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? LastChangePass { get; set; }

        public bool? IsFirstLogin { get; set; }
        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public string NewPass { get; set; }

        public List<long> ListRoleId { get; set; }
    }
}
