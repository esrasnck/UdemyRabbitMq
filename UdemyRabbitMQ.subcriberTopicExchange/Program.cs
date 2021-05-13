using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.subcriberTopicExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            // bind olmak istediğim kuyruğu yakalıyorum

            var queueName = channel.QueueDeclare().QueueName; // channel üzerinden random bir kuyruk ismi bana geliyor.

            var routeKey = "*.Warning.*";

            channel.QueueBind(queueName, "logs-topic", routeKey);     // bind edecez. subrciber düştüğünde kuyruk da düşsün.

           // channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                //Thread.Sleep(1500);
                Console.WriteLine("Gelen mesaj :" + message);
                // mesajı txt dosyasına yazdırıyoruz.
                // File.AppendAllText("log-critical.txt",message +"\n");

                channel.BasicAck(e.DeliveryTag, false);

            };


            channel.BasicConsume(queueName,false,consumer);   // hata burada. ben bastıramıyorum :(


            Console.ReadLine();
        }
    }
}
