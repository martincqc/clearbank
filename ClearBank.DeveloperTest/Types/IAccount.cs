using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Types
{
    public interface IAccount
    {
        string AccountNumber { get; set; }
        decimal Balance { get; set; }
        AccountStatus Status { get; set; }
        AllowedPaymentSchemes AllowedPaymentSchemes { get; set; }
    }
}
