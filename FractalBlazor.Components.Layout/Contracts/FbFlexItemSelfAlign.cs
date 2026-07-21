namespace  FractalBlazor.Components.Layout
{
    /*.container {
    display: flex;
    ...
    gap: 10px;
    gap: 10px 20px; -- row-gap column gap --
    row-gap: 10px;
    column-gap: 20px;
    }*/




    /*.item {
        align-self: auto | flex-start | flex-end | center | baseline | stretch;
    }*/
    public enum FbFlexItemSelfAlign : byte
    {
        None,
        Auto,
        Start,
        End,
        Center,
        Baseline,
        Stretch
    }
}
