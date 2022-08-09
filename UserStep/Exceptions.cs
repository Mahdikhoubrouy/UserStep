using System;
using System.Collections.Generic;
using System.Text;

namespace UserStep
{

    public class UserNotFound : Exception
    {
        public UserNotFound(string message) : base(message)
        {
        }

    }
}
