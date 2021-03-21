using DepsWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DepsWebApp.Services
{
    public class DatabaseService : IDbService
    {
        // TODO: can chance on concurent dictionary and get rid of semaphores, login will be a key

        private readonly DepsWebAppContext _dbContext;

        //private readonly SemaphoreSlim _semaphore1 = new SemaphoreSlim(1, 1);
        //private readonly SemaphoreSlim _semaphore2 = new SemaphoreSlim(1, 1);

        public DatabaseService(DepsWebAppContext contex)
        {
            _dbContext = contex;
        }

        public async Task<bool> Add(Account newAccount)
        {
            if (newAccount == null)
            {
                throw new ArgumentNullException(nameof(newAccount));
            }

            try
            {
                _dbContext.Accounts.Add(newAccount);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch 
            {
                return false;
            }

            //await _semaphore1.WaitAsync();
            //try
            //{
            //    var isExist = await Find(newAccount);

            //    if (isExist)
            //    {
            //        return false;
            //    }

            //    _accounts.Add(newAccount);
            //    return true;
            //}
            //finally
            //{
            //    _semaphore1.Release();
            //}
        }

        public async Task<bool> Find(Account newAccount)
        {
            var account = await _dbContext.Accounts
                .FirstOrDefaultAsync(account => account.LoginId == newAccount.LoginId && account.Password == newAccount.Password);

            return account is { }
                ? true
                : false;
        }
    }
}
