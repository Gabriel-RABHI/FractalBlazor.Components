# Plan d'implémentation du theming

## Objectif

Le theming doit permettre de définir un thème complet une seule fois, de le décliner en branches comme `Dark` et `Light`, puis d'activer localement des variantes comme `Error`, `Success` ou `Disabled`.

L'implémentation doit conserver le principe déjà employé par les feuilles CSS :

1. des variables fixes définissent le socle du thème actif ;
2. des variables actives peuvent être redéfinies par une variante locale ;
3. des variables temporaires portent les modifications d'interaction, comme le survol ;
4. des variables calculées produisent les couleurs finales avec `color-mix()`.

L'héritage entre thèmes est résolu en C#. La cascade CSS ne sert qu'à appliquer un thème ou une variante dans le DOM et à gérer les états temporaires.

## Répartition des responsabilités

Le système reste réparti entre deux assemblies.

### `FractalBlazor.Components.Layout`

Layout ne connaît que les notions nécessaires à la mise en page :

- espacements ;
- rayons ;
- couleurs de fond ;
- frames, bordures et séparateurs ;
- génération de variables CSS ;
- thème et variantes utilisables lorsque Layout est employé seul.

### `FractalBlazor.Components.Forms`

Forms référence Layout et ajoute :

- couleurs de premier plan ;
- intensités de texte ;
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

public sealed class FbThemeLayoutBorders
{
    public string? LightMix { get; init; }
    public string? MediumMix { get; init; }
    public string? StrongMix { get; init; }
}

public sealed class FbThemeLayoutColors
{
    public string? LowAnchor { get; init; }
    public string? Tint { get; init; }
    public string? HighAnchor { get; init; }
    public string? SurfaceMix { get; init; }
    public string? AccentOffset { get; init; }
    public string? HighlightOffset { get; init; }
}
```

Les espacements S/M/L/X alimentent les margins, paddings et gutters. Les rayons S/M/L/X alimentent les `border-radius`. Les frames Light/Medium/Strong définissent leur force par leur taux de mélange ; leur épaisseur est fixée à `1px`.

### Groupes Forms

```csharp
public sealed class FbThemeFormTextVariant
{
    public string? DefaultHighMix { get; init; }
    public string? SubtleHighMix { get; init; }
    public string? MutedHighMix { get; init; }
    public string? HighlightHighMix { get; init; }
}

public sealed class FbThemeFormColors
{
    public string? LowAnchor { get; init; }
    public string? HighAnchor { get; init; }
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

Les tailles de texte ne constituent pas un groupe séparé : elles font partie de `FbThemeFormTypography`. Les six tailles sont calculées à partir de `FontSizeBase` et des coefficients XS/S/M/L/XL/XXL.

Toutes les propriétés sont nullable afin qu'un thème dérivé ne déclare que ses différences. `null` signifie « hériter ». Une chaîne vide ou composée d'espaces est invalide et doit être refusée par la validation.

Ces groupes sont des objets de données. Ils ne génèrent pas eux-mêmes du CSS : la génération est centralisée après résolution afin qu'une propriété C# ne puisse avoir qu'un seul mapping de variable CSS dans tout le système.

## Les trois couches du thème Forms

Un thème Forms possède trois couches strictes.

| Couche | Contenu | Portée |
| --- | --- | --- |
| Main | `Spacings`, `Corners`, `Typography` | Commun à toutes les branches et variantes du thème |
| Branch | `TextVariant` | Propre à une branche, par exemple `Dark` ou `Light` |
| Variant | `LayoutColors`, `FormColors`, `Borders` | Propre à un état visuel local |

La couche Main ne contient pas les couleurs. Une branche choisit les intensités de texte adaptées à son contraste. Chaque variante fournit ensuite les ancres de fond, les ancres de premier plan et les frames correspondant à cette branche.

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
    public FbThemeFormTextVariant? TextVariant { get; init; }

    public IReadOnlyList<FbThemeVariant> Variants { get; init; }
        = Array.Empty<FbThemeVariant>();
}

public sealed class FbThemeVariant
{
    public FbThemeVariant(string name) => Name = name;

    public string Name { get; }

    // Variant
    public FbThemeLayoutColors? LayoutColors { get; init; }
    public FbThemeFormColors? FormColors { get; init; }
    public FbThemeLayoutBorders? Borders { get; init; }
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
- thème impossible à résoudre jusqu'à une racine complète.

## Registre de thèmes

Le registre est un singleton injecté par DI :

```csharp
public interface IFbThemeRegistry
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
                new FbThemeVariant(FbThemeVariants.Error)
                {
                    LayoutColors = new FbThemeLayoutColors
                    {
                        LowAnchor = "#fff4f5",
                        Tint = "#ffd9dd",
                        HighAnchor = "#701824"
                    },
                    FormColors = new FbThemeFormColors
                    {
                        LowAnchor = "#701824",
                        HighAnchor = "#ffffff"
                    }
                }
            ]
        }
    ]
};
```

Les mixes et les frames non renseignés sont hérités de `Default/Light/Error`.

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
--fb-default-bg-low-anchor: #111113;
--fb-default-bg-tint: #34343a;
--fb-default-bg-high-anchor: #f7f7f8;
--fb-default-bg-surface-mix: 8%;
--fb-default-bg-accent-offset: 10%;
--fb-default-bg-highlight-offset: 18%;

--fb-error-bg-low-anchor: #2a0f14;
--fb-error-bg-tint: #4a1820;
--fb-error-bg-high-anchor: #fff5f6;
--fb-error-bg-surface-mix: 10%;
--fb-error-bg-accent-offset: 12%;
--fb-error-bg-highlight-offset: 22%;
```

Le même principe s'applique aux couleurs de premier plan et aux frames :

```css
--fb-default-fg-low-anchor
--fb-default-fg-high-anchor
--fb-default-frame-light-mix

--fb-error-fg-low-anchor
--fb-error-fg-high-anchor
--fb-error-frame-light-mix
```

Ces variables sont des valeurs sources. Elles ne sont jamais consommées directement par les classes CSS des composants.

Les paramètres Main et Branch, qui ne changent pas avec la variante, sont également émis dans le bloc du thème. Ils conservent leur préfixe `default` lorsqu'un alias actif est nécessaire :

```css
--fb-default-space-s
--fb-default-space-m
--fb-default-space-l
--fb-default-space-x

--fb-default-radius-s
--fb-default-radius-m
--fb-default-radius-l
--fb-default-radius-x

--fb-default-fg-default-high-mix
--fb-default-fg-subtle-high-mix
--fb-default-fg-muted-high-mix
--fb-default-fg-highlight-high-mix
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

--fb-bg-low-anchor: var(--fb-default-bg-low-anchor);
--fb-bg-tint: var(--fb-default-bg-tint);
--fb-bg-high-anchor: var(--fb-default-bg-high-anchor);
--fb-bg-surface-mix: var(--fb-default-bg-surface-mix);
--fb-bg-accent-offset: var(--fb-default-bg-accent-offset);
--fb-bg-highlight-offset: var(--fb-default-bg-highlight-offset);

--fb-fg-low-anchor: var(--fb-default-fg-low-anchor);
--fb-fg-high-anchor: var(--fb-default-fg-high-anchor);
--fb-fg-default-high-mix: var(--fb-default-fg-default-high-mix);
--fb-fg-subtle-high-mix: var(--fb-default-fg-subtle-high-mix);
--fb-fg-muted-high-mix: var(--fb-default-fg-muted-high-mix);
--fb-fg-highlight-high-mix: var(--fb-default-fg-highlight-high-mix);

--fb-frame-light-mix: var(--fb-default-frame-light-mix);
--fb-frame-medium-mix: var(--fb-default-frame-medium-mix);
--fb-frame-strong-mix: var(--fb-default-frame-strong-mix);
```

Une variante ne change jamais les classes CSS. Elle redéfinit localement les mêmes variables actives pour les faire pointer vers un autre préfixe :

```css
--fb-bg-low-anchor: var(--fb-error-bg-low-anchor);
--fb-bg-tint: var(--fb-error-bg-tint);
--fb-bg-high-anchor: var(--fb-error-bg-high-anchor);
--fb-bg-surface-mix: var(--fb-error-bg-surface-mix);
--fb-bg-accent-offset: var(--fb-error-bg-accent-offset);
--fb-bg-highlight-offset: var(--fb-error-bg-highlight-offset);

--fb-fg-low-anchor: var(--fb-error-fg-low-anchor);
--fb-fg-high-anchor: var(--fb-error-fg-high-anchor);

--fb-frame-light-mix: var(--fb-error-frame-light-mix);
--fb-frame-medium-mix: var(--fb-error-frame-medium-mix);
--fb-frame-strong-mix: var(--fb-error-frame-strong-mix);
```

Les valeurs sources de chaque variante sont complètes après résolution C#. Les redirections ne nécessitent donc ni valeur en dur, ni fallback, ni calcul de thème dans `FbVariant`.

### 3. Variables locales temporaires

La classe statique `.fb-theme-scope` réinitialise les variables temporaires à partir des variables actives :

```css
.fb-theme-scope {
    --fb-current-bg-surface-mix: var(--fb-bg-surface-mix);
    --fb-current-bg-accent-offset: var(--fb-bg-accent-offset);
    --fb-current-bg-highlight-offset: var(--fb-bg-highlight-offset);

    --fb-current-frame-light-mix: var(--fb-frame-light-mix);
    --fb-current-frame-medium-mix: var(--fb-frame-medium-mix);
    --fb-current-frame-strong-mix: var(--fb-frame-strong-mix);

    --fb-current-fg-default-high-mix: var(--fb-fg-default-high-mix);
    --fb-current-fg-subtle-high-mix: var(--fb-fg-subtle-high-mix);
    --fb-current-fg-muted-high-mix: var(--fb-fg-muted-high-mix);
    --fb-current-fg-highlight-high-mix: var(--fb-fg-highlight-high-mix);
}
```

Le hover vient par-dessus la variante : il modifie seulement les variables `--fb-current-*`, à partir des variables actives déjà redirigées. Un offset peut ainsi être ajouté avec `calc()` puis borné par `clamp(0%, ..., 100%)` sans connaître la variante courante.

La même règle s'applique à `:focus-visible`, à l'état pressé et aux autres interactions temporaires.

### 4. Variables calculées

Les formules existantes en `oklab` restent statiques et relatives aux variables actives ou temporaires :

```css
--fb-bg-surface: color-mix(
    in oklab,
    var(--fb-bg-low-anchor),
    var(--fb-bg-tint) var(--fb-current-bg-surface-mix)
);

--fb-bg-accent: color-mix(
    in oklab,
    var(--fb-bg-surface),
    var(--fb-bg-high-anchor) var(--fb-current-bg-accent-offset)
);

--fb-bg-highlight: color-mix(
    in oklab,
    var(--fb-bg-surface),
    var(--fb-bg-high-anchor) var(--fb-current-bg-highlight-offset)
);

--fb-fg-default: color-mix(
    in oklab,
    var(--fb-fg-low-anchor),
    var(--fb-fg-high-anchor) var(--fb-current-fg-default-high-mix)
);
```

`Subtle`, `Muted` et `Highlight` utilisent la même formule avec leur mix respectif.

Ces formules doivent être déclarées à la fois sur `:root` et sur `.fb-theme-scope`. Un scope de variante redéfinit les variables actives sur son propre élément ; les variables calculées doivent donc être redéclarées sur ce même élément pour être réévaluées à partir de ces nouvelles valeurs, et non simplement héritées du parent.

Les couleurs de frame restent calculées à partir de `--fb-current-bg`, de `--fb-bg-high-anchor` et du mix temporaire Light/Medium/Strong. Les tailles viennent des variables actives `--fb-frame-*-size`.

Cette indirection garantit quatre propriétés :

- le CSS et les classes des composants restent statiques ;
- l'activation d'une variante est uniquement une redirection de variables CSS ;
- le hover et les autres interactions se superposent à la variante active ;
- les valeurs calculées restent relatives aux ancres et aux mixes de la variante courante.

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

        --fb-default-bg-low-anchor: #f7f7f8;
        --fb-default-bg-tint: #d8dae0;
        --fb-default-bg-high-anchor: #111113;
        --fb-default-bg-surface-mix: 8%;
        --fb-default-bg-accent-offset: 8%;
        --fb-default-bg-highlight-offset: 16%;

        --fb-error-bg-low-anchor: #fff4f5;
        --fb-error-bg-tint: #ffd9dd;
        --fb-error-bg-high-anchor: #701824;
        --fb-error-bg-surface-mix: 10%;
        --fb-error-bg-accent-offset: 12%;
        --fb-error-bg-highlight-offset: 22%;

        --fb-error-fg-low-anchor: #701824;
        --fb-error-fg-high-anchor: #ffffff;
        --fb-error-frame-light-mix: 14%;

        --fb-bg-low-anchor: var(--fb-default-bg-low-anchor);
        --fb-bg-tint: var(--fb-default-bg-tint);
        --fb-bg-high-anchor: var(--fb-default-bg-high-anchor);
        --fb-bg-surface-mix: var(--fb-default-bg-surface-mix);
        --fb-bg-accent-offset: var(--fb-default-bg-accent-offset);
        --fb-bg-highlight-offset: var(--fb-default-bg-highlight-offset);

        --fb-fg-low-anchor: var(--fb-default-fg-low-anchor);
        --fb-fg-high-anchor: var(--fb-default-fg-high-anchor);
        --fb-frame-light-mix: var(--fb-default-frame-light-mix);
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
ValueName("Error", "bg-low-anchor");
// --fb-error-bg-low-anchor

ActiveReference("Error", "bg-low-anchor");
// --fb-bg-low-anchor:var(--fb-error-bg-low-anchor);
```

La liste des tokens Variant est centralisée et partagée par le générateur du thème et par `FbVariant`. Ainsi, une propriété ajoutée à `FbThemeLayoutColors`, `FbThemeFormColors` ou `FbThemeLayoutBorders` est nécessairement définie dans le `<style>` et redirigée dans le scope local.

La liste initiale est :

```text
bg-low-anchor
bg-tint
bg-high-anchor
bg-surface-mix
bg-accent-offset
bg-highlight-offset
fg-low-anchor
fg-high-anchor
frame-light-mix
frame-medium-mix
frame-strong-mix
```

Les filets ont une épaisseur fixe de `1px`. Leur force visuelle dépend uniquement des tokens `frame-*-mix`.

Les composants dérivés de `FbLayoutSurfaceComponentBase` exposent le booléen `WOl` pour ajouter un outline utilisant la couleur calculée de la frame courante :

```css
outline: var(--fb-outline-size) solid var(--fb-frame-border-color);
outline-offset: 0;
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
4. rend ensuite son `ChildContent` ;
5. peut fournir en cascade la liste des variantes disponibles pour la validation des composants descendants.

Il ne génère aucune classe CSS et n'ajoute pas le nom du thème au DOM. Son `Selector` vaut `:root` par défaut.

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
2. peut vérifier, via le contexte fourni par `FbTheme`, que la variante existe ;
3. génère par simple concaténation les redirections des tokens Variant ;
4. rend un `<div class="fb-theme-scope">` visuellement neutre ;
5. place les redirections dans l'attribut `style` de cette div ;
6. rend son `ChildContent` dans ce scope.

Pour `Error`, le rendu est de la forme :

```html
<div class="fb-theme-scope"
     style="--fb-bg-low-anchor:var(--fb-error-bg-low-anchor);
            --fb-bg-tint:var(--fb-error-bg-tint);
            --fb-bg-high-anchor:var(--fb-error-bg-high-anchor);
            --fb-fg-low-anchor:var(--fb-error-fg-low-anchor);
            --fb-fg-high-anchor:var(--fb-error-fg-high-anchor);
            --fb-frame-light-mix:var(--fb-error-frame-light-mix);">
    ...
</div>
```

Le rendu réel contient la redirection de tous les tokens de `LayoutColors`, `FormColors` et `Borders`. Il ne contient aucune valeur calculée en C# et aucune classe propre à `Error`.

Une erreur de thème, de branche ou de variante doit produire une exception explicite en développement. Une stratégie de fallback vers `Default` peut être configurable pour la production, mais ne doit pas masquer silencieusement une faute de configuration pendant le développement.

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

    public IReadOnlyList<FbLayoutThemeVariant> Variants { get; init; }
        = Array.Empty<FbLayoutThemeVariant>();
}

public sealed class FbLayoutThemeVariant
{
    public FbLayoutThemeVariant(string name) => Name = name;

    public string Name { get; }
    public FbThemeLayoutColors? LayoutColors { get; init; }
    public FbThemeLayoutBorders? Borders { get; init; }
}
```

La résolution suit les mêmes règles que le thème Forms, sans niveau Branch. `FbLayoutTheme` génère une balise `<style>` contenant la variante `Default`, toutes les variantes Layout résolues et les alias actifs. `FbLayoutVariant` rend une div neutre dont l'attribut `style` redirige les variables actives de fond et de frame vers le préfixe choisi.

Aucune classe propre à un thème ou à une variante Layout n'est générée.

## Adaptation des feuilles CSS

L'implémentation doit réorganiser les déclarations existantes sans réécrire les formules de rendu.

Les règles à suivre sont :

- conserver `color-mix(in oklab, ...)` pour les fonds, textes et frames ;
- faire consommer aux classes statiques uniquement les variables actives et calculées ;
- employer systématiquement `--fb-bg-low-anchor` et `--fb-fg-low-anchor` pour les ancres basses actives ;
- conserver les variables `--fb-current-*` pour tous les états temporaires ;
- compléter les scopes afin qu'ils réinitialisent à la fois fond, texte et frame ;
- faire consommer à chaque taille son coefficient XS/S/M/L/XL/XXL ;
- ajouter les variables de familles de fontes au lieu de coder `text-font` et `code-font` directement dans les règles ;
- retirer toute logique de thème ou de variante des classes CSS statiques ;
- garantir que le bloc `<style>` définit aussi `Tint`, `SurfaceMix` et les mixes de frame pour chaque variante ;
- garantir que `FbVariant` redirige exactement la même liste de tokens que celle émise par `FbTheme` ;
- centraliser la création des noms de variables pour que Layout, Forms, `FbTheme` et `FbVariant` produisent exactement la même nomenclature.

La classe statique actuellement nommée `FbTheme` est supprimée ; ce nom est ensuite attribué au composant `<FbTheme>`. Les anciens helpers de palettes, alias de propriétés et variantes orthographiées incorrectement ne sont pas conservés.

Les composants de configuration séparés du socle sont remplacés par la nouvelle architecture :

- `FbLayoutThemeStyle` est supprimé au profit du bloc `<style>` produit directement par `FbLayoutTheme` ;
- les réglages de couleurs Forms alimentent `FbThemeFormColors` et `FbThemeFormTextVariant` ;
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
- centraliser la liste des tokens qu'un `FbVariant` doit rediriger ;
- produire les redirections par concaténation de chaînes, sans recalculer le thème ;
- conserver les niveaux résolu, actif, temporaire et calculé ;
- garantir un ordre de sortie déterministe pour les tests et le diagnostic.

### Étape 4 — Composants Blazor

- implémenter `FbTheme` et la génération de sa balise `<style>` ;
- implémenter `FbVariant` avec une div `.fb-theme-scope` et des redirections inline ;
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
- tests bUnit des redirections inline produites par `FbVariant` ;
- tests navigateur des valeurs calculées pour les scopes imbriqués ;
- tests de hover, focus et retour à la variante `Default` ;
- tests Layout seul, sans chargement de Forms.

## Critères d'acceptation

L'implémentation est terminée lorsque :

- un thème dérivé peut ne redéfinir qu'une seule propriété terminale ;
- cette propriété est héritée correctement sur les autres branches et variantes ;
- `Dark/Error` et `Light/Error` peuvent produire des palettes différentes ;
- changer de thème ou de branche régénère les mêmes noms de variables avec de nouvelles valeurs résolues ;
- aucune classe CSS ne contient le nom d'un thème, d'une branche ou d'une variante ;
- le bloc `<style>` contient un jeu complet `--fb-{variant}-*` pour chaque variante disponible ;
- `FbVariant` ne contient que des redirections `--fb-* : var(--fb-{variant}-*)` ;
- une variante imbriquée `Default` annule complètement une variante extérieure ;
- hover et focus modifient uniquement les variables temporaires ;
- les calculs `color-mix()` existants produisent toujours les fonds, textes et frames ;
- tous les composants consomment les variables actives, sans dépendre du nom du thème ;
- Layout fonctionne sans référence à Forms ;
- le contenu du `<style>` est déterministe et ne contient aucune valeur non résolue ;
- aucun fallback CSS autoréférencé n'est généré ;
- aucun membre `[Obsolete]`, alias historique, façade ou variable CSS de compatibilité ne subsiste.
