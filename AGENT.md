# AGENT.md

Rules for any agent (or person) changing this repository. They come before convenience: a change
that breaks one of them is wrong, even if it works.

## Where things live

| Project | Role |
| --- | --- |
| `FractalBlazor.Components.Common` | Contracts shared by the library and the states: actions (`IStateAction`), notification (`ObservableState`, `INotifyStateChanged`), view registration (`ViewFor`), and the services the library defines. |
| `FractalBlazor.Components.Layout` | Layout components: `FbRow`, `FbStack`, items, separators, grid, `FbPageAnchor`, `FbDropDown`… |
| `FractalBlazor.Components.Forms` | Theming, `FbDynamicView`, and the components under `Components/` (actions, display, icons, inputs, panels, selectors, switchers). |
| `FractalBlazor.WebSite.StateModel` | The states of the web site. References `Common` only. |
| `FractalBlazor.WebSite` | The web site: documentation, demos and the theme editor, as views of the states. |
| `FractalBlazor.WebSite.ServerSide`, `.ClientSide` | Hosts: the HTML shell and the services, nothing else. |

In these rules, **Components** means `FractalBlazor.Components.Forms/Components` and **Layout**
means `FractalBlazor.Components.Layout`.

## Rules

### 1. No `div` outside Components and Layout

Raw HTML elements (`div`, `span`, `a`, `pre`, `b`…) are written only inside the components of
Components and Layout. Everything else, and above all every demonstration of the library in
`FractalBlazor.WebSite`, is built exclusively from library components. When a demo needs
something the library cannot express, add or extend a library component; do not write markup
in the site.

The only exception is the HTML shell of a host (`App.razor`, `index.html`).

### 2. Everything is driven by the states

The interface is the image of a state model. Conventions flow from the library to the states:
the library defines its contracts in `Common`, and the states only depend on `Common`, never on
a component, a view or a host API. The states publish implementations of the services the
library defines.

When a state model needs a service from an assembly that references it, the model declares an
interface; that assembly implements it and injects it (example: `IThemeModeStore`, declared in
`FractalBlazor.WebSite.StateModel`, implemented by `FractalBlazor.WebSite` with a cookie).

State models are tested without any interface, in their own xUnit project
(`FractalBlazor.WebSite.StateModel.Tests`).

### 3. Every component is based on actions

A component changes nothing by itself: it sends an `IStateAction` to its `Handler`, whose
`Handle(TAction)` method updates the state, which notifies its views. Fields expose
`TAction`, `Handler` and `ValueChanged` (so `@bind-Value` works with `FbSetValue<T>`); buttons
expose `Handler` and `Action`. Do not add a component whose main API is a plain callback.

### 4. Every item is displayed by `FbDynamicView`

A component that shows an item (an option, a value, a label, a page, a menu entry) never
formats it itself: it renders it through `FbDynamicView` with an item selector, as `FbSelect`
and `FbTabs` do. Simple values (strings, enumerations with `[Display]`, numbers, dates) fall
back to `FbValueView`; anything else gets a view declared with `[ViewFor<TSelector>]`. A list
may therefore mix types, each rendered by its own view.

### 5. Every error is displayed on the surface background

An error is shown in the `Error` variant on its `-surface` background (`WBS`,
`--fb-bg-surface`), never on Accent or Highlight.

### 6. Components are built from Layout

The components of Components are composed from Layout components (`FbRow`, `FbStack`,
`FbRowItem`, `FbDropDown`…) wherever Layout can express the structure. Raw elements are kept
for what Layout cannot render (a native `input`, an `svg`, a `button`).

## Principles already in place

- Components derive from `FbComponentBase` (`IComponent`), not `ComponentBase`; class strings
  come from a cache keyed by the complete parameter values.
- Layout declarations are atoms: very short generated classes (`fb-a`, `fb-ab`…) defined by the
  `FbAtoms` table. Only free-form values stay inline. Never edit the atoms section of
  `fb-components-layout.css` by hand: change the table, then run
  `dotnet run --project FractalBlazor.Components.Tools -- atoms`.
- A class set by a flag is named after it: `WBA` → `fb-wba`, `WFM` → `fb-wfm`, `EOutline` →
  `fb-eoutline`, `VO_XL` → `fb-vo-xl`, `Muted` → `fb-muted`. CSS variables keep their names
  (`--fb-bg-accent`): they are the contract of the theme.
- Icons that belong to a component (a check mark, a chevron) are drawn by the stylesheet with
  `mask` and `currentColor`, on an empty element; `FbIcon` renders inline SVG for the icons
  chosen by the user.
- Components do not read the theme: they set classes and redirect CSS variables. Every color is
  a `color-mix()` in oklab between two anchors; a variant only redirects anchors.
- Short boolean flags are the main API (`WB*`, `WF*`, `WR*`, `P*`, `M*`, `G*`, `J*`), completed
  by an enum for computed values. Flags are reset before each parameter assignment.
- Layout never references Forms. Every CSS class and `@keyframes` starts with `fb-`.
- No backward compatibility: no `[Obsolete]`, no aliases.
- An editor never throws away what the user edited: switching a branch, a tab or an item keeps
  the edits, and only an explicit, confirmed reset discards them.

## Conventions

- Code, comments and commit messages in English; design documents (`Plan.md`, `Theming.md`,
  `README.md`) in French.
- One commit per step of `Plan.md`. Stage only the files of the change.

## Commands

```bash
dotnet build FractalBlazor.Components.slnx
```

```bash
dotnet test --project FractalBlazor.Components.Tests/FractalBlazor.Components.Tests.csproj
```

```bash
dotnet test --project FractalBlazor.WebSite.StateModel.Tests/FractalBlazor.WebSite.StateModel.Tests.csproj
```

```bash
dotnet run --project FractalBlazor.WebSite.ServerSide/FractalBlazor.WebSite.ServerSide.csproj --launch-profile http
```
