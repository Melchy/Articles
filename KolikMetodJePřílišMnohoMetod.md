# Jak dlouhá metoda je příliš dlouhá metoda

Uncle bob v jeho knize Clean Code píše že většina funkcí by měla mít méně než 20 řádků.
Lidé na stack overflow uvádějí že délka metody by měla mít [5-15](https://softwareengineering.stackexchange.com/a/133406) řádků. Další post pak uvádí méně než [35](https://softwareengineering.stackexchange.com/a/27802) řádků. Z těchto infromací můžeme vyvodit že většina metod by měla mít méně než cca 35 řádků.

U metod kratších než 35 řádků nemůžeme přesně určit jak dlouhé by měli být. Můžeme se ale řídit následujícím pravidlem: Pokud musíte strávit čas koukáním na kus kódu abyste pochopili co dělá tak byste ho měli extrahovat do metody a pojmenovat ji tak aby popisovala své chování. [1](https://martinfowler.com/bliki/FunctionLength.html)

## Jednořádkové metody

Někteří lidé neradi používají jednořádkové metody jelikož se jim zdají zbytečné. Můžeme ale najít mnoho "one-linerů" které nejsou na první pohled čitelné. Například
`products.OrderBy(x => !x.IsDisabled)` je velice zvláštní kus kódu u kterého si většina lidí uvědomí co dělá až po chvíli zkoumání. Pro tento případ je vhodné udělat jednořádkovou metodu:

```csharp
public IEnumerable<Product> PlaceDisabledProductsToTheEnd(this IEnumberalbe<Product> products){
    return products.OrderBy(x => !x.IsDisabled);
}

```

`products.PlaceDisabledProductsToTheEnd();` je mnohem čitelnější než předchozí verze `products.OrderBy(x => !x.IsDisabled)`.

Pokud programujeme hru ve které se hází kostkou tak můžeme kód `random.Next(1,6)` vložit do metody `ThrowADice()`. `random.Next(1,6)` je sice čitelné ale pomocí `ThrowADice()` navádíme čtenáře aby si představil reálné házení kostkou a tím i hru kterou programujeme.

Další příklad uvádí [Martin Fowler](https://martinfowler.com/bliki/FunctionLength.html): "Smalltalk in those days ran on black-and-white systems. If you wanted to highlight some text or graphics, you would reverse the video. Smalltalk's graphics class had a method for this called 'highlight', whose implementation was just a call to the method 'reverse'. The name of the method was longer than its implementation - but that didn't matter because there was a big distance between the intention of the code and its implementation."

## Závěr

* Většina metod by měla být kratší než 35 řádků
* "If you have to spend effort into looking at a fragment of code to figure out what it's doing, then you should extract it into a function and name the function after that “what”."
* Nebojte se vytvářet jednořádkové metody.
