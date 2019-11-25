# Ukládání doménového modelu

Jedním z hlavních problémů [DDD](https://en.wikipedia.org/wiki/Domain-driven_design) je ukládání [doménového modelu](https://martinfowler.com/eaaCatalog/domainModel.html). Doménový model ve většině případů neopovídá relačnímu modelu databáze a proto je potřeba provést jejich mapování. Problém mezi mapováním objektového modelu a relační databází se obecně nazývá [Object-relational impedance mismatch](https://en.wikipedia.org/wiki/Object-relational_impedance_mismatch). Object-relational impedance mismatch není triviální problém a existuje velké množství řešení které mají své výhody a nevýhody. V dalších částech se podíváme na 
výhody a nevýhody různých řešení tohoto problému.

## Dva modely

Prvním řešením je ruční mapování obou modelů. Typycky je deménový model ručně namapován na databázový model který přesně odpovídá tabulkám databáze. Databázový model je následně uložen pomocí ORM typycky Entity frameworku. V aplikaci tedy vznikají dva modely mezi kterými je vytvořeno mapování.

Výhody a nevýhody tohoto přístupu můžeme nálézt v [článku od Vladimira Khorikova](https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/).

Problémem dvou modelů je velké množství kódu který pouze přesouvá data z jednoho místa na druhé a ve výsledku nepřidává žadnout hodnotu aplikaci. Více informací o těchto problémech je možné nalézt [zde](https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/). Vladimír popisuje jak výhody tak i nevýhody tohoto přístupu. Kromě Vladimírem zmíněných výhod můžeme najít ještě další dvě:

1. Programátor potřebuje jen minimum znalostí - pro mapování komplexních doménových modelů jsou často potřeba velké znalosti použitého ORM.
2. Jednoduché migrace - v některých případech je potřeba zrefaktorovat agregáty a přesunou některá data do jiných agregátů. Tyto změny mohou být o něco jednodušší při použití dvou modelů jelikož nemusí být potřeba migrovat data v databázi.

## Použití NoSQL

Dalším způsobem jak vyřešit object-relational impedance mismatch je ukládání doménového modelu do Dokumentové databáze. Způsob ukládání dat do NoSQL je velice vhodný pro doménový model jelikož jednotlivé agregáty mohou být serializovány a uloženy do dokumentů. Tímto přístupem zajistíme že není potřeba žádné mapování pouze serializace a deserializace. Problém s dokumentovou databází nastává při zobrazování dat uživateli. Při zobrazování dat je často potřeba získívat data z více agregátů. Dokumentové databáze ale běžně neumožňují čtení dat z více dokumentů (agregátů) zároveň. Dokumentová databáze tedy řeší mapování doménového modelu neřeší ale dotazovaní nad daty.

MongoDb od verze 3.4 podporuje [lookup](https://docs.mongodb.com/manual/reference/operator/aggregation/lookup/) a [facet](https://docs.mongodb.com/manual/reference/operator/aggregation/facet/). Tyto operace umožňují dotazy nad více dokumenty. Pokud by kombinace dokumentů pomocí lookup byla příliš pomalá můžeme použít [index](https://medium.com/dbkoda/coding-efficient-mongodb-joins-97fe0627751a).

## Použití ORM pro mapování modelů

Dalším řešením je nastavit ORM tak aby bylo schopné mapovat databázové tabulky na komplexní doménový model. Tento přístup není v .NET příliš rozšířený jelikož Entity framework nepodporuje komplexní mapování objektů (hlavně ve starších verzích). Jedinou existující altrenativou je ORM Nhybernate které podporuje poměrně komplexní mapování. Bohužel ale Nhybernate není příliš populární a vypadá spíše jako stále méně používaná technologie.

ORM řeší velké množství  mapovnání v některých případech je ale doménový model natolik složitý že ORM není schopné vyřešit mapování. V takových případech se často dělají ústupky které vedou k horšímu doménovému modelu. Dalším problémem je složitý mapovací kód. Mnoha případech je potřeba napsat složitý mapovací kód který nemusí být jednoduché pochopit. Největším problémem ORM se ale zdá být jejich nízká [popularita](https://martinfowler.com/bliki/OrmHate.html).

## Event sourcing

Popis event sourcingu můžeme nají v [článku Martina Fowlera](https://martinfowler.com/eaaDev/EventSourcing.html). Fowler zminuje výhody i některé nevýhody a jejich řešení. Jako nevýhody zmiňuje práci s externími systémy, práci s časem a rušení eventů. Při práci s Event sourcingem můžeme najít ještě nekolik dalších nevýhod:

1. Migrace událostí - v některých případech je potřeba aktualizovat události na novější verzi. Řešením může být přidání verze k jednotlivím eventům, vytvoření kopie všech eventů a několik dalších řešení. Všechny řešení které existují mají své výhody a nevýhody. Neexistuje tedy žádný ideální způsob. Podrobnosti o tomto problému můžeme najít v [článku od Michiela Overeema](https://www.researchgate.net/publication/315637858_The_dark_side_of_event_sourcing_Managing_data_conversion). [Fowler](https://martinfowler.com/eaaDev/EventSourcing.html) také zmiňuje jedno z popsaných řešení a ukazuje jeho problémy s extreními systémy.

2. Tři modely - při použití event sourcingu v kombinaci s doménovým modelem je nutné pracovat se třemi modely. Model událostí, doménový model a model sloužící ke čtení. Události slouží k ukládání dat v systému. Doménový model je projekcí událostí a slouží k vytváření nových událostí. Poslední model je také projekcí událostí a slouží ke zobrazení dat uživateli. Tento model je nutný jelikož ani doménový model ani log událostí nejsou vhodné pro složité dotazy nad daty.

3. Immidiate consistency - zeptat se michala.

4. Malé množství infrastruktury

Celkově je event sourcing zajímavé řešení které ale přináší větší množství problémů než užitku. Nehodí se tedy jako obecné řešení pro ukládání doménových modelů ale může mít užitek v některých aplikacích.

## State backed aggregate

Tento způsob vytváří agregáty které znají schéma databáze a slouží pouze jako jejich obal. Při načítání agregátu se z databáze získají všechny tabulky ve kterých má agregát svá data a uloží se jako properta agregátu. Agregát poté používá tyto tabulky jako úložiště svých dat. Následující příklad demonstruje State backed aggregate:

```csharp
    public class User{
        public UserTable UserTable {get;set;}

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

V případě Entity Frameworku je možné použít [OwnedEntity](https://docs.microsoft.com/cs-cz/ef/core/modeling/owned-entities) pro modelování hodnotových objektů uvnitř stavového objektu. Podrobnější popis tohoto přístupu je možné najít [zde](https://kalele.io/modeling-aggregates-with-ddd-and-entity-framework/)

State backed aggregate je velice elegantní řešení jelikož nevyžaduje žádné mapování a zároveň umožňuje poměrně dobré vyjádření byznys logiky. V případech kdy je databáze odlišná od doménového modelu není tento přístup příliš použitelný. Mapovací logika se pak začne dostávat přímo do agregátu což zkomplikuje celou byznys logiku.

## Ideal aggregate store

V článku [The Ideal Aggregate Store?](https://kalele.io/the-ideal-domain-driven-design-aggregate-store/) popisuje Vaughn Vernon ukládání agregátů do SQL ve formátu JSON. PostgreSQL v nových verzích podporuje ukládání a dotazování nad JSON objekty. Vernon jako hlavní výhodu oproti dokumentovým databázím zmiňuje podporu atomických transakcí nad více agregáty. Od napsání článu již ale uběhla nějaká doba a MongoDB již podporuje [transkace](https://docs.mongodb.com/manual/core/transactions/).

Aktuálně tedy ukládání agregátů v JSONu do SQL nemá téměř žádné výhody a navíc jsou dotazy limitovány Postgre databází. Postgre například neumožňuje dotazy nad polem zanořeným v poly. Celkově se tedy zdá MongoDb lepší něž použití PostgreSQL.

## Hybridní řešení s Entity Frameworkem

Jak už jsem zmínil entity framework neumožňuje mapování komplikovaných entit. Entity framework core 2 a 3 se ale v tomto ohledu velice zlepšil a umožnil mapování velkého množství jednodušších agregátů.
Se složitějšími případy si EF core nedokáže poradit ale v těchto případech můžeme použít ruční mapování bez ORM. Tímto přístupem nás entity framework zbaví vekého množství zbytečného mapování a zároveň se zbavíme komplikovaného ORM kódu.

Otázkou nyní je - jak často bude nutné použít ruční mapování. [Eric Evans v Effective aggregate design](https://dddcommunity.org/library/vernon_2011/) píše o případu kde 70% agregátů
obsahuje pouze kořenovou entitu a value objekty. Dále také naznačuje že běžná aplikace bude obsahovat podobné rozložení. Pokud má Eric Evans pravdu pak by pro mapování ve
většině případů měl stačit Entity framework.

O této technice neexistuje mnoho informací a proto v dalším článku ukáži jak nejlépe tento způsob implementovat. Zároveň se podíváme na další nevýhody tohoto přístupu. //TOD nevýhody