using System;
using System.Collections.Generic;
using System.Text;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Helpers
{
    public static class AccountHelper
    {
        public static IAccount GetAccount(string dataStoreType, string accountNumber, IBackupAccountDataStore backupAccountDataStore, IAccountDataStore accountDataStore)
        {
            try
            {
                return dataStoreType == "Backup" ? backupAccountDataStore.GetAccount(accountNumber) : accountDataStore.GetAccount(accountNumber);
            }
            catch (Exception e)
            {
                //log exception
                return null;
            }
        }

        public static bool UpdateAccount(string dataStoreType, Decimal amount, IAccount account, IBackupAccountDataStore backupAccountDataStore, IAccountDataStore accountDataStore)
        {
            try
            {
                account.Balance -= amount;
                if (dataStoreType == "Backup")
                {
                    backupAccountDataStore.UpdateAccount(account);
                }
                else
                {
                    accountDataStore.UpdateAccount(account);
                }

                return true;
            }
            catch (Exception e)
            {
                //rollback
                account.Balance += amount;
                //log exception
                return false;
            }
        }
    }

}
