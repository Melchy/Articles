# Jak funguje Struct Equals

Každý objekt v C# dědí od bázové třídy ``Object``, která definuje metodu Equals sloužící k porovnávání objektů.
Defaultní implementace této metody se chová stejně jako ``Object.ReferenceEquals(Object a, Object b)``. Porovnává
tedy, zda reference objektů ukazují na stejné místo v paměti.

Tento způsob ekvivalnce je vhodný, pouze pokud porovnáváme objekty předáváné referencí.
Pokud objekty předáváme pomocí kopírování, nemá význám porovnávat reference, jelikož vždy ukazují na
nově vytvořenou kopii objektu. Jsou tedy vždy ``false``.

```csharp
//struct se v C# předává kopírováním
struct A{}

var a = new A();
var v = a; //vytvoří kopii, která se uloží na jiné místo v paměti
Console.WriteLine(Object.ReferenceEquals(a,v)); //False
```

<!---
Tato implementace
je definována ve třídě [ValueType](https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/src/System/ValueType.cs).
-->

Z tohoto důvodu hodnotové objekty (struktury) přepisují chování Object.Equals.
Algoritmus, který porovnává hodnotové typy, funguje tak, že najde všechny fieldy porovnávaného typu a zavolá na nich Equals.
Pokud je výsledek všech těchto porovnání ``True``, tak jsou oba hodnotové objekty ekvivalentní.
Toto porovnávání je ale poměrně pomalé a proto se v některých případech použije bitová kontrola.
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

Několik následujících příkladů ukazuje popsané chování. První příklad ukazuje nejjednodušší případ kdy struktura obsahuje
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
Je důležité poznamenat že referenční typ nemusí být 
obsažený přímo v porovnávané struktuře. Může také být zanořený hlouběji v hodnotovém objektu.

Další příklad ukazuje uživatelem definovanou strukturu přepisující Equals.

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
V tomto případě by také mohl hodnotový objekt přepisující Equals být zanořena hlouběji ve struktuře. 


Z toho algoritmu plyne, že bitovou kontrolu nemůžeme provést pokud:

1. Hodnotový typ obsahuje referenční typ --- tyto typy není možné porovnávat bitově, jelikož definice referenčních objektů říká, že dva objekty jsou si rovny, pokud se rovnají jejich reference.
Mohlo by se tedy stát, že by se dva objekty rovnaly bitově, ale přesto by nebyly ekvivalentní.
2. Zanořený hodnotový typ definovaný uživatelem přepisuje Equals --- pokud by algoritmus použil porovnání bitů, ignoroval by uživatelem definovaný Equals algoritmus.
Což by mohlo způsobit bug.

## Bug v .NET Frameworku

``Float`` a ``double`` jsou výjmečné datové typy, jelikož obsahují hodnoty 0.0 a -0.0, které jsou ekvivalentní, ale mají rozdílnou bitovou reprezentaci.
Tato vlastnost může způsobit bug v chování ``ValueType.Equals``. Pokud .NET použije porovnání pomocí bitů a struktura obsahuje ``float``
nebo ``double``, může se stát, že výsledek porovnání bude chybný.

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

    Console.WriteLine(f.Equals(s));  // vypíše False, jelikož se provede bitová kontrola
}
```

Další problém nastane při přidání referenčního typu do struktury. Přidání referenčního typu způsobí, že se použije kontrola pomocí Equals, což způsobí,
že výsledek bude správný.

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
    f.obj = null;
    s.obj = null;
    Console.WriteLine(f.Equals(s));  // vypíše True
}
```

Odstranění referenčního typu ze struktury tedy může způsobit chybné chování aplikace. Tento bug je naštěstí přítomný pouze v .NET Frameworku.
NET Core upravilo chování ``ValueType.Equals`` tak, aby se zkontrolovalo, zda typ neobasuje žádný float nebo double před použitím bitové kontroly.

.NET Core ``ValueType.Equals(object obj)`` pak vypadá následovně:

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

Defaultní implmenetace ValueType.Equals v .NET Frameworku by měla být vždy přepsána jelikož obsahuje bug který může způsobit neočekávané chování aplikace. .NET Core tento bug opravuje a díky tomu je vhodné tuto implementaci používat ve většině případů. Problém může nastat u výpočetně intenzivních aplikací pro které může použití reflexe znamenat značné zpomalení. Toto zpomalení ale nemusí být ve skutečnosti problém díky optimalizaci pomocí bitové kontroly kterou .NET provádí. Ve výpočetně intezivních aplikací je tedy často vhodné nejdříve použít defaultní implementaci a až později optimalizovat místa ve kterých .NET vynutil porovnání pomocí reflexe.