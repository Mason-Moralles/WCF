using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    public class CustomChannelService : ICustomChannelService
    {
        public string Echo(string message)
        {
            Console.WriteLine($"[Service] Получено сообщение: {message}");
            return $"Echo from custom TCP+encoder: {message}";
        }
    }
}
