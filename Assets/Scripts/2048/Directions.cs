using System.Collections;
using UnityEngine;

namespace Puzzle2048
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public delegate void TileMatrixLoopBody(int x, int y, ref TileValue[][] matrix, Direction direction);
    public delegate void NoreturnNoParameterFunc();
    static public class DirectionExtensions
    {
        public static Vector2Int GetDirection(this Direction direction)
        {
            return new Vector2Int[4]
            {
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(-1,0),
                new Vector2Int(1,0)
            }[(int)direction];
        }

        public static void TileMatrixLoopWithDiection(this Direction direction, TileValue[][] matrix, TileMatrixLoopBody loopBody)
        {
            var width = matrix.Length;
            var height = matrix[0].Length;
            if (direction == Direction.Down)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        loopBody(x, y, ref matrix, direction);
                    }
                }
            }
            if (direction == Direction.Right)
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    for (int y = 0; y < height; y++)
                    {
                        loopBody(x, y, ref matrix, direction);
                    }
                }
            }
            if (direction == Direction.Left)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        loopBody(x, y, ref matrix, direction);
                    }
                }
            }
            if (direction == Direction.Up)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        loopBody(x, y, ref matrix, direction);
                    }
                }
            }
        }

    }
}