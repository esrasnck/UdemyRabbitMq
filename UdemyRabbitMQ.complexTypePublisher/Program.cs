using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace UdemyRabbitMQ.complexTypePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Topic);

            Dictionary<string, object> headers = new Dictionary<string, object>(); // header dictionary tipinde.
            headers.Add("format1", "pdf");
            headers.Add("shape2", "a4");


            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true; // bu özelliği true ya set edersek, mesajlar kalıcı hale gelir.

            var product = new Product { Id=1, Name="Kalem",Price=100,Stock=10};

            var productJsonString = JsonSerializer.Serialize(product); //serilaze ediyoruz.

            // mesajda büyük datalar göndermek uygun değil. ama göndermek istersek bu şekilde olacak.

            channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

            // kuyruk consumer tarafında oluşcak

            Console.WriteLine("Mesaj gönderilmiştir.");

            Console.ReadLine();
        }
    }
}
