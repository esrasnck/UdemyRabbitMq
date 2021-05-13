using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace UdemyRabbitMQ.subcriberHeaderExchange
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Topic); // varsa herhangi birşey olmayacak. yoksa oluşacak

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            // bind olmak istediğim kuyruğu yakalıyorum

            var queueName = channel.QueueDeclare().QueueName; // channel üzerinden random bir kuyruk ismi bana geliyor.

            // bind işlemi gerçekleştircez.

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match","all");  // key value çiftlerinin hepsi eşleşmeli.

            //headers.Add("x-match","any"); biri eşleşse yeterli.



            channel.QueueBind(queueName, "header-exchange",String.Empty,headers);     // bind edecez. subrciber düştüğünde kuyruk da düşsün.

            channel.BasicConsume(queueName, false, consumer);   // hata burada. ben bastıramıyorum :(

            Console.WriteLine("Loglar dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

              
                Console.WriteLine("Gelen mesaj :" + message);
            

                channel.BasicAck(e.DeliveryTag, false);

            };

            Console.ReadLine();
        }
    }
}
