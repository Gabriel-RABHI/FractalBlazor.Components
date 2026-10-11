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
| `FractalBlazor.Components.Layout` | `FbPageAnchor`, `FbRow`, `FbStack`, items, séparateurs, `FbGridContainer`, `FbGridItem`, `FbDropDown`, `FbShowWhenVisible`, thème Layout autonome |
| `FractalBlazor.Components.Forms` | `FbText`, `FbIcon`, `FbButton`, `FbInputText`, `FbInputNumber`, `FbInputDate`, `FbSelect`, `FbCheckbox`, `FbSwitch`, `FbTheme`, `FbVariant`, `FbDynamicView` |
| `FractalBlazor.Components.Common` | Contrats partagés par les états et les vues : actions, notification, `ViewFor` |

## Solution

| Projet | Rôle |
| --- | --- |
| `FractalBlazor.Components.Layout`, `.Forms`, `.Common` | Les bibliothèques génériques, publiées en paquets. |
| `FractalBlazor.Components.Benchmarks` | Comparaisons de rendu avec `ComponentBase` et MudBlazor. |
| `FractalBlazor.Components.Tests` | Tests xUnit et bUnit. |
| `FractalBlazor.Components.Tools` | Outil qui produit la palette de base à partir d'une image. |
| `FractalBlazor.WebSite` | Le site : documentation, démos, éditeur de thème et leurs modèles d'état, en bibliothèque de composants. Il ne dépend d'aucun hôte. |
| `FractalBlazor.WebSite.ServerSide` | Hôte serveur : Blazor interactif côté serveur, avec rendu initial. |
| `FractalBlazor.WebSite.ClientSide` | Hôte WebAssembly : des fichiers statiques, servis par n'importe quel serveur web. |

Chaque hôte ne fournit que la page HTML et le mode de thème initial, lu dans un cookie avant le premier rendu : par le serveur pendant son rendu, par JS côté WebAssembly. Les deux appellent `AddFractalBlazorWebSite()` et rendent `WebSiteApplication`.

## Installation

`Program.cs` :

```csharp
builder.Services.AddFractalBlazor();
```

`AddFractalBlazor` enregistre les registres de thèmes et un registre de vues pour chaque énumération de sélecteurs (`ViewSelector`, `FbViewSelector`…), utilisé par `FbDynamicView` et `FbSelect`.

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

Avec Layout seul, `FbLayoutTheme` remplace `FbTheme` et `AddFractalBlazor` enregistre déjà son registre.

## Thème

Un thème se déclare une fois en C# : espacements, rayons et typographie, puis des branches (`Dark`, `Light`), puis des variantes (`Default`, `Selected`, `Error`, `Warning`, `Disabled`, `Success`, `Info`). Un thème dérivé ne déclare que ses différences.

```csharp
builder.Services.AddFractalBlazor(registry =>
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

Sans vue déclarée, les types simples (`string`, énumérations, nombres, dates, `Guid`) sont rendus par `FbValueView` : le texte formaté dans la culture courante, ou le `[Display(Name = …)]` d'une valeur d'énumération.

## Select

`FbSelect` choisit un élément dans une liste et délègue le rendu : il ouvre un `FbDropDown` (fermé par un clic à l'extérieur ou par Échap) et affiche chaque élément, ainsi que la valeur choisie, avec la vue `FbViewSelector.Item` de son type. La liste peut donc être polymorphe :

```csharp
public static IReadOnlyList<object> ContactOptions { get; } =
    ["Pas de préférence", ContactChannel.Email, ContactChannel.Phone, new ContactPerson("Ada Lovelace", "Assistante")];
```

```razor
@attribute [ViewFor<FbViewSelector>(typeof(ContactPerson), FbViewSelector.Item)]
```

```razor
<FbSelect TValue="object" TAction="ContactState.SetPreferredContact" Handler="State"
          Value="@State.PreferredContact" Items="ContactState.ContactOptions" />
```

La chaîne et les canaux passent par `FbValueView`, la personne par sa propre vue. `ItemSelector` choisit un autre sélecteur, d'une autre énumération si besoin. Le clavier suit le motif ARIA combobox : flèches, Début, Fin, Entrée, Espace, Échap, Tab.

## Application d'une seule page

`FbPageAnchor` est la racine d'une application : il occupe toute la fenêtre, qui ne défile jamais, et dispose ses enfants en colonne. Rows et stacks grandissent par défaut. `NoFlex` garde la hauteur de l'en-tête et du pied de page, et une zone `Scrollable` prend l'espace restant puis défile seule, avec sa barre de défilement.

```razor
<FbPageAnchor>
    <FbStack GM PM>
        <FbRow NoFlex>…en-tête…</FbRow>
        <FbRow GM JVSt>
            <FbStack NoFlex Scrollable JVS>…menu…</FbStack>
            <FbStack Scrollable JVS>…page…</FbStack>
        </FbRow>
        <FbRow NoFlex>…pied de page…</FbRow>
    </FbStack>
</FbPageAnchor>
```

Ouverte, la zone d'un `FbDropDown` est fixée à la fenêtre : une zone défilante ne coupe pas la liste d'un Select.

L'application elle-même est un modèle. Dans la démo, `RootState` porte l'en-tête, la navigation et le pied de page. Chaque section du menu du haut porte la sous-liste du menu de gauche, et chaque entrée de ce menu donne le modèle de sa page :

```csharp
new SectionState(Navigation, MenuItemIntent.Components, section =>
[
    new PageLinkState(section, MenuItemIntent.Layout, new LayoutPage()),
    new PageLinkState(section, MenuItemIntent.Forms, new FormsPage(new ContactState()))
])
```

Chaque type d'état déclare sa vue avec `[ViewFor<ViewSelector>]`, et la zone de contenu rend la page ouverte sans la connaître :

```razor
<FbStack Scrollable GL PX JVS>
    <FbDynamicView TSelector="ViewSelector" Item="Item.Page" Selector="ViewSelector.Viewer" />
</FbStack>
```

Ouvrir une page ne notifie que l'ancienne entrée, la nouvelle et la zone de contenu : le reste de l'application ne se redessine pas. Chaque modèle de page garde son état d'une visite à l'autre.

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

Le site, rendu côté serveur (`http://localhost:5115`) :

```bash
dotnet run --project FractalBlazor.WebSite.ServerSide/FractalBlazor.WebSite.ServerSide.csproj --launch-profile http
```

Le même site en WebAssembly (`http://localhost:5125`) :

```bash
dotnet run --project FractalBlazor.WebSite.ClientSide/FractalBlazor.WebSite.ClientSide.csproj --launch-profile http
```

Le site est une application d'une seule page pilotée par son modèle : démarrage rapide, principes state-first, mise en page, formulaires, variantes, et un éditeur de thème où couleurs et mélanges se règlent en direct sur un jeu de composants, avec le rapport de contraste et le code C# du thème obtenu. Le plan de finalisation et son avancement sont dans [Plan.md](Plan.md).

## Licence

MIT, voir [LICENSE.txt](LICENSE.txt).
