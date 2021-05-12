using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace UdemyRabbitMq.subciber2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg"); 


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // rabitmq dan mesajları kaçar kaçar aldığımı belirtmek için;   channel.BasicQos();
            channel.BasicQos(0,1,false);
            // 1) bana herhangi bir boyuttaki mesajı gönderebilirsin. 2) mesajlar kaç kaç gelsin 3) değer global olsun mU? false verirsem, her bir subcriber a verdiğim sayı kadar mesaj gönderir.(ör: 5er 5er) eğer true yaparsam, kaç tane subcriber varsa, tek bir seferde tum subcriberların toplam değeri 5 olarak gönderir. yani iki subcriber'ım varsa, tek bir seferde 3-2 diye gönderir. yani toplamdaki sayıyı gönderir.


            var consumer = new EventingBasicConsumer(channel); 

            channel.BasicConsume("hello-queue", false, consumer);  // false değerine set ediyorum. hemen bunu silme diyorum

            consumer.Received += (object sender, BasicDeliverEventArgs e) => 
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);
                Console.WriteLine("Gelen mesaj :" + message);

                channel.BasicAck(e.DeliveryTag,false); // bana ulaştırılan tag'ı artık silebilirisin diyorum. bu tag rabbitmq ya gönderiyorum. o da ilgili taga ait mesajı alıp kuyruktan siliyor. arkasından sildikten sonra 2) multiple tagı var. eğer ben buna true dersem, o anda memoryde işlenmiş ama rabitmq'ya gitmemiş başka mesajlarda varsa onun bilgilerini rabitmq ya haber eder. ben bunu şimdilik false veriyorum. bunun anlamı ise, her bir bilgi ile beraber, ilgili mesajın durumunu rabitmq ya bildir diyorum. eğer ben bunu haberdar etmezsem, rabitmq da subcribe edilmeyen mesajları başka bir consumera gönderir.

            };

           

            Console.ReadLine();
        }
    }
}
