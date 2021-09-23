using UnityEngine;

namespace Simulation
{
    internal interface IGridObject
    {
        Vector2Int Position { get; }

        void SetPosition(Vector2Int position);
    }
}