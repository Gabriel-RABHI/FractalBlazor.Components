using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.DragAndDrop;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace FractalBlazor.Components.Tests.Forms;

/// <summary>
/// A drag and drop area: a draggable item, grabbed by its handle, and receivers whose zones
/// open while the rules accept the dragged item. The area sends <see cref="FbDrop"/> to its
/// handler when the item is released on a zone that accepts it.
/// </summary>
public sealed class DragAndDropTests : BunitContext
{
    /// <summary>
    /// The state of the area: its rules, and the drops it received.
    /// </summary>
    public sealed class AreaState : IFbDragAndDropRules
    {
        public Func<object, object, bool> Accept { get; set; } = (_, _) => true;

        public Func<object, object, FbDropZone, FbDropFeedback> Previewer { get; set; } = (_, _, zone) => FbDrop.NaturalFeedback(zone);

        public List<FbDrop> Drops { get; } = [];

        public bool Accepts(object dragId, object dropId) => Accept(dragId, dropId);

        public FbDropFeedback Preview(object dragId, object dropId, FbDropZone zone) => Previewer(dragId, dropId, zone);

        public void Handle(FbDrop drop) => Drops.Add(drop);
    }

    private readonly AreaState _state = new();

    private IRenderedComponent<FbDragAndDropMaster> RenderArea(object? handler = null, bool showInfo = false, bool withReceiverB = true)
        => Render<FbDragAndDropMaster>(p => p
            .Add(x => x.Handler, handler ?? _state)
            .Add(x => x.ShowInfo, showInfo)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<FbDraggable>(0);
                builder.AddComponentParameter(1, nameof(FbDraggable.DragId), "item");
                builder.AddComponentParameter(2, nameof(FbDraggable.ChildContent), Handle("Item"));
                builder.CloseComponent();
                Receiver(builder, 10, "A", FbDropSplit.TopCenterBottom);
                if (withReceiverB)
                    Receiver(builder, 20, "B", FbDropSplit.LeftRight);
            }));

    private static RenderFragment Handle(string text) => builder =>
    {
        builder.OpenComponent<FbDraggableHandle>(0);
        builder.AddComponentParameter(1, nameof(FbDraggableHandle.ChildContent), (RenderFragment)(content => content.AddContent(0, text)));
        builder.CloseComponent();
    };

    private static void Receiver(RenderTreeBuilder builder, int sequence, string dropId, FbDropSplit split)
    {
        builder.OpenComponent<FbDraggableReceiver>(sequence);
        builder.AddComponentParameter(sequence + 1, nameof(FbDraggableReceiver.DropId), dropId);
        builder.AddComponentParameter(sequence + 2, nameof(FbDraggableReceiver.Split), split);
        builder.AddComponentParameter(sequence + 3, nameof(FbDraggableReceiver.ChildContent), (RenderFragment)(content => content.AddContent(0, "Receiver " + dropId)));
        builder.CloseComponent();
    }

    private static AngleSharp.Dom.IElement ReceiverElement(IRenderedComponent<FbDragAndDropMaster> cut, int index)
        => cut.FindAll(".fb-drop-receiver")[index];

    private static AngleSharp.Dom.IElement Zone(IRenderedComponent<FbDragAndDropMaster> cut, int receiver, string zone)
        => ReceiverElement(cut, receiver).QuerySelector(".fb-drop-zone-" + zone)!;

    private static Task StartAsync(IRenderedComponent<FbDragAndDropMaster> cut)
        => cut.Find(".fb-draggable").TriggerEventAsync("ondragstart", new DragEventArgs());

    // ------------------------------------------------------------------ At rest

    [Fact]
    public void At_rest_the_item_is_not_draggable_and_the_receivers_add_nothing()
    {
        var cut = RenderArea();

        Assert.Equal("false", cut.Find(".fb-draggable").GetAttribute("draggable"));
        Assert.Equal("Item", cut.Find(".fb-draggable > .fb-drag-handle").TextContent);
        Assert.Equal("Receiver A", ReceiverElement(cut, 0).InnerHtml);
        Assert.Empty(cut.FindAll(".fb-drop-zones"));
    }

    [Fact]
    public void Each_part_placed_where_it_cannot_work_says_so_and_keeps_its_content()
    {
        var draggable = Render<FbDraggable>(p => p.Add(x => x.DragId, "item").AddChildContent("Content"));
        var receiver = Render<FbDraggableReceiver>(p => p.Add(x => x.DropId, "receiver").AddChildContent("Content"));
        var handle = Render<FbDraggableHandle>(p => p.AddChildContent("Content"));

        Assert.Contains("FbDraggable must be placed inside an FbDragAndDropMaster.", draggable.Markup);
        Assert.Contains("FbDraggableReceiver must be placed inside an FbDragAndDropMaster.", receiver.Markup);
        Assert.Contains("FbDraggableHandle must be placed inside an FbDraggable.", handle.Markup);
        Assert.All([draggable.Markup, receiver.Markup, handle.Markup], markup =>
        {
            Assert.Contains("Content", markup);
            Assert.Contains("fb-v-error", markup);
        });
        Assert.Empty(draggable.FindAll(".fb-draggable"));
    }

    [Fact]
    public void An_item_or_a_receiver_without_identifier_says_so()
    {
        var cut = Render<FbDragAndDropMaster>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<FbDraggable>(0);
            builder.CloseComponent();
            builder.OpenComponent<FbDraggableReceiver>(1);
            builder.CloseComponent();
        }));

        Assert.Contains("FbDraggable needs a DragId.", cut.Markup);
        Assert.Contains("FbDraggableReceiver needs a DropId.", cut.Markup);
    }

    [Fact]
    public void An_item_and_a_receiver_take_their_share_of_a_row()
    {
        var cut = Render<FbDragAndDropMaster>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<FbDraggable>(0);
            builder.AddComponentParameter(1, nameof(FbDraggable.DragId), "item");
            builder.AddComponentParameter(2, nameof(FbDraggable.Flex), 2);
            builder.CloseComponent();
            builder.OpenComponent<FbDraggableReceiver>(3);
            builder.AddComponentParameter(4, nameof(FbDraggableReceiver.DropId), "A");
            builder.AddComponentParameter(5, nameof(FbDraggableReceiver.Flex), 1);
            builder.CloseComponent();
        }));

        Assert.Contains(FbAtoms.NameOf("flex:2"), cut.Find(".fb-draggable").ClassList);
        Assert.Contains(FbAtoms.NameOf("flex:1"), cut.Find(".fb-drop-receiver").ClassList);
    }

    // ------------------------------------------------------------------ Drag

    [Fact]
    public async Task A_drag_opens_the_zones_of_the_receivers_that_accept_the_item()
    {
        _state.Accept = (dragId, dropId) => (string)dropId == "A";
        var cut = RenderArea();

        await StartAsync(cut);

        Assert.Equal(["fb-drop-zone-top", "fb-drop-zone-center", "fb-drop-zone-bottom"],
            ReceiverElement(cut, 0).QuerySelectorAll(".fb-drop-zone").Select(zone => zone.ClassList[1]));
        Assert.Contains("fb-v-selected", ReceiverElement(cut, 0).QuerySelector(".fb-drop-feedback")!.ClassList);
        Assert.Null(ReceiverElement(cut, 1).QuerySelector(".fb-drop-zones"));
        Assert.Equal("item", cut.Instance.DragId);
    }

    [Fact]
    public async Task Without_rules_every_receiver_accepts_and_each_zone_offers_its_natural_place()
    {
        var handler = new DropsOnlyHandler();
        var cut = RenderArea(handler);

        await StartAsync(cut);
        await Zone(cut, 1, "right").TriggerEventAsync("ondragenter", new DragEventArgs());

        Assert.NotNull(ReceiverElement(cut, 0).QuerySelector(".fb-drop-zones"));
        Assert.Contains("fb-drop-right", ReceiverElement(cut, 1).QuerySelector(".fb-drop-feedback")!.ClassList);
    }

    [Fact]
    public async Task Entering_a_zone_shows_its_preview_and_the_left_receiver_waits_again()
    {
        var cut = RenderArea();
        await StartAsync(cut);

        await Zone(cut, 0, "top").TriggerEventAsync("ondragenter", new DragEventArgs());
        Assert.Contains("fb-drop-top", ReceiverElement(cut, 0).QuerySelector(".fb-drop-feedback")!.ClassList);

        await Zone(cut, 1, "left").TriggerEventAsync("ondragenter", new DragEventArgs());
        await Zone(cut, 0, "top").TriggerEventAsync("ondragleave", new DragEventArgs());

        Assert.Equal(["fb-drop-feedback", "fb-variant", "fb-v-selected"], ReceiverElement(cut, 0).QuerySelector(".fb-drop-feedback")!.ClassList);
        Assert.Contains("fb-drop-left", ReceiverElement(cut, 1).QuerySelector(".fb-drop-feedback")!.ClassList);
    }

    [Fact]
    public async Task A_forbidden_zone_shows_the_refusal_and_lets_nothing_in()
    {
        _state.Previewer = (_, _, _) => FbDropFeedback.Forbidden;
        var cut = RenderArea();
        await StartAsync(cut);

        await Zone(cut, 0, "center").TriggerEventAsync("ondragenter", new DragEventArgs());

        var feedback = ReceiverElement(cut, 0).QuerySelector(".fb-drop-feedback")!.ClassList;
        Assert.Contains("fb-drop-forbidden", feedback);
        Assert.Contains("fb-v-error", feedback);
        Assert.Contains("fb-drop-refused", Zone(cut, 0, "center").ClassList);
        Assert.DoesNotContain("fb-drop-refused", Zone(cut, 0, "top").ClassList);
    }

    [Fact]
    public async Task A_drop_on_an_accepting_zone_ends_the_drag_and_sends_the_drop()
    {
        _state.Previewer = (_, _, zone) => zone == FbDropZone.Bottom ? FbDropFeedback.Center : FbDropFeedback.Top;
        var cut = RenderArea();
        await StartAsync(cut);

        await Zone(cut, 0, "bottom").TriggerEventAsync("ondragenter", new DragEventArgs());
        await Zone(cut, 0, "bottom").TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.Equal([new FbDrop("item", "A", FbDropZone.Bottom, FbDropFeedback.Center)], _state.Drops);
        Assert.Empty(cut.FindAll(".fb-drop-zones"));
        Assert.False(cut.Instance.IsDragging);
    }

    [Theory]
    [InlineData(FbDropFeedback.None)]
    [InlineData(FbDropFeedback.Forbidden)]
    public async Task A_refused_drop_sends_nothing(FbDropFeedback refusal)
    {
        _state.Previewer = (_, _, _) => refusal;
        var cut = RenderArea();
        await StartAsync(cut);

        await Zone(cut, 0, "top").TriggerEventAsync("ondragenter", new DragEventArgs());
        await Zone(cut, 0, "top").TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.Empty(_state.Drops);
        Assert.Empty(cut.FindAll(".fb-drop-zones"));
    }

    [Fact]
    public async Task The_end_of_the_drag_closes_the_zones()
    {
        var cut = RenderArea();
        await StartAsync(cut);
        Assert.NotEmpty(cut.FindAll(".fb-drop-zones"));

        await cut.Find(".fb-draggable").TriggerEventAsync("ondragend", new DragEventArgs());

        Assert.Empty(cut.FindAll(".fb-drop-zones"));
        Assert.Null(cut.Instance.DragId);
    }

    [Fact]
    public async Task The_information_line_follows_the_drag()
    {
        var cut = RenderArea(showInfo: true);
        Assert.Contains("rest", ReceiverElement(cut, 0).QuerySelector(".fb-drop-info")!.TextContent);

        await StartAsync(cut);
        await Zone(cut, 0, "center").TriggerEventAsync("ondragenter", new DragEventArgs());

        Assert.Contains("Center → Center", ReceiverElement(cut, 0).QuerySelector(".fb-drop-info")!.TextContent);
        Assert.Contains("accepts", ReceiverElement(cut, 1).QuerySelector(".fb-drop-info")!.TextContent);
        Assert.Contains("item", cut.Markup);
    }

    [Fact]
    public void A_receiver_leaves_its_area_when_it_goes()
    {
        var cut = RenderArea();
        Assert.Equal(2, cut.Instance.ReceiverCount);

        cut.Render(p => p.AddChildContent(builder => Receiver(builder, 0, "A", FbDropSplit.Center)));

        Assert.Equal(1, cut.Instance.ReceiverCount);
    }

    // ------------------------------------------------------------------ Splits

    public static TheoryData<FbDropSplit, string[]> Splits => new()
    {
        { FbDropSplit.Center, ["center"] },
        { FbDropSplit.TopBottom, ["top", "bottom"] },
        { FbDropSplit.TopCenterBottom, ["top", "center", "bottom"] },
        { FbDropSplit.LeftTopCenterBottomRight, ["left", "top", "center", "bottom", "right"] },
        { FbDropSplit.LeftTopBottomRight, ["left", "top", "bottom", "right"] },
        { FbDropSplit.LeftCenterRight, ["left", "center", "right"] },
        { FbDropSplit.LeftRight, ["left", "right"] }
    };

    [Theory]
    [MemberData(nameof(Splits))]
    public async Task Each_split_lays_out_its_zones(FbDropSplit split, string[] zones)
    {
        var cut = Render<FbDragAndDropMaster>(p => p.Add(x => x.Handler, _state).AddChildContent(builder =>
        {
            builder.OpenComponent<FbDraggable>(0);
            builder.AddComponentParameter(1, nameof(FbDraggable.DragId), "item");
            builder.CloseComponent();
            Receiver(builder, 10, "A", split);
        }));

        await StartAsync(cut);

        Assert.Equal(zones, cut.FindAll(".fb-drop-zone").Select(zone => zone.ClassList[1]["fb-drop-zone-".Length..]));
    }

    public static TheoryData<string, FbDropSplit> Flags => new()
    {
        { nameof(FbDraggableReceiver.ZC), FbDropSplit.Center },
        { nameof(FbDraggableReceiver.ZTB), FbDropSplit.TopBottom },
        { nameof(FbDraggableReceiver.ZTCB), FbDropSplit.TopCenterBottom },
        { nameof(FbDraggableReceiver.ZLTCBR), FbDropSplit.LeftTopCenterBottomRight },
        { nameof(FbDraggableReceiver.ZLTBR), FbDropSplit.LeftTopBottomRight },
        { nameof(FbDraggableReceiver.ZLCR), FbDropSplit.LeftCenterRight },
        { nameof(FbDraggableReceiver.ZLR), FbDropSplit.LeftRight }
    };

    [Theory]
    [MemberData(nameof(Flags))]
    public void A_zone_flag_writes_its_split_and_falls_back_to_the_default(string flag, FbDropSplit split)
    {
        var cut = Render<FbDraggableReceiver>(p => p.Add(x => x.DropId, "A").Add(Flag(flag), true));
        Assert.Equal(split, cut.Instance.Split);

        cut.Render(p => p.Add(x => x.DropId, "A").Add(Flag(flag), false));

        Assert.Equal(FbDropSplit.TopCenterBottom, cut.Instance.Split);
    }

    private static System.Linq.Expressions.Expression<Func<FbDraggableReceiver, bool>> Flag(string name)
    {
        var receiver = System.Linq.Expressions.Expression.Parameter(typeof(FbDraggableReceiver));
        return System.Linq.Expressions.Expression.Lambda<Func<FbDraggableReceiver, bool>>(
            System.Linq.Expressions.Expression.Property(receiver, name), receiver);
    }

    // ------------------------------------------------------------------ Script

    /// <summary>
    /// The components and the delegated listeners of the Layout script meet on class names
    /// only: each one the script relies on is one the components render.
    /// </summary>
    [Theory]
    [InlineData("'.fb-drag-handle'")]
    [InlineData("'.fb-draggable'")]
    [InlineData("'.fb-drop-zone'")]
    [InlineData("'fb-drop-refused'")]
    [InlineData("'fb-dragging'")]
    public void The_script_knows_the_classes_of_the_components(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        var script = File.ReadAllText(Path.Combine(directory!.FullName, "FractalBlazor.Components.Layout", "wwwroot", "fb-components-layout.js"));

        Assert.Contains(name, script);
    }

    /// <summary>
    /// A handler without rules, which takes the drops only.
    /// </summary>
    public sealed class DropsOnlyHandler
    {
        public List<FbDrop> Drops { get; } = [];

        public void Handle(FbDrop drop) => Drops.Add(drop);
    }
}
