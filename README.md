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

- **Composants ultra-légers.** Ils implémentent `IComponent` directement, sans `ComponentBase`. La mise en page s'écrit en classes techniques très courtes (`fb-a`, `fb-ab`…), une déclaration chacune, générées depuis une table C# ; une rangée ou une pile n'ajoute que celles qui diffèrent de sa règle de base. La chaîne `class` vient d'un cache partagé, indexé par la valeur complète des paramètres. Seules les valeurs libres (`Style`, `MinWidth`, `FlexBasis`…) restent en style inline.
- **Les composants ne lisent pas le thème.** Ils posent des classes ; le thème est résolu en C#, sérialisé en variables CSS, et la cascade fait le reste.
- **Une grammaire courte.** Des drapeaux booléens préfixés : `WB*` fond, `WF*` frame, `WR*` rayon, `P*` padding, `M*` marge, `G*` gutter, `J*` alignement. Une API enum les complète pour les valeurs calculées. Le DOM parle la même langue : un drapeau donne une classe de son nom (`WBA` → `fb-wba`, `WFM` → `fb-wfm`, `Muted` → `fb-muted`). Dans la démo, l'assistant des flags (bouton ⚑ de l'en-tête) les retrouve par leurs lettres ou par composant : `PHS` se lit Padding → Horizontal → Small.
- **Rendu ≠ fonctionnel.** `Muted` est un rendu ; `Error`, `Selected`, `Disabled` sont des variantes posées par `FbVariant`.
- **Layout autonome.** `FractalBlazor.Components.Layout` ne dépend pas de Forms et préfixe tout son CSS par `fb-` : il cohabite avec MudBlazor ou Radzen.
- **State-first.** Les champs envoient des actions à un objet d'état ; la vue est choisie d'après le type de l'état.

Le raisonnement complet est dans [Conception.md](Conception.md), le modèle de thème dans [Theming.md](Theming.md).

## Bibliothèques

| Paquet | Contenu |
| --- | --- |
| `FractalBlazor.Components.Layout` | `FbPageAnchor`, `FbRow`, `FbStack`, items, séparateurs, `FbGridContainer`, `FbGridItem`, `FbDropDown`, `FbDrawer`, `FbShowWhenVisible`, thème Layout autonome |
| `FractalBlazor.Components.Forms` | `FbText`, `FbIcon`, `FbButton`, `FbInputText`, `FbInputNumber`, `FbInputDate`, `FbSelect`, `FbTabs`, `FbCheckbox`, `FbSwitch`, `FbWidthSensor`, `FbTheme`, `FbVariant`, `FbDynamicView` |
| `FractalBlazor.Components.Common` | Contrats partagés par les états et les vues : actions, notification, `ViewFor`, catégories de largeur (`FbWidthClass`) |

## Solution

| Projet | Rôle |
| --- | --- |
| `FractalBlazor.Components.Layout`, `.Forms`, `.Common` | Les bibliothèques génériques, publiées en paquets. |
| `FractalBlazor.Components.Benchmarks` | Comparaisons de rendu avec `ComponentBase`, MudBlazor, Radzen et Fluent UI. |
| `FractalBlazor.Components.Tests` | Tests xUnit et bUnit des bibliothèques. |
| `FractalBlazor.Components.Tools` | Outil qui produit la palette de base à partir d'une image. |
| `FractalBlazor.WebSite.StateModel` | Le modèle d'état du site : en-tête, navigation, pages, formulaire de contact, sélecteur de thème. Des classes simples, qui ne référencent que `FractalBlazor.Components.Common`. |
| `FractalBlazor.WebSite.StateModel.Tests` | Tests xUnit du modèle, pilotés sans interface. |
| `FractalBlazor.WebSite` | Le site : vues, documentation, démos et éditeur de thème, en bibliothèque de composants. Il référence le modèle et ne dépend d'aucun hôte. |
| `FractalBlazor.WebSite.ServerSide` | Hôte serveur : Blazor interactif côté serveur, avec rendu initial. |
| `FractalBlazor.WebSite.ClientSide` | Hôte WebAssembly : des fichiers statiques, servis par n'importe quel serveur web. |

Quand le modèle a besoin d'un service qui vit dans une assembly qui le référence, il déclare une interface ; cette assembly l'implémente et l'injecte. Ainsi `ThemeSelectorState` enregistre le mode choisi par `IThemeModeStore`, que `FractalBlazor.WebSite` implémente avec un cookie et enregistre dans `AddFractalBlazorWebSite()`.

Chaque hôte ne fournit que la page HTML et le mode de thème initial, relu dans ce cookie avant le premier rendu : par le serveur pendant son rendu, par JS côté WebAssembly. Les deux appellent `AddFractalBlazorWebSite()` et rendent `WebSiteApplication`.

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

## Onglets

`FbTabs` ne fait que sélectionner un index : l'affichage du contenu de l'onglet choisi revient à la page. Comme les champs, il envoie son action à l'état (`TAction`, `Handler`) ou se lie avec `@bind-Value`, et ses libellés sont rendus par `FbDynamicView`.

```razor
<FbTabs TAction="FbSetValue<int>" @bind-Value="tab" Items="@(["Compte", "Mot de passe", "Réglages"])" />

@switch (tab)
{
    case 0: <AccountView /> break;
    case 1: <PasswordView /> break;
    default: <SettingsView /> break;
}
```

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

L'application elle-même est un modèle. Dans la démo, `RootState` porte l'en-tête, la navigation et le pied de page. Chaque section du menu du haut porte les entrées du menu de gauche : des pages, et des groupes qui titrent des pages. Chaque page donne son modèle. Une section sans entrées est elle-même une page :

```csharp
new SectionState(Navigation, MenuItemIntent.Components, section =>
[
    new MenuGroupState(section, MenuItemIntent.Layout, group =>
    [
        new PageLinkState(group, MenuItemIntent.RowStack, new RowStackPage()),
        new PageLinkState(group, MenuItemIntent.Drawer, new DrawerPage(new DrawerDemoState()))
    ]),
    new MenuGroupState(section, MenuItemIntent.Inputs)
]),
new SectionState(Navigation, MenuItemIntent.Compositions, new FormsPage(new ContactState()))
```

Chaque type d'état déclare sa vue avec `[ViewFor<ViewSelector>]`, et la zone de contenu rend la page ouverte sans la connaître :

```razor
<FbStack Scrollable GL PX JVS>
    <FbDynamicView TSelector="ViewSelector" Item="Item.Page" Selector="ViewSelector.Viewer" />
</FbStack>
```

Ouvrir une page ne notifie que l'ancienne entrée, la nouvelle et la zone de contenu : le reste de l'application ne se redessine pas. Chaque modèle de page garde son état d'une visite à l'autre.

## Écran piloté par l'état

La mise en page ne dépend d'aucune media query : c'est l'état qui la décide. `FbWidthSensor` mesure la largeur de son parent et envoie sa catégorie à l'état, comme un champ envoie sa valeur, et seulement quand elle change. Les catégories `FbWidthClass` suivent les seuils des drapeaux `VO_*`/`VU_*` et de la grille : `Base`, puis `XS` dès 512 px, `S` dès 640, `M` dès 768, `L` dès 1024, `XL` dès 1280 et `XXL` dès 1536. L'énumération est ordonnée, l'état compare donc directement :

```csharp
public sealed class ScreenState : ObservableState
{
    public sealed record SetWidth(FbWidthClass Value) : IStateAction<FbWidthClass>;

    // Le premier rendu précède toute mesure : celui d'un bureau.
    public FbWidthClass Width { get; private set; } = FbWidthClass.XL;

    public bool Compact => Width < FbWidthClass.L;
}
```

Les vues suivent l'état avec `if`. Dans la démo, sous 1024 px, les deux menus se replient. L'en-tête prend sa vue compacte (`Summary`), où sections et liens passent dans un menu, et gagne une seconde ligne : le bouton du tiroir qui porte désormais le menu de gauche, puis le fil d'Ariane de la page ouverte (Section ▶ Groupe ▶ Page) :

```razor
<FbPageAnchor>
    <FbWidthSensor TAction="ScreenState.SetWidth" Handler="Item.Screen" Value="Item.Screen.Width" />
    <FbStack>
        <FbStack NoFlex WFL WSL>
            <FbRow NoFlex GM PHL PVM JVC>
                <FbDynamicView TSelector="ViewSelector" Item="Item.Header" Selector="@HeaderView" />
            </FbRow>
            @if (Item.Screen.Compact)
            {
                <FbRow NoFlex GS PHL PVS JVC>
                    <FbDynamicView TSelector="ViewSelector" Item="Item.Navigation.Drawer" Selector="ViewSelector.Viewer" />
                    <FbDynamicView TSelector="ViewSelector" Item="Item.Navigation.Content" Selector="ViewSelector.Summary" />
                </FbRow>
            }
        </FbStack>
        …
```

Le tiroir et le menu de l'en-tête sont des états à part : les ouvrir ne redessine que leur vue. La navigation les referme quand une page ou une section s'ouvre, et l'écran quand il repasse au-dessus de 1024 px.

`FbDrawer` glisse depuis un bord (`Left`, `Right`, `Top`, `Bottom`) et porte les drapeaux de surface. `Size` donne sa largeur, ou sa hauteur en haut et en bas. Trois modes :

- `Modal` : par-dessus tout l'écran, dans un `<dialog>` natif. La page devient inerte, le focus reste dans le tiroir, puis revient où il était.
- `Inline` : à sa place dans la mise en page, qu'il pousse en s'ouvrant.
- `InlineModal` : à sa place, mais par-dessus sa zone (la row ou la stack qui le contient), avec un voile limité à celle-ci.

Il est contrôlé par `IsOpen` : Échap, un clic sur le voile ou une demande du navigateur n'appellent que `IsOpenChanged(false)`, et l'état décide. Fermé, un tiroir en place garde son contenu, rendu inerte ; un tiroir modal ne rend le sien que lorsqu'il est ouvert. Le voile vient de la variable `--fb-drawer-scrim`.

```razor
<FbDrawer Modal Left IsOpen="@Item.IsOpen" IsOpenChanged="@SetOpen" Title="Menu" Size="16rem" WFL>
    <FbDynamicView TSelector="ViewSelector" Item="Item.Menu" Selector="ViewSelector.Summary" />
</FbDrawer>
```

## Assistant des flags

Le bouton ⚑ de l'en-tête de la démo ouvre, dans un tiroir à droite, un assistant pour retrouver un drapeau. Le champ Flag filtre les drapeaux qui commencent par sa saisie ; les cinq premiers s'affichent en haut, lus partie par partie. Les lettres se choisissent aussi colonne par colonne : la première liste les débuts possibles, chaque choix ouvre la suivante, groupée par catégorie (Direction, Spacing…). Des lettres qui se lisent de deux façons sélectionnent les deux : `PL` est Padding → Large, et le début de Padding → Left → Small. La recherche de composants, par nom ou partie de nom, restreint les drapeaux ; choisir un composant le place dans la recherche.

Le site lit les drapeaux sur les composants eux-mêmes : chaque composant public de Layout et de Forms, chacun de ses paramètres booléens. Une grammaire par famille dit ce que signifient les lettres, car elles changent de sens d'une famille à l'autre (`M` ouvre une marge dans une row, c'est la taille moyenne d'un `FbText`). Un test échoue si un drapeau n'est pas expliqué. L'état de l'assistant reçoit ce catalogue par une interface déclarée dans le modèle (`IFlagCatalog`), qui ne connaît aucun composant.

## Performances

Rendu côté serveur de 1 000 lignes encadrées, puis de 1 000 éditeurs à cinq champs (trois textes, une case, une liste), face à MudBlazor, Radzen et Fluent UI. Détails, méthode et limites dans [FractalBlazor.Components.Benchmarks](FractalBlazor.Components.Benchmarks/README.md).

| Scénario | FractalBlazor | MudBlazor | Radzen | Fluent UI |
| --- | ---: | ---: | ---: | ---: |
| Lignes, premier rendu | 4,29 ms | 8,81 ms | 12,53 ms | 16,14 ms |
| Lignes, re-rendu | 1,26 ms | 3,20 ms | 4,23 ms | 4,79 ms |
| Éditeurs, premier rendu | 131,5 ms | 1 107,5 ms | 203,6 ms | 280,2 ms |
| Éditeurs, re-rendu | 27,4 ms | 118,9 ms | 84,2 ms | 112,0 ms |
| Éditeurs, HTML brut | 1 519 Ko | 5 955 Ko | 3 885 Ko | 1 850 Ko |
| Éditeurs, HTML gzip | 60 Ko | 175 Ko | 175 Ko | 131 Ko |

## Développement

```bash
dotnet build FractalBlazor.Components.slnx
```

```bash
dotnet test --project FractalBlazor.Components.Tests/FractalBlazor.Components.Tests.csproj
```

```bash
dotnet test --project FractalBlazor.WebSite.StateModel.Tests/FractalBlazor.WebSite.StateModel.Tests.csproj
```

Le site, rendu côté serveur (`http://localhost:5115`) :

```bash
dotnet run --project FractalBlazor.WebSite.ServerSide/FractalBlazor.WebSite.ServerSide.csproj --launch-profile http
```

Le même site en WebAssembly (`http://localhost:5125`) :

```bash
dotnet run --project FractalBlazor.WebSite.ClientSide/FractalBlazor.WebSite.ClientSide.csproj --launch-profile http
```

Le site est une application d'une seule page pilotée par son modèle : démarrage rapide, principes state-first, mise en page, formulaires, variantes, et un éditeur de thème où couleurs, filets, mélanges, survol, espacements, rayons et typographie se règlent en direct sur un jeu de composants, avec le rapport de contraste et le code C# du thème obtenu. Le plan de finalisation et son avancement sont dans [Plan.md](Plan.md).

## Licence

MIT, voir [LICENSE.txt](LICENSE.txt).
