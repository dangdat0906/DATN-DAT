using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace APP.WEBSITE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                     "document",
                     "hethongvanban",
                    new { controller = "Document", action = "Index" });
                endpoints.MapControllerRoute(
                     "search",
                     "search/{search}",
                    new { controller = "Search", action = "Index" });
                endpoints.MapControllerRoute(
                     "rss",
                     "rss",
                    new { controller = "Categories", action = "Rss" });
                endpoints.MapControllerRoute(
                    "rss",
                    "rss/feed/{categoryId}",
                   new { controller = "Categories", action = "GetRss" });
                endpoints.MapControllerRoute(
                    "question",
                    "cau-hoi-doc-gia",
                   new { controller = "KhaoSat", action = "QuestionCustomer" });
                endpoints.MapControllerRoute(
                     "khaosat",
                     "khaosat",
                    new { controller = "KhaoSat", action = "Index" });
                endpoints.MapControllerRoute(
                     "document",
                     "hethongvanban-{id}",
                    new { controller = "Document", action = "Detail" });
                endpoints.MapControllerRoute(
                     "content",
                     "{url}.html",
                    new { controller = "ContentDetail", action = "Index" });
                endpoints.MapControllerRoute(
                     "tags",
                     "tag-{id}",
                    new { controller = "Categories", action = "GetContentByTag" });
                endpoints.MapControllerRoute(
                     "category",
                     "{categoryUrl}",
                    new { controller = "Categories", action = "Index" });
                endpoints.MapControllerRoute(
                     "thuvienanh",
                     "thuvienanh",
                    new { controller = "ImageLibrary", action = "Index" });
                endpoints.MapControllerRoute(
                     "thuvienanh",
                     "thuvienanh/album{albumId}",
                    new { controller = "ImageLibrary", action = "DetailAlbum" });
                endpoints.MapControllerRoute(
                     "thuvienvideo",
                     "thuvienvideo",
                    new { controller = "VideoLibrary", action = "Index" });
                endpoints.MapControllerRoute(
                    "thuvienvideo",
                    "thuvienvideo/album{albumId}",
                   new { controller = "VideoLibrary", action = "DetailAlbum" });
                endpoints.MapControllerRoute(
                    "sodocong",
                    "so-do-trang",
                   new { controller = "Categories", action = "SiteMap" });
            });

        }
    }
}
