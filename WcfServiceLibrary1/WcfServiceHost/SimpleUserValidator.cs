using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace WcfServiceHost
{
    public class SimpleUserValidator : UserNamePasswordValidator
    {
        public override void Validate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new SecurityTokenException("Имя пользователя или пароль пусты");

            if (username == "admin" && password == "123")
                return;

            if (username == "user" && password == "123")
                return;

            throw new SecurityTokenException("Неверный логин или пароль");
        }
    }
}
