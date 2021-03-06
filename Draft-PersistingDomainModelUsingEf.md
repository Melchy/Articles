# Ukládání doménového modelu pomocí EF

Minulý článel vysvětluje proč ukládání je použití Entity Frameworku jeden z nejvhodnějších způsobů ukládání agregátů. Entity Framework
ale neumožňuje mapování doménového modelu na databázový ve všech případech a proto je potřeba použít také ruční mapování. V tomto článku
se podíváme jaké případy dokáže Entity Framework řešit.

## Typycké problémy při ukládání doménového modelu

Při ukládání doménového modelu se můžeme setkat s několika typy agreagátů. Pro jednoduchost je můžeme rozdělit do následujících skupin:

1. Jendoduché agregáty - obsahují pouze kořenovou entitu a jeden nebo více hodnotových objektů.
2. Složité agregáty - obsahují jednu nebo více entit které mohou obsahovat hodnotové objekty.

## Mapování jenoduchých agregatů

Při ukládání jednoduchých agregátů mohou nastat následující případy:

1. Kořenová entita odpovídá některé z tabulek databáze.
2. Kořenová entita obsahuje data z více než jedné tabulky.
3. Kořenová entita obsahuje data pouze z části jedné tabulky.

Prní případ je nejjednodušší EF od verze 2.1 podporuje OwnedTypes které umožňují mapovní hodnotových objektů. Následující příklad ukazuje použití OwnedTypů:

```csharp
public class User
{
    public Guid Id { get; private set; }
    public Address Address { get; private set; }

    private User()
    {
    }
}

public class Address
{
    public PostCode PostCode { get; private set; }

    private Address()
    {
    }
}

//dbContext settings
modelBuilder.Entity<User>().OwnsOne(p => p.Address,
    x=> x.OwnsOne(y=>y.PostCode));

```

Tento příklad také ukazuje že EF podporuje privátní settery, privátní konstruktor i zanořené honotové objekty. Rozdíl mezi běžným doménovým modelem tedy není téměř žádný. Pokud je pro vás privátní konstruktor problém můžete použít i konstruktor s [parametry](https://docs.microsoft.com/en-us/ef/core/modeling/constructors).

V některých případech je nutné aby agregáty odkazovali na jiný kořen agregátu. V těchto případech se často používá odkaz pouze pomocí ID namísto reference na objekt. Entity framework ale vyžaduje vždy použít referenční entity. Není tedy možné vytvořit odkaz pouze pomocí ID.

```csharp
//Doménový model (DDD)
public class User
{
    public Guid Id { get; private set; }
    public Guid ProductAggregateId { get; private set; }
}

//Entity framework
public class User
{
    public Guid Id { get; private set; }
    public Guid ProductAggregateId { get; private set; }
    public Product ProductAggregate { get; private set; }
}
```

Odkazování pomocí referencí by mohlo být považováno za méně "čistý" přístup. Ve skutečnosti ale můžeme najít i argumenty pro použití tohoto [přístupu](https://enterprisecraftsmanship.com/posts/link-to-an-aggregate-reference-or-id/). V praxi ale není příliš důležité který z těchto přístupů je čistější. Důležité je že EF ušetří velké množství mapování a proto je výhodnější použít reference na celé objekty.

Jak už jsem zmínil někdy se může stát že je potřebné jeden agregát namapovat na více tabulek nebo naopak část tabulky na jeden agregát. Mapování jednoho agregátu do více tabulek aktuálně není [podporováno](https://github.com/aspnet/EntityFrameworkCore/issues/620) v tomto případě tedy bude nutné použít ruční mapování. Mapování jedné tabulky na více agregátů je v EF [podporováno](https://docs.microsoft.com/cs-cz/ef/core/modeling/table-splitting) bohužel ale mezi sebou tyto agregáty musejí mít 1:1 vztah což často není žádoucí. V tomto případě je tedy také nutné použít ruční mapování.

## Složité agregáty

U agregátů které mají jednu nebo více vnořených entit máme dvě možnosti. Rozdělení databázových tabulek vztahy 1:1 tak aby odpovídali agregátu nebo použití [rozdělení tabulek](https://docs.microsoft.com/cs-cz/ef/core/modeling/table-splitting). Rozdělení databáze pomocí 1:1 vztahů zhoršuje databázový model a proto bychom měli ve většině případů preferovat rozdělení tabulek entity frameworkem. Použití Ef by mohlo vypadat následovně:

```csharp
public class User
{
    public Guid Id { get; private set; }
    public Basket Basket { get; private set; }
}

public class Basket
{
    public Guid Id{get; private set;}
    public List<CatalogueItem> ItemsInBasket {get; private set;}
}

//dbContext
modelBuilder.Entity<Basket>(dob =>
{
    dob.ToTable(nameof(User));
});

modelBuilder.Entity<User>(ob =>
{
    ob.ToTable(nameof(User));
    ob.HasOne(o => o.Basket).WithOne()
        .HasForeignKey<Basket>(o => o.Id);
});
```

Tento kód vloží uživatele a jeho košík do jedné tabulky. Agregát je tedy rozdělen do dvou entit ale v databázi jsou obě entity uloženy v jedné tabulce.

Složitější případy bude možné namapovat pouze pokud budou poměrně přesně odpovídat databázovým tabulkám.

## Migrace agregátu na entitu

Pokud některý z agragátů není možné namapovat pomocí Entity Frameworku je potřeba vytvořit entitu která odpovídá databázové tabulce a nahradit agregát touto databázovou entitou. Následně je potřeba upravit všechny cizí klíče tak aby ukazovali na nově vytvořenou entitu namísto předchozího agregátu. Na závěr je potřeba upravit repozitář tak aby načetl databázovou entitu a namapoval ji na agregát. Pokud používáte EF migrace je také potřeba nastavit EF tak aby nově vytvořenou databázovou entitu mapovalo na existující tabulku. Zároveň je potřeba nastavit názvy všem cizím klíčům které ukazují na novou entitu tak aby odpovídali aktuálním sloupcům v databázi. Pokud nenastavíme nazvy cizím klíčů EF se pokusí staré klíče odstranit a vytvořit nové s názvy nastaví podle názvu nově vytvořené databázové entity.  

## Příklad na závěr

//TODO priklad
