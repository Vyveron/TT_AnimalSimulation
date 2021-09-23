using UnityEngine;

namespace Simulation.Pathfinding
{
    internal sealed class AstarCell
    {
        internal readonly Vector2Int Position;
        internal readonly float F;
        internal readonly float H;
        internal readonly AstarCell Origin;

        internal float G => F + H;
        internal int ArrivingTime => Mathf.RoundToInt(F + 1f);

        internal AstarCell(Vector2Int position, float f, float h, AstarCell origin)
        {
            Position = position;
            F = f;
            H = h;
            Origin = origin;
        }
    }
}