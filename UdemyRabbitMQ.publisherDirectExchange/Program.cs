using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisherDirectExchange
{
    class Program
    {
        public enum LogNames
        {
            Critical=1, Error=2, Warning =3, Info=4 
        }


        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            // her bir log için hem bir route oluşturcam, hem bir kuyruk oluşturcam. onun için;

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}"; // hangi rootu hangi rootkey e karşılık gelecek. artık onu belirtmem gerek
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, true, false, false);
                // exculse'i false olsun ki farklı channellardan da bağlanabiliyim. (3 numara)

                // kuyruğa bağlanıyoruz.
                channel.QueueBind(queueName, "logs-direct", routeKey,null);
                
            });




            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                // her bir döngüde bana random bir log gelsim.
                LogNames log =(LogNames)new Random().Next(1, 5);

                string message = $"log-type :{log}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                // benim bir de burada artık bir root oluşturmam gerek. 

                var routeKey = $"route-{log}"; // bu da rootu

                channel.BasicPublish("logs-direct", routeKey, null, messageBody);
              
                Console.WriteLine($"Log gönderilmiş. : {message}");

            });


            Console.ReadLine();
        }
    }
}
