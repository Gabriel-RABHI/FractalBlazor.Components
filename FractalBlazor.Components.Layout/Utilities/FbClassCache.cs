using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FractalBlazor.Components.Layout.Utilities
{
    /// <summary>
    /// Process-wide cache of computed <c>class</c> attributes. The key is the complete value of
    /// an unmanaged state struct, compared byte by byte: two different states never share
    /// classes, and two identical states share the same string instance.
    /// </summary>
    /// <remarks>
    /// Only classes built from enumerated values are cached, free-form classes being appended
    /// by each component, so the number of keys is bounded by the combinations actually used.
    /// <see cref="Capacity"/> is a safety net: once it is reached, new classes are still
    /// computed but no longer shared.
    /// </remarks>
    internal static class FbClassCache<TKey> where TKey : unmanaged
    {
        internal const int Capacity = 4096;

        private static readonly ConcurrentDictionary<FbStateKey<TKey>, string> _classes = new();

        internal static int Count => _classes.Count;

        public static bool TryGet(in TKey key, [NotNullWhen(true)] out string? classes)
            => _classes.TryGetValue(new FbStateKey<TKey>(key), out classes);

        /// <summary>
        /// Stores classes and returns the shared instance, or returns <paramref name="classes"/>
        /// unchanged when the cache is full.
        /// </summary>
        public static string Add(in TKey key, string classes)
        {
            if (_classes.Count >= Capacity)
                return classes;
            return _classes.GetOrAdd(new FbStateKey<TKey>(key), classes);
        }

        internal static void Clear() => _classes.Clear();
    }

    /// <summary>
    /// Value equality over the raw bytes of an unmanaged state. State structs use
    /// <c>Pack = 1</c>, so they contain no padding bytes.
    /// </summary>
    internal static class FbState
    {
        public static bool Equal<T>(in T left, in T right) where T : unmanaged
            => Bytes(in left).SequenceEqual(Bytes(in right));

        public static int Hash<T>(in T state) where T : unmanaged
        {
            var hash = new HashCode();
            hash.AddBytes(Bytes(in state));
            return hash.ToHashCode();
        }

        private static ReadOnlySpan<byte> Bytes<T>(in T state) where T : unmanaged
            => MemoryMarshal.AsBytes(new ReadOnlySpan<T>(in state));
    }

    internal readonly struct FbStateKey<T> : IEquatable<FbStateKey<T>> where T : unmanaged
    {
        private readonly T _state;
        private readonly int _hash;

        public FbStateKey(in T state)
        {
            _state = state;
            _hash = FbState.Hash(in state);
        }

        public bool Equals(FbStateKey<T> other)
            => _hash == other._hash && FbState.Equal(in _state, in other._state);

        public override bool Equals(object? obj) => obj is FbStateKey<T> other && Equals(other);

        public override int GetHashCode() => _hash;
    }

    /// <summary>
    /// Free-form strings that feed an attribute, compared by value to know whether the
    /// attribute of the last render still holds. They never enter
    /// <see cref="FbClassCache{TKey}"/>.
    /// </summary>
    internal readonly struct FbStyleInputs
    {
        private readonly string? _a, _b, _c, _d, _e, _f, _g;

        public FbStyleInputs(string? a, string? b, string? c = null, string? d = null, string? e = null, string? f = null, string? g = null)
        {
            _a = a; _b = b; _c = c; _d = d; _e = e; _f = f; _g = g;
        }

        public bool IsEmpty
            => string.IsNullOrEmpty(_a) && string.IsNullOrEmpty(_b) && string.IsNullOrEmpty(_c)
            && string.IsNullOrEmpty(_d) && string.IsNullOrEmpty(_e) && string.IsNullOrEmpty(_f)
            && string.IsNullOrEmpty(_g);

        public bool Equals(in FbStyleInputs other)
            => string.Equals(_a, other._a, StringComparison.Ordinal)
            && string.Equals(_b, other._b, StringComparison.Ordinal)
            && string.Equals(_c, other._c, StringComparison.Ordinal)
            && string.Equals(_d, other._d, StringComparison.Ordinal)
            && string.Equals(_e, other._e, StringComparison.Ordinal)
            && string.Equals(_f, other._f, StringComparison.Ordinal)
            && string.Equals(_g, other._g, StringComparison.Ordinal);
    }

    /// <summary>
    /// Remembers the last attribute computed by one component, so that a render with unchanged
    /// parameters returns the previous value, possibly <c>null</c>, without building or
    /// looking anything up.
    /// </summary>
    internal struct FbStyleMemo
    {
        private object? _key;
        private FbStyleInputs _inputs;
        private string? _value;

        public bool TryGet<TKey>(in TKey key, in FbStyleInputs inputs, out string? value)
            where TKey : unmanaged
        {
            if (_key is TKey last && FbState.Equal(in last, in key) && _inputs.Equals(in inputs))
            {
                value = _value;
                return true;
            }

            value = null;
            return false;
        }

        public void Set<TKey>(in TKey key, in FbStyleInputs inputs, string? value) where TKey : unmanaged
        {
            _key = key;
            _inputs = inputs;
            _value = value;
        }
    }

    /// <summary>
    /// Builds a <c>style</c> attribute from optional declarations, each ended by a semicolon,
    /// or <c>null</c> when there is none, so that no empty attribute is rendered.
    /// </summary>
    internal static class FbInlineStyle
    {
        public static string? Join(params ReadOnlySpan<string?> parts)
        {
            System.Text.StringBuilder? builder = null;
            foreach (var part in parts)
            {
                var text = part?.Trim();
                if (string.IsNullOrEmpty(text))
                    continue;
                builder ??= new System.Text.StringBuilder(64);
                builder.Append(text);
                if (text[^1] != ';')
                    builder.Append(';');
            }
            return builder?.ToString();
        }
    }
}
