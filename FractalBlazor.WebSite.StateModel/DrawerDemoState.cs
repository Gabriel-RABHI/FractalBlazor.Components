using System.ComponentModel.DataAnnotations;
using FractalBlazor.Components.Common.Contracts;

namespace FractalBlazor.WebSite.StateModel
{
    /// <summary>
    /// The drawer of the layout page: where it opens, from which edge, and whether it is open.
    /// </summary>
    public sealed class DrawerDemoState : PopupState
    {
        public sealed record SetMode(DrawerMode Value) : IStateAction<DrawerMode>;

        public sealed record SetSide(DrawerSide Value) : IStateAction<DrawerSide>;

        public static IReadOnlyList<DrawerMode> Modes { get; } = Enum.GetValues<DrawerMode>();

        public static IReadOnlyList<DrawerSide> Sides { get; } = Enum.GetValues<DrawerSide>();

        public DrawerMode Mode { get; private set; } = DrawerMode.Inline;

        public DrawerSide Side { get; private set; } = DrawerSide.Left;

        /// <summary>
        /// Top and bottom drawers take a height, the others a width.
        /// </summary>
        public bool IsHorizontal => Side is DrawerSide.Left or DrawerSide.Right;

        /// <summary>
        /// Another mode closes the drawer: a modal one only opens from its button.
        /// </summary>
        public void Handle(SetMode action)
        {
            if (Mode == action.Value)
                return;

            Mode = action.Value;
            Close();
            NotifyStateChanged();
        }

        public void Handle(SetSide action)
        {
            if (Side == action.Value)
                return;

            Side = action.Value;
            NotifyStateChanged();
        }
    }

    public enum DrawerMode
    {
        [Display(Name = "Modal")]
        Modal,

        [Display(Name = "En place")]
        Inline,

        [Display(Name = "En place, par-dessus")]
        InlineModal
    }

    public enum DrawerSide
    {
        [Display(Name = "Gauche")]
        Left,

        [Display(Name = "Droite")]
        Right,

        [Display(Name = "Haut")]
        Top,

        [Display(Name = "Bas")]
        Bottom
    }
}
