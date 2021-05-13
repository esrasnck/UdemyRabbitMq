using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisherFanout
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg"); 

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            // artık kuyruğu sildik. onun yerine exchange oluşturcaz.
            channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout); // tek bunu ekledik

                //1) exchange ismi. 2) fiziksel olarak kaydedilsin. uygulama restart attığında, bu exchange kaybolmasın. false'a set edersem, uygulama restart ettiğinde tüm exchangeler kaybolur. 3) exchange'in tipi. 


            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {

                string message = $"log{x}";

               


                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-fanout","", null, messageBody);
                //1) exchange ismi, 2) herhangi bir filtreleme olmadığından boş veriyoruz. 3) basic propert olarak null'ı set ediyoruz. sonra da messageBody i yazıyoruz.
                Console.WriteLine($"Mesaj gönderilmiş.: {message}");

            });

            




            Console.ReadLine();
        }
    }
}
