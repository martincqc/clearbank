using System;
using System.Collections.Generic;
using System.Text;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public interface IBackupAccountDataStore
    {
        Account GetAccount(string accountNumber);
        void UpdateAccount(IAccount account);
    }
}
