using System;
using UnityEngine;

namespace TrafficJam.Players
{
    public sealed class PlayerScore : MonoBehaviour
    {
        [SerializeField] 
        int startingScore;

        public int Score { get; private set; }

        public event Action<int> ScoreChanged;

        void Awake()
        {
            // Initial score assigned when the game begins.
            Score = startingScore;
        }

        /// <summary>
        /// Adds money to the player's current score.
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoney(int amount)
        {
            if (amount <= 0)
                return;

            Score += amount;

            ScoreChanged?.Invoke(Score);
        }

        /// <summary>
        /// Removes money from the player's score after a traffic collision.
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveMoney(int amount)
        {
            if (amount <= 0)
                return;

            Score -= amount;

            ScoreChanged?.Invoke(Score);
        }
    }
}
