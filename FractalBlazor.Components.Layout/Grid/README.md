# Responsive CSS Grid

`FbGridContainer` and `FbGridItem` provide a mobile-first CSS Grid system based
on container queries. There is no JavaScript, resize listener, cascading value,
or parent/child component registration.

The breakpoints match the project's responsive container scale:

| Step | Minimum container width |
| --- | ---: |
| XS | 512px |
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
                 GM
                 GapL="FbSpacing.X">
    <FbGridItem ColumnSpan="4" ColumnSpanM="5" ColumnSpanXL="8" WBA WFM WRM PM>
        Main content
    </FbGridItem>

    <FbGridItem ColumnSpan="4" ColumnSpanM="3" ColumnSpanXL="4" WBA WFM WRM PM>
        Sidebar
    </FbGridItem>
</FbGridContainer>
```

The base gap uses the same flags as rows and stacks: `GS`, `GM`, `GL`, `GX`, with
`GR*` and `GC*` for row and column gaps only. Responsive gaps are set with
`GapXS` to `GapXXL`.

Items also support responsive `ColumnStart`, `RowStart`, `RowSpan`, and `Order`
parameters. Explicit starts use one-based CSS grid lines. Use the `SJV*` and
`SJH*` flags for item alignment, or configure defaults on the container with
the `JV*` and `JH*` flags.

Containers and items are surfaces: they accept the fill, frame, radius, padding
and hover flags of the Layout grammar (`WBA`, `WFM`, `WRM`, `PM`, `Hover`…).

For advanced layouts, `TemplateColumns`, `TemplateRows`, and `AutoRows` accept
native CSS values. `TemplateColumns` supersedes the numeric column parameters.
`AdditionalAttributes`, `Classes`, and `Style` are applied to each component's
outer element.

Use responsive `Order` only when visual order does not need to match reading and
keyboard-navigation order. The DOM order remains unchanged for accessibility.
