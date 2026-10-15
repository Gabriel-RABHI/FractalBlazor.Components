# Plan de finalisation

## Objectif

Amener FractalBlazor.Components à un état publiable sans trahir ses principes. Chaque étape doit laisser la solution compilable, testée et cohérente avec `Conception.md` et `Theming.md`.

Ce plan est exécuté sur la branche `FinalizeLibrary`, une étape par commit. Les cases cochées indiquent les étapes terminées.

## Principes à respecter

Chaque changement est vérifié contre ces règles. Une étape qui en viole une est refusée, même si elle « marche ».

1. **Composants ultra-légers.** Les composants dérivent de `FbComponentBase` (`IComponent` direct). Pas de `ComponentBase`, pas d'`IHandleEvent`, pas d'`IHandleAfterRender` sauf besoin JS réel, pas de paramètre en cascade non fixe dans les composants répétés.
2. **Les composants ne lisent pas le thème.** Ils posent des classes et des redirections de variables. Le thème est résolu en C#, sérialisé en variables CSS, puis appliqué par la cascade.
3. **Grammaire visuelle minimale.** Fonds `Surface` / `Accent` / `Highlight`, frames `Light` / `Medium` / `Strong`, textes `Subtle` / `Muted` / `Default` / `Highlight`, tailles S / M / L / X. Aucune couleur dans l'API d'un composant.
4. **Couleurs dérivées.** Toute couleur rendue est un `color-mix()` en oklab entre deux ancres. Les variantes ne font que rediriger des ancres.
5. **Rendu ≠ fonctionnel.** `Muted` est un rendu ; `Error`, `Selected`, `Disabled` sont des variantes portées par `FbVariant`.
6. **Layout autonome.** Layout ne référence jamais Forms et doit cohabiter avec MudBlazor ou Radzen : classes et `@keyframes` préfixés `fb-`.
7. **DSL booléen + API enum.** Les booléens courts restent la voie principale ; une API enum explicite les complète pour les cas dynamiques.
8. **State-first.** Les champs envoient des actions à l'objet d'état (`Handle(TAction)`) ; la vue est choisie d'après le type de l'état.
9. **Aucune rétrocompatibilité.** La bibliothèque est neuve : pas d'`[Obsolete]`, pas d'alias historique, pas de façade.

## Décisions prises pour ce plan

- **Modèle d'interaction des champs.** Le state-first reste la voie principale. Chaque champ expose aussi `ValueChanged` (`EventCallback<T>`), ce qui rend `@bind-Value` possible sans coût quand il n'est pas utilisé. Une action générique `FbSetValue<T>` sert de `TAction` quand seul le binding est voulu.
- **JS.** Les fonctions JS restent chargées par une balise `<script>`, mais sont regroupées sous un unique objet global `FractalBlazor`. On évite ainsi la pollution de l'espace global sans payer un `import()` par composant.
- **Palette.** Les couleurs de base et leurs réglages restent un choix de design. Le plan ajoute un contrôle de contraste, mais ne retouche pas la palette.
- **Hors périmètre.** La fusion dans `master`, la publication NuGet et le push restent des décisions manuelles.

## Phase 0 — Cadre

- [x] **0.1 Branche et plan.** Créer `FinalizeLibrary` et ce document.
- [x] **0.2 Projet de tests.** Ajouter `FractalBlazor.Components.Tests` (xUnit + bUnit) à la solution, avec un test de fumée sur `FbRow` et sur la résolution du thème par défaut. Toutes les étapes suivantes ajoutent leurs tests ici.

## Phase 1 — Fiabiliser le socle

- [x] **1.1 Paramètres déclaratifs.** Ajouter à `FbComponentBase` deux points d'extension, `CaptureParameterDefaults()` au premier `SetParametersAsync` et `RestoreParameterDefaults()` aux suivants. Chaque couche qui stocke des paramètres dans des champs privés les implémente : `FbLayoutComponentBase`, `FbLayoutSurfaceComponentBase`, `FbFlexBoxBase`, `FbRow`, `FbStack`, items, séparateurs, breaks, Grid.
  - Critère : `<FbRow WBA="@flag">` perd sa classe de fond quand `flag` repasse à `false`, et les défauts posés par les constructeurs (`FbRow` : `JHSt`, `Flex = 1`) reviennent.
  - Les paires de contournement devenues inutiles (`DOutline`) sont conservées seulement si elles gardent un sens propre.
- [x] **1.2 Cache de styles sûr et borné.** Remplacer les dictionnaires statiques indexés par un hachage 32 bits par un cache dont la clé est la valeur complète de l'état (`struct` non managée comparée octet par octet, `Pack = 1`). Les chaînes libres (`Style`, `FlexBasis`, `MinWidth`…) contournent le cache partagé. Une capacité maximale protège la mémoire.
  - Critère : deux états différents ne partagent jamais une chaîne, deux états identiques la partagent, et un `Style` libre n'entre jamais dans le cache.
- [x] **1.3 Cycle de vie et ressources.**
  - `FbDynamicView` implémente `IDisposable` pour se désabonner de l'état.
  - `FbShowWhenVisible` est réécrit : un `IntersectionObserver` par instance, une `DotNetObjectReference`, `IAsyncDisposable`, plus aucun registre statique ni sondage du DOM. Le composant `FbCircularProgress` est réellement rendu.
- [x] **1.4 Rendu sobre.** N'attacher `onclick` que si un délégué existe (`FbRow`, `FbStack`, items). Supprimer l'attribut de debug `cpnt` et le paramètre `StoreId` qui ne servait qu'à lui. Corriger la duplication de `Classes` dans les items.
- [x] **1.5 Préfixe CSS.** Préfixer toutes les classes et `@keyframes` encore globales (`framed-*`, `row-break-*`, `column-break-*`, `rotate`, `dash`, `color`…) par `fb-`, et supprimer les classes mortes. `FbColumn` et `FbColumnContainner`, qui ne rendaient rien et que Grid remplace, sont supprimés dès l’étape 1.1.

## Phase 2 — Terminer le theming

- [x] **2.1 Inversion locale des ancres.** Implémenter `Invert` dans `FbVariant` (ancres de fond et de premier plan échangées, comme dans `Conception.md`) et dans `FbLayoutVariant` (ancres basse et haute du fond). L’inversion reste une simple redirection de variables. Une première version, `SB` / `SF` par paire, a été remplacée après vérification visuelle : elle produisait un fond gris au texte illisible.
- [x] **2.2 Thème local.** Ajouter `Scoped` à `FbTheme` et `FbLayoutTheme` : le thème est alors posé en style inline sur une `div.fb-theme-scope` au lieu d'un `<style>` global. Une page peut ainsi contenir un îlot `Light` dans une page `Dark`.
- [x] **2.3 Validation des variantes sans coût.** `FbTheme` fournit, comme `FbLayoutTheme`, un contexte en cascade fixe (`IsFixed`) qui liste ses variantes. `FbVariant` et `FbLayoutVariant` valident le nom quand le contexte existe et n'échouent pas quand il est absent (frontière de mode de rendu interactif).
- [x] **2.4 Contrôle de contraste.** Ajouter un calcul C# des couleurs résolues, fidèle aux formules CSS (`color-mix` en oklab), et un rapport WCAG : chaque couleur de texte sur chaque fond, pour chaque branche et chaque variante. Tests : noir sur blanc = 21:1 ; rapport complet du thème par défaut.
- [x] **2.5 Couleurs codées en dur.** Le spinner `FbCircularProgress` consomme les variables du thème.
- [x] **2.6 Documentation du theming.** Aligner `Theming.md` sur les noms du code et documenter `Scoped`, `Invert` et le rapport de contraste.

## Phase 3 — Couche Forms

- [x] **3.1 Base des champs.** Créer `FbInputBase<TValue, TAction>` : `Value`, `ValueChanged`, `Handler`, `Disabled`, `Variant`, et l'envoi d'action. Ajouter `FbSetValue<T>` et un répartiteur d'actions non générique pour les actions passées comme instances. Reconstruire `FbInputText` sur cette base.
- [x] **3.2 Tokens de contrôle.** Définir dans le CSS Forms les tokens `--fb-control-*` (fond, survol, focus, bordure, placeholder, curseur, sélection), tous dérivés des variables existantes.
- [x] **3.3 API enum de `FbText`.** Ajouter `Size`, `Weight`, `Color` à côté des booléens, et rendre les modificateurs `NW` / `TR` cumulables.
- [x] **3.4 `FbIcon`.** Rendre une icône de `FbMaterialIcons` en SVG `currentColor`, tailles S / M / L / X.
- [x] **3.5 `FbButton`.** Fond, frame et rayon de la grammaire Layout, état `Disabled`, `OnClick` et envoi d'une action instance (`Handler` + `Action`).
- [x] **3.6 `FbCheckbox` et `FbSwitch`.** Champs booléens state-first.
- [x] **3.7 `FbInputNumber` et `FbInputDate`.** Nombre générique (`INumber<T>`) et date (`DateOnly`) sur les contrôles natifs du navigateur.
- [x] **3.8 `FbSelect`.** Liste d’options typée sur `<select>` natif. Remplacé par la phase 6 : le Select repose désormais sur `FbDropDown` et `FbDynamicView`.
- [x] **3.9 Démo state-first.** Brancher `FractalBlazor.Demo.States` sur la démo, montrer `FbDynamicView` de bout en bout et une page de formulaires. Ranger les namespaces de `Common` (`FractalBlazor.Components.Common.*`).

## Phase 4 — Qualité, mesure et publication

- [x] **4.1 Avertissements.** Supprimer explicitement BL0007 dans les bibliothèques (inhérent au DSL booléen), corriger les autres avertissements. Objectif : zéro avertissement dans les bibliothèques.
- [x] **4.2 Dépendances.** Retirer SixLabors.ImageSharp de `Tools`. La version 3.2.0 reste vulnérable et exige une licence Six Labors pour les builds Release ; un lecteur PNG minimal fondé sur `ZLibStream` produit exactement la même palette.
- [x] **4.3 JS regroupé.** Regrouper les fonctions JS sous l'objet global `FractalBlazor` et mettre à jour les appels d'interop.
- [x] **4.4 Mesure.** Ajouter un projet BenchmarkDotNet qui compare le rendu de N `FbRow` à un équivalent fondé sur `ComponentBase` et à MudBlazor, et noter les résultats. Les chaînes `class` sont mises en cache comme les styles.
- [x] **4.5 Publication.** README complet, README de Grid corrigé, licence renseignée, métadonnées NuGet (`Directory.Build.props`, version 0.1.0-preview, documentation XML), workflow CI (build sans avertissement, tests, paquets).

## Phase 5 — Éditeur de thème

Objectif : régler les couleurs et les valeurs de mélange d'un thème en voyant le résultat en direct sur un jeu de composants, puis récupérer le code C# du thème obtenu. L'éditeur passe par le modèle C# (`FbThemeSetup`) et le pipeline normal (`FbTheme`) : aucune variable CSS n'est écrite à la main.

- [x] **5.1 Aperçu d’un thème non enregistré.** `IFbFormThemeRegistry.Resolve(FbThemeSetup, branche)` résout un thème qui hérite d'un thème enregistré, avec les mêmes validations, sans l'enregistrer ni le mettre en cache. `FbTheme` accepte ce thème via un paramètre `Setup`.
- [x] **5.2 `FbInputColor` et `FbInputRange`.** Champs state-first sur les contrôles natifs `color` et `range`, stylés par les tokens de contrôle.
- [x] **5.3 Page « Thème ».** En haut, les réglages : branche éditée, mélanges de surface, de frame et de texte, ancres de chaque variante. En dessous, dans un `FbTheme Scoped` qui applique le thème édité : surfaces et frames, textes, variantes, boutons, icônes, carte inversée et un faux formulaire. Un rapport de contraste et le code C# du thème suivent les réglages.

## Phase 6 — Select fondé sur DropDown et FbDynamicView

Un Select permet de choisir un élément dans une liste, mais délègue le rendu de cette liste. Il s'appuie sur `FbDropDown`, une zone ouverte par-dessus la page qui se ferme par un clic en dehors ou au clavier. Chaque élément, et la valeur choisie, sont rendus par `FbDynamicView` : la liste peut être polymorphe et mêler chaînes, énumérations et objets, chacun rendu par sa propre vue. Cette phase remplace le select natif de l'étape 3.8.

- [x] **6.1 FbDynamicView pour les types simples.** Une vue intégrée, `FbValueView`, rend les chaînes, énumérations (avec `[Display(Name)]`), nombres, dates et identifiants. Le registre s'y replie quand aucune vue n'est déclarée pour le type. Le registre de vues est enregistré en générique ouvert par `AddFractalBlazor()`, pour tout sélecteur, et la bibliothèque fournit le sélecteur `FbViewSelector.Item`.
- [x] **6.2 FbDropDown au clavier.** `Échap` ferme la zone, et `Block` donne à la zone toute la largeur de son parent.
- [x] **6.3 FbSelect sur FbDropDown.** L'ancre affiche la valeur choisie par `FbDynamicView`, la liste rend chaque élément de la même façon, avec le sélecteur `ItemSelector`. Navigation au clavier (flèches, `Début`, `Fin`, `Entrée`, `Espace`, `Échap`), rôles ARIA `combobox` et `listbox`, placeholder.
- [x] **6.4 Démo et documentation.** Une liste polymorphe dans la page Formulaires, l'enregistrement par `AddFractalBlazor()` dans la démo, le README à jour.

## Phase 7 — Application SPA : FbPageAnchor et modèle de navigation

FractalBlazor sert d'abord à écrire des applications d'une seule page. `FbPageAnchor` est la racine de leur mise en page : il occupe toute la fenêtre, sans jamais la faire défiler, et accueille un layout d'application fait de stacks et de rows. Une zone qui grandit défile seule, avec sa barre de défilement. L'application elle-même est un modèle : chaque élément de l'écran (en-tête, menu, sous-menu, page, pied de page) est une classe d'état, rendue par `FbDynamicView` avec la vue déclarée par `[ViewFor<ViewSelector>]`. Le menu du haut est une liste d'instances du modèle, chacune portant la sous-liste du menu vertical de gauche, et chaque entrée de menu donne le modèle de sa page.

- [x] **7.1 FbPageAnchor et zones de défilement.** `FbPageAnchor` (Layout) occupe la fenêtre (`position: fixed; inset: 0`), dispose ses enfants en colonne et laisse le premier occuper toute la hauteur ; il accepte les fonds, frames et paddings des surfaces. `Scrollable` sur `FbStack` et `FbRow` pose la classe `fb-scrollable` (`overflow: auto`) au lieu d'un `overflow: scroll` en ligne. Dans un anchor, chaque ancêtre d'une zone défilante peut descendre sous la taille de son contenu : la zone prend l'espace restant et défile, au lieu d'agrandir la page. Tests de rendu.
- [x] **7.2 FbDropDown au-dessus des zones de défilement.** Une zone défilante (`overflow: auto`) coupe tout ce qui en dépasse, y compris la liste d'un Select. Ouverte, la zone d'un `FbDropDown` est positionnée en `fixed` d'après le rectangle de son ancre, recalculé au défilement et au redimensionnement : elle passe par-dessus la page, quelle que soit la zone où vit l'ancre.
- [x] **7.3 Modèle de l'application.** Dans `FractalBlazor.Demo.States`, `RootState` devient le modèle de la démo : `HeaderState` (marque, sections du menu du haut, liens externes), `NavigationState` (menu de gauche `SideMenuState`, contenu `ContentState`), `FooterState` (texte, `ThemeSelectorState`). Chaque `SectionState` porte la sous-liste de son menu de gauche (`PageLinkState`, `MenuSeparatorState`), chaque `PageLinkState` porte le modèle de sa page. Les actions `Select` passent par la navigation, et seuls les états modifiés notifient leurs vues. Le modèle remplace les états de menu de l'ancienne page State-first. Tests de navigation sans interface.
- [x] **7.4 Vues et coquille de la démo.** Une vue par type du modèle (`[ViewFor<ViewSelector>]`), la coquille rendue dans un `FbPageAnchor` : en-tête en row, corps en row (menu de gauche en stack, contenu en stack défilante), pied de page en row. Les anciennes pages deviennent des vues de modèles de page, la démo n'a plus qu'une route. Le sélecteur de thème du pied de page est un `FbSelect` sur le mode clair ou sombre, et garde le cookie qui évite le flash au chargement.
- [x] **7.5 Documentation.** README : `FbPageAnchor`, `Scrollable` et le modèle d'application piloté par l'état.

## Phase 8 — Un site, deux hôtes : Server et WebAssembly

Les bibliothèques génériques restent `Layout`, `Forms` et `Common` ; `Benchmarks` les compare aux meilleures bibliothèques. Tout le site (documentation, démos, éditeur de thème, modèles d'état) devient une bibliothèque de composants, `FractalBlazor.WebSite`, chargée par deux hôtes : `FractalBlazor.WebSite.ServerSide` (rendu serveur interactif) et `FractalBlazor.WebSite.ClientSide` (WebAssembly autonome). Le site ne dépend d'aucun hôte : pas de `HttpContext`, pas d'API serveur ; chaque hôte ne fournit que la page HTML et le mode de thème initial.

- [x] **8.1 Bibliothèque FractalBlazor.WebSite.** Bibliothèque Razor (`SupportedPlatform browser`) qui reçoit les états de `FractalBlazor.Demo.States` (supprimé), les vues, l'éditeur de thème, les exemples et `ViewSelector`. Elle expose `AddFractalBlazorWebSite()`, le composant racine `WebSiteApplication` (mode de thème initial en paramètre), le cookie du thème (`ThemeCookie`) et ses ressources `fb-website.css` et `fb-website.js`.
- [x] **8.2 FractalBlazor.WebSite.ServerSide.** `FractalBlazor.Components` est renommé. Sa page d'accueil lit le cookie pendant le rendu serveur et rend `WebSiteApplication` en `InteractiveServer`.
- [x] **8.3 FractalBlazor.WebSite.ClientSide.** Application WebAssembly autonome qui référence `FractalBlazor.WebSite`. Le mode initial est lu dans le cookie par JS avant le premier rendu, et un script de `index.html` pose le `color-scheme` avant le chargement de Blazor.
- [x] **8.4 Registre de vues en WebAssembly.** Le registre parcourt les assemblies chargées au premier rendu d'un sélecteur, et reste tel quel. Une vue doit vivre dans une assembly déjà chargée quand son modèle est rendu pour la première fois : c'est le cas ici, états et vues partagent `FractalBlazor.WebSite`.
- [x] **8.5 Documentation et outillage.** README (structure de la solution, lancement des deux hôtes), configurations de lancement, CI.

## Phase 9 — Le modèle d'état dans son propre projet

Les états du site quittent `FractalBlazor.WebSite` pour `FractalBlazor.WebSite.StateModel`, qui ne référence que `FractalBlazor.Components.Common` : aucun composant, aucune API d'hôte. Quand le modèle a besoin d'un service qui vit dans une assembly qui le référence, il déclare une interface ; `FractalBlazor.WebSite` l'implémente et l'injecte. Les vues restent dans `FractalBlazor.WebSite`, chargée avant le premier rendu d'un état : le registre de vues les trouve toujours.

- [x] **9.1 Projet FractalBlazor.WebSite.StateModel.** Bibliothèque de classes qui reçoit le contenu de `States` sous l'espace de noms `FractalBlazor.WebSite.StateModel`. `FractalBlazor.WebSite` la référence.
- [x] **9.2 Projet FractalBlazor.WebSite.StateModel.Tests.** Projet xUnit qui reçoit les tests de navigation, jusqu'ici dans `FractalBlazor.Components.Tests` ; ce dernier ne teste plus que les bibliothèques. La CI exécute les deux projets.
- [x] **9.3 Services du modèle.** Enregistrer le mode de thème choisi est l'affaire de l'état, pas de la vue. `ThemeSelectorState` reçoit un `IThemeModeStore` déclaré dans le modèle ; `FractalBlazor.WebSite` l'implémente avec le cookie (JS) et l'enregistre dans `AddFractalBlazorWebSite()`. `SetMode` enregistre le choix, `Restore` applique le mode relu par l'hôte sans le réécrire. La vue n'envoie plus que l'action.
- [x] **9.4 Documentation.** README : les projets du site et la règle des services du modèle.

## Phase 10 — Espacements et rayons dans l'éditeur de thème

Les espacements (`Spacings`) et les rayons, ou courbures (`Corners`), sont des valeurs du thème communes à toutes ses branches. L'éditeur ne réglait que des valeurs de branche ; il règle désormais aussi celles-ci.

- [x] **10.1 Éditeur de thème.** Une carte « Espacements et rayons » règle les quatre espacements et les quatre rayons S, M, L, X, en rem, comme le thème par défaut. Ces valeurs appartiennent au thème, pas à la branche : changer de branche les conserve, et la carte a son propre bouton de réinitialisation. L'aperçu montre les quatre rayons et les quatre espacements, et le code C# déclare `Spacings` et `Corners` au niveau du thème quand ils diffèrent du thème par défaut.
- [x] **10.2 Documentation.** README : ce que règle l'éditeur.

## Phase 11 — Couleurs de filet, survol dans le thème, onglets et éditeur en onglets

- [x] **11.1 Couleurs de filet.** `FbThemeColorVariant.FrameColors` (`LowAnchor`, `HighAnchor`) donne aux filets leurs propres ancres, écrites dans `--fb-{variante}-frame-low-anchor` et `--fb-{variante}-frame-high-anchor` et redirigées comme les autres ancres (échangées par `Invert`). Un filet va de l'ancre basse des filets vers leur ancre haute ; sans ancre de filet, il va comme aujourd'hui du fond courant vers l'ancre haute des fonds (repli CSS). Frames, séparateurs, contours et piste des interrupteurs utilisent ce calcul.
- [x] **11.2 Survol dans le thème.** Le décalage de mélange au survol devient `FbThemeLayoutSurfaceMix.HoverOffset`, une valeur de branche écrite dans `--fb-bg-hover-offset` (4 % par défaut) ; l'appui d'un bouton en vaut le double. `HoverMixOffset` reste un réglage local.
- [x] **11.3 FbTabs.** Un sélecteur d'index en onglets, state-first comme les champs (`TAction`, `Handler`, `ValueChanged`) : il ne gère pas l'affichage de la page. Les libellés sont rendus par `FbDynamicView`. Rôles ARIA `tablist` et `tab`, flèches, `Début` et `Fin`.
- [x] **11.4 Éditeur en onglets.** Trois onglets d'édition : « Espacements et rayons », « Typographie » (toutes les valeurs typographiques du thème) et « Branches et mélanges » (couleurs des variantes, y compris les ancres de filet, mélanges de surface, de filet, de survol et de texte, contraste). Les couleurs se copient et se collent d'une ancre à l'autre.
- [x] **11.5 Aperçu en onglets.** « Surfaces et frames » : Surface, Accent, Highlight et Survol en colonnes, frames Light, Medium et Strong en lignes. « Textes » : de Subtle à Highlight en colonnes, de XS à XL en lignes. « Variantes » : la même grille pour chaque variante, avec un sélecteur de force du texte. « Boutons » : chaque variante sur chaque fond, avec des sélecteurs de frame et de texte. « Formulaire » : le faux formulaire, avec un sélecteur de variante.
- [x] **11.6 Documentation.** Theming.md : ancres de filet et décalage de survol ; README : `FbTabs`.

## Phase 12 — Ne jamais perdre un réglage de l'éditeur

- [x] **12.1 Une valeur par branche.** Changer de branche ne recharge plus les valeurs par défaut : chaque branche garde ses mélanges et ses ancres, et l'aperçu comme le code C# déclarent toutes les branches modifiées.
- [x] **12.2 Enregistrement automatique.** L'état de l'éditeur (valeurs du thème, de chaque branche, branche, variante et onglet ouverts) est enregistré dans le navigateur à chaque modification, et restauré à l'ouverture de l'éditeur, sur les deux hôtes. Un rechargement, une coupure du circuit ou un redémarrage ne perdent plus rien. « Repartir du thème par défaut » efface la session, après confirmation.

## Phase 13 — Case à cocher dessinée par le thème

- [x] **13.1 FbCheckbox personnalisé.** La case native, cachée mais focalisable et annoncée, porte l'état ; une case carrée (ou ronde, `Circle`) est dessinée avec le thème à côté d'un contenu quelconque, qui prend la largeur restante. `Mirror` la place à droite ; `Top`, `Center` (par défaut) et `Bottom` l'alignent sur un contenu plus haut, `Align` en donne la valeur calculée. Cochée, elle prend la couleur de texte Highlight, faite pour ressortir sur tous les fonds. Démonstration dans la page Formulaires.

## Phase 14 — Classes atomiques

Les styles inline des composants de mise en page répétaient les mêmes chaînes d'un élément à l'autre, dans le HTML comme dans les lots de rendu.

- [x] **14.1 Atomes.** Chaque déclaration de mise en page issue d'une valeur énumérée (display, padding, marge, flex, rayon, direction, alignements, gutter…) devient une classe technique très courte, nommée d'après sa position dans la table `FbAtoms` : `fb-a` à `fb-z`, puis `fb-aa`… La table est ordonnée comme la feuille : une propriété suit le raccourci qu'elle surcharge. Deux côtés opposés égaux s'écrivent en un atome `padding-block` ou `padding-inline`.
- [x] **14.2 Règles de base.** `.fb-row` et `.fb-stack` déclarent les valeurs de leur constructeur ; une rangée ou une pile n'ajoute que les atomes qui en diffèrent (une rangée par défaut n'a que `fb-row`). La section de la feuille est générée par `dotnet run --project FractalBlazor.Components.Tools -- atoms`, et un test vérifie qu'elle est à jour.
- [x] **14.3 Cache.** La clé de chaque niveau réunit désormais tout l'état (style et classes) ; la chaîne `class` est mise en cache comme l'était le style, et les `Classes` libres s'ajoutent à la fin, composant par composant, sans priver le reste du cache. Le style inline ne porte plus que les valeurs libres, et n'est pas rendu quand il est vide. Le cache reste un `ConcurrentDictionary`.
- [x] **14.4 Vérification.** Les styles calculés de chaque élément du site (35 propriétés, dimensions comprises) sont identiques avant et après, page par page.

## Phase 15 — Classes nommées comme les drapeaux, icônes en CSS

- [x] **15.1 Une classe par drapeau, du nom du drapeau.** `WBS`, `WBA`, `WBH` donnent `fb-wbs`, `fb-wba`, `fb-wbh` ; `WFL`, `WFM`, `WFS` donnent `fb-wfl`, `fb-wfm`, `fb-wfs` (au lieu de `fb-framed fb-framed-light`…) ; `WSL`, `WSM`, `WSS` donnent `fb-wsl`… sur les rangées, les piles et les grilles ; `EOutline` donne `fb-eoutline`, `WNTW` `fb-wntw`, `VO_XL` et `VU_XL` `fb-vo-xl` et `fb-vu-xl` ; `Subtle`, `Muted` et `Highlight` donnent `fb-subtle`, `fb-muted` et `fb-highlight` sur `FbText` et `FbIcon`. Les variables CSS gardent leur nom. La classe morte `fb-fg-default` est supprimée.
- [x] **15.2 Icônes des composants en CSS.** La coche de `FbCheckbox`, le chevron de `FbSelect` et la coche de l'option choisie sont dessinés par la feuille de style (`mask` et `currentColor`) sur un élément vide, au lieu d'un SVG inline à chaque instance. `FbIcon` garde le SVG inline pour les icônes de l'utilisateur.
- [x] **15.3 Vérification.** Styles calculés identiques sur onze écrans du site, liste ouverte comprise ; même boîte, même couleur et même opacité pour chaque icône.

## Phase 16 — Variante portée par la racine du contrôle

- [x] **16.1 Plus d'enveloppe.** Les champs, listes, cases, interrupteurs, onglets et boutons ne s'enveloppent plus dans une `div.fb-theme-scope`. Leur variante (`Variant`, ou `Disabled`) est posée sur leur élément racine : la classe `fb-variant` et les redirections dans son `style`. `:where(.fb-variant)` recalcule les couleurs sans spécificité, si bien que les règles du composant gardent la main. Passer à `Error` pendant la saisie ne change que deux attributs du même élément : le DOM et le focus restent en place.
- [x] **16.2 Vérification.** Styles calculés identiques sur le site, enveloppes exclues. Seule différence : une case posée sur un fond Accent dessine sa coche avec ce fond, et non plus avec le fond Surface que l'enveloppe rétablissait. Le focus du champ « Nom » reste sur le même élément en passant d'`Error` à normal et inversement.

## Phase 17 — Mesures vérifiables

- [x] **17.1 Taille du HTML.** `dotnet run -c Release --project FractalBlazor.Components.Benchmarks -- html` rend chaque scénario comme les benchmarks et affiche la taille du HTML (brut, gzip, Brotli), son nombre d'éléments, et les appels JavaScript faits au rendu, au re-rendu et à la destruction. Les services et les composants de chaque scénario sont communs aux benchmarks et à cette commande.
- [x] **17.2 FbDropDown n'appelle plus le script pour rien.** Il appelait `destroyDropdown` à chaque fermeture et à chaque destruction, même quand `initDropdown` n'avait jamais tourné : une liste jamais ouverte, ou prérendue, coûtait un appel JavaScript en quittant la page. Il ne détruit plus que ce qu'il a initialisé.
- [x] **17.3 Documentation.** Les README ne présentent que des mesures relancées sur le code du commit qui les contient.

## Phase 18 — Écran piloté par l'état

La structure de l'écran ne dépend plus de media queries : un capteur envoie à l'état la catégorie de largeur de sa zone, l'état en déduit ses modes, et les vues choisissent leur mise en page avec des `if`. Les grilles restent en CSS pour l'intérieur des pages.

- [x] **18.1 Catégories de largeur.** `FbWidthClass` (Common) : `Base` sous 512 px, puis `XS`, `S`, `M`, `L`, `XL`, `XXL` aux seuils des drapeaux `VO_*`/`VU_*` et de la grille. L'énumération est ordonnée : un état compare `Width < FbWidthClass.M`. `FbWidthClasses` porte les seuils, seule source pour le capteur.
- [x] **18.2 FbWidthSensor.** Composant de Components, basé sur une action comme les champs (`TAction`, `Handler`, `Value`, `ValueChanged`). Il observe la largeur de son parent (`ResizeObserver`) et n'appelle .NET que quand la catégorie change.
- [x] **18.3 FbDrawer.** Composant de Layout, contrôlé par `IsOpen` et `IsOpenChanged`, avec les drapeaux de surface. Côtés `Left`, `Right`, `Top`, `Bottom` ; trois modes : `Modal`, par-dessus tout l'écran, sur un `<dialog>` natif (focus piégé puis rendu, Échap, voile) ; `Inline`, à sa place dans la mise en page, qu'il pousse ; `InlineModal`, à sa place mais par-dessus sa zone, avec un voile limité à celle-ci.
- [x] **18.4 ScreenState.** L'état de l'écran du site : la catégorie de largeur, le menu en tiroir sous `M` (768 px), l'en-tête réduit sous `S` (640 px). Le tiroir (`MenuDrawerState`) et le menu de l'en-tête (`HeaderMenuState`) sont deux états à part : les ouvrir ne rend que leur vue. Ils se referment quand une page ou une section s'ouvre, et quand l'écran s'élargit au-delà de leur seuil.
- [x] **18.5 Le site.** L'application suit `ScreenState` : sous 768 px, le menu de gauche passe dans un tiroir ouvert par un bouton de l'en-tête ; sous 640 px, les sections de l'en-tête passent dans un sous-menu.
- [x] **18.6 Démonstration et documentation.** La page « Mise en page » montre les trois modes du tiroir ; README.

## Phase 19 — Nouveau layout du site

Le menu suit l'arbre donné : `Quick Start` (Introduction, Installation, Basics, Principles, Themes), `Components` (le groupe Layout : Row / Stack, Grid, Show When Over, Show When Visible, Drawer, Drop Down, Updatable ; les groupes Panels, View, Inputs, Selectors), `Compositions`, `Advanced`, `Building Blocks` (Templates), `Pricing`. Les libellés sont ceux de l'arbre ; les marqueurs [pro] et les pastilles de l'en-tête attendent.

- [x] **19.1 Un menu à trois niveaux.** Section, groupe, page. Un groupe (`MenuGroupState`) titre ses pages dans le menu de gauche ; une section sans pages est elle-même une page. Les pages existantes sont rangées : State-first sous Principles ; Variantes puis l'éditeur sous Themes ; le formulaire de contact sous Compositions ; l'ancienne page « Mise en page » se répartit entre Row / Stack, Grid et Drawer. Les autres feuilles affichent « Contenu à venir ».
- [x] **19.2 Un seul seuil.** Sous 1024 px (`L`), l'écran est compact : le menu du haut passe dans un sous-menu, celui de gauche dans un tiroir.
- [x] **19.3 L'en-tête.** Large : le logo et le nom à gauche ; à droite, les sections en texte, puis LinkedIn et GitHub en icônes. Compact, sur deux lignes : le logo, le nom et le sous-menu (⋮) ; puis le bouton du tiroir (☰) et le fil d'Ariane de la page ouverte (Section ▶ Groupe ▶ Page).
- [x] **19.4 Documentation.** Textes du site et README à jour.

## Phase 20 — Assistant des flags

Un outil du site, ouvert par un bouton icône de l'en-tête dans un tiroir large, pour trouver un flag par ses lettres ou par composant.

- [x] **20.1 Le catalogue.** Le site lit les flags sur les composants eux-mêmes : chaque composant public de Layout et Forms, chacun de ses paramètres booléens. Une grammaire dit ce que signifient les lettres, par famille (mise en page, texte, icône, points de rupture, mots) : `PHS` se lit Padding → Horizontal → Small, `PL` Padding → Large, `PLS` Padding → Left → Small. Un test échoue si un flag n'est pas expliqué. L'état reçoit le catalogue par une interface (`IFlagCatalog`).
- [x] **20.2 L'état de l'assistant.** Le champ Flag, éditable ; les colonnes de lettres : la première liste les débuts possibles, chaque choix ouvre la colonne suivante, groupée par catégorie ; les flags qui commencent par la saisie, les cinq premiers en haut ; la colonne des composants, filtrée par nom ou partie de nom, qui restreint les flags. Choisir un composant le place dans la recherche.
- [x] **20.3 La vue.** Un bouton dans l'en-tête, large comme compact ; un tiroir modal à droite, assez large, dont les colonnes passent à la ligne sur un écran étroit.
- [x] **20.4 Documentation.** README.
- [x] **20.5 Retours.** Les colonnes ne gardent que les parties qui mènent à un flag commençant par la saisie, sans tenir compte des majuscules : le reste est masqué, non grisé. Les premiers flags tiennent dans une grille de six cases à hauteur fixe ; leur nombre passe sur la ligne du titre. Un trait sépare les colonnes. Le bouton d'effacement reste en place, masqué quand le champ est vide : le champ ne change plus de hauteur.

## Phase 21 — Noms courts

- [x] **21.1 Flags renommés.** `FRow`, `FColumn`, `FRowDense`, `FColumnDense` deviennent `FR`, `FC`, `FRD`, `FCD` (Flow → Rows → Dense) ; `EOutline` devient `EO` et sa classe `fb-eo` ; `Disabled` devient `D` sur les boutons et les champs, le code lisant `IsDisabled`. Une classe de flag aussi courte pourrait porter le nom d'un atome : `FbAtoms` saute les noms réservés (`fb-eo`).

## Phase 22 — Teinte générale

- [x] **22.1 MasterTint appliqué.** `TintPercent` (entier) devient `TintMix`, un pourcentage CSS comme les autres mélanges ; `ColorTint` et `TintMix` se résolvent un à un, et `TintMix` doit valoir de `0%` à `100%`. Le CSS déclare `--fb-tint-color` et `--fb-tint-mix`, et chaque ancre, des thèmes Layout comme des branches Forms, les lit : elle garde sa luminosité, sa teinte et sa saturation penchent vers la teinte. Un gris baisse l'intensité de toutes les couleurs. `FbThemeContrast` mesure les ancres teintées. Le thème Layout par défaut ne cuit plus sa teinte dans ses ancres ; le thème Forms par défaut la garde à `0%`, ses ancres ayant été réglées sans elle.
- [x] **22.2 L'éditeur.** Teinte et mélange par branche, sauvegardés avec elle ; une sauvegarde plus ancienne garde la teinte par défaut. Le code du thème déclare la teinte quand elle change.

## Phase 23 — Frames sans place

- [x] **23.1 Frames en outline.** Une frame est un outline tracé à l'intérieur du bord (`outline-offset: -1px`) au lieu d'une bordure : l'ajouter ou la retirer ne bouge plus la boîte ni son contenu. Peinte par-dessus le contenu, elle reste visible sur un enfant qui a un fond et sur une zone qui défile. `EO` passe en ombre (`box-shadow`) pour ne pas entrer en conflit avec elle : un champ qui a le focus garde ses deux traits.

## Phase 24 — Un dégradé à trois pôles, relatif à l'imbrication

Chaque variante n'a plus qu'un dégradé : trois ancres, à 0 %, 50 % et 100 %. Fonds, textes et filets sont des positions sur ce dégradé. Le neutre part d'une valeur de branche (0 %) ; chaque bloc rempli le décale de son offset (Surface, Accent, Highlight), et ses textes et ses filets se placent par rapport à ce nouveau neutre, par la fin du dégradé : `100 − (100 − mix) × (100 − neutre) / 100`. Dark et Light gardent chacune tous leurs réglages.

- [x] **24.1 Le modèle et le CSS.** `FbThemeLayoutColors` prend une ancre `MidAnchor` ; `FbThemeColorVariant` ne garde que `LayoutColors` (`FormColors`, `FrameColors` et `FbThemeFormColors` disparaissent). `FbThemeLayoutSurfaceMix` devient `NeutralMix`, `SurfaceOffset`, `AccentOffset`, `HighlightOffset`, `HoverOffset` ; tous les mélanges sont des pourcentages de 0 % à 100 %. Le neutre et les offsets restent des pourcentages en CSS (ils s'additionnent, et `HoverMixOffset` garde sa forme) ; les mélanges des textes et des filets s'écrivent en fractions, CSS ne multipliant pas deux pourcentages. Les feuilles échantillonnent le dégradé à trois pôles ; le neutre s'imbrique sur six niveaux (une variable par profondeur, choisie en comptant les ancêtres remplis) ; un thème, de page ou `Scoped`, remet toutes les profondeurs à son neutre, si bien que l'aperçu de l'éditeur repart du sien ; le survol ajoute son offset au neutre de l'élément survolé ; `Invert` échange les ancres basse et haute. Le contrôle (focus, interrupteur, curseur) prend l'ancre médiane. `FbThemeContrast` suit les mêmes formules (`FbColorMath.Sample`, `FbColorMath.Relative`), sur les fonds de premier niveau. Les thèmes par défaut viennent de la palette : 950 / 500 / 50 en sombre, l'inverse en clair ; Disabled garde un dégradé court (950 / 700 / 500) pour rester terne. Mesuré sur le thème par défaut : le texte Default sur Highlight (45 %) atteint 4,1:1 en sombre et 4,2:1 en clair, sous AA ; le texte Highlight y passe (5,2:1 et 4,8:1). Le garde-fou exige AA pour Default et Highlight sur Surface et Accent, et pour Highlight sur Highlight.
- [x] **24.2 L'éditeur.** Trois couleurs par variante, le neutre de départ et les offsets ; le code du thème déclare `LayoutColors` avec ses trois ancres. Le format de sauvegarde passe à 2, sous une nouvelle clé : l'ancienne sauvegarde reste en place dans le navigateur, et ses espacements, rayons, typographie et teintes sont repris. Le tableau de contraste laisse Disabled de côté, WCAG n'exigeant rien d'un contrôle inactif.
- [x] **24.3 Documentation.** `Theming.md` décrit le dégradé, le neutre et son imbrication, les variables et les formules ; `Conception.md` garde la conversation d’origine et y ajoute l’évolution ; `README.md`, `AGENT.md`, la page d’introduction, la page des variantes et l’assistant des flags parlent du dégradé à trois ancres.

## Phase 25 — FbButton, la fonction seule ; les variantes en classes

`FbButton` incarne la fonction et ne traite pas du contenu. La variante d'un élément passe par une classe CSS : plus de contexte en cascade. Mesuré avant la phase (BenchmarkDotNet MediumRun, i9-9900K, .NET 10.0.12, 1 000 boutons minimaux identiques dans un `FbTheme`) : le `[CascadingParameter]` coûtait 27 à 56 µs au premier rendu (+2,3 % à +4,6 %, profondeur 1 à 10) et environ 100 Ko, 18 à 36 µs au re-rendu.

- [x] **25.1 Les variantes en classes.** `Variant="Error"` pose les classes `fb-variant fb-v-error` (et `fb-vi` inversée) ; la règle `.fb-v-error` redirige les trois ancres. Les règles des variantes standard sont dans la feuille Layout ; celles qu'un thème ajoute, dans le `<style>` de son `FbTheme`. Une variante inconnue ne correspond à aucune règle et garde celle de son parent. `FbButton`, les champs, `FbVariant` et `FbLayoutVariant` n'ont plus de style inline de variante ; `FbThemeContext`, `FbLayoutThemeContext` et leur `CascadingValue` disparaissent.
- [x] **25.2 FbButton.** Paramètres d'apparence : `S`, `M`, `L` (padding `PHM PVS`, `PHL PVM`, `PHX PVL`) et le genre `KW` (`WBN WFM`, par défaut), `KS` (`WBS WFM`), `KP` (`WBH WFM`), `KT` (`WBS WFN`), plus `Variant`. Les flags Layout restent, pour ajuster : écrit après un genre, un flag le corrige. Paramètres de fonction : `Handler`, `Action`, `OnClick`, `D`, `Submit`, `Title`, `ChildContent`. `Icon` et `EndIcon` disparaissent ; les usages mettent leur `FbIcon` dans le contenu. `FcButton` viendra plus tard.
- [x] **25.3 Documentation.** `Theming.md` décrit la variante en classe, les règles de la feuille et celles du thème, et l’absence de cascade, avec la mesure ; `README.md` présente le bouton ; `AGENT.md` dit qu’une variante est une classe.

## Phase 26 — Frames d'un côté, en pointillés, coins ronds

- [x] **26.1 Les flags.** Dans `FbLayoutSurfaceComponentBase` : `WFTx`, `WFRx`, `WFBx`, `WFLx` (With → Frame → Top, Right, Bottom, Left, x = `L`, `M`, `S`) tracent une ligne sur un côté, chacun avec sa force ; `WFD` (With → Frame → Dot) met en pointillés la frame et les lignes de côté ; `WRR` (With → Radius → Round) met le rayon à 50 %. Comme la frame, une ligne de côté ne prend aucune place. Elle est tracée par un `::after` posé sur la boîte, par-dessus le contenu ; sur une boîte qui défile elle-même (`Scrollable`), par des couches de fond, qui restent en place pendant le défilement. L'assistant des flags les explique.
- [ ] **26.2 Documentation.**

## Critères de fin

La bibliothèque est considérée finalisée quand :

- toutes les cases ci-dessus sont cochées ;
- `dotnet build` ne produit aucune erreur et aucun avertissement dans les bibliothèques ;
- `dotnet test` passe entièrement ;
- la démo montre Layout, theming, variantes, champs et `FbDynamicView` ;
- aucune classe CSS ni `@keyframes` global n'est sans préfixe `fb-` ;
- aucun état statique non borné ni partagé entre circuits ne subsiste.
