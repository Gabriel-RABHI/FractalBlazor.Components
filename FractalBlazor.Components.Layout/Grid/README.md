# Responsive CSS Grid

`FbGridContainer` and `FbGridItem` provide a mobile-first CSS Grid system based
on container queries. There is no JavaScript, resize listener, cascading value,
or parent/child component registration.

The breakpoints match the project's current responsive container scale:

| Step | Minimum container width |
| --- | ---: |
| XS | 360px |
| S | 640px |
| M | 768px |
| L | 1024px |
| XL | 1280px |
| XXL | 1536px |

Responsive parameters are mobile-first. An omitted value inherits the closest
value from a smaller breakpoint, ultimately falling back to the un-suffixed
parameter.

```razor
<FbGridContainer Columns="4"
                 ColumnsM="8"
                 ColumnsXL="12"
                 Gap="FbSpacing.M"
                 GapL="FbSpacing.X">
    <FbGridItem ColumnSpan="4" ColumnSpanM="5" ColumnSpanXL="8">
        Main content
    </FbGridItem>

    <FbGridItem ColumnSpan="4" ColumnSpanM="3" ColumnSpanXL="4">
        Sidebar
    </FbGridItem>
</FbGridContainer>
```

Items also support responsive `ColumnStart`, `RowStart`, `RowSpan`, and `Order`
parameters. Explicit starts use one-based CSS grid lines. Use `AlignSelf` and
`JustifySelf` for item alignment, or configure defaults on the container with
`AlignItems` and `JustifyItems`.

For advanced layouts, `TemplateColumns`, `TemplateRows`, and `AutoRows` accept
native CSS values. `TemplateColumns` supersedes the numeric column parameters.
`AdditionalAttributes`, `Classes`, and `Style` are applied to each component's
outer element.

Use responsive `Order` only when visual order does not need to match reading and
keyboard-navigation order. The DOM order remains unchanged for accessibility.

`FbGridContainner` remains available as an obsolete compatibility alias for the
existing project spelling. New code should use `FbGridContainer`.
