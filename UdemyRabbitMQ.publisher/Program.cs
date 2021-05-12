using RabbitMQ.Client;
using System;
using System.Text;

namespace UdemyRabbitMQ.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://evrdybgg:TQLBLVPPb7_OR1qBR3hajFa7ncOrB_HM@fish.rmq.cloudamqp.com/evrdybgg"); // gerçek dünyada bu bilgiler app settinjson da tutuluyor.

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); // kanal oluştu.

            channel.QueueDeclare("hello-queue", true, false, false); // rabbitMq da kuyruk oluşturuyorum.

            //1) queue adı 2) durable false yapılırsa, burada oluşan kuyruklar memoryde tutulur. rabbitmq restart atarsa memoryde atılacağı için tüm kuyruk gider. eğer true olursa, kuyruklar fiziksel olarak kaydedilir ve restart atsanızda kaybedilmez.
            //3) excluse=> true yaparsak, buradaki kuyruğa sadece burada oluşturmuş olduğum kanal üzerinden bağlanabilrim. ama ben bunu subcriber üzerinden farklı bir kanaldan ulaşmak istiyorum. o yüzden bunu false 'a çekmem gerek. kuyruğa gönderdiğin mesajı farklı kanallardan bağlanmak istediğim için. 
            // 4) autodelete=> eğer bu kuyruğa bağlı olan son subcriber bağlantısını kopartırsa, kuyruğu otomatikman siler. ben ama bu kuyruğun hep durmasını istiyorum. o yüzden false set edeceğim. ( en son kalan subcriber gittiğinde de kuyruk dursun istiyorum)

            string message = "hello world";

            //* rabbitMQ'ya mesajlar byte dizisi olarak gider. bu yüzden string ifadeyi convert etmek gerek

            var messageBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            // ilk olarak exchange koymadığımızdan, string empty diyoruz. eğer bunu kullanmazsak, direkt kuyruğa gönderirsek,  default exchange kullanırız. eğer default exchange kullanıyorsanız, buradaki route key'e kuyruğunuzdaki ismi vermeniz  gerekiyor.
            // basic propert olarak null'ı set ediyoruz. sonra da messageBody i yazıyoruz.

            Console.WriteLine("Mesaj gönderilmiş.");
            Console.ReadLine();


        }
    }
}
