using System.Collections;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public sealed class DataGrid<T>(BoundedSquareGrid bounds)
{
    #if DEBUG
    public 
    #else
    private
    #endif
    T[,] _dat = new T[bounds.XCells, bounds.YCells];

    public BoundedSquareGrid Bounds { get; } = bounds;

    public CellData this[Point point] => this[point.X, point.Y];
    
    public CellData this[int x, int y]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(x, Bounds.XCells);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(y, Bounds.YCells);
            ArgumentOutOfRangeException.ThrowIfNegative(x);
            ArgumentOutOfRangeException.ThrowIfNegative(y);

            return new CellData(this, x, y);
        }
    }

    public CellDataEnumerator GetCells()
        => new CellDataEnumerator(this);

    public CellDataYFirstEnumerator GetCellsYFirst()
        => new CellDataYFirstEnumerator(this);
    
    public struct CellDataYFirstEnumerator(DataGrid<T> grid)
    {
        private int x;
        private int y;
        
        public CellDataYFirstEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (++y >= grid.Bounds.YCells)
            {
                x++;
                y = 0;
            }

            if (x >= grid.Bounds.XCells) return false;
            Current = new CellData(grid, x, y);
            return true;
        }

        public void Reset()
        {
            x = y = 0;
        }

        public CellData Current { get; private set; }
    }
    
    public struct CellDataEnumerator(DataGrid<T> grid)
    {
        private int x;
        private int y;
        
        public CellDataEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (++x >= grid.Bounds.XCells)
            {
                y++;
                x = 0;
            }

            if (y >= grid.Bounds.YCells) return false;
            
            Current = new CellData(grid, x, y);
            return true;
        }

        public void Reset()
        {
            x = y = 0;
        }

        public CellData Current { get; private set; }
    }
    
    public readonly record struct CellData
    {
        private readonly int x;
        private readonly int y;
        private readonly DataGrid<T> grid;
        
        internal CellData(DataGrid<T> grid, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.grid = grid;
        }

        public Vector2 GetPosition(GridPositionOffset xoffset = default, GridPositionOffset yoffset = default) 
            => grid.Bounds.GetPosition(x, y, xoffset, yoffset);

        public RectangleF AreaF => new RectangleF(
            x * grid.Bounds.Grid.XScale,
            y * grid.Bounds.Grid.YScale,
            grid.Bounds.Grid.XScale,
            grid.Bounds.Grid.YScale
        );

        public Rectangle Area => new Rectangle(
            (int)(x * grid.Bounds.Grid.XScale),
            (int)(y * grid.Bounds.Grid.YScale),
            (int)(grid.Bounds.Grid.XScale),
            (int)(grid.Bounds.Grid.YScale)
        );

        public ref T Data
        {
            get
            {
                #if DEBUG
                if (x < 0 || x >= grid._dat.GetLength(0) || y < 0 || y >= grid._dat.GetLength(1)) 
                    Debugger.Break();
                #endif
                return ref grid._dat[x, y];
            }
        }
    }
}