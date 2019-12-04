# Ukládání doménového modelu

[Doménový model](https://martinfowler.com/eaaCatalog/domainModel.html) přináší mnoho výhod při práci s doménou aplikace. Bouhužel ale není vhodný pro ukládání dat v databázi jelikož neumožňuje komplikované dotazování nad daty. Z tohoto důvodu se často provádí mapování mezi doménovým a databázovým modelem. Tento článek porovnává několik nejčastějších způsobů používaných pro ukládání doménového modelu.

## Dva modely

Programátor vytvoří v aplikaci dva modely - doménový a databázový. Doménový model může být velice složitý a nemusí odpovídat SQL databázi. Databázoví model naopak přesně odpovídá tabulkám SQL databáze. Ukládání probíhá tak že se doménového model namapuje na databázový a ten je poté uložen pomocí Entity Frameworku.

Podrobný popis dvou modelů je možné nalézt v [článku od Vladimira Khorikova](https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/). Vladimír zmiňuje že hlavní výhodou je čistota doménového modelu za kterou ale programátor zaplatí velkým množstvím kódu potřebného pro mapování. Tento kompromis se pak v praxi téměř nikdy nevyplatí.

## Použití NoSQL

NoSQL obsahuje dokumenty do kterých je možné uložit jakýkoliv JSON. Není tedy nutné model mapovat na relační schéma. Stačí pouze serializovat objekty a následně je uložit do dokumentů. NoSQL se tedy zdá být perfektním řešením. Problém ale nastává pří vytváření dotazů na databázi.

Jednou z velkých výhod SQL databází je možnost použití i velice komplexních dotazů. NoSQL je v tomto ohledu často velice omezená. Při použití NoSQL databáze v kombinaci s doménovým modelem se často stává že je potřeba získat data z více dokumentů zároveň. Čtení z více dokumentů ale není často podporované. Jednou z vyjímek je MongoDb.

MongoDb od verze 3.4 podporuje [lookup](https://docs.mongodb.com/manual/reference/operator/aggregation/lookup/) a [facet](https://docs.mongodb.com/manual/reference/operator/aggregation/facet/). Tyto operace umožňují dotazy nad více dokumenty. MongoDB tedy řeší jak ukládání tak dotazování nad daty.

## Použití ORM pro mapování modelů

Dalším řešením je nastavit ORM tak aby bylo schopné mapovat databázové tabulky na komplexní doménový model. Tento přístup není v .NET příliš rozšířený jelikož Entity framework nepodporuje komplexní mapování objektů (hlavně ve starších verzích). Jedinou existující altrenativou je ORM Nhybernate které podporuje poměrně komplexní mapování. Bohužel ale Nhybernate není příliš populární a vypadá spíše jako stále méně používaná technologie.

V některých případech je doménový model natolik složitý že ORM není schopné vyřešit mapování. V takových případech se často dělají ústupky které vedou k horšímu doménovému modelu. O tomto problému dále píše již zmiňovaný článek od [Vladimíra](https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/). Největším problémem problémem s ORM se ale zdá být jejich nízká [popularita](https://martinfowler.com/bliki/OrmHate.html).

## Event sourcing

Popis event sourcingu můžeme nají v [článku Martina Fowlera](https://martinfowler.com/eaaDev/EventSourcing.html). Fowler zminuje výhody i některé nevýhody a jejich řešení. Jako nevýhody zmiňuje práci s externími systémy, práci s časem a rušení eventů. Při práci s Event sourcingem můžeme najít ještě nekolik dalších nevýhod:

1. Migrace událostí - v některých případech je potřeba aktualizovat události na novější verzi. Řešením může být přidání verze k jednotlivím eventům, vytvoření kopie všech eventů a několik dalších řešení. Všechny řešení které existují mají své výhody a nevýhody. Neexistuje tedy žádný ideální způsob. Podrobnosti o tomto problému můžeme najít v [článku od Michiela Overeema](https://www.researchgate.net/publication/315637858_The_dark_side_of_event_sourcing_Managing_data_conversion). [Fowler](https://martinfowler.com/eaaDev/EventSourcing.html) také zmiňuje jedno z popsaných řešení a ukazuje jeho problémy s extreními systémy.

2. Tři modely - při použití event sourcingu v kombinaci s doménovým modelem je nutné pracovat se třemi modely. Model událostí, doménový model a model sloužící ke čtení. Události slouží k ukládání dat v systému. Doménový model je projekcí událostí a slouží k vytváření nových událostí. Poslední model je také projekcí událostí a slouží ke zobrazení dat uživateli. Tento model je nutný jelikož ani doménový model ani log událostí nejsou vhodné pro složité dotazy nad daty. Výsledek je tedy obvykle složitější než použití dvou modelů.

3. Malé množství infrastruktury - event sourcing je poměrně nový způsob ukládání doménových modelů a zatím neexistuje velké množství infrastruktury která by ho podporovala.

Celkově je event sourcing zajímavé řešení které ale podle mého názoru přináší větší množství problémů než užitku. Nehodí se tedy jako obecné řešení pro ukládání doménových modelů ale může mít užitek v některých aplikacích.

## State backed aggregate

Tento způsob vytváří doménové objekty které znají schéma databáze a slouží pouze jako obal databázového modelu. Při načítání objektů se z databáze získají všechny tabulky ve kterých má doménový objekt svá data. Doménový objekt poté používá tyto tabulky jako úložiště svých dat. Následující příklad demonstruje State backed aggregate:

```csharp
    public class User{
        public UserTable UserTable {get;set;}
        public AddressTable AddressTable {get;set;}

        public Cart Cart {get;set;}

        public User(){

        }

        //Konstruktor kterým je entita načtena z databáze
        public User(UserTable user, AddressTable addressTable){
            User = user;
            AddressTable = addressTable;
        }

        public ChangeAddress(Address newAddress){
            AddressTable.Street = newAddress.Street;
            AddressTable.City = newAddress.City;
        }
    }


    public class Cart{
        public CartTable CartTable {get;set;}

        public Cart(){
        }
        //Konstruktor kterým je entita načtena z databáze
        public Cart(CartTable cartTable){
            CartTable = cartTable;
        }
    }

```

Podrobnější popis tohoto přístupu je možné najít [zde](https://kalele.io/modeling-aggregates-with-ddd-and-entity-framework/)

State backed aggregate je zajímavé řešení jelikož nevyžaduje žádné mapování a zároveň umožňuje poměrně dobré vyjádření byznys logiky. V případech kdy je databáze velice odlišná od doménového modelu ale není tento přístup použitelný. Mapovací logika se pak začne dostávat přímo do agregátu což zkomplikuje celou byznys logiku.

## Ukládání Jsonu do SQL

PostgreSQL v nových verzích podporuje ukládání a dotazování nad JSON objekty. Je tedy možné vzít doménové objekty serializovat je a uložit do SQL jako JSON. [Vaughn Vernon napsal o tomto přístupu článek](https://kalele.io/the-ideal-domain-driven-design-aggregate-store/) ve kterém jako hlavní výhodu oproti MongoDB zmiňuje podporu atomických transakcí. Od napsání článu již ale uběhla nějaká doba a MongoDB již podporuje [transkace](https://docs.mongodb.com/manual/core/transactions/).

Aktuálně tedy ukládání doménového modelu v JSONu do SQL nemá téměř žádné výhody. Naopak můžeme najít jednu velkou nevýhodu. PostgreSQL má pouze limitovanou podporu dotazů nad JSON obekty. Například není možné provádět dotazy nad polem zanořeným v poly.

## Hybridní řešení s Entity Frameworkem

Jak už jsem zmínil Entity Framework neumožňuje mapování komplikovaných entit. Entity framework core 2 a 3 se ale v tomto ohledu velice zlepšil a umožnil mapování velkého množství jednodušších agregátů.
Se složitějšími případy si EF core nedokáže poradit ale v těchto případech můžeme použít ruční mapování bez ORM. Tímto přístupem nás entity framework zbaví vekého množství zbytečného mapování a zároveň se zbavíme komplikovaného ORM kódu.

Otázkou nyní je - jak často bude nutné použít ruční mapování. [Eric Evans v Effective aggregate design](https://dddcommunity.org/library/vernon_2011/) píše o případu kde 70% agregátů
obsahuje pouze kořenovou entitu a value objekty. Dále také naznačuje že běžná aplikace bude obsahovat podobné rozložení. Pokud má Eric Evans pravdu pak by pro mapování ve
většině případů měl stačit Entity framework.

O této technice neexistuje mnoho informací a proto v dalším článku ukáži jak nejlépe tento způsob implementovat. Zároveň se podíváme na další nevýhody tohoto přístupu. //TOD nevýhody