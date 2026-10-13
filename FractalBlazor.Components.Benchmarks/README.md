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

Rendu de 1 000 éditeurs empilés, un par candidat. Le DTO `ApplicantDto` porte un prénom, un nom, une ville, une date de naissance (`DateTime`), un statut parmi `Pending`, `Review`, `Admitted` et `Fail`, et une case `Interviewed` ; les 1 000 candidats sont générés avec une graine fixe. Chaque ligne édite cinq champs, avec leur libellé : trois textes, la case et le statut. La date n'est pas éditée : chaque bibliothèque l'affiche à sa façon (voir plus bas), ce qui fausserait la comparaison.

- **FractalBlazor** : un `FbStack` de `FbRow`, chacune avec trois `FbInputText` (libellé en `StartContent`), un `FbCheckbox` et un `FbSelect`. Chaque champ envoie son action au DTO, qui se met à jour lui-même ;
- **MudBlazor** : un `MudStack` de `MudStack Row`, chacune avec trois `MudTextField` en variante `Outlined`, un `MudCheckBox` avec son `Label` et un `MudSelect` à quatre `MudSelectItem`, liés par `@bind-Value`, avec le `MudPopoverProvider` qu'une application place dans sa mise en page ;
- **Radzen** : un `RadzenStack` de `RadzenStack` horizontaux, chacun avec trois `RadzenTextBox` et un `RadzenDropDown` dans des `RadzenFormField` en variante `Outlined`, et un `RadzenCheckBox` relié à son `RadzenLabel` par son nom, comme le montre la documentation de Radzen, liés par `@bind-Value` ;
- **Fluent UI** : un `FluentStack` de `FluentStack` horizontaux, chacun avec trois `FluentTextInput`, un `FluentCheckbox` et un `FluentSelect`, avec leur `Label`, liés par `@bind-Value`.

`Rerender` passe chaque candidat au statut suivant et coche ou décoche sa case.

| Bibliothèque | FirstRender | Mémoire | Rerender | Mémoire |
| --- | ---: | ---: | ---: | ---: |
| FractalBlazor | 140 ms | 48,2 Mo | 28,0 ms | 3,2 Mo |
| Radzen | 210 ms | 69,3 Mo | 82,2 ms | 12,3 Mo |
| Fluent UI | 282 ms | 96,0 Mo | 120,1 ms | 20,8 Mo |
| MudBlazor | 1 108 ms | 404,9 Mo | 119,2 ms | 16,9 Mo |

Au premier rendu, FractalBlazor est 1,5 fois plus rapide que Radzen, 2 fois plus que Fluent UI et 8 fois plus que MudBlazor ; au re-rendu, de 2,9 à 4,3 fois plus rapide, avec 4 à 6 fois moins de mémoire.

| HTML de la page | Brut | gzip | Brotli | Éléments |
| --- | ---: | ---: | ---: | ---: |
| FractalBlazor | 1 685 Ko | 57 Ko | 47 Ko | 28 001 |
| Radzen | 3 885 Ko | 184 Ko | 131 Ko | 40 001 |
| Fluent UI | 1 849 Ko | 131 Ko | 81 Ko | 23 001 |
| MudBlazor | 5 954 Ko | 173 Ko | 99 Ko | 49 002 |

Le HTML de FractalBlazor est le plus léger, brut comme compressé (de 2,3 à 3,2 fois en gzip). Celui de Fluent UI est court, mais ses web components (`<fluent-text-input>`, `<fluent-checkbox>`) construisent ensuite leur propre DOM dans le navigateur, ce que cette mesure côté serveur ne voit pas, et ses identifiants aléatoires se compressent mal. Les classes de FractalBlazor portent le nom de leurs drapeaux (`fb-wba`, `fb-wfm`), et ses icônes internes (coche, chevron) sont dessinées par la feuille de style : avant cela, la page pesait 2 170 Ko et 32 001 éléments.

Quelle taille compte dépend du mode de rendu. En Interactive Server, les échanges SignalR sont compressés par défaut depuis .NET 8 (`permessage-deflate`, désactivable par `DisableWebSocketCompression`), avec une politique `frame-ancestors` qui interdit l'intégration dans une `iframe` d'une autre origine, contre les attaques de type CRIME et BREACH. Le HTML du rendu statique ou du prérendu, lui, n'est compressé que par un middleware ajouté explicitement (`EnableForHttps` vaut `false` par défaut, pour la même raison) ou par un proxy : sans eux, c'est la taille brute qui part sur le réseau.

Ce que chaque bibliothèque rend pèse sur les écarts :

- **Le provider de MudBlazor.** Chaque `MudSelect` inscrit son popover auprès du `MudPopoverProvider`, qui rend 1 000 conteneurs : c'est l'essentiel du coût de son premier rendu.
- **Les listes fermées.** Aucune bibliothèque ne rend les options d'une liste fermée : `FbDropDown` ne rend son contenu qu'une fois ouvert.
- **Le calendrier.** Avec un champ de date à la place de la case (`FbInputDate`, `MudDatePicker`, `RadzenDatePicker`, `FluentDatePicker`), les écarts se creusent : `FbInputDate` s'appuie sur le calendrier natif du navigateur, MudBlazor ne rend le sien qu'à l'ouverture, mais Radzen et Fluent UI rendent un mois entier par ligne, même fermé, et le rendent à nouveau à chaque re-rendu. Mesure du même jour :

| Avec un champ de date | FirstRender | Rerender | HTML produit |
| --- | ---: | ---: | ---: |
| FractalBlazor | 173 ms | 34,0 ms | 2 080 Ko |
| Radzen | 458 ms | 292,0 ms | 14 071 Ko |
| Fluent UI | 511 ms | 300,6 ms | 8 424 Ko |
| MudBlazor | 1 779 ms | 104,4 ms | 7 326 Ko |

Les champs des quatre bibliothèques injectent `IJSRuntime`, que `HtmlRenderer` ne fournit pas ; `NoJSRuntime` le remplace, et n'est appelé ni au rendu ni au re-rendu.

Avec les styles inline, le HTML de FractalBlazor pesait 3 926 Ko dans la version avec date, dont 52 % d'attributs `style` (7 001 attributs pour 5 chaînes distinctes), pour 57,5 Mo alloués au premier rendu. Les classes atomiques le ramènent à 2 080 Ko ; compressé, il passe de 89 à 68 Ko en gzip et de 54 à 51 Ko en Brotli.

## Lancer les mesures

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*"
```

Un seul scénario :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*EditorBenchmarks*"
```
