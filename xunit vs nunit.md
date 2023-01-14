# Xunit vs NUnit

Pokud vygooglíte "xUnit vs NUnit" najdete mnoho článků které považují xUnit za lepší testovací framework.
Následující seznam shrnuje nejčastější argumenty pro použití xUnitu:

1. xUnit chrání programátora před psaním chybných testů tím že vytváří novou instanci třídy pro každý spuštěný
   test. [1](https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/)[2](https://stackoverflow.com/a/40220724/5324847)
2. xUnit spouští testy paralelně narozdíl od NUnitu který testy spouští v
   serii [3](https://stackoverflow.com/a/39025623/5324847)
3. xUnit používá konstruktor a Dispose namísto `[SetUp]` and `[TearDown]`
   attributů [4](https://stackoverflow.com/a/33377029/5324847)[5](https://medium.com/@kylia669/xunit-vs-nunit-what-to-choose-to-write-clean-code-part-1-cb1a39ce0e8a)

Většina těchto argumentů je buď nepřesné nebo zastaralá. Pojďme je tedy po jednom rozebrat a uvést pravou míru.

## Vytváření instance pro každý test

NUnit od verze 3.13 dokáže vytvářet novou instanci pro každý test stejně jako xUnit.
Pro spuštění této funkcionality stačí do testovacího projektu přidat
attribut `[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]`.
Dokumentace [zde](https://docs.nunit.org/articles/nunit/writing-tests/attributes/fixturelifecycle.html)

## Paralelní spouštění testů

Nunit 3 i xUnit umějí spouštět testy paralelně.
Nunit navíc umožňuje podrobnější nastavení paralelizace testů.

### xUnit

V xUnitu se defaultně spouští testy v jedné třídě seriově. Testy ve více třídách se spouští paralelně.

Pokud chceme testy z více tříd spustit sériově tak je musíme přidat do stejné kolekce pomocí attributu
a `[CollectionDefinition("collection name")]`.

### NUnit

NUnit defaultně spouští všechny testy seriově a umožňuje použít attribut
`[assembly: Parallelizable(ParallelScope.Children)]` který spustí všechny testy v assembly paralelně.
Pokud chceme stejné chování jako v xUnitu tak můžeme použít`[assembly: Parallelizable(ParallelScope.Fixtures)]`.

Pokud nechceme aby běželi všechny testy v assembly paralelně tak můžeme testovacím třídám přidat attribut
`[Parallelizable(ParallelScope.Self)]` nebo `NonParallelizable` který
zajistí že daný test nikdy není spuštěn paralelně.

`Parallelizable` je možné využít mnoha dalšími způsoby a umožní vám nastavit téměř jakoukoliv paralelizaci.
Více o tomto atributu můžete najít
v [dokumentaci](https://docs.nunit.org/articles/nunit/writing-tests/attributes/parallelizable.html?q=Parallelizable).

## Constructor vs SetUp

V Nunit můžeme metody označit pomocí attributů `[SetUp]`, `[TearDown]`, `[OneTimeSetup]` a `[OneTimeTeardown]`.
Metody označené atributem `[OneTimeSetup]` a `[OneTimeTeardow]` se spustí jednou pro všechny testy v dané třídě (
přesněji v kolekci).
Metody označené atributem `[SetUp]` a `[TearDown]` se spustí před každým testem.

xUnit používá konstruktor a Dispose. Tyto metody se spouštějí pro každý test. Jsou tedy ekvivalentní k  `[SetUp]`
a `[TearDown]`.
Pokud potřebujeme jendorázový setup v xUnitu tak je nutné implementovat interface `IClassFixture`. Ukázka:

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

Čitelnost se dále zhoršuje v xUnitu pokud pokud máme setup a
teardown který je asynchroní. Příklad:

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

Jak vidíte xUnit potřebuje implementovat interface, naimplmentovat nové metody a přidat AsyncFixture. NUnit
vyžaduje pouze změnu metody na async.

## Podporované testovací atributy

[Zde](https://docs.nunit.org/articles/nunit/writing-tests/attributes.html) je výčet všech testovacích atibutů
podporovaných NUnitem. xUnit bohužel žádný takový
seznam nemá a nejlepší dostupný seznam je [tento](https://xunit.net/docs/comparisons).

## Dokumentace

Dokumentace xUnitu je mnohem horší než NUnitu.
Předchozí sekce poukazuje na tento problém a na githubu můžeme najít
další [issues](https://github.com/xunit/xunit/issues/1762) poukazující na téměř neexistující dokumentaci.

## Asserty

[Oficiální stránka](https://xunit.net/docs/comparisons) xUnitu porovnávající xUnit a NUnit
poukazuje na skutečnost že NUnit obsahuje mnohem větší množství assertů.
Dokumentace xUnitu ale opět neobsahuje žádný úplný výčet assertovacích metod tak že nemůžeme s
jistotou říct který framework je v tomto ohledu lepší.

Pro asertování bych ale doporučoval používat package [FluentAssertions](https://fluentassertions.com/) který předčí
NUnit i
xUnit.

## Závěr

Osobně se mi zdá že je opravdu obtížné najít nějaký aspekt ve kterém je xUnit lepší než NUnit.
