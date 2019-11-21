# Aplikační vrsva, Servisní vrstva, Interactor

Aplikace používající vrstvenou architekturu obvykle používají tři vrstvy - Prezentační/UI, Byznyss/Domain a datovou. Toto rozdělení je ale v mnoha aplikacích nedostačující.
Mnoho architektur představuje další vrstvu která patří mezi Prezentační a Doménovou vrstu. Tato vrstva má mnoho názvů. V textech inspirovaných Martinem Fowlerem se nazývá
Aplikační. V DDD se nazývá aplikační. Uncle Bob používá název Interactor a Hexagonální architektura používá port+adaptér na levé straně aplikace.
V tomto článku se dozvíte k čemu aplikační vrstva slouží a jaké jsou její výhody.

## Aplikační vrsva
Aplikační vrstva leží mezi doménovou a prezentační vrstvou a slouží k přípravě dat pro provedení byznys logiky. Běžná operace vyžaduje získání dat z databáze, provedení operace
nad těmito daty a poté uložení nově upravených dat. V klasiké tří vrstvé architektuře všechny tyto operace provádí byznys logika. Při použití aplikační vrsty je zodpovědností
aplikační vrstvy získání dat z databáze a poté předání těchto dat byznys vrstvě která provede potřebné operace a aplikační vrstva následně uloží data.

//TODO image


Přidání aplikační vrstvy tedy způsobí značné zjednodušení a oddělení byznys logiky aplikace. Tento přístup také způsobí že byznys logika obsahuje pouze pure funkce.
Pure funkce je taková funkce která dostane vstup provede na něm operace a vrátí výstup. Bez toho aby měla jakékoliv vedlejší efekty. Hlavní výhodou pure funkcí je jejich
testovatelnost. Jelikož tyto funkce neobsahují žádné závislosti pouze vstupní a výstupní data není potřeba vytvářet žádné mocky.

```csharp
class UserApplicationService{

    private readonly ApplicationCotenxt appContext {get;set;}
    private readonly UserDomainService userService {get;set;}

    public UserApplicationService(ApplicationContext appcotnext, UserDomainService userService){
        this.appContext = appcontext;
        this.userService = userService;
    }


    public async Task ChangeUserName(string newUserName){
        var user = appContext.GetUser();
        userService.ChangeName(newUserName);
        appContext.SaveChanges();
    }
}
```

## Problémy při implementaci aplikační vrstvy

Při implementaci aplikační vrstvy se můžeme setkat s několika problémy. V někteých případech je nutné provést operaci která má vedlejší efekty.
Například u předchozího příkladu bychom mohli chtít odeslat email uživateli a upozornit ho že jeho jméno bylo změněno. 










Zakrývá byznys logiku aplikace a zveřejňuje pouze ty metody které jsou dúležité z pohledu byznysu aplikace.
Byznys logika aplikace například může obsahovat metody pro změnu 


 V tomto článku budu používat
název aplikační vrstva jelikož jsem ho viděl použit nejčastěji.





 Domain model often uses dispatch mechanics to call domain services or infrastructure services. For example Jimmi shows this tehnique here https://lostechies.com/jimmybogard/2010/03/30/strengthening-your-domain-the-double-dispatch-pattern/.

In the Tackling complexity... book this technique is also sometimes used. Mainly to achive higer flexibility of the domain.