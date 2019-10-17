# Jak funguje Struct Equals

V C# rozlišujeme hodnotové a referenční typy. Hodnotové typy jsou definovány pomocí klíčového slova `struct` a referenční
typy jsou definovány pomocí `class`. Pro tento článek je důležité že hodnotové typy jsou předávány hodnotou a refrenční
typy jsou předávány referencí. Více informací o těchto způsobech předávání můžete nalézt [zde](https://www.mathwarehouse.com/programming/passing-by-value-vs-by-reference-visual-explanation.php).
Můžeme také rozlišovat uživatelem definované typy. Jsou to takové typy které nejsou defaultně v .NET ale
jsou definovány programátorem.

Všechny typy v C# dědí od bázové třídy ``Object``, která definuje metodu Equals sloužící k porovnávání objektů.
Defaultní implementace této metody se chová stejně jako ``Object.ReferenceEquals(Object a, Object b)``. Porovnává
tedy, zda reference objektů ukazují na stejné místo v paměti.

Tento způsob ekvivalnce je ale vhodný, pouze pokud porovnáváme objekty předáváné referencí.
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

Z tohoto důvodu hodnotové objekty přepisují chování Object.Equals.
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

Z toho algoritmu plyne že mohou nastat dva případy kdy není možné provést bitovou kontrolu.

První případ nastane pokud hodnotový typ obsahuje referenční typ. `Equals` referenčních typů říká že dva typy jsou stejné pokud ukazují na stejné místo v paměti. Porovnání pomocí bitové
kontroly tedy není možné jelikož neodpovídá definici `Equals` referenčních typů. Následující příklad ukazuje neintuitivní chování které by mohlo nastat při provedení
bitové kontroly pokud struktura obsahuje referenční typ.

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
    //Property struktur tedy nejsou ekvivalentní ale struktury samotné jsou.
}
```

Druhý případ kdy není možné použít bitovou kontrolu nastane pokud zanořený hodnotový typ definovaný uživatelem přepisuje `Equals`. Pokud by .NET v tomto případě použil porovnání bitů, ignoroval by uživatelem definovaný `Equals` algoritmus. Což by mohlo způsobit bug.

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

Další problém nastane při přidání referenčního typu do struktury. Přidání referenčního typu způsobí, že se použije kontrola pomocí `Equals`, což způsobí,
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
NET Core upravilo chování ``ValueType.Equals`` tak, aby se zkontrolovalo, zda typ neobasuje žádný `float` nebo `double` před použitím bitové kontroly.

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

Defaultní implmenetace ValueType.Equals v .NET Frameworku by měla být vždy přepsána jelikož obsahuje bug který může způsobit neočekávané chování aplikace. .NET Core tento bug opravuje a díky tomu je vhodné tuto implementaci používat ve většině případů. Problém může nastat u výpočetně intenzivních aplikací pro které může použití reflexe znamenat značné zpomalení. Toto zpomalení ale nemusí být ve skutečnosti problém díky optimalizaci pomocí bitové kontroly kterou .NET core provádí. Ve výpočetně intezivních aplikacích na .NET Core je tedy často vhodné nejdříve použít defaultní implementaci a až později optimalizovat místa ve kterých ;`Equals` algoritmus vynutil porovnání pomocí reflexe.
