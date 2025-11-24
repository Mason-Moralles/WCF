using System;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace WcfServiceLibrary1
{
    public class SecurityService : ISecurityService
    {
        public string GetPublicInfo()
        {
            return "Это общая информация. Доступна любому авторизованному пользователю.";
        }

        public string GetManagerSecret()
        {
            // Имя аутентифицированного пользователя
            var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;

            // Разрешаем только admin
            if (!string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new FaultException("Access denied: только менеджер (admin) может получить эти данные.");
            }

            return "Секретные данные менеджера: $$$";
        }
    }
}
