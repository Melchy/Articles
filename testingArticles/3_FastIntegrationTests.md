# Fast Integration tests

V minululém díle jsme popsali vlastnosti rychlích integračních testů. Ještě jednou tyto vlastnosti zopakuji:

* Jsou rychlejší než integrační testy ale pomalejší než unit testy.
* Jsou méně křehké než unit testy ale křehčí než integrační testy. (Křehkost určuje jak často se testy rozbíjejí při refactoringu kódu.)
* Vyžadují pouze minimální nebo žádný setup
* Vyžadují minimum externích závislostí
* Jsou spolehlivější než unit testy ale méně spolehlivé než integrační. (Vyšší spolehlivost ukazuje že testy najdou větší množství chyb.)

V tomto díle se podíváme jak docílit těchto vlastností.

## Od integračních testů k rychlím integračním testům

Integrační testy jsou obvykle pomalé a vyžadují setup kvůli externím závislostem (např. databáze). Pokusíme se tedy zbavit externích závislostí pomocí mocků a tím zvýšit rychlost a odstranit náročnost na setup.

Vedlejším efektem při použití mocků je snížení spolehlivosti a zvýšení křeshkosti. Spolehlivost se sníží jelikož mock nahradí část kódu která nikdy nebude otestována a
křehkost se zvyší jelikož při změnách kódu budeme muset opravovat rozbitý mock.

Jelikož chceme křehkost a spolehlivost ovlivnit co nejméně tak použijeme mocky pouze na hranách aplikace. Co přesně jsou hrany aplikace si ukážeme na následujícím příkladu který ukazuje registraci uživatele s odesláním emailu.

```csharp
public class UserService{
    public void Register(string email){
        var user = new User(){Email = email};
        // uložení uživatele do databáze by mělo být v tomto místě ale v tuto chvíli se zaměříme pouze na odesílání emailu
        _emailService.SendRegistrationEmail(email);
    }
}

public class EmailService{

    public void SendRegistrationEmail(string reciever){
            var message = new MimeMessage ();
            message.From.Add (new MailboxAddress ("joey@friends.com"));
            message.To.Add (new MailboxAddress (reciever));
            message.Subject = "...";
            message.Body = new TextPart ("plain") {
                Text = "...."
            };
			using (var client = new SmtpClient ()) {
				client.Connect ("smtp.friends.com", 587, false);
				client.Send (message);
				client.Disconnect (true);
			}
    }
}


```

V běžných testech bychom nahradili metodu SendRegistrationEmail mockem a kvůli tomu by tělo této metody nebylo otestováno a jakákoliv změna parametrů by vedla k rozbití mocku.

Pro rychlé integrační testy nahradíme mockem pouze třídu SmtpClient. SmtpClient je ale třídou z knihovny a neumožňuje nám jednoduše nahradit jednotlivé metody (nejsou virtual).
Pro vytvoření mocku tedy využijeme pattern zvaný Humble object. Humble object je jednoduchý objekt který neobsahuje žádnou logiku a pouze provolává jinou třídu.
Ukázka mockování pomocí humble objectu: //TODO precist si o humle objectu jestli to je spravna terminologie

```csharp
public class UserService{
    public void Register(string email){
        var user = new User(){Email = email};
        _userService.SaveUser(email);
        _emailService.SendRegistrationEmail(email);
    }
}

public class EmailService{

    public void SendRegistrationEmail(string reciever){
            var message = new MimeMessage ();
            message.From.Add (new MailboxAddress ("joey@friends.com"));
            message.To.Add (new MailboxAddress (reciever));
            message.Subject = "...";
            message.Body = new TextPart ("plain") {
                Text = "...."
            };
			using (var client = new SmtpClient ()) {
				client.Connect ("smtp.friends.com", 587, false);
				client.Send (message);
				client.Disconnect (true);
			}
    }

    public class ApplicationSmtpClient{
        //.. todo
    }
}


```

Nyní můžeme podědit ApplicationSmtpClient a vytvořit ApplicationSmtpClientSpy který pouze ukládá odeslané emaily.

```csharp
    //todo příklad spy

```

A výsledný test pak bude vypadat následujícím způsobem:

```csharp
//TODO
```


//TODO co vlsatne potrebujem?
Tento způsob mockování je vhodný pokud dokážeme jednodušše a věrně namockovat všechny odpovědi externí služby. Mockování některých služeb ale není jednoduché a proto můžeme zvolit jiný způsob mockování. Typickým příkladem je SQL databáze. SQL databáze může vrátit nekonečné množství odpovědí které jsou závislé na předchozích voláních databáze. Oproti tomu smtp klient vrací asi 10 druhů odpovědí a není závislý na předchozích voláních. Jak asi můžete tušit tak namockovat SQL databázi pomocí humble objektů bude velice náročné. U smtp klienta se v 99% případů spokojíme s tím že mock nehodí exception a pouze uloží odeslanou zprávu kterou na konci testu zkontrolujeme a zjistíme jestli obsahuje správná data.


 Vyjímkou je například SQL databáze. Abychom mohli vytvořit mock SQL databáze museli bychom
namockovat celý SQL jazyk. Asi už tušíte že něco takového není v silách běžného programátora. Naštěstí si ale můžeme v těchto případech pomoci jinými technikami mockování.

## Mockování databáze

Při mockování SQL databáze můžeme použít jednu z následujících možností:

* Použití mocku (bez huble object patternu)
* Použití in memmory providera
* Použití SQL lite in memmory
* Použití reálné databáze
* Použití transakcí
* Kombinace SQL lite a transakcí

## Použití mocku

Při mockování databáze nemůžeme použít humble object ale můžeme použít se spokojit s běžným mockem. Abychom mohli namockovat databázi musíme všechny volání
schovat za abstrakci a tu pak nahradit v testech. Zakrývání databáze vrstvou kódu je tak časté že existuje název pro tento pattern - Repository. V aplikaci tedy
vzniknou repozitáře které mají metody jako například `GetUserById`, `GetUserWithAllProductsInCart` atd. Repozitáře pak můžeme jednodušše namockovat. 

Výhodou dohoto přístupu je že k spuštění testů není potřeba mít nainstalovanou SQL databázi a také rychlost testů. Tím ale výhody končí a přichází nevýhody.
Kód repozitářů může být často poměrně komplikovaný a poměrně často se stává že obsahuje bugy. Mockování repozitářů tedy značně snižuje spolehlivost testů. Druhou
nevýhodou je o něco větší křehkost.

## Použití in memmory providera

Pokud používáte entity framework tak je možné pro testování využít [inmemmory providera](https://entityframeworkcore.com/providers-inmemory). InMemmory provider
funguje tak že v testech nakonfikurujete aplikaci tak abz namísto SQL databáze volala inmemmory providera. InMemmory provider pak slouží jako mock databáze který je přímo v paměti.

Tento způsob mockování je rychlý, nevyžaduje setup a má nízkou křehkost. Oproti mockování repozitářů jsou také testy přehlednější jelikož není potřeba vytvářet mocky.
Jedinou nevýhodou je že inmemmory provider nemá stejné vlastnosti jako SQL databáze. Například nekontroluje existenci cizích klíčů . Často tedy narazíte na situace
kdy chování inmemmory databáze neodpovídá reálnému serveru.

Více o rozdílech mezi databází a inMemmory providerem si můžete přečíst [zde](https://jimmybogard.com/avoid-in-memory-databases-for-tests/).

## Použití SQL lite in memmory

Entity framework nabízí také SQL lite providera ktero můžeme použít v testech. SQL lite databáze totiž umožňuje ukládání data do paměti bez uložení na disk.
Sql lite provider tedy funguje téměř stejně jako inmemmory provider. Má tedy stejné výhody a nevýhody. Jediným rozdílem je že SQL lite podporuje mnohem více SQL funkcí.

SQL lite je tedy téměř ve všech ohledech lepší než inMemmory provider. Návod na použití SQL lite in memmory najdete [zde](https://www.meziantou.net/testing-ef-core-in-memory-using-sqlite.htm).

SQL lite provider ale má ale také několik nevýhod. Pokud jako váší databázi používáte MSSQL tak musíte počítat s tím že SQL lite je jiná databáze a nepodporuje všechny
funkce z MSSQL. Například SQL lite nepodporuje datovýtyp DateTime a proto je pro tyto sloupce použit datový typ string. Problém pak nastane pokud potřebujeme seřadit datumy.
SQL lite jednoduše provede seřazení řetězců podle abecedy což neodpovídá seřazení podle data a času.

Další nevýhodou stejně jako u InMemmory providera je že není možné psát čisté SQL bez entity frameworku. Jazyk SQL v SQL lite se liší od jazyku SQL který je použit v MSSQL.
Z tohoto důvodu je mnoho SQL dotazů nekompatibilních.

## Použití reálné databáze (naivní přístup)

Použití reálné SQL databáze sebou nese všechny problémy které vyplývají z integračních testů. Programátor musí mít nainstalovanou
SQL databázi, testy jsou pomalé, testy je nutné spouštět sériově, mezi průběhy testů je potřeba mazat data v databázi.

Výhody jsou pak stejné jako u in memmory databází s tím rozdílem že je možné testovat i SQL bez entity frameworku a také že testy přesně odpovídají skutečnosti (mají tedy maximální
spolehlivost).

Z předchozího popisu se použítí reálné databáze zdá jako jedna z nejhorších variant a proto uvedu několik typů které vám ulehčí práci s reálnou databází.

1. Pro mazání dat mezi testy můžete použít nástroj [respawn](https://github.com/jbogard/Respawn) který dokáže jendodušše a rychle smazat všechna data z databáze.
2. Pokud programujete v C# tak je velice pravděpodobné že máte nainstalovanou localdb. Tu můžete použít pro testování a díky tomu programátor nemusí nic instalovat.
3. Pro sériové spuštění testů a připravení setupu (vytvoření tabulek databáze atd.) doporučujeme NUnit.

## Použití reálné databáze (optimalizovaný přístup)

Předchozí přistup při použití reálné databáze je používán nejčastěji. Existují ale i alternativy které dokáží několikanásobně zvýšit rychlost testů. Nejjednoduším způsobem je pro každý test vytvořit novou databázi a na konci testu jí smazat. Tento přístup umožní testy spustit paralelně což vede k mnohanásobnému zryhlení.

Dalším druhem optimalizace je spuštění testů paralelně proti poolu databází které jsou přiřazovány podle toho jestli jsou zrovna volné.

## Použití transakcí

Při testování je možné vytvořit na začítku testu transakci poté provést všechny operace a nakonec transakci zahodit. Tímto způsobem umožníme paralelní spuštění testů
jelikož do databáze nikdy nedostaneme žádná data. Obecně se transakce chovají velice podobně jako kdybychom data reálně ukládali do databáze.

Transakce se stejně jako použití InMemmory providera může lišit od reálné databáze. Problém může nastat pokud aplikace používá transakce. V těchto případech test musí vytvořit zastřešující transakci která obsahuje vnořené transakce ([(Nested transaction](https://stackoverflow.com/questions/527855/nested-transactions-in-sql-server)). Zastřešení transakcí pak může způsobit rozdílné chování oproti reálné databázi. Kromě tohoto problému osobně nevím o žádném rozdílu.

Transakce mají tedy téměř stejné výhody a nevýhody jako použití reálné databáze s optimalizovaným přístupem. Mezi těmito dvěmi přístupy jsme neprováděli benchmark ale můžeme očekávat že transakce budou o něco málo rychlejší.

## Kombinace SQL lite a transakcí

Možná by vás mohlo napadnout zkombinovat některé z výše uvedených přístupů pro ještě lepší výsledek. Nejvíce se nabízí kombinace SQL lite a transakcí. Testy spuštěné proti Sql lite nebo v transakci se můžou lišit od chování reálné databáze. Můžeme tedy každý test spustit dvakrát. Jednou proti SQL lite a jednou v transakci. Pokud jsou výsledky rozdílné tak víme že jeden nebo oba z testů se chovají jinak než reálná databáze.

Tento přístup k testování se zdá zajímavý avšak realita je taková že spuštění dvou testů je ve výsledku pomalejší než spuštění testu proti reálné databázi.

## Jednoduchý benchmark

Důležitou otázkou je jak moc pomalé je použití SQL databáze oproti jiným variantám. Náš jednoduchý benchmark ukazuje následující výsledky:

Použití reálné databáze s optimalizací - jeden test: 2-5 sec, 148 testů: cca 35 sec
Použití inmmemory databáze - jeden test: 1 - 4 sec (často 1 sec), 148 testů: cca 30 sec  (7 testů které failnuly kvůli odlišnému chování)

## Naše volba

V naší aplikaci jsme vyzkoušely téměř všechny druhy databázových přístupů a nakonec jsme se rozhodli použít reálnou databázi s optimalizovaným přístupem. Ostatní způsoby přináší příliš malé zrychlení na to aby kompenzovali jejich nespolehlivost.

Jedna z našich starších aplikací má velice složité databázové schéma a nejsme schopni databázi nastavit do potřebného stavu pro testování. V této situaci jsme tedy zvolili mockování repozitářů které vrací předpřipravená data.

## Závěr

V tomto díle jsme se podívali na to jak přistupovat k externím závislostem při testování. Popsali jsme také koncept rychlého integračního testu což je v určitém smyslu kompromis mezi integračním testem a unit testem.

















Abychom zvýšili rychlost a snážili náročnost na setup je tedy potřeba zbavit se externích závislostí. Externích závislostí se zbavíme stejně jako v unit testech - mockováním.
//End jak vytvorit mock nebudu popisovat

Volání externí služby schováme za třídu a tu pak nahradíme mockem v testech.

Je důležité aby nově vytvořená třída neobashovala žádnou logiku a sloužila pouze k provolávání externí služby. Logika v této třídě by nebyla otestována. Následující příklad ukazuje jak bychom mohli upravit službu k odesílání emailů abychom ji mohli otestovat.

```csharp

public class EmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService(SmtpClient smtpClient)
    {
       _smtpClient = smtpClient;
    }

    public virtual async Task Send(EmailMessage emailMessage)
    {
        using var mailMessage = new MailMessage(emailMessage.From, emailMessage.To);
        mailMessage.Subject = emailMessage.Subject;
        mailMessage.Body = emailMessage.Body;
        await _smtpClient.SendMailAsync(mailMessage);
    }
}

```

Tuto službu upravíme následujícím způsobem:

```csharp
public class EmailService
{
    private readonly ISmtpSender _smtpSender;

    public EmailService(ISmtpSender smtpSender)
    {
        _smtpSender = smtpSender;
    }

    public virtual async Task Send(EmailMessage emailMessage)
    {
        using var mailMessage = new MailMessage(emailMessage.From, emailMessage.To);
        mailMessage.Subject = emailMessage.Subject;
        mailMessage.Body = emailMessage.Body;
        await _smtpSender.SendMailAsync(mailMessage);
    }
}

public class SmtpSender : ISmtpSender
{
    private readonly SmtpClient _smtpClient;

    public SmtpSender(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task SendMailAsync(MailMessage mailMessage)
    {
        await _smtpClient.SendMailAsync(mailMessage);
    }
}

public interface ISmtpSender
{
    Task SendMailAsync(MailMessage mailMessage);
}
```

ISmtpSender bychom poté nahradili v testu.

## Mockování databáze

Volání databáze je odlišné od většiny externích závislostí. Pro dotazování obykle používáme SQL namísto přesně definovaného API. Z tohoto důvodu nemůžeme namockovat všechny volání databáze. Můžeme ale zakrýt všechny volání za vrstvu tříd a ty poté namockovat. Tyto třídy se obvykle nazívají repozitáře.

```csharp
public class UserService{
    public IUserRepository _userRepository;

    public UserService(IUserRepository userRepository){
        _userRepository = userRepository;
    }
    public void DoSomethingWithUser(Guid userId){
        var user = _userRepository.GetById(userId);
        // Some operation....
    }
}
```

Repozitáře pak můžeme namockovat stejně jako interface v předchozím případě s emailem. Problém ale je že repozitář se poměrně často mění a při každé úpravě je potřeba změnit i mock. Další nevýhodou je že nemůžeme otestovat obsah repozitářů jelikož v testech používáme jejich mocky. Z těchto důvodů je vhodné použít jiný způsob mockování databáze.

Na výběr máme z následujících možností:

1. Inmemmory providera pro entity framework
2. SQL lite in memmory
3. Transakce
4. Kombinace tranakcí a in memmory
5. Spouštění testů proti SQL serveru

Inmemmory provider není vhodný pro testování jelikož se velice liší od produkční databáze. Hlavním rozdílem je že inmemmory provider vůbec nekontroluje integritu cizích klíčů.

SqlLite je mnohem blíž produkční databázi ale má také několik nevýhod - narozdíl od MSSQL je porovnání stringu case sensitive, nepodporuje řazení podle DateTime a další.

Transakce můžeme k testování využít následujícím způsobem - na začátku testu otevřeme transakci poté provedeme test a nakonec transakci zahodíme. Obsah databáze tedy není nikdy změněn a testy mohou běžet paralelně. Problémem transakcí je že ke spuštění testů potřebujeme reálnou databázi s aktuálním schématem. Další nevýhodou je že i transakce se liší od produkční databáze - např. Unique constraint není zvalidován.

Další možností testování je kombinace inmemmory a transakcí. Poměrně jednodušše můžeme napsat infrastrukturu která spouští každý test dvakrát - jednou proti inmemmory a jednou v transakci. Pokud je výsledek testů shodný v obou spuštění tak víme že se databáze chová stejně jako produkční. Pokud je výsledek odlišný pak víme že jeden z testů se chová jinak než by se zachovala produkční databáze. V těchto případech nemáme jinou možnost než napsat integrační test.

Poslední možností je spuštění testů proti SQL serveru. Nevýhodou tohoto přístupu je že testy musejí běžet seriově což je často pomalé. Máme ale jistotu že se testy chovají přesně jako v produkci.

## Kterou možnost vybrat?

Osobně jsem vyzkoušel všechny z vyjmenovaných možností a nakonec se ukázalo že spouštění testů proti reálné databázy je nejlepší. První tři varianty jsme po několika měsících zavrhli kvůli rozdílům s produkční databází.

Čtvrtá varianta vypadala velice nadějně avšak po čase se ukázalo že je pomalejší než spouštění testů seriově proti reálné databázi.

## Obecné tipy

Pro mazání databáze se nám osvědčil Respawn. //TODO odkaz

Lokálně testy spouštíme proti localDb a v Ci pipeline proti Sql Express.

Pro vytvoření aktuálního schématu používáme následující kód:

//ToDO code

Infrastrukturu pro spouštění testů různými způsoby můžete najít zde:

//TODO github odkaz

Je lepsi pouzit NUNIT

## Závěr

Fast integration tests se snaží testovat celou aplikaci bez externích závislostí. Databázi není možné namockovat bez kompromisů a proto raději volíme reálnou nebo inmemmory databázi.

V příštím díle se podíváme na to jak volat aplikaci v rychlích integračních testech.
