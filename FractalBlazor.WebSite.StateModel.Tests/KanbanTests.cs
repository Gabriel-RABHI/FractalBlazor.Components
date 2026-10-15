using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The kanban board: its rules, and the moves of columns and tasks by drag and drop.
/// </summary>
public sealed class KanbanTests
{
    private readonly KanbanBoardState _board = new();

    private KanbanColumnState Column(string title) => _board.Columns.Single(column => column.Title == title);

    private KanbanTaskState Task(string title) => All(_board.Columns.SelectMany(column => column.Tasks)).Single(task => task.Title == title);

    private static IEnumerable<KanbanTaskState> All(IEnumerable<KanbanTaskState> tasks)
        => tasks.SelectMany(task => All(task.Tasks).Prepend(task));

    private static IEnumerable<string> Titles(KanbanNode node) => node.Tasks.Select(task => task.Title);

    private void Drop(object dragged, object receiver, FbDropZone zone)
    {
        Assert.True(_board.Accepts(dragged, receiver));
        _board.Handle(new FbDrop(dragged, receiver, zone, _board.Preview(dragged, receiver, zone)));
    }

    [Fact]
    public void A_column_moves_to_the_side_of_the_zone()
    {
        var done = Column("Terminé");

        Drop(done, Column("À faire"), FbDropZone.Left);
        Assert.Equal(["Terminé", "À faire", "En cours"], _board.Columns.Select(column => column.Title));

        Drop(done, Column("En cours"), FbDropZone.Center);
        Assert.Equal(["À faire", "En cours", "Terminé"], _board.Columns.Select(column => column.Title));
    }

    [Fact]
    public void A_task_dropped_on_a_column_goes_at_its_end()
    {
        var task = Task("Documenter le code");

        Drop(task, Column("Terminé"), FbDropZone.Left);

        Assert.Equal("Documenter le code", Titles(Column("Terminé")).Last());
        Assert.DoesNotContain("Documenter le code", Titles(Column("À faire")));
        Assert.Same(Column("Terminé"), task.Parent);
    }

    [Fact]
    public void A_task_goes_above_or_below_another_one()
    {
        var task = Task("Publier les paquets NuGet");

        Drop(task, Task("Créer les projets"), FbDropZone.Top);
        Assert.Equal(["Publier les paquets NuGet", "Créer les projets", "Définir les contrats", "Référencer les espaces de noms"], Titles(Column("En cours")));

        Drop(task, Task("Définir les contrats"), FbDropZone.Bottom);
        Assert.Equal(["Créer les projets", "Définir les contrats", "Publier les paquets NuGet", "Référencer les espaces de noms"], Titles(Column("En cours")));
    }

    [Fact]
    public void A_task_dropped_on_the_center_of_another_becomes_its_subtask()
    {
        var task = Task("Documenter le code");
        var target = Task("Créer les projets");
        Assert.True(target.Done);

        Drop(task, target, FbDropZone.Center);

        Assert.Equal(["Documenter le code"], Titles(target));
        Assert.Same(target, task.Parent);
        Assert.False(target.Done);
    }

    [Fact]
    public void A_task_never_goes_into_its_own_subtasks_nor_a_column_into_a_task()
    {
        var parent = Task("Définir les contrats");

        Assert.False(_board.Accepts(parent, Task("IStateAction")));
        Assert.False(_board.Accepts(parent, parent));
        Assert.False(_board.Accepts(Column("À faire"), parent));
        Assert.True(_board.Accepts(Task("IStateAction"), parent));
    }

    [Fact]
    public void The_progress_follows_the_subtasks_and_notifies_the_ancestors()
    {
        var column = Column("En cours");
        var parent = Task("Définir les contrats");
        var notifications = new List<string>();
        column.OnStateChanged += () => notifications.Add("column");
        parent.OnStateChanged += () => notifications.Add("parent");
        Assert.Equal((1, 3), (parent.DoneCount, parent.LeafCount));

        Task("Règles de dépôt").Handle(new KanbanTaskState.ToggleDone());
        Task("Notifications").Handle(new KanbanTaskState.ToggleDone());

        Assert.True(parent.Done);
        Assert.Equal((4, 5), (column.DoneCount, column.LeafCount));
        Assert.Equal(["parent", "column", "parent", "column"], notifications);
    }

    [Fact]
    public void A_task_with_subtasks_is_not_checked_by_hand()
    {
        var parent = Task("Définir les contrats");

        parent.Handle(new KanbanTaskState.ToggleDone());

        Assert.False(parent.Done);
    }

    [Fact]
    public void Tasks_and_columns_are_added_ready_to_be_named_and_deleted()
    {
        var column = Column("À faire");

        column.Handle(new KanbanColumnState.AddTask());
        Assert.Equal("Nouvelle tâche", column.Tasks[0].Title);
        Assert.True(column.Tasks[0].Editing);

        column.Tasks[0].Handle(new KanbanTaskState.Delete());
        Assert.DoesNotContain("Nouvelle tâche", Titles(column));

        _board.Handle(new KanbanBoardState.AddColumn());
        Assert.Equal("Nouvelle liste", _board.Columns[^1].Title);
        _board.Columns[^1].Handle(new KanbanColumnState.Delete());
        Assert.Equal(3, _board.Columns.Count);
    }

    [Fact]
    public void Titles_and_details_are_edited()
    {
        var task = Task("Liste");

        task.Handle(new KanbanNode.ToggleEdit());
        task.Handle(new KanbanNode.SetTitle("Liste virtuelle"));
        task.Handle(new KanbanTaskState.SetDetails("Avec défilement"));
        task.Handle(new KanbanNode.ToggleEdit());

        Assert.Equal(("Liste virtuelle", "Avec défilement", false), (task.Title, task.Details, task.Editing));
    }
}
