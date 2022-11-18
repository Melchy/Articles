# Integrační testy a unit testy

//TODO uvod

## Unit testy jsou lepší než integrační testy

Unit testy jsou často preferovaným způsobem testovanání jelikož jsou - rychlejší, nevyžadují externí závistlosti (databáze atd.) a programátor je může jednodduše spustit bez jakéhokoliv setupu. Pokud jste někdy četli něco o testování je pravděpodobné že jste o těchto výhodách již slyšely. Unit testy ale mají i několik nevýhod o kterých se nedočtete až tak často.
Těmito nevýhodami jsou - nižší spolehlivost a vyšší cenna údržby.

Unit testy jsou méně spolehlivé jelikož obvykle testují jen samostatné části kódu a né celou aplikaci. Při psaní unit testů je tedy nutné značnou část aplikace otestovat ručně abychom 
měli jistotu že aplikace funguje.

Pro pochopení druhé nevýhody je potřeba vysvětlit pojem Fragile test.

## Fragile test

U testů můžeme pozorovat zajímavou vlastnost s názvem křehkost (anglicky fragility). Křehké testy jsou ty které se lehce rozbijí při změně kódu. V následujícím příkladu je první test
velice křehký:

```csharp

//Co vlastne tetno test testuje ??????????????????
    public class OrderService{

        public IPriceCalculationService priceCalculationService{get;set;}
        public IOrderRepository orderRepostiory{get;set;}
        public IUserService GetCurrentUser{get;set;}

        public OrderService(){
            //TODO inicializace
        }

        public void Checkout(Cart cart){
            var price = priceCalculationService.GetPrice(cart.Items);
            var user = UserService.GeCurrentUser();
            var order = CreateOrder(cart, user, price);
            orderRepository.SaveOrder(order);
        }

        public virtual Order CreateOrder(Cart cart, User user, Price price){
            //Create order from cart
        }
    }

    [Fact]
    public void FragileTestExample(){
        var item = new Item(){Name="Item"}
        var cart = new Cart(){Items = new List<Item>(){item}};
        var order = new Order(){Items = new List<Item>{item}};
        var user = new User(){Name="name"}
        var price = 100;
        var priceCalculationMock = new Mock<IPriceCalculationService>().Setup(x=> x.GetPrice(order.Items)).Returns(price);
        var orderRepositoryMock = new Mock<IOrderRepository>().Setup(x=> x.SaveOrder(order));
        var userService = new Mock<IUserService>().Setup(x=> x.GetCurrentUser()).Returns(name);
        var orderServicePartialMock = new OrderService(priceCalculationMock, orderRepositoryMock, userService);
        orderServicePartialMock.CallBase = true;
        orderServicePartialMock.Setup(x => x.CreateOrder(cart, user, price)).Returns(order);

        orderServicePartialMock.Object.Checkout(cart);

    }

```

Tento test (a kód) obsahuje mnoho chyb avšak v tuto chvíli je důležité se zaměřit na křehkost testu. Jakákoliv z následujících úprav povede k rozbití testu:

* Změna parametrů některého z mocků
* Změna return typu některého z mocků
* Přesun jakékoliv z namockovaných metod do jiné třídy.
* Změna konstruktoru order servisy.

Každá z těchto změn může rozbít test i přes to že kód funguje správně i po změně. Odborně se tomuto stavu říká False Positive (TODO ověřit) - testováný kód funguje ale test
hlásí že je někde chyba. Jiným příkladem by mohl být test na koronavirus který říká že pacient je pozitivní ale ve skutečnosti pacient koronu nemá. Jak můžete asi tušit
tak false positive stav je situace které se chceme vyhnout.

False positive testy se nemusí zdát jako velký problém do té doby než si uvědomíme že při refactoringu můžeme přesunout až desítky metod a každá taková změna může vést k rozbití
X testů.

False positive testy jsou tedy způsobeny vysokou křehkostí testů. V budoucích dílech si povíme o několika způsobech jak křehkost snížit avšak nyní je pro nás důležité že unti testy
mají vyšší křehkost než integrační testy. Integrační testy se nerozbijí pokud změníme return typ metody nebo pokud přidáme parametr konstruktoru. Integrační testy se rozbijí pouze
pokud změníme API/veřejné rozhraní aplikace (nebo testovanou funkcionalitu).

Dlouhodobně jsou tedy integrační testy levnější na údržbu jelikož vyžadují menší množství úprav při změně kódu.

## Shrnutí výhod a nevýhod integračních testů

Výhody - Dlouhodobě levnější (méně fragile), spolehlivější
Nevýhody - pomalé, vyžadují externí závistloti (databázi atd.), často vyžadují setup

Jsou tedy integrační testy lepší než unit testy?

## Kompromis

Jak můžeme vidět tak integrační i unit testy mají své výhody a nevýhody. Není tedy jednoduché říct který druh testů je lepší. Můžeme ale vytvořit nový druh testu který se snaží
použít to nejlepší z obou světů.

Tento nový druh testů nemá oficiální název (alespoň jsem o něm nikdy neslyšel) a proto je pro účel těchto článků pojmenuji Fast Integration Tests neboli rychlé integrační testy.

Rychlé integrační testy mají následuící vlastnosti:

* Jsou rychlejší než integrační testy ale pomalejší než unit testy.
* Jsou méně křehké než unit testy ale křehčí než integrační testy. Díky tomu jsou tedy levnější než unit testy.
* Vyžadují pouze minimální nebo žádný setup
* Vyžadují minimum externích závislostí
* Jsou spolehlivější než unit testy ale méně spolehlivé než integrační.

## Závěr

V tomto článku jsme se podívali na výhody a nevýhody integračních a unit testů. Naznačili jsme také jak by měl vypadat nový druh testu který se snaží brát to nejlepší z obou druhů
testů. Tyto testy nesou název Rychlé integrační testy a v příštím díle si je popíšeme podrobně.