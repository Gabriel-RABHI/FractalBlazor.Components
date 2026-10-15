# Plan d'implémentation du theming

## Objectif

Le theming doit permettre de définir un thème complet une seule fois, de le décliner en branches comme `Dark` et `Light`, puis d'activer localement des variantes comme `Error`, `Success` ou `Disabled`.

L'implémentation doit conserver le principe déjà employé par les feuilles CSS :

1. des variables fixes définissent le socle du thème actif ;
2. des variables actives peuvent être redéfinies par une variante locale ;
3. des variables temporaires portent les modifications d'interaction, comme le survol ;
4. des variables calculées produisent les couleurs finales avec `color-mix()`, en échantillonnant le dégradé de la variante active.

L'héritage entre thèmes est résolu en C#. La cascade CSS ne sert qu'à appliquer un thème ou une variante dans le DOM et à gérer les états temporaires.

## Répartition des responsabilités

Le système reste réparti entre deux assemblies.

### `FractalBlazor.Components.Layout`

Layout ne connaît que les notions nécessaires à la mise en page :

- espacements ;
- rayons ;
- le dégradé de chaque variante, le neutre et les fonds ;
- frames, bordures et séparateurs ;
- génération de variables CSS ;
- thème et variantes utilisables lorsque Layout est employé seul.

### `FractalBlazor.Components.Forms`

Forms référence Layout et ajoute :

- intensités de texte, placées sur le même dégradé ;
- typographie ;
- branches `Dark` et `Light` ;
- thème Forms complet ;
- composants Blazor d'activation du thème et des variantes.

Un consommateur peut donc utiliser Layout sans Forms, notamment avec MudBlazor ou Radzen. Forms ne doit pas dupliquer les primitives déjà définies dans Layout.

## Composition d'un thème

Les paramètres sont répartis dans les groupes suivants.

### Groupes Layout

```csharp
public sealed class FbThemeLayoutSpacings
{
    public string? S { get; init; }
    public string? M { get; init; }
    public string? L { get; init; }
    public string? X { get; init; }
}

public sealed class FbThemeLayoutCorners
{
    public string? S { get; init; }
    public string? M { get; init; }
    public string? L { get; init; }
    public string? X { get; init; }
}

public sealed class FbThemeLayoutBordersMix
{
    public string? LightMix { get; init; }
    public string? MediumMix { get; init; }
    public string? StrongMix { get; init; }
}

public sealed class FbThemeLayoutColors
{
    public string? LowAnchor { get; init; }   // 0 %
    public string? MidAnchor { get; init; }   // 50 %
    public string? HighAnchor { get; init; }  // 100 %
}

public sealed class FbThemeLayoutSurfaceMix
{
    public string? NeutralMix { get; init; }
    public string? SurfaceOffset { get; init; }
    public string? AccentOffset { get; init; }
    public string? HighlightOffset { get; init; }
    public string? HoverOffset { get; init; }
}

public sealed class FbThemeMasterTint
{
    public string? ColorTint { get; init; }
    public string? TintMix { get; init; }
}
```

La teinte générale (`MasterTint`) fait pencher toutes les ancres d'un thème, ou d'une branche Forms, vers `ColorTint`, de `TintMix`. Chaque ancre garde sa luminosité : seules sa teinte et sa saturation bougent. Une teinte colorée attire toutes les couleurs vers elle ; une teinte grise baisse l'intensité de toutes. Comme chaque couleur des feuilles échantillonne les ancres, toutes suivent. `ColorTint` et `TintMix` s'héritent séparément.

Les espacements S/M/L/X alimentent les margins, paddings et gutters. Les rayons S/M/L/X alimentent les `border-radius`. Les frames Light/Medium/Strong définissent leur force par leur position sur le dégradé ; leur épaisseur est fixée à `1px`.

Tous les mélanges (`TintMix`, `SurfaceMix`, `BordersMix`, et `TextVariantMix` côté Forms) sont des pourcentages CSS de `0%` à `100%` : ce sont des positions sur le dégradé, que le C# et les feuilles calculent.

### Le dégradé d'une variante

Chaque variante a un seul dégradé, défini par trois ancres : `LowAnchor` à 0 %, `MidAnchor` à 50 %, `HighAnchor` à 100 %. Le dégradé va de l'ancre basse à la médiane sur sa première moitié, de la médiane à l'ancre haute sur la seconde. Fonds, textes et filets en sont tous des positions.

- **Le neutre.** La page est au neutre de la branche (`NeutralMix`, 0 % par défaut).
- **Les fonds.** Un bloc rempli (`WBS`, `WBA`, `WBH`) prend le neutre de son plus proche ancêtre rempli et y ajoute l'offset de son fond (`SurfaceOffset`, `AccentOffset`, `HighlightOffset`), borné à 100 %. Il peint le dégradé à ce nouveau neutre, qui devient celui de son contenu. Avec les valeurs par défaut (5 %, 15 %, 45 %), une Surface sur la page est à 5 %, un Accent dans cette Surface à 20 %, un Highlight dans cet Accent à 65 %. L'imbrication compte six niveaux.
- **Les textes et les filets** se placent depuis le neutre de leur bloc, par la fin du dégradé : `100 − (100 − mix) × (100 − neutre) / 100`. À un neutre de 0 %, un texte `Default` à 80 % est à 80 % ; dans un bloc à 75 %, il est à 95 %. Ils restent toujours au-dessus du fond qui les porte.
- **Le survol** (`HoverOffset`) s'ajoute au neutre de l'élément survolé : son fond, ses textes et ses filets montent ensemble.
- **L'inversion** (`Invert`) retourne le dégradé : les ancres basse et haute s'échangent, la médiane reste.

Les branches `Dark` et `Light` gardent chacune leurs ancres et tous leurs réglages. Le thème par défaut lit la palette de base de 950 à 50 en sombre (`Low` 950, `Mid` 500, `High` 50), dans l'autre sens en clair.

### Groupes Forms

```csharp
public sealed class FbThemeFormTextVariantMix
{
    public string? DefaultHighMix { get; init; }
    public string? SubtleHighMix { get; init; }
    public string? MutedHighMix { get; init; }
    public string? HighlightHighMix { get; init; }
}

public sealed class FbThemeFormTypography
{
    public string? TextFontFamily { get; init; }
    public string? CodeFontFamily { get; init; }
    public string? FontSizeBase { get; init; }
    public string? LineHeight { get; init; }

    public string? ExtraSmallCoef { get; init; }
    public string? SmallCoef { get; init; }
    public string? MediumCoef { get; init; }
    public string? LargeCoef { get; init; }
    public string? ExtraLargeCoef { get; init; }
    public string? ExtraExtraLargeCoef { get; init; }

    public string? ThinWeight { get; init; }
    public string? DefaultWeight { get; init; }
    public string? BoldWeight { get; init; }
    public string? ExtraBoldWeight { get; init; }
}
```

Les textes n'ont pas d'ancres propres : `FbThemeFormTextVariantMix` place Subtle, Muted, Default et Highlight sur le dégradé de la variante, comme les filets.

Les tailles de texte ne constituent pas un groupe séparé : elles font partie de `FbThemeFormTypography`. Les six tailles sont calculées à partir de `FontSizeBase` et des coefficients XS/S/M/L/XL/XXL.

Toutes les propriétés sont nullable afin qu'un thème dérivé ne déclare que ses différences. `null` signifie « hériter ». Une chaîne vide ou composée d'espaces est invalide et doit être refusée par la validation.

Ces groupes sont des objets de données. Ils ne génèrent pas eux-mêmes du CSS : la génération est centralisée après résolution afin qu'une propriété C# ne puisse avoir qu'un seul mapping de variable CSS dans tout le système.

## Les trois couches du thème Forms

Un thème Forms possède trois couches strictes.

| Couche | Contenu | Portée |
| --- | --- | --- |
| Main | `Spacings`, `Corners`, `Typography` | Commun à toutes les branches et variantes du thème |
| Branch | `MasterTint`, `TextVariantMix`, `BordersMix`, `SurfaceMix` | Propre à une branche Dark/Light |
| Variant | `LayoutColors` (trois ancres) | Propre à un état visuel local |

La couche Main ne contient pas les couleurs. Une branche choisit la teinte générale, le neutre de départ, les offsets des fonds et du survol, et les positions des textes et des filets adaptées à son contraste. Chaque variante fournit ensuite les trois ancres de son dégradé.

Cette séparation permet à `Error` d'avoir des couleurs différentes en `Dark` et en `Light`, tout en partageant les mêmes espacements, rayons et règles typographiques.

## Modèle C# du thème Forms

```csharp
public sealed class FbThemeSetup
{
    public FbThemeSetup(string name, FbThemeSetup? parent = null)
    {
        Name = name;
        Parent = parent;
    }

    public string Name { get; }
    public FbThemeSetup? Parent { get; }

    // Main
    public FbThemeLayoutSpacings? Spacings { get; init; }
    public FbThemeLayoutCorners? Corners { get; init; }
    public FbThemeFormTypography? Typography { get; init; }

    public IReadOnlyList<FbThemeBranch> Branches { get; init; }
        = Array.Empty<FbThemeBranch>();
}

public sealed class FbThemeBranch
{
    public FbThemeBranch(string name) => Name = name;

    public string Name { get; }

    // Branch
    public FbThemeMasterTint? MasterTint { get; init; }
    public FbThemeFormTextVariantMix? TextVariantMix { get; init; }
    public FbThemeLayoutBordersMix? BordersMix { get; init; }
    public FbThemeLayoutSurfaceMix? SurfaceMix { get; init; }

    public IReadOnlyList<FbThemeColorVariant> Variants { get; init; }
        = Array.Empty<FbThemeColorVariant>();
}

public sealed class FbThemeColorVariant
{
    public FbThemeColorVariant(string name) => Name = name;

    public string Name { get; }

    // Variant
    public FbThemeLayoutColors? LayoutColors { get; init; }
}
```

Seul `FbThemeSetup` possède un `Parent`, car l'héritage porte sur un thème complet. Les branches et variantes sont contenues dans leur thème ; elles n'exposent pas de propriété `Parent` ou `Owner` modifiable qui pourrait contredire cette arborescence.

Lors de la résolution d'un thème dérivé, une branche et une variante masquent les éléments de même nom dans le thème parent.

## Noms réservés

Les noms standards doivent être centralisés pour éviter les chaînes dispersées dans les composants :

```csharp
public static class FbThemeBranches
{
    public const string Dark = "Dark";
    public const string Light = "Light";
}

public static class FbThemeVariants
{
    public const string Default = "Default";
    public const string Selected = "Selected";
    public const string Error = "Error";
    public const string Warning = "Warning";
    public const string Disabled = "Disabled";
    public const string Success = "Success";
    public const string Info = "Info";
}
```

Une valeur de variante vide ou null est normalisée en `Default`. La comparaison des noms est insensible à la casse. Le préfixe CSS correspondant est normalisé en minuscules et en kebab-case : `Error` devient `error` dans `--fb-error-*`.

## Thème par défaut

Le thème racine est créé explicitement par une fabrique :

```csharp
FbThemeSetup defaultTheme = FbThemeDefaults.Create();
```

Il respecte les invariants suivants :

- son nom est `Default` ;
- son `Parent` est null ;
- les groupes Main sont entièrement renseignés ;
- il contient les branches `Dark` et `Light` ;
- chaque branche contient les variantes `Default`, `Selected`, `Error`, `Warning`, `Disabled`, `Success` et `Info` ;
- ses dégradés viennent de la palette de base : 950 / 500 / 50 en sombre, 50 / 500 / 950 en clair ; `Disabled` garde un dégradé court (950 / 700 / 500, et son miroir en clair) pour que ses textes restent ternes ;
- ses mélanges valent, dans les deux branches : neutre 0 %, Surface 5 %, Accent 15 %, Highlight 45 %, survol 10 % ; filets 5 / 10 / 15 % ; textes Subtle 40 %, Muted 60 %, Default 80 %, Highlight 95 % ;
- chaque valeur terminale est non null après résolution.

Le constructeur sans argument ne doit pas créer implicitement ce thème. La fabrique rend l'intention explicite et évite de fabriquer plusieurs racines indépendantes par erreur.

## Héritage et résolution

Le registre conserve les définitions partielles, mais les composants et le générateur CSS ne consomment que des snapshots entièrement résolus.

### Règles de résolution

Pour un thème `T`, une branche `B` et une variante `V` :

1. les valeurs Main sont recherchées dans `T`, puis dans chacun de ses parents ;
2. la branche `B` de `T` masque la branche `B` du parent ;
3. les propriétés null de la branche sont complétées par la branche `B` du parent ;
4. la variante `V` de la branche `B` masque la même variante du parent ;
5. les propriétés null de la variante sont complétées par la variante `V` du parent ;
6. si `V` n'existe dans aucun parent, ses valeurs manquantes retombent sur la variante `Default` résolue de la même branche ;
7. la chaîne doit finir sur le thème racine `Default`, qui est complet.

Cette résolution est effectuée propriété par propriété à l'intérieur de chaque groupe. Remplacer `Spacings.S` ne doit donc pas obliger à recopier M/L/X.

### Validation

L'enregistrement doit échouer immédiatement dans les cas suivants :

- nom de thème, de branche ou de variante vide ;
- thème portant le même nom qu'un thème déjà enregistré ;
- doublon de branche dans un thème ;
- doublon de variante dans une branche ;
- cycle dans la chaîne des parents ;
- valeur CSS vide ou manifestement mal formée ;
- mélange qui n'est pas un pourcentage de `0%` à `100%` ;
- thème impossible à résoudre jusqu'à une racine complète.

## Registre de thèmes

Le registre est un singleton injecté par DI :

```csharp
public interface IFbFormThemeRegistry
{
    FbThemeSetup Default { get; }

    void Register(FbThemeSetup theme);
    bool TryGet(string name, out FbThemeSetup theme);
    FbResolvedTheme Resolve(string theme, string branch);
}
```

`Register` refuse les doublons. Un éventuel remplacement doit passer par une méthode explicite séparée afin d'éviter les modifications silencieuses.

Les snapshots résolus sont immuables et mis en cache par couple `(Theme, Branch)`. L'enregistrement se fait au démarrage de l'application, avant l'activation du premier composant `FbTheme`.

L'extension DI construit d'abord le thème par défaut, puis exécute les enregistrements applicatifs :

```csharp
builder.Services.AddFractalBlazorTheming(registry =>
{
    registry.Register(myTheme);
    registry.Register(mySecondTheme);
});
```

Exemple de thème dérivé :

```csharp
var myTheme = new FbThemeSetup("MyTheme", registry.Default)
{
    Spacings = new FbThemeLayoutSpacings
    {
        S = "0.3rem",
        M = "0.5rem"
    }
};

registry.Register(myTheme);
```

`SpaceL`, `SpaceX`, les rayons, la typographie, les branches et les variantes sont hérités du thème par défaut.

Un second thème peut hériter du premier :

```csharp
var mySecondTheme = new FbThemeSetup("MySecondTheme", myTheme)
{
    Spacings = new FbThemeLayoutSpacings
    {
        S = "0.25rem"
    },
    Corners = new FbThemeLayoutCorners
    {
        S = "0rem"
    }
};

registry.Register(mySecondTheme);
```

Ici, `Spacing.S` et `Corner.S` viennent de `MySecondTheme`, `Spacing.M` vient de `MyTheme`, et le reste vient du thème `Default`.

Une branche ou une variante se redéfinit sans recopier toute la hiérarchie :

```csharp
var brandTheme = new FbThemeSetup("Brand", registry.Default)
{
    Branches =
    [
        new FbThemeBranch(FbThemeBranches.Light)
        {
            Variants =
            [
                new FbThemeColorVariant(FbThemeVariants.Error)
                {
                    LayoutColors = new FbThemeLayoutColors
                    {
                        LowAnchor = "#fff4f5",
                        MidAnchor = "#d03050"
                    }
                }
            ]
        }
    ]
};
```

L'ancre haute, non renseignée, est héritée de `Default/Light/Error`. Les mélanges sont hérités au niveau de `Default/Light`. Une variante nouvelle prend les ancres qu'elle ne déclare pas à la variante `Default` de la même branche.

## Presets

Chaque groupe propose des presets cohérents avec son rôle :

```csharp
FbThemeLayoutSpacings.Dense
FbThemeLayoutSpacings.Default
FbThemeLayoutSpacings.Large
FbThemeLayoutSpacings.Spaced

FbThemeLayoutCorners.Square
FbThemeLayoutCorners.Default
FbThemeLayoutCorners.Rounded

FbThemeFormTypography.Compact
FbThemeFormTypography.Default
FbThemeFormTypography.Large
```

Un preset doit être immuable ou retourner une nouvelle instance. Il ne doit jamais exposer une instance statique mutable partagée entre plusieurs thèmes.

Exemple :

```csharp
var spaciousTheme = new FbThemeSetup("Spacious", registry.Default)
{
    Spacings = FbThemeLayoutSpacings.Spaced,
    Corners = FbThemeLayoutCorners.Rounded
};
```

## Modèle CSS

Le CSS des composants est entièrement statique. Il ne contient aucun nom de thème, de branche ou de variante. Il ne connaît que les variables actives `--fb-*`, les variables temporaires `--fb-current-*` et les valeurs calculées.

Le composant `FbTheme` génère les valeurs propres au thème sélectionné. Le composant `FbVariant` redirige localement les variables actives vers l'un des jeux de valeurs déjà générés.

Le pipeline comporte quatre niveaux.

### 1. Variables résolues et préfixées

Après résolution de l'héritage, `FbTheme` écrit dans sa balise `<style>` un jeu complet de variables pour la variante `Default`, puis un jeu complet pour chaque autre variante de la branche.

La variante fait partie du nom de la variable :

```css
--fb-default-low-anchor: #09090B;
--fb-default-mid-anchor: #71717B;
--fb-default-high-anchor: #FAFAFA;

--fb-error-low-anchor: #4D0218;
--fb-error-mid-anchor: #FF2056;
--fb-error-high-anchor: #FFF1F2;
```

Les mélanges de la branche ne dépendent pas de la variante. Le neutre et les offsets restent des pourcentages, puisqu'ils s'additionnent ; les positions des filets et des textes sont écrites en fractions, puisque CSS ne multiplie pas deux pourcentages :

```css
--fb-neutral-mix: 0%;
--fb-surface-offset: 5%;
--fb-accent-offset: 15%;
--fb-highlight-offset: 45%;
--fb-hover-offset: 10%;
--fb-frame-light-mix: 0.05;
--fb-frame-medium-mix: 0.1;
--fb-frame-strong-mix: 0.15;

/* Forms */
--fb-fg-subtle-mix: 0.4;
--fb-fg-muted-mix: 0.6;
--fb-fg-default-mix: 0.8;
--fb-fg-highlight-mix: 0.95;
```

Les ancres sont des valeurs sources. Elles ne sont jamais consommées directement par les classes CSS des composants.

Quand la teinte générale est active (`TintMix` au-dessus de `0%`), le bloc la déclare une fois, et chaque ancre la lit. L'ancre est mélangée, dans l'espace Oklab, avec la teinte ramenée à sa propre luminosité, que le C# calcule :

```css
/* Thème Layout par défaut : Zinc 950 teinté à 5 % vers Sky 500 */
--fb-tint-color:#00A6F4;
--fb-tint-mix:5%;
--fb-default-low-anchor:color-mix(in oklab,#09090B,oklch(from var(--fb-tint-color) 0.1408 c h) var(--fb-tint-mix));
```

Une ancre qui n'est pas hexadécimale est mélangée avec la teinte telle quelle, sans garder sa luminosité. À `0%`, les ancres sont écrites telles que déclarées. `FbThemeContrast` applique la même teinte avant de mesurer les contrastes.

Les espacements et les rayons, qui ne changent pas avec la variante, sont également émis dans le bloc du thème. Ils conservent leur préfixe `default` pour leur alias actif :

```css
--fb-default-space-s
--fb-default-space-m
--fb-default-space-l
--fb-default-space-x

--fb-default-radius-s
--fb-default-radius-m
--fb-default-radius-l
--fb-default-radius-x
```

La typographie, qui appartient à Main et n'est pas affectée par `FbVariant`, est émise une seule fois :

```css
--fb-txt-font-family
--fb-code-font-family
--fb-txt-base-size
--fb-txt-base-weight
--fb-txt-base-line-height
--fb-txt-xs-coef
--fb-txt-s-coef
--fb-txt-m-coef
--fb-txt-l-coef
--fb-txt-xl-coef
--fb-txt-xxl-coef
--fb-txt-t-weight
--fb-txt-b-weight
--fb-txt-xb-weight
```

`--fb-txt-base-size` est l'unique variable de taille de base. Aucun alias non préfixé comme `--font-size-base` n'est conservé.

### 2. Variables actives

Les classes CSS statiques ne consomment que les variables actives. Dans le bloc `<style>` du thème, elles pointent initialement vers la variante `Default` :

```css
--fb-space-s: var(--fb-default-space-s);
--fb-space-m: var(--fb-default-space-m);
--fb-radius-s: var(--fb-default-radius-s);

--fb-low-anchor: var(--fb-default-low-anchor);
--fb-mid-anchor: var(--fb-default-mid-anchor);
--fb-high-anchor: var(--fb-default-high-anchor);
```

Une variante redéfinit localement les trois ancres actives pour les faire pointer vers un autre préfixe. Elle le fait par une classe (`fb-v-error`, voir « Variante en classe »), dont la règle contient :

```css
--fb-low-anchor: var(--fb-error-low-anchor);
--fb-mid-anchor: var(--fb-error-mid-anchor);
--fb-high-anchor: var(--fb-error-high-anchor);
```

Les valeurs sources de chaque variante sont complètes après résolution C#. Les redirections ne nécessitent donc ni valeur en dur, ni fallback, ni calcul de thème dans un composant.

### 3. Le neutre de chaque bloc

Le neutre d'un bloc rempli est celui de son plus proche ancêtre rempli, plus l'offset de son fond. Une propriété personnalisée ne peut pas lire sa propre valeur héritée (`--fb-neutral: calc(var(--fb-neutral) + …)` est invalide), et la fonction `inherit()` n'est pas disponible : chaque profondeur a donc sa variable, `--fb-neutral-1` à `--fb-neutral-6`, choisie en comptant les ancêtres remplis. Chaque fond donne son offset dans `--fb-fill-offset` :

```css
.fb-wba { --fb-fill-offset: var(--fb-accent-offset); }

/* F = :is(.fb-wbs, .fb-wba, .fb-wbh) ; un bloc qui a deux ancêtres remplis, pas trois */
F F F:not(F F F *) {
    --fb-neutral-3: min(100%, calc(var(--fb-neutral-2) + var(--fb-fill-offset)));
    --fb-neutral: var(--fb-neutral-3);
}
```

La sixième règle n'a pas de `:not()` : un bloc plus profond repart du neutre du cinquième niveau. Un thème, de page ou `Scoped` (classe `fb-theme`), met toutes les profondeurs à son neutre : le premier fond à l'intérieur, à quelque profondeur qu'il soit dans la page, part de là. L'aperçu de l'éditeur de thème repart ainsi de son propre neutre.

Le survol vient par-dessus : `--fb-current-neutral`, le neutre dont les couleurs sont calculées, vaut le neutre du bloc, plus `--fb-hover-mix-offset` tant qu'un élément `fb-hover` est survolé. Sa valeur vient de la branche (`SurfaceMix.HoverOffset`, écrit dans `--fb-hover-offset`) ; l'appui d'un bouton en ajoute le double, et le paramètre `HoverMixOffset` la remplace sur un composant. Le survol monte le fond, les textes et les filets de l'élément ensemble ; les blocs remplis qu'il contient gardent leur neutre.

### 4. Variables calculées

Toutes les couleurs échantillonnent le dégradé actif à une position `P` : de l'ancre basse à la médiane sur la première moitié, de la médiane à l'ancre haute sur la seconde.

```css
color-mix(in oklab,
    color-mix(in oklab, var(--fb-low-anchor), var(--fb-mid-anchor) clamp(0%, calc(P * 2), 100%)),
    var(--fb-high-anchor) clamp(0%, calc(P * 2 - 100%), 100%))
```

| Variable | Position |
| --- | --- |
| `--fb-current-bg` | le neutre courant : le fond du bloc |
| `--fb-bg-surface`, `--fb-bg-accent`, `--fb-bg-highlight` | le neutre courant plus l'offset du fond : ce que prendrait un fond de ce type ici |
| `--fb-frame-light`, `--fb-frame-medium`, `--fb-frame-strong` | `100% - (1 - mix) * (100% - neutre)` |
| `--fb-fg-subtle`, `--fb-fg-muted`, `--fb-fg-default`, `--fb-fg-highlight` (Forms) | `100% - (1 - mix) * (100% - neutre)` |

Ces couleurs sont calculées là où le neutre ou les ancres changent : sur `:root`, `.fb-theme-scope`, `:where(.fb-variant)` (voir « Variante d'un contrôle »), sur chaque fond `fb-wbs`, `fb-wba`, `fb-wbh`, et sur `.fb-hover:hover`. Partout ailleurs, elles sont héritées. Une variable personnalisée est résolue sur l'élément qui la déclare : sans cette redéclaration, un enfant hériterait des couleurs calculées avec les ancres et le neutre de son parent. Ces mêmes éléments rétablissent `color: var(--fb-fg-default)`, sans spécificité, si bien qu'un texte brut suit les couleurs de son bloc.

Une frame (`WFL`, `WFM`, `WFS`) et un séparateur prennent `--fb-frame-light`, `--fb-frame-medium` ou `--fb-frame-strong` du bloc qui les porte. La piste des interrupteurs, la case à cocher et les barres de défilement prennent `--fb-frame-strong`, l'anneau de progression `--fb-frame-medium`. Les contrôles (anneau de focus, interrupteur coché, curseur) prennent l'ancre médiane, la plus colorée du dégradé ; le voile d'un tiroir modal, l'ancre basse.

Cette indirection garantit quatre propriétés :

- le CSS et les classes des composants restent statiques ;
- l'activation d'une variante est uniquement une redirection de variables CSS ;
- le hover et les autres interactions se superposent à la variante active ;
- les valeurs calculées restent relatives aux ancres de la variante courante et au neutre du bloc.

## Génération du bloc `<style>` du thème

Il n'existe aucune classe CSS générée pour un thème, une branche ou une variante. Les classes des composants sont écrites une fois dans les feuilles statiques de Layout et Forms.

L'activation de `<FbTheme Theme="MyTheme" Branch="Light">` déclenche les opérations suivantes :

1. résoudre complètement `MyTheme/Light` par la logique d'héritage C# ;
2. sérialiser les paramètres Main et Branch ;
3. sérialiser la variante `Default` sous le préfixe `--fb-default-*` ;
4. sérialiser toutes les autres variantes sous leurs préfixes respectifs ;
5. initialiser les variables actives avec des références vers `--fb-default-*` ;
6. rendre le résultat dans une balise `<style>`.

Avec le sélecteur par défaut, le DOM produit est de la forme :

```html
<style>
    :root {
        --fb-default-space-s: 0.3rem;
        --fb-default-space-m: 0.5rem;

        --fb-neutral-mix: 0%;
        --fb-surface-offset: 5%;
        --fb-frame-light-mix: 0.05;
        --fb-fg-default-mix: 0.8;
        color-scheme: light;

        --fb-default-low-anchor: #FAFAFA;
        --fb-default-mid-anchor: #71717B;
        --fb-default-high-anchor: #09090B;

        --fb-error-low-anchor: #FFF1F2;
        --fb-error-mid-anchor: #FF2056;
        --fb-error-high-anchor: #4D0218;

        --fb-space-s: var(--fb-default-space-s);
        --fb-low-anchor: var(--fb-default-low-anchor);
        --fb-mid-anchor: var(--fb-default-mid-anchor);
        --fb-high-anchor: var(--fb-default-high-anchor);
    }
</style>
```

L'exemple est abrégé : la sortie réelle contient toutes les propriétés résolues de toutes les variantes enregistrées dans la branche active.

Le composant peut accepter un `Selector`, égal à `:root` par défaut. Ce sélecteur détermine où les variables du thème sont déclarées ; il ne dépend jamais du nom du thème ou de la branche.

### Générateur de noms

La nomenclature doit être produite par un utilitaire unique. Deux opérations suffisent :

```csharp
static string ValueName(string variant, string token)
    => $"--fb-{Normalize(variant)}-{token}";

static string ActiveReference(string variant, string token)
    => $"--fb-{token}:var(--fb-{Normalize(variant)}-{token});";
```

Exemples :

```csharp
ValueName("Error", "low-anchor");
// --fb-error-low-anchor

ActiveReference("Error", "low-anchor");
// --fb-low-anchor:var(--fb-error-low-anchor);
```

Les tokens Variant sont centralisés dans `FbThemeCssNames.VariantReferences`, partagé par le générateur du thème, `FbVariant` et `FbLayoutVariant`, pour Layout comme pour Forms : les trois ancres de `FbThemeLayoutColors`. Les mélanges restent au niveau de la branche.

```text
low-anchor   mid-anchor   high-anchor
```

Une variante peut être inversée (`Invert`) : l'ancre basse active pointe vers l'ancre haute de la variante, et l'inverse ; la médiane reste. Le dégradé est retourné, ce qui transforme une variante sombre en variante claire. L'inversion reste une redirection, sans aucune valeur calculée en C#.

Les chaînes de redirection sont construites une fois par combinaison (variante, inversion) puis mises en cache, dans une limite de 256 entrées.

Les filets ont une épaisseur fixe de `1px`. Leur force visuelle dépend uniquement des tokens `frame-*-mix`.

Une frame (`WFL`, `WFM`, `WFS`) est un outline tracé sur le bord, à l'intérieur de la boîte : elle ne prend aucune place. L'ajouter ou la retirer ne change ni la taille de la boîte ni la position de son contenu. Peinte par-dessus le contenu, elle n'est cachée ni par un enfant qui a son propre fond, ni par le contenu d'une zone qui défile. Un anneau de focus, outline lui aussi, la remplace tant qu'il s'affiche.

```css
outline: 1px solid var(--fb-frame-border-color);
outline-offset: -1px;
```

Une frame peut ne border qu'un côté : `WFTx`, `WFRx`, `WFBx`, `WFLx` (With → Frame → Top, Right, Bottom, Left, avec `x` = `L`, `M` ou `S`) tracent une ligne sur ce côté, chacun avec sa force ; ils se combinent entre eux et avec une frame complète. `WFL` reste la frame Light ; `WFLL`, `WFLM` et `WFLS` sont la ligne de gauche. Comme la frame, une ligne de côté ne prend aucune place. Elle est tracée par un `::after` posé sur la boîte (`position: absolute; inset: 0`, avec le rayon de la boîte), peint par-dessus le contenu : un enfant qui a un fond ne la cache pas. La boîte devient le bloc contenant de ce `::after` par un `position: relative` sans spécificité, si bien qu'une boîte positionnée autrement garde sa position. Une boîte qui défile elle-même (`Scrollable`) emporterait ce `::after` avec son contenu : elle trace ses lignes de côté en couches de fond, qui restent en place, sous le contenu.

`WFD` (With → Frame → Dot) met en pointillés la frame et les lignes de côté. Il n'agit que sur une boîte qui a une frame ou une ligne : seul, un style pointillé ferait apparaître l'outline par défaut du navigateur.

`WRR` (With → Radius → Round) met le rayon à 50 % : un cercle sur une boîte carrée, une ellipse sinon. Écrit après lui, un rayon de l'échelle (`WRS`…`WRX`, `WRN`) le remplace, et inversement.

Les composants dérivés de `FbLayoutSurfaceComponentBase` exposent le drapeau `EO` (Enable → Outline) pour ajouter un anneau autour de la boîte, à l'extérieur de sa frame, dans la couleur calculée de la frame courante. C'est une ombre, puisque la frame est un outline : un champ qui a le focus garde ses deux traits, et l'anneau ne prend pas de place non plus.

```css
box-shadow: 0 0 0 var(--fb-outline-size) var(--fb-frame-border-color);
```

`--fb-outline-size` vaut `1px` par défaut et peut être redéfini localement.

Le changement de `Theme` ou de `Branch` provoque une nouvelle résolution et un nouveau rendu du contenu de `<style>`. Il ne modifie aucune feuille CSS statique.

## Composants Blazor

### `FbTheme`

`FbTheme` active un thème et une branche sur un sous-arbre :

```razor
<FbTheme Theme="MyTheme" Branch="Light">
    ...
</FbTheme>
```

Le composant :

1. résout `(MyTheme, Light)` dans le registre ;
2. génère la balise `<style>` contenant le thème résolu et toutes ses variantes ;
3. initialise les variables actives vers la variante `Default` ;
4. ajoute à ce `<style>` les règles des variantes que le thème ajoute aux variantes standard (voir « Variante en classe ») ;
5. rend ensuite son `ChildContent`, sans rien lui passer en cascade.

Il n'ajoute pas le nom du thème au DOM. Son `Selector` vaut `:root` par défaut. Les variables ne sont sérialisées qu'au changement de thème ou de branche.

Rien n'est cascadé aux descendants : une variante est une classe CSS, que la feuille ou le `<style>` du thème résout. Mesuré avant ce choix (BenchmarkDotNet MediumRun, i9-9900K, .NET 10.0.12, 1 000 boutons minimaux identiques dans un `FbTheme`), un `[CascadingParameter]` coûtait 27 à 56 µs au premier rendu (+2,3 % à +4,6 %, sous 1 à 10 niveaux de composants) et environ 100 Ko, 18 à 36 µs au re-rendu.

#### Thème local

Avec `Scoped="true"`, `FbTheme` n'écrit aucun `<style>` global pour ses variables. Il rend une `div class="fb-theme-scope fb-theme"` qui porte toutes les variables en style inline, puis son contenu. `Selector` est alors ignoré. Si le thème ajoute des variantes aux variantes standard, leurs règles sont écrites dans un `<style>` juste avant cette div.

```razor
<FbTheme Theme="Default" Branch="Dark">
    ...
    <FbTheme Theme="Default" Branch="Light" Scoped>
        <FbStack WBS PL>Îlot clair dans une page sombre</FbStack>
    </FbTheme>
</FbTheme>
```

La classe `fb-theme` rétablit dans l'îlot la police, la taille et la couleur de texte que `html` reçoit pour le thème de page. `FbLayoutTheme` accepte le même paramètre.

Deux `FbTheme` non locaux écrivent sur le même sélecteur et s'écrasent : une page n'en contient qu'un, les autres sont `Scoped`.

Installation à la racine de l'application :

```razor
<body>
    <FbTheme Theme="Default" Branch="Dark">
        <Routes />
    </FbTheme>
</body>
```

### `FbVariant`

`FbVariant` active une variante dans le contexte du thème englobant :

```razor
<FbVariant Variant="Error">
    ...
</FbVariant>
```

Le composant :

1. normalise la variante vide en `Default` ;
2. rend un `<div class="fb-theme-scope">` visuellement neutre (`display: contents`) : ses enfants participent directement à la mise en page du parent ;
3. pose sur cette div les classes de la variante, `fb-invert` en plus si `Invert` est posé ;
4. rend son `ChildContent` dans ce scope.

Pour `Error`, le rendu est de la forme :

```html
<div class="fb-theme-scope fb-variant fb-v-error">
    ...
</div>
```

Le rendu ne contient aucune valeur calculée en C#, ni aucun style inline. Le neutre n'est pas touché : une variante placée dans un bloc à 20 % peint ses fonds à partir de 20 %, avec ses propres ancres.

Le scope rétablit aussi `color: var(--fb-fg-default)`, si bien qu'un texte brut placé dans `Error` prend la couleur de la variante.

#### Variante en classe

Une variante est une classe CSS. `fb-variant` recalcule les couleurs sur l'élément ; `fb-v-error` redirige ses trois ancres vers celles de `Error` ; `fb-invert` échange les ancres basse et haute :

```css
.fb-v-error {
    --fb-low-anchor: var(--fb-error-low-anchor);
    --fb-mid-anchor: var(--fb-error-mid-anchor);
    --fb-high-anchor: var(--fb-error-high-anchor);
}

.fb-v-error.fb-invert {
    --fb-low-anchor: var(--fb-error-high-anchor);
    --fb-high-anchor: var(--fb-error-low-anchor);
}
```

Les règles des sept variantes standard sont dans la feuille Layout, mise en cache par le navigateur ; un test vérifie qu'elles sont celles qu'écrit `FbThemeCssNames.VariantRules`. Celles des variantes qu'un thème ajoute sont écrites par `FbTheme` (ou `FbLayoutTheme`) dans son `<style>`. Le nom de la classe est le nom de la variante normalisé : `Brand Accent` donne `fb-v-brand-accent`.

Une variante que le thème ne connaît pas n'a pas de règle : l'élément garde les ancres de son parent, au lieu de perdre ses couleurs. Aucune vérification n'est faite à l'exécution, et rien n'a besoin d'être cascadé.

#### Variante d'un contrôle

Les contrôles (champs, listes, cases, interrupteurs, onglets, boutons) et les icônes ne s’enveloppent pas dans un scope. Quand leur `Variant` ou le drapeau `D` (désactivé) le demande, leur élément racine porte lui-même les classes de la variante :

```html
<div class="fb-row … fb-control fb-field fb-variant fb-v-error">
```

Les formules sont redéclarées sur `:where(.fb-variant)`, de spécificité nulle : les règles propres au composant (`fb-wba`, `fb-control`…) gardent la main, comme lorsqu'elles s'appliquaient à l'enfant d'un scope. Sans variante, l'élément n'a pas ces classes, et le contrôle hérite de la variante de son parent. Passer à `Error` pendant la saisie ne change que l'attribut `class` du même élément : le DOM, et le focus, restent en place.

Un thème ou une branche inconnus lèvent une exception explicite dans `FbTheme`.

#### Inversion locale

`Invert` retourne le dégradé :

```razor
<FbVariant Invert>
    <FbStack WBS PL>Carte claire dans une branche sombre</FbStack>
</FbVariant>
```

```html
<div class="fb-theme-scope fb-variant fb-v-default fb-invert">
```

Les ancres basse et haute sont les deux extrémités du dégradé : la carte est claire, ses textes sombres. Avec le thème par défaut, le sombre inversé donne le clair, puisque la branche claire lit la même palette dans l'autre sens. `FbThemeContrast.Analyze(theme, inverted: true)` vérifie les variantes inversées. `FbLayoutVariant` accepte aussi `Invert`, avec les mêmes classes.

### Imbrication et retour au défaut

```razor
<FbVariant Variant="Error">
    <FbText>Erreur</FbText>

    <FbVariant Variant="Default">
        <FbText>Zone normale</FbText>
    </FbVariant>

    <FbText>Erreur à nouveau</FbText>
</FbVariant>
```

La variante interne redirige le même jeu de variables actives vers `--fb-default-*`. Ses déclarations inline remplacent l'héritage du scope `Error`, puis les variables temporaires et calculées se reconstruisent automatiquement à partir de ce nouveau jeu actif.

Un composant métier peut relayer directement son état :

```razor
<FbVariant Variant="@Status">
    ... rendu du champ ...
</FbVariant>

@code {
    [Parameter]
    public string Status { get; set; } = FbThemeVariants.Default;
}
```

## Thème Layout autonome

Lorsque Layout est utilisé sans Forms, le modèle ne comporte pas de branche :

```csharp
public sealed class FbLayoutThemeSetup
{
    public FbLayoutThemeSetup(string name, FbLayoutThemeSetup? parent = null)
    {
        Name = name;
        Parent = parent;
    }

    public string Name { get; }
    public FbLayoutThemeSetup? Parent { get; }

    // Main
    public FbThemeLayoutSpacings? Spacings { get; init; }
    public FbThemeLayoutCorners? Corners { get; init; }
    public FbThemeMasterTint? MasterTint { get; init; }
    public FbThemeLayoutBordersMix? BordersMix { get; init; }
    public FbThemeLayoutSurfaceMix? SurfaceMix { get; init; }

    public IReadOnlyList<FbLayoutThemeVariant> Variants { get; init; }
        = Array.Empty<FbLayoutThemeVariant>();
}

public sealed class FbLayoutThemeVariant
{
    public FbLayoutThemeVariant(string name) => Name = name;

    public string Name { get; }
    public FbThemeLayoutColors? LayoutColors { get; init; }
}
```

La résolution suit les mêmes règles que le thème Forms, sans niveau Branch. `FbLayoutTheme` génère une balise `<style>` contenant la variante `Default`, toutes les variantes Layout résolues et les alias actifs. `FbLayoutVariant` rend une div neutre dont l'attribut `style` porte les classes de la variante choisie ; les mélanges restent communs au thème Layout. Les feuilles sont les mêmes : neutre, imbrication, survol et filets fonctionnent sans Forms, qui n’ajoute que les textes.

Aucune classe propre à un thème n’est générée ; seules les règles des variantes qu’un thème ajoute aux variantes standard sont écrites dans son `<style>`.

## Contrôle de contraste

Les pourcentages de mélange garantissent une palette cohérente, mais aucun contraste. `FbThemeContrast.Analyze` calcule donc en C# les couleurs que le navigateur peindra, avec les mêmes formules que les feuilles CSS :

- `FbColorMath.Mix` reproduit `color-mix(in oklab, A, B p%)` ;
- `FbColorMath.Sample` échantillonne le dégradé à trois ancres comme les feuilles ;
- `FbColorMath.Relative` place un texte ou un filet depuis le neutre de son bloc ;
- `FbColorMath.ContrastRatio` donne le ratio WCAG 2.2, de 1 à 21.

```csharp
var theme = registry.Resolve("Default", FbThemeBranches.Dark);
var report = FbThemeContrast.Analyze(theme);

foreach (var entry in report.Below(4.5))
    Console.WriteLine(entry); // Warning Default on Highlight: #FFE7C4 on #EA8C01 = 2.12:1
```

Le rapport couvre chaque texte (`Subtle`, `Muted`, `Default`, `Highlight`) sur chaque fond (`Surface`, `Accent`, `Highlight`) de chaque variante. Chaque fond est un bloc de premier niveau, posé sur la page : le neutre de la branche plus son offset ; les textes sont placés depuis ce neutre. `MeetsAA` vaut 4,5:1 (texte courant) et `MeetsAALarge` 3:1 (grand texte, contours de contrôles). Une variante dont une ancre n'est pas au format `#RGB` ou `#RRGGBB` apparaît dans `Skipped`.

Les tests de la solution imposent l’AA aux textes `Default` et `Highlight` de la variante `Default`, celle du corps des pages, sur les fonds Surface et Accent, et au texte `Highlight` sur le fond Highlight, pour les deux branches du thème par défaut et pour une carte inversée. Le fond Highlight (45 %) est près du milieu du dégradé : le texte `Default` y atteint 4,1:1 en sombre et 4,2:1 en clair, sous l’AA ; le texte `Highlight` y donne 5,2:1 et 4,8:1. L’éditeur de thème affiche le plus faible ratio de chaque texte, sans la variante `Disabled`, que WCAG n’oblige à aucun contraste. `Subtle` et `Muted` sont des textes secondaires, et les variantes colorées un choix de design réglé dans l'éditeur de thème : leur contraste figure dans le rapport, sans être imposé.

## Adaptation des feuilles CSS

Les règles à suivre sont :

- conserver `color-mix(in oklab, ...)` pour les fonds, textes et frames ;
- faire consommer aux classes statiques uniquement les variables actives et calculées ;
- ne lire que les trois ancres actives `--fb-low-anchor`, `--fb-mid-anchor` et `--fb-high-anchor` ;
- porter les états temporaires (survol, appui) par `--fb-current-neutral` ;
- recalculer fonds, textes et frames sur chaque scope, variante, fond et élément survolé ;
- faire consommer à chaque taille son coefficient XS/S/M/L/XL/XXL ;
- ajouter les variables de familles de fontes au lieu de coder `text-font` et `code-font` directement dans les règles ;
- retirer toute logique de thème ou de variante des classes CSS statiques ;
- garantir que le bloc `<style>` définit les ancres de chaque variante et les mélanges de la branche ;
- garantir que la règle d’une variante redirige exactement les ancres que `FbTheme` émet pour elle ;
- centraliser la création des noms de variables pour que Layout, Forms, `FbTheme` et `FbVariant` produisent exactement la même nomenclature.

La classe statique actuellement nommée `FbTheme` est supprimée ; ce nom est ensuite attribué au composant `<FbTheme>`. `FbThemeBaseColors` devient l’unique source de couleurs du thème par défaut ; les alias de propriétés et variantes orthographiées incorrectement ne sont pas conservés.

Les composants de configuration séparés du socle sont remplacés par la nouvelle architecture :

- `FbLayoutThemeStyle` est supprimé au profit du bloc `<style>` produit directement par `FbLayoutTheme` ;
- les réglages de couleurs alimentent `FbThemeLayoutColors` et `FbThemeFormTextVariantMix` ;
- les réglages de fontes alimentent `FbThemeFormTypography`.

La bibliothèque étant nouvelle, aucun membre `[Obsolete]`, aucune façade de compatibilité et aucun ancien nom public ne doivent subsister dans l'implémentation finale.

## Ordre d'implémentation

### Étape 1 — Modèle de données

- créer les sept groupes de paramètres ;
- ajouter les presets immuables ;
- créer les types Setup, Branch et Variant ;
- ajouter les constantes de noms standards ;
- normaliser tous les noms et toutes les valeurs CSS.

### Étape 2 — Résolution

- implémenter le registre DI ;
- créer le thème racine complet ;
- implémenter l'héritage propriété par propriété ;
- matérialiser des snapshots immuables ;
- mettre en cache les couples `(Theme, Branch)` ;
- valider les cycles, doublons et valeurs manquantes.

### Étape 3 — Génération des variables

- créer un mappeur unique entre propriétés C# et variables CSS ;
- sérialiser dans `<style>` toutes les valeurs résolues de `Default` et des autres variantes ;
- générer les alias actifs pointant initialement vers `Default` ;
- centraliser les tokens qu’une règle de variante redirige ;
- produire les classes de variante par concaténation de chaînes, mises en cache, sans recalculer le thème ;
- conserver les niveaux résolu, actif, temporaire et calculé ;
- garantir un ordre de sortie déterministe pour les tests et le diagnostic.

### Étape 4 — Composants Blazor

- implémenter `FbTheme` et la génération de sa balise `<style>` ;
- implémenter `FbVariant` avec une div `.fb-theme-scope` et les classes de sa variante ;
- gérer l'imbrication et le retour à `Default` ;
- implémenter les équivalents Layout autonomes.

### Étape 5 — Intégration des composants

- remplacer les valeurs de thème codées en dur par les variables actives ;
- vérifier les mappings spacing, radius, background, frame et texte ;
- faire porter les statuts des composants par `FbVariant` ;
- supprimer tous les membres marqués `[Obsolete]` ;
- supprimer les anciens alias, helpers et composants remplacés, sans façade de compatibilité.

### Étape 6 — Tests

- tests unitaires de résolution sur trois niveaux d'héritage ;
- tests des branches Dark/Light et de toutes les variantes standards ;
- tests de rejet des cycles et doublons ;
- snapshots du contenu de la balise `<style>` générée par `FbTheme` ;
- tests bUnit des classes posées par `FbVariant`, et des règles écrites par la feuille et par `FbTheme` ;
- tests navigateur des valeurs calculées pour les scopes imbriqués ;
- tests de hover, focus et retour à la variante `Default` ;
- tests Layout seul, sans chargement de Forms.

## Critères d'acceptation

L'implémentation est terminée lorsque :

- un thème dérivé peut ne redéfinir qu'une seule propriété terminale ;
- cette propriété est héritée correctement sur les autres branches et variantes ;
- `Dark/Error` et `Light/Error` peuvent produire des palettes différentes ;
- changer de thème ou de branche régénère les mêmes noms de variables avec de nouvelles valeurs résolues ;
- aucune classe CSS ne contient le nom d’un thème ou d’une branche ; une variante n’apparaît que dans sa classe `fb-v-*` ;
- le bloc `<style>` contient un jeu complet `--fb-{variant}-*` pour chaque variante disponible ;
- une règle de variante ne contient que des redirections `--fb-* : var(--fb-{variant}-*)` ;
- une variante imbriquée `Default` annule complètement une variante extérieure ;
- hover et focus ne modifient que le neutre courant ;
- les calculs `color-mix()` existants produisent toujours les fonds, textes et frames ;
- tous les composants consomment les variables actives, sans dépendre du nom du thème ;
- Layout fonctionne sans référence à Forms ;
- le contenu du `<style>` est déterministe et ne contient aucune valeur non résolue ;
- aucun fallback CSS autoréférencé n'est généré ;
- aucun membre `[Obsolete]`, alias historique, façade ou variable CSS de compatibilité ne subsiste.
