Je développe une librairie de composants pour Blazor. Elle contient à la fois des composants de Layout (Row, Stack, Column, Sparator, Spacer, etc) et des composants d'édition (Sellect, Button, StringField, DatePicker, etc).

Sur le plan du design, je veux pouvoir limiter la complexité de la mise en place. Les composants vraiment centraux sont Row et Stack. On peut leur attribuer des fonds, des border et un radius. La partie spacing, radius sont définis globalement sous la forme d'une déclinaison S, M, L, X en terme de taille.

Pour la gestion des couleurs, j'aimerais un maximum de flexibilité. L'idée est que passer d'un theme Dark à Light, se fasse pas un simple booléen. L'idée est donc de baser les variantes de couleurs sur une couleur de fond (Back Color) et une couleur de devant (Fore Color). Tout ce qui est Row et Stack utilise la couleur de fond, tout ce qui est texte, icon, utilise la couleurs Fore. Mais je veux aussi qu'on puisse définit des teintes : de couleur de fond, de couleur de devant. Surtout, je voudrais que du point de vue des composants, ces notions ne soient pas explicitement définies : un thème est défini globalement ou localement, et les composants employés ont un nombre limitées d'options.

Les Row et les Stack (exemple pour un theme Dark) :
- Font transparent par défaut, sinon `Surface` (fonde de la page), `Accent` (un peu plus clair), `Highlight` (encore plus clair).
- Frame (border et séparateurs) : `None`, `Light` (très proche du background du composant), `Medium` (un peu plus clair), `Strong` (nettement plus clair).
Pour tout ce qui est Front :
- `Default`, `Shadow` (très sombre), `Mute` (sombre), `Select` (plus clair que Default).

L'idée est d'utiliser des variable css pour faire tout ceci.

Imaginons une ligne, qui va de Back Color (BC) jusqu'à Front Color (FC) :

Pour le fond (S=Surface, A=Accent et H=Highlight) :

BC-----S-----A----H--------------------------------------------------FC

Ces valeurs sont un Mix en % entre BC et FC. Les Frame serait un offset de ce Mix, en % :
Light = +6%, Medium = +12% et Strong = +18%

Pour le Fore Color (S = Shadom, M = Mute, D = default et SL = Select) :

BC--------------------------------------S------------M-------D-----SL-FC

Dans ce système, si BC = Black et FC = White, alors toutes les couleurs sont des valeur de gris.

L'idée est donc de séparer les couleurs du dégradé. Pour le Back on aurait deux couleurs : Absolute Back Color (ABC) et Base Back Color (BBC) :

ABC-----S-----A----H----------------BBC.............................................

Pour le Front on aurait, Absolute Front Color (AFC) et Base Front Color (BFC)

.............................................NFC---------S------------M-------D-----SL-AFC

Ainsi, si on reste sur Black pour ABC et un bleue pour BBC, on obtient des fonds qui sont un dégradé de bleus. Si on reste sur White pour AFC, et que NFC est vert, on aura des textes et icones en dégradé de verts. Si on inverse juste ABC et AFC, on passe d'un thème sombre à un théme clair.

On aurait donc 4 couleurs de base, qu'on peut changer, pour coloriser le fond (rouge pour Error, vert pour Success), qu'on peut passer en thème clair ou sombre par simple inversion des couleurs ABC et AFC.

De ces 4 couleurs et les taux de mix de couleur, on peut donc déduire toutes les autres couleurs. Comme ce sont des variables CSS, on peut les redéfinir localement. Comme les composants ne connaissent que les variantes (Surface, Accent Highlight, Light, Medium, Strong, Shadow, Mute, Select), ils sont indépendant à la fois des couleurs et des taux de Mix qui les définissent.

Est-ce que ce système est correctement structuré ? Est-ce que c'est la meilleure manière de penser un système de couleur souple ?


-------

Oui, le système est **bien structuré**. Tu es sur une bonne direction : tu sépares les **couleurs sources**, les **couleurs dérivées**, puis les **rôles exposés aux composants**. C’est exactement l’esprit des design tokens : définir des valeurs réutilisables, aliasées et sémantiques plutôt que faire porter aux composants la connaissance des couleurs réelles. Le Design Tokens Community Group formalise d’ailleurs cette logique de tokens échangeables entre outils, même si ton implémentation peut rester pure CSS. ([designtokens.org][1])

La réponse courte serait : **oui, mais je renforcerais ton modèle avec des couples “background / on-background” et des garanties de contraste**.

## Ce qui est très bon dans ton approche

Ton idée principale est bonne : les composants ne devraient connaître que des intentions simples.

Par exemple :

```csharp
<Row Background="Surface" Frame="Medium">
<Stack Background="Accent" Frame="Light">
<Button Tone="Success" Variant="Filled">
```

Et non :

```csharp
<Row BackgroundColor="#171b24" BorderColor="#2f3644">
```

Ça garde l’API propre, stable et thémable.

La cascade CSS est aussi un bon choix : les custom properties sont faites pour être redéfinies localement et participer à la cascade, ce qui colle très bien à l’idée d’un thème global ou local. ([MDN Web Docs][2])

## Le point à ajuster : ne pense pas seulement “dégradé”, pense “rôles appairés”

Ton système repose sur deux rampes :

```text
Back ramp:  ABC ---- Surface ---- Accent ---- Highlight ---- BBC

Fore ramp:  BFC ---- Shadow ---- Mute ---- Default ---- Select ---- AFC
```

C’est élégant. Mais il y a un piège : **une couleur de texte n’existe jamais seule**. Elle doit toujours être pensée par rapport au fond sur lequel elle s’affiche.

Donc je garderais ton modèle, mais je le formulerais ainsi :

```text
Palette primitive
- back.absolute
- back.base
- fore.base
- fore.absolute

Tokens dérivés
- bg.surface
- bg.accent
- bg.highlight
- fg.default
- fg.muted
- fg.subtle
- fg.selected
- frame.light
- frame.medium
- frame.strong

Tokens de couple
- on.surface.default
- on.surface.muted
- on.accent.default
- on.highlight.default
```

Même si dans un premier temps `on.surface.default`, `on.accent.default` et `on.highlight.default` pointent tous vers `fg.default`, le fait de prévoir cette couche te protège pour la suite.

Pourquoi ? Parce qu’un texte `Default` qui passe très bien sur `Surface` peut devenir limite sur `Highlight`, surtout si l’utilisateur redéfinit localement le thème avec une couleur Success, Error ou Warning. Les pourcentages de mix donnent une cohérence visuelle, mais ne garantissent pas automatiquement le contraste. WCAG demande notamment 4.5:1 pour le texte normal et 3:1 pour le grand texte, et 3:1 pour les informations non textuelles comme les contours de composants ou indicateurs d’état. ([W3C][3])

## Je changerais légèrement les noms

Tes noms ABC, BBC, BFC, AFC sont logiques techniquement, mais je les trouve un peu difficiles à manipuler mentalement. Je proposerais plutôt :

```css
--ui-bg-anchor: ...;   /* ex: noir en dark, blanc en light */
--ui-bg-tint: ...;     /* ex: bleu, rouge, vert, neutre */
--ui-fg-tint: ...;     /* ex: vert, bleu, neutre */
--ui-fg-anchor: ...;   /* ex: blanc en dark, noir en light */
```

Ou encore :

```css
--ui-back-0
--ui-back-100
--ui-fore-0
--ui-fore-100
```

Le mot “absolute” peut devenir ambigu, car une couleur “absolue” est en réalité un **point d’ancrage thématique**. En dark, `bg-anchor` peut être presque noir ; en light, presque blanc.

## Pour le passage Dark / Light

L’idée du booléen est bonne, mais je ne ferais pas forcément une inversion purement mécanique de `ABC` et `AFC`.

Je ferais plutôt :

```css
:root[data-theme="dark"] {
  --ui-bg-anchor: oklch(12% 0.02 250);
  --ui-bg-tint:   oklch(28% 0.08 250);

  --ui-fg-tint:   oklch(72% 0.08 145);
  --ui-fg-anchor: oklch(98% 0 0);
}

:root[data-theme="light"] {
  --ui-bg-anchor: oklch(98% 0 0);
  --ui-bg-tint:   oklch(88% 0.05 250);

  --ui-fg-tint:   oklch(34% 0.08 145);
  --ui-fg-anchor: oklch(12% 0.02 250);
}
```

Donc l’API reste bien un booléen ou un `data-theme`, mais les valeurs peuvent être ajustées séparément. Un bleu qui marche bien sur fond noir n’est pas toujours le bon bleu sur fond blanc. Ton système doit permettre l’inversion simple, mais ne pas l’imposer comme seule stratégie.

## Utilise `color-mix()` en `oklch` ou `oklab`

Ton idée de calculer les couleurs par pourcentage est bonne avec CSS moderne. `color-mix()` permet de mélanger des couleurs dans un espace colorimétrique donné, et MDN indique que la fonction est largement disponible depuis 2023. ([MDN Web Docs][4])

Je choisirais plutôt `oklch` ou `oklab` que `srgb`, car ces espaces sont plus adaptés à des progressions visuellement régulières. MDN indique notamment qu’Oklab cherche à refléter la perception humaine et qu’il est utile pour des gradients plus uniformes. ([MDN Web Docs][5])

Exemple de structure :

```css
:root {
  --ui-bg-surface-mix: 18%;
  --ui-bg-accent-mix: 28%;
  --ui-bg-highlight-mix: 38%;

  --ui-fg-shadow-mix: 55%;
  --ui-fg-muted-mix: 70%;
  --ui-fg-default-mix: 86%;
  --ui-fg-select-mix: 94%;

  --ui-bg-surface: color-mix(
    in oklch,
    var(--ui-bg-anchor) calc(100% - var(--ui-bg-surface-mix)),
    var(--ui-bg-tint) var(--ui-bg-surface-mix)
  );

  --ui-bg-accent: color-mix(
    in oklch,
    var(--ui-bg-anchor) calc(100% - var(--ui-bg-accent-mix)),
    var(--ui-bg-tint) var(--ui-bg-accent-mix)
  );

  --ui-bg-highlight: color-mix(
    in oklch,
    var(--ui-bg-anchor) calc(100% - var(--ui-bg-highlight-mix)),
    var(--ui-bg-tint) var(--ui-bg-highlight-mix)
  );

  --ui-fg-default: color-mix(
    in oklch,
    var(--ui-fg-tint) calc(100% - var(--ui-fg-default-mix)),
    var(--ui-fg-anchor) var(--ui-fg-default-mix)
  );
}
```

Pour les frames, je les calculerais plutôt **depuis le fond courant**, pas uniquement depuis un pourcentage abstrait :

```css
.ui-surface {
  --ui-current-bg: var(--ui-bg-surface);
  background: var(--ui-current-bg);
}

.ui-accent {
  --ui-current-bg: var(--ui-bg-accent);
  background: var(--ui-current-bg);
}

.ui-frame-light {
  border-color: color-mix(
    in oklch,
    var(--ui-current-bg) 94%,
    var(--ui-fg-anchor) 6%
  );
}

.ui-frame-medium {
  border-color: color-mix(
    in oklch,
    var(--ui-current-bg) 88%,
    var(--ui-fg-anchor) 12%
  );
}

.ui-frame-strong {
  border-color: color-mix(
    in oklch,
    var(--ui-current-bg) 82%,
    var(--ui-fg-anchor) 18%
  );
}
```

Ça rend la frame naturellement relative au fond réel du composant.

## Je ne mettrais pas `Transparent` dans la rampe

`Transparent` n’est pas vraiment une variante de couleur. C’est plutôt une stratégie de rendu : “ne pas peindre le fond”.

Donc je garderais :

```csharp
Background = None | Surface | Accent | Highlight
```

Plutôt que :

```csharp
Background = Transparent | Surface | Accent | Highlight
```

La différence est subtile, mais importante : un composant transparent doit hériter du contexte visuel parent, tandis qu’un composant `Surface` pose explicitement une surface.

## Pour les composants d’édition, ajoute une couche “control”

Row et Stack peuvent consommer directement `bg.surface`, `bg.accent`, `frame.medium`, etc.

Mais Button, Select, DatePicker, StringField auront besoin de rôles plus spécifiques :

```text
control.bg
control.bg.hover
control.bg.active
control.bg.disabled
control.border
control.border.hover
control.border.focus
control.fg
control.placeholder
control.caret
control.selection
```

Ces tokens peuvent être dérivés de ton système général, mais je ne les exposerais pas directement comme `Surface`, `Accent`, `Highlight` dans tous les cas. Un bouton “filled”, un champ texte, une bordure de focus et un placeholder n’ont pas exactement les mêmes contraintes.

## Ma recommandation finale

Ton système est une très bonne base, mais je le formaliserais en 4 niveaux :

```text
1. Primitive palette
   bg-anchor, bg-tint, fg-tint, fg-anchor

2. Ramp tokens
   bg.surface, bg.accent, bg.highlight
   fg.subtle, fg.muted, fg.default, fg.selected
   frame.light, frame.medium, frame.strong

3. Semantic / component tokens
   container.bg
   container.fg
   control.bg
   control.border
   control.focus
   button.primary.bg
   button.primary.fg
   status.error.bg
   status.error.fg

4. Component API
   Background="Surface"
   Frame="Medium"
   Tone="Success"
   Variant="Filled"
```

Donc oui : **c’est correctement pensé**. Ce n’est pas seulement un système de couleurs, c’est déjà un système de **tokens contextuels dérivés**. La seule chose que je ne laisserais pas implicite, c’est le couple fond/texte : prévois des tokens `on-*` ou des tests de contraste, sinon certains thèmes locaux finiront par produire des combinaisons belles mais peu lisibles.

[1]: https://www.designtokens.org/TR/2025.10/format/ "Design Tokens Format Module 2025.10"
[2]: https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Properties/--%2A?utm_source=chatgpt.com "Custom properties (--*): CSS variables - MDN Web Docs"
[3]: https://www.w3.org/TR/WCAG22/ "Web Content Accessibility Guidelines (WCAG) 2.2"
[4]: https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Values/color_value/color-mix "color-mix() CSS function - CSS | MDN"
[5]: https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Values/color_value/oklab "oklab() CSS function - CSS | MDN"


------

Très bien. Mes composants sont configurés via des paramètres booléens :

```
<FbRow GL WFL PL L1>
    <FbText XS T>Extra Small</FbText>
    <FbText S T>Small</FbText>
    <FbText M T>Medium</FbText>
    <FbText L T>Large</FbText>
    <FbText XL T>Extra Large</FbText>
    <FbText XXL T>Extra Extra Large</FbText>
</FbRow>
```

GL = Gutter Large
WFL = With Frame Light
PL = Padding Large
L1 = Accent

S = small .... T = Thin / M = Medium / B = Bold / XB = Extra Bold

Donc, pour mes Row et Stack :

WBS = With Background Surface
WBA = With Background Accent
WBH = With Background Highlight

Pour les foreground :

CS = Color Subtle
CM = Color Muted
CD = Color Default
CH = Color Highlight (Select est une sémantique fonctionnelle, ici on est dans le rendu)

L'idée, c'est que les composants font appel à cette sémantique, mais que celle-ci ne soit pas trop restrictive. On distingue la sémantique du rendu de la sémentique fonctionnelle.

Le thème au niveau Layout n'a pas à supporter plus que les BackGround, Frames et éventuellement les Foreground colors. Au niveau Form, on doit supporter les Foreground complètement. Donc, faire descendre les Foreground au niveau layout, c'est peut être logique sur un plan technique, mais pas sur un plan logique. L'assembly Lyout doit pouvoir être utilisé avec n'importe quelle librairie de composants : MudBlazor, Radzen. En revanche, Forms doit implémenter la sémantique Foreground : Subtle, Muted, Default, Select.

Egalement, le thème doit implémenter plusieurs autres sémantiques fonctionnelles, qui sont des "pointeurs" vers une sémantique de rendu :
- Focused
- Disabled
- Selected
- Error

Voici un example de composant :

```
public enum FbTextSize : byte
{
    None,
    XS,
    S,
    M,
    L,
    XL,
    XXL
}

public enum FbTextWeight : byte
{
    None,
    T,
    B,
    XB
}

public enum FbTextIntent : byte
{
    Default,
    /// <summary>
    /// Really dark.
    /// </summary>
    Shadow,
    /// <summary>
    /// Darker.
    /// </summary>
    Mute,
    /// <summary>
    /// Lighter
    /// </summary>
    Accent
}

public enum FbTextModifiers : byte
{
    NoWrap,
    Trim
}

public class FbText : FbComponentBase
{
    private static string[] FbTextSizeClasses = { "", "fb-t-xs ", "fb-t-s ", "fb-t-m ", "fb-t-l ", "fb-t-xl ", "fb-t-xxl " };
    private static string[] FbTextWeightClasses = { "", "fb-t-t ", "fb-t-b ", "fb-t-xb " };
    private static string[] FbTextIntentClasses = { "", "fb-t-shadow ", "fb-t-mute ", "fb-t-acc ", "fb-t-pri ", "fb-t-err " };
    private static string[] FbTextModifiersClasses = { "", "fb-t-nw ", "fb-t-tr " };

    private RenderHandle _renderHandle;

    private FbTextSize _size;
    private FbTextWeight _weight;
    private FbTextIntent _intent;
    private FbTextModifiers _modifiers;

    // -------- Scale
    [Parameter]
    public bool XS { get => _size == FbTextSize.XS; set => _size = FbTextSize.XS; }

    [Parameter]
    public bool S { get => _size == FbTextSize.S; set => _size = FbTextSize.S; }

    [Parameter]
    public bool M { get => _size == FbTextSize.M; set => _size = FbTextSize.M; }

    [Parameter]
    public bool L { get => _size == FbTextSize.L; set => _size = FbTextSize.L; }

    [Parameter]
    public bool XL { get => _size == FbTextSize.XL; set => _size = FbTextSize.XL; }

    [Parameter]
    public bool XXL { get => _size == FbTextSize.XXL; set => _size = FbTextSize.XXL; }

    // -------- Weight
    [Parameter]
    public bool T { get => _weight == FbTextWeight.T; set => _weight = FbTextWeight.T; }

    [Parameter]
    public bool B { get => _weight == FbTextWeight.B; set => _weight = FbTextWeight.B; }

    [Parameter]
    public bool XB { get => _weight == FbTextWeight.XB; set => _weight = FbTextWeight.XB; }

    // -------- Intent
    [Parameter]
    public bool Shadow { get => _intent == FbTextIntent.Shadow; set => _intent = FbTextIntent.Shadow; }

    [Parameter]
    public bool Mute { get => _intent == FbTextIntent.Mute; set => _intent = FbTextIntent.Mute; }

    [Parameter]
    public bool Accent { get => _intent == FbTextIntent.Accent; set => _intent = FbTextIntent.Accent; }

    // -------- Modifiers
    [Parameter]
    public bool NW { get => _modifiers == FbTextModifiers.NoWrap; set => _modifiers = FbTextModifiers.NoWrap; }

    [Parameter]
    public bool TR { get => _modifiers == FbTextModifiers.Trim; set => _modifiers = FbTextModifiers.Trim; }

    // -------- Content
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    protected override void OnBeforeParametersSet()
    {
        _size = default;
        _weight = default;
        _intent = default;
        _modifiers = default;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");

        builder.AddAttribute(1, "class", BuildCssClass());

        if (Value != null)
            builder.AddContent(2, Value);
        else if (ChildContent != null)
            builder.AddContent(3, ChildContent);

        builder.CloseElement();
    }

    private string BuildCssClass()
    {
        string size = FbTextSizeClasses[(byte)_size];
        string weight = FbTextWeightClasses[(byte)_weight];
        string intent = FbTextIntentClasses[(byte)_intent];
        string mod = FbTextModifiersClasses[(byte)_modifiers];

        return $"fb-txt {size}{weight}{intent}{mod}".TrimEnd();
    }
}
```

L'autre idée, c'est que ces composants sont "ultra-light" : 

```
public abstract class FbComponentBase : IComponent, IHandleAfterRender
{
    protected RenderHandle _renderHandle;
    private bool _initialized;
    private bool _hasCalledOnAfterRender;

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }
```

C'est un composant "light" (et d'ailleurs, `FbComponentBase` ne devrait pas implémenter `IHandleAfterRender`). Le but est que ces composants soit beaucoup plus rapide en rendu et instanciation que ceux d'une librairie comme MudBlazor.

Encore un autre point : le changement de thème doit pouvoir se faire :
- par programmation 
- sur une branche du DOM

Donc, sur le plan du Theming, je voudrais avoir un objet thème :
- Pour le Layout seul (les fonds seulement), dans l'assembly Layout
- Pour le "fore color" dans l'assembly Forms, qui hérite de celui du Layout

Qu'en penses tu ?


-------

Oui, je pense que ta séparation est cohérente — et même assez saine — à condition de distinguer clairement **trois couches** :

```text
1. Sémantique de layout
   Surface, Accent, Highlight, Frame Light/Medium/Strong

2. Sémantique de rendu foreground
   Subtle, Muted, Default, Highlight

3. Sémantique fonctionnelle
   Focused, Disabled, Selected, Error, Success, Warning...
```

Et surtout : la couche 3 ne doit pas être une nouvelle palette parallèle. Elle doit être une couche d’**alias** qui pointe vers la couche 1 ou 2.

Par exemple :

```css
--fb-state-disabled-fg: var(--fb-fg-muted);
--fb-state-disabled-bg: transparent;
--fb-state-disabled-frame: var(--fb-frame-light);

--fb-state-selected-bg: var(--fb-bg-highlight);
--fb-state-selected-fg: var(--fb-fg-default);
--fb-state-selected-frame: var(--fb-frame-medium);

--fb-state-error-fg: var(--fb-error-fg-default);
--fb-state-error-bg: var(--fb-error-bg-surface);
--fb-state-error-frame: var(--fb-error-frame-medium);
```

Ça te permet de garder une API simple côté composants, tout en laissant le thème décider comment un état fonctionnel est rendu.

## Sur la séparation Layout / Forms

Ton intuition est bonne : **l’assembly Layout ne devrait pas porter toute la sémantique foreground**.

Je structurerais plutôt comme ceci :

```text
Fb.Layout
- spacing
- radius
- backgrounds
- frames
- composants : Row, Stack, Column, Spacer, Separator...

Fb.Forms
- dépend de Fb.Layout
- ajoute foreground
- ajoute states fonctionnels
- composants : Text, Button, Select, Field, DatePicker...
```

Donc :

```csharp
public class FbLayoutTheme
{
    public FbBackTheme Back { get; set; }
    public FbFrameTheme Frame { get; set; }
    public FbSpacingTheme Spacing { get; set; }
    public FbRadiusTheme Radius { get; set; }
}

public class FbFormTheme : FbLayoutTheme
{
    public FbForeTheme Fore { get; set; }
    public FbStateTheme States { get; set; }
}
```

Ou, encore mieux à long terme :

```csharp
public class FbFormTheme
{
    public FbLayoutTheme Layout { get; set; }
    public FbForeTheme Fore { get; set; }
    public FbStateTheme States { get; set; }
}
```

L’héritage est pratique, mais la composition est souvent plus claire pour générer des variables CSS.

## Le thème ne devrait pas être “lu” par chaque composant

Je ferais attention à un piège : si chaque composant Blazor reçoit le thème via `[CascadingParameter]`, tu risques de perdre une partie du bénéfice “ultra-light”. La documentation Blazor indique que les cascading parameters non fixes installent une souscription, ce qui est plus coûteux qu’un paramètre classique ; `IsFixed="true"` est recommandé quand la valeur ne change pas. Elle indique aussi que les composants répétés à grande échelle peuvent coûter en mémoire et CPU, et que limiter le nombre d’instances est souvent plus efficace que seulement les alléger. ([Microsoft Learn][1])

À ta place, je ferais donc :

```text
Objet Theme C# 
    ↓
génère des CSS variables
    ↓
scope DOM : :root, body, div, FbThemeScope
    ↓
les composants ne lisent pas le thème
    ↓
ils posent seulement des classes
```

Donc le composant ne fait pas :

```csharp
[CascadingParameter]
public FbTheme Theme { get; set; }
```

Il fait seulement :

```html
<div class="fb-row fb-bg-acc fb-fr-l fb-p-l">
```

Et le thème vit dans le CSS :

```css
.fb-theme-dark {
  --fb-bg-surface: ...;
  --fb-bg-accent: ...;
  --fb-bg-highlight: ...;
  --fb-frame-light: ...;
}
```

Les CSS custom properties sont naturellement adaptées à ça parce qu’elles participent à la cascade, sont héritées et sont scopées à l’élément sur lequel elles sont déclarées. ([MDN Web Docs][2])

## Pour le changement de thème global ou local

Ton besoin :

```text
- changement par programmation
- changement sur une branche du DOM
```

se traduit très bien par un composant de scope :

```razor
<FbThemeScope Theme="Dark">
    <FbRow WBA WFM PL>
        ...
    </FbRow>
</FbThemeScope>

<FbThemeScope Theme="Error">
    <FbStack WBS WFS>
        ...
    </FbStack>
</FbThemeScope>
```

Rendu possible :

```html
<div class="fb-theme fb-theme-dark">
    ...
</div>
```

Ou, pour un thème dynamique local :

```html
<div class="fb-theme" style="
  --fb-bg-anchor: ...;
  --fb-bg-tint: ...;
  --fb-frame-light: ...;
">
    ...
</div>
```

L’avantage : changer le thème d’une branche ne demande pas forcément de rerendre tous les composants enfants. Le navigateur recalcule le style CSS.

## Sur ton API booléenne

Je comprends très bien l’idée :

```razor
<FbRow GL WFL PL L1>
    <FbText S T>Small</FbText>
</FbRow>
```

C’est compact, lisible une fois qu’on connaît le DSL, et très efficace pour des composants de layout.

Mais il y a trois risques.

Le premier : **les conflits**.

```razor
<FbText S M L>
```

Qui gagne ? Le dernier ? L’ordre des paramètres ? L’ordre du compilateur ? Idéalement, tu ne devrais pas dépendre de ça.

Le deuxième : **les booléens conditionnels**.

Avec ton setter actuel :

```csharp
[Parameter]
public bool S { get => _size == FbTextSize.S; set => _size = FbTextSize.S; }
```

ceci pose problème :

```razor
<FbText S="@false">
```

Même si la valeur est `false`, le setter assigne quand même `S`.

Il faut plutôt écrire :

```csharp
[Parameter]
public bool S
{
    get => _size == FbTextSize.S;
    set { if (value) _size = FbTextSize.S; }
}
```

Le troisième : **les modifiers cumulables**.

Aujourd’hui :

```csharp
public enum FbTextModifiers : byte
{
    NoWrap,
    Trim
}
```

et :

```csharp
private FbTextModifiers _modifiers;
```

ne permettent qu’un seul modifier à la fois. Si tu veux autoriser :

```razor
<FbText NW TR>
```

alors il faudrait un enum flags :

```csharp
[Flags]
public enum FbTextModifiers : byte
{
    None = 0,
    NoWrap = 1,
    Trim = 2
}
```

Puis :

```csharp
[Parameter]
public bool NW
{
    get => _modifiers.HasFlag(FbTextModifiers.NoWrap);
    set { if (value) _modifiers |= FbTextModifiers.NoWrap; }
}

[Parameter]
public bool TR
{
    get => _modifiers.HasFlag(FbTextModifiers.Trim);
    set { if (value) _modifiers |= FbTextModifiers.Trim; }
}
```

## Je garderais les booléens, mais avec une API secondaire enum

Pour les usages très fréquents, les booléens sont bien :

```razor
<FbText S Mute T>
```

Mais je proposerais aussi une API explicite :

```razor
<FbText Size="FbTextSize.S"
        Weight="FbTextWeight.Thin"
        Color="FbForeColor.Muted" />
```

Pourquoi ? Parce que c’est plus pratique pour les cas dynamiques :

```razor
<FbText Size="@GetSize(item)"
        Color="@GetColor(item)" />
```

Donc tu peux avoir les deux :

```csharp
[Parameter]
public FbTextSize Size
{
    get => _size;
    set => _size = value;
}

[Parameter]
public bool S
{
    get => _size == FbTextSize.S;
    set { if (value) _size = FbTextSize.S; }
}
```

Ça garde le DSL compact sans enfermer l’utilisateur.

## Je renommerais certains concepts

Tu as déjà commencé à corriger le vocabulaire, et je pense que c’est important.

Pour le foreground, je préférerais :

```csharp
public enum FbForeColor : byte
{
    None,
    Subtle,
    Muted,
    Default,
    Highlight
}
```

plutôt que :

```csharp
Shadow,
Mute,
Accent
```

`Accent` peut être confondu avec le background `Accent`. Et `Shadow` décrit une perception, pas vraiment un rôle de rendu. `Subtle`, `Muted`, `Default`, `Highlight` sont plus réguliers.

Pour les états fonctionnels :

```csharp
public enum FbIntent : byte
{
    None,
    Disabled,
    Focused,
    Selected,
    Error,
    Success,
    Warning,
    Info
}
```

Mais je ne mélangerais pas ça avec `FbForeColor`.

Donc :

```razor
<FbText Muted>
<FbText Highlight>
<FbButton Selected>
<FbStringField Error>
```

`Muted` est une couleur de rendu.
`Selected` est un état fonctionnel.
`Error` est un état fonctionnel ou un tone fonctionnel.

## Pour Row et Stack

Je ferais quelque chose de ce genre :

```csharp
public enum FbBackColor : byte
{
    None,
    Surface,
    Accent,
    Highlight
}

public enum FbFrame : byte
{
    None,
    Light,
    Medium,
    Strong
}
```

Et côté raccourcis :

```razor
<FbRow WBS WFL PL>
<FbRow WBA WFM PM>
<FbRow WBH WFS PS>
```

Je trouve tes préfixes cohérents :

```text
WB* = With Background
WF* = With Frame
P*  = Padding
G*  = Gutter
```

Mais je garderais une convention stricte :

```text
WBS = With Background Surface
WBA = With Background Accent
WBH = With Background Highlight

WFL = With Frame Light
WFM = With Frame Medium
WFS = With Frame Strong
```

Pour éviter la collision entre `S = Small` et `S = Surface`, le préfixe est essentiel.

## Sur le composant “ultra-light”

L’idée est valide, mais je serais prudent sur la promesse “beaucoup plus rapide que MudBlazor”.

Oui, un composant qui n’hérite pas de `ComponentBase`, qui ne fait pas d’injection, pas d’événement, pas de cascading parameter, pas d’attribute splatting, et qui génère juste une balise + une classe peut être très léger.

Mais Blazor conserve malgré tout un coût par instance de composant. La documentation Microsoft donne un exemple où le coût observé était d’environ 0,06 ms par instance dans un test Blazor WebAssembly simple, et recommande parfois d’éviter des milliers d’instances plutôt que seulement les optimiser. ([Microsoft Learn][1])

Donc ta stratégie est bonne pour :

```text
- Text
- Row
- Stack
- Spacer
- Separator
- Icon simple
```

Mais pour des listes énormes, grids, cellules, etc., il faudra quand même mesurer.

La documentation indique aussi que `ComponentBase` déclenche des rerenders après mise à jour de paramètres, cascading parameters, événements et `StateHasChanged`. ([Microsoft Learn][3]) Si ton `FbComponentBase` court-circuite une partie de ça, tu peux gagner en overhead, mais tu dois être très strict sur les conventions que tu supportes.

## Point important : SetParametersAsync

Le gain réel viendra surtout de là.

La documentation indique que l’implémentation par défaut de `ComponentBase.SetParametersAsync` assigne les propriétés `[Parameter]` et `[CascadingParameter]` correspondant à la `ParameterView`. ([Microsoft Learn][4]) Elle indique aussi que, dans des cas extrêmes, implémenter manuellement `SetParametersAsync` peut éviter une partie de l’overhead d’écriture des paramètres, notamment quand il y a beaucoup de composants, beaucoup de paramètres, ou un impact observable sur la réactivité. ([Microsoft Learn][1])

Donc pour ton modèle, je viserais plutôt ce genre de base :

```csharp
public abstract class FbComponentBase : IComponent
{
    protected RenderHandle RenderHandle;

    public void Attach(RenderHandle renderHandle)
    {
        RenderHandle = renderHandle;
    }

    public abstract Task SetParametersAsync(ParameterView parameters);
}
```

Et dans un composant ultra-light critique, tu peux parser directement les paramètres :

```csharp
public override Task SetParametersAsync(ParameterView parameters)
{
    _size = default;
    _weight = default;
    _intent = default;
    _modifiers = default;
    Value = null;
    ChildContent = null;

    foreach (var parameter in parameters)
    {
        switch (parameter.Name)
        {
            case nameof(XS):
                if (parameter.Value is true) _size = FbTextSize.XS;
                break;

            case nameof(S):
                if (parameter.Value is true) _size = FbTextSize.S;
                break;

            case nameof(M):
                if (parameter.Value is true) _size = FbTextSize.M;
                break;

            case nameof(L):
                if (parameter.Value is true) _size = FbTextSize.L;
                break;

            case nameof(XL):
                if (parameter.Value is true) _size = FbTextSize.XL;
                break;

            case nameof(XXL):
                if (parameter.Value is true) _size = FbTextSize.XXL;
                break;

            case nameof(T):
                if (parameter.Value is true) _weight = FbTextWeight.T;
                break;

            case nameof(B):
                if (parameter.Value is true) _weight = FbTextWeight.B;
                break;

            case nameof(XB):
                if (parameter.Value is true) _weight = FbTextWeight.XB;
                break;

            case nameof(Value):
                Value = (string?)parameter.Value;
                break;

            case nameof(ChildContent):
                ChildContent = (RenderFragment?)parameter.Value;
                break;
        }
    }

    RenderHandle.Render(BuildRenderTree);
    return Task.CompletedTask;
}
```

Là, tu assumes complètement ton modèle : pas de lifecycle complet, pas d’after render, pas de reflection, pas de capture d’attributs arbitraires.

## Attention à ton tableau de classes

Dans ton exemple :

```csharp
private static string[] FbTextIntentClasses = 
{ 
    "", 
    "fb-t-shadow ", 
    "fb-t-mute ", 
    "fb-t-acc ", 
    "fb-t-pri ", 
    "fb-t-err " 
};
```

Mais ton enum ne contient que :

```csharp
Default,
Shadow,
Mute,
Accent
```

Donc ton tableau est déjà en avance sur ton enum. C’est le type de bug qui arrive vite avec ce système.

Je te conseille de normaliser :

```csharp
public enum FbTextColor : byte
{
    None = 0,
    Subtle = 1,
    Muted = 2,
    Default = 3,
    Highlight = 4
}
```

Puis :

```csharp
private static readonly string[] ColorClasses =
{
    "",
    "fb-fg-subtle ",
    "fb-fg-muted ",
    "fb-fg-default ",
    "fb-fg-highlight "
};
```

Et si `Default` doit être réellement le comportement par défaut, alors tu peux aussi décider que `None` et `Default` produisent la même chose côté CSS.

## La meilleure structure globale

Je verrais ton système comme ça :

```text
Fb.Layout

C#:
- FbLayoutTheme
- FbBackColor
- FbFrame
- FbSpacing
- FbRadius
- FbRow
- FbStack
- FbColumn
- FbSpacer
- FbSeparator

CSS:
- --fb-bg-surface
- --fb-bg-accent
- --fb-bg-highlight
- --fb-frame-light
- --fb-frame-medium
- --fb-frame-strong
- --fb-space-s/m/l/x
- --fb-radius-s/m/l/x
```

Puis :

```text
Fb.Forms

C#:
- FbFormTheme : ou compose FbLayoutTheme
- FbForeColor
- FbControlState
- FbText
- FbButton
- FbStringField
- FbSelect
- FbDatePicker

CSS:
- --fb-fg-subtle
- --fb-fg-muted
- --fb-fg-default
- --fb-fg-highlight
- --fb-state-focused-*
- --fb-state-disabled-*
- --fb-state-selected-*
- --fb-state-error-*
```

Et enfin :

```text
Fb.Theme ou Fb.Core, optionnel

- génération CSS
- helpers de thème
- conversion objet thème -> style string / stylesheet / class
```

## Mon avis final

Ton architecture est bonne, et ta séparation Layout / Forms est logique.

Je ferais simplement ces ajustements :

1. **Layout ne connaît que background, frame, spacing, radius.**
2. **Forms ajoute foreground et états fonctionnels.**
3. **Les états fonctionnels sont des alias vers des tokens de rendu.**
4. **Les composants ne lisent pas le thème : ils émettent seulement des classes.**
5. **Le thème est appliqué par CSS variables sur `:root` ou sur une branche DOM.**
6. **Les paramètres booléens sont acceptables, mais il faut gérer `false`, les conflits et les modifiers cumulables.**
7. **Pour les composants ultra-light, le vrai levier est `SetParametersAsync` manuel, pas seulement éviter `ComponentBase`.**

Donc oui : le modèle est très cohérent. Je dirais même que tu tiens une bonne ligne directrice : **Layout expose une grammaire visuelle minimale ; Forms enrichit cette grammaire avec la lecture, les contrôles et les états.**

[1]: https://learn.microsoft.com/en-us/aspnet/core/blazor/performance/rendering?view=aspnetcore-10.0 "ASP.NET Core Blazor rendering performance best practices | Microsoft Learn"
[2]: https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Properties/--%2A?utm_source=chatgpt.com "Custom properties (--*): CSS variables - MDN Web Docs"
[3]: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/rendering?view=aspnetcore-10.0 "ASP.NET Core Razor component rendering | Microsoft Learn"
[4]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.componentbase.setparametersasync?view=aspnetcore-10.0 "ComponentBase.SetParametersAsync(ParameterView) Method (Microsoft.AspNetCore.Components) | Microsoft Learn"
