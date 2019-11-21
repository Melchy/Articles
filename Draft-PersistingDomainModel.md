# Ukládání doménového modelu
(Doménový model)[https://martinfowler.com/eaaCatalog/domainModel.html] umožnuje lépe vyjádřit byznys logiku aplikace a také lépe zapouzdřit jednotlivé byznys operace. Díky tomu je doménový 
model pro mnoho aplikací vhodnější než obyčejný databázový model. Bohužel ale trpí jednou velkou nevýhodou která se obecně nazývá (Object-relational impedance mismatch)[https://en.wikipedia.org/wiki/Object-relational_impedance_mismatch]. Object-relational impedance mismatch jedonudušše říká že můžeme naši doménu namodelovat naprosto přesně pomocí doménového modelu ale tento model nakonec stejně budeme
muset uložit do databáze která má uplně jinou strukturu. V tomto článku ukáži TODO

## Vyplatí se doménový model ?
V ideálním světě by Object-relational impedance mismatch vůbec neexistoval a doménový model by byl vhodný pro všechny aplikace s výjímkou nejjednodušších CRUD aplikací. Bohužel ale nežijeme
v ideálním světě a problém s ukládáním doménového modelu může způsobit značné zpomalení vývoje aplikace. Aby se doménový model vyplatil
musí přinést dostatečné zrychlení práce s byznys logikou které vyváží zpomalení při ukládání dat. Bohužel ale není možné v žádném kromě pár extremních vyjímek zjistit zda doménový model ušetří dostatek času.

> Jednoduché ukládání doménového modelu je tedy velice důležité jelikož může rozhodnout o tom zda má význam použít doménový model i pro jednodušší aplikace které nemají obrvské množství byznys pravidel.

## Dva modely
Jedeno z oblíbených řešení object-relational impedance mismatch je ruční mapování doménového modelu na dotabázový a zpět. Mapování obvykle provádí repozotář. Mapování modelů není vhodné
jelikož může u jednodušších aplikací přidat mnoho zbytečného kódu který bude pouze přesouvat data z jednoho objektu do druhého. Více o problémech tohoto přístupu můžete najít (zde)[https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/]. Použití doménového modelu s dvěma modely tedy není příliš vhodné jelikož je velice obtížné odhadnout zda
mapování mezi modely nepřidá příliš mnoho práce.

## Použití NoSQL
Dalším způsobem jak vyřešit object-relational impedance mismatch je serializování doménového modelu a ukládání do NoSql databáze. V případě C# je to obvykle MongoDB nebo RavenDB. Ukládání
do NoSQL databáze je velice jednoduché a zdá se jako skvělé řešení. Problém s NoSQL databázemi ale nastane ve chvíli kdy potřebujeme provádět čtení. Agregáty které se do NoSQL ukládají jsou
modelovány podle (invariant)[https://dddcommunity.org/library/vernon_2011/]. Tetnto způsob modelování agregátů způsobí že při čtení musíme data často získávat z několika agregátů zároveň.
NoSQL databáze ale neumožňují složité dotazovaní nad daty. Což způsobí že ukládání doménového modelu bude jednoduché ale dotazování bude často složité. NoSQL tedy ve výsledku bude trpět podobným
problémem jako řešení s dvěma modely.

Při modelování doménového modelu se také často stává že je potřeba agregáty zrefaktorovat. V případě dvou modelů je to jednoduchá operace jelikož není potřeba měnit nic v databázy. U NoSQL
databází je ale nutné zmigrovat data což může být u produkčního systému poměrně komplikované. Tyto obtíže povedou programátory k provádění menšího počtu refactoringů což sníží čitelnost byznys logiky.

## Použití ORM pro mapování modelů
Dalším řešením je nastavit ORM tak aby bylo schopné mapovat databázové tabulky na komplexní doménový model. Tento přístup není v .NETu příliš rozšířený jelikož Entity framework nepodporuje
komplexní mapování objektů. Ve světě Javy je ale tento přístup poměrně populární. V C# se dá použít Nhybernate který umožňuje poměrně komplexní mapování. Bohužel ale Nhybernate
není velcie populární a vypadá spíše jako stále méně používaná technologie.

## Hybridní řešení s Entity Frameworkem
Jak už jsem zmínil entity framework neumožňuje mapování komplikovaných entit. Umožnuje pouze poměrně jednoduché entity. Nový entity framework umožnuje mapování hodnotových objektů.
Což umožnuje mapovat velké množství jednodušších případů. U složitějších případů je potřeba použít ruční mapování (dva modely).

Otázkou nyní je - Jak často bude nutné použít ruční mapování. (Eric Evans v Effective aggregate design)[https://dddcommunity.org/library/vernon_2011/] píše o případu kde 70% agregátů
obsahuje pouze kořenovou entitu a value objekty. Dále také naznačuje že v běžné aplikací bude obsahovat podobné rozložení. Pokud má Eric Evans pravdu pak by ve
většině případů měl stačit Entity framework pro mapování Entit.

Tento způsob by tedy měl značně snížit složitost mapování doménového modelu oproti řešení se dvěma modely. Zároveň tanto způsob netrpí problémem s refactoringem agregátů jelikož programátor vždy
může změnit mapovací logiku. Díky zjednodušení mapování můžou i aplikace s poměrně jednoduchou byznys logikou použít doménový model bez strachu z přidané komplexity. 






Pro zodpovězení této otázky rozdělím aplikace do tří skupin:

1. Aplikace typu Startup - aplikace které nemají přesně definovanou doménu se kterou budou pracovat. Do této kategorie spadá každá aplikace která nemá přesnou představu o tom co všechno bude
ve výsledku dělat. V některých případech se může zdát že je zřejmé co bude aplikace dělat avšak po několika měsících vývoje se může ukázat že aplikace bude dělat mnohem více nebo naopak méně věcí.
Příkladem může být skladový systém pro firmu která vyrábjí klávesnice. Na počátku se zdálo že bude stačit jednoduchý skladový systém avšak po čase programátoři zjistili že je potřeba zautomatizovat
procesy celé firmy což vedlo k napsání informačního systému pro celou firmu.

Důležité je poznamenat že řešením těchto problémů není lepší analýza na začátku vývoje systému. Komplikovaná analýza pouze povede k přístupu s názvem (Big Design Up Front)[https://en.wikipedia.org/wiki/Big_Design_Up_Front]. Tento přístup ale málo kdy funguje a dnes již se zdá být přežitý. Už v roce (2004 Kent Back v Extremín programování)[https://www.amazon.com/Extreme-Programming-Explained-Embrace-Change/dp/0321278658] napsal že lepším přístupem je postupný design.

2. Dalším typem jsou zaběhlé aplikace - aplikace které mají poměrně přesně definovanou doménu a je jen malá šance že se změní. Do této kategorie typycky patří aplikace které vznikly přepisem již existujícího
systému.

Doménový model bude vhodný pro aplikace které mají na první pohled extrémě složitou byznys logiku. Nehledně na to zda jsou to zaběhlé aplikace nebo startupy. Další případ který se zdá být vhodný pro doménový model
jsou zaběhlé aplikace se složitější byznys logikou. Jak ale zjistit že byzyns logika aplikace je dostatečně složitá aby se doménový model vyplatil. 

 Otázkou ale je jak složitá byznys logika je dostatečně složitá.



3. Posledním typem jsou aplikace s extrémě složitou byznys logikou. Příkladem takové aplikace jsou obvykle finanční systémy. V tomto případě nezál




Pokud praktikujete Agile tak určitě víte že základním pravidlem je adaptace na změny v systému.



Argumentem proti těmto změnám často bývá že analýza požadavků systému by měla odhalit tyto chyby. 




avšak časem se může ukázat že je potřeba vytvořit informační systém pro celou firmu. 


Příkladem může být Facebook v roce 2004.
Facebook byl na začátku vývoje určen pro komunikaci studentů vysokých škol. Doménou Facebooku tedy byla komunikace studentů vysokých škol. Časem se ale doména změnila na celosvětovou síť.
Doména tedy byla někdy v průběhu vývoje změněna a nikdo nemohl předvídat že tato změna nastane. Druhým případem může být nová aplikace pro již zaběhnutý byznys. Například nový 
informační systém pro výrobna klávesnic



 Dalším příkladem může být informační systém pro firmu vyrábějící klávesnice. V tomto případě je doména aplikace poměrně známá 




Představme si že doménový model je opravdu úžasné řešení které dokáže zjednodušit doménu aplikace pětinásobně. Mapování ale naopak způsobí dvojnásobně složitější ukládání dat. Jak tedy
rozhodneme že naše doména je dostatečně složitá aby dokázala vyvážit vyšší složitost v ukládání? Odpověď je že takový odhad není možný pro většinu aplikací. Kromě několika procent těch opravdu
nejsložitějších aplikací. 


Object-relational impedance mismatch se často řeší ručním mapováním z doménového modelu na databázový. Ruční mapování je ale poměrně 


Mapování doménového modelu na databázový můžeme jednoduše vyřešit mapováním těchto dvou modelů. Mapování ale přináší poměrně  