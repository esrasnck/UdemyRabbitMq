using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisherHeaderExchange
{
    class Program
    {

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            Dictionary<string, object> headers = new Dictionary<string, object>(); // header dictionary tipinde.
            headers.Add("format", "pdf");
            headers.Add("shape2", "a4");
         

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true; // bu özelliği true ya set edersek, mesajlar kalıcı hale gelir.

            // Basic property oluşturuyoruz. header'ı da buna gönderiyoruz.


            channel.BasicPublish("header-exchange", string.Empty,properties,Encoding.UTF8.GetBytes("header mesajim"));

            // kuyruk consumer tarafında oluşcak

            Console.WriteLine("Mesaj gönderilmiştir.");

            Console.ReadLine();
        }
    }
}
