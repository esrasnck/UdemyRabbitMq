using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMq.publisher2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg"); // gerçek dünyada bu bilgiler app settinjson da tutuluyor.

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue", true, false, false); // yoksa sıfırdan oluşturur. varsa herhangi bir şey yapmaz. ama farklı configurasyonda olursa hata verir. => rabbitMQ.Client.Exceptions.operationInterruptedException hatası.



            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {

                string message = $"Mesajı{x}";

                // x=> 1'den 50 ye kadar olan sayılar :P


                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
                Console.WriteLine($"Mesaj gönderilmiş.: {message}");

            });
        
            // bunu şimdi subcriberda iki instance üzerinden ayağa kaldırcaz.
         

         
            
            Console.ReadLine();
        }
    }
}
