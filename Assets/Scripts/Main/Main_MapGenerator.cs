using UnityEngine;
using System.Collections.Generic;

public class Main_MapGenerator
{
    public Cell[] Generate(int width, int height, IEnumerable<Coordinate> playerStartCoordinates)
    {
        var cells = new List<Cell>();
        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var coordinate = new Coordinate(x, y);
                var type = Cell.Types.Floor;
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || (x % 2 == 0 && y % 2 == 0))
                {
                    type = Cell.Types.StaticWall;
                }
                else if (!IsTargetCoordinatesOrAround(coordinate, playerStartCoordinates) && Random.Range(0, 2) == 0)
                {
                    type = Cell.Types.Wall;
                }
                cells.Add(new Cell(coordinate, type));
            }
        }
        return cells.ToArray();
    }

    bool IsTargetCoordinatesOrAround(Coordinate coordinate, IEnumerable<Coordinate> targetCoordinates, float limitDistance = 1f)
    {
        foreach (var targetCoordinate in targetCoordinates)
        {
            if ((targetCoordinate - coordinate).Magnitude <= limitDistance)
            {
                return true;
            }
        }
        return false;
    }

    public struct Coordinate
    {
        public int x;
        public int y;

        public float Magnitude
        {
            get { return Mathf.Sqrt(x * x + y * y); }
        }

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Coordinate operator +(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1.x + c2.x, c1.y + c2.y);
        }

        public static Coordinate operator -(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1.x - c2.x, c1.y - c2.y);
        }

        public static Coordinate operator *(Coordinate c, int i)
        {
            return new Coordinate(c.x * i, c.y * i);
        }
    }

    public struct Cell
    {
        public enum Types
        {
            Floor,
            Wall,
            StaticWall
        }

        public readonly Coordinate coordinate;
        public readonly Types type;

        public Cell(Coordinate coordinate, Types type)
        {
            this.coordinate = coordinate;
            this.type = type;
        }
    }
}
