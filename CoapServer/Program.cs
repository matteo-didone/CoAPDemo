using CoAP.Server;
using CoAP.Server.Resources;

namespace MyCoapServer  // Cambiato il nome del namespace
{
    class Program
    {
        static void Main(string[] args)
        {
            // Crea un nuovo server CoAP sulla porta predefinita (5683)
            var server = new CoapServer();  // Ora non c'è più conflitto

            // Aggiungi la risorsa "hello"
            server.Add(new HelloResource());

            // Avvia il server
            server.Start();

            Console.WriteLine("Server CoAP avviato. Premi un tasto per terminare...");
            Console.ReadKey();

            // Arresta il server quando l'applicazione viene chiusa
            server.Dispose();
        }
    }

    // Definizione della risorsa "hello"
    class HelloResource : Resource
    {
        public HelloResource() : base("hello")
        {
            Attributes.Title = "Hello Resource";
        }

        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Respond("Hello, CoAP!");
        }
    }
}