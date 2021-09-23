using System.Collections.Generic;
using UnityEngine;
using Simulation.Pathfinding;

namespace Simulation
{
    internal sealed class SimulationGrid : Grid<SimulationGridCell>
    {
        internal readonly Transform Root;

        internal SimulationGrid(Transform root, int size) : base(size)
        {
            Root = root;

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                    GridMatrix[x, y] = new SimulationGridCell();
            }
        }

        internal void Occupy(Vector2Int position)
        {
            GridMatrix[position.x, position.y].AddObstacle(new PathObstacle(0, Vector2Int.zero));
        }

        internal void SetPathObstacle(List<PathCell> path)
        {
            for (var i = 0; i < path.Count; i++)
            {
                var pathCell = path[i];
                var position = pathCell.Position;
                var fromDirection = i < path.Count - 1 ? path[i + 1].Position - position : Vector2Int.zero;

                GridMatrix[position.x, position.y].AddObstacle(new PathObstacle(pathCell.ArrivingTime, fromDirection));
            }
        }

        internal void DecreaseObstacleValues()
        {
            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                    GridMatrix[x, y].DecreaseObstacleValues();
            }
        }

        internal List<Vector2Int> GetFreeSpace(List<Vector2Int> space)
        {
            for (var i = space.Count - 1; i >= 0; i--)
            {
                var cell = space[i];
                var x = cell.x;
                var y = cell.y;

                if (IsInBounds(x, y) == false)
                    continue;

                if (GridMatrix[x, y].IsOccupied())
                    space.RemoveAt(i);
            }

            return space;
        }


        internal List<Vector2Int> GetFreeSpace()
        {
            var result = new List<Vector2Int>();

            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    var cell = GridMatrix[x, y];

                    if (cell.IsOccupied() == false)
                        result.Add(new Vector2Int(x, y));
                }
            }

            return result;
        }

        internal Vector3 GetLocalScaleForObject()
        {
            return Vector3.one / Size;
        }

        internal Vector3 GridToWorldPosition(Vector2Int position)
        {
            var pivotOffset = .5f / Size;
            var scaledPosition = (Vector2)position / Size;
            var centeredPosition = scaledPosition - new Vector2(.5f, .5f);
            var x = centeredPosition.x + pivotOffset;
            var z = centeredPosition.y + pivotOffset;
            var unitPosition = new Vector3(x, 0f, z);
            var worldPosition = Vector3.Scale(unitPosition, Root.localScale);

            return worldPosition;
        }
    }
}