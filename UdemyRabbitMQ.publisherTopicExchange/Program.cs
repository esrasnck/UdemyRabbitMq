using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisherTopicExchange
{
    class Program
    {

        public enum LogNames
        {
            Critical = 1, Error = 2, Warning = 3, Info = 4
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            Random rnd = new Random();
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
             
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);


                var routeKey = $"{log1}.{log2}.{log3}"; // hangi rootu hangi rootkey e karşılık gelecek. artık onu belirtmem gerek

                string message = $"log-type :{log1}-{log2}-{log3}";
                var messageBody = Encoding.UTF8.GetBytes(message);

                // benim bir de burada artık bir root oluşturmam gerek. 



                channel.BasicPublish("logs-topic", routeKey, null, messageBody);

                Console.WriteLine($"Log gönderilmiş. : {message}");

                // kuyruğu subcrriberda oluşturcaz.

            });


            Console.ReadLine();
        }
    }
}
