WCF-сервис с аутентификацией по имени пользователя (WS-Security Message Mode) и защищённым доступом по ролям

Проект реализован в рамках КТ6.
Цель — разработать защищённый WCF-сервис и WPF-клиент, демонстрирующий аутентификацию, авторизацию и безопасный доступ к методам.

📌 1. Цель работы

Создать WCF-службу, использующую аутентификацию по имени пользователя (UserName/Password) в режиме WS-Security (Message Security).
Сервис должен:

принимать запросы только от аутентифицированных клиентов;

обеспечивать доступ к определённым методам только определённым пользователям/ролям;

позволять клиентскому приложению WPF вызывать методы и корректно отображать ошибки доступа.

📌 2. Архитектура решения
Состав проекта

WcfServiceLibrary1
Библиотека WCF-контрактов и реализации сервисов.

WcfServiceHost
Консольный self-host WCF (ServiceHost).
Хостит 4 сервиса:

Service1 (КТ2)

EmployeeService (КТ4)

OrderService (КТ5)

SecurityService (КТ6)

SecurityClient (WPF)
Клиентское приложение WPF, вызывающее методы SecurityService.

📌 3. SecurityService — функциональность

Интерфейс:

[ServiceContract]
public interface ISecurityService
{
    [OperationContract]
    string GetPublicInfo();

    [OperationContract]
    string GetManagerSecret();
}


Реализация:

GetPublicInfo() — доступен всем аутентифицированным пользователям.

GetManagerSecret() — доступ только пользователю admin (роль «Manager» по смыслу).

var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;

if (username != "admin")
    throw new FaultException("Access denied: только менеджер может получить эти данные.");

📌 4. Аутентификация (UserName/Password)

В self-host'е используется кастомный валидатор:

public class SimpleUserValidator : UserNamePasswordValidator
{
    public override void Validate(string username, string password)
    {
        if (username == "admin" && password == "123") return;
        if (username == "user"  && password == "123") return;

        throw new SecurityTokenException("Неверный логин или пароль");
    }
}

Поддерживаемые пользователи:
Логин	Пароль	Доступ к GetManagerSecret
admin	123	✔ есть
user	123	❌ нет
📌 5. Сервисная безопасность (WS-Security Message)

SecurityService использует:

wsHttpBinding

security mode="Message"

<message clientCredentialType="UserName"/>

сертификат X.509 для подписи WS-Security сообщений

Пример binding:
<wsHttpBinding>
  <binding name="UsernameBinding">
    <security mode="Message">
      <message clientCredentialType="UserName" />
    </security>
  </binding>
</wsHttpBinding>

📌 6. Сертификат службы

Для WS-Security требуется сервисный сертификат.
Создан самоподписанный сертификат:

Subject: CN=localhost
Thumbprint: 31E4AE7B6BC65B87603202A64FC6BE22164C0B1C


В App.config хоста:

<serviceCertificate
    findValue="31E4AE7B6BC65B87603202A64FC6BE22164C0B1C"
    x509FindType="FindByThumbprint"
    storeLocation="CurrentUser"
    storeName="My" />


Клиент доверяет сертификату через PeerTrust, поэтому сертификат был помещён в:

Certificates → CurrentUser → Trusted People

📌 7. Настройка WPF-клиента

В клиенте используется Service Reference к:

http://localhost:8740/SecurityService/mex


Настроен endpoint:

<endpoint address="http://localhost:8740/SecurityService/"
          binding="wsHttpBinding"
          bindingConfiguration="WSHttpBinding_ISecurityService"
          contract="SecurityRef.ISecurityService"
          behaviorConfiguration="SecurityClientBehavior" />


Клиент доверяет сертификату:

<authentication certificateValidationMode="PeerTrust"
                revocationMode="NoCheck" />

📌 8. Логика WPF-клиента
var client = new SecurityServiceClient();
client.ClientCredentials.UserName.UserName = username;
client.ClientCredentials.UserName.Password = password;

try
{
    var info = client.GetPublicInfo();
    Output("Public: " + info);

    var secret = client.GetManagerSecret();
    Output("Manager: " + secret);
}
catch (FaultException ex)
{
    Output("Fault: " + ex.Message);
}

📌 9. Демонстрация работы
✔ user / 123
Public: Это общая информация...
Fault: Access denied: только менеджер может получить эти данные.

✔ admin / 123
Public: Это общая информация...
Manager: Секретные данные менеджера: $$$

📌 10. Что показано в КТ6

Реализована защищённая WCF-служба.

Использована аутентификация по имени пользователя (UserName).

Использован WS-Security Message Mode.

Настроен кастомный валидатор логин/пароль.

Реализована авторизация (метод доступен только определённому пользователю).

Клиент WPF корректно:

передаёт логин/пароль;

получает данные;

обрабатывает SOAP Fault;

отображает различия в правах пользователей.

📌 11. Вывод

Контрольная точка 6 выполнена полностью:

✔ разработан защищённый WCF-сервис
✔ реализована аутентификация UserName/Password
✔ реализована авторизация по ролям
✔ создан WPF-клиент
✔ корректно настроены WS-Security, сертификаты, binding’и и behaviors

Проект демонстрирует полноценный пример защищённого взаимодействия WCF-службы и клиента.