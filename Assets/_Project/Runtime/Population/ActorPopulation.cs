using System;
using System.Collections.Generic;
using Project.Runtime.Actors;
using Project.Runtime.Actors.Contacts;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Runtime.Population
{
    /// <summary>
    /// Владеет живыми акторами и счётчиками смертей; применяет правила поедания при контакте.
    /// </summary>
    internal sealed class ActorPopulation : IActorContactHandler, IDisposable
    {
        private readonly ActorSpawner _spawner;
        private readonly List<Actor> _actors = new();
        private readonly ReactiveProperty<PopulationSnapshot> _state = new(new(0, 0, 0));
        private readonly Subject<Actor> _foodConsumed = new();
        private int _preyDeaths;
        private int _predatorDeaths;

        /// <summary>
        /// Сообщает о поедании и передаёт выжившее животное, съевшее жертву.
        /// </summary>
        internal Observable<Actor> FoodConsumed => _foodConsumed;
        internal ReadOnlyReactiveProperty<PopulationSnapshot> State => _state;
        internal IReadOnlyList<Actor> Actors => _actors;
        internal PopulationSnapshot Snapshot => _state.Value;

        internal ActorPopulation(ActorSpawner spawner)
        {
            _spawner = spawner;
        }

        /// <summary>
        /// Добавляет не более одного актора за шаг; отрицательное время считает нулевым.
        /// </summary>
        internal void Advance(float deltaTime)
        {
            var step = Mathf.Max(0f, deltaTime);
            if (_spawner.Advance(step, _actors, this, out var actor))
            {
                _actors.Add(actor);
                PublishState();
            }
        }

        /// <summary>
        /// Применяет поедание только к акторам этой популяции; одну смерть учитывает один раз.
        /// </summary>
        public void HandleContact(Actor first, Actor second)
        {
            if (_actors.Contains(first) is false || _actors.Contains(second) is false)
                return;

            if (PredationRules.CanEat(first.State, second.State))
                Eat(first, second);
            else if (PredationRules.CanEat(second.State, first.State))
                Eat(second, first);
        }

        /// <summary>
        /// Удаляет всех акторов и обнуляет статистику; следующее появление не требует ожидания.
        /// </summary>
        internal void Reset()
        {
            for (var i = _actors.Count - 1; i >= 0; i--)
            {
                var actor = _actors[i];
                actor.State.TryDie();
                RemoveActor(actor);
            }

            _spawner.Reset();
            _preyDeaths = 0;
            _predatorDeaths = 0;
            PublishState();
        }

        /// <summary>
        /// Завершает потоки популяции при освобождении сценового контейнера.
        /// </summary>
        public void Dispose()
        {
            _foodConsumed.Dispose();
            _state.Dispose();
        }

        private void PublishState()
        {
            _state.Value = new(_preyDeaths, _predatorDeaths, _actors.Count);
        }

        private void Eat(Actor eater, Actor victim)
        {
            if (victim.State.TryDie() is false)
                return;

            if (victim.State.Role == FoodRole.Prey)
                _preyDeaths++;
            else
                _predatorDeaths++;

            RemoveActor(victim);
            PublishState();
            _foodConsumed.OnNext(eater);
        }

        private void RemoveActor(Actor actor)
        {
            _actors.Remove(actor);
            actor.gameObject.SetActive(false);
            Object.Destroy(actor.gameObject);
        }
    }
}