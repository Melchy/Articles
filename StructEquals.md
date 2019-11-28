# Jak funguje Struct Equals

Všechny typy v C# dědí od bázové třídy ``Object``, která definuje metodu Equals sloužící k porovnávání objektů.
Defaultní implementace této metody se chová stejně jako ``Object.ReferenceEquals(Object a, Object b)``. Porovnává
tedy, zda reference objektů ukazují na stejné místo v paměti.

Porovnání pomocí reference není vhodné pro [hodnotové objekty](https://docs.microsoft.com/cs-cz/dotnet/csharp/language-reference/keywords/struct) jelikož jsou [předávány hodnotou](https://www.mathwarehouse.com/programming/passing-by-value-vs-by-reference-visual-explanation.php). 
Předání hodnotou vždy vytvoří novou kopii objektu a proto nemá význam používat ``Object.ReferenceEquals`` pro porovnání ekvivalence.

```csharp
//hodnotový objekt
struct A{}

var a = new A();
var v = a; //vytvoří kopii, která se uloží na jiné místo v paměti
Console.WriteLine(Object.ReferenceEquals(a,v)); //False
```

Z tohoto důvodu Všechny hodnotové typy v C# dědí od třídy [ValueType](https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/src/System/ValueType.cs) která přepisuje chování metody ``Object.Equals``. V další části se podíváme jak tento override funguje.


## ``ValueType.Equals(object obj)``

``ValueType.Equals(object obj)`` projde všechny fieldy porovnávaných typů a zavolá na nich Equals.
Pokud je výsledek všech těchto porovnání ``True``, tak jsou oba hodnotové objekty ekvivalentní.
Toto porovnávání je ale poměrně pomalé a proto se v některých případech použije optimalizace pomocí bitové kontrola. V případě použití této optimalizace se jednoduše vezme bitová reprezentace obou struktur a porovná se.
Celý algoritmus metody ``ValueType.Equals(object obj)`` popisuje následující zjednodušený kód:

```csharp
public bool Equals(object obj){
    if(obj == null) return false;

    //Pokud je obj odlišného typu než this -> return false;
    if(!SameType(this, obj)) return false;

    //Pokud this obsahuje referenční typ
    if(ContainsReferenceType(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

    //Pokud některý ze zanořených fieldů obsahuje referenční typ
    if(InnerFieldContainsReferenceType(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

    //Pokud některý ze zanořených fieldů přepisuje metodu Equals
    if(InnerFieldOveridesEquals(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

    //Zkontroluj, zda jsou bity v paměti stejné u this a obj
    return PerformBitwiseCheck(this, obj);
}
```

Optimalizace pomocí bitové kontroly se tedy zavolá pokud struktura neobsahuje referenční typ nebo strukturu přepisující Equals. Několik následujících příkladů ukazuje popsané chování. První příklad ukazuje nejjednodušší situaci kdy struktura obsahuje
pouze hodnotový typ.

```csharp
public struct MyThing
{
    public int ValueType;
}

public static void Main()
{
    MyThing f, s;
    f.ValueType = 0;
    s.ValueType = 0;

    f.Equals(s);  // Provede bitovou kontrolu
}
```

Následující příklad ukazuje strukturu s vnořeným referenčním typem.

```csharp
public struct MyThing
{
    public string ReferenceType;
}

public static void Main()
{
    MyThing f, s;
    f.ReferenceType = null;
    s.ReferenceType = null;

    f.Equals(s);  // Provede kontrolu pomocí Equals -> zavolá tedy f.ReferenceType.Equals(s.ReferenceType);
}
```

Je důležité poznamenat že referenční typ nemusí být obsažený přímo v porovnávané struktuře. Může také být zanořený hlouběji v hodnotovém objektu.

Další příklad ukazuje uživatelem definovanou strukturu přepisující `Equals`.

```csharp
public struct MyThing
{
    public MyThing2 ValueType;
}

public struct MyThing2
{
    public int ValueType;

    public override bool Equals(object obj){
        return false;
    }
}

public static void Main()
{
    MyThing f, s;
    f.ValueType = new MyThing2();
    s.ValueType = new MyThing2();
    f.ValueType.ValueType = 0;
    s.ValueType.ValueType = 0;
    f.Equals(s);  // Provede kontrolu pomocí Equals -> zavolá tedy f.ValueType.Equals(s.ValueType);
}
```

V tomto případě by také mohl hodnotový objekt přepisující `Equals` být zanořena hlouběji ve struktuře.

Bitová kontrola nemůže být provedena pro žádný typ přepisující Equals. Pokud by se bitová kontrola provedla i v tomto případě mohla by nastat zvláštní situace:

```csharp
public struct MyThing
{
    public object MyObject;
}

public static void Main()
{
    MyThing f, s;
    f.MyObject = new object();
    s.MyObject = new object();
    f.MyObject.Equals(s.MyObject) //false jelikož objekty ukazují na odlišná místa v paměti
    f.Equals(s);  //pokud by se provedla bitová kontrola výsledek by byl true
    //Fieldy struktur tedy nejsou ekvivalentní ale struktury jsou ekvivalentní.
}
```

Stejný problém  nastane i pokud struktura obsahuje strukturu přepisující `Equals`.

## Bug v .NET Frameworku

``Float`` a ``double`` jsou výjmečné datové typy, jelikož obsahují hodnoty 0.0 a -0.0, které jsou ekvivalentní, ale mají rozdílnou bitovou reprezentaci. Tato vlastnost může způsobit bug v chování ``ValueType.Equals``. Pokud .NET použije porovnání pomocí bitů a struktura obsahuje ``float`` nebo ``double``, může se stát, že výsledek porovnání bude chybný.

```csharp
public struct MyThing
{
    public float MyFloat;
}

public static void Main()
{
    MyThing f, s;
    f.MyFloat = 0.0f;
    s.MyFloat = -0.0f;

    f.MyFloat.Equals(s.MyFloat) //True 0.0 se rovná -0.0
    Console.WriteLine(f.Equals(s));  // vypíše False, jelikož se provede bitová kontrola
}
```

Přidání referenčního typu do struktury vynutí porovnání pomocí Equals což změní výsledek porovnání:

```csharp
public struct MyThing
{
    public float MyFloat;
    public object obj;
}

public static void Main()
{
    MyThing f, s;
    f.MyFloat = 0.0f;
    s.MyFloat = -0.0f;
    f.obj = new object();
    s.obj = f.obj;
    Console.WriteLine(f.Equals(s));  // vypíše True
}
```

Stejný problém by nastal i při přidání struktury přepisující Equals.

Chybné chování je opraveno v .NET Core. .NET Core ``ValueType.Equals`` nejdříve zkontroluje zda typ neobasuje žádný `float` nebo `double` před použitím bitové kontroly.

.NET Core ``ValueType.Equals(object obj)`` vypadá následovně:

```csharp
public bool Equals(object obj){
    if(obj == null) return false;

    //Pokud je obj odlišného typu než this -> return false;
    if(!SameType(this, obj)) return false;

#region NewDotNetCorePart

    //Pokud this obsahuje double nebo float
    if(ContainsDoubleOrFloat(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);
    //Pokud některý ze zanořených fieldů obsahuje double nebo float
    if(InnerFieldContainsDoubleOrFloat(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

#endregion

    //Pokud this obsahuje referenční typ
    if(ContainsReferenceType(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

    //Pokud některý ze zanořených fieldů obsahuje referenční typ
    if(InnerFieldContainsReferenceType(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

    //Pokud některý ze zanořených fieldů přepisuje metodu Equals
    if(InnerFieldOveridesEquals(this))
        //Zavolej Equals na všech vnořených fieldech a pokud všechny vrátí true, vrať true
        return CallEqualsOnAllInnerFields(this, obj);

    //Zkontroluj, zda jsou bity stejné pro this a obj
    return PerformBitwiseCheck(this, obj);
}
```

## Shrnutí

`ValueType.Equals` prochází všechny fieldy a volá na nich `Equals`. Pokud jsou všechny ekvivalentní tak je výsledek `true`. V některých případech .NET používá optimalizaci která bitově porovná obě struktury. .NET Framework obsahuje bug který způsobuje chybné chování Equals pro `float` a `double`. Net Core tento bug opravuje.
