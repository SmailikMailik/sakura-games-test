using System;
using Project.Runtime.Actors;
using Project.Runtime.Extensions;
using Project.Runtime.Formatting;
using Project.Runtime.UI.Popups;
using R3;
using TMPro;
using UnityEngine;

namespace Project.Runtime.UI
{
    /// <summary>
    /// Связывает потоки симуляции с HUD и владеет подписками активного интерфейса.
    /// </summary>
    internal sealed class Hud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _preyDeaths;
        [SerializeField] private TMP_Text _predatorDeaths;
        [SerializeField] private TMP_Text _population;
        [SerializeField] private TMP_Text _clock;
        [SerializeField] private LabeledButton _pauseButton;
        [SerializeField] private LabeledButton _restartButton;
        [SerializeField] private RectTransform _popupRoot;
        [SerializeField] private EatingPopup _popupPrefab;

        private readonly CompositeDisposable _subscriptions = new();
        private EatingPopupPresenter _popupPresenter;
        private Observable<HudSnapshot> _state;
        private Observable<Actor> _foodConsumed;
        private Action _pause;
        private Action _restart;

        private void OnEnable()
        {
            SubscribeToSimulation();
        }

        private void OnDisable()
        {
            _subscriptions.Clear();
            ClearPopups();
        }

        private void OnDestroy()
        {
            Unbind();
            _subscriptions.Dispose();
        }

        /// <summary>
        /// Подключает потоки и действия симуляции, освобождая предыдущую привязку и надписи.
        /// </summary>
        internal void Bind(Observable<HudSnapshot> state, Observable<Actor> foodConsumed,
            Action pause, Action restart, EatingPopupPresenterFactory popupFactory)
        {
            Unbind();
            _popupPresenter = popupFactory.Create(_popupPrefab, _popupRoot);
            _state = state;
            _foodConsumed = foodConsumed;
            _pause = pause;
            _restart = restart;
            SubscribeToSimulation();
        }

        /// <summary>
        /// Показывает надпись о поедании только при подключённом и активном HUD.
        /// </summary>
        internal void ShowEatingPopup(Actor eater)
        {
            if (_state != null && isActiveAndEnabled)
                _popupPresenter.Show(eater);
        }

        /// <summary>
        /// Убирает активные надписи, сохраняя их в пуле для следующего показа.
        /// </summary>
        internal void ClearPopups()
        {
            _popupPresenter?.Clear();
        }

        /// <summary>
        /// Отключает потоки и действия симуляции, освобождает пул надписей; повторный вызов безопасен.
        /// </summary>
        internal void Unbind()
        {
            _subscriptions.Clear();
            _state = null;
            _foodConsumed = null;
            _pause = null;
            _restart = null;
            _popupPresenter?.Dispose();
            _popupPresenter = null;
        }

        private void SubscribeToSimulation()
        {
            if (_state == null || isActiveAndEnabled is false)
                return;

            // Общее подключение сразу передаёт актуальное состояние каждому элементу HUD.
            var state = _state.Replay(1).RefCount();
            state.BindToText(_preyDeaths, snapshot => snapshot.Population.PreyDeaths).AddTo(_subscriptions);
            state.BindToText(_predatorDeaths, snapshot => snapshot.Population.PredatorDeaths).AddTo(_subscriptions);
            state.BindToText(_population, snapshot => snapshot.Population.LivingCount).AddTo(_subscriptions);
            state.BindToText(_clock, snapshot => snapshot.Elapsed.TotalSeconds, DurationFormatter.FormatMinutesAndSeconds).AddTo(_subscriptions);
            state.Select(snapshot => snapshot.IsPaused).DistinctUntilChanged()
                .Subscribe(paused => _pauseButton.SetText(paused ? "Resume" : "Pause")).AddTo(_subscriptions);
            _restartButton.SetText("Restart");
            _pauseButton.SetAction(_pause).AddTo(_subscriptions);
            _restartButton.SetAction(_restart).AddTo(_subscriptions);
            _foodConsumed.Subscribe(ShowEatingPopup).AddTo(_subscriptions);
            Observable.EveryUpdate(UnityFrameProvider.PostLateUpdate).Subscribe(_ => _popupPresenter.Update()).AddTo(_subscriptions);
        }
    }
}