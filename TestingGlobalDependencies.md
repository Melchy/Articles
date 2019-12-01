# Testování globálních závislostí v C# od verze 6.0

Legacy systémy často obsahují mnoho globálních závislotí, které není možné testovat. Obvyklým řešením tohoto problému je refaktorování. Refaktorování ale může do kódu přidat velké množství bugů. Z tohoto důvodu je lepší nejdříve kód otestovat a až později refaktorovat. Tento postup nás zavede do nekonečného kruhu.

> Legacy kód nelze otestovat -> je potřeba refactoring -> před refactoringem je potřeba napsat testy ->
> legacy kód nelze otestovat -> ...

Řešením tohoto problému je udělat jen minimum úprav, abychom dostali kód do testovatelného stavu a následně napsat testy. V tomto článku ukáži způsob, který umožní testovat globální proměné použitím naprostého minima změn.

## Odstranění globální proměné

Odstranění globální proměné si ukážeme na následujícím příkladu. V tomto příkladu má uživatel závislost na globální servise, která se stará o odesílání emailů.

```csharp
//File UserService.cs
using Email;

public class UserService{
    public ResetUserPasword(User user, string newPassword){
        user.password = newPassword;
        EmailSender.SendEmail(user.email, "Password changed");
    }
}
```

```csharp
//File EmailSender.cs
namespace Email{
    public static class EmailSender{
        public static void SendEmail(string email, string emailText){
            //email sending logic
        }
    }
}
```

Prvním krokem pro odstranění závislosti je přidání `using static`. [Using static](https://docs.microsoft.com/cs-cz/dotnet/csharp/language-reference/keywords/using-static) bylo do C# přidáno ve verzi 6.0 a umožňuje použití statických metod bez jmenování typu. Předchozí příklad upravíme následovně:

```csharp
using Email;
using static EmailSender; //using static

public class UserService{

    public ResetUserPasword(User user, string newPassword){
        user.password = newPassword;
        //Nyní můžeme použí SendEmail namísto EmailSender.SendEmail
        SendEmail(user.email, "Password changed");
    }
}
```

Dalším krokem je vytvoření virtuální metody `SendEmail` uvnitř `UserService`.

```csharp
using static EmailSender;

public class UserService{

    public ResetUserPasword(User user, string newPassword){
        user.password = newPassword;
        SendEmail(user.email, "Password changed");
    }

    public virtual void SendEmail(string email, string emailText){
        EmailSender.SendEmail(email, emailText);
    }
}
```

Metoda `SendEmail` zakryje globální metodu `EmailSender.SendEmail`, což způsobí, že každé volání `SendEmail()` použije metodu uvnitř `UserService` namísto `EmailSender.SendEmail`. Zakrytí globální závislosti umožní namokovat metodu `SendEmail`.

```csharp
[Fact]
public void mock_global_dependency()
{
    var mock = new Mock<UserService>();
    mock.Setup(x=> x.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Returns();

    var userService = mock.Object();

    var user = new User(){Password = "test@test.cz"}
    userService.ResetUserPasword(user, "new@email.cz"); //no email is sent thanks to mock

    Assert.Equal(user.email, "new@email.cz");
}
```

Po napsání několika testů můžeme provést větší refaktoring bez strachu, že se nějaká část systému rozbije. Při refactoringu můžeme odstranit globální závislosti a následně odstranit `using static`.

## Závěr

V legacy systémech je možné zneužít `using static` pro dočasné mokování globálních závislostí bez obavy z rozbití již existujícího kódu. Následně je možné legcy kód otestovat, zrefaktorovat a poté odstranit `using static`. Tento přístup je možné použít od verze C# 6.
