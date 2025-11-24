Контрольная точка 7
Пользовательский канал и пользовательский кодер сообщений в WCF (Custom Binding + TCP)

Цель КТ7 — освоить низкоуровневые механизмы WCF путём разработки кастомного транспорта (TCP) и кастомного message encoder-а, которые совместно формируют пользовательскую binding.
В рамках задания реализована служба WCF и клиент, обменивающиеся SOAP-сообщениями через полностью кастомный стек.

📌 1. Цель работы

Разработать WCF-службу, использующую кастомный канал и кастомный message encoder.

Реализовать транспорт на основе TCP.

Реализовать собственный encoder, логирующий SOAP-сообщения при чтении и записи.

Создать собственную Binding, которая объединяет encoder и транспорт.

Разработать клиентское приложение, взаимодействующее с сервисом через эту binding.

Все пункты ТЗ выполнены.

📌 2. Архитектура решения
Проекты в solution:

WcfServiceLibrary1
Содержит:

Интерфейс сервиса и реализацию (ICustomChannelService, CustomChannelService).

Все классы КТ7:

CustomMessageEncoder

CustomMessageEncoderFactory

CustomMessageEncodingBindingElement

CustomTcpBinding

WcfServiceHost
Self-host, поднимающий сервис по адресу:

net.tcp://localhost:9001/CustomChannelService


и использующий CustomTcpBinding.

CustomChannelClient
Консольный клиент, создающий канал через CustomTcpBinding и выполняющий вызов Echo.

📌 3. Реализованные компоненты (КТ7)
✔ 3.1. Контракт сервиса
[ServiceContract]
public interface ICustomChannelService
{
    [OperationContract]
    string Echo(string message);
}

✔ 3.2. Реализация сервиса
public class CustomChannelService : ICustomChannelService
{
    public string Echo(string message)
    {
        Console.WriteLine($"[Service] Получено сообщение: {message}");
        return $"Echo from custom TCP+encoder: {message}";
    }
}

📌 4. Пользовательский message encoder
✔ CustomMessageEncoder

Расширяет стандартный TextMessageEncoder и логирует:

входящие SOAP-сообщения (ReadMessage)

исходящие SOAP-сообщения (WriteMessage)

Таким образом демонстрируется вмешательство в процесс кодирования.

Пример фрагмента логирования:

[Encoder] WriteMessage (buffer):
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
    ...
</s:Envelope>

✔ CustomMessageEncoderFactory

Фабрика encoder'ов, обязательная часть WCF pipeline.
Позволяет привязке создавать экземпляры CustomMessageEncoder.

✔ CustomMessageEncodingBindingElement

BindingElement, подключающий encoder в стек binding.

📌 5. Пользовательский транспорт (TCP)
✔ CustomTcpBinding

Формирует binding:

[CustomMessageEncodingBindingElement]
[TcpTransportBindingElement (стандартный)]


Таким образом стек сообщений выглядит так:

CustomMessageEncoder → SOAP → TCP Transport → Сеть


Данный вариант закрывает требование работы поверх TCP.

📌 6. Self-host: регистрация сервиса

В WcfServiceHost:

var customHost = new ServiceHost(
    typeof(CustomChannelService),
    new Uri("net.tcp://localhost:9001/CustomChannelService"));

customHost.AddServiceEndpoint(
    typeof(ICustomChannelService),
    new CustomTcpBinding(),
    "");

📌 7. Клиентское приложение (CustomChannelClient)

Клиент напрямую использует CustomTcpBinding без App.config:

var binding = new CustomTcpBinding();
var address = new EndpointAddress("net.tcp://localhost:9001/CustomChannelService");

var factory = new ChannelFactory<ICustomChannelService>(binding, address);
var client = factory.CreateChannel();


Пример консольного диалога:

> Hello KT7
Ответ сервиса: Echo from custom TCP+encoder: Hello KT7

📌 8. Доказательство работы encoder'а

В консоли хоста выводятся:

SOAP-сообщения входящего вызова

SOAP-сообщения ответа

Пример:

[Encoder] ReadMessage (buffer):
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
    ...
</s:Envelope>

[Service] Получено сообщение: Hello KT7


Это демонстрирует корректную работу кастомного message encoder'а.
