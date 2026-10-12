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
- [x] **4.4 Mesure.** Ajouter un projet BenchmarkDotNet qui compare le rendu de N `FbRow` à un équivalent fondé sur `ComponentBase` et à MudBlazor, et noter les résultats. La première mesure montrait FractalBlazor plus lent que `ComponentBase` : les chaînes `class` sont désormais mises en cache comme les styles.
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
- [x] **8.4 Registre de vues en WebAssembly.** Mesuré dans l'hôte WebAssembly (Debug) : le premier parcours des assemblies chargées (48) prend environ 34 ms, plus 10 ms pour `FbViewSelector`. Le registre reste tel quel : ne parcourir que les assemblies qui référencent `FractalBlazor.Components.Common` ne ferait gagner que quelques dizaines de millisecondes. Une vue doit vivre dans une assembly déjà chargée quand son modèle est rendu pour la première fois : c'est le cas ici, états et vues partagent `FractalBlazor.WebSite`.
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

## Critères de fin

La bibliothèque est considérée finalisée quand :

- toutes les cases ci-dessus sont cochées ;
- `dotnet build` ne produit aucune erreur et aucun avertissement dans les bibliothèques ;
- `dotnet test` passe entièrement ;
- la démo montre Layout, theming, variantes, champs et `FbDynamicView` ;
- aucune classe CSS ni `@keyframes` global n'est sans préfixe `fb-` ;
- aucun état statique non borné ni partagé entre circuits ne subsiste.
