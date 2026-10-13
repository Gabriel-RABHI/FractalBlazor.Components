using System.Text;

namespace FractalBlazor.Components.Layout.Utilities
{
    /// <summary>
    /// Writes the section of <c>fb-components-layout.css</c> that defines the atoms: the base
    /// rules of rows and stacks first, then one rule per atom, so that an atom overrides the
    /// base rule of its component. The section sits between <see cref="Begin"/> and
    /// <see cref="End"/>, at the end of the stylesheet.
    /// </summary>
    internal static class FbAtomStylesheet
    {
        public const string Begin = "/* fb-atoms:begin */";
        public const string End = "/* fb-atoms:end */";

        /// <summary>
        /// Path of the stylesheet, relative to the repository root: the project folder is
        /// named after the assembly.
        /// </summary>
        public static string RelativePath { get; } =
            Path.Combine(typeof(FbAtomStylesheet).Assembly.GetName().Name!, "wwwroot", "fb-components-layout.css");

        /// <summary>
        /// The section, markers included, with <c>\n</c> line ends.
        /// </summary>
        public static string Section()
        {
            var css = new StringBuilder();
            css.Append(Begin).Append('\n');
            css.Append(".fb-row{").Append(FbRow.Baseline.Declarations()).Append("}\n");
            css.Append(".fb-stack{").Append(FbStack.Baseline.Declarations()).Append("}\n");
            for (var atom = 0; atom < FbAtoms.Names.Count; atom++)
                css.Append('.').Append(FbAtoms.Names[atom]).Append('{').Append(FbAtoms.Declarations[atom]).Append("}\n");
            css.Append(End);
            return css.ToString();
        }

        /// <summary>
        /// The section currently written in <paramref name="stylesheet"/>, with <c>\n</c>
        /// line ends, or <c>null</c> when it has no markers.
        /// </summary>
        public static string? Read(string stylesheet)
        {
            var begin = stylesheet.IndexOf(Begin, StringComparison.Ordinal);
            var end = stylesheet.IndexOf(End, StringComparison.Ordinal);
            if (begin < 0 || end < begin)
                return null;
            return stylesheet[begin..(end + End.Length)].Replace("\r\n", "\n");
        }

        /// <summary>
        /// <paramref name="stylesheet"/> with its section replaced by <see cref="Section"/>, in
        /// the line ends of the stylesheet.
        /// </summary>
        public static string Update(string stylesheet)
        {
            var begin = stylesheet.IndexOf(Begin, StringComparison.Ordinal);
            var end = stylesheet.IndexOf(End, StringComparison.Ordinal);
            if (begin < 0 || end < begin)
                throw new InvalidOperationException($"The stylesheet has no '{Begin}' … '{End}' section.");

            var section = Section();
            if (stylesheet.Contains("\r\n", StringComparison.Ordinal))
                section = section.Replace("\n", "\r\n");
            return string.Concat(stylesheet.AsSpan(0, begin), section, stylesheet.AsSpan(end + End.Length));
        }
    }
}
