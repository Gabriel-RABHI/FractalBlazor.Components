using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// A column or a task of the board: it holds tasks. When it changes, it notifies its views
    /// and those of its ancestors, whose progress follows its own.
    /// </summary>
    public abstract class KanbanNode : ObservableState
    {
        public sealed record SetTitle(string Value) : IStateAction<string>;

        public sealed record ToggleEdit : IStateAction;

        private readonly List<KanbanTaskState> _tasks = [];

        protected KanbanNode(KanbanBoardState board, string title)
        {
            Board = board;
            Title = title;
        }

        public KanbanBoardState Board { get; }

        public string Title { get; private set; }

        /// <summary>
        /// In edition: its view shows fields instead of texts.
        /// </summary>
        public bool Editing { get; internal set; }

        /// <summary>
        /// The column or the task that holds this one; <c>null</c> for a column.
        /// </summary>
        public KanbanNode? Parent { get; private set; }

        public IReadOnlyList<KanbanTaskState> Tasks => _tasks;

        /// <summary>
        /// The tasks without subtasks below this node: the progress counts them.
        /// </summary>
        public virtual int LeafCount => _tasks.Sum(task => task.LeafCount);

        public virtual int DoneCount => _tasks.Sum(task => task.DoneCount);

        public void Handle(SetTitle action)
        {
            if (Title == action.Value)
                return;

            Title = action.Value;
            NotifyStateChanged();
        }

        public void Handle(ToggleEdit action)
        {
            Editing = !Editing;
            NotifyStateChanged();
        }

        /// <summary>
        /// True when <paramref name="node"/> lies below this one.
        /// </summary>
        public bool Contains(KanbanNode node)
        {
            for (var parent = node.Parent; parent is not null; parent = parent.Parent)
                if (parent == this)
                    return true;
            return false;
        }

        internal void Insert(int index, KanbanTaskState task)
        {
            _tasks.Insert(Math.Clamp(index, 0, _tasks.Count), task);
            task.Parent = this;
        }

        internal void Add(KanbanTaskState task) => Insert(_tasks.Count, task);

        internal void Remove(KanbanTaskState task) => _tasks.Remove(task);

        internal int IndexOf(KanbanTaskState task) => _tasks.IndexOf(task);

        /// <summary>
        /// This node and its ancestors notify their views.
        /// </summary>
        internal void Changed()
        {
            NotifyStateChanged();
            Parent?.Changed();
        }
    }

    /// <summary>
    /// A column of the board: a titled list of tasks.
    /// </summary>
    public sealed class KanbanColumnState : KanbanNode
    {
        public sealed record AddTask : IStateAction;

        public sealed record Delete : IStateAction;

        public KanbanColumnState(KanbanBoardState board, string title, params KanbanTaskState[] tasks)
            : base(board, title)
        {
            foreach (var task in tasks)
                Add(task);
        }

        /// <summary>
        /// A new task comes first, ready to be named.
        /// </summary>
        public void Handle(AddTask action)
        {
            Insert(0, new KanbanTaskState(Board, "Nouvelle tâche") { Editing = true });
            Changed();
        }

        public void Handle(Delete action) => Board.Remove(this);
    }

    /// <summary>
    /// A task: done or not, with details, and maybe subtasks. A task with subtasks is done when
    /// they all are.
    /// </summary>
    public sealed class KanbanTaskState : KanbanNode
    {
        public sealed record SetDetails(string Value) : IStateAction<string>;

        public sealed record ToggleDone : IStateAction;

        public sealed record Delete : IStateAction;

        private bool _done;

        public KanbanTaskState(KanbanBoardState board, string title, string details = "", bool done = false, params KanbanTaskState[] subTasks)
            : base(board, title)
        {
            Details = details;
            _done = done;
            foreach (var task in subTasks)
                Add(task);
        }

        public string Details { get; private set; }

        public bool HasSubTasks => Tasks.Count > 0;

        public bool Done => HasSubTasks ? DoneCount == LeafCount : _done;

        public override int LeafCount => HasSubTasks ? base.LeafCount : 1;

        public override int DoneCount => HasSubTasks ? base.DoneCount : _done ? 1 : 0;

        public void Handle(SetDetails action)
        {
            if (Details == action.Value)
                return;

            Details = action.Value;
            NotifyStateChanged();
        }

        /// <summary>
        /// Only a task without subtasks is checked by hand.
        /// </summary>
        public void Handle(ToggleDone action)
        {
            if (HasSubTasks)
                return;

            _done = !_done;
            Changed();
        }

        public void Handle(Delete action)
        {
            if (Parent is not { } parent)
                return;

            parent.Remove(this);
            parent.Changed();
        }

        public override string ToString() => Title;
    }

    /// <summary>
    /// A board of columns of tasks, arranged by drag and drop: a column moves between the
    /// others; a task goes into a column, before or after another task, or inside it as a
    /// subtask.
    /// </summary>
    public sealed class KanbanBoardState : ObservableState, IFbDragAndDropRules
    {
        public sealed record AddColumn : IStateAction;

        private readonly List<KanbanColumnState> _columns;

        public KanbanBoardState()
        {
            _columns =
            [
                new KanbanColumnState(this, "À faire",
                    new KanbanTaskState(this, "Créer la classe du composant", "Voir les composants internes", subTasks:
                    [
                        new KanbanTaskState(this, "Bouton", "Voir les composants internes"),
                        new KanbanTaskState(this, "Liste"),
                        new KanbanTaskState(this, "Image", "Par URL ou par ressource de l’assembly")
                    ]),
                    new KanbanTaskState(this, "Documenter le code"),
                    new KanbanTaskState(this, "Publier les paquets NuGet")),
                new KanbanColumnState(this, "En cours",
                    new KanbanTaskState(this, "Créer les projets", done: true),
                    new KanbanTaskState(this, "Définir les contrats", subTasks:
                    [
                        new KanbanTaskState(this, "IStateAction", "Une action par changement", done: true),
                        new KanbanTaskState(this, "Règles de dépôt"),
                        new KanbanTaskState(this, "Notifications", "Ciblées sur les vues concernées")
                    ]),
                    new KanbanTaskState(this, "Référencer les espaces de noms")),
                new KanbanColumnState(this, "Terminé",
                    new KanbanTaskState(this, "Passer à .NET 10", done: true),
                    new KanbanTaskState(this, "Vérifier les bibliothèques externes", done: true))
            ];
        }

        public IReadOnlyList<KanbanColumnState> Columns => _columns;

        public void Handle(AddColumn action)
        {
            _columns.Add(new KanbanColumnState(this, "Nouvelle liste") { Editing = true });
            NotifyStateChanged();
        }

        internal void Remove(KanbanColumnState column)
        {
            if (_columns.Remove(column))
                NotifyStateChanged();
        }

        // ------------------------------------------------------------------ Rules

        /// <summary>
        /// A column goes among the columns; a task into a column, or next to or into another
        /// task, but never into its own subtasks.
        /// </summary>
        public bool Accepts(object dragId, object dropId) => (dragId, dropId) switch
        {
            (KanbanColumnState column, KanbanColumnState target) => column != target,
            (KanbanTaskState, KanbanColumnState) => true,
            (KanbanTaskState task, KanbanTaskState target) => task != target && !task.Contains(target),
            _ => false
        };

        /// <summary>
        /// A column goes on the side of the zone, a task dropped on a column goes at its end,
        /// and a task dropped on a task goes above it, below it, or inside it.
        /// </summary>
        public FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone) => (dragId, dropId) switch
        {
            (KanbanColumnState, KanbanColumnState) => zone == FbDropZone.Left ? FbDropFeedback.Left : FbDropFeedback.Right,
            (KanbanTaskState, KanbanColumnState) => FbDropFeedback.Center,
            (KanbanTaskState, KanbanTaskState) => zone switch
            {
                FbDropZone.Top => FbDropFeedback.Top,
                FbDropZone.Bottom => FbDropFeedback.Bottom,
                _ => FbDropFeedback.Center
            },
            _ => FbDropFeedback.None
        };

        public void Handle(FbDrop drop)
        {
            if (!Accepts(drop.DragId, drop.DropId))
                return;

            switch (drop.DragId, drop.DropId)
            {
                case (KanbanColumnState column, KanbanColumnState target):
                    _columns.Remove(column);
                    var index = _columns.IndexOf(target);
                    _columns.Insert(drop.Feedback == FbDropFeedback.Left ? index : index + 1, column);
                    NotifyStateChanged();
                    break;

                case (KanbanTaskState task, KanbanColumnState column):
                    Move(task, column, () => column.Tasks.Count);
                    break;

                case (KanbanTaskState task, KanbanTaskState target) when drop.Feedback == FbDropFeedback.Center:
                    Move(task, target, () => target.Tasks.Count);
                    break;

                case (KanbanTaskState task, KanbanTaskState target):
                    var parent = target.Parent!;
                    Move(task, parent, () => parent.IndexOf(target) + (drop.Feedback == FbDropFeedback.Bottom ? 1 : 0));
                    break;
            }
        }

        /// <summary>
        /// Takes the task out of its place, then puts it at the index the new place gives once
        /// it is gone. Both places, and their ancestors, notify.
        /// </summary>
        private static void Move(KanbanTaskState task, KanbanNode parent, Func<int> index)
        {
            var from = task.Parent!;
            from.Remove(task);
            parent.Insert(index(), task);
            from.Changed();
            if (parent != from)
                parent.Changed();
        }
    }
}
