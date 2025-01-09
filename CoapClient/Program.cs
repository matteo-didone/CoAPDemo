using CoAP;

namespace MyCoapClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nCoAP Client - Seleziona un'operazione:");
                Console.WriteLine("1. Resource Discovery (GET /.well-known/core)");
                Console.WriteLine("2. GET /hello");
                Console.WriteLine("3. GET /sensor");
                Console.WriteLine("4. POST /sensor");
                Console.WriteLine("5. PUT /sensor");
                Console.WriteLine("6. DELETE /sensor");
                Console.WriteLine("7. Monitor /sensor (polling)");
                Console.WriteLine("0. Esci");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DiscoverResources();
                        break;
                    case "2":
                        GetHello();
                        break;
                    case "3":
                        GetSensor();
                        break;
                    case "4":
                        PostSensor();
                        break;
                    case "5":
                        PutSensor();
                        break;
                    case "6":
                        DeleteSensor();
                        break;
                    case "7":
                        MonitorSensor();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Scelta non valida!");
                        break;
                }
            }
        }

        static void DiscoverResources()
        {
            var request = new Request(Method.GET);
            request.URI = new Uri("coap://localhost/.well-known/core");
            request.Send();

            var response = request.WaitForResponse();
            if (response != null)
            {
                Console.WriteLine($"Risorse disponibili: {response.PayloadString}");
            }
            else
            {
                Console.WriteLine("Nessuna risposta ricevuta");
            }
        }

        static void GetHello()
        {
            var request = new Request(Method.GET);
            request.URI = new Uri("coap://localhost/hello");
            request.Send();

            var response = request.WaitForResponse();
            if (response != null)
            {
                Console.WriteLine($"Risposta: {response.PayloadString}");
            }
            else
            {
                Console.WriteLine("Nessuna risposta ricevuta");
            }
        }

        static void GetSensor()
        {
            var request = new Request(Method.GET);
            request.URI = new Uri("coap://localhost/sensor");
            request.Send();

            var response = request.WaitForResponse();
            if (response != null)
            {
                Console.WriteLine($"Valore sensore: {response.PayloadString}");
            }
            else
            {
                Console.WriteLine("Nessuna risposta ricevuta");
            }
        }

        static void PostSensor()
        {
            Console.Write("Inserisci il nuovo valore: ");
            var value = Console.ReadLine();

            if (!string.IsNullOrEmpty(value))
            {
                var request = new Request(Method.POST);
                request.URI = new Uri("coap://localhost/sensor");
                request.SetPayload(value);
                request.Send();

                var response = request.WaitForResponse();
                if (response != null)
                {
                    Console.WriteLine($"Risposta: {response.PayloadString}");
                }
                else
                {
                    Console.WriteLine("Nessuna risposta ricevuta");
                }
            }
            else
            {
                Console.WriteLine("Valore non valido");
            }
        }

        static void PutSensor()
        {
            Console.Write("Inserisci il valore da impostare: ");
            var value = Console.ReadLine();

            if (!string.IsNullOrEmpty(value))
            {
                var request = new Request(Method.PUT);
                request.URI = new Uri("coap://localhost/sensor");
                request.SetPayload(value);
                request.Send();

                var response = request.WaitForResponse();
                if (response != null)
                {
                    Console.WriteLine($"Risposta: {response.PayloadString}");
                }
                else
                {
                    Console.WriteLine("Nessuna risposta ricevuta");
                }
            }
            else
            {
                Console.WriteLine("Valore non valido");
            }
        }

        static void DeleteSensor()
        {
            var request = new Request(Method.DELETE);
            request.URI = new Uri("coap://localhost/sensor");
            request.Send();

            var response = request.WaitForResponse();
            if (response != null)
            {
                Console.WriteLine("Sensore resettato");
            }
            else
            {
                Console.WriteLine("Nessuna risposta ricevuta");
            }
        }

        static void MonitorSensor()
        {
            Console.WriteLine("Monitoraggio del sensore avviato (premi un tasto per terminare)...");
            bool monitoring = true;

            // Avvia un task separato per il polling
            Task.Run(async () =>
            {
                while (monitoring)
                {
                    var request = new Request(Method.GET);
                    request.URI = new Uri("coap://localhost/sensor");
                    request.Send();

                    var response = request.WaitForResponse();
                    if (response != null)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Valore sensore: {response.PayloadString}");
                    }

                    await Task.Delay(2000); // Attendi 2 secondi prima del prossimo polling
                }
            });

            Console.ReadKey();
            monitoring = false;
        }
    }
}