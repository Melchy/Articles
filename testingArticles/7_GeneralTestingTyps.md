# General testing typs

V minulích dílech jsme si pospali unit testy a integrační testy v tomto díle se podíváme na několik triků které dále vylepší naše testy.

## Pojmenování testů

Pokud používáte klasické pojmenovávání [MethodUnderTest]_[Scenario]_[ExpectedResult] tak bych doporučil [článek](https://enterprisecraftsmanship.com/posts/you-naming-tests-wrong/)
od Vladimira Khorikova. V tomto článku Vladimír popisuje lepší způsob pojmenovávání testů - běžným člověkem čitelné věty. Jako příklad Vladimír uvádí test s názvem
IsDeliveryValid_InvalidDate_ReturnsFalse který můžeme lépe pojmenovat Delivery_with_invalid_date_should_be_considered_invalid.

Na našich projektech testy pojmenováváme větami ale nepoužíváme podržítka v názvu. Test Delivery_with_invalid_date_should_be_considered_invalid bychom tedy pojmenovali
DeliveryWithPastDateIsInvalid. Tento způsob jsme zvolili jelikož v některých případech nebylo na první pohled jasné zda programátor používá [MethodUnderTest]_[Scenario]_[ExpectedResult]
nebo běžné věty.

## Factory methods

Při psaní testů často narazíte na problém kdy se změní konstruktor některého objektu. Oprava všech testů které tento objekt vytváří je poměrně zdlouhavá práce. Z tohoto důvodu
je vhodné všechna volání konstruktoru schovat do metod s nepovinnými parametry. Příklad:

```csharp
[Test]
public void IncorectTest(){
    // Tímto způsobem bychom neměli uživatele vytvářet
    var user = new User("name", "surname", age:10);
    // test...
}

[Test]
public void TestWithFactoryMethod(){
    // správný způsob vytvoření uživatele
    var user = CreateUser("name", "surname", 10);
    //test...
}

public User CreateUser(string name = "John", string surname = "Smith", age = 10){
     return new User(name, surname, age);
}

```

Pokud nyní přidáme parametr do konstruktoru tak stačí přidat nepovinný parametr do factory metody a žádný z testů se nerozbije.

```csharp
public User CreateUser(string name = "John",
                       string surname = "Smith",
                       age = 10,
                       string title = "Mr")
{
     return new User(name, surname, age, title);
}
```

V některých případech je ale vhodné úmyslně použít konstruktor bez factory metody. Ukažme si test na změnu uživatelských dat:

```csharp
public void UserCanBeUpdated(){
    var createUserDto = CreateUserDto(name:"John", secondName: "Smith");
    var userId = await CreateUser(createUserDto);

    var updateUserDto = new UpdateUserDto(name:"UpdatedJohn", secondName:"UpdatedSmith");
    await UpdateUser(userId, updateUserDto);

    var userDto = await GetUser(userId);
    userDto.Name.Should.Be("UpdatedJohn");
    userDto.SecondName.Should.Be("UpdatedSmith");
}
```

Všiměte si že při vytváření UpdateUserDto jsme použili konstruktor. Pokud nyní upravíme konstruktor UpdateUserDto tak se test rozbije. To povede
programátora k nalezení testu který by měl upravit tak aby testoval nově přidaný parametr v konstruktoru.

CreateUserDto je vytvořeno pomocí factory metody protože cílem tohoto testu není testovat vytváření uživatele. Někde v aplikaci by pak existoval test na vytvoření uživatele a v něm by byl použit konstruktor objektu CreateUserDto.

## Kvalita kódu v testech

Testy by měli mít stejnou kvalitu kódu jako zbytek aplikace.

Pokud testy neudržujeme tak se stanou nečitelnými což značně stíží práci programátorů.

## Ukaž jen to co je nutné

V testech je důležité ukazovat čtenáři jen to co je nutné k pochopení testu. Pokud některá část není nutná k pochopení testu tak ji vyjmememe do metody.

Podrobné vysvětlení by bylo příliš dlouhé a proto jen odkáži na vynikající [přednášku](https://www.youtube.com/watch?v=qdSns9BOFrM) od Gerarda Meszaros.
Od času 6:43 až do konce přednášky Gerard ukazuje refactoring testu. V tomto refactoringu demonstruje mnoho skělých typů včetně skrývání nepodstatných částí testu.

## Prepare metody

Předchozí přednáška obsahuje další důležitý typ a tím jsou prepare metody. Prepare metoda je něco co skrývá složitý setup tak abychom
zakrili nepodstatné části testu.

Předchozí příklad s updatem uživatele by mohl být upraven následujícím způsobem:

```csharp
public void UserCanBeUpdated(){
    var userId = await PrepareUser(name:"John", secondName: "Smith"); // tento řádek je změněn
    var updateUserDto = new UpdateUserDto(name:"UpdatedJohn", secondName:"UpdatedSmith");
    await UpdateUser(userId, updateUserDto);

    var userDto = await GetUser(userId);

    userDto.Name.Should.Be("UpdatedJohn");
    userDto.SecondName.Should.Be("UpdatedSmith");
}
```

Vytváření uživatele je v tomto testu naprosto nepodstatné a proto ho schováme do metody a předáme jí pouze ty parametry které nás zajímají dále v testu.
Při vytvářžení uživatele nás zajímá pouze jméno a příjmení aby si čtenář uvědomil že proběhla změna.

## Buildery

V některých případech může být setup velice složitý. V těchto případech je obvykle jednoduchý preparer nedostačující a potřebujeme něco lepšího - builder.
Builder je jednodušše builder pattern tak jak ho znáte z GOF.
Přiklad:

```csharp
public void UserCanBeUpdated(){
    var userId = CreateUser(name:"John")
                .WithCart(Get_Item(name:"item"))
                .WithPreviousOrder(order: Get_Order(Get_Item(name:"name")))
                .Build();
    //test...
}
```

## Arrange Act Assert

V testech většinou není nutné psát komentáře //arange //act a //assert. Obvykle stačí jednotlivé části oddělit jedním nebo dvěma práznými řádky
tak jak jsem je použil v předchozích příkladech.

## Sut

Je vhodné pojmenovat testovanou třídu Sut aby bylo na první pohled jasné která třída je testována.

## Společný setUp a teardown

Xunit umožňuje použití konstruktoru a metody dispose pro provedení setupu a teardownu který je společný pro všechny testy (Nunit používá attributy SetUp a Teardown).
Těmto metodám bychom se měli vyhýbat aby čtenář našeho testu nemusel hledat kde se vytváří proměné pro test. Mnohem čitelnější je na začátku všech testů zavolat
metodu která provede setup.

## Custom assertions

V některých případech se hodí vytvořit vlastní assert který zakryje nedůležité části testu. Custom asserty se ale z našich zkušeností vyplatí pouze pokud
se daný assert opakuje v mnoha testech. Více informací o custom assertech v [přednášce](https://www.youtube.com/watch?v=qdSns9BOFrM) od Gerarda.

## Nástroje pro testování

Pro ověření výsledků testů doporučujeme knihovnu Fluent Assertions. Fluent assertions nabízí velkou škálu assertovacích metod které mnoho jiných knihoven postrádá. Příkladem může být provnání [propert dvou objektů](https://fluentassertions.com/objectgraphs/). Mezi další výhody fluent assertions patří lepší čitelnost a hezčí chybové hlášky.

## V příštím díle

V příštím díle si ukážeme aplikaci která obsahuje testy implementované podle této série článků.