# Benchmarks

Deux scénarios, rendus avec le moteur de rendu HTML côté serveur de Blazor (`HtmlRenderer`), pour FractalBlazor, MudBlazor, Radzen et Fluent UI (Microsoft) : une liste de lignes simples, puis une pile d'éditeurs. Dans les deux, `FirstRender` instancie et rend tous les composants, et `Rerender` change une valeur de chaque ligne d'une liste déjà rendue.

BenchmarkDotNet 0.15.8, MediumRun (2 lancements, 15 itérations), .NET 10.0.12, Intel Core i9-9900K, Windows 10. MudBlazor 9.11.0, Radzen.Blazor 12.0.5, Microsoft.FluentUI.AspNetCore.Components 5.0.0. Mesures du 13 octobre 2026.

## Liste de lignes (`RenderBenchmarks`)

Rendu de 1 000 lignes encadrées contenant chacune un texte :

- **ComponentBase** : `PlainRow` et `PlainText`, les mêmes paramètres et le même balisage que `FbRow` et `FbText`, écrits comme des composants Blazor ordinaires qui reconstruisent leurs classes à chaque rendu ;
- **FractalBlazor** : `<FbRow PL GM WBA WFM WRM><FbText S Muted>` ;
- **MudBlazor** : `<MudPaper Outlined><MudStack Row><MudText>` ;
- **Radzen** : `<RadzenCard Variant="Outlined"><RadzenStack Orientation="Horizontal"><RadzenText>` ;
- **Fluent UI** : `<FluentCard Appearance="Outline"><FluentStack Orientation="Horizontal"><FluentText>`.

`Rerender` change le texte de chaque ligne.

| Bibliothèque | FirstRender | Mémoire | Rerender | Mémoire |
| --- | ---: | ---: | ---: | ---: |
| ComponentBase | 3,94 ms | 3 881 Ko | 1,29 ms | 516 Ko |
| FractalBlazor | 4,21 ms | 4 030 Ko | 1,23 ms | 352 Ko |
| MudBlazor | 8,99 ms | 7 350 Ko | 3,17 ms | 1 016 Ko |
| Radzen | 8,51 ms | 8 314 Ko | 3,64 ms | 1 462 Ko |
| Fluent UI | 16,23 ms | 10 054 Ko | 4,79 ms | 2 485 Ko |

FractalBlazor rend deux fois plus vite que MudBlazor et Radzen, et près de quatre fois plus vite que Fluent UI. Face à un `ComponentBase` équivalent, il est 7 % plus lent au premier rendu, où il calcule et mémorise la clé de cache, et plus rapide au re-rendu, avec un tiers de mémoire en moins : la chaîne `class` vient d'un cache partagé et n'est pas reconstruite quand les paramètres ne changent pas.

## Pile d'éditeurs (`EditorBenchmarks`)

Rendu de 1 000 éditeurs empilés, un par candidat. Le DTO `ApplicantDto` porte un prénom, un nom, une ville, une date de naissance (`DateTime`) et un statut parmi `Pending`, `Review`, `Admitted` et `Fail` ; les 1 000 candidats sont générés avec une graine fixe. Chaque ligne édite les cinq champs, avec leur libellé :

- **FractalBlazor** : un `FbStack` de `FbRow`, chacune avec trois `FbInputText`, un `FbInputDate` et un `FbSelect`, le libellé en `StartContent`. Chaque champ envoie son action au DTO, qui se met à jour lui-même ;
- **MudBlazor** : un `MudStack` de `MudStack Row`, chacune avec trois `MudTextField`, un `MudDatePicker` et un `MudSelect` à quatre `MudSelectItem`, en variante `Outlined`, liés par `@bind-Value`, avec le `MudPopoverProvider` qu'une application place dans sa mise en page ;
- **Radzen** : un `RadzenStack` de `RadzenStack` horizontaux, chacun avec cinq `RadzenFormField` en variante `Outlined` autour de trois `RadzenTextBox`, d'un `RadzenDatePicker` et d'un `RadzenDropDown`, liés par `@bind-Value` ;
- **Fluent UI** : un `FluentStack` de `FluentStack` horizontaux, chacun avec trois `FluentTextInput`, un `FluentDatePicker` et un `FluentSelect`, avec leur `Label`, liés par `@bind-Value`.

`Rerender` passe chaque candidat au statut suivant.

| Bibliothèque | FirstRender | Mémoire | Rerender | Mémoire | HTML produit |
| --- | ---: | ---: | ---: | ---: | ---: |
| FractalBlazor | 173 ms | 55,3 Mo | 34,0 ms | 3,6 Mo | 2 080 Ko |
| Radzen | 458 ms | 151,1 Mo | 292,0 ms | 49,6 Mo | 14 071 Ko |
| Fluent UI | 511 ms | 184,6 Mo | 300,6 ms | 58,7 Mo | 8 424 Ko |
| MudBlazor | 1 779 ms | 628,7 Mo | 104,4 ms | 14,1 Mo | 7 326 Ko |

Au premier rendu, FractalBlazor est 2,6 fois plus rapide que Radzen, 3 fois plus que Fluent UI et 10 fois plus que MudBlazor ; au re-rendu, 3 fois plus rapide que MudBlazor et près de 9 fois plus que Radzen et Fluent UI. Son HTML est le plus léger, de 3,5 à 7 fois.

Ce que chaque bibliothèque rend explique une part des écarts :

- **Le calendrier.** `FbInputDate` s'appuie sur le calendrier natif du navigateur. Radzen et Fluent UI rendent le calendrier complet de chaque sélecteur de date, même fermé : un mois entier par ligne, rendu à nouveau à chaque re-rendu. MudBlazor ne le rend qu'à l'ouverture.
- **Le provider de MudBlazor.** Chaque `MudDatePicker` et chaque `MudSelect` inscrit son popover auprès du `MudPopoverProvider`, qui rend 2 000 conteneurs. Sans provider, irréaliste dans une application, MudBlazor mettait 1 062 ms et 342 Mo au premier rendu (ShortRun).
- **Les web components.** Fluent UI rend des éléments personnalisés (`<fluent-text-input>`, `<fluent-field>`) : le navigateur fait ensuite une partie du travail en les instanciant, ce que cette mesure côté serveur ne voit pas.
- **Les listes fermées.** Aucune bibliothèque ne rend les options d'une liste fermée : `FbDropDown` ne rend son contenu qu'une fois ouvert.

Les champs des quatre bibliothèques injectent `IJSRuntime`, que `HtmlRenderer` ne fournit pas ; `NoJSRuntime` le remplace, et n'est appelé ni au rendu ni au re-rendu.

Avec les styles inline, le HTML de FractalBlazor pesait 3 926 Ko, dont 52 % d'attributs `style` (7 001 attributs pour 5 chaînes distinctes), pour 57,5 Mo alloués au premier rendu. Les classes atomiques le ramènent à 2 080 Ko ; compressé, il passe de 89 à 68 Ko en gzip et de 54 à 51 Ko en Brotli.

## Lancer les mesures

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*"
```

Un seul scénario :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*EditorBenchmarks*"
```
