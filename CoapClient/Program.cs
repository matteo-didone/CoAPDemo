using CoAP;
using System.Net;

namespace MyCoapClient
{
    class Program
    {
        private static string serverAddress = "localhost";
        private static int timeout = 10000; // 10 secondi di timeout
        private static Random random = new Random();

        static void Main(string[] args)
        {
            Console.Write("Inserisci l'indirizzo IP del server CoAP (premi Enter per usare localhost): ");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                serverAddress = input;
            }

            Console.WriteLine($"Connesso al server: coap://{serverAddress}:5683");

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
                Console.WriteLine("8. Cambia indirizzo server");
                Console.WriteLine("0. Esci");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case 1:
                                DiscoverResources();
                                break;
                            case 2:
                                GetHello();
                                break;
                            case 3:
                                GetSensor();
                                break;
                            case 4:
                                PostSensor();
                                break;
                            case 5:
                                PutSensor();
                                break;
                            case 6:
                                DeleteSensor();
                                break;
                            case 7:
                                MonitorSensor();
                                break;
                            case 8:
                                ChangeServerAddress();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Scelta non valida!");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore durante l'operazione: {ex.Message}");
                    }
                }
            }
        }

        static Request CreateRequest(Method method, string path)
        {
            var request = new Request(method);
            request.ID = random.Next(1, 65535);
            request.Token = new byte[] { (byte)random.Next(256), (byte)random.Next(256),
                                       (byte)random.Next(256), (byte)random.Next(256) };
            request.URI = new Uri($"coap://{serverAddress}/{path.TrimStart('/')}");
            return request;
        }

        static void SendRequest(Request request)
        {
            try
            {
                Console.WriteLine($"Inviando richiesta a {request.URI}...");
                Console.WriteLine($"Message ID: {request.ID}");
                Console.WriteLine($"Token: {BitConverter.ToString(request.Token)}");

                request.Send();
                Console.WriteLine("Richiesta inviata, in attesa di risposta...");

                var response = request.WaitForResponse(timeout);

                if (response != null)
                {
                    Console.WriteLine($"Risposta ricevuta:");
                    Console.WriteLine($"- Status Code: {response.StatusCode}");
                    Console.WriteLine($"- Message ID: {response.ID}");
                    if (!string.IsNullOrEmpty(response.PayloadString))
                    {
                        Console.WriteLine($"- Payload: {response.PayloadString}");
                    }
                    else
                    {
                        Console.WriteLine("- Payload: vuoto");
                    }

                    if (response.Token != null)
                    {
                        Console.WriteLine($"- Response Token: {BitConverter.ToString(response.Token)}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nessuna risposta ricevuta dopo {timeout / 1000} secondi");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la richiesta: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        static void ChangeServerAddress()
        {
            Console.Write("Inserisci il nuovo indirizzo IP del server: ");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                serverAddress = input;
                Console.WriteLine($"Indirizzo server cambiato a: coap://{serverAddress}:5683");
            }
        }

        static void DiscoverResources()
        {
            Console.WriteLine("\nScoperta risorse...");
            var request = CreateRequest(Method.GET, ".well-known/core");
            SendRequest(request);
        }

        static void GetHello()
        {
            Console.WriteLine("\nRichiesta Hello...");
            var request = CreateRequest(Method.GET, "hello");
            SendRequest(request);
        }

        static void GetSensor()
        {
            Console.WriteLine("\nLettura sensore...");
            var request = CreateRequest(Method.GET, "sensor");
            SendRequest(request);
        }

        static void PostSensor()
        {
            Console.Write("\nInserisci il nuovo valore: ");
            var value = Console.ReadLine();

            if (!string.IsNullOrEmpty(value))
            {
                var request = CreateRequest(Method.POST, "sensor");
                request.SetPayload(value);
                SendRequest(request);
            }
            else
            {
                Console.WriteLine("Valore non valido");
            }
        }

        static void PutSensor()
        {
            Console.Write("\nInserisci il valore da impostare: ");
            var value = Console.ReadLine();

            if (!string.IsNullOrEmpty(value))
            {
                var request = CreateRequest(Method.PUT, "sensor");
                request.SetPayload(value);
                SendRequest(request);
            }
            else
            {
                Console.WriteLine("Valore non valido");
            }
        }

        static void DeleteSensor()
        {
            Console.WriteLine("\nCancellazione valore sensore...");
            var request = CreateRequest(Method.DELETE, "sensor");
            SendRequest(request);
        }

        static void MonitorSensor()
        {
            Console.WriteLine("\nMonitoraggio del sensore avviato (premi un tasto per terminare)...");
            bool monitoring = true;

            Task.Run(async () =>
            {
                while (monitoring)
                {
                    try
                    {
                        var request = CreateRequest(Method.GET, "sensor");
                        request.Send();

                        var response = request.WaitForResponse(timeout);
                        if (response != null)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Valore sensore: {response.PayloadString}");
                        }
                        else
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Timeout - Nessuna risposta");
                        }

                        await Task.Delay(2000); // Poll ogni 2 secondi
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore durante il monitoraggio: {ex.Message}");
                        await Task.Delay(2000); // Aspetta prima di riprovare
                    }
                }
            });

            Console.ReadKey();
            monitoring = false;
            Console.WriteLine("\nMonitoraggio terminato");
        }
    }
}