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
    public enum TinyFlexDisplay : byte
    {
        Flex,
        InlineFlex
    }

    // https://the-echoplex.net/flexyboxes/
    /*
    bool JustifyStart

    TinyFlexJustify Justify 

    */
    public enum TinyFlexJustify : byte
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
    public enum TinyFlexDirection : byte
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

    TinyFlexWrap Wrap
     
     */

    public enum TinyFlexWrap : byte
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
    public enum TinyFlexAlignItems : byte
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
    public enum TinyFlexAlignContent : byte
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
    public enum TinyFlexPlaceItems : byte
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
    public enum TinyFlexItemSelfAlign : byte
    {
        None,
        Auto,
        Start,
        End,
        Center,
        Baseline,
        Stretch
    }


    public enum TinyFlexMasterSlave : byte
    {
        None,
        Master,
        Slave
    }
    public enum TinyFlexSize : byte
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
