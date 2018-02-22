using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Game
{
    public enum Symbol
    {
        Empty = 0,
        Circle = 1,
        Cross = 2
    }

    public static class SymbolExtensions {
        public static Symbol Opposite(this Symbol self) {
            switch (self) {
                case Symbol.Circle:
                    return Symbol.Cross;

                case Symbol.Cross:
                    return Symbol.Circle;

                default:
                    return Symbol.Empty;
            }
        }
    }

    public struct Position {
        int row, col;

        public Position(int i, int j) {
            this.row = i;
            this.col = j;
        }

        public int Row { get { return row; } }
        public int Col { get { return col; } }
    }

    public class Slot
    {
        public Symbol symbol;
        public List<Game.Position[]> spaces;

        public Slot()
        {
            symbol = Symbol.Empty;
            spaces = new List<Position[]>();
        }
    }

    public class Board
    {
        private Slot[] board;
        int emptyCount;
        int winSize;

        public Board(int w, int h, int size)
        {
            Width = w;
            Height = h;
            winSize = size;

            CreateBoard();

            Clear();
        }

        private struct GenerateSpacesIteration
        {
            public Game.Position dir;
            public Game.Position start;
            public Game.Position end;

            public GenerateSpacesIteration(
                Game.Position dir,
                Game.Position start,
                Game.Position end)
            {
                this.dir = dir;
                this.start = start;
                this.end = end;
            }
        }

        private void CreateBoard()
        {
            int slotCount = Width * Height;
            board = new Slot[slotCount];

            for (int i = 0; i != slotCount; ++i)
                board[i] = new Slot();

            GenerateSpacesIteration[] iterations = new GenerateSpacesIteration[] {
                new GenerateSpacesIteration(
                    new Game.Position(0, 1),
                    new Game.Position(0, 0),
                    new Game.Position(Height, Width - winSize + 1)
                ),

                new GenerateSpacesIteration(
                    new Game.Position(1, 0),
                    new Game.Position(0, 0),
                    new Game.Position(Height - winSize + 1, Width)
                ),

                new GenerateSpacesIteration(
                    new Game.Position(1, 1),
                    new Game.Position(0, 0),
                    new Game.Position(Height - winSize + 1, Width - winSize + 1)
                ),

                new GenerateSpacesIteration(
                    new Game.Position(-1, 1),
                    new Game.Position(winSize - 1, 0),
                    new Game.Position(Height, Width - winSize + 1)
                )
            };

            foreach (GenerateSpacesIteration iteration in iterations)
            {
                for (int i = iteration.start.Row; i != iteration.end.Row; ++i)
                {
                    for (int j = iteration.start.Col; j != iteration.end.Col; ++j)
                    {
                        Game.Position[] space = new Game.Position[winSize];

                        for (int k = 0; k != winSize; ++k)
                        {
                            var currentPosition = new Game.Position(
                                i + iteration.dir.Row * k,
                                j + iteration.dir.Col * k
                            );

                            space[k] = currentPosition;

                            SlotAt(currentPosition).spaces.Add(space);
                        }
                    }
				}
            }
        }

        public void Mark(Position position, Symbol symbol)
        {
            Debug.Assert(IsEmpty(position));
            Debug.Assert(symbol != Symbol.Empty);

            board[IndexOf(position)].symbol = symbol;
            --emptyCount;
        }

        private Symbol[] PositionsToSymbols(Game.Position[] spacePositions)
        {
            return Array.ConvertAll(spacePositions, pos => At(pos));
        }

        public bool HasWon(Position position)
        {
            Slot slot = SlotAt(position);
            Symbol targetSymbol = slot.symbol;

            return slot.spaces.Exists(positions =>
                Array.TrueForAll(PositionsToSymbols(positions), sym => sym == targetSymbol)
            );
        }

        public Symbol At(Position position)
        {
            return board[IndexOf(position)].symbol;
        }

        public Slot SlotAt(Position position)
        {
            return board[IndexOf(position)];
        }

        public bool IsEmpty(Position position)
        {
            return At(position) == Symbol.Empty;
        }

        private int IndexOf(Position position)
        {
            return (position.Row * Width) + position.Col;
        }

        public void Clear()
        {
            int size = Width * Height;

            for (int i = 0; i != size; ++i)
                board[i].symbol = Symbol.Empty;

            emptyCount = size;
        }

        public bool HasAnyEmpty {
            get => emptyCount > 0;
        }
        public int Width { get; }
        public int Height { get; }
    }
}