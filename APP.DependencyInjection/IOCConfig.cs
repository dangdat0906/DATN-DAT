using APP.MANAGER;
using APP.MODELS;
using APP.REPOSITORY;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portal.Data;
using System;
using System.Collections.Generic;
using System.Text;


namespace APP.DependencyInjection
{
    public class IOCConfig
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Db
            services.AddDbContext<APPDbContext>(ServiceLifetime.Scoped, ServiceLifetime.Singleton);

            services.AddTransient<IDbContextFactory<APPDbContext>, APPDbContextFactory>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IMenuManager, MenuManager>();
            services.AddTransient<IAccountsManager, AccountsManager>();
            services.AddTransient<IRolesManger, RolesManager>();
            services.AddTransient<IRole_PermissionsManager, Role_PermissionsManager>();
            services.AddTransient<IAccountRolesManager, AccountRolesManager>();
            services.AddTransient<ICategoryManager, CategoryManager>();
            services.AddTransient<IAuthorManager, AuthorManager>();
            services.AddTransient<IGroupsManager, GroupsManager>();
            services.AddTransient<INewsSourcesManager, NewsSourcesManager>();
            services.AddTransient<ITypesManager, TypesManager>();
            services.AddTransient<IContentsManager, ContentsManager>();
            services.AddTransient<IMediasManager, MediasManager>();
            services.AddTransient<IContent_GroupsManager, Content_GroupsManager>();
            services.AddTransient<IContent_CategoriesManager, Content_CategoriesManager>();
            services.AddTransient<IContentTypesManager, ContentTypesManager>();
            services.AddTransient<ITitleImagesManager, TitleImagesManager>();
            services.AddTransient<IQuanLyDonGiaNhuanButManager, QuanLyDonGiaNhuanButManager>();
            services.AddTransient<ITheLoai_HeSoManager, TheLoai_HeSoManager>();
            services.AddTransient<IThongKeNhuanButManager, ThongKeNhuanButManager>();
        }
    }
}
