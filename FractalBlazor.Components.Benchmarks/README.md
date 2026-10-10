# Benchmarks

Rendu de 1 000 lignes encadrées contenant chacune un texte, avec le moteur de rendu HTML côté serveur de Blazor (`HtmlRenderer`). Trois implémentations produisent la même structure :

- **ComponentBase** : `PlainRow` et `PlainText`, les mêmes paramètres et le même balisage que `FbRow` et `FbText`, écrits comme des composants Blazor ordinaires ;
- **FractalBlazor** : `<FbRow PL GM WBA WFM WRM><FbText S Muted>` ;
- **MudBlazor** : `<MudPaper Outlined><MudStack Row><MudText>`.

`FirstRender` instancie et rend tous les composants. `Rerender` change le texte de chaque ligne d'une liste déjà rendue.

## Résultats

BenchmarkDotNet 0.15.8, ShortRun, .NET 10.0.12, Intel Core i9-9900K, Windows 10. Mesures du 10 octobre 2026.

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

## Lancer les mesures

```bash
dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- --filter "*"
```
