// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// WrapPanel is a panel that position child control vertically or horizontally based on the orientation and when max width/ max height is received a new row(in case of horizontal) or column (in case of vertical) is created to fit new controls.
    /// </summary>
    public partial class WrapPanel
    {
        [System.Diagnostics.DebuggerDisplay("U = {U} V = {V}")]
        private struct UvMeasure
        {
            internal static readonly UvMeasure Zero = default;

            internal double U { get; set; }

            internal double V { get; set; }

            public UvMeasure(Orientation orientation, Size size)
                : this(orientation, size.Width, size.Height)
            {
            }

            public UvMeasure(Orientation orientation, double width, double height)
            {
                if (orientation == Orientation.Horizontal)
                {
                    U = width;
                    V = height;
                }
                else
                {
                    U = height;
                    V = width;
                }
            }

            public UvMeasure Add(double u, double v)
                => new UvMeasure { U = U + u, V = V + v };
        }

        private struct UvRect
        {
            public UvMeasure Position { get; set; }

            public UvMeasure Size { get; set; }

            public UvRect WithVerticalAlignment(VerticalAlignment alignment, double maxHeight)
            {
                switch (alignment)
                {
                    case VerticalAlignment.Center:
                        return new UvRect
                        {
                            Position = Position.Add(
                                u: 0,
                                v: Math.Max((maxHeight - Size.V) / 2.0, 0.0)),
                            Size = Size,
                        };
                    case VerticalAlignment.Bottom:
                        return new UvRect
                        {
                            Position = Position.Add(
                                u: 0,
                                v: Math.Max(maxHeight - Size.V, 0.0)),
                            Size = Size,
                        };
                    case VerticalAlignment.Stretch:
                        return new UvRect
                        {
                            Position = Position,
                            Size = new UvMeasure { U = Size.U, V = maxHeight },
                        };
                    case VerticalAlignment.Top:
                    default:
                        return this;
                }
            }

            public UvRect WithHorizontalAlignment(HorizontalAlignment alignment, double maxHeight)
            {
                switch (alignment)
                {
                    case HorizontalAlignment.Center:
                        return new UvRect
                        {
                            Position = Position.Add(
                                u: 0,
                                v: Math.Max((maxHeight - Size.V) / 2.0, 0.0)),
                            Size = Size,
                        };
                    case HorizontalAlignment.Right:
                        return new UvRect
                        {
                            Position = Position.Add(
                                u: 0,
                                v: Math.Max(maxHeight - Size.V, 0.0)),
                            Size = Size,
                        };
                    case HorizontalAlignment.Stretch:
                        return new UvRect
                        {
                            Position = Position,
                            Size = new UvMeasure { U = Size.U, V = maxHeight },
                        };
                    case HorizontalAlignment.Left:
                    default:
                        return this;
                }
            }

            public Rect ToRect(Orientation orientation)
            {
                switch (orientation)
                {
                    case Orientation.Vertical:
                        return new Rect(Position.V, Position.U, Size.V, Size.U);
                    case Orientation.Horizontal:
                        return new Rect(Position.U, Position.V, Size.U, Size.V);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private class Row
        {
            private readonly List<UvRect> _childrenRects;
            private UvMeasure _rowSize;

            public Row()
            {
                _childrenRects = new List<UvRect>();
                _rowSize = UvMeasure.Zero;
            }

            public IReadOnlyList<UvRect> ChildrenRects => _childrenRects;

            /// <summary>
            /// Gets the size of the row.
            /// </summary>
            public UvMeasure Size => _rowSize;

            public void Add(UvMeasure position, UvMeasure size)
            {
                _childrenRects.Add(new UvRect { Position = position, Size = size });
                _rowSize.U = Math.Max(_rowSize.U, position.U + size.U);
                _rowSize.V = Math.Max(_rowSize.V, size.V);
            }
        }
    }
}
