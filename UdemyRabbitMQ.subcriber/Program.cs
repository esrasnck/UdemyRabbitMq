using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace UdemyRabbitMQ.subcriber
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg"); // gerçek dünyada bu bilgiler app settinjson da tutuluyor.

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); // kanal oluştu.

            //channel.QueueDeclare("hello-queue", true, false, false); // rabbitMq da kuyruk oluşturuyorum. (ne işe yaradığı pıublisherda)
            // publisherda olduğundan emin olduğumuzdan, yorum satırına aldık.

            // okuma işlemi artık buradan sonra başlıyor.
            // istersek bunu channel.QueueDeclare silebiliriz. ya da kullanabiliriz. Eğer bunu silersek, biz subcribe'ı ayağa kaldırdığımızda, "hello-queue" isimli kuyruk yoksa, hata alırız. eğer silmezsek, publisher bu kuyruğu oluşturmazsa, subcriber bu kuyruğu oluşturur ve uygulamada herhangi bir hata almayız. Eğer publisher, "hello-queue" bu kuyruğu oluşturduğundan eminsek, bunu silebiliriz. 

            // kısacası bu kuyruk oluşturma işlemi, publisherda da yapılarabilir. subcriberda da oluşabilir. iki taraftada varsa, hata fırlatmaz. publisherda yoksa eğer, bu kuyruğu yeniden oluşturur. (olması güzel birşeymiş)

            // burada dikkat edilmesi gereken şey ise şu: eğer iki taraftada da bir kuyruk oluşuyorsa, içerisindeki parametrelerin aynı olmasına dikkat edilmesi gerekir. yada bir yerde false iken diğer tarafta true ise, yine hata verir.

            var consumer = new EventingBasicConsumer(channel); // benden model istiyor. hemen oluşturduğum kanalı veriyorum.

            channel.BasicConsume("hello-queue",true,consumer); // consumer hangi kuyruğu dinleyecek? burada belirtiyoruz. 2) olarak (önemli) autoAck isminde bir property var. bu property şu işe yarıyor. eğer ben bunu true verirsem, subcriber'a bir mesaj gönderdiğinde, bu mesaj doğru da işlerse yanlış da işlerse bu mesajı siler. eğer ben bunu false yaparsam, ben diyorum ki; sen bu kuyruğu silme. eğer ben gelen mesajı doğru bir şekilde işlersem, o zaman sen bunu silersin diyorum. (gerçek dünyada bu genelde false yapılır)

            // artık event üzerinden dinleyebilirim..

            consumer.Received += (object sender, BasicDeliverEventArgs e) => // bu event rabbitMq subcriber'a mesaj gönderdiğinde fırlıyor.
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());  // gelen bind'ı array' üzerinden alıp, stringe çeviriyoruz. (byte array üzerinden göndermiştik)

                Console.WriteLine("Gelen mesaj :" + message);

                // kuyrukta iki mesaj var. ben iki mesaj bekliyorum :)
            
            };

            Console.ReadLine();

        }

        //private static void (object sender, BasicDeliverEventArgs e)(object sender, BasicDeliverEventArgs e)  // metodu bu şekilde de yazabilirim.
        //{
        //    throw new NotImplementedException();
        //}
    }
}
