using UnityEngine;
using UnityEngine.UIElements;
using TrafficJam.Core;
using TrafficJam.Players;

namespace TrafficJam.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GameHUD : MonoBehaviour
    {
        [SerializeField]
        UIDocument menuDoc;
        [SerializeField]
        UIDocument gameDoc;

        // References to each player's score component.
        [Header("Players")]
        [SerializeField] 
        PlayerScore player1;
        [SerializeField] 
        PlayerScore player2;
        [SerializeField] 
        PlayerScore player3;
        [SerializeField]
        PlayerScore player4;

        Label player1Score;
        Label player2Score;
        Label player3Score;
        Label player4Score;
        Label timer;
        Label countdown;

        /// <summary>
        /// Retrieves UI elements and subscribes to gameplay events.
        /// </summary>
        void Start()
        {
            VisualElement gameRoot = gameDoc.rootVisualElement;

            player1Score = gameRoot.Q<Label>("player-1-score-text");
            player2Score = gameRoot.Q<Label>("player-2-score-text");
            player3Score = gameRoot.Q<Label>("player-3-score-text");
            player4Score = gameRoot.Q<Label>("player-4-score-text");
            timer = gameRoot.Q<Label>("timer");
            countdown = gameRoot.Q<Label>("countdown");

            player1.ScoreChanged += OnPlayer1ScoreChanged;
            player2.ScoreChanged += OnPlayer2ScoreChanged;
            player3.ScoreChanged += OnPlayer3ScoreChanged;
            player4.ScoreChanged += OnPlayer4ScoreChanged;

            GameManager.Instance.TimeChanged += UpdateTimer;
            GameManager.Instance.CountdownChanged += UpdateCountdown;

            RefreshScores();
        }

        /// <summary>
        /// Unsubscribes from events when the HUD is disabled.
        /// </summary>
        void OnDisable()
        {
            player1.ScoreChanged -= OnPlayer1ScoreChanged;
            player2.ScoreChanged -= OnPlayer2ScoreChanged;
            player3.ScoreChanged -= OnPlayer3ScoreChanged;
            player4.ScoreChanged -= OnPlayer4ScoreChanged;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TimeChanged -= UpdateTimer;
                GameManager.Instance.CountdownChanged -= UpdateCountdown;
            }
        }

        /// <summary>
        /// Updates the countdown text shown in the centre of the screen.
        /// </summary>
        /// <param name="text"></param>
        void UpdateCountdown(string text)
        {
            countdown.text = text;
            countdown.style.display =
                string.IsNullOrEmpty(text)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
        }

        void OnPlayer1ScoreChanged(int score)
        {
            UpdateScore(player1Score, score);
        }

        void OnPlayer2ScoreChanged(int score)
        {
            UpdateScore(player2Score, score);
        }

        void OnPlayer3ScoreChanged(int score)
        {
            UpdateScore(player3Score, score);
        }

        void OnPlayer4ScoreChanged(int score)
        {
            UpdateScore(player4Score, score);
        }

        /// <summary>
        /// Refreshes every player score displayed on the HUD.
        /// </summary>
        void RefreshScores()
        {
            UpdateScore(player1Score, player1.Score);
            UpdateScore(player2Score, player2.Score);
            UpdateScore(player3Score, player3.Score);
            UpdateScore(player4Score, player4.Score);
        }

        /// <summary>
        /// Formats and displays a player's score.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="score"></param>
        void UpdateScore(Label label, int score)
        {
            label.text = $"${score:N0}K";
        }

        /// <summary>
        /// Updates the remaining game time.
        /// </summary>
        /// <param name="time"></param>
        void UpdateTimer(float time)
        {
            timer.text = Mathf.CeilToInt(time).ToString();
        }
    }
}
