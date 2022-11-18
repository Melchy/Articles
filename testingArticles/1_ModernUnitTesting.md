# Modern Unit Testing

V posledních letech jste si mohli všimnout že způsob testování se začal značně měnit. Mnoho nejlepších praktiky začalo být méně používané a několik anti patternů se naopak proměnilo v patterny. Například Integrační testy se stávají v poslední době stále populárnější a to hlavně díky dockeru který nám umožňuje jednoduše spustit jakoukoliv servisu pomocí jednoho příkazu v konzoli. Tato série článků popisuje "nový" přístup k psaní testů.

//TODO shrnutí o čem budu vlastně psát

## Problémy standartních testů

Tyto best practices mě vedly k testů které testovali pouze malé části aplikace a neustále se rozbíjeli
kvůli refactoringu kódu. Z těchto důvodů jsem hledal způsoby jak psát lepší testy a naštěstí jsem našel poměrně odlišný způsob testování který sice porušuje best practices ale zajišťuje mnohem lepší psaní testů.

Po čase jsem si začal všímat že stejné problémy řeší i mnoho jiných programátorů. Obecně bych je rozdělil do následujících skupin:

* Programátoři kteří považují testování za luxus který si nemohou dovolit jelikož to jejich zákazník/business nezaplatí
* Programátoři kteří nepíšou testy jelikož jejich aplikace je tak jednoduchá že nemají nic co by testovali.
* Programátoři kteří považují testy za zbytečné jelikož stejně nenajdou žádné chyby a pouze stojí peníze.

Všechny tyto problémy mají v mnoha případech stejnou příčinu na kterou jsem narážel i já. Tato série článků popisuje "nový" způsob psaní testů který značně vylepšuje standartní unit testování.

## Předpoklady o čtenáři

Tato série článků je určena pro pokročilejší programátory kteří mají zkušenti s testováním a znají alespoň základní pojmy jako jsou

* Dependency injection
* Mockování
* Integrační testy
* Unit testy

## Závěr

V příštím díle se podíváme na to jak vypadá standartní test který už jste pravděpodobně mnohokrát viděli.

# Standartní test

V tomto díle se podíváme na běžný test které jste pravděpodobně viděli v článcích o testování a popíšeme si jejich výhody a nevýhody.

## Ukázkový test

Následující příklad ukazuje asp.net MVC core API aplikaci která obsahuje jednoduchý controller který se stará o registraci uživatele
a odeslání registračního emailu.

//todo

Tento kód bychom mohli otestovat tak že přesuneme logiku z controlleru do servisy:

//todo

Nyní můžeme vytvořit test který namockuje všechny závislosti a otestuje obsah servisy:

//TODO

Test psané tímto způsobem budu v dalších článcích označovat názvem "Standartní test". Standartní test nemá přesnou definici ale je definován podle jeho vlastností které jsou popsané níže. 
Pokud byste chtěli více příkladů standartních testů tak můžete použít téměř jakýkoliv příklad z knihy [Growing Object-Oriented Software, Guided by Tests 1st Edition](https://www.amazon.com/Growing-Object-Oriented-Software-Guided-Tests/dp/0321503627)
a [The Art of Unit Testing: With Examples in C#](https://www.amazon.com/Art-Unit-Testing-examples/dp/1617290890).

Stanratní testy jsou často považovány za lepší způsob testování než integrační testy. Například výše zmíněná kniha [The Art of Unit Testing: With Examples in C#](https://www.amazon.com/Art-Unit-Testing-examples/dp/1617290890) popisuje na dvou stránkách nevýhody integračních testů. Problémem ale je že standartní unit testy mají také několik nevýhod které ale nejsou tak často zmińovány. Z tohoto důvodu v následujících odstavcích popíši vlastnosti testů a odhalím vlastnosti ve kterých unit testy nevynikají.

## Vlastnosti testů

U testů můžeme pozorovota následující vlastnosti:

* Rychlost - jak dlouho trvá průběh testu
* Obtížnost na setup - kolik práce musí programátor vykonat aby mohl test spustit
* Obtížnost implementace - kolik času programátorovi zabere implementace test
* Spolehlivost - určuje jak moc se můžeme spolehnout na to že test odhalí chybu. Například pokud test použije mock tak je možné že mock nebude odpovídat realitě a proto se nemůžeme na test 100% spolehnout.
* Křehkost - určuje jak často se test rozbije při refactoringu kódu. Je důležité zmínit že refactorovat znamená úprava kódu bez změny chování. Jde o přesun metod mezi třídami, rozdělení metod atd. Při změně chování aplikace se očekává že se testy rozbijí.

## Vlastnosti standartního unit testu

Standartní unit test má následující vlastnosti:

* Vysoká rychlost
* Minimální obtížnost na setup
* Poměrně složitá implementace - programátor musí často připravit nemalé množství mocků což zabere značný čas
* Malá spolehlivost - standartní unit testy často testůjí pouze malé části aplikace a netestují interakce mezi jednotlivými částmi systému. Mocky se také mohou chovat odlišně než reálné závislosti.
* Vysoká křehkost - refactoring velice často rozbije testy. Tento problém je ještě zhoršen tím že je potřeba udržovat mocky.

Pro úplnost bych doplnil ještě jednu často zmiňovanou výhodu unit testů která nezapadá do výše popsaných vlastností.

## Další výhoda unit testů

V některých situacích můžete zaslechnout že unit testy zvyšují kvalitu kódu tím že nutí programátora napsat testovatelný kód. Zastánci tohoto názoru obvykle považují testovatelný kód za lepší jelikož obsahuje jen malé množství globálních proměných a naopak obsahuje velké množství interfaců a nutí programátora rozdělovat kód do testovatelných částí.

Tento argument je pravdivý pro začátečníky. Pokud jste ale pokročilejší programátor který používá dependecy injection, interfacy, vyhýbáte se globálním proměným a nepíšete celou aplikaci do jednoho souboru tak
je pravděpodobné že psaní standartních unit testů váš kód o mnoho nezlepší.

## Vlastnosti integračního testu

* Nízká rychlost
* Vysoká obtížnost na setup
* Jednodušší implementace - nasetupovat externích závislostí je poměrně náročné ale je to obvykle jednorázová akce. Oproti tomu u unit testu musíte v každém testu mockovat což zabere v dlouhodobém horizontu mnohem více času.
* Vysoká spolehlivost - integrační testy otestují celou aplikaci
* Nízká křehkost - při použití integračních testů můžete z refactorovat téměř celou aplikaci a nic se nerozbije.

## Problém standatních testů

O problémech které vyplývají z nízké spolehlivosti, vysoké křehkosti a složité implmentace jste už pravděpodobně slyšeli. Obvykle znějí nějak takhle:

* Testování se nevyplatí jelikož je psaní testů a jejich údržba příliš nákladná (business/zákazník to nezaplatí).
* Testy nemá význam psát jelikož je aplikace tak jednoduchá že není co testovat testovat.
* Testování je k ničemu jelikož stejně odhalí jen minimum bůgů.
* Refactorování kódu není možné jelikož testy nejsou dostatečně spolehlivé na to aby odhalili chyby.
* Refactorování kódu se nevyplatí jelikož je potřeba opravit všechny rozbité testy.

Většinou si ale programátoři neuvědomují že to co popisují je problém standartních unit testů a né unit testů jako takových.

## Kompromis

Obecně můžeme říct že nemůže existovat test který má všech pět vlastností nejlepších jelikož si některé z nich odporují. Například není možné mít maximální spolehlivost a zároveň maximální rychlost jelikož pro dosažení maximální spolehlivost je nutné použít reálné závislosti které ale budou vždy pomalejší než mocky.

V následujících dílech této serie článků si popíšeme nový druh testů který je kompromisem mezi unit testy a integračními testy.

## Závěr

Standartní unit testy mají několik nevýhod o kterých se moc často nedočtete:

* Testují jen malou část kódu
* Často rozbíjejí. Hlavně při refactoringu.
* Nejsou vhodné pro kód který obsahuje jen malé množství logiky.

V příštim díle se podíváme na to jak vylepšit náš test aby testoval větší část aplikace.



## Lepší test

V minulém díle jsme si ukázali "standardní" test a popsali ukázali jsme si jeho nevýhody které jsou

* Testuje jen malou část kódu
* Často se rozbíjí. Hlavně při refactoringu.
* Nejsou vhodné pro kód který obsahuje jen malé množství logiky.

Dnes se podíváme na stejný kód a zrefactorujeme ho tak aby testoval větší část aplikace.

## Příklad z minulého dílu

Pro zopakování se podíváme na příklad z minulého dílu:

TODO

## Mockování na hranách aplikace

První vylepšení které si ukážeme je mockování na hranách aplikace. Tuto techniku je nejelpší demonstrovat na pčíkladu a proto se zaměříme na metodu pro odesílání emailu z předchozího příkladu:

//TODO

Celý tento kód není v předchozím příkladu otestován jelikož je nahrazen mockem. Abychom ho mohli otestovat tak posuneme mock na hranu systému a namockujeme pouze metodu TODO.
Mockování TODO ale není jednoduché jelikož je to třída z nuget knihovny která neimplementuje žádný interface který bysme mohli v testu nahradit. Z tohoto důvodu si poumůžeme patternem humble class. Humble class je jednodušše třída která nemá žádnou logiku a pouze provolává jinou třídu.

V našem případě tedy upravíme kód následujícím způsobem:

//TODO

V testu nyní můžeme humble class nahradit mockem:

//TODO

Celý příklad:

//TODO

Pomocí humble interfacu jsme rozšířili množství kódu který je pokrytý testy. Zároveň jsme zajistili že se test bude méně rozbíjet při refactoringu/úpravě kódu jelikož je pravděpodobné že třída TODO a metoda TODO se 
bude měnit méně než třída TODO a metoda TODO.

## Složitější příklad

Pro ukázku představím ještě jeden přiklad u kterého je mockování na hraně systému o něco složitější.

Příklad:

//TODO

Abychom mohli namockovat metodu TODO tak musíme přidat factory třídu která vytváří TODO a také musíme přidat humble class která nahradí třídu TODO. 
Implementace:

Nyní můžeme kód otestovat:

//TODO

## Závěr

V dnešním díle jsme si ukázali jak mockovat závislosti na hranách systému. V příštim díle se podíváme na to jak mockovat složité závislosti jako je databáze.


# Mockování složitých závislostí

V minulém díle jsme si ukázali jak mockovat na hranách systému. V tomto díle se podíváme jak mockovat složité závislosti jako je volání databáze.

## Příklad z minulého dílu

V minulém díle jsme skončili s následujícím kódem:

//TODO

Pro namockování TODO jsme použili humble class. Nyní se podíváme blíže na volání metody TODO. Z názvu metody asi tušíte že ukládá TODO do databáte.

## Mockování databáze

Pro testování databáze nemůžeme použít mockování na hraně systému tak jako u odesílání emailu v [minulém díle](TODO). U databáze bychom museli nějakým způsobem namockovat celé SQL což je časově náročné a proto musíme zvolit jiný způsob.

Při mokování SQL databáze máme následující možnosti:

* Použití "standartního" mocku
* Použití in memmory providera
* Použití SQL lite in memmory
* Použití reálné databáze
* Použití transakcí
* Kombinace SQL lite a transakcí

## Použití standartního mocku

Při mockování databáze nemůžeme použít humble object ale můžeme použít se spokojit s běžným mockem:

TODO

Výhody:

* Testy jsou velice rychlé.
* Programátor nemusí instalovat databázový server na svůj počítač.
* Po každém testu není potřeba mazat data z databáze.
* Testy mohou běžet paralelně.

Nevýhody:

* Kód repozitářu není otestován jelikož je nahrazen mockem.
* Je potřeba často opravovat mocky repozitářů jelikož se repozitáře často mění.
* Mockovaný kód nemusí odpovídat SQL dotazu který se provede.

## Použití in memmory providera

Pokud používáte entity framework tak je možné pro testování využít [inmemmory providera](https://entityframeworkcore.com/providers-inmemory). InMemmory provider
funguje tak že v testech nakonfikurujete aplikaci tak aby namísto SQL databáze volala inmemmory providera. InMemmory provider pak slouží jako mock databáze který je přímo v paměti.

Výhody:

* Testy jsou velice rychlé.
* Programátor nemusí instalovat databázový server na svůj počítač.
* Po každém testu není potřeba mazat data z databáze.
* Testy mohou běžet paralelně.
* Kód repozitářů je otestován.

Nevýhody:

* Inmemmory provider se často chová odlišně než běžná SQL databáze. Více o těchto problémech [zde](https://jimmybogard.com/avoid-in-memory-databases-for-tests/).
* Nemůžete ostestovat dotazy v čistém SQL.

## Použití SQL lite in memmory

Entity framework nabízí také SQL lite providera kterého můžeme použít v testech stejně jako inmmemory providera. SQL lite umožňuje ukládání data do paměti bez uložení na disk.
Sql lite provider tedy funguje téměř stejně jako inmemmory provider a má také velice podobné výhody a nevýhody.

Pokud jako váší databázi používáte MSSQL tak musíte počítat s tím že SQL lite je jiná databáze a nepodporuje všechny
funkce z MSSQL. SQL lite například nepodporuje datovýtyp DateTime a proto entity framework pro tyto sloupce použije datový typ string. Problém pak nastane pokud potřebujete datumy seřadit. SQL lite jednoduše provede seřazení řetězců podle abecedy což neodpovídá seřazení podle data a času v MSSQL.

Obecně ale můžeme říct že sql lite podporuje mnohem více funkcí než inmemmory provider.

Výhody:

* Testy jsou velice rychlé.
* Programátor nemusí instalovat databázový server na svůj počítač.
* Po každém testu není potřeba mazat data z databáze.
* Testy mohou běžet paralelně.
* Kód repozitářů je otestován.

Nevýhody:

* SQLlite se chová odlišně než jiné SQL databáze.
* Nemůžete ostestovat dotazy v čistém SQL (jazyk pro SQL lite se liší napříkad od MSSQL).

SQL lite je tedy ve všech ohledech lepší než inMemmory provider. Návod na použití SQL lite in memmory najdete [zde](https://www.meziantou.net/testing-ef-core-in-memory-using-sqlite.htm).

## Použití reálné databáze (naivní přístup)

Další přístup který můžete zvolit je použití reálné SQL databáze.

Výhody:

* Testy přesně odpovídají produkci.
* Můžete otestovat i volání čístého SQL.

Nevýhody:

* Testy jsou pomalejší.
* Testy musí běžet seriově.
* Programátor musí mít nainstalovanou databázi.
* Po každém testu je potřeba mazat obsah databáze.

Použití reálné databáze se může zdát jako jedna z nejhorších variant a proto uvedu několik typů které vám ulehčí práci s reálnou databází.

1. Pro mazání dat mezi testy můžete použít nástroj [respawn](https://github.com/jbogard/Respawn) který dokáže jendodušše a rychle smazat všechna data z databáze.
2. Pro MSSQL je pro lokální spuštění testů možné použít localDB která je nainstalována automaticky s visual studiem.
3. Pro spuštění v CI/CD pipeline je možné použít docker image.
4. Pro sériové spuštění testů a připravení setupu (vytvoření tabulek databáze atd.) je vhodné použít NUnit.

## Použití reálné databáze (optimalizovaný přístup)

Použití reálné databáze můžeme optimalizovat tím že pro každý test vytvoříme novou databázi a na konci testu jí smažeme. Tento přístup umožní testy spustit paralelně což vede k mnohanásobnému zryhlení. Mazání a vytváření databází není tak pomalé jak byste mohli očekávát.

Výhody:

* Testy přesně odpovídají produkci.
* Můžete otestovat i volání čístého SQL.

Nevýhody:

* Testy jsou o něco pomalejší.
* Programátor musí mít nainstalovanou databázi.

## Použití transakcí

Při testování je možné vytvořit na začítku testu transakci poté provést všechny operace a nakonec transakci zahodit. Tímto způsobem umožníme paralelní spuštění testů
jelikož do databáze nikdy nedostaneme žádná data. Obecně se transakce chovají velice podobně jako kdybychom data reálně ukládali do databáze.

Transakce se stejně jako použití InMemmory providera může lišit od reálné databáze. Problém může nastat pokud aplikace používá transakce. V těchto případech test musí vytvořit zastřešující transakci která obsahuje vnořené transakce ([(Nested transaction](https://stackoverflow.com/questions/527855/nested-transactions-in-sql-server)). Zastřešení transakcí pak může způsobit rozdílné chování oproti reálné databázi. Kromě tohoto rozdílu osobně nevím o žádném jiném rozdílu oproti použití reálné databáze.

Výhody:

* Testy téměř odpovídají produkci.
* Můžete otestovat i volání čístého SQL.

Nevýhody:

* Testy nejsou o tolik rychlejší než použití reálné databáze (optimalizovaný přístup). Stále je potřeba před spuštěním testu nasetupovat schéma databáze tak aby odpovídalo aktuálnímu stavu.
* Programátor musí mít nainstalovanou databázi.

## Kombinace SQL lite a transakcí

Možná by vás mohlo napadnout zkombinovat některé z výše uvedených přístupů pro ještě lepší výsledek. Nejvíce se nabízí kombinace SQL lite a transakcí. Testy spuštěné proti Sql lite nebo v transakci se můžou lišit od chování reálné databáze. Můžeme tedy každý test spustit dvakrát. Jednou proti SQL lite a jednou v transakci. Pokud jsou výsledky rozdílné tak víme že jeden nebo oba z testů se chovají jinak než reálná databáze.

Tento přístup k testování se zdá zajímavý avšak realita je taková že spuštění dvou testů je ve výsledku pomalejší než spuštění testu proti reálné databázi.

## Jednoduchý benchmark

Důležitou otázkou je jak moc pomalé je použití SQL databáze oproti jiným variantám. Náš jednoduchý benchmark ukazuje následující výsledky:

Použití reálné databáze (optimalizovaný přístup) - jeden test: 2-5 sec, 148 testů: cca 35 sec
Použití inmmemory databáze - jeden test: 1 - 4 sec (často 1 sec), 148 testů: cca 30 sec  (7 testů které selhali kvůli odlišnému chování inmmemory DB)

Tento benchmark určitě neberte jako velice přesný. Je to jen jednoduchoučký benchmark na náhodně vybrané aplikaci.

## Naše volba

V naší aplikaci jsme vyzkoušely téměř všechny způsoby mockování databáze a nakonec jsme se rozhodli použít reálnou databázi s optimalizovaným přístupem. Ostatní způsoby přináší příliš malé zrychlení na to aby kompenzovali jejich odlišné chování od produkce.

Obecně na našich projektech preferujeme mockování v tomto pořadí:

1. Použití reálné databáze (optimalizovaný přístup)
2. Použití reálné databáze
3. Použití SQL lite in memmory
4. Použití standartního mocku

Ostatní varianty jsme zavrhli jako zbytečné nebo nepraktické.

Je důležité poznamenat že všechny varianty mají své místo a vždy je potřeba se rozhodovat podle vašich možností.

## Závěr

V tomto díle jsme si ukázali jak mockovat (nebo nemockovat) SQL databázi. V příštim díle se podíváme na to jak mockovat jiné externí závislosti jako jsou NoSQL databáze, fronty a další.

## Mockování dalších externích závislostí

V předchozím díle jsme si popsali jak mockovat nebo nemockovat SQL databázi. V tomto díle se podíváme na to jak mockovat jiné závislosti. Konkrétně si ukážeme příklady - redis, service bus a elastic search. Pokud vaše aplikace obsahuje jiné závislosti tak nezoufejte jelikož pravděpodobně budete moci použít jeden z přístupů prezentovaných v tomto článku.

## Mockování Redisu

Redis je key-value databáze což jednodušše znamená že je to jednoduché dictionary do kterého uložíte klíč a nějakou hodnotu.

Příklad:

//TODO

Redis umí i složitější operace jako je získání všech hodnot jejichž klíče jsou v určitém číselném rozsahu. Pro jednoduché vysvětlení ale postačí metafora s dictionary.

Jak bychom tedy redis mockovali? Hodně záleží jak naše aplikace redis používá. Pokud by využívala pouze základní operace - uložení záznamu podle klíče a získání
záznamu podle klíče tak by bylo pravděpodobně nejlepší řešení nahradit Redis pomocí dictionary:

//TODO

Pokud by naše aplikace používala sloužité operace tak bylo potřeba celý redis spustit v dockeru a testy následně spouštět proti dockeru. Testy poběží o něco delší dobu a vyžadují setup od programátora (obvykle spuštění jednoho řádku v konzoli) ale výsledkem jsou testy které přesně odpovídají reálnému redisu.

## Mockování service busu

Service bus je služba v azure která slouží pro posílání zpráv mezi aplikacemi. Jedna aplikace uloží zprávu do service busu a druhá ji pak může přečíst.
Jako příklad uvedu aplikaci která ukládá zprávy do service busu a zároveň je i čte:

//TODO

Mockování je podobné jako u redisu. Pokud používáte pouze základní funkcionalitu kterou jsem představil v předchozím příkladu tak si pravděpodobně vystačíte s vlastním mockem:

//TODO

Pokud byste chtěli otestovat složitější funkcionalitu tak vám service bus poměrně znepříjemní život. Servicebus nepodporuje spuštění v dockeru ani žádný jiný způsob pro lokální spuštění [github issue](https://github.com/Azure/azure-service-bus/issues/223). Řešením tedy je použití reálné instance v azure kterou budou vaše testy provolávat.

## Mockování Elastic searche

Elastic search je NoSQL databáze která se používá hlavně pro full text search. U elasticu obvykle nemá význam mockování jelikož potřebujete otestovat dotazy které elastic provádí. Řešením je tedy spuštění elasticu v dockeru. Optimalizovat rychlost testů můžete stejným způsobem jako u SQL databáze a vytvářet nový index (index je v elasticu něco jako tabulka v SQL) pro každý test.

## Poznámky

Jak můžete vidět tak testování externích závislostí není nic složitého. Stačí vždy zjistit jaké máte možnosti a poté se rozhodujete podle rychlosti nebo spolehlivosti.
Pokud použijete mock tak vaše testy budou rychlé ale budou se lišit od produkčního prostředí. Pokud použijete reálnou instanci tak budete mít jistotu že se závislost chová stejně jako v produkci ale vaše testy budou pomalejší.

Obecně bycho doporučil upřednostňovat spolehlivost testů na úkor rychlosti. Radši mít pomalejší testy které reálně odhalí chyby než mít rychlé testy které chyby neodhalí. Každé rozhodnutí ale samozřejmě záleží na okolnostech a nedá se jednoduše říct co je lepší.

## Závěr

Tímto jsme ukončili kapitoly o mockování externích závislostí a v příštím díle se podíváme na to jak volat aplikaci v testech.


# Volání aplikace

V minulích dílech jsme se bavili o tom jak mockovat externí závislosti. V tomto díle se podíváme jak volat aplikaci.

## Příklad z předchozích dílů

V minulích dílech jsme si ukázali příklad s registrací uživatele:

// TODO

V díle TODO jsme si ukázali jak mockovat odesílání emailu a v díle TODO jsme si ukázali jak mockovat nebo spíše nemockovat SQL databázi. Následující příklad ukazuje
výsledný test:

//TODO

## Obecné volání aplikace

Všechny testy které píšete by měli volat pouze veřejné api aplikace. V případě ukázkové aplikace by to byly metody v TODO třídách. Testování interních tříd vede k testům které je potřeba často opravovat
jelikož spoléhají na interní detali aplikace a proto se jim snažíme vyhýbat.

## Příprava dat pomocí vkládání do databáze

Následující příklad ukazuje test který ověřuje že uživatel se stejným emailem nemůže být registrován dvakrát.

TODO

Předchozí test manipuluje s daty přímo v databázi což může být problém jelikož pracuje s interními detaili aplikace které se mohou kdykoliv změnit. Pokud bychom se rozhodli že musíme hashovat emaily kvuli anonimizaci zákazníků museli bychom upravi kód následujícím způsobem:

//TODO

Poté bychom museli opravit test tak aby používal stejný hashovací algoritmus pro nahrání dat:

TODO

Jak můžete vidět tak interní změna aplikace kterou uživatel nijak nepozná rozbila ukázkový test. Z tohoto důvodu není vhodné spoléhat na interní detaili aplikace v testech.

## Příprava dat pomocí api

Data nemusíme připravovat pouze pomocí vkládání do databáze. Předchozí test bychom mohli přepsat následucím způsobem:

TODO

Test nyní neví nic o interní implementaci a testuje stejnou věc jako předchozí příklad.

## Složitý setup

V některých případech se může stát že setup testu je poměrně složitý - například můžeme potřevbovat uživatele který je registrovaný má 5 produktů v košíku a už udělal tři nákupy.
V těchto případech můžeme použít builder pattern který na pozadí zavolá všechny metody které jsou potřeba. Ukázka:

TODO

## Metody pro setup neexistují

Pokud aplikace neobsahuje veřejné api které potřebujeme pro přípravu dat tak nemáme jinou možnost než data vložit přímo do databáze.

## Závěr

V tomto článku jsme si ukázali jak volat a setupovat testy. V příštím díle se podíváme na to jak použít pure testy.

# Pure unit tests

V předchozích díle jsme si ukázali jak volat aplikaci bez toho abychom testy zatěžovali interními detaili aplikace. Dnes se podíváme na situace kdy je nutné otestovat interní detaili bez použití API.

Api v tomto čláku neznamená nutně API aplikace ale vrstva aplikace která je nejblíže controlleru a je testovatelná. Příklad z předchozích dílů:

TODO

Třída TODO by byla v tomto případě veřejné API.

## Volání veřejního api

Volání veřejného api zajišťuje že vaše testy nespoléhají na interní detaily aplikace. Můžete tedy celou aplikaci refactorovat a dokud nezměníte api tak je jisté že se vaše testy nerozbijí.

Problém nastane pokud narazíme na situaci kdy aplikace obsahuje tolik kombinací vstupů že není možné je všechny pokrýt api testy. Například pokud máme v aplikaci 10 vrstev a každá obsahuje čtyři větve kódu tak bychom museli napsat 4^10 testů abychom měli 100% pokrytí všech řádků. Asi nikdo nebude psát takové množství testů a proto nemůžeme použít volání api v testech.

Podrobnější vysvětlení combinatorial explosion problému můžete najít [zde](https://www.youtube.com/watch?v=V_-gKrxMUcw).

Standartní unit testy které testují interní třídy a nemockují na hranách aplikace tímto problémem netrpí. Standartní test by otestoval každou vrstvu samostatně a namockoval všechny její závislosti. Namockování všech vrstev způsobí že stačí napsat cca 40 testů abychom dosáhli 100% pokrytí. Standartní testy ale trpí problémy které jsme popsali v minulích dílech.

Kombinatorial explosion vzniká pouze v případě že váš kód je velice složitý. U jednoduché CRUD aplikace combinatorial explosion nikdy nenastane. V další části si tedy ukážeme jak rozdělit aplikaci na složitý
a jednoduchý kód a jak tyto části otestovat.

## Složitý kód a jednoduchý kód

Aplikace téměř vždy obsahuje složitý i jednoduchý kód. Složitý kód obsahuje velké množství větvení (ifů). Jednoduchý kód je obvykle provolávání metod a mapování dat mezi třídami.

Následující příklad demonstruje složitý kód:

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

Předchozí příklad by se těžko testoval pomocí veřejného api a proto použijeme jiný způsob - pure testy.

## Pure unit tests

Pure unit testy jsou testy které nevyužívají žádné mocky. Tyto testy jsou inspirovány funkcionálním programováním kde se často používají "pure metody" což jsou metody které nemají žádné závislosti kromě svého vstupu.

Příkladem pure metody je metoda Sum která sčítá dvě čísla:

//TODO priklad

Jak můžete vidět tak metoda TODO nevyžaduje žádné pomocné třídy pro výpočet. Potřebuje pouze dvě čísla na základě kterých vrátí výstup. Z tohoto důvodu můžeme říct že metoda TODO je pure.
Pokud by ale metoda TODO každý výsledek zapsala do databáze tak už není pure jelikož vyžaduje závislost - databázi.

Určitě si dokážete představit jak jednoduché je testovat metodu TODO stačí vždy vytvořit instanci třídy TODO a zavolat metodu TODO s různými vstupy a ověřit výsledek. Nepotřebujeme tedy nic mockovat a
naše testy jsou pure.

## Výhody pure testů oproti běžnému testu s mocky

Pure testy jsou o něco jednodušší na implementaci jelikož nemusíme připravovat žádné mocky. Další výhodou je že nemusíme udržovat mocky pokud refactorujeme/upravujeme kód.

## Pure testy v běžné aplikaci

V běžné aplikaci není možné většinu metod testovat pomocí pure testů jelikož obsaují volání jiných tříd a testy potřebují namockovat tato volání. Můžeme si ale pomoci a upravit kód tak aby obsahobal třídy které obsahují pouze pure metody.

Následující příklad ukazuje refactoring předchozího kódu který vyextrahuje pure metodu do samostatné třídy.

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

Metodu `IsUserEligibleToBuyItem` nyní můžeme otestovat bez použití mocků a obě větve `If(HappyHours)` můžeme otestova pomocí API testů.

Pro použití pure testů je tedy nutné upravit kód tak aby obsahoval metody/třídy bez závislostí. Toho můžeme docílit přípravou dat před zavoláním metody a následným
předáním těchto dat pomocí parametrů. Metoda poté vrátí výsledek na základě kterého může systém provést další operace.

## Použití mocků

V některých situacích se může stát že transformace na pure test není možná. V těchto případech je nutné použít mockování. Příkladem takové situace je logování.
Logování obvykle nemůžeme odstranit z kódu a udělat ho pure - nemáme tedy jinou možnost než logger namockovat.

Je důležité ale poznamenat že situací kde je nutné použít mockování v unit testech není mnoho.

## Skupiny objektů

V některých případech není nutné testovat pouze jeden objekt. Může se stát že máme skupinu několika objektů která nemá žádné závislosti mimo danou skupinu.
Tyto malé skupiny je v některých případech vhodné testovat společně pomocí pure testů.

## Api testy vs pure testy

Výhody api testů:

* Nemusí znát interní detaili aplikace
* Testují celou aplikaci. Nemůže se stát že testy nezachytí bug v integraci mezi třídami.

Nevýhody api testů:

* Nedokáží otestovat složitý kód.
* Setup dat může být poměrně složitý.

Kvůli těmto výhodám a nevýhodám je vhodné preferovat api testy a pure testy použít pouze v situacích kdy je potřeba testovat složitý kód.

## Závěr

TODO

# General typs

V minulích dílech jsme si pospali unit testy a integrační testy v tomto díle se podíváme na několik triků které dále vylepší naše testy

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

Pro ověření výsledků testů doporučujeme knihovnu Fluent Assertions. Fluent assertions nabízí velkou škálu assertovacích metod které mno

## Závěr

TODO

# Ridge

TODO

## Testování controllerů

V jednom z prvních dílu této série jsem ukazoval jak otestovat následující kód.

TODO

Prvním krokem k otestování bylo přesunutí kódu z controlleru do servisy.

TODO

Nyní je možné otestovat servisu a tvářit se jakože controller neexistuje. Problémem je že controller také může obsahovat bugy.
Abych vyřešil tento problém tak jsem navrhnul knihovnu [ridge](TODO) která transformuje volání metod na http volání:

TODO

Tímto způsobem můžeme otestovat celou aplikaci včetně controllerů. V našich aplikacích používáme výhradně ridge pro volání aplikace v API testech.
Zatím jsme narazili na následující výhody a nevýhody:

Výhody

* Otestujeme opravdu celou aplikaci včetně middlewaru, controlleru a vrácených http kódů.
* V testech můžeme použít dependenci injection.
* Testy jsou o něco méně závislé na interních detailech aplikace.

Nevýhody

* Testy jsou o něco pomalejší. První request na ASP.Net core je pomalý a trvá cca 300 milisekund.







V posledních letech se automatické testování stává stále populárnější a mnoho programátorů považuje testy za nedílnou součástí jejich práce. Pravděpodobně jste slyšeli nějakého známého programátora který popisuje automatické testování jako zlatý grál vývoje softwaru. 
Na druhé straně ale stojí lidé kteří považují testování za luxus který si nemohou dovolit.

Samotné psaní testů ale obvykle není nákladné a v mnoha případech stojí programátora podobné množství času jako ruční testování.
Zvláště když si uvědomíme že ruční testy musí programátor několikrát opakovat.

Automatické testy ale na rozdíl od ručního testování vyžadují určitou míru dlouhodobé údržby - při změně aplikace je potřeba opravit všechny rozbité testy.
Neustále opravování testů je pak velice nákladné a odradí mnoho programátorů od psaní testů.

Tato serie článků popisuje několik technik které snižují cenu psaní a údržby na minimum.
Díky tomuto snížení nákladů přestanou být testy nadbytečným nákladem a místo toho se stanou nástrojem který šetří čas programátorů.











//Pryc

V následujících dílech popíši rozdělení unit testů na dva druhy - EndToEnd unit testy a Pure unit testy. EndToEnd testy testují větší části aplikace a jsou někde na hraně mezi unit testy a integračními testy.
Pure testy jsou pak běžné testy které ale nepoužívají mocky.
