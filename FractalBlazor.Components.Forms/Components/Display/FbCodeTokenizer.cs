namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// The kind of a piece of code, which gives it its color.
    /// </summary>
    internal enum FbCodeTokenKind : byte
    {
        Plain,
        Keyword,
        Type,
        Function,
        String,
        Number,
        Comment,
        Punctuation,
        Tag,
        Attribute,
        Razor,
        Property,
        Selector,
        Variable
    }

    /// <summary>
    /// A piece of code and its kind.
    /// </summary>
    internal readonly record struct FbCodeToken(FbCodeTokenKind Kind, string Text);

    /// <summary>
    /// Splits code into colored pieces for <see cref="FbCodeBlock"/>. A light lexer, not a
    /// parser: it knows the words, strings, numbers and comments of each language, and the tags
    /// of Razor. Code it does not understand stays plain, never lost: the pieces always join back
    /// into the code.
    /// </summary>
    internal sealed class FbCodeTokenizer
    {
        private static readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> CSharpKeywords = new HashSet<string>(
        [
            "abstract", "and", "as", "async", "await", "base", "bool", "break", "byte", "case", "catch", "char",
            "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "dynamic",
            "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "get", "global", "goto", "if", "implicit", "in", "init", "int", "interface", "internal", "is", "lock",
            "long", "nameof", "namespace", "new", "not", "null", "object", "operator", "or", "out", "override",
            "params", "partial", "private", "protected", "public", "readonly", "record", "ref", "required",
            "return", "sbyte", "sealed", "set", "short", "sizeof", "stackalloc", "static", "string", "struct",
            "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
            "using", "var", "virtual", "void", "volatile", "when", "where", "while", "with", "yield"
        ], StringComparer.Ordinal).GetAlternateLookup<ReadOnlySpan<char>>();

        /// <summary>
        /// The words after <c>@</c> that open a directive or a block: the rest of their line is C#.
        /// </summary>
        private static readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> RazorDirectives = new HashSet<string>(
        [
            "attribute", "await", "code", "do", "else", "for", "foreach", "functions", "if", "implements",
            "inherits", "inject", "layout", "lock", "model", "namespace", "page", "preservewhitespace",
            "rendermode", "section", "switch", "try", "typeparam", "using", "while"
        ], StringComparer.Ordinal).GetAlternateLookup<ReadOnlySpan<char>>();

        private readonly string _code;
        private readonly List<FbCodeToken> _tokens = [];
        private FbCodeTokenKind _pendingKind;
        private int _pendingStart, _pendingEnd;

        private FbCodeTokenizer(string code) => _code = code;

        /// <summary>
        /// The pieces of <paramref name="code"/> in <paramref name="language"/>; two neighbors
        /// never have the same kind.
        /// </summary>
        public static IReadOnlyList<FbCodeToken> Tokenize(string? code, FbCodeLanguage language)
        {
            if (string.IsNullOrEmpty(code))
                return [];

            var tokenizer = new FbCodeTokenizer(code);
            switch (language)
            {
                case FbCodeLanguage.Razor:
                    tokenizer.Razor();
                    break;
                case FbCodeLanguage.CSharp:
                    tokenizer.CSharp(0, code.Length);
                    break;
                case FbCodeLanguage.Css:
                    tokenizer.Css();
                    break;
                default:
                    tokenizer.Add(FbCodeTokenKind.Plain, 0, code.Length);
                    break;
            }
            tokenizer.Flush();
            return tokenizer._tokens;
        }

        // ------------------------------------------------------------------ Pieces

        private void Add(FbCodeTokenKind kind, int start, int end)
        {
            if (end <= start)
                return;
            if (_pendingEnd > _pendingStart && kind == _pendingKind && start == _pendingEnd)
            {
                _pendingEnd = end;
                return;
            }
            Flush();
            (_pendingKind, _pendingStart, _pendingEnd) = (kind, start, end);
        }

        private void Flush()
        {
            if (_pendingEnd > _pendingStart)
                _tokens.Add(new FbCodeToken(_pendingKind, _code[_pendingStart.._pendingEnd]));
            _pendingStart = _pendingEnd = 0;
        }

        private char At(int index) => index < _code.Length ? _code[index] : '\0';

        private bool StartsWith(int index, string text) => string.CompareOrdinal(_code, index, text, 0, text.Length) == 0;

        private int Skip(int index, int end, Func<char, bool> keep)
        {
            while (index < end && keep(_code[index]))
                index++;
            return index;
        }

        private int LineEnd(int index, int end)
        {
            var newline = _code.IndexOf('\n', index, end - index);
            return newline < 0 ? end : newline;
        }

        /// <summary>
        /// The end of <paramref name="close"/> from <paramref name="index"/>, or the end of the
        /// range when it never comes.
        /// </summary>
        private int After(string close, int index, int end)
        {
            var found = index < end ? _code.IndexOf(close, index, end - index, StringComparison.Ordinal) : -1;
            return found < 0 ? end : found + close.Length;
        }

        private static bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_';

        private static bool IsIdentifierPart(char c) => char.IsLetterOrDigit(c) || c == '_';

        // ------------------------------------------------------------------ C#

        private void CSharp(int start, int end)
        {
            var i = start;
            while (i < end)
            {
                var c = _code[i];
                int next;
                if (char.IsWhiteSpace(c))
                {
                    next = Skip(i, end, char.IsWhiteSpace);
                    Add(FbCodeTokenKind.Plain, i, next);
                }
                else if (c == '/' && At(i + 1) == '/')
                {
                    next = LineEnd(i, end);
                    Add(FbCodeTokenKind.Comment, i, next);
                }
                else if (c == '/' && At(i + 1) == '*')
                {
                    next = After("*/", i + 2, end);
                    Add(FbCodeTokenKind.Comment, i, next);
                }
                else if (StringEnd(i, end) is var stringEnd and > 0)
                {
                    next = stringEnd;
                    Add(FbCodeTokenKind.String, i, next);
                }
                else if (char.IsDigit(c))
                {
                    next = Skip(i, end, ch => char.IsLetterOrDigit(ch) || ch is '_');
                    if (At(next) == '.' && char.IsDigit(At(next + 1)))
                        next = Skip(next + 1, end, ch => char.IsLetterOrDigit(ch) || ch is '_');
                    Add(FbCodeTokenKind.Number, i, next);
                }
                else if (IsIdentifierStart(c))
                {
                    next = Skip(i, end, IsIdentifierPart);
                    Add(Word(i, next, end), i, next);
                }
                else
                {
                    next = i + 1;
                    Add(FbCodeTokenKind.Punctuation, i, next);
                }
                i = next;
            }
        }

        /// <summary>
        /// A keyword, a call (a name before a parenthesis), a type (a name in capitals) or a
        /// plain name.
        /// </summary>
        private FbCodeTokenKind Word(int start, int end, int limit)
        {
            if (CSharpKeywords.Contains(_code.AsSpan(start, end - start)))
                return FbCodeTokenKind.Keyword;
            if (At(Skip(end, limit, ch => ch is ' ' or '\t')) == '(')
                return FbCodeTokenKind.Function;
            return char.IsUpper(_code[start]) ? FbCodeTokenKind.Type : FbCodeTokenKind.Plain;
        }

        /// <summary>
        /// The end of the string or character literal at <paramref name="start"/>, or -1 when
        /// none starts there: <c>"…"</c>, <c>@"…"</c>, <c>$"…"</c>, <c>"""…"""</c>, <c>'x'</c>.
        /// </summary>
        private int StringEnd(int start, int end)
        {
            var i = start;
            var verbatim = false;
            while (i < end && i - start < 3 && _code[i] is '$' or '@')
            {
                verbatim |= _code[i] == '@';
                i++;
            }

            if (At(i) == '\'' && i == start)
            {
                var close = At(i + 1) == '\\' ? _code.IndexOf('\'', Math.Min(i + 2, _code.Length - 1)) : i + 2;
                return close > i && close < end && close - i <= 8 && At(close) == '\'' ? close + 1 : -1;
            }
            if (At(i) != '"')
                return -1;

            if (StartsWith(i, "\"\"\""))
            {
                var quotes = Skip(i, end, ch => ch == '"') - i;
                var close = _code.IndexOf(new string('"', quotes), i + quotes, StringComparison.Ordinal);
                return close < 0 || close >= end ? end : close + quotes;
            }

            for (var j = i + 1; j < end; j++)
            {
                var c = _code[j];
                if (verbatim && c == '"' && At(j + 1) == '"')
                    j++;
                else if (!verbatim && c == '\\')
                    j++;
                else if (c == '"')
                    return j + 1;
                else if (!verbatim && c == '\n')
                    return j;
            }
            return end;
        }

        // ------------------------------------------------------------------ Razor

        private enum Line : byte
        {
            /// <summary>The start of a line, whose first sign says what it holds.</summary>
            Start,

            /// <summary>The text of an element, after a tag on the same line.</summary>
            Text,

            /// <summary>C#, after a directive or on a line without markup.</summary>
            Code
        }

        private void Razor()
        {
            var end = _code.Length;
            var line = Line.Start;
            var i = 0;
            while (i < end)
            {
                var c = _code[i];
                if (c == '\n')
                {
                    Add(FbCodeTokenKind.Plain, i, i + 1);
                    line = Line.Start;
                    i++;
                }
                else if (line == Line.Start && char.IsWhiteSpace(c))
                {
                    var next = Skip(i, end, ch => ch is ' ' or '\t' or '\r');
                    Add(FbCodeTokenKind.Plain, i, next);
                    i = next;
                }
                else if (c == '@' && At(i + 1) == '*')
                {
                    var next = After("*@", i + 2, end);
                    Add(FbCodeTokenKind.Comment, i, next);
                    i = next;
                }
                else if (c == '<' && StartsWith(i, "<!--"))
                {
                    var next = After("-->", i + 4, end);
                    Add(FbCodeTokenKind.Comment, i, next);
                    i = next;
                }
                else if (c == '<' && IsTagStart(i, line))
                {
                    i = Tag(i, end);
                    line = Line.Text;
                }
                else if (c == '@' && (i == 0 || !IsIdentifierPart(_code[i - 1])))
                {
                    i = Transition(i, end, ref line);
                }
                else if (line == Line.Text)
                {
                    var next = i + 1;
                    while (next < end && _code[next] is not ('\n' or '@' or '<'))
                        next++;
                    Add(FbCodeTokenKind.Plain, i, next);
                    i = next;
                }
                else
                {
                    var next = LineEnd(i, end);
                    CSharp(i, next);
                    line = Line.Code;
                    i = next;
                }
            }
        }

        /// <summary>
        /// A tag starts at a <c>&lt;</c> followed by a name. In the text of an element, or for a
        /// closing tag, that is enough; elsewhere it must follow a space, a line start or a
        /// transition, so that <c>List&lt;int&gt;</c> stays C#.
        /// </summary>
        private bool IsTagStart(int index, Line line)
        {
            var closing = At(index + 1) == '/';
            if (!char.IsLetter(closing ? At(index + 2) : At(index + 1)))
                return false;
            if (closing || line == Line.Text)
                return true;
            var before = index == 0 ? ' ' : _code[index - 1];
            return char.IsWhiteSpace(before) || before is '>' or '@' or '(' or '{' or '}' or ';' or ',';
        }

        /// <summary>
        /// A tag, its name and its attributes, up to its end, across lines.
        /// </summary>
        private int Tag(int start, int end)
        {
            var i = start + (At(start + 1) == '/' ? 2 : 1);
            Add(FbCodeTokenKind.Punctuation, start, i);

            var nameEnd = Skip(i, end, ch => IsIdentifierPart(ch) || ch is '.' or ':' or '-');
            Add(char.IsUpper(_code[i]) ? FbCodeTokenKind.Type : FbCodeTokenKind.Tag, i, nameEnd);
            i = nameEnd;

            while (i < end)
            {
                var c = _code[i];
                int next;
                if (c == '>')
                {
                    Add(FbCodeTokenKind.Punctuation, i, i + 1);
                    return i + 1;
                }
                if (c == '/' && At(i + 1) == '>')
                {
                    Add(FbCodeTokenKind.Punctuation, i, i + 2);
                    return i + 2;
                }

                if (char.IsWhiteSpace(c))
                {
                    next = Skip(i, end, char.IsWhiteSpace);
                    Add(FbCodeTokenKind.Plain, i, next);
                }
                else if (c is '"' or '\'')
                {
                    // "@(…)" may hold quotes of its own: the value ends after its parenthesis.
                    var from = At(i + 1) == '@' && At(i + 2) == '(' ? MatchingParenthesis(i + 2, end) : i + 1;
                    var close = from < end ? _code.IndexOf(c, from) : -1;
                    next = close < 0 || close >= end ? end : close + 1;
                    Add(FbCodeTokenKind.String, i, next);
                }
                else if (c == '=')
                {
                    next = i + 1;
                    Add(FbCodeTokenKind.Punctuation, i, next);
                }
                else if (c == '@' && At(i + 1) == '*')
                {
                    next = After("*@", i + 2, end);
                    Add(FbCodeTokenKind.Comment, i, next);
                }
                else
                {
                    next = i + 1;
                    while (next < end && !char.IsWhiteSpace(_code[next]) && _code[next] is not ('=' or '>' or '"' or '\'')
                           && !(_code[next] == '/' && At(next + 1) == '>'))
                        next++;
                    Add(c == '@' ? FbCodeTokenKind.Razor : FbCodeTokenKind.Attribute, i, next);
                }
                i = next;
            }
            return end;
        }

        /// <summary>
        /// A transition: <c>@@</c>, a template (<c>@&lt;</c>), a block (<c>@{</c>), an
        /// explicit expression (<c>@(…)</c>), a directive whose line goes on in C#
        /// (<c>@if</c>, <c>@code</c>…), or an implicit expression (<c>@Item.Name</c>).
        /// </summary>
        private int Transition(int start, int end, ref Line line)
        {
            var next = At(start + 1);
            if (next == '@')
            {
                Add(FbCodeTokenKind.Plain, start, start + 2);
                return start + 2;
            }
            if (next == '(')
            {
                Add(FbCodeTokenKind.Razor, start, start + 1);
                var close = MatchingParenthesis(start + 1, end);
                CSharp(start + 1, close);
                return close;
            }
            if (!IsIdentifierStart(next))
            {
                Add(FbCodeTokenKind.Razor, start, start + 1);
                return start + 1;
            }

            var word = Skip(start + 1, end, IsIdentifierPart);
            if (RazorDirectives.Contains(_code.AsSpan(start + 1, word - start - 1)))
            {
                Add(FbCodeTokenKind.Razor, start, word);
                line = Line.Code;
                var lineEnd = LineEnd(word, end);
                CSharp(word, lineEnd);
                return lineEnd;
            }

            while (At(word) == '.' && IsIdentifierStart(At(word + 1)))
                word = Skip(word + 1, end, IsIdentifierPart);
            Add(FbCodeTokenKind.Razor, start, word);
            if (At(word) == '(')
            {
                var close = MatchingParenthesis(word, end);
                CSharp(word, close);
                return close;
            }
            return word;
        }

        /// <summary>
        /// The end of the parenthesis that opens at <paramref name="open"/>, past the one that
        /// closes it; the end of the range when it never closes.
        /// </summary>
        private int MatchingParenthesis(int open, int end)
        {
            var depth = 0;
            for (var i = open; i < end; i++)
            {
                if (StringEnd(i, end) is var stringEnd and > 0)
                {
                    i = stringEnd - 1;
                    continue;
                }
                if (_code[i] == '(')
                    depth++;
                else if (_code[i] == ')' && --depth == 0)
                    return i + 1;
            }
            return end;
        }

        // ------------------------------------------------------------------ CSS

        private void Css()
        {
            var end = _code.Length;
            var i = 0;
            while (i < end)
            {
                var c = _code[i];
                if (char.IsWhiteSpace(c))
                {
                    var next = Skip(i, end, char.IsWhiteSpace);
                    Add(FbCodeTokenKind.Plain, i, next);
                    i = next;
                }
                else if (c == '/' && At(i + 1) == '*')
                {
                    var next = After("*/", i + 2, end);
                    Add(FbCodeTokenKind.Comment, i, next);
                    i = next;
                }
                else if (c is '{' or '}' or ';')
                {
                    Add(FbCodeTokenKind.Punctuation, i, i + 1);
                    i++;
                }
                else if (c == '@')
                {
                    var next = Skip(i + 1, end, ch => IsIdentifierPart(ch) || ch == '-');
                    Add(FbCodeTokenKind.Keyword, i, next);
                    i = next;
                }
                else
                {
                    var stop = StatementEnd(i, end);
                    if (At(stop) == '{')
                        Selector(i, stop);
                    else
                        Declaration(i, stop);
                    i = stop;
                }
            }
        }

        /// <summary>
        /// The <c>;</c>, <c>{</c> or <c>}</c> that ends the statement at <paramref name="start"/>,
        /// outside strings, parentheses and comments; the end of the code without one.
        /// </summary>
        private int StatementEnd(int start, int end)
        {
            var depth = 0;
            for (var i = start; i < end; i++)
            {
                var c = _code[i];
                if (c is '"' or '\'')
                {
                    var close = _code.IndexOf(c, i + 1);
                    i = close < 0 ? end : close;
                }
                else if (c == '/' && At(i + 1) == '*')
                    return i;
                else if (c == '(')
                    depth++;
                else if (c == ')')
                    depth--;
                else if (depth <= 0 && c is ';' or '{' or '}')
                    return i;
            }
            return end;
        }

        private void Selector(int start, int end)
        {
            for (var i = start; i < end; i++)
            {
                var c = _code[i];
                var kind = char.IsWhiteSpace(c) ? FbCodeTokenKind.Plain
                    : c is ',' or '>' or '+' or '~' ? FbCodeTokenKind.Punctuation
                    : FbCodeTokenKind.Selector;
                Add(kind, i, i + 1);
            }
        }

        /// <summary>
        /// A property, its colon and its value; a value alone when there is no colon.
        /// </summary>
        private void Declaration(int start, int end)
        {
            var colon = _code.IndexOf(':', start, end - start);
            var i = start;
            if (colon > start)
            {
                var name = _code.AsSpan(start, colon - start).TrimEnd();
                Add(name.StartsWith("--") ? FbCodeTokenKind.Variable : FbCodeTokenKind.Property, start, start + name.Length);
                Add(FbCodeTokenKind.Plain, start + name.Length, colon);
                Add(FbCodeTokenKind.Punctuation, colon, colon + 1);
                i = colon + 1;
            }

            while (i < end)
            {
                var c = _code[i];
                int next;
                if (char.IsWhiteSpace(c))
                {
                    next = Skip(i, end, char.IsWhiteSpace);
                    Add(FbCodeTokenKind.Plain, i, next);
                }
                else if (c is '"' or '\'')
                {
                    var close = _code.IndexOf(c, i + 1);
                    next = close < 0 || close >= end ? end : close + 1;
                    Add(FbCodeTokenKind.String, i, next);
                }
                else if (char.IsDigit(c) || (c is '.' or '-' && char.IsDigit(At(i + 1))) || (c == '#' && char.IsLetterOrDigit(At(i + 1))))
                {
                    next = Skip(i + 1, end, ch => char.IsLetterOrDigit(ch) || ch is '.' or '%');
                    Add(FbCodeTokenKind.Number, i, next);
                }
                else if (c == '!')
                {
                    next = Skip(i + 1, end, char.IsLetter);
                    Add(FbCodeTokenKind.Keyword, i, next);
                }
                else if (IsIdentifierStart(c) || c == '-')
                {
                    next = Skip(i, end, ch => IsIdentifierPart(ch) || ch == '-');
                    var kind = At(next) == '(' ? FbCodeTokenKind.Function
                        : _code.AsSpan(i, next - i).StartsWith("--") ? FbCodeTokenKind.Variable
                        : FbCodeTokenKind.Plain;
                    Add(kind, i, next);
                }
                else
                {
                    next = i + 1;
                    Add(FbCodeTokenKind.Punctuation, i, next);
                }
                i = next;
            }
        }
    }
}
