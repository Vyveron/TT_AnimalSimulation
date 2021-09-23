using System.Collections.Generic;
using UnityEngine;
namespace Simulation.Extensions
{
    internal static class ListExtensions
    {
        internal static T PickRandom<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}