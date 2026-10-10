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

- [x] **2.1 Inversion locale des ancres.** Implémenter `SB` (inverser les ancres de fond) et `SF` (inverser les ancres de premier plan) dans `FbVariant`, et `SB` dans `FbLayoutVariant`. L'inversion reste une simple redirection de variables.
- [x] **2.2 Thème local.** Ajouter `Scoped` à `FbTheme` et `FbLayoutTheme` : le thème est alors posé en style inline sur une `div.fb-theme-scope` au lieu d'un `<style>` global. Une page peut ainsi contenir un îlot `Light` dans une page `Dark`.
- [x] **2.3 Validation des variantes sans coût.** `FbTheme` fournit, comme `FbLayoutTheme`, un contexte en cascade fixe (`IsFixed`) qui liste ses variantes. `FbVariant` et `FbLayoutVariant` valident le nom quand le contexte existe et n'échouent pas quand il est absent (frontière de mode de rendu interactif).
- [x] **2.4 Contrôle de contraste.** Ajouter un calcul C# des couleurs résolues, fidèle aux formules CSS (`color-mix` en oklab), et un rapport WCAG : chaque couleur de texte sur chaque fond, pour chaque branche et chaque variante. Tests : noir sur blanc = 21:1 ; rapport complet du thème par défaut.
- [x] **2.5 Couleurs codées en dur.** Le spinner `FbCircularProgress` consomme les variables du thème.
- [x] **2.6 Documentation du theming.** Aligner `Theming.md` sur les noms du code et documenter `Scoped`, `SB`, `SF` et le rapport de contraste.

## Phase 3 — Couche Forms

- [ ] **3.1 Base des champs.** Créer `FbInputBase<TValue, TAction>` : `Value`, `ValueChanged`, `Handler`, `Disabled`, `Variant`, et l'envoi d'action. Ajouter `FbSetValue<T>` et un répartiteur d'actions non générique pour les actions passées comme instances. Reconstruire `FbInputText` sur cette base.
- [ ] **3.2 Tokens de contrôle.** Définir dans le CSS Forms les tokens `--fb-control-*` (fond, survol, focus, bordure, placeholder, curseur, sélection), tous dérivés des variables existantes.
- [ ] **3.3 API enum de `FbText`.** Ajouter `Size`, `Weight`, `Color` à côté des booléens, et rendre les modificateurs `NW` / `TR` cumulables.
- [ ] **3.4 `FbIcon`.** Rendre une icône de `FbMaterialIcons` en SVG `currentColor`, tailles S / M / L / X.
- [ ] **3.5 `FbButton`.** Fond, frame et rayon de la grammaire Layout, état `Disabled`, `OnClick` et envoi d'une action instance (`Handler` + `Action`).
- [ ] **3.6 `FbCheckbox` et `FbSwitch`.** Champs booléens state-first.
- [ ] **3.7 `FbInputNumber` et `FbInputDate`.** Nombre générique (`INumber<T>`) et date (`DateOnly`) sur les contrôles natifs du navigateur.
- [ ] **3.8 `FbSelect`.** Liste d'options typée sur `<select>` natif.
- [ ] **3.9 Démo state-first.** Brancher `FractalBlazor.Demo.States` sur la démo, montrer `FbDynamicView` de bout en bout et une page de formulaires. Ranger les namespaces de `Common` (`FractalBlazor.Components.Common.*`).

## Phase 4 — Qualité, mesure et publication

- [ ] **4.1 Avertissements.** Supprimer explicitement BL0007 dans les bibliothèques (inhérent au DSL booléen), corriger les autres avertissements. Objectif : zéro avertissement dans les bibliothèques.
- [ ] **4.2 Dépendances.** Mettre à jour SixLabors.ImageSharp dans `Tools` vers une version sans vulnérabilité connue.
- [ ] **4.3 JS regroupé.** Regrouper les fonctions JS sous l'objet global `FractalBlazor` et mettre à jour les appels d'interop.
- [ ] **4.4 Mesure.** Ajouter un projet BenchmarkDotNet qui compare le rendu de N `FbRow` à un équivalent fondé sur `ComponentBase`, et noter les résultats dans le README.
- [ ] **4.5 Publication.** README complet, README de Grid corrigé, licence renseignée, métadonnées NuGet, workflow CI (build et tests).

## Critères de fin

La bibliothèque est considérée finalisée quand :

- toutes les cases ci-dessus sont cochées ;
- `dotnet build` ne produit aucune erreur et aucun avertissement dans les bibliothèques ;
- `dotnet test` passe entièrement ;
- la démo montre Layout, theming, variantes, champs et `FbDynamicView` ;
- aucune classe CSS ni `@keyframes` global n'est sans préfixe `fb-` ;
- aucun état statique non borné ni partagé entre circuits ne subsiste.
