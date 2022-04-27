## Static factory method in C# 

Při psaní C# často narazíme na situace kdy potřebujeme použít statickou factory metodu. V tomto článku se
podíváme na nejčastější použití kterými jsou - pojmenování konstruktoru, zapouzdření třídy, 
zjednodušení generiky, asynchroní konstruktor, vracení hodnot při vytváření třídy a 
transformace parametrů před voláním base třídy.

Nejdříve se ale podíváme jak vlastně vypadá jednoduchá faktory metoda:

```csharp
    public class User
    {
        public User()
        {
        }
        
        public static User CreateUser(){
            return new User();
        }
    }
```

Jak můžete vidět je to vlastně jednoduchá metoda která vytváří třídu ve které je implementováná.
Použití je následující:

```csharp
    User user = User.CreateUser();
```

V dalších odstavcích si ukážeme důvody proč použít factory metodu na místo klasického
konstruktoru.

## Použití factory metody

### Pojmenovani konstruktoru

V některých případech se hodí při psaní kódu pospsat čtenáři jaký druh objekt vytváříme.
Představme si že vytváříme uživatele který si zaplatil prémiové předplatné. Vytvoření
takového uživatele může vypadat následovně:

```csharp
var user = new User(name: "John", surname: "doe", age: 20, UserType.Premium)
```

Pomocí factory metody můžeme přepsat tento kód následujícím způsobem:

```csharp
var user = User.CreatePremiumUser(name: "John", surname: "doe", age: 20);

public class User
{
    //...
    
    public static CreatePremiumUser(string name, string surname, int age)
    {
        return new User(name, surname, age, UserType.Premium)
    }
}
```

### Zapouzdření

Factory metoda nám může také pomoci abychom umožnili objekt vytvářet pouze
ve validním stavu. 

Představme si že vytváříme response objekt který nese stavový
kód a obsah. Pokud je ale stavový kód `Error` tak objekt nemůže nikdy obsahovat
obsah.

Popsaný objektu respose může vypadat následucjícím způsobem:

```csharp
public class Response
{
    public StatusCode StatusCode { get; }
    public string? Content { get; }
    
    public Response(StatusCode statusCode, string? Content)
    {
        StatusCode = statusCode;
        Content = content;
    }
}
```

Problémem této implementace ale je že nic nezabrání programátorovi vytvořit
Response objekt se stavovým kódem `Error` a `Contente` který není null:

```csharp
var response = new Response(StatusCode.Error, "This is incorrect");
```

Mohly bychom do konstruktoru přidat následující podmínku:

```csharp
public class Response
{
    public StatusCode StatusCode { get; }
    public string? Content { get; }
    
    public Response(StatusCode statusCode, string? Content)
    {
        if(statusCode == StatusCode.Error && Content != null)
        {
            throw new InvalidOperationException($"Can not create {nameof(Response)} with content and status code {StatusCode.Error}");
        }
        StatusCode = statusCode;
        Content = content;
    }
}
```

Tato implementace zajistí že programátor nemůže vytvořit `Response` objekt v 
nevalidním stavu. Problémem ale je že programátor který se nepodívá na implementaci
konstruktoru nemá jak zjistit že objekt nemůže být vytvořen se status code Error a
nenullovým contentem. Dozví se to až při spuštění. V lepším případě při testování
v horším případě až v produkci.

Pomocí statické factory metody můžeme zajistit že programátor nevytvoří
objekt v nevalidním stavu pomocí kompilátoru:

```csharp
public class Response
{
    public StatusCode StatusCode { get; }
    public string? Content { get; }
    
    // Notice that constructor is private
    private Response(StatusCode statusCode, string? Content)
    {
        StatusCode = statusCode;
        Content = content;
    }
    
    public static Response CreateOkResponse(string? content)
    {
        return new Response(StatusCode.Ok, content);
    }
    
    public static Response CreateErrorResponse()
    {
        return new Response(StatusCode.Error, null)
    }
}
```

Privátní konstruktor zajistí že objekt nemůže být vytvořen přes new a může
být vytvořen pouze pomocí předem definovaných factory metod které zajistí
správné předání parametrů.

### Úprava argumentů před voláním bázové třídy

C# neumoňuje volání bázového konstruktoru uprostřed konstruktoru:

```csharp
public class PremiumUser : User
{
    public PremiumUser()
    {
        var discount = CalculateDiscount();
        base(discount); // this is invalid in c#
    }

    public int CalculateDiscount()
    {
        // Complex discount calculation
    }
}
```

Tento problém ale můžeme vyřešit statickou factory metodou:

```csharp
public class PremiumUser : User
{
    private PremiumUser(int discount)
    {
        var discount = CalculateDiscount();
        base(discount); // this is not allowed
    }

    public static int CalculateDiscount()
    {
        // Complex discount calculation
    }
    
    public static PremiumUser Create()
    {
        var discount = PremiumUser.CalculateDiscount();
        reurn new PremiumUser(discount);
    }
}
```

Volání bázového konstruktoru může být vyřešeno i jinými způsoby ale 
factory metoda může občast být nejjednodušší nebo nejelegantnější řešení.

Zjednodušení generiky

Konstruktory v C# neumějí odvodit generické parametry z typů předaných
hodnot. Musí být vždy uvedeny což vede ke zbytečnému psaní kódu.

Přikladem může být třída `KeyValuePair` z .net knihovny. Tuto třídu
můžeme vytvořit pomocí `new`:

```csharp
var keyValueExample = new KeyValuePair<string, string>("key", "value");
```

Microsoft nám ale připravil statickou factory metodu který nám trochu zjednoduší
vytváření:

```csharp
var keyValueExample = KeyValuePair.Create("key", "value");
```

Všiměte si že u druhého příkladu nemusíme uvádět generické typy protože 
si je kompilátor dokáže sám odvodit.

### Asynchroní konstruktor

C# také neumožňuje aby byly konstruktory asynchroní. Pokud ale potřebujete
vytvořit objekt který v při konstrukci musí volat asynchroní operace 
tak můžete použít factory metodu.

Pamatujte ale že bychom se obecně měli vyhýbat konstruktorům které provádí
složité operace. [Microsoft guidlines říkají](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/constructor?redirectedfrom=MSDN):

> Constructors should not do much work other than capture the constructor parameters. The cost of any other processing should be delayed until required.

### Vracení hodnot z konstrukce objektu

Posledním použitím je vracení hodnot v konstruktoru. Typickým příkladem
je situace kde konstruktor dělá validace a potřebuje informovat volajícího že
nastala chyba a nechce vyhazovat vyjímku.

Takovou situaci bychom mohly vyřešit statickou factory metodou která provede
validace namísto konstruktoru a vrátí výsledek:

```csharp

public class User
{
    private User(int age)
    {
        //...
    }
    
    public static Result<User> Create(int age)
    {
        if(age < 18)
        {
            return Result.CreateError("User is too young");
        }
        return new Result.CreateOk(new User(age));
    }
}
```


### Nevýhody factory metody

Jak už to tak bývá tak žádný pattern nemá pouze výhody. Nevýhody factory
metody jsou následující:

* Programátor se nemá jak dozvědět že musí objekt vytvářet pomocí factory metody a může pro něj být matoucí že nemůže objekt vytvořit pomocí `new`.
* Všechny parametry je potřeba přemapovat. Většinu parametrů které předáváme do konstruktoru musíme předat i do factory metody což vytváří zbytečnou duplikaci.
