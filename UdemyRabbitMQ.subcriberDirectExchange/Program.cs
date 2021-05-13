
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.subcriberDirectExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.BasicQos(0,1, false);

            var consumer = new EventingBasicConsumer(channel);

            // bind olmak istediğim kuyruğu yakalıyorum

            var queueName = "direct-queue-Critical";
            //channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);
                Console.WriteLine("Gelen mesaj :" + message);
                // mesajı txt dosyasına yazdırıyoruz.
               // File.AppendAllText("log-critical.txt",message +"\n");

                channel.BasicAck(e.DeliveryTag, false);

            };

            // basic consumer dinlemeye başlıyor. eğer ben bunu yukarı koymuş olsaydım, basicconsumer dinlemeye başladıktan sonra, kuyruk dönen mekanizma devreye girmediği için, mesajlar gelmemiş gibi oluyor. boşa dinlemiş oluyor.
            // önce dinleme mekanizmasını kuruyorum(ekrana bastırma mekanizması) sonrasında abone oluyorum. 
             channel.BasicConsume(queueName, false, consumer);  // önce abone olmam gerekiyor ki sonra mesajları al. diğer türlü recieved da tanımlı olmadığı için gelmiyor mesajlar. kod sırası önemli.


            Console.ReadLine();
        }
    }
}
