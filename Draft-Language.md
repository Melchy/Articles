Metafory z realneho sveta
Evolving language - priklad s kosikem
Přesné názvy/pojmenování


Jazyk a programování
Komunikace je jedním z nejdůležitějších nástrojů programátora. Komunikací programátor získává nové informace
a zároveň předává již existující znalosti kolegům v týmu. Komunikace je tedy jednou z nejdůležitějších částí programování a jakékoliv
zlepšení vede k rychlejší implementaci projektu. Důležitou součástí komunikace je správné pojmenování konceptů. Bohuže ale pojmenování 
v programování není vždy jednoduché. 


V pravidlech čistého kódu můžeme najít informaci že pojmenování by mělo být raději dlouhé a jednoznačné než
krátké a nepřesné. Napříkald namísto Divide(x, y) bychom měli použít Divide(dividend, divisor). Jednoduché a zřejmé.
Toto pravidlo ale bohužel není tak jednoduché aplikovat v reálném světě kde neprogramujeme ty nejtriviálnější příklady
s dělením. Co takhle reálnější příklad - řekněme že máme v aplikaci uživatele který má přístup ke skladu a zároveň k
administraci. Pokud se budeme držet již zmíněného pravidla pak je název pro takového uživatele zřejmý 
- UserWithAccessToWarehouseAndAdministration. Pro čtenáře našeho kódu je nyní naprosto zřejmé co tento název vyjadřuje.
Bohužel ale není jednoduché o takové proměnné mluvit s jinými programátory.

Příklad konverzace:
X: Co se má stát když uživatel s přístupem ke skladu a k administraci přistoupí do uživatelské sekce?
Y: To by se nemělo stát k uživatelské sekci mají přístup pouze uživatelé klientské sekce a administrátoři
s přístupem do klientské sekce.
X: To je zřejmé. Zájímá mě ale zda by měl být uživatel s přístupem ke skladu a k administraci odkázán na přihlašovací 
stránku pro uživatele s přístupem ke klientské sekci?
Y: .... (Pokračování rozhovoru bude naznačeno dále)  //--- bude tam že bude chtít vytvořit novou roli která je kombinací těchto 3
ale přitom nedává smysl  

Jak můžeme vidět tak ačkoliv jsme našli dlouhý a přesně definovaný název není jednoduché vést diskuzi s takto dlouhým pojmenováním.
V některých případech se může také stát že název je tak dlouhý že se programátoři jednoduše začnou bavit jen o zkráceném názvu.
Například v předchozí diskuzi by mohli o "uživatel s přístupem ke skladu a k administraci" začít mluvit pouze jako o uživateli.
S trochou štěstí oba programátoři pochopí o čem se baví podle kontextu. Někdy ale může dojít k nedorozumění díky záměně pojmů.
Zároveň je konverzace pro programátory obtížnější jelikož si musí stále uvědomovat o kterém uživateli mluví. Dalším problémem
je navázání na předchozí konverzaci. Pokud další den přijde X za Y a řeknemu "pamatuješ jak jsme se včera bavili o tom uživateli?".
Pro Y bude poměrně náročné si uvědomit co tím X myslí.

Dlouhé názvy jsou sice přesné ale často nejsou vhodné pro konverzaci.

Můžeme si představit stejnou situaci tentokrát ale tým rozhodl udělat něco radikálního. Namísto použití proměnné 
UserWithAccessToWarehouseAndAdministration se rozhodli vytvořit název pro tuto roli. A zároveň pojmenovat další role
vystupující v jejich systému. Tyto role jsou:
uživatel s přístupem k administraci -- admin (admin)
uživatel s přístupem ke klientské sekci -- customer (zákazník)
uživatel s přístupem ke skladu -- warehouseman (skladník)
uživatel s přístupem ke skladu a k administraci -- mainWarehouseman (hlavní skladník)
administrátoři s přístupem do klientské sekce -- mainAdmin (hlavní administrátor)

Tyto názvy mohli být vytvořeny ve spolupráci s klientem tak aby odpovídali reálnému světu. Předchozí konverzace je najednou mnohem 
jednodušší:
Konverzace:
X: Co se má stát když hlavní skladník přistoupí do zákaznické sekce?
Y: To by se nemělo stát k zákaznické sekci mají přístup pouze zákaznici a hlavní administrátoři.
X: To je zřejmé. Zájímá mě ale zda by měl být hlavní skladník odkázán na přihlašovací stránku pro zákazníky?
Y: ...

Můžeme vidět že rozhovor je najednou mnohem kratší a jasnější. Duležité ale je i to že pokračování rozhovoru by pravděpodobně ani nevedlo
k tvorbě další role jelikož k tomu nenavádí pojmenování podle jednotlivých oprávnění.

Nyní si můžete říkat že je zřejmé že v systému je potřeba pojmenovat jednotlivé role. Avšak pojmenování 
rolí sebou nese nevýhodu. Programátor který není seznámen s tím jaké opravnění daná role má bude mít problém
vyznat se v kódu a porozumět rozhovoru.

Jakoukoli věc v programu můžeme pojmenovat náhodným názvem. Což povede ke zjednodušení rozhovoru a kódu pokud jsou všichni
programátoři seznámeni s tímto pojmenováním.

Co to znamená? Jednoduše jsem mohl předchozí role pojmenovat X,Y,Z,V,W a rozhovor by byl stejně jasný pro někoho kdo ví 
že X je uživatel s přístupem ke skladu, Y - uživatel s přístupem k administraci a tak dále.          

Použít dlouhý název který vede ke kódu který jde pochopit bez předchozích znalostí ale je obtížné o něm hovořit. 
Použít jednoduchý název o kterém se dá dobře hovořit ale je méně pochopitelný jelikož vyžaduje znalosti programátora.


Nyní se můžeme podívat zpět na příklad s dělením.
Je tedy metoda Divide(dividend, divisor) lepší než Divide(x, y)?
Pokud někdo nezná pojmy dividend, divisor potom je pro něj lepší nebo alespoň stejně dobrá varianta s Divide(x, y).
Naopak pokud pojmy dividend a divisor zná bude jednodušší Divide(dividend, divisor).

Druhý příklad který jsem uvedl byla název UserWithAccessToWarehouseAndAdministration a mainWarehouseman.
Můžeme se opět ptát je název UserWithAccessToWarehouseAndAdministration lepší než mainWarehouseman?
Zde je odpověď komplikovanější proto nyní nebudu odpovídat. To zajímavé ale je že mezi příkladem
Divide(dividend, divisor)/Divide(x, y) a UserWithAccessToWarehouseAndAdministration/mainWarehouseman je velký rozdíl.

Všiměte si že pro pochopení Divide(dividend, divisor) potřebuje předchozí znalosti o tom co je dividend a divisor.
Naopak ve druhém příkladu programátor nepotřebuje žádné předchozí znalosti jelikož všechny potřebné informace jsou 
již obsaženy v názvu proměnné UserWithAccessToWarehouseAndAdministration.
Aby tyto příklady byly stejné museli bychom metodu dělení změnit tak aby vypadala takto:
Divide(numberThatWillBeDivided, numberThatWillDivide) - nyní programátor nepotřebuje žádné znalosti pro pochopení 
významu parametrů.

Proč tedy pojmenovávat dělení Divide(dividend, divisor) namísto Divide(numberThatWillBeDivided, numberThatWillDivide)?
Jednoduše proto že se očekává že progrmátor má znalosti matematiky a očekává se že bude znát pojmy dividend, divisor.
Informace o významu proměnných tedy nemusejí být v názvu proměnné jelikož programátor vý jaký význam mají 
pojmy dividend, divisor. Zároveň jsou tyto názvy dostatečně jednoduché abychom se o něm mohli jednodušše bavit. 

Vyřešili jsme tedy dilema zda použít krátký název vyžadující předchozí znalosti nebo dlouhý název přesně vystihující význam.
Řešením bylo využít pojem jehož význam programátor již zná a je dostatečně krátký. Toto řešení je ideálná případ a můžeme
ho použít vždy když programujeme něco o čem mají znalosti všichni programátoři.

Při pojmenovávání vždy používejte předchozí znalosti programátorů.

Další příklady tohoto pojmenovávání:
Na místo dlouhého přesného pojmenování Tree.IncreaseHeightAndWidth() můžeme využít předchozí znalosti programátorů o růstu stromů
a použít krátký název Tree.Grow()
Stejně tak na místo dlouhého Person.GetTimeSincBorn() můžeme použít krátké Person.GetAge()

Tyto pojmenování jsou samozřejmostí snad pro všechny programátory. Je ale důležité si uvědomit že i v těchto případech používáme
předchozí znalosti. 

##Doménové názvy
Každá aplikace kterou píšeme modeluje nějakou doménu reálného světa. Doménou aplikace může být například půjčování peněž, kovoobrábění,
prodej zboží. Doména je tedy ta oblast kterou se naše aplikace zabývá. 

V doméně obvykle existuje mnoho specifických názvů jako jsou věřitel, dlužník, insolvence atd.
Tyto názvy můžeme pojmenovat jako doménový jazyk. Pokud bychom psali aplikaci která pracuje s 
půjčováním peněz mohlo bychom se rozhodnout že nepoužijeme doménové názvy jelikož jsou příliš složité a běžný programátor nemusí jejich
definici znát. 


Pro jednoduchost dalšího textu použiji název alias pro názvy které jsou krátké a využívají znalosti programátorů.

Další případ kdy je vhodné použít alias je u názvů vyskytujících se v doméně aplikace. Například pokud vytváříme aplikaci
která modeluje půjčování peněz je vhodné v takové aplikaci použít názvy jako je věřitel, dlužní, insolvence atd. 






Dalším případem kdy je vhodné použít názvy namísto přesného pojmenování je použití modelování objektů z domény aplikace. 

kdyz o tom mluvim se zakaznikem.
kdyz to reprezentuje obecnou znalost - dividend, divisor
kdyz o tom casto mluvime -
docasne pojmenovani kdyz to piseme - toto pojmenovani vede i k odstraneni byasu.



Jak pojmenovávat?




Ideální případ je tedy takový kdy má název přesně existuje v reálném světě. 




Předpoklady a očekávání
Ke každému pojmenování se vážou určité předpoklady a očekávání. Typicky očekáváme že sčítání je komutativní tedy že 3+2=2+3.
Pokud tedy definujeme metody Add a Subtract měly by odpovídat tomuto očekávání. Pokud metody neodpovídají je vhodné 
použít jiný název například Advance namísto Add.

Názvy sebou nesou jisté předpoklady je důležité s těmito předpoklady počítat.


Každé pojmenování sebou nese předpoklady které nám může znepříjemnit další programování. Ukažme si pokračování předchozího rozhovoru. 

Konverzace:
X: Co se má stát když uživatel s přístupem ke skladu a k administraci přistoupí do uživatelské sekce?
Y: To by se nemělo stát k uživatelské sekci mají přístup pouze uživatelé klientské sekce a administrátoři
s přístupem do klientské sekce.
X: To je zřejmé. Zájímá mě ale zda by měl být uživatel s přístupem ke skladu a k administraci odkázán na přihlašovací 
stránku pro uživatele s přístupem ke klientské sekci?
Y: To vypadá jako případ na který jsme nemysleli. Měli bychom přidat uživatele s přístupem ke skladu a k administraci a k 
uživatelské sekci. Uživatel s přístupem ke skladu a k administraci by pak mohl být přesměrován na stránku která ho informuje
o tom že musí požádat hlavního administrátora o přístup.


V tomto případě díky názvu uživatel s přístupem ke skladu a k administraci došel Y k tomu že musejí pokrýt
další případ opravnění. Y přemýšlel nad rolemi jednoduše tak že je ideální aby byly pokryté všechny kombinace.
K této myšlence ho vedlo pojmenování proměnné UserWithAccessToWarehouseAndAdministration.








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