using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Pathfinding
{
    internal class Grid<T>
    {
        protected readonly T[,] GridMatrix;

        internal T this[int x, int y]
        {
            get => GridMatrix[x, y];
            set => GridMatrix[x, y] = value;
        }

        internal int Size { get; }

        internal Grid(int size)
        {
            Size = size;

            GridMatrix = new T[size, size];
        }
        
        internal bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Size &&
                y >= 0 && y < Size;
        }
        
        internal List<Vector2Int> GetInRadius(Vector2Int center, int radius, int innerRadius = 0)
        {
            var radiusVector = radius * Vector2Int.one;
            var startCell = center - radiusVector;
            var finishCell = center + radiusVector;
            var result = new List<Vector2Int>();

            for (var x = startCell.x; x <= finishCell.x; x++)
            {
                for (var y = startCell.y; y <= finishCell.y; y++)
                {
                    if (IsInBounds(x, y) == false)
                        continue;

                    var distance = Mathf.Abs(center.x - x + center.y - y);

                    if (distance > radius || distance < innerRadius)
                        continue;

                    result.Add(new Vector2Int(x, y));
                }
            }

            return result;
        }
    }
}