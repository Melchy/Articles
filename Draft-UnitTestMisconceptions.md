# Test misconceptions

O unit testech často můžeme slyšet několik mýtů a polopravd. V tomto článku popíši několik chybných tvrzení o testech a vysvětlím proč jsou nepravdivé.

V následujícím textu je slovem test myšlen unit test. V případě že popisuji jiný test tak uvedu celý název.

## Testovatelný kód je lepší kód

V posledních letech se testovatelná kód stal synonymem s dobrým kódem. Argument "testovatelný kód je lepší kód" můžeme zaslechnou i na místech kde nikdo nepíše testy. U testovatelného kódu není důležité zda píšete testy. U testovatelného kódu se prostě předpokládá že je lepší než kód který nejde testovat.

Skutečnost je ale trochu jiná. Můžeme najít příklady testovatelného kódu který je špatný - [lasagna architecture](https://twitter.com/CodeWisdom/status/967451306460884997?ref_src=twsrc%5Etfw%7Ctwcamp%5Etweetembed%7Ctwterm%5E967451306460884997&ref_url=https%3A%2F%2Fmatthiasnoback.nl%2F2018%2F02%2Flasagna-code-too-many-layers%2F). Zároveň můžeme najít patterny které jsou velice užitečné ale často nejsou testovatelné - [Active record](https://en.wikipedia.org/wiki/Active_record_pattern).

V některých případech se tedy může stát že testovatelný kód je horší než netestovatelný. Příště až uslyšíte "testovatelný kód je lepší kód" zkuste se zamyslet zda vůbec dává smysl daný kód testovat a zda nestačí pouze integrační testy.

Software který je pokrytý testy není nutně dobrý software. Testovatelnost neříká nic o kvalitě kódu.

## Vždy je lepší mít testy

Software pokrytý testy je obvykle považován za lépe rozšiřitelný a jednodušeji refactorovatelný. Často se ale stává že testy mají přesně opačný efekt - programátor udělá malý refactoring a poté straví několik hodin opravováním testů. Ve výsledku se automatické testy ani nevyplatí jelikož programátoři tráví všechen svůj čas úpravamy testů.

Problémem ale není jen čas strávený úpravamy testů. Dalším problémem je že programátoři po čase přestanou refactorovat kód jelikož se jim nebude chtít opravovat testy. Což dále povede ke zhoršení kódu a dalšímu zpomalení celého vývoje.

Příčinou těchto problému jsou špatně napsané testy. Testy jsou buď nečitelné nebo jsou [fragile](http://xunitpatterns.com/Fragile%20Test.html). Řešení těchto problémů se můžete dočíst [zde](https://www.manning.com/books/unit-testing) a v dalších knihách o testování.

Testy tedy mohou vést k celkovému zhoršení kvality kódu a někdy je lepší nepsat testy vůbec a nebo psát pouze integrační testy.

## Testy se nevyplatí

Častou odpovědí na otázku "proč nepíšete testy" je "nejsou na to peníze" nebo "nemáme dost času". Podle mých zkušeností je tato odpověď spíše výmluvou než skutečným důvodem. Obvykle programátoři neumí psát dobré testy což vede k problémům zmíněným výše. Výsledek je nakonec opravdu takový že se testy nevyplatí.

Pokud ale programátor opravdu umí psát dobré testy pak nedává argument s časem a penězy smysl. Investice do testů se za velice krátkou dobu vrátí. Obvykle v řádu týdnů. V některých případech je dokonce jednodušší napsat test než ručně spouštět kód. V takovém případě se investice vrátí okamžitě.

## Závěr

Testy se vždy nemusejí vyplatit.
Testovatelný kód nemusí být lepší kód.
Pokud píšete dobré testy tak se vyplatí.
