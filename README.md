# FractalBlazor.Components

Composants Blazor ultra-légers pour la mise en page et les formulaires, avec un thème entièrement porté par des variables CSS. Les composants ne connaissent que des intentions visuelles (`Surface`, `Accent`, `Highlight`, frames `Light` / `Medium` / `Strong`, textes `Subtle` / `Muted` / `Default` / `Highlight`). Toutes les couleurs naissent d'un `color-mix()` en oklab entre deux ancres.

```razor
<FbStack GL PL WBS WFM WRL>
    <FbRow GM JVC>
        <FbIcon Icon="@FbMaterialIcons.Person" Muted />
        <FbText L B>Profil</FbText>
    </FbRow>
    <FbVariant Variant="Error">
        <FbRow GS PM WBA WRM>
            <FbText S>Ce bloc et tout son contenu prennent les couleurs Error.</FbText>
        </FbRow>
    </FbVariant>
</FbStack>
```

## Principes

- **Composants ultra-légers.** Ils implémentent `IComponent` directement, sans `ComponentBase`. Les chaînes `class` et `style` viennent d'un cache partagé, indexé par la valeur complète des paramètres.
- **Les composants ne lisent pas le thème.** Ils posent des classes ; le thème est résolu en C#, sérialisé en variables CSS, et la cascade fait le reste.
- **Une grammaire courte.** Des drapeaux booléens préfixés : `WB*` fond, `WF*` frame, `WR*` rayon, `P*` padding, `M*` marge, `G*` gutter, `J*` alignement. Une API enum les complète pour les valeurs calculées.
- **Rendu ≠ fonctionnel.** `Muted` est un rendu ; `Error`, `Selected`, `Disabled` sont des variantes posées par `FbVariant`.
- **Layout autonome.** `FractalBlazor.Components.Layout` ne dépend pas de Forms et préfixe tout son CSS par `fb-` : il cohabite avec MudBlazor ou Radzen.
- **State-first.** Les champs envoient des actions à un objet d'état ; la vue est choisie d'après le type de l'état.

Le raisonnement complet est dans [Conception.md](Conception.md), le modèle de thème dans [Theming.md](Theming.md).

## Bibliothèques

| Paquet | Contenu |
| --- | --- |
| `FractalBlazor.Components.Layout` | `FbRow`, `FbStack`, items, séparateurs, `FbGridContainer`, `FbGridItem`, `FbDropDown`, `FbShowWhenVisible`, thème Layout autonome |
| `FractalBlazor.Components.Forms` | `FbText`, `FbIcon`, `FbButton`, `FbInputText`, `FbInputNumber`, `FbInputDate`, `FbSelect`, `FbCheckbox`, `FbSwitch`, `FbTheme`, `FbVariant`, `FbDynamicView` |
| `FractalBlazor.Components.Common` | Contrats partagés par les états et les vues : actions, notification, `ViewFor` |

## Installation

`Program.cs` :

```csharp
builder.Services.AddFractalBlazorTheming();
builder.Services.AddSingleton<IFbViewRegistry<ViewSelector>, FbViewRegistry<ViewSelector>>(); // pour FbDynamicView
```

`App.razor` :

```razor
<head>
    <link rel="stylesheet" href="@Assets["_content/FractalBlazor.Components.Layout/fb-components-layout.css"]" />
    <link rel="stylesheet" href="@Assets["_content/FractalBlazor.Components.Forms/fb-components-forms.css"]" />
    <script src="@Assets["_content/FractalBlazor.Components.Layout/fb-components-layout.js"]"></script>
</head>
<body>
    <FbTheme Theme="Default" Branch="Dark">
        <Routes />
    </FbTheme>
    <script src="@Assets["_framework/blazor.web.js"]"></script>
</body>
```

Avec Layout seul, `FbLayoutTheme` remplace `FbTheme` et `AddFractalBlazorTheming` enregistre déjà son registre.

## Thème

Un thème se déclare une fois en C# : espacements, rayons et typographie, puis des branches (`Dark`, `Light`), puis des variantes (`Default`, `Selected`, `Error`, `Warning`, `Disabled`, `Success`, `Info`). Un thème dérivé ne déclare que ses différences.

```csharp
builder.Services.AddFractalBlazorTheming(registry =>
    registry.Register(new FbThemeSetup("Spacious", registry.Default)
    {
        Spacings = FbThemeLayoutSpacings.Spaced,
        Corners = FbThemeLayoutCorners.Rounded
    }));
```

- `<FbVariant Variant="Error">` redirige quatre variables sur une branche du DOM, sans changer aucune classe.
- `<FbVariant Invert>` échange les ancres de fond et de premier plan : une carte claire dans une page sombre.
- `<FbTheme Branch="Light" Scoped>` pose une autre branche sur un seul sous-arbre.
- `FbThemeContrast.Analyze(theme)` calcule les couleurs réellement peintes et leur contraste WCAG.
- `<FbTheme Setup="@monTheme" Scoped>` applique un thème non enregistré, pour un aperçu ou un éditeur.

## Formulaires state-first

Un champ envoie une action à l'état ; l'état se met à jour et notifie ses vues.

```csharp
public sealed class ContactState : ObservableState
{
    public sealed record SetName(string Value) : IStateAction<string>;

    public string Name { get; private set; } = "";

    public void Handle(SetName action)
    {
        Name = action.Value;
        NotifyStateChanged();
    }
}
```

```razor
<FbInputText TAction="ContactState.SetName" Handler="State" Value="@State.Name"
             Variant="@(State.Name.Length < 2 ? FbThemeVariants.Error : null)" />
```

Le binding classique reste possible : `<FbInputText TAction="FbSetValue<string>" @bind-Value="name" />`.

`FbDynamicView` affiche un état avec la vue déclarée pour son type :

```razor
@attribute [ViewFor<ViewSelector>(typeof(ContactState), ViewSelector.Editor)]
```

```razor
<FbDynamicView TSelector="ViewSelector" Item="State" Selector="ViewSelector.Editor" />
```

## Performances

Rendu de 1 000 lignes encadrées contenant un texte (détails dans [FractalBlazor.Components.Benchmarks](FractalBlazor.Components.Benchmarks/README.md)) :

| Scénario | ComponentBase équivalent | FractalBlazor | MudBlazor |
| --- | ---: | ---: | ---: |
| Premier rendu | 4,28 ms · 4 123 Ko | 4,19 ms · 3 991 Ko | 8,64 ms · 7 350 Ko |
| Re-rendu | 1,46 ms · 758 Ko | 1,26 ms · 352 Ko | 3,53 ms · 1 016 Ko |

## Développement

```bash
dotnet build FractalBlazor.Components.slnx
```

```bash
dotnet test --project FractalBlazor.Components.Tests/FractalBlazor.Components.Tests.csproj
```

```bash
dotnet run --project FractalBlazor.Components/FractalBlazor.Components.csproj --launch-profile http
```

La démo (`http://localhost:5115`) montre la mise en page et le thème, un formulaire state-first, un menu piloté par son état et un éditeur de thème : couleurs et mélanges réglés en direct sur un jeu de composants, avec le rapport de contraste et le code C# du thème obtenu. Le plan de finalisation et son avancement sont dans [Plan.md](Plan.md).

## Licence

MIT, voir [LICENSE.txt](LICENSE.txt).
