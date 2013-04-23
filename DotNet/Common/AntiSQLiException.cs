using System;
using System.Collections.Generic;
using System.Text;

namespace IronCloud.AntiSQLi.Common
{
    public class AntiSQLiException : Exception
    {
        public AntiSQLiException(String Message)
            : base(Message)
        {

        }
    }
}
