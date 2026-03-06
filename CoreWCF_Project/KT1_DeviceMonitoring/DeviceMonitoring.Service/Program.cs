using CoreWCF;
using CoreWCF.Configuration;
using DeviceMonitoring.Contracts;
using DeviceMonitoring.Service;

var builder = WebApplication.CreateBuilder(args);

// Kestrel слушает HTTP на 5000 (из appsettings.json)
// TCP-транспорт CoreWCF — отдельный механизм, НЕ Kestrel
builder.WebHost.UseNetTcp(8090);

// Регистрируем CoreWCF
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Регистрируем DeviceService как singleton (единое хранилище данных)
builder.Services.AddSingleton<DeviceService>();

var app = builder.Build();

// Настройка CoreWCF эндпоинтов
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<DeviceService>();

    // HTTP endpoint (BasicHttpBinding) — через Kestrel на порту 5000
    var httpBinding = new BasicHttpBinding(CoreWCF.Channels.BasicHttpSecurityMode.None);
    serviceBuilder.AddServiceEndpoint<DeviceService, IDeviceManager>(
        httpBinding,
        "/DeviceService/basic");

    // TCP endpoint (NetTcpBinding) — через UseNetTcp(8090) выше
    var tcpBinding = new NetTcpBinding(SecurityMode.None);
    serviceBuilder.AddServiceEndpoint<DeviceService, IDeviceManager>(
        tcpBinding,
        new Uri("net.tcp://localhost:8090/DeviceService/nettcp"));

    // Метаданные (WSDL)
    var metadata = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
    metadata.HttpGetEnabled = true;
});

// Health-check endpoint
app.MapGet("/health", () =>
    "DeviceMonitoring Service is running.\n" +
    "HTTP SOAP : http://localhost:5000/DeviceService/basic\n" +
    "TCP  SOAP : net.tcp://localhost:8090/DeviceService/nettcp\n" +
    "WSDL      : http://localhost:5000/DeviceService/basic?wsdl");

app.Run();
