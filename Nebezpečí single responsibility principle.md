# Nebezpečí single responsibility principle

Single responsibility principle je definovaný následujícím způsobem: "Class or module should have one, and only one, reason to change."[[1]](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
neboli "třída nebo modul by měli mít pouze jeden důvod ke změně". V tomto článku si ukážeme proč je tato definice nepřesná a v mnoha případech i nebezpečná a představíme novou definici která mnohem lépe vystihuje co vlastně single responsibility principle (SRP) znamená.

## Nepřesná definice

Předchozí definice vyvolává otázku co je "důvod ke změně"? Na internetu můžeme najít mnoho lidí kteří si vykládají důvod ke změně různými způsoby. Pro ukázku uvedu několik příkladů:

* Můžeme najít mnoho lidí kteří si SRP vyloží tak že jedna třída by měla mít pouze jednu metodu [1](https://stackoverflow.com/q/46541197/5324847), [2](https://stackoverflow.com/questions/58986929/doesnt-having-more-than-1-method-break-the-single-responsibility-principle), [3](https://stackoverflow.com/questions/62844197/single-responsibility-principle-for-many-methods).
* Někteří lidé si SRP vyloží jako důvod proč obalovat metody wrapper metodami které nic nedělají. [4](https://www.overcoded.net/solid-single-responsibility-principle-srp-284015/)
* Dále můžeme najít programátory kteří říkají že bysme měli volání ``new`` vždy obalovat do factory tříd jelikož vytváření objektu je zodpovědnost samostatné třídy. [5](https://www.brandonsavage.net/breaking-the-single-responsibility-principle/)
* [Tato otázka](https://softwareengineering.stackexchange.com/q/150760) na stackoverflow popisuje aplikaci ve které programátoři rozbili mnoho tříd na menší jelikož jejich metody považovali za "důvod ke změně". Výsledek byl podle autora horší kód než ten se kterým začali.

Všechny tyto přiklady jsou pochybným použitím SRP a ve většině příkladů povedou ke zhoršení kódu na místo zlepšení.
Na internetu můžeme najít velké množtví těchto pochybných použití a nemá cenu se zabývat každým zvlášť.
Důležité je že SRP může znamenat prakticky cokoliv co si programátor vymyslí a v mnoha případech nevede ke zlepšení
kódu.

## Odpovědi na stackoverflow a hlubší problém

U předchozích pochzbných použití můžeme často najít diskuzy o tom proč je dané použití SRP špatně.
Většinu odpovědí v těchto diskuzích můžeme shrnout do tří kategorií:

1. SRP je potřeba používat se zdravým rozumem.
2. SRP musí být použito pouze pokud vede k udržitelnějšímu kódu.
3. Nepochopil jsi SRP.

Všechny tyto odpověďi v sobě mají trochu pravdy ale neřeší hlubší problém kterým je že SRP je špatně definováno a poměrně jednoduše může vést programátora k horšímu kódu.
Další problém který z diskuzí na stackoverflow vyplývá je že jen málo kdo opravdu ví co SRP znamená.

## Co na to Uncle Bob?

Uncle Bob (tvůrce SRP) si tyto
problémy [sám uvědomuje](https://blog.cleancoder.com/uncle-bob/2014/05/08/SingleReponsibilityPrinciple.html)
a proto v novějších článcích používá lepší definici která mnohem lépe popisuje co SRP znamená.

## Lepší definice

Novější definice od Uncle Boba zní takto:

"Gather together things that change for the same reasons and at the same times.
Separate things that change for different reasons or at different
times."[6](https://twitter.com/unclebobmartin/status/1023560222005227520?s=20).

Příklad: Představme si aplikaci která obsahuje třídu X s metodami A,B,C.
V průběhu několika měsíců přijde spousta požadavků na změny a vy si všimnete že v
90% případů se mění metody A a B společně. V tu chvíli můžeme říct že třída X
porušuje SRP a měli bychom metodu C vyjmout a vložit do jiné třídy.

Opačný příklad: V jiné aplikaci máme třídy X a Y která má každá jednu metodu.
Opět přijdou požadavky na změnu a my si všimneme že ve většině případů se mění třídy
X a Y společně. V tu chvíli bychom měli třídy X a Y spojit do jedné protože porušují SRP.

Všimněte si že SRP není možné aplikovat bez znalosti budoucích
požadavků.[7](https://twitter.com/unclebobmartin/status/1023578923907645440?s=20)
Často ale můžeme hádat jak se bude aplikace měnit a třídy rozdělit na základě těchto odhadů.
Pokud se v našem odhadu spleteme tak třídy můžeme jednodušše zrefaktorovat tak aby odpovídali SRP.

V mnoha případech je také vhodné metody ponechat v jedné třídě a počkat na další požadavky.
Tento jev popisuje i Martin Fowler v knize
[Refactoring (2nd edition)](https://www.amazon.com/gp/product/0134757599/ref=as_li_tl?ie=UTF8&camp=1789&creative=9325&creativeASIN=0134757599&linkCode=as2&tag=martinfowlerc-20):
"You’ve probably read guidelines that a class should be a crisp abstraction, only handle a
few clear responsibilities, and so on. In practice, classes grow. You add some operations
here, a bit of data there. You add a responsibility to a class feeling that it’s not worth a
separate class—but as that responsibility grows and breeds, the class becomes too
complicated. Soon, your class is as crisp as a microwaved duck."

## Kdy použít starou definici

Nikdy. Nejlepší je pravděpodobně starou definici úplně zapomenout a tvářit se jako že neexistuje.
Stará definice může být reprezentována mnoha způsoby které často neodpovídají originální myšlence SRP.
Naopak nová definice poměrně přesně vystihuje co měl Uncle Bob na mysli když se snažil SRP poprvé popsat.
[Originální článek](https://www.win.tue.nl/~wstomv/edu/2ip30/references/criteria_for_modularization.pdf) ze kterého
vycházel totiž porovnává
změny modulů v aplikaci při změně požadavků.

## SRP jako code smell

Kód který porušuje SRP můžete někdy poznát tak že pro implementaci nových featur vždy musíte upravovat X míst v aplikaci.

## Závěr

* Nepoužívejte definici SRP která zní: "Class or module should have one responsibility" nebo "Class or module should have one, and only one, reason to change". Je nepřesná a může být i nebezpečná.
* Používejte následující definici: "Gather together things that change for the same reasons and at the same times. Separate things that change for different reasons or at different times.". Je novější a lépe vystihuje originální myšlenku.
