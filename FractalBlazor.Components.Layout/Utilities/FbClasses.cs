using System.Text;

namespace FractalBlazor.Components.Layout.Utilities
{
    /// <summary>
    /// Builds a <c>class</c> attribute from optional parts, separated by single spaces.
    /// Only called when a class string is not already cached.
    /// </summary>
    internal static class FbClasses
    {
        public static string Join(string? a, string? b, string? c = null, string? d = null, string? e = null, string? f = null)
        {
            var builder = new StringBuilder(64);
            Append(builder, a);
            Append(builder, b);
            Append(builder, c);
            Append(builder, d);
            Append(builder, e);
            Append(builder, f);
            return builder.ToString();
        }

        private static void Append(StringBuilder builder, string? part)
        {
            if (string.IsNullOrWhiteSpace(part))
                return;
            if (builder.Length > 0)
                builder.Append(' ');
            builder.Append(part.Trim());
        }
    }
}
