# Benchmarks

Deux scénarios, rendus avec le moteur de rendu HTML côté serveur de Blazor (`HtmlRenderer`), pour FractalBlazor, MudBlazor, Radzen et Fluent UI (Microsoft) : une liste de lignes simples, puis une pile d'éditeurs. Dans les deux, `FirstRender` instancie et rend tous les composants, puis les détruit avec le moteur de rendu ; `Rerender` change une valeur de chaque ligne d'une liste déjà rendue.

Chaque chiffre de cette page a été mesuré sur le code de ce commit, et les commandes en bas de page le reproduisent. BenchmarkDotNet 0.15.8, MediumRun (2 lancements, 15 itérations), .NET 10.0.12, Intel Core i9-9900K, Windows 10 ; MudBlazor 9.11.0, Radzen.Blazor 12.0.5, Microsoft.FluentUI.AspNetCore.Components 5.0.0. Les temps sont des moyennes, suivies de leur écart-type ; sur cette machine, BenchmarkDotNet a signalé certaines distributions du premier rendu comme bimodales, ce que l'écart-type reflète.

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
| ComponentBase | 4,14 ms ± 0,31 | 3 989 Ko | 1,39 ms ± 0,05 | 624 Ko |
| FractalBlazor | 4,29 ms ± 0,20 | 4 184 Ko | 1,26 ms ± 0,02 | 506 Ko |
| MudBlazor | 8,81 ms ± 0,47 | 7 575 Ko | 3,20 ms ± 0,10 | 1 241 Ko |
| Radzen | 12,53 ms ± 1,13 | 8 539 Ko | 4,23 ms ± 0,26 | 1 687 Ko |
| Fluent UI | 16,14 ms ± 0,07 | 10 278 Ko | 4,79 ms ± 0,15 | 2 710 Ko |

FractalBlazor rend la liste 2,1 fois plus vite que MudBlazor, 2,9 fois plus que Radzen et 3,8 fois plus que Fluent UI, et la re-rend 2,5 à 3,8 fois plus vite. Face à un `ComponentBase` équivalent, il fait jeu égal au premier rendu, où il calcule la clé de ses classes (l'écart est dans le bruit de la mesure), et il est 10 % plus rapide au re-rendu, avec un cinquième de mémoire en moins : la chaîne `class` vient d'un cache partagé et n'est pas reconstruite quand les paramètres ne changent pas.

## Pile d'éditeurs (`EditorBenchmarks`)

Rendu de 1 000 éditeurs empilés, un par candidat. Le DTO `ApplicantDto` porte un prénom, un nom, une ville, une date de naissance (`DateTime`), un statut parmi `Pending`, `Review`, `Admitted` et `Fail`, et une case `Interviewed` ; les 1 000 candidats sont générés avec une graine fixe. Chaque ligne édite cinq champs, avec leur libellé : trois textes, la case et le statut.

- **FractalBlazor** : un `FbStack` de `FbRow`, chacune avec trois `FbInputText` (libellé en `StartContent`), un `FbCheckbox` et un `FbSelect`. Chaque champ envoie son action au DTO, qui se met à jour lui-même ;
- **MudBlazor** : un `MudStack` de `MudStack Row`, chacune avec trois `MudTextField` en variante `Outlined`, un `MudCheckBox` avec son `Label` et un `MudSelect` à quatre `MudSelectItem`, liés par `@bind-Value`, avec le `MudPopoverProvider` qu'une application place dans sa mise en page ;
- **Radzen** : un `RadzenStack` de `RadzenStack` horizontaux, chacun avec trois `RadzenTextBox` et un `RadzenDropDown` dans des `RadzenFormField` en variante `Outlined`, et un `RadzenCheckBox` relié à son `RadzenLabel` par son nom, comme le montre la documentation de Radzen, liés par `@bind-Value` ;
- **Fluent UI** : un `FluentStack` de `FluentStack` horizontaux, chacun avec trois `FluentTextInput`, un `FluentCheckbox` et un `FluentSelect`, avec leur `Label`, liés par `@bind-Value`.

`Rerender` passe chaque candidat au statut suivant et coche ou décoche sa case.

| Bibliothèque | FirstRender | Mémoire | Rerender | Mémoire |
| --- | ---: | ---: | ---: | ---: |
| FractalBlazor | 131,5 ms ± 5,2 | 47,8 Mo | 27,4 ms ± 0,9 | 3,2 Mo |
| Radzen | 203,6 ms ± 3,4 | 69,3 Mo | 84,2 ms ± 2,7 | 12,3 Mo |
| Fluent UI | 280,2 ms ± 8,6 | 95,8 Mo | 112,0 ms ± 3,2 | 20,7 Mo |
| MudBlazor | 1 107,5 ms ± 70,9 | 404,9 Mo | 118,9 ms ± 1,5 | 16,9 Mo |

Au premier rendu, FractalBlazor est 1,5 fois plus rapide que Radzen, 2,1 fois plus que Fluent UI et 8,4 fois plus que MudBlazor ; au re-rendu, de 3,1 à 4,3 fois plus rapide, avec 3,8 à 6,4 fois moins de mémoire.

Pourquoi un champ texte, une case et une liste, et pas de date : Radzen et Fluent UI rendent le calendrier complet de chaque sélecteur de date, même fermé, quand `FbInputDate` s'appuie sur celui du navigateur. Un champ de date mesurerait cette différence de conception plutôt que les bibliothèques.

## HTML produit

| Scénario | Bibliothèque | Brut | gzip | Brotli | Éléments |
| --- | --- | ---: | ---: | ---: | ---: |
| Lignes | ComponentBase | 120,0 Ko | 2,9 Ko | 1,2 Ko | 2 000 |
| Lignes | FractalBlazor | 120,0 Ko | 2,9 Ko | 1,2 Ko | 2 000 |
| Lignes | MudBlazor | 202,0 Ko | 3,2 Ko | 1,8 Ko | 3 000 |
| Lignes | Radzen | 290,9 Ko | 30,6 Ko | 26,0 Ko | 3 000 |
| Lignes | Fluent UI | 243,1 Ko | 3,3 Ko | 1,7 Ko | 3 000 |
| Éditeurs | FractalBlazor | 1 519,1 Ko | 59,7 Ko | 46,1 Ko | 23 001 |
| Éditeurs | MudBlazor | 5 955,3 Ko | 174,5 Ko | 98,5 Ko | 49 002 |
| Éditeurs | Radzen | 3 885,2 Ko | 174,9 Ko | 131,4 Ko | 40 001 |
| Éditeurs | Fluent UI | 1 849,6 Ko | 130,6 Ko | 81,1 Ko | 23 001 |

Les tailles compressées varient d'un rendu à l'autre de quelques dixièmes de Ko : les identifiants aléatoires de chaque bibliothèque changent. Celles des lignes de Radzen restent élevées : chacun de ses éléments porte un identifiant aléatoire. Le HTML des éditeurs de FractalBlazor est le plus léger, brut comme compressé. Celui de Fluent UI a autant d'éléments, mais ses web components (`<fluent-text-input>`, `<fluent-checkbox>`) construisent ensuite leur propre DOM dans le navigateur, ce que cette mesure côté serveur ne voit pas.

Quelle taille compte dépend du mode de rendu. En Interactive Server, les échanges SignalR sont compressés par défaut depuis .NET 8 (`permessage-deflate`, désactivable par `DisableWebSocketCompression`), avec une politique `frame-ancestors` qui interdit l'intégration dans une `iframe` d'une autre origine, contre les attaques de type CRIME et BREACH. Le HTML du rendu statique ou du prérendu, lui, n'est compressé que par un middleware ajouté explicitement (`EnableForHttps` vaut `false` par défaut, pour la même raison) ou par un proxy : sans eux, c'est la taille brute qui part sur le réseau. Dans tous les cas, le navigateur analyse le HTML brut et construit chacun de ses éléments.

## Ce que rend chaque bibliothèque

- **Le provider de MudBlazor.** Chaque `MudSelect` inscrit son popover auprès du `MudPopoverProvider`, qui rend 1 000 conteneurs : c'est l'essentiel du coût de son premier rendu.
- **Les listes fermées.** Aucune bibliothèque ne rend les options d'une liste fermée : `FbDropDown` ne rend son contenu qu'une fois ouvert.
- **JavaScript.** Les champs des quatre bibliothèques injectent `IJSRuntime`, que `HtmlRenderer` ne fournit pas ; `NoJSRuntime` le remplace et compte ses appels. Aucune bibliothèque ne l'appelle, ni au rendu, ni au re-rendu, ni à la destruction des composants.

## Lancer les mesures

Tous les benchmarks :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*"
```

Un seul scénario :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*EditorBenchmarks*"
```

La taille du HTML et les appels JavaScript :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- html
```
