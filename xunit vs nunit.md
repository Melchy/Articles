# Xunit vs NUnit

Pokud vygooglíte "xUnit vs NUnit" najdete mnoho článků které považují xUnit za lepší testovací framework.
Následující seznam shrnuje nejčastější argumenty pro použití xUnitu:

1. xUnit chrání programátora před psaním chybných testů tím že vytváří novou instanci třídy pro každý spuštěný
   test. [1](https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/)[2](https://stackoverflow.com/a/40220724/5324847)
2. xUnit spouští testy paralelně narozdíl od NUnitu který testy spouští v
   serii [3](https://stackoverflow.com/a/39025623/5324847)
3. xUnit používá konstruktor a Dispose namísto `[SetUp]` and `[TearDown]`
   attributů [4](https://stackoverflow.com/a/33377029/5324847)[5](https://medium.com/@kylia669/xunit-vs-nunit-what-to-choose-to-write-clean-code-part-1-cb1a39ce0e8a)

Většina těchto argumentů je buď nepřesné nebo zastaralá. Pojďme je tedy po jednom rozebrat a uvést je na pravou míru.

## Vytváření instance pro každý test

Toto je zastaralý argument jelikož NUnit od verze 3.13 dokáže vytvářet novou instanci pro každý test stejně jako xUnit.
Pro
spuštění této funkcionality stačí do testovacího projektu přidat
attribut `[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]`.
Dokumentace [zde](https://docs.nunit.org/articles/nunit/writing-tests/attributes/fixturelifecycle.html)

## Paralelní spouštění testů

Nunit 3 i xUnit umějí spouštět testy paralelně. Druhý bod je tedy nepravdivý/zastaralý.
Nunit je dokonce flexibilitu ve spouštění paralelních testů než xUnit.

V xUnitu se všechny testy v jedné třídě vždy spouštějí seriově a neexistuje žádný způsob
jak spustit všechny testy paralelně. Pokud chceme spustit testy z několika tříd seriově musíme třídám přidat
attribut `[CollectionDefinition("collection name")]`.

NUnit defaultně spouští všechny testy seriově a umožňuje použít attribut
`[assembly: Parallelizable(ParallelScope.All)]` který spustí všechny testy v assembly paralelně.
Dále můžeme i některé třídy označit attributem `[Parallelizable(ParallelScope.Self)]` což způsobý že všechny testy v
dané třídě budou spuštěny seriově.

`Parallelizable` je velice mocný argument a obvykle umožňuje mnohem lepší nastavení než alternativy v xUnitu.
Více o atributu můžete najít
v [dokumentaci](https://docs.nunit.org/articles/nunit/writing-tests/attributes/parallelizable.html?q=Parallelizable).

## Constructor vs SetUp

V Nunit můžeme metody označit pomocí attributů `[SetUp]`, `[TearDown]`, `[OneTimeSetup]` a `[OneTimeTeardown]`.
Metody označené atributem `[OneTimeSetup]` a `[OneTimeTeardow]` se spustí jednou pro všechny testy v dané třídě (
přesněji v kolekci).
Metody označené atributem `[SetUp]` a `[TearDown]` se spustí před každým testem.

xUnit používá konstruktor a Dispose. Tyto metody se spouštějí pro každý test. Jsou tedy ekvivalentní k  `[SetUp]`
a `[TearDown]`.
Abychom docílili jendorázoví setup musíme v xUnitu implementovat interface IClassFixture. Ukázka rozdílu:

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

Osobně si myslím že syntaxe NUnitu je o něco čitelnější. Tento problém se ještě zhoršuje pro xUnit pokud máme setup a
teardown který je asynchroní.

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

Jak vidíte xUnit potřebuje implementovat interface, naimplmentovat nové metody a přidat AsyncFixture. NUnit naopak
vyžaduje pouze změnu metody na async.

## Podporované testovací atributy

[Zde](https://docs.nunit.org/articles/nunit/writing-tests/attributes.html) je výčet všech testovacích atibutů
podporovaných NUnitem. xUnit bohužel žádný takový
seznam nemá a nejlepší dostupný seznam je [tento](https://xunit.net/docs/comparisons).

## Dokumentace

Dokumentace xUnitu je mnohem horší než NUnitu.
Předchozí sekce poukazuje na tento problém a na githubu můžeme najít
další [issues](https://github.com/xunit/xunit/issues/1762) poukazující na tento problém.

## Asserty

Oficiální stránka porovnávající xUnit a NUnit poukazuje na skutečnost že NUnit obsahuje mnohem větší množství assertů.
Dokumentace xUnitu ale opět neobsahuje žádný úplný výčet tak že nemůžeme s jistotou říct který framework je v tomto
ohledu lepší.

Osobně bych ale doporučoval používat package [FluentAssertions](https://fluentassertions.com/) který předčí NUnit i
xUnit.

## Závěr

Zdá se že najít argument pro použití xUnitu je v dnešní době poměrně obtížné.
