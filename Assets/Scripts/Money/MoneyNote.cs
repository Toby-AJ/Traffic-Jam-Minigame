using TrafficJam.Core;
using TrafficJam.Players;
using UnityEngine;

namespace TrafficJam.Money
{
    [RequireComponent(typeof(Collider))]
    public class MoneyNote : MonoBehaviour
    {
        [SerializeField] 
        int value;

        public int Value => value;

        private bool _collected;

        /// <summary>
        /// Awards the note's value to the player that collected it.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Car"))
            {
                if (_collected)
                    return;

                if (GameManager.Instance.State != GameState.Playing)
                    return;

                if (!other.TryGetComponent(out PlayerScore playerScore))
                    return;

                _collected = true;

                playerScore.AddMoney(value);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMoneyPickup(0.25f);
                }

                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StateChanged -= OnStateChanged;
        }

        /// <summary>
        /// Removes the money note when the game finishes.
        /// </summary>
        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Finished)
            {
                Destroy(gameObject);
            }
        }
    }
}
