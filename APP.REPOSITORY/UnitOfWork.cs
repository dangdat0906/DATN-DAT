﻿using APP.MODELS;
using Microsoft.EntityFrameworkCore.Storage;
using Portal.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APP.REPOSITORY
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository UserRepository { get; }
        Task CreateTransaction();
        Task Commit();
        Task Rollback();
        Task SaveChange();
        public IMenuRepository MenuRepository { get; }
        public IRolesRepository RolesRepository { get; }
        public IRole_PermissionsRepository Role_PermissionsRepository { get; }
        public IAccountsRepository AccountsRepository { get; }
        public IAccountRolesRepository AccountRolesRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IContent_CategoriesRepository Content_CategoriesRepository { get; }
        public IContentsRepository ContentsRepository { get; }
        public IAuthorsRepository AuthorsRepository { get; }
        public IGroupsRepository GroupsRepository { get; }
        public INewsSourcesRepository NewsSourcesRepository { get; }
        public ITypesRepository TypesRepository { get; }
        public IContentTypesRepository ContentTypesRepository { get; }
        public IContent_GroupsRepository Content_GroupsRepository { get; }
        public ITitleImagesRepository TitleImagesRepository { get; }
        public IMediasRepository MediasRepository { get; }
        public IQuanLyDonGiaNhuanButRepository QuanLyDonGiaNhuanButRepository { get; }
        public ITheLoai_HeSoRepository TheLoai_HeSoRepository { get; }
        public INhuanButRepository NhuanButRepository { get; }
        public IContactRepository ContactRepository { get; }
    }
    public class UnitOfWork : IUnitOfWork
    {
        APPDbContext _dbContext;
        IDbContextTransaction _transaction;
        
        public UnitOfWork(IDbContextFactory<APPDbContext> dbContextFactory, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContextFactory.GetContext();
            UserRepository = new UserRepository(_dbContext);
            MenuRepository = new MenuRepository(_dbContext);
            RolesRepository = new RolesRepository(_dbContext);
            Role_PermissionsRepository = new Role_PermissionsRepository(_dbContext);
            AccountsRepository = new AccountsRepository(_dbContext);
            AccountRolesRepository = new AccountRolesRepository(_dbContext);
            CategoryRepository = new CategoryRepository(_dbContext);
            Content_CategoriesRepository = new Content_CategoriesRepository(_dbContext);
            AuthorsRepository = new AuthorsRepository(_dbContext);
            GroupsRepository = new GroupsRepository(_dbContext);
            NewsSourcesRepository = new NewsSourcesRepository(_dbContext);
            TypesRepository = new TypesRepository(_dbContext);
            ContentTypesRepository = new ContentTypesRepository(_dbContext);
            Content_GroupsRepository = new Content_GroupsRepository(_dbContext);
            TitleImagesRepository = new TitleImagesRepository(_dbContext);
            MediasRepository = new MediasRepository(_dbContext);
            ContentsRepository = new ContentsRepository(_dbContext);
            QuanLyDonGiaNhuanButRepository = new QuanLyDonGiaNhuanButRepository(_dbContext);
            TheLoai_HeSoRepository = new TheLoai_HeSoRepository(_dbContext);
            NhuanButRepository = new NhuanButRepository(_dbContext);
            ContactRepository = new ContactRepository(_dbContext);
        }
        #region Transaction
        public async Task CreateTransaction()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public async Task SaveChange()
        {
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        private bool disposedValue = false; // To detect redundant calls
        public IUserRepository UserRepository { get; }
        public IMenuRepository MenuRepository { get; }
        public IRolesRepository RolesRepository { get; }
        public IRole_PermissionsRepository Role_PermissionsRepository { get; }
        public IAccountsRepository AccountsRepository { get; }
        public IAccountRolesRepository AccountRolesRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IContent_CategoriesRepository Content_CategoriesRepository { get; }
        public IContentsRepository ContentsRepository { get; }
        public IAuthorsRepository AuthorsRepository { get; }
        public IGroupsRepository GroupsRepository { get; }
        public INewsSourcesRepository NewsSourcesRepository { get; }
        public ITypesRepository TypesRepository { get; }
        public IContentTypesRepository ContentTypesRepository { get; }
        public IContent_GroupsRepository Content_GroupsRepository { get; }
        public ITitleImagesRepository TitleImagesRepository { get; }
        public IMediasRepository MediasRepository { get; }
        public IQuanLyDonGiaNhuanButRepository QuanLyDonGiaNhuanButRepository { get; }
        public ITheLoai_HeSoRepository TheLoai_HeSoRepository { get; }
        public INhuanButRepository NhuanButRepository { get; }
        public IContactRepository ContactRepository { get; }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
