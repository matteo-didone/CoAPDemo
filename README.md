# CoAP Server e Client Demo

Questa repository contiene un'implementazione dimostrativa di un server e un client CoAP (Constrained Application Protocol) in C#. Il progetto include un server che espone alcune risorse di base e un client interattivo per comunicare con esse.

## Struttura del Progetto

Il progetto è diviso in due parti principali:

- `CoapServer/`: Implementazione del server CoAP
- `CoapClient/`: Implementazione del client CoAP

## Prerequisiti

- .NET 8.0 SDK
- Visual Studio 2022 o VS Code
- Pacchetto NuGet Com.AugustCellars.CoAP (v1.9.0)

## Installazione

1. Clona la repository:
```bash
git clone https://github.com/matteo-didone/CoAPDemo.git
```

2. Naviga nelle directory del server e del client e ripristina i pacchetti NuGet:
```bash
cd CoapServer
dotnet restore

cd ../CoapClient
dotnet restore
```

## Utilizzo

### Avvio del Server

1. Naviga nella directory del server:
```bash
cd CoapServer
```

2. Avvia il server:
```bash
dotnet run
```

Il server si avvierà e mostrerà l'indirizzo IP locale su cui è in ascolto (default: porta 5683).

### Avvio del Client

1. Naviga nella directory del client:
```bash
cd CoapClient
```

2. Avvia il client:
```bash
dotnet run
```

3. Inserisci l'indirizzo IP del server quando richiesto (premi Enter per usare localhost)

### Funzionalità Disponibili

Il server espone le seguenti risorse:

- `/.well-known/core`: Resource Discovery (standard CoAP)
- `/hello`: Risorsa di test che risponde con "Hello CoAP!"
- `/sensor`: Risorsa simulata di un sensore con operazioni GET, POST, PUT e DELETE

Il client offre un menu interattivo con le seguenti operazioni:

1. Resource Discovery (GET /.well-known/core)
2. GET /hello
3. GET /sensor
4. POST /sensor
5. PUT /sensor
6. DELETE /sensor
7. Monitor /sensor (polling)
8. Cambia indirizzo server

## Struttura del Codice

### Server

Il server implementa tre risorse principali:

1. `HelloResource`: Risorsa di base che risponde alle richieste GET
2. `SensorResource`: Risorsa che simula un sensore con supporto completo CRUD
3. `WellKnownCoreResource`: Risorsa standard CoAP per il resource discovery

### Client

Il client implementa:

- Gestione delle richieste CoAP con token e Message ID
- Timeout configurabile per le richieste
- Monitoraggio asincrono delle risorse
- Interfaccia utente a linea di comando

## Note Tecniche

- Il server utilizza il binding UDP sulla porta 5683 (default CoAP)
- Le richieste hanno un timeout di 10 secondi
- Il monitoraggio del sensore effettua polling ogni 2 secondi
- I token delle richieste sono generati casualmente (4 byte)

## Troubleshooting

Se si verificano problemi di connessione:

1. Verificare che il firewall permetta il traffico UDP sulla porta 5683
2. Controllare che client e server siano sulla stessa rete
3. Verificare che l'indirizzo IP del server sia corretto
4. Controllare eventuali antivirus che potrebbero bloccare la comunicazione

## Contribuire

Le pull request sono benvenute. Per modifiche importanti, aprite prima un issue per discutere cosa vorreste cambiare.

## Licenza

[MIT](https://choosealicense.com/licenses/mit/)

## Contatti

Matteo Didonè - [matteo.didone@stud.itsaltoadriatico.it]