## Jednořádkové metody

V některých případech je velice užitečné používat jednořádkové metody k lepšímu vysvětlení kódu
čtenáři. Například následující kód nemusí být na první pohled jednoduše čitelný:

```csharp
public IEnumerable<Products> GetProducts()
{
    //...
    var orderedProducts = products.OrderBy(x => !x.IsDisabled);
    //...
}    
```

`products.OrderBy(x => !x.IsDisabled)` přesune disablované produkty na konec seznamu.
Tento kód není nijak složitý, přesto na první pohled vyjadřuje něco jiného, než co
od metody `OrderBy` běžně očekáváme.

Abychom čtení kódu zjednodušili, můžeme použít jednořádkovou metodu:

```csharp
public IEnumerable<Product> PlaceDisabledProductsToTheEndOfCollection(this IEnumberalbe<Product> products){
    return products.OrderBy(x => !x.IsDisabled);
}
```

Přestože je název metody delší než řazení samotné, tak to nevadí. Podstatné je, že metoda lépe vyjadřuje,
co se snažíme udělat.

### Házení kostkou

Pokud bychom programovali hru, ve které se hází kostkou, tak bychom
někde v kódu museli generovat náhodné číslo pro hod:

```csharp
public void DoMove(){
    //...
    var step = random.Next(1,6);
    //...
}

```

V tomto případě bychom také mohli generování čísla nahradit jednořádkovou metodu:

```csharp
public int ThrowADice(){
    return random.Next(1,6);
}
```

Touto metodou navádíme čtenáře, aby si představil reálné házení kostkou a tím i hru, kterou programujeme.

### Smalltalk od Martina Fowlera

Další příklad uvádí [Martin Fowler](https://martinfowler.com/bliki/FunctionLength.html):
"Smalltalk in those days ran on black-and-white systems. If you wanted to highlight some text or graphics,
you would reverse the video. Smalltalk's graphics class had a method for this called 'highlight',
whose implementation was just a call to the method 'reverse'. The name of the method was longer than its
implementation -
but that didn't matter because there was a big distance between the intention of the code and its implementation."

V C# by mohl tento příklad vypadat následovně:

```csharp
public void Main()
{
    ///...
    _graphic.Reverse(text);
    ///...
}
```

Kentu Backovi se tento kód nezdál dostatečně čitelný, a proto přidal následující metodu:

```csharp
public void Higlight(string text)
{
    _graphic.Reverse(text);
}
```

### Složité podmínky

Dalším místem, kde je možné použít jednořádkové metody jsou složité Ify.
Například:

```csharp
///...
if(
(package.Width - epsilon) < slot.With  && 
(package.Height - epsilon) < slot.Height &&
package.State == PackageState.ReadyToPutInSlot &&
package.IsPayed &&
package.Slot != null
{
    ///... put package to slot
}
///...
```

Tento kód bychom mohli přepsat následujícím způsobem:

```csharp
///...
if(package.FitsInSlot(slot)
package.State == PackageState.ReadyToPutInSlot &&
package.IsPayed &&
!package.HasAsignedSlot()
{
    ///... put package to slot 
}
///...
```