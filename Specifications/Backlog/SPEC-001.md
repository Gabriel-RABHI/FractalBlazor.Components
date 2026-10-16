## Système de navigation du Site

Toutes les pages doivent avoir une entrée dans une Enum générale : c'est une adresse absolue. La valeur pourrait être intégrée dans l'URL. Ainsi, en enregistrant l'URL, on peut retrouver une page. Dans l'Enum, les pages sont triées par l'ordre du menu générale et des sous menus : "Introduction", "Installation"..."Row / Stack", "Grid"...

Sur cette base, je voudrais développer un système de Navigation par onglet. L'idée est qu'au dessus de la page, il y ait la possibilité de créer des onglets de navigation. Quand on ouvre un nouvel onglet, il ouvre sur "Quick-Start -> Introduction". Cet onglet permet de naviguer dans le site via les menus.

Chaque page peut donc avoir un bouton "page suivante" et "page précédente", basé sur l'Enum.

Chaque page doit se refléter dans l'URL. L'URL du navigateur contient l'onglet actif : "t1/Basics". Quand on change d'onglet, cela devient "t2/Themes". Si l'onglet n'est pas retrouvé quand l'utilisateur colle une URL dans le navigateur, on créé l'onglet manquant. Une URL formaté "/Basic" doit aussi fonctionner.

Un système de navigation par onglet permet de revenir, uniquement sur l'onglet en question.

![[Pasted image 20261016120348.png]]

Dans le local Storage, on conserve l'état des onglets, avec leur 10 dernières navigations et l'index courrant dans leur navigation. Quand le site est chargé, on recréé les onglets.

Les boutons de navigation, les onglets et le bouton "+" sont affichés via une Grid, ou une Row multi-ligne.

Ce système permetra d'ouvrir un nouvel onglet

## Structuration du code

Dans le dossier Pages, chaque item du grand menu doit avoir son dossier (appellée Section) : `QuickStart`, `Components`, ... Dans chaque section, chaque page doit avoir son dossier. On doit avoir la même chose dans le `StateModel`. Dans `StateModel` on doit retrouver la même architecture : `States/App` et `States/Section/Pages/Page-X` avec un dossier par page.

Chaque page traite d'un sujet, par exemple "Row / Stack". Dans le dossier "RowStack" on doit retrouver les éléments de premier niveau, mais aussi 3 sous dossiers : `SandBoxes`, `Samples` et `References`.

Les références doivent lister la totalité des paramètres visibles, mais en combinant les héritages. Par exemple, `FbButton` aura, empilés, en premier ses paramètres propres et exclusifs, puis ceux de sa classe de base `FbLayoutSurfaceComponentBase`, puis ceux, encore en dessous, de `FbLayoutComponentBase`.

Les paramètres sont groupés par : `Flags`, `Values`, `Others` (champs de type `object`, évènements, action, RenderFragment, etc).

## Le Helper

Le champs du Helper n'est plus tant réservé aux noms des Flags, mais comme recherche. Plutot de chercher les Flags par leur code exclusivement, il recherche par leur description également. Cela ne change rien au système d'affichage et sélection de Flag en dessous, sous la forme des N colonnes.

Le Helper aura deux filtres : les flags, et les champs valeur (tout sauf bool). Le Helper doit permettre de retrouver ces deux type de champs. Il disposera de deux flags ronds activable (activés par défaut), "F" (pour "Flag property") et "V" (pour "Values property").

Quand un type de composant est actif (séllectionné), en dessous s'affiche trois Tabs (en bas du Helper, pleine largeur) : "Sand boxes", "Samples" et "References", qui reprennent les composants associés, s'ils existent.

## Convention de nom des paramètres

Actuellement, certains Flags sont sans logique d'acronyme. Par exemple, `Dot` est bien un bool, mais il est en texte plein. Tout ces champs là doivent commencer par `F` pour Flag. Pour `Dot`, cela donne `FDot`.

## Les blocs de code

Les Samples doivent reprendre la totalité du code source, et afficher le code source des States. On doit donc avoir des Tabs pour chaque fichier source. Il faut donc extraire chaque State dans un fichier séparé. Le `FbCodeBlock` doit n'afficher que le début du code, et un bouton pour déplier doit permettre de tout afficher. Quand on passe au dessus du nom d'un composant dans le code Razor, on doit avoir une petite bulle qui s'affiche, avec les boutons "Helper" (ouvre le Helper avec le composant sélectionné dans le filtre par composants), "Sand Box" (ouvre le Helper avec le composant sélectionné dans le filtre par composants, mais uniquement avec le Tab "Sand Box" sélectionné), "Samples" (Idem), "Refs." (Idem) et "Doc." (affichage de la page dans un nouvel onglet).

Développe le composant "FbTooltip" (contenu libre, ou simple string), qui doit supporter le clic dans son contenu.