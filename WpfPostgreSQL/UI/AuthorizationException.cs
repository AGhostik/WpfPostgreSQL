using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPostgreSQL.UI
{
    class AuthorizationException : Exception
    {
        public AuthorizationException()
        {
            Message = "Введите все поля для авторизации";
        }
        public AuthorizationException(string message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}
