using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class WrongPasswordException : BaseException
    {
        public WrongPasswordException()
            :base("Wrong Password")
        {

        }
    }
}
