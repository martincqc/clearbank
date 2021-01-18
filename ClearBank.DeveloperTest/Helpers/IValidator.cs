using System;
using System.Collections.Generic;
using System.Text;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Helpers
{
    public interface IValidator
    {
        bool Validate(IAccount account, MakePaymentRequest request);
    }
}
