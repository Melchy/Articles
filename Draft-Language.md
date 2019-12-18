Metafory z realneho sveta
Evolving language - priklad s kosikem
Přesné názvy/pojmenování

# Pojmenovávání v programování

V pravidlech čistého kódu můžeme najít informaci že pojmenování by mělo být raději dlouhé a jednoznačné než krátké a nepřesné. Napříkald namísto Divide(x, y) bychom měli použít Divide(dividend, divisor). Jednoduché a zřejmé. Toto pravidlo ale bohužel není tak jednoduché aplikovat v reálném světě kde neprogramujeme ty nejtriviálnější příklady s dělením.

Reálnějším příkladem může být objekt který vyjadřuje uživatelem s přístupem ke skladu a zároveň k administraci. Pokud použijeme dlouhý a přesný název mohly bychom objekt pojmenovat - UserWithAccessToWarehouseAndAdministration. Pro čtenáře našeho kódu je nyní naprosto zřejmé co tento název vyjadřuje. Bohužel ale není jednoduché o takové proměnné mluvit s jinými programátory.

Příklad:
X: Co se má stát když *uživatel s přístupem ke skladu a k administraci* přistoupí do uživatelské sekce?

Obvykle v takovéto situaci začneme název zkracovat například na uživatele. O kterého uživatele se jedná poté poznáme podle kontextu. Zkracování názvů při rozhovorech ale vede k několika problémům:

1. V některých příkladech může dojít k nedorozumění.

2. Konverzace je obtížnější jelikož je potřeba si stále uvědomovat kontext.

3. Navázat na předchozí konverzaci je obtížné. Například otázka "pamatuješ si jak jsme se bavili o tom uživateli?". Bude velice pravděpodobně následována otázkou "O kterém?".

Dlouhé názvy jsou tedy přesné ale často zhoršují konverzaci mezi programátory. Řešením může být pojmenování dlouhých názvů. Například *uživatel s přístupem ke skladu a k administraci* by mohl být *hlavní skladník* - MainWarehouseman. Toto pojmenování zjednodušší konverzaci a kód pokud všichni v týmu vědí že *hlavní skladní* má právo přistupovat ke skladu a k administraci. Jak by ale měl programátor znalosti o hlavním skladníkovy získat. Další otázkou je proč zrovna název *hlavní skladník* a né nějaký jiný název. Tyto dvě otázky spoulu úzce souvisý a něž je zodpovím tak nachvíli odbočím.

V jednom z velkých e-shopů se zavedla zkratka PERA jako název pro skladový systém a několik dalších subsystémů. Po mnoha letech a mnoha změnách v týmu se zjistilo že nikdo vlastně neví jaké slovo či větu má zkratka PERA představovat. Toto zjištění ale překvapivě nikomu nevadilo. PERA byl tak zaběhnutý název že všicni věděli o kterou část systému se jedná a nikdy nebylo potřeba uvádět celý text této zkratky.

PERA byl tedy náhodně vybraný název který ale fungoval velice dobře. Umožnil zjednodušení kódu a diskuzí. Všichni v týmu věděli co PERA vyznačuje a nový členové velice rychle pochopily o co jde.

Jaké je tedy ponaučení z tohoto příběhu? Měly bychom vždy používat vymyšlené názvy namísto dlouhých a přesných názvů? Pravděpodobně ne. Náhodé názvy fungují poměrně dobře dokud jich v systému není velké množství. Pokud máme takových názvů v systému jen pár může nám to velice zjednodušit kód a komunikaci v týmu. Dále je také vhodné mít tyto názvy někde sepsané spolu s jejich jednoduchým popisem.

Náhodné názvy bychom také měli použivat pouze jako poslední možnost. Ve většině případů je vhodné využít jeden z následujících způsobů pojmenovávání:

1. Názvy založené na obecných znalostech.

2. Názvy existující v doméně.

3. Názvy podobající se konceptu který se snaží vyjádřit.

//TODO nekde tady by bylo dobre vyjadrit dilema -> bud mas presny nazev ktery obsahuje mnoho informaci nebo kratky nazev ktery obsahuje malo informaci.

## Názvy založené na obecných znalostech

Na začátku tohoto článku byl uveden příklad s dělením - namísto Divide(x, y) je lepší použít Divide(dividend, divisor). Následně byl uveden stejný příklad z reálného světa *uživatel s přístupem ke skladu a k administraci* by se měl jmenovat *UserWithAccessToWarehouseAndAdministration*. Problémem ale je že tyto dva příklady jsou velice odlišné.

Metoda Divide využívá předchozí znalosti programátora o matematice. Předpokládá že programátor ví co je to dividend a divisor. Naopak příklad s uživatelem nevyžaduje žádné předchozí znalosti programátora. Všechny relevantní inforamce o objektu jsou obsaženy v jejím názvu. Pokud bychom tedy chtěli tyto dva příklady opravdu ekvivalentní museli bychom metodu Divide změnit na Divide(numberThatWillBeDivided, numberThatWillDivide). Nyní metoda Divide také nevyžaduje žádné předchozí znalosti matematiky (kromě definice dělení samotného).

Proč tedy pojmenovávat dělení Divide(dividend, divisor) namísto Divide(numberThatWillBeDivided, numberThatWillDivide)? Obecně očekáváme že programátor má znalosti matematiky a díky tomu informace o významu proměnných nemusejí být v názvu proměnné. Díky tomu získáme krátké názvy a náš tým se nemusí učit nic nového. Získáme tedy to nejlepší z obou světů.

> Při pojmenovávání vždy používejte předchozí znalosti programátorů.

Je důležité si uvědomit že názvy založené na obecných znalostech využíváme poměrně běžně bez přemýšlení. Uvedu ještě dva příklady aby bylo zřejmé toto časté použití:

1. Namísto `Person.GetTimeSinceBorn()` můžeme použít `Person.GetAge()`. Využili jsme obecně známý koncept 'věk'.
2. Namísto `Car.IncreaseSpeed()` můžeme použít `Car.Accelerate()`. Využili jsme základní znalosti fyzyky.

## Názvy existující v doméně

Doména aplikace je oblast kterou se aplikace zabývá. Příkladem může být půjčování peněž, kovoobrábění,
prodej zboží nebo také kompilace kódu. Ve většině domén existuje mnoho specifických pojmů například v doméně půjčování peněž existují pojmy jako věřitel, dlužník, insolvenceích.

### Doménový expert

K získání doménových pojmů slouží doménový expert. Doménový expert je osoba která se vyzná v doméně aplikace. Například v doméně účetnictví je doménovým expertem účetní. V některých případech je doménovým expertem člověk který požaduje napsání aplikace ale nemusí to tak být vždy. U velkých firem může být doménových expertů několik. Například může být jeden expert na sklad a jeden na nákupy zboží.

V některých případech také doménový expert vůbec nemusí existovat. Příkladem může být například firma Facebook. V době kdy Facebook začínal neexistoval pojem sociální síť a proto nemohl existovat ani doménový expert na sociální sítě. V případech kdy expert neexistuje je vhodné použít člověka který má znalosti s podobnou doménou. V případě Facebooku by to mohl být člověk který se podílel na vývoji ICQ nebo MySpace.

### Mapování domény

Od doménového experta získáme pojmy používané v doméně aplikace. Tyto pojmy poté můžeme použít pro pojmenování konceptů v kódu. V případě již zmiňované domény půjčování peněž bychom tedy z pojmu dlužník mohly vytvořit objekt `Debtor`. Bez použití doménových výrazů bychom museli použít název `ClientThatOwesMoney`. `ClientThatOwesMoney` by byl delší a zároveň méně přesný název jelikož Dlužník nemusí dlužit pouze peníze.

Problémem doménových názvů může být jejich složitost. Málo kdo ví jak přesně je pode zákona definován dlužník a věřitel. Mnohdy ale není potřeba znát celou definici. Obykle je dostačující pouze obecný popis pojmu s ohledem na potřeby aplikace. Například dlužník je definován následně:

*Dlužník je subjekt práva povinný ze závazkového právního vztahu k plnění vůči věřiteli.*

Tato definice je pro běžného člověka poměrně nepochopitelná. Pro zjednodušení této definice je potřeba definovat čím se naše aplikace zabívá. Jako příklad uvedu aplikaci jejíž doménou je půjčování peněz mezi firmami. Nyní už víme že se aplikace zabívá pouze firmami a peňězmi. Můžeme tedy definici zjednodušit následovně:

*Dlužník je firma která musí splatit peníze věřiteli.*

Definici jsme tedy ořezali na informace které jsou relevantní pro naši aplikaci. Díky tomu jsme dosáhli poměrně jednodušše pochopitelné definice. Všiměte si také že definice obsahuje pouze minimum detailů. Není v ní uvedeno například zda dlužník může víckrát půjčit peníze nebo zda věřitel může mít více dlužníků. Tyto detaily je možné vyčíst z kódu.

V některých případech není definici možné zjednodušit. V takových situacích dává smysl přemýšlet nad logickým rozdělením aplikace na více částí a pro každou uvést jinou definici. Například pokud bycho měli jinou aplikaci která se zabývá dluhy pro firmy i pro běžné občany mohly bychom aplikaci rozdělit na dvě části. První část by se zabívala dluhy pro firmy a druhá dluhy pro běžné občany. Každá z těchto částí by pak měla svou definici dlužníka.

*Dlužník je firma která musí splatit peníze věřiteli.* - pro část aplikace která se zabývá firmamy.

*Dlužník je osoba která musí splatit peníze věřiteli.* - pro část aplikace která se zabývá osobami.

V moderním světě by tyto části mohli být samostatné microservices. Ve světě monolitu to mohou být samostatné bounded contexty.

Pokud rozdělení aplikace není možné tak není jiná možnost než se spokojit se složitou definicí.

### Dokumentace doménových pojmů

O doménových pojmech je potřeba informovat tým. K tomu se obvykle používá dokument který obsahuje definice všech doménových pojmů tak jak jsem je již popsal. Těchto dokumentů může být i více pro jednu aplikaci. Dokument by měl obsahovat doménový pojem česky, anglicky a popis tohoto pojmu. Příklad:

Pojem česky   Pojem anglicky  Popis
dlužník   Debter   *Dlužník je firma která musí splatit peníze věřiteli.*

Český název je důležitý jelikož ho můžeme používat při diskuzy se zákazníkem, lidmi z byznysu a mezi programátory. Anglický název je pak použit v kódu.

U běžné dokumentace se často stává že přestane být udržována a neodpovídá aktuálnímu kódu. Doménová dokumentace trpí stejnýn problémem avšak doména aplikace se mnění mnohem méně než běžný kód. Z tohoto důvodu je mnohem jednoduší udržet doménovou dokumentaci aktuální.

## Názvy podobající se konceptu který se snaží vyjádřit



Evolving you application language

- priklad s opravnenimy a jak dokazi nazvy vycistit mluvu
- priklad jak pojmenovavani utvari myslenku o objektu - dat metody a jak by to pojmenovali pes, kosik, advance x add, kos atd.
  k tomu patri asi i kosik -> jezdici pas 
- chybjejici metody nam z nejakeho duvodu nechybi
- prebivajici metody jsou problem -> special casy
- dokumentace metafor

Next part ubiquitous language


























Brzké pojmenování a strach z přejmenování
Na předpoklady a očekávání také navazuje brzké pojmenování








 .... (Pokračování rozhovoru bude naznačeno dále)  //--- bude tam že bude chtít vytvořit novou roli která je kombinací těchto 3
ale přitom nedává smysl  





Objekty by také neměli mít překvapující vlastnosti. Obecně si představujeme auto se 4 koly. Programátor ale může počítat
i s rezervou. Může se tedy stát že někdo vytvoří objekt Car s 5 koly. Proč by takové rozhodnutí dělal? Typycky je to z
toho důvodu že potřebuje s koly pracovat jako s celkem. Například potřebuje zjistit tlak ve všech pneumatikách a tak je
jednodušší vytvořit pole všech 5 pneumatiky a pracovat se všemi naráz. Správné řešení by ale bylo neulehčovat si práci 
a reprezentovat kód podle toho jak vypadá v reálném světě.








Například pokud definujeme metody Add(int amount) a Subtract(int amount) 
očekává se že tyto metody budou komutativní. Tedy že x.Add(2); x.Subtract(3); je stejne jako x.Subtract(3); x.Add(2);
Stejně tak je neočekávané když objekt Car obsahuje metodu Car.Fly() která způsobí že auto bude létat. Tyto předpoklady
jsou v některých případech dobré ale někdy mohou být i špatné. Je dobré že nám pomáhají vytvořit představu o kódu
ještě předtím než jsme ho viděli. Na druhou stranu tato představa může být zavádějící. Kód nemusí přesně reprezentovat
naši představu. Například u objektu Car můžeme očekávat že má 4 kola. Avšak programátor mohl počítat i s rezervou a 
napsat třídu Car tak aby měla 5 kol.

V ideálním případě by měl objekt přesně odpovídat skutečnosti.

Naše předpolady zároveň vedou k přemýšlení  







I přes to že existují pravidla čistého kódu která říkají že by pojmenování mělo být dlouhé a přesné
raději než krátké a neúplné 


Programátoři často volí nepřesné názvy jelikož nejsou schopni  
a proto často volíme nepřesné názvy. Tento článek tedy popisuje některá ... nevim  



Je zřejmé že špatné nebo nepřesné pojmenování 
vede k horší čitelnosti kódu. Chybné pojmenování ale vede i k dalším problémům:

1) Zhoršení komunikace mezi programátory. Pokud můžeme něco jednodušše pojmenovat můžeme se o tom i jednodušše bavit.
2) Pojmenování sebou nese určité předpoklady. 








V programování je ale často problém správně pojmenovat některé koncepty. 