using CoAP;
using CoAP.Server;
using CoAP.Server.Resources;

namespace MyCoapServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new CoapServer();

            // Aggiunta delle risorse
            server.Add(new HelloResource());         // Risorsa base
            server.Add(new SensorResource());        // Risorsa sensore
            server.Add(new WellKnownCoreResource()); // Resource discovery

            server.Start();
            Console.WriteLine("Server CoAP avviato sulla porta 5683");
            Console.WriteLine("Premi un tasto per terminare...");
            Console.ReadKey();
            server.Dispose();
        }
    }

    // Risorsa base che risponde a GET
    class HelloResource : Resource
    {
        public HelloResource() : base("hello")
        {
            Attributes.Title = "Hello Resource";
            Attributes.AddResourceType("message");
        }

        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Respond(StatusCode.Content, "Hello CoAP!", MediaType.TextPlain);
        }
    }

    // Risorsa che simula un sensore con supporto a GET, POST, PUT, DELETE
    class SensorResource : Resource
    {
        private string sensorValue = "25.5"; // Valore iniziale simulato

        public SensorResource() : base("sensor")
        {
            Attributes.Title = "Sensor Resource";
            Attributes.AddResourceType("sensor");
            Attributes.Observable = true; // Permette l'osservazione della risorsa
        }

        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Respond(StatusCode.Content, sensorValue);
        }

        protected override void DoPost(CoapExchange exchange)
        {
            try
            {
                sensorValue = exchange.Request.PayloadString;
                exchange.Respond(StatusCode.Created, "Valore aggiornato: " + sensorValue);
                Changed(); // Notifica gli observer che il valore è cambiato
            }
            catch
            {
                exchange.Respond(StatusCode.BadRequest);
            }
        }

        protected override void DoPut(CoapExchange exchange)
        {
            try
            {
                sensorValue = exchange.Request.PayloadString;
                exchange.Respond(StatusCode.Changed, "Valore modificato: " + sensorValue);
                Changed(); // Notifica gli observer
            }
            catch
            {
                exchange.Respond(StatusCode.BadRequest);
            }
        }

        protected override void DoDelete(CoapExchange exchange)
        {
            sensorValue = "0.0"; // Reset del valore
            exchange.Respond(StatusCode.Deleted);
            Changed(); // Notifica gli observer
        }
    }

    // Implementazione personalizzata di /.well-known/core
    class WellKnownCoreResource : Resource
    {
        public WellKnownCoreResource() : base(".well-known/core")
        {
            Attributes.Title = "Resource Discovery";
        }

        protected override void DoGet(CoapExchange exchange)
        {
            // Risponde con la lista delle risorse disponibili in formato Core Link Format
            exchange.Respond(StatusCode.Content, "</hello>;rt=\"message\",</sensor>;rt=\"sensor\";obs", MediaType.ApplicationLinkFormat);
        }
    }
}