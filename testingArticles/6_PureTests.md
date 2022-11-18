# Pure unit tests

V předchozích dílech jsme si vysvětlili že bychom měli využívat rychlé integrační testy namísto unit testů. V tomto článku se podíváme na situace ve
kterých je vhodné použít unit testy. Dále se také podíváme na několik vylepšení unit testů.

## Kdy použít unit testy

Rychlé integrační testy jsou spolehlivější a méně křehké než unit testy. Běžně se tedy snažíme celou apliaci pokrýt rychlími integračními testy a unit testům se úplně vyhnout.

Problém nastane pokud narazíme na situaci kdy aplikace obsahuje tolik kombinací vstupů že není možné je všechny pokrýt integračními testy. Například pokud máme v aplikaci 10 vrstev a každá obsahuje čtyři větve kódu tak bychom museli napsat 4^10 testů abychom měli 100% pokrytí všech řádků. Asi nikdo nebude psát takové množství testů a proto musíme zvolit jinou techniku testování - unit testy.

Podrobnější vysvětlení combinatorial explosion problému můžete najít [zde](https://www.youtube.com/watch?v=V_-gKrxMUcw).

## Složitý kód

Aplikace téměř vždy obsahují dva druhy kódu - složitý kód a jednoduchý kód. Složitý kód obsahuje velké množství větvení (ifů) a jednoduchý kód je zbytek aplikace.
Například CRUD operace které pouze vezmou nějaká data a následně je uloží do databáze obsahují pouze jednoduchý kód. Příklad složitého kódu může být výpočet slevy na základě velkého množství faktorů jako jsou - věk uživatele, bydliště, předchozí nákupy atd.

Problém s combinatorial explosion se objevuje pouze ve složitých částech aplikace a proto je musíme testovat pomocí unit testů. Následující příklad demonstruje složitý kód.

```csharp
//TODO lepsi priklad
public async Task Cart{
    public void Checkout(){
        if(HappyHours){
            BuyItem(user, ItemFactory.CreateHappyHoursItem(id));
        }else{
            BuyItem(user, ItemFactory.CreateItem(id));
        }
    }
}

public async Task BuyItem(User user, Item itemToBuy){
    if(user.Age > 18){
        if(user.HasPremium && item.IsCheaperForPremiumUsers()){
            _paymentGateway.BuyItem(user, itemToBuy);
        }else{
            throw new InvalidOperationException("User can not buy item");
        }
    }else if(user.Age > 23){
        if(user.HasGoldCard){
            _paymentGateway.BuyItem(user, itemToBuy);
        }else{
            throw new InvalidOperationException("User can not buy item");
        }
    }else if(item.IsSuitableForOldPeople){
        _paymentGateway.BuyItem(user, itemToBuy);
    }else{
        throw new InvalidOperationException("User can not buy item");
    }
}
```

Nyní můžeme napsat unit testy na metodu `xxx` které otestují všechny větve této metody. Následně můžeme napsat ještě rychlý integrační test který otestuje okolní integrace.

## Pure unit tests

V předchozíhch dílech jsme si ukázali že unit testy jsou křehčí než integrační testy. Jedním z hlavních důvodů proč jsou unit testy křehčí je že využívají mocky které musíme opravovat pokaždé když měníme namockovanou třídu. Unit testy je ale možné napsat tak aby nepoužívaly mocky a tím se snížila jejich křehkost.

Unit testy bez mocků budu nazývat pure unit tests. Tento název vychází z funkcionálního programování kde se často používají "pure metody" což jsou metody které nemají žádné závislosti kromě svého vstupu.

Příkladem pure unit testu může být test metody Sum která sčítá dvě čísla:

//TODO priklad

V běžné aplikaci není možné většinu metod testovat pomocí pure testů jelikož obsaují volání jiných tříd a testy potřebují namockovat tato volání. Můžeme si ale pomoci a upravit kód tak aby neobsahoval žádná volání které by bylo potřeba namockovat.

Předchozí příklad vyžadoval mocky pro otestování složitého kódu. Následující příklad ukazuje jam můžeme tento kód upravit abycho mmohli použít pure unit testy.

```csharp
//TODO lepsi priklad
public class Cart{
    public async Task Checkout(){
        if(HappyHours){
            BuyItem(user, ItemFactory.CreateHappyHoursItem(id));
        }else{
            BuyItem(user, ItemFactory.CreateItem(id));
        }
    }

    public async Task BuyItem(User user, Item itemToBuy){
        if(IsUserEligibleToBuyItem(User user, Item itemToBuy)){
            _paymentGateway.BuyItem(user, itemToBuy);
        }else{
            throw new InvalidOperationException("User can not buy item");
        }
    }

    internal bool IsUserEligibleToBuyItem(User user, Item itemToBuy){
        if(user.Age > 18){
            if(user.HasPremium && item.IsCheaperForPremiumUsers()){
                return true;
            }
        }else if(user.Age > 23){
            if(user.HasGoldCard){
                return true;
            }
        }else if(item.IsSuitableForOldPeople){
            return true;
        }else{
            return false;
        }
    }
}
```

Metodu `IsUserEligibleToBuyItem` nyní můžeme otestovat bez použití mocků a obě větve `If(HappyHours)` můžeme otestova pomocí integračních testů.

Všiměte si že metoda IsUserEligibleToBuyItem by mohla být privale ale kvůli testům jsme ji museli udělat internal nebo public. Dalším problémem je že musíme vytvářet celý objekt `Cart` a předávat do jeho konstruktoru null hodnoty. Lepší možností je vyjmout celou logiku metody `IsUserEligibleToBuyItem` do samostatné třídy což vede k vyřešení setupu třídy `Cart`.

Pro použití pure testů je tedy nutné upravit kód tak aby obsahoval metody/třídy bez závislostí. Toho můžeme docílit přípravou dat před zavoláním metody a následným
předáním těchto dat pomocí parametrů. Metoda poté vrátí výsledek na základě kterého může systém provést další operace. Např. odeslat email nebo uložit data do databáze.

## Použití mocků

V některých situacích se může stát že transformace na pure test není možná. V těchto případech je nutné použít mockování. Příkladem takové situace je logování.
Logování obvykle nemůžeme odstranit z kódu a udělat ho pure - nemáme tedy jinou možnost než logger namockovat.

Je důležité poznamenat že situací kde je nutné použít mockování v unit testu není mnoho.

## Skupiny objektů

V některých případech není nutné testovat pouze jeden objekt. Může se stát že máme skupinu několika objektů která nemá žádné závislosti mimo danou skupinu.
Tyto malé skupiny je v některých případech vhodné testovat společně.

## Shrnutí

Unit testy bychom měli používat pouze pokud testujeme složité části aplikace a měli bychom pomožívat minimální množství mocků.

V příštím díle se podíváme na několik obecných typů jak zlepšit testy.