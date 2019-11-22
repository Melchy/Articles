# Ukládání doménového modelu
(Doménový model)[https://martinfowler.com/eaaCatalog/domainModel.html] umožnuje lépe vyjádřit byznys logiku aplikace a také lépe zapouzdřit jednotlivé byznys operace. Díky tomu je pro mnoho aplikací vhodnější než obyčejný databázový model. Bohužel ale trpí jednou velkou nevýhodou která se obecně nazývá (Object-relational impedance mismatch)[https://en.wikipedia.org/wiki/Object-relational_impedance_mismatch]. Object-relational impedance mismatch jedonudušše říká že můžeme naši doménu namodelovat naprosto přesně podle byznys požadavků ale nakonec tento model stejně budeme muset uložit do databáze která má uplně jinou strukturu. Mapování mezi těmito modeli nemusí být triviálná záležitost a může nakonec vést ke značnému zesložitění celé aplikace. Tento článek popisuje výhody a nevýhody několika způsobů které se často používají pro ukládání doménového modelu. Ještě než popíši jednotlivé způsoby ukládání je důležité vysvětlit proč je ukládání tak důležité.

## Vyplatí se doménový model ?
V ideálním světě by Object-relational impedance mismatch vůbec neexistoval a doménový model by byl vhodný pro téměř všechny aplikace. Bohužel ale nežijeme v ideálním světě a Object-relational impedance mismatch může způsobit značné zpomalení vývoje. Zpomalení může být tak zásadní že použití doménového modelu nakonec zpomalí celý vývoj aplikace. Pokud se tedy rozhodujeme zda použít doménový model musíme nejdříve zjistit zda je naše byznys logika dostatečně složitá. Prakticky ale neexistuje žádný způsob jak tuto složitost zjistit. Dobrá zpráva ale je že můžeme zjednodušit ukládání doménového modelu a přiblížit se tak ideálnímu světu ve kterém se tento model vyplatí vždy.

> Pokud je ukládání doménového modelu dostatečně jednoduché je možné tento model použít i pro jednodušší aplikace.

## Dva modely
Prvním řešením je mapování doménového modelu na dotabázový pomocí kódu napsaného programátorem. Tento přístup v aplikaci vytvoří dva modely. Jeden doménový který odpovídá byznysu aplikace a jeden databázový který přesně odpovídá databázi. Doménový model s použitím dvěma modelů se vyplatí pouze u aplikací s koplikovanou byznys logikou. U jednodušších aplikací přidají dva modely mnoho zbytečného kódu který pouze přesouvá data z jednoho objektu do druhého. Dva modely přístup tedy způsobí problém kdy není možné téměř v žádném z případů rozhnodnout zda se doménový model vyplatí. Více informací o složitosti dvou modelů můžete najít (zde)[https://enterprisecraftsmanship.com/posts/having-the-domain-model-separate-from-the-persistence-model/].

## Použití NoSQL
Dalším způsobem jak vyřešit object-relational impedance mismatch je ukládání doménového modelu do Dokumentové databáze. Způsob ukládání dat do NoSQL je velice vhodný pro doménový model jelikož jednotlivé agregáty mohou být serializovány a uloženy do dokumentů. Tímto přístupem zajistíme že není potřeba žádné mapování pouze serializace a deserializace. Problém s dokumentovou databází nastává při zobrazování dat uživateli. Při zobrazování dat je často potřeba získívat data z více agregátů. Dokumentové databáze ale běžně neumožňují čtení dat z více dokumentů (agregátů) zároveň. Dokumentová databáze tedy řeší mapování doménového modelu neřeší ale dotazovaní nad daty. 

Při modelování doménového modelu se také často stává že je potřeba agregáty zrefaktorovat. Typycky je potřeba přesunou několik propert a metod do jiného agregátu. V případě použití ručního mapování je takový refactoring velice triviální jelikož není potřeba měnit data v databázy. U NoSQL databází je ale nutné zmigrovat data což může být u produkčního systému poměrně komplikované. Tyto obtíže povedou programátory k provádění menšího počtu refactoringů což sníží čitelnost byznys logiky.

Dokumentové databáze ulehčují ukládání a načítání agregátů ale přidávají jiné problémy. Ve výsledku není jednoduché určit zda Dokumentová databáze ušetří programátorům čas. V průměru by tento přístup neměl být o mnoho lepší než použití dvou modelů.

## Použití ORM pro mapování modelů
Dalším řešením je nastavit ORM tak aby bylo schopné mapovat databázové tabulky na komplexní doménový model. Tento přístup není v .NET příliš rozšířený jelikož Entity framework nepodporuje komplexní mapování objektů (hlavně ve starších verzích). Jedinou existující altrenativou je ORM hybernate které podporuje poměrně komplexní mapování. Bohužel ale Nhybernate není příliš populární a vypadá spíše jako stále méně používaná technologie. Pro mapování doménových modelů je ale použití ORM velice praktické. ORM nemá mnoho nevýhod snad jen komplexní mapování u některých složitých případů. Další nevýhodou může být použití netypované XML konfigurace. XML konfigurace je ale v moderní době často nahrazena silně typovaným fluent mapováním.

## Další řešení
Pro mapování doménového modelu se používá ještě několik dalších řešení. Všechny jsou ale podobně složité jako dva modely nebo neumožňují použití vytvoření doménového modelu. Pro úplnost tedy výjmenuji zbylá řešení a jejich nevýhody:

1) Event sourcing - obtížná oprava eventů, práce s externími systémy, nutnost vytvoření dvou modelů, eventuální konzistence mezi read and write stranou.
2) State backed aggregate - neumožňuje použití hodnotových objektů a zanořených entit.
3) Dvě databáze - je potřeba vytvořit dva modely a provést mezi nimi mapování. Nepřináší tedy mnoho výhod oproti dvoum modelům.


## Hybridní řešení s Entity Frameworkem
Jak už jsem zmínil entity framework neumožňuje mapování komplikovaných entit. Umožnuje pouze poměrně jednoduché entity. Nový entity framework umožnuje mapování hodnotových objektů.
Což umožnuje mapovat velké množství jednodušších případů. U složitějších případů je potřeba použít ruční mapování (dva modely).

Otázkou nyní je - Jak často bude nutné použít ruční mapování. (Eric Evans v Effective aggregate design)[https://dddcommunity.org/library/vernon_2011/] píše o případu kde 70% agregátů
obsahuje pouze kořenovou entitu a value objekty. Dále také naznačuje že v běžné aplikací bude obsahovat podobné rozložení. Pokud má Eric Evans pravdu pak by ve
většině případů měl stačit Entity framework pro mapování Entit.

Tento způsob by tedy měl značně snížit složitost mapování doménového modelu oproti řešení se dvěma modely. Zároveň tento způsob netrpí problémem s refactoringem agregátů jelikož programátor vždy může změnit mapovací logiku. Díky zjednodušení mapování můžou i aplikace 
s poměrně jednoduchou byznys logikou použít doménový model bez strachu z přidané komplexity. 


V dalším článku si ukážeme jaké případy dokáže Entity Framework vyřešit a v jakých je potřeba použít ruční mapování.