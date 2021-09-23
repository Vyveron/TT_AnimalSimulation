using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Simulation
{
    internal sealed class FruitParticlePool : IUniTaskAsyncDisposable
    {
        private readonly ParticleSystem _prefab;
        private readonly Transform _root;
        private readonly Vector3 _size;
        
        private readonly Queue<ParticleSystem> _pool;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        private int _currentlyPlaying;

        internal FruitParticlePool(ParticleSystem prefab, Vector3 size, Transform root, int startAmount = 10)
        {
            _prefab = prefab;
            _size = size;
            _root = root;
            _pool = new Queue<ParticleSystem>();

            AddInstances(startAmount);
        }

        private void AddInstances(int amount)
        {
            for(var i = 0; i < amount; i++)
                AddInstance();
        }

        private void AddInstance()
        {
            var instance = CreateInstance();

            _pool.Enqueue(instance);
        }

        private ParticleSystem CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _root);

            instance.transform.localScale = _size;

            return instance;
        }

        internal void Play(Vector3 position)
        {
            PlayAsync(position).Forget();
        }
        
        private async UniTask PlayAsync(Vector3 position)
        {
            var instance = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();

            instance.transform.position = position;
            instance.Play();

            _currentlyPlaying++;

            await UniTask.Delay(TimeSpan.FromSeconds(instance.main.duration));

            _pool.Enqueue(instance);

            _currentlyPlaying--;
        }

        private async UniTask DestroyParticles()
        {
            await UniTask.WaitUntil(() => _currentlyPlaying == 0);

            while (_pool.Count > 0)
            {
                var instance = _pool.Dequeue();

                Object.Destroy(instance.gameObject);
            }
        }
        
        public UniTask DisposeAsync()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            return DestroyParticles();
        }
    }
}