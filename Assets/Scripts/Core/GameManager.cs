using System;
using System.Collections;
using UnityEngine;

namespace TrafficJam.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField, Min(1f)]
        float gameDuration = 30f;

        [SerializeField, Min(0f)]
        float countdownDuration = 3f;

        public GameState State { get; private set; }
        public float TimeRemaining { get; private set; }

        public bool IsPlaying => State == GameState.Playing;

        // Events used to notify UI and gameplay systems.
        public event Action<GameState> StateChanged;
        public event Action<float> TimeChanged;
        public event Action<string> CountdownChanged;

        /// <summary>
        /// Creates the singleton instance.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Plays the countdown before entering the Playing state.
        /// </summary>
        IEnumerator Start()
        {
            TimeRemaining = gameDuration;

            SetState(GameState.Countdown);

            int countdown = Mathf.CeilToInt(countdownDuration);

            while (countdown > 0)
            {
                CountdownChanged?.Invoke(countdown.ToString());

                yield return new WaitForSeconds(1f);

                countdown--;
            }

            CountdownChanged?.Invoke("GO!");

            SetState(GameState.Playing);

            yield return new WaitForSeconds(1f);

            CountdownChanged?.Invoke(string.Empty);
        }

        /// <summary>
        /// Updates the game timer while gameplay is active.
        /// </summary>
        void Update()
        {
            if (State != GameState.Playing)
                return;

            TimeRemaining -= Time.deltaTime;
            TimeRemaining = Mathf.Max(0f, TimeRemaining);

            TimeChanged?.Invoke(TimeRemaining);

            if (TimeRemaining <= 0f)
                FinishGame();
        }

        /// <summary>
        /// Ends the game and transitions to the Finished state.
        /// </summary>
        void FinishGame()
        {
            SetState(GameState.Finished);
        }

        /// <summary>
        /// Changes the current game state and notifies listeners.
        /// </summary>
        /// <param name="state"></param>
        void SetState(GameState state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}
