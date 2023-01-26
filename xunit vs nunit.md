# Xunit vs NUnit

Pokud vygooglíte "xUnit vs NUnit" najdete mnoho článků které považují xUnit za lepší testovací framework.
Následující seznam shrnuje nejčastější argumenty pro použití xUnitu:

1. xUnit chrání programátora před psaním chybných testů tím že vytváří novou instanci třídy pro každý spuštěný
   test. [1](https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/), [2](https://stackoverflow.com/a/40220724/5324847)
2. xUnit spouští testy paralelně narozdíl od NUnitu který testy spouští v
   serii [3](https://stackoverflow.com/a/39025623/5324847)
3. xUnit používá konstruktor a Dispose namísto `[SetUp]` and `[TearDown]`
   attributů [4](https://stackoverflow.com/a/33377029/5324847), [5](https://medium.com/@kylia669/xunit-vs-nunit-what-to-choose-to-write-clean-code-part-1-cb1a39ce0e8a)

Většina těchto argumentů je buď nepřesné nebo zastaralá. Pojďme je tedy po jednom rozebrat a uvést pravou míru.

## 1. Vytváření instance pro každý test

NUnit od verze 3.13 dokáže vytvářet novou instanci třídy pro každý test stejně jako xUnit.
Pro spuštění této funkcionality stačí do testovacího projektu přidat
attribut `[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]`.
Dokumentaci k tomuto atributu můžete najít [zde](https://docs.nunit.org/articles/nunit/writing-tests/attributes/fixturelifecycle.html).

## 2. Paralelní spouštění testů

Nunit i xUnit umějí spouštět testy paralelně. Pro nastavení paralelizmu ale používají velice odlišný
způsob a proto se blíže podíváme na tyto rozdíly

### xUnit

V xUnitu se testy defaultně spouští v jedné třídě sériově a testy ve více třídách (přesněji kolekcích) se spouští paralelně. Pokud chceme testy z více tříd spustit sériově tak je musíme přidat do stejné kolekce pomocí attributu
a `[CollectionDefinition("collection name")]`. Jiná nastavení paralelizmu jsou buď velice obtížná nebo přímo nemožná.

### NUnit

NUnit defaultně spouští všechny testy seriově a umožňuje použít attribut
`[assembly: Parallelizable(ParallelScope.Children)]` který spustí všechny testy v assembly paralelně.
Pokud chceme stejné chování jako v xUnitu (spouštět pouze kolekce paralelně) tak můžeme použít attribut`[assembly: Parallelizable(ParallelScope.Fixtures)]`.

Attribut `Parallelizable` je možné využít mnoha dalšími způsoby a umožní vám nastavit téměř jakoukoliv paralelizaci.
Více o tomto atributu můžete najít
v [dokumentaci](https://docs.nunit.org/articles/nunit/writing-tests/attributes/parallelizable.html?q=Parallelizable).

Nunit nabízí také další attribut s názvem `NonParallelizable` který
zajistí že daný test nikdy není spuštěn paralelně i přesto že jeho kolekce nebo assembly má nastaveno paralelní spuštění.


## 3. Constructor vs SetUp

V Nunit můžeme metody označit pomocí attributů `[SetUp]`, `[TearDown]`, `[OneTimeSetup]` a `[OneTimeTeardown]`.
Metody označené atributem `[OneTimeSetup]` a `[OneTimeTeardow]` se spustí jednou pro všechny testy v dané třídě (přesněji v kolekci).
Metody označené atributem `[SetUp]` a `[TearDown]` se spustí před každým testem.

XUnit používá konstruktor a `Dispose`. Tyto metody se spouštějí pro každý test. Jsou tedy ekvivalentní k  `[SetUp]`
a `[TearDown]`.
Pokud potřebujeme jendorázový setup v xUnitu tak je nutné implementovat interface `IClassFixture`:

```csharp

//------------------- XUNIT ------------------------------
public class MyDatabaseTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture fixture;

    public MyDatabaseTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }
    
    // ... write tests, using fixture.Db to get access to the SQL Server ...
}

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        Db = new SqlConnection("MyConnectionString");
    }

    public void Dispose()
    {
        Db.Dispose();
    }

    public SqlConnection Db { get; private set; }
}
```

V Nunitu bysme mohly stejný kód napsat následujícím způsobem:

```csharp
//------------------- NUNIT ------------------------------
public class MyDatabaseTests
{
    SqlConnection sqlConnection;
    
    [OneTimeSetup]
    public void SetUp(){
        sqlConnection = new SqlConnection("MyConnectionString");
    }

    [OneTimeTearDown]
    public void TearDown(){
        sqlConnection.Dispose();
    }
}
```

Osobně si myslím že syntaxe NUnitu je o něco čitelnější.

Čitelnost se dále v xUnitu zhoršuje pokud pokud máme setup a
teardown který je asynchroní:

```csharp
//------------------- XUNIT ------------------------------

// IClassFixture<DatabaseFixture> changed to IClassFixture<AsyncFixture<DatabaseFixture>>
public class MyDatabaseTests : IClassFixture<AsyncFixture<DatabaseFixture>>
{
    DatabaseFixture fixture;

    public MyDatabaseTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }
}

public class DatabaseFixture : IAsyncLifetime // additional interface
{
    // Xunit can not use ctor or dispose
    public async Task InitializeAsync(){
        // init
    }

    public async Task DisposeAsync(){
         // dispose
    }

    public SqlConnection Db { get; private set; }
}
```

Stejný kód v Nunitu:

```csharp
//------------------- NUNIT ------------------------------
public class MyDatabaseTests : IClassFixture<DatabaseFixture>
{
    SqlConnection sqlConnection;
    [OneTimeSetup]
    public async Task SetUp(){
        // init
    }

    [OneTimeTearDown]
    public async Task TearDown(){
        //dispose
    }
}
```

XUnit tedy vyžaduje implementovat interface, naimplmentovat nové metody a přidat `AsyncFixture`. NUnit
vyžaduje pouze změnu metody na async.

## Další nevýhody xUnitu

Dokumentace xUnitu je prakticky neexistující. Stránky xUnitu popisují několik základních scénářů ale nikde neexistuje
žádný úpný výčet assertovacích metod, testovacích attributů a dalších funkcí xUnitu. Na tyto problémy poukazuje i několik issues na 
githubu. Například [zde](https://github.com/xunit/xunit/issues/1762). NUnit má oproti tomu velice dobrou a přehlednou dokumentaci.

Téměř neexistující dokumentace xUnitu nám velice ztěžuje další porovnání frameworků. I přesto se pokusím porovnat další aspekty kterými
jsou -  testovací attributy, assertovací metod a test context.  

### Testovací attributy

Testovací attributy se používají ke konfiguraci testů. Nejznámějším je attribut `[Test]` v Nunitu a jeho ekvivalent `[Fact]` v xUnitu.
Tento attribut jedoduše označí které metody mají být spouštěny jako testy.

Xunit i Nunit nabízejí velké množství pokročilejších attributů jako je `[TestCase]`(NUnit) (`[InlineData]` (xUnit)) který umožnuje spustit 
jednu testovací metodu s různými parametery. 

[Zde](https://docs.nunit.org/articles/nunit/writing-tests/attributes.html) je výčet všech testovacích atributů
podporovaných NUnitem. xUnit bohužel žádný takový
seznam nemá a nejlepší dostupný seznam je [tento](https://xunit.net/docs/comparisons) který není ani zdaleka úpný.

Podle mého googlení a historických zkušeností to ale vypadá že xUnit podporuje mnohem menší množství testovacích attributů.
Bez úplného seznamu je ale obtížné udělat úplný závěr.

### Asserty

[Oficiální stránka](https://xunit.net/docs/comparisons) xUnitu porovnávající xUnit a NUnit
poukazuje na skutečnost že NUnit obsahuje mnohem větší množství assertů.
Dokumentace xUnitu ale opět neobsahuje žádný úplný výčet assertovacích metod tak že nemůžeme s
jistotou říct který framework je v tomto ohledu lepší.

Pro asertování bych ale doporučoval používat package [FluentAssertions](https://fluentassertions.com/) který předčí
NUnit i xUnit.

### Test context

Nunit nabízí statickou třídu s názvem [`TestContext`](https://docs.nunit.org/articles/nunit/writing-tests/TestContext.html) 
která obsahuje informace o aktuálně běžícím testu a také několik velice užitečných metod. Příkladem může být generátor
náhodných čísel který vždy generuje stejné hodnoty při opakovaném spuštění testu.

Nunit žádnou alternativu k `TestContext` třídě nenabízí.

## Závěr

Osobně se mi zdá že je opravdu obtížné najít nějaký aspekt ve kterém je xUnit lepší než NUnit.
