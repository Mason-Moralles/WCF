using System;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    public class SecureService : ISecureService
    {
        public string GetSecureData()
        {
            return "Доступ разрешён: данные получены через защищённый канал WS-Security (Message)";
        }

        public string GetUserName()
        {
            return "Ваш Windows-логин: " + ServiceSecurityContext.Current.PrimaryIdentity.Name;
        }
    }
}
