# Ukládání doménového modelu

[Doménový model](https://martinfowler.com/eaaCatalog/domainModel.html) přináší mnoho výhod při práci s byznys logikou aplikace. Nejčastěji uváděnou výhodou je modelování byzyns logiky aplikace podle reálného světa. Například objekt `User` může obsahovat data jako jsou `Name`, `Address`. Zároveň muže obsahovat metodu `Move` která slouží k přestěhování uživatele na novou adresu. `User` objekt je pak jednodušše pochopitelný pro programátora jelikož připomíná uživatele v reálném světě. Díky této a dalším výhodám je doménový model skvélý přístup pro tvorbu složitější byznys logiky.

V programování existuje jen málo návrhových vzorů které nemají žádné nevýhody. V případě doménového modelu je nevýhodou ukládání do databáze a dotazování nad daty. Doménový model neodpovídá relačnímu schématu databáze a proto je často potřeba provést mapování mezi těmito modely. Mapování ovšem přináší velké množství kódu který pouze přesouvá data z jednoho místa na druhé. Existuje tedy lepší způsob než mapování mezi těmito modely? V tomto článku se podíváme na několik způsobů ukládání doménvového modelu a jejich výhody a nevýhody.

## Dva modely

Programátor vytvoří v aplikaci dva modely - doménový a databázový. Doménový model může být velice složitý a nemusí odpovídat SQL databázi. Databázoví model naopak přesně odpovídá tabulkám SQL databáze. Ukládání probíhá tak že se doménového model namapuje na databázový a ten je poté uložen pomocí ORM (obvykle Entity Frameworku).

Podrobný popis dvou modelů je možné nalézt v [článku od Vladimira Khorikova](https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/). Vladimír zmiňuje že hlavní výhodou je čistota doménového modelu za kterou ale programátor zaplatí velkým množstvím kódu potřebného pro mapování. Tento kompromis se pak v praxi téměř nikdy nevyplatí.

## Použití NoSQL

Problém s úkládáním doménového modelu vzniká jelikož objekty musejí být mapovány na relační schéma. Pokud ale použijeme databázi která nepoužívá relační schéma můžeme jednoduše objekty serializovat a uložit. NoSQL se tedy zdá být perfektním řešením. Problém ale nastává pří vytváření dotazů na databázi.

SQL umožňuje velké množství dotazů a filtrování napříč všemy tabulkami. NoSQL databáze jsou ale často v tomto ohledu velice omezené. Obvykle umožňují dotazy pouze nad zvoleným indexem nebo v případě dokumentové databáze nad jedním dokumentem. Při zobrazování dat uživateli je ale obvykle potřeba získávat data z celé databáze a poté je navíc filtrovat. Při použití NoSQL tedy můžeme pohodlně uložit data ale nemůžeme nad nimi  dělat složité dotazy tak jako u SQL databáze. Naštěstí ale ve světě NoSQL existují vyjímky které umožňují i složitější dotazy napříč databázý. Touto vyjímkou je například MongoDb které od verze 3.4 podporuje [lookup](https://docs.mongodb.com/manual/reference/operator/aggregation/lookup/) a [facet](https://docs.mongodb.com/manual/reference/operator/aggregation/facet/). Mongo je tedy jedno z nejlepších úložišť doménového modelu.

Další často zmiňovanou nevýhodou NoSQL databází je nepřítomnost atomockých transakcí. Zastánci NoSQL často tvrdí že transakce nejsou nutné ale obvykle se hodní hlavně při použití [outbox patternu](https://jimmybogard.com/life-beyond-transactions-implementation-primer/). Mongo naštěstí od verze 4 podporuje i atomocké transakce nad několika dokumenty. Řeší tedy všechny problémy NoSQL databází.

## Použití ORM pro mapování modelů

Dalším řešením je nastavit ORM tak aby bylo schopné mapovat databázové tabulky na komplexní doménový model. Tento přístup není v .NET příliš rozšířený jelikož Entity framework nepodporuje komplexní mapování objektů. Jedinou existující altrenativou je ORM Nhybernate které podporuje komplexní mapování. Bohužel ale Nhybernate není příliš populární a vypadá spíše jako stále méně používaná technologie.

Nevýhodou ORM je že v některých případech není schopné vyřešit mapování. Poté se často dělají ústupky které vedou k horšímu doménovému modelu. O tomto problému dále píše již zmiňovaný článek od [Vladimíra](https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/). Největším problémem ORM se ale zdá být jejich nízká [popularita](https://martinfowler.com/bliki/OrmHate.html).

## Event sourcing

Popis event sourcingu můžeme nají v [článku Martina Fowlera](https://martinfowler.com/eaaDev/EventSourcing.html). Fowler zminuje výhody i některé nevýhody a jejich řešení. Jako nevýhody zmiňuje práci s externími systémy, práci s časem a rušení eventů. Kromě Fowlerem popsaných nevýhod můžeme najít ještě několik dalších:

1. Migrace událostí - v některých případech je potřeba aktualizovat události na novější verzi. Tento problém je popsán v [článku Michiela Overeema](https://www.researchgate.net/publication/315637858_The_dark_side_of_event_sourcing_Managing_data_conversion) který zmiňuje několik řešení a jejich výhody a nevýhody. Fowler [v článku o event sourcingu](https://martinfowler.com/eaaDev/EventSourcing.html) zmiňuje transformaci eventů a její problém s extreními systémy.

2. Tři modely - při použití event sourcingu v kombinaci s doménovým modelem je nutné pracovat se třemi modely. Model událostí, doménový model a model sloužící ke čtení. Události slouží k ukládání dat v systému. Doménový model je projekcí událostí a slouží k vytváření nových událostí. Poslední model je také projekcí událostí a slouží ke zobrazení dat uživateli. Tento model je nutný jelikož ani doménový model ani log událostí nejsou vhodné pro složité dotazy nad daty. Výsledek je tedy obvykle složitější než použití dvou modelů.

3. Malé množství infrastruktury - event sourcing je poměrně nový způsob ukládání doménových modelů a zatím neexistuje velké množství infrastruktury která by ho podporovala.

Celkově je event sourcing zajímavé řešení které ale podle mého názoru přináší větší množství problémů než užitku. Nehodí se tedy jako obecné řešení pro ukládání doménových modelů ale může mít užitek v některých aplikacích.

## State backed aggregate

Tento způsob vytváří doménové objekty které znají schéma databáze a slouží pouze jako obal databázového modelu. Při načítání objektů se z databáze získají všechny tabulky ve kterých má doménový objekt svá data. Doménový objekt poté používá tyto tabulky jako úložiště dat. Následující příklad demonstruje State backed aggregate:

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

State backed aggregate je zajímavé řešení jelikož nevyžaduje žádné mapování a zároveň umožňuje poměrně dobré vyjádření byznys logiky. V případech kdy je databáze velice odlišná od doménového modelu ale není tento přístup použitelný. Mapovací logika se pak začne dostávat přímo do agregátu což zkomplikuje celou byznys logiku. Tento způsob je možné použít jako dočasné řešení a později migrovat na dva modely nebo Hybridní řešení s EF (popsané dále).

## Ukládání JSON do SQL

PostgreSQL v nových verzích podporuje ukládání a dotazování nad JSON objekty. Je tedy možné vzít doménové objekty serializovat je a uložit do SQL jako JSON. [Vaughn Vernon napsal o tomto přístupu článek](https://kalele.io/the-ideal-domain-driven-design-aggregate-store/) ve kterém jako hlavní výhodu oproti MongoDB zmiňuje podporu transakcí. Od napsání článu již ale uběhla nějaká doba a MongoDB již podporuje [transkace](https://docs.mongodb.com/manual/core/transactions/).

Aktuálně tedy ukládání doménového modelu v JSONu do SQL nemá téměř žádné výhody. Naopak můžeme najít jednu velkou nevýhodu. PostgreSQL má pouze limitovanou podporu dotazů nad JSON obekty. Například není možné provádět dotazy nad polem zanořeným v poly.

## Hybridní řešení s Entity Frameworkem

Jak už jsem zmínil Entity Framework neumožňuje mapování komplikovaných entit. Entity framework core 2 a 3 se v tomto ohledu velice zlepšil a umožnil mapování velkého množství jednodušších objektů.
U složitějších případů pak můžeme použít ruční mapování bez ORM.

Otázkou je jak často bude nutné použít ruční mapování. [Eric Evans v Effective aggregate design](https://dddcommunity.org/library/vernon_2011/) píše že v běžné aplikaci by cca 70% aggregátů mělo být jednoduchých. Pokud má Eric Evans pravdu pak by pro mapování ve většině případů měl stačit Entity framework.

Nevýhodou hybridního řešení je že projekt obsahující byznys logiku musí obsahovat i databázové entity. Další nevýhodou je o něco méně čistý doménový model jelikož v některých případech musí obsahovat odkazy na databázové tabulky.

O hybridním řešení neexistuje mnoho informací a proto si v dalším článku ukážeme implementaci tohoto přístupu.

## Shrnutí

Pro ukládání doménového modelu je možné použít mnoho způsobů. Častým řešením je použití dvou modelů mezi kterými je provedeno mapování. Dva modely obvykle přinášejí velké množství kódu který poze přesouvá data. Z tohoto důvodu je lepší použít ORM nebo NoSQL databázi.
