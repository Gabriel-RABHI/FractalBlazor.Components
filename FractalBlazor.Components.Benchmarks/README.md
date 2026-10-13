# Benchmarks

Deux scénarios, rendus avec le moteur de rendu HTML côté serveur de Blazor (`HtmlRenderer`) : une liste de lignes simples, puis une pile d'éditeurs. Dans les deux, `FirstRender` instancie et rend tous les composants, et `Rerender` change une valeur de chaque ligne d'une liste déjà rendue.

BenchmarkDotNet 0.15.8, ShortRun, .NET 10.0.12, Intel Core i9-9900K, Windows 10.

## Liste de lignes (`RenderBenchmarks`)

Rendu de 1 000 lignes encadrées contenant chacune un texte. Trois implémentations produisent la même structure :

- **ComponentBase** : `PlainRow` et `PlainText`, les mêmes paramètres et le même balisage que `FbRow` et `FbText`, écrits comme des composants Blazor ordinaires ;
- **FractalBlazor** : `<FbRow PL GM WBA WFM WRM><FbText S Muted>` ;
- **MudBlazor** : `<MudPaper Outlined><MudStack Row><MudText>`.

`Rerender` change le texte de chaque ligne. Mesures du 10 octobre 2026.

| Scénario | Bibliothèque | Temps moyen | Mémoire allouée |
| --- | --- | ---: | ---: |
| FirstRender | ComponentBase | 4,28 ms | 4 123 Ko |
| FirstRender | FractalBlazor | 4,19 ms | 3 991 Ko |
| FirstRender | MudBlazor | 8,64 ms | 7 350 Ko |
| Rerender | ComponentBase | 1,46 ms | 758 Ko |
| Rerender | FractalBlazor | 1,26 ms | 352 Ko |
| Rerender | MudBlazor | 3,53 ms | 1 016 Ko |

FractalBlazor rend environ deux fois plus vite que MudBlazor et alloue moitié moins au premier rendu. Au re-rendu, il bat aussi un `ComponentBase` équivalent : les chaînes `class` et `style` viennent d'un cache partagé et ne sont pas reconstruites quand les paramètres ne changent pas.

Avant le cache des classes CSS, FractalBlazor était plus lent que `ComponentBase` : 5,64 ms pour le premier rendu, 1,95 ms et 969 Ko pour le re-rendu.

## Pile d'éditeurs (`EditorBenchmarks`)

Rendu de 1 000 éditeurs empilés, un par candidat. Le DTO `ApplicantDto` porte un prénom, un nom, une ville, une date de naissance (`DateTime`) et un statut parmi `Pending`, `Review`, `Admitted` et `Fail` ; les 1 000 candidats sont générés avec une graine fixe. Chaque ligne édite les cinq champs, avec leur libellé :

- **FractalBlazor** : un `FbStack` de `FbRow`, chacune avec trois `FbInputText`, un `FbInputDate` et un `FbSelect`, le libellé en `StartContent`. Chaque champ envoie son action au DTO, qui se met à jour lui-même ;
- **MudBlazor** : un `MudStack` de `MudStack Row`, chacune avec trois `MudTextField`, un `MudDatePicker` et un `MudSelect` à quatre `MudSelectItem`, en variante `Outlined`, liés par `@bind-Value`.

`Rerender` passe chaque candidat au statut suivant. Mesures du 13 octobre 2026.

| Scénario | Bibliothèque | Temps moyen | Mémoire allouée |
| --- | --- | ---: | ---: |
| FirstRender | FractalBlazor | 165 ms | 57,5 Mo |
| FirstRender | MudBlazor | 1 067 ms | 342,4 Mo |
| Rerender | FractalBlazor | 44,6 ms | 3,6 Mo |
| Rerender | MudBlazor | 124,3 ms | 14,1 Mo |

FractalBlazor rend la pile 6,5 fois plus vite que MudBlazor au premier rendu, avec 6 fois moins de mémoire, et 2,8 fois plus vite au re-rendu, avec 4 fois moins de mémoire. Le re-rendu est bruité en ShortRun : son écart-type atteint 5 ms pour FractalBlazor et 28 ms pour MudBlazor.

Les listes fermées ne sont rendues d'aucun côté : `FbDropDown` ne rend son contenu qu'une fois ouvert, et MudBlazor confie le sien au `MudPopoverProvider`, absent ici. Les champs des deux bibliothèques injectent `IJSRuntime`, que `HtmlRenderer` ne fournit pas ; `NoJSRuntime` le remplace, et n'est appelé ni au rendu ni au re-rendu.

## Lancer les mesures

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*"
```

Un seul scénario :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*EditorBenchmarks*"
```
