using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FractalBlazor.Components.Layout.Utilities
{
    /// <summary>
    /// Process-wide cache of computed inline styles. The key is the complete value of an
    /// unmanaged state struct, compared byte by byte: two different states never share a
    /// style, and two identical states share the same string instance.
    /// </summary>
    /// <remarks>
    /// Only styles built from enumerated values are cached, so the number of keys is bounded
    /// by the combinations actually used. <see cref="Capacity"/> is a safety net: once it is
    /// reached, new styles are still computed but no longer shared.
    /// </remarks>
    internal static class FbStyleCache<TKey> where TKey : unmanaged
    {
        internal const int Capacity = 4096;

        private static readonly ConcurrentDictionary<FbStateKey<TKey>, string> _styles = new();

        internal static int Count => _styles.Count;

        public static bool TryGet(in TKey key, [NotNullWhen(true)] out string? style)
            => _styles.TryGetValue(new FbStateKey<TKey>(key), out style);

        /// <summary>
        /// Stores a style and returns the shared instance, or returns <paramref name="style"/>
        /// unchanged when the cache is full.
        /// </summary>
        public static string Add(in TKey key, string style)
        {
            if (_styles.Count >= Capacity)
                return style;
            return _styles.GetOrAdd(new FbStateKey<TKey>(key), style);
        }

        internal static void Clear() => _styles.Clear();
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
    /// Free-form strings that feed an inline style. A style built from any of them is
    /// specific to one component and never enters <see cref="FbStyleCache{TKey}"/>.
    /// </summary>
    internal readonly struct FbStyleInputs
    {
        private readonly string? _a, _b, _c, _d, _e, _f;

        public FbStyleInputs(string? a, string? b, string? c = null, string? d = null, string? e = null, string? f = null)
        {
            _a = a; _b = b; _c = c; _d = d; _e = e; _f = f;
        }

        public bool IsEmpty
            => string.IsNullOrEmpty(_a) && string.IsNullOrEmpty(_b) && string.IsNullOrEmpty(_c)
            && string.IsNullOrEmpty(_d) && string.IsNullOrEmpty(_e) && string.IsNullOrEmpty(_f);

        public bool Equals(in FbStyleInputs other)
            => string.Equals(_a, other._a, StringComparison.Ordinal)
            && string.Equals(_b, other._b, StringComparison.Ordinal)
            && string.Equals(_c, other._c, StringComparison.Ordinal)
            && string.Equals(_d, other._d, StringComparison.Ordinal)
            && string.Equals(_e, other._e, StringComparison.Ordinal)
            && string.Equals(_f, other._f, StringComparison.Ordinal);
    }

    /// <summary>
    /// Remembers the last style computed by one component, so that a render with unchanged
    /// parameters returns the previous string without building or looking anything up.
    /// </summary>
    internal struct FbStyleMemo
    {
        private object? _key;
        private FbStyleInputs _inputs;
        private string? _style;

        public bool TryGet<TKey>(in TKey key, in FbStyleInputs inputs, [NotNullWhen(true)] out string? style)
            where TKey : unmanaged
        {
            if (_style is not null && _key is TKey last && FbState.Equal(in last, in key) && _inputs.Equals(in inputs))
            {
                style = _style;
                return true;
            }

            style = null;
            return false;
        }

        public void Set<TKey>(in TKey key, in FbStyleInputs inputs, string style) where TKey : unmanaged
        {
            _key = key;
            _inputs = inputs;
            _style = style;
        }
    }
}
