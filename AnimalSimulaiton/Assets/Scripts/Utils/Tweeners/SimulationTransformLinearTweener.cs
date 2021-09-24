using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Simulation
{
    internal sealed class SimulationTransformLinearTweener : IDisposable
    {
        private Transform _root;
        private SimulationTime _simulationTime;
        private CancellationTokenSource _cancellationToken;

        internal SimulationTransformLinearTweener(Transform root, SimulationTime simulationTime)
        {
            _root = root;
            _simulationTime = simulationTime;
        }

        internal async UniTask MoveTo(Vector3 destination, float time)
        {
            _cancellationToken = new CancellationTokenSource();
            
            if (time <= 0)
            {
                _root.position = destination;

                return;
            }

            var start = _root.position;
            var path = destination - start;

            for (var timeSpent = 0f; timeSpent <= time; timeSpent += Time.deltaTime * _simulationTime.TimeScale)
            {
                await UniTask.WaitForEndOfFrame();

                var progress = timeSpent / time;


                if (_cancellationToken.Token.IsCancellationRequested)
                    return;
                
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                    return;
#endif

                _root.position = start + path * progress;
            }

            _root.position = destination;
        }
        
        public void Dispose()
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
        }
    }
}