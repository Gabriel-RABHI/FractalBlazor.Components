using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The states of the drag and drop demos: what their rules answer, and what a drop changes.
/// </summary>
public sealed class DragDropTests
{
    [Fact]
    public void A_demo_keeps_its_last_drop_and_notifies()
    {
        var demo = new DragDropDemoState();
        var notifications = 0;
        demo.OnStateChanged += () => notifications++;

        demo.Handle(new FbDrop("Élément", "Receveur", FbDropZone.Top, FbDropFeedback.Top));
        demo.Handle(new FbDrop("Élément", "Receveur", FbDropZone.Center, FbDropFeedback.Center));

        Assert.Equal(new FbDrop("Élément", "Receveur", FbDropZone.Center, FbDropFeedback.Center), demo.LastDrop);
        Assert.Equal(2, demo.DropCount);
        Assert.Equal(2, notifications);
    }

    [Fact]
    public void The_content_of_the_items_stays_alive()
    {
        var demo = new DragDropDemoState();

        demo.Handle(new DragDropDemoState.Click());
        demo.Handle(new DragDropDemoState.SetNote("Bonjour"));

        Assert.Equal(1, demo.Clicks);
        Assert.Equal("Bonjour", demo.Note);
    }

    [Fact]
    public void Only_the_compatible_item_suits_the_receiver()
    {
        var demo = new CompatibilityDemoState();

        Assert.True(demo.Accepts(CompatibilityDemoState.Compatible, CompatibilityDemoState.Receiver));
        Assert.False(demo.Accepts(CompatibilityDemoState.Incompatible, CompatibilityDemoState.Receiver));
    }

    [Fact]
    public void Each_item_asks_for_its_own_place_and_the_preview_is_kept()
    {
        var demo = new PreviewDemoState();

        Assert.All(PreviewDemoState.Items, item =>
            Assert.Equal(item.Feedback, demo.Preview(item.Name, PreviewDemoState.Receiver, FbDropZone.Bottom)));
        Assert.Equal(new DropPreview("En haut", PreviewDemoState.Receiver, FbDropZone.Bottom, FbDropFeedback.Top), demo.LastPreview);
    }

    [Fact]
    public void Each_zone_of_a_split_offers_its_natural_place()
    {
        var demo = new SplitDemoState();

        Assert.Equal(FbDropFeedback.Left, demo.Preview(SplitDemoState.Item, "ZLR", FbDropZone.Left));
        Assert.Equal(FbDropFeedback.Center, demo.Preview(SplitDemoState.Item, "ZC", FbDropZone.Center));
    }

    [Theory]
    [InlineData(FbDropFeedback.Top, new[] { "Élément 2", "Élément 4", "Élément 1", "Élément 3" })]
    [InlineData(FbDropFeedback.Bottom, new[] { "Élément 2", "Élément 1", "Élément 4", "Élément 3" })]
    [InlineData(FbDropFeedback.Left, new[] { "Élément 2", "Élément 4", "Élément 1", "Élément 3" })]
    [InlineData(FbDropFeedback.Right, new[] { "Élément 2", "Élément 1", "Élément 4", "Élément 3" })]
    public void A_dropped_item_goes_before_or_after_its_receiver(FbDropFeedback feedback, string[] order)
    {
        var list = new ReorderListState();
        var notifications = 0;
        list.OnStateChanged += () => notifications++;
        list.Handle(new FbDrop(list.Items[0], list.Items[1], FbDropZone.Bottom, FbDropFeedback.Bottom));

        list.Handle(new FbDrop(list.Items[3], list.Items[1], FbDropZone.Center, feedback));

        Assert.Equal(order, list.Items.Select(item => item.Name));
        Assert.Equal(2, notifications);
    }

    [Fact]
    public void An_item_over_itself_goes_nowhere()
    {
        var list = new ReorderListState();

        Assert.Equal(FbDropFeedback.None, list.Preview(list.Items[0], list.Items[0], FbDropZone.Top));
        Assert.Equal(FbDropFeedback.Top, list.Preview(list.Items[0], list.Items[1], FbDropZone.Top));
    }
}
