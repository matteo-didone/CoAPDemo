using CoAP;

namespace CoapClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Crea una richiesta GET per la risorsa "hello"
            Request request = new Request(Method.GET);
            request.URI = new Uri("coap://localhost/hello");

            // Invia la richiesta
            request.Send();

            // Attendi la risposta
            Response response = request.WaitForResponse();

            if (response != null)
            {
                Console.WriteLine("Risposta ricevuta: " + response.ResponseText);
            }
            else
            {
                Console.WriteLine("Nessuna risposta ricevuta");
            }

            Console.WriteLine("Premi un tasto per terminare...");
            Console.ReadKey();
        }
    }
}