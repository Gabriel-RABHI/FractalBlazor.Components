# Benchmarks

Deux scénarios, rendus avec le moteur de rendu HTML côté serveur de Blazor (`HtmlRenderer`) : une liste de lignes simples, puis une pile d'éditeurs. Dans les deux, `FirstRender` instancie et rend tous les composants, et `Rerender` change une valeur de chaque ligne d'une liste déjà rendue.

BenchmarkDotNet 0.15.8, ShortRun, .NET 10.0.12, Intel Core i9-9900K, Windows 10. Mesures du 13 octobre 2026, avec les classes atomiques de Layout.

## Liste de lignes (`RenderBenchmarks`)

Rendu de 1 000 lignes encadrées contenant chacune un texte. Trois implémentations produisent la même structure :

- **ComponentBase** : `PlainRow` et `PlainText`, les mêmes paramètres et le même balisage que `FbRow` et `FbText`, écrits comme des composants Blazor ordinaires qui reconstruisent leurs classes à chaque rendu ;
- **FractalBlazor** : `<FbRow PL GM WBA WFM WRM><FbText S Muted>` ;
- **MudBlazor** : `<MudPaper Outlined><MudStack Row><MudText>`.

`Rerender` change le texte de chaque ligne.

| Scénario | Bibliothèque | Temps moyen | Mémoire allouée |
| --- | --- | ---: | ---: |
| FirstRender | ComponentBase | 4,00 ms | 3 881 Ko |
| FirstRender | FractalBlazor | 4,12 ms | 4 030 Ko |
| FirstRender | MudBlazor | 8,63 ms | 7 350 Ko |
| Rerender | ComponentBase | 1,39 ms | 516 Ko |
| Rerender | FractalBlazor | 1,24 ms | 352 Ko |
| Rerender | MudBlazor | 2,97 ms | 1 016 Ko |

FractalBlazor rend environ deux fois plus vite que MudBlazor et alloue moitié moins au premier rendu. Il fait jeu égal avec un `ComponentBase` équivalent au premier rendu, et le bat au re-rendu : la chaîne `class` vient d'un cache partagé et n'est pas reconstruite quand les paramètres ne changent pas.

Avant le cache des classes CSS, FractalBlazor était plus lent que `ComponentBase` : 5,64 ms pour le premier rendu, 1,95 ms et 969 Ko pour le re-rendu.

## Pile d'éditeurs (`EditorBenchmarks`)

Rendu de 1 000 éditeurs empilés, un par candidat. Le DTO `ApplicantDto` porte un prénom, un nom, une ville, une date de naissance (`DateTime`) et un statut parmi `Pending`, `Review`, `Admitted` et `Fail` ; les 1 000 candidats sont générés avec une graine fixe. Chaque ligne édite les cinq champs, avec leur libellé :

- **FractalBlazor** : un `FbStack` de `FbRow`, chacune avec trois `FbInputText`, un `FbInputDate` et un `FbSelect`, le libellé en `StartContent`. Chaque champ envoie son action au DTO, qui se met à jour lui-même ;
- **MudBlazor** : un `MudStack` de `MudStack Row`, chacune avec trois `MudTextField`, un `MudDatePicker` et un `MudSelect` à quatre `MudSelectItem`, en variante `Outlined`, liés par `@bind-Value`.

`Rerender` passe chaque candidat au statut suivant.

| Scénario | Bibliothèque | Temps moyen | Mémoire allouée | HTML produit |
| --- | --- | ---: | ---: | ---: |
| FirstRender | FractalBlazor | 165 ms | 55,3 Mo | 2 080 Ko |
| FirstRender | MudBlazor | 1 062 ms | 342,4 Mo | 6 714 Ko |
| Rerender | FractalBlazor | 32,9 ms | 3,6 Mo | |
| Rerender | MudBlazor | 106,1 ms | 14,1 Mo | |

FractalBlazor rend la pile 6,4 fois plus vite que MudBlazor au premier rendu, avec 6 fois moins de mémoire, et 3,2 fois plus vite au re-rendu, avec 4 fois moins de mémoire. Son HTML pèse trois fois moins.

Avec les styles inline, le HTML de FractalBlazor pesait 3 926 Ko, dont 52 % d'attributs `style` (7 001 attributs pour 5 chaînes distinctes), pour 57,5 Mo alloués au premier rendu. Les classes atomiques le ramènent à 2 080 Ko ; compressé, il passe de 89 à 68 Ko en gzip et de 54 à 51 Ko en Brotli.

Les listes fermées ne sont rendues d'aucun côté : `FbDropDown` ne rend son contenu qu'une fois ouvert, et MudBlazor confie le sien au `MudPopoverProvider`, absent ici. Les champs des deux bibliothèques injectent `IJSRuntime`, que `HtmlRenderer` ne fournit pas ; `NoJSRuntime` le remplace, et n'est appelé ni au rendu ni au re-rendu.

## Lancer les mesures

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*"
```

Un seul scénario :

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*EditorBenchmarks*"
```
