# Volání aplikace v testech

V minulém díle jsme se podívali na to jak mockovat externí závisloti a tím zrychlit integrační testy. V tomto článku představíme několik způsobů volání aplikace v rychlích integračních testech.

## Volání apliace nebo volání metod

Integrační testy obvykle volají API aplikace. Unit testy naopak volají metody přímo v kódu aplikace. Podle popisu v předchozích článcích už asi tušíte že volání API je spolehlivější a méně křehké.
Spolehlivost je vyšší jelikož testujeme celý endpoint tak jak ho bude volat i náš uživatel. Nemůže se tedy stát že nám zůstane nějaká neotestovaná část aplikace. Křehkost je nižší u integračních testů
jelikož API se mění méně často než kód aplikace.

Oba dva přístupy k testování se hodí v různých situacích. V dalších částech tohoto článku se podíváme na oba způsoby o něco podrobněji.

## Volání metod v unit testech

V unit testech  není možné volat Controllery jelikož obsahují věci spojené s obsluhováním http requestu. Z tohoto důvodu musíme všechnu logiku vyjmout z controllerů do nové vrstvy kterou poté otestujeme a tváříme se že controller neexistuje
Příklad:

//TODO příklad.

Spolehlivost takového testu je poměrně nízká jelikož není otestován ani controller a ani asp.net pipeline.
V kódu pak často často narazíme na problémy s mapováním dat v controllerech.

```csharp
[HttpGet("{notificationId}")]
public Task<ActionResult> Get([FromQuery] Guid notificationId) //[FromQuery] by mělo být [FromRoute]
{
     return await service.Execute(notificationId);
}

```

Tyto chyby se dějí tak často že nás donutí každý call testovat ručním nebo integračním testem.

## Volání api v integračních testech

.Net core nabízí testovacího [clienta](https://docs.microsoft.com/cs-cz/aspnet/core/test/integration-tests?view=aspnetcore-3.1) který dokáže spustit aplikaci a simulovat http dotazy.

//TODO priklad

Testovací klient řeší problémy unit testů. Nevýhodou ale je že neumožňuje silně typované volání metod na controlleru.

ASP.Net core ale obsahuje [LinkGenerator](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.linkgenerator?view=aspnetcore-3.1) který umožňuje vygenerování http cesty na základě jména controleru a akce - `linkGenerator.GetPathByAction("actionName", "controllerName", routeParams)`. Pokud použijeme linkGenerator a trochu magie [interceptorů](https://stackoverflow.com/questions/2592787/what-really-interceptors-do-with-my-c-sharp-class) tak můžeme vytvořit kód který se tváří že volá metodu controlleru ale ve skutečnosti transformuje volání na http request.

```csharp
[Test]
public async Task Test()
{
    // CreateCaller vytvoří interceptor který z volané metody
    // získá jméno controlleru, jméno akce, všechny parametry
    // a z attributu [HttpGet, Post..] akci která se má zavolat.
    // Poté pomocí link generátoru vytvoří link a zavolá
    // danou metodu pomocí testovacího http clienta
    var controllerCaller = CreateCaller<Controller>();
    var result = await controllerCaller.SecurePage(1,2);
}
```

Knihovnu která transformuje cally metod na http requesty můžete najít zde. Celý integrační test může vypadat následujícím způsobem.

```csharp
[Test]
public async Task Test()
{
    // Create test client, execute additional setup
    // New "C# 8.0 using"
    using var app = CreateApp();
    // Create interceptor
    var controllerCaller = app.CreateCaller<Controller>();
    // Call action with two numbers
    ActionResult response = await controllerCaller.SumNumbers(1,2);
    // Custom extension method. Get content or throw for 500
    var result = response.GetContentOrThrow();
    // Validate result using fluent assertions
    result.Value.Should().Be(3);
}
```

Integrační testy s interceptorem nejsou o mnoho složitější než kdybychom použili unit testy pro volání aplikace.

## Setup dat v testech

Setup dat můžeme provést dvěmy způsoby. Voláním metod/api a nebo přímým vložením dat do databáze.

// TODO příklad obou přístupů

Preferovaným přístupem je vkládání dat pomocí volání metod/api. Pokud bychom vkládali data přímo do databáze tak se naše testy rozbijí pokaždé když upravíme schéma databáze. Výsledkem vkládání dat do databáze jsou opět fragile testy.

V některých případech není možné při integračních testech nastavit správná data jelikož aplikace nemá potřebné endpointy. Můžeme si představit jednoduchý příklad. Představme si aplikaci která obsahuje endpointy pouze pro získání faktur o uživateských objednávkách. Pro potřeby testování ale potřebujeme do aplikace nahrát několik faktur.

Tento problém můžeme vyřešit tak že přidáme speciální metodu pro vytváření faktur v testech. Tato metoda je definicí kontraktu pro testování a měli bychom se k ní chovat podobně jako by to byl reálný endpoint používaný uživatelem.

//TODO příklad

Osobně tyto metody vkládám do speciálního projektu s názvem PublicContracts. Tento projekt by neměl mít žádné závislosti aby nedošlo k úniku implementačních detailů ze zbytku aplikace (což opět vede ke snížení křehkosti testů).

## Razor a volání aplikace

//TODO az dodelam tool

## Shrnutí

Při volání aplikace máme na výběr ze dvou způsobů. Volání API a vytvoření vlastní vrstvy který leží hned pod controllery. Volání Api je často lepší způsob jelikož jsou výsledné testy spolehlivější. Pro další zjednodušení integračních testů je možné použít package který zajišťuje silně typované volání aplikace.

Tímto článkem jsme ukončili povídání o ryhlích integračních testech. V příštím díle se podíváme na druhou kategorii testů - unit testy.
