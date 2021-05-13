using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.subcriberFanout
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg");


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // aynı exchange i burada da deklere edebiliriz. bir sıkıntı olmaz. ama oluşturduğum için gerek yok. kuyruktaki ayarlar gibi buradaki ayarları değiştirirsek hata verir. mantık queue ile aynı aslında...

            //  channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);


            // buradaki kuyruk ismi random olmasında fayda var. cunsomer olarak bağlanacam ama kuyruğun ismi ne olacak? random olarak vermekte fayda var.
            // çünkü ben üç tane cunsomer instance ı oluşturursam, hepsi tek bir kuyruğa gidebilir. ben de bunu istemiyorum.

            // ben istiyorum ki, üç tane instance oluşturursam, her bir cunsomer instance ının kendisine ait kuyruğu olsun. o yüzden;

            // ben bunu quid olarak oluşturabilirim. ama rabitmq nun içinde random bir kuyruk ismi oluşturma sistemi var. ben onu kullanıyorum. yani

             var randomQueueName = channel.QueueDeclare().QueueName; // random bir kuyruk adı veriyor bana.

           // var randomQueueName = "log-database-save-queue"; // kuyruk kalıcı olsun diye randomdan kaldırdım. 

            channel.QueueDeclare(randomQueueName, true, false, false);

            // ben bunu exchangeime bind edecem.

            channel.QueueBind(randomQueueName, "logs-fanout", "", null); // nulll neydi? tekrar bak.

            // ben işlem bitince kuyruk silinsin istiyorum. o yüzden declare etmek yerine, publisherda ürettiğim exchange e bind ediyorum. ilk olarak benden bir kuyruk ismi istiyor. ikinci olarak, bana exchange adı istiyor. üçüncü olarak ise benden root istiyor. henüz oluşmadığından ben bunu boş bırakıyorum. son kısım da null yine. son olarak da ; basic propert olarak null'ı set ediyoruz.

            // artık uygulama her ayağa kalktığında bu kuyruk işini bitirince silinecek. ama eğer ben declare edersem, uygulama bitince bu kuyruk durur.(burası çokomelli)

            channel.BasicQos(0, 1, false);

  
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName, false, consumer);   // artık üretilen random kuyruğu tüketmek istiyorum.

            Console.WriteLine("Loglar dinleniyor...");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);
                Console.WriteLine("Gelen mesaj :" + message);

                channel.BasicAck(e.DeliveryTag, false); 

            };



            Console.ReadLine();
        }
    }
}
