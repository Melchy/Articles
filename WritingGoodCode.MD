# Střípky z code review

## Střípky z code review 1 - Přirozený jazyk

```csharp

public string NameShortener(string name){
    var userName = ShortenName(name);

    if (name.Contains("OurCompanyName"))
    {
        userName = name;
    }
    return userName;
}



```

Tato metoda zkracuje všechna jména s vyjímkou těch které obsahují název firmy.

Dokážete najít co je na ni špatně z pohledu čitelnosti kódu?

### Problém

Pokud byste logiku kterou kód provádí popisovali přirozeným jazykem tak byste řekli něco jako: "Pokud uživatelské jméno neobsahuje OurCompanyName tak ho zkrátíme.".

Kód který jsem uvedl výše bychom ale popsali jinak. Pravděpodobně nějak takhle: "Zkrátíme jméno a pokud obsahuje OurCompanyName tak ho nastavíme zpět na původní hodnotu.".
Kód tedy dělá stejnou logiku ale popisuje ji odlišným způsobem.

### Fix

```csharp

public string NameShortener(string name){

    if (name.Contains("OurCompanyName"))
    {
        return name;
    }

    return ShortenName(name);
}

```

### Pravidlo

Kód by měl být napsaný stejným způsobem jakým ho popisuje přirozeným jazyk.

Přirozený jazyk je důležitý jelokož je to přirozený způsob jakým přemýšlíme. Pokud kód vypadá jinak než jak ho popisujeme tak musí náš mozek více přemýšlet.

## Střípky z code review 2 - Nenuť mě přemýšlet

```csharp
public string RemoveNumberFromUrlIfPresent(){
    if (url.Contains("/"))
    {
        int slashIndex = url.LastIndexOf('/');

        if (int.TryParse(url.Substring(slashIndex + 1), out _)))
        {
            url = url.Substring(0, slashIndex);
        }
    }

    return url;
}

```

Toto metoda rozděluje Url na dvě části podle znaku '/' poté kontroluje zda druhá část obsahuje číslo. Pokud ho obsahuje tak vrátí pouze první část. 
Pokud číslo neobsahuje nebo neobsahuje ani zna '/' tak se vrátí celá Url bez úprav.

Dokážete najít co je na ni špatně z pohledu čitelnosti kódu?

## Problém

Pro čtenáře kódu je poměrně náročné studovat indexy a rozdělování řetězců.
Na první pohled není zřejmé jestli pod odříznutí čísla bude výsledná Url obsahovat '/' nebo bude také odstraněno.

Zároveň od čtenáře očekáváte že ví jak přesně funguje metoda `Substring`.
Např. co vrátí `"abcdef".Substring(1, 4)`? Je druhý parametr v metodě Substring index nebo počet?

### Fix

```csharp

public string RemoveNumberFromUrlIfPresent(string url)
{
    (var firstPartUrl, var secondPartOfUrl) = url.SplitAtLastOccurenceIfSeparatorIsPresent("/");

    if (int.TryParse(ssecondPartOfUrl, out _))
    {
        return firstPartUrl;
    }

    return url;
}


public static class DotNetExtensionMethods{

    public static (string firstPart, string? secondPart) SplitAtLastOccurenceIfSeparatorIsPresent(this string text, string separator)
    {
        int slashIndex = text.LastIndexOf("/");

        if (slashIndex > -1)
        {
            return (text.Substring(0, slashIndex), text.Substring(slashIndex + 1));
        }
        else
        {
            return (text, null);
        }
    }

}
```

### Pravidlo

`Don't make me thing` je kniha o UX designu ktrá popisuje jak by měli být navrženy uživatelské rozhraní tak aby nenutili uživatele přemýšlet.
Myslím, že v programování je také vhodné se tímto pravidlem řídit.

Každý pokročilejší programátor je schopný přečíst kód který rozděluje stringy ale musí nad tím alespoň trochu přemýšlet. Pokud dodržujeme pravidlo
`Don't make me thing` tak se snažíme skrýt před programátorem všechny části kódu které by ho nutili přemýšlet o detailech které nejsou podstatné pro
aktuální metodu.

Metoda `RemoveNumberFromUrlIfPresent` slouží k odstranění čísla z URL a není pro ni podstatné jak přesně se URL rozdělí. Čtenáře tedy pravděpodobně
nebude zajímat jak přesně se počítá index pro rozdělení a proto ho tím nemusíme zatěžovat. Pokud ho zajímá tento detail tak se může podívat do specializované metody.


https://gitlab.notino.com/web/blog/-/merge_requests/157#note_468909



## Střípky z code review 3 - Neočekávané chování

```csharp

public IEnumerable<Recomendations> GetRecomentadtionsFromExternalService()
{
    if (!settings.AreReccomedationsOn)
    {
        return new IEnumerable<Recomendations>();
    }

    //.. call external service
    return reccomendations;
}

```

Tato metoda slouží pro získání doporučení z externí služby. Pokud jsou doporučení vypnuté pomocí nastavení tak nevrátí žádné výsledky.

Dokážete najít co je na ni špatně z pohledu čitelnosti kódu?

### Problém

Metoda se jmenuje `GetRecomentadtionsFromExternalService` ale nevrací pouze výsledek z externí služby. Dělá také kontrolu zda jsou reccomendations vypnuté.

Pokud v kódu uvidíte volání `GetRecomentadtionsFromExternalService()` které vrátilo prázdný výsledek tak si budete myslet že externí služba nevrátia
žádné recommendations. Jak ale můžete vidět z implementace tak to nemusí být pravda.

### Fix

```csharp

public void DoSomeThing()
{
    if (settings.AreReccomedationsOn)
    {
        return;
    }

    var recommendations = GetRecomentadtionsFromExternalService();
    
    //... do something with reccomendations
}


public IEnumerable<Recomendations> GetRecomentadtionsFromExternalService()
{
    //.. call external service
    return reccomendations;
}

```

Jednoduše stačí kontrolu na spuštěné reccomendations přesunout na jinou úroveň.
Dalším možným řešením by bylo informovat o stavu:

```csharp
public RecomendationsResult GetRecomentadtionsFromExternalService()
{
    if (settings.AreReccomedationsOn)
    {
        return new RecomendationsResult(false, null);
    }

    //.. call external service
    return new RecomendationsResult(true, reccomendations);
}

public class RecomendationsResult{

    public bool AreRecommendationsOn { get; }
    public IEnumerable<RecomendationsResult>? Reccomendations { get; }

    private RecomendationsResult(bool areRecommendationsOn, IEnumerable<ReccomedationsResult>? _reccomendations){
        areRecommendationsOn = _reccomendations;
    }
}

```

Dále by se také metoda dala přejmenovat třeba na `GetRecomentadtions()`.

### Pravidlo

Metody a třídy by měli provádět pouze tu logiku kterou od nich volající očekává. Vždy když píšete metodu tak se ptejte: "Kdybych neznal implementaci této metody
a chtěl bych ji zavolat tak co bych očekával že bude dělat?".

## Střípky z code review 4 - Invert if

```csharp
public static bool DoSomething()
{
    if(something){
        //..loong method
    }
    else
    {
        return false;
    }    
}
```

### Problém

Metoda je zbytečně zanořená.

### Fix

```csharp
public static bool DoSomething()
{
    if(!something){
        return false;
    }
    
    //..loong method 
}
```


### Pravidlo

Pokud můžete tak se vždy snažte vyhnout zanoření.


## Střípky z code review 5 - Code intent

```csharp
public string GetWebsiteUrl()
{
    return "{ShopId}/website/{ItemUrl}/";
}
```
Kód uvedený výše vrací url nějaké webové stránky a obsahuje bug. Pokud je `ItemUrl = null` tak se vygeneruje url `"cz/website//"` namísto `"cz/website/"`.

Jeden z programátorů tento kód opravil následujícím způsobem:

```csharp
public string GetWebsiteUrl()
{
    return Regex.Replace($"{ShopId}/beautyblog/{ItemUrl}/", @"/+", @"/");
}
```

Přidal tedy regex který nahradí všechny výskyty opakovaných lomítek jedním lomítkem.

Dokážete najít co je této opravě špatně z pohledu čitelnosti kódu?

### Problém

`Regex.Replace` sice opravuje zmíněný bug ale nevysvětlí četenáři kódu proč by nově vygenerovaná URL měla obsahovat více lomítek.
Většinu čtenářů by napadlo že proměná `ShopId` nebo `ItemUrl` může obsahovat více lomítek a proto je potřeba je odstranit.
Opak je ale pravdou. `ShopId` ani `ItemUrl` lomítka nemusí nikdy obsahovat a i přesto je potřeba `Regex.Replace`.

Dalším problémem tohoto kódu je že může skrýt bug v aplikaci. Pokud se do proměné `ItemUrl` dostane `//` které tam nemá být tak se tiše 
odstraní bez toho aby se o tom někdo dozvěděl. Aplikace se tedy bude tvářit jakože funguje ale ve skutečnosti je někde 
bug který umožňuje aby se do proměné `ItemUrl` dostávali nevalidní vstupy.

### Fix

```csharp
public string GetWebsiteUrl()
{
    var itemUrl = ItemUrl == null ? "" : "${ItemUrl}/" 
    return $"{ShopId}/beautyblog/{itemUrl}";
}
```

V případě že bychom opravdu chtěli počítat se situací kdy `ItemUrl` nebo `ShopId` může obsahovat `//` tak je potřeba zalogovat chybu
aby programátoři věděli že se aplikace dostala do chybného stavu:

```csharp
public string GetWebsiteUrl()
{
    var itemUrl = ItemUrl == null ? "" : "${ItemUrl/}"
    var websiteUrl = $"{ShopId}/beautyblog/{ItemUrl}/";
    if(websiteUrl.Contains("//"))
    {
        _logger.LogError("Website url {websiteUrl} contains repeated slash. Url was generated from {ShopId} and {ItemUrl}.", websiteUrl, ShopId, ItemUrl);
    }
    return Regex.Replace(websiteUrl, @"/+", @"/");
}
```

### Pravidlo

Kód by měl popisovat myšlenku a měl by ji popisovat co nejpřesněji. 
Zobecněním kódu často ztratíme infromaci o původním problému který jsme měli řešit což vede k horší čitelnosti.



## Střípky z code review 6 - Http client call


## Code review 7 - If else instead of if

var queueOptions = new CreateQueueOptions(queueName)
{
MaxSizeInMegabytes = 1024,
MaxDeliveryCount = 3,
DefaultMessageTimeToLive = TimeSpan.FromDays(14),
LockDuration = TimeSpan.FromMinutes(5),
DeadLetteringOnMessageExpiration = true,
EnablePartitioning = true,
};

                var namespaceProperties = await serviceBusAdministrationClient.GetNamespacePropertiesAsync();
                if (namespaceProperties.Value.MessagingSku == MessagingSku.Standard)
                {
                    queueOptions.MaxSizeInMegabytes = 2048;
                    queueOptions.LockDuration = TimeSpan.FromSeconds(30);
                    queueOptions.RequiresDuplicateDetection = true;
                    queueOptions.RequiresSession = true;
                }

Ja myslim ze tohle je trošičku zvalstni.
Nahore nastavíš property a pak je o par radku niz zase přenastavíš.
Myslim si ze by bylo citelnejsi nahore nastavit spolecne property a pak mit
If else ktery nastavy hodnoty pro Standard a basic.

Priklad proc je tohle hure citelne: nekdo zkouma kod a
koukne se pouze na začátek metody a rekne si AHA tak ze
tohle jsou veci ktere nastavujeme a dal uz necte.

## Code review 8 - shorter code is not always clearer

return (validBarcodeEntered && partner.CarrierLoginKind != 1) || passwordEntered || validShippingListEntered;

se da prepsat na

```csharp
if (partner.CarrierLoginKind == 1)
{
    return validPasswordEntered || validShippingListEntered;
}
else
{
    return validPasswordEntered || validShippingListEntered || validBarcodeEntered;
}
```

### Problém

### Fix

### Pravidlo

## Střípky z code review 11 - Dobře definovaný objekt

### Problém
### Fix
### Pravidlo


https://gitlab.notino.com/web/navigationapi/-/merge_requests/119#note_415321


## Střípky z code review 11 - Konstruktory

### Problém
### Fix
### Pravidlo



## Střípky z code review 11 - Podrobne logy

### Problém
### Fix

### Pravidlo

### Střípky z code review 11 - explicitni vyjadreni kodu

Null jako druhe heslo znamena ze druhe heslo je nepovinne.

### Střípky z code review 11 - dlouhe ify

private bool IsSuspectTransition(SlotEvent e)
{
return _alarmActivationStates.Contains(e.ToState) && e.FromState == QuidoSlotState.Locked;
}

return !IsSuspectTransition(e) && !(e.FromState == QuidoSlotState.Locked && e.ToState == QuidoSlotState.Open) && !(
e.FromState == QuidoSlotState.Open && e.ToState == QuidoSlotState.Locked);

//Hard topics

## Střípky z code review 9 - Radsi vice citelne nez nahrane

```csharp
// no code this time :(
```

### Problém


### Pravidlo




## Střípky z code review 7 - Encapsulation ???

### Problém
### Fix
### Pravidlo

## Střípky z code review 8 - Nucene poradi metod

### Problém
### Fix
### Pravidlo

https://gitlab.notino.com/web/navigationapi/-/merge_requests/143#note_447200
