using CoAP;
using CoAP.Server;
using CoAP.Server.Resources;
using System.Net;

namespace MyCoapServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ottieni l'indirizzo IP locale
            var localIP = GetLocalIPAddress();
            Console.WriteLine($"Indirizzo IP locale trovato: {localIP}");

            // Crea il server
            var server = new CoapServer();
            server.Add(new HelloResource());
            server.Add(new SensorResource());
            server.Add(new WellKnownCoreResource());

            try
            {
                server.Start();
                Console.WriteLine($"Server CoAP avviato con successo su coap://{localIP}:5683");
                Console.WriteLine("Server in ascolto su tutte le interfacce disponibili:");
                var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up);
                foreach (var iface in interfaces)
                {
                    Console.WriteLine($"- {iface.Name}: {iface.NetworkInterfaceType}");
                    var addrs = iface.GetIPProperties().UnicastAddresses;
                    foreach (var addr in addrs)
                    {
                        if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Console.WriteLine($"  IPv4: {addr.Address}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'avvio del server: {ex.Message}");
                return;
            }

            Console.WriteLine("\nPremi un tasto per terminare...");
            Console.ReadKey();
            server.Dispose();
        }

        private static IPAddress GetLocalIPAddress()
        {
            var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                // Cerca solo interfacce attive che non sono loopback
                if (iface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                    iface.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                {
                    var addrs = iface.GetIPProperties().UnicastAddresses;
                    foreach (var addr in addrs)
                    {
                        if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return addr.Address;
                        }
                    }
                }
            }
            // Se non troviamo un indirizzo valido, usiamo loopback come fallback
            return IPAddress.Parse("127.0.0.1");
        }
    }

    class HelloResource : Resource
    {
        public HelloResource() : base("hello")
        {
            Attributes.Title = "Hello Resource";
            Attributes.AddResourceType("message");
        }

        protected override void DoGet(CoapExchange exchange)
        {
            try
            {
                Console.WriteLine($"Ricevuta richiesta GET da {exchange.Request.Source}");
                Console.WriteLine($"Token: {BitConverter.ToString(exchange.Request.Token)}");
                Console.WriteLine($"Message ID: {exchange.Request.ID}");

                var response = new Response(StatusCode.Content);
                response.PayloadString = "Hello CoAP!";
                exchange.Respond(response);
                Console.WriteLine("Risposta inviata");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella gestione della richiesta: {ex}");
                exchange.Respond(StatusCode.InternalServerError);
            }
        }
    }

    class SensorResource : Resource
    {
        private string sensorValue = "25.5"; // Valore iniziale simulato

        public SensorResource() : base("sensor")
        {
            Attributes.Title = "Sensor Resource";
            Attributes.AddResourceType("sensor");
            Attributes.Observable = true;
        }

        protected override void DoGet(CoapExchange exchange)
        {
            try
            {
                Console.WriteLine($"Ricevuta richiesta GET da {exchange.Request.Source}");
                Console.WriteLine($"Token: {BitConverter.ToString(exchange.Request.Token)}");
                Console.WriteLine($"Message ID: {exchange.Request.ID}");

                var response = new Response(StatusCode.Content);
                response.PayloadString = sensorValue;
                exchange.Respond(response);
                Console.WriteLine($"Inviato valore sensore: {sensorValue}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella gestione della richiesta: {ex}");
                exchange.Respond(StatusCode.InternalServerError);
            }
        }

        protected override void DoPost(CoapExchange exchange)
        {
            try
            {
                sensorValue = exchange.Request.PayloadString;
                Console.WriteLine($"Aggiornato valore sensore a: {sensorValue}");
                exchange.Respond(StatusCode.Created, "Valore aggiornato: " + sensorValue);
                Changed();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella gestione della richiesta POST: {ex}");
                exchange.Respond(StatusCode.BadRequest);
            }
        }

        protected override void DoPut(CoapExchange exchange)
        {
            try
            {
                sensorValue = exchange.Request.PayloadString;
                Console.WriteLine($"Modificato valore sensore a: {sensorValue}");
                exchange.Respond(StatusCode.Changed, "Valore modificato: " + sensorValue);
                Changed();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella gestione della richiesta PUT: {ex}");
                exchange.Respond(StatusCode.BadRequest);
            }
        }

        protected override void DoDelete(CoapExchange exchange)
        {
            try
            {
                sensorValue = "0.0";
                Console.WriteLine("Reset valore sensore a 0.0");
                exchange.Respond(StatusCode.Deleted);
                Changed();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella gestione della richiesta DELETE: {ex}");
                exchange.Respond(StatusCode.InternalServerError);
            }
        }
    }

    class WellKnownCoreResource : Resource
    {
        public WellKnownCoreResource() : base(".well-known/core")
        {
            Attributes.Title = "Resource Discovery";
        }

        protected override void DoGet(CoapExchange exchange)
        {
            try
            {
                Console.WriteLine($"Ricevuta richiesta Resource Discovery da {exchange.Request.Source}");
                exchange.Respond(StatusCode.Content, "</hello>;rt=\"message\",</sensor>;rt=\"sensor\";obs", MediaType.ApplicationLinkFormat);
                Console.WriteLine("Inviata lista risorse");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella gestione della richiesta Resource Discovery: {ex}");
                exchange.Respond(StatusCode.InternalServerError);
            }
        }
    }
}