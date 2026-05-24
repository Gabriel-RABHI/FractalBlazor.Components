using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  FractalBlazor.Components.Layout
{

    /*.container {
        display: flex;  or inline-flex 
    }
    
    bool IsInline

     */
    public enum FbFlexDisplay : byte
    {
        Flex,
        InlineFlex
    }

    // https://the-echoplex.net/flexyboxes/
    /*
    bool JustifyStart

    FbFlexJustify Justify 

    */
    public enum FbFlexJustify : byte
    {
        Start,
        End,
        Center,
        Stretch,
        SpaceBetween,
        SpaceAround,
        Evenly
    }

    /*.container {
        flex-direction: row | row-reverse | column | column-reverse;
    }
    
    bool DirectionRow;
     
     */
    public enum FbFlexDirection : byte
    {
        Row,
        RowReverse,
        Column,
        ColumnReverse
    }

    /*.container {
        flex-wrap: nowrap | wrap | wrap-reverse;
    }
    
    bool WrapNone
    bool WrapWrap
    bool WrapReverse

    FbFlexWrap Wrap
     
     */

    public enum FbFlexWrap : byte
    {
        NoWrap,
        Wrap,
        WrapReverse
    }

    /*.container {
        align-items: stretch | flex-start | flex-end | center | baseline | first baseline | last baseline | start | end | self-start | self-end + ... safe | unsafe;
    }
    
    AlignItems
     
     */
    public enum FbFlexAlignItems : byte
    {
        Start,
        End,
        Center,
        Baseline,
        Stretch,
    }

    /*.container {
        align-content: flex-start | flex-end | center | space-between | space-around | space-evenly | stretch | start | end | baseline | first baseline | last baseline + ... safe | unsafe;
    }*/
    public enum FbFlexAlignContent : byte
    {
        Start,
        End,
        Center,
        SpaceBetween,
        SpaceAround,
        Stretch,
    }

    /*
    PlaceItems

     */
    public enum FbFlexPlaceItems : byte
    {
        None,
        Center,
        Normal,
        Start,
        End,
        SelfStart,
        SelfEnd,
        FlexStart,
        FlexEnd,
        Baseline,
        Stretch
    }


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


    public enum FbFlexMasterSlave : byte
    {
        None,
        Master,
        Slave
    }
    public enum FbFlexSize : byte
    {
        None,
        MaxContent,
        MinContent
    }

    public enum BaseDisplayMode : byte
    {
        None,
        Block,
        InlineBloc,
        Flex,
        InlineFlex,
        Table,
        TableRow,
        TableCell
    }
}
