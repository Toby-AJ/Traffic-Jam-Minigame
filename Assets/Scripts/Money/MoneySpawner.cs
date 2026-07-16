using System.Collections;
using System.Collections.Generic;
using TrafficJam.Core;
using UnityEngine;

namespace TrafficJam.Money
{
    public class MoneySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField]
        float spawnInterval = 1f;
        [SerializeField]
        float spawnRadius = 8f;
        [SerializeField]
        int maxMoney = 5;
        [SerializeField]
        int moneySpawnHeight = 2;

        [Header("Money Prefabs")]
        [SerializeField]
        GameObject money1k;
        [SerializeField]
        GameObject money5k;
        [SerializeField]
        GameObject money10k;
        [SerializeField]
        GameObject money50k;

        private readonly List<GameObject> spawnedMoney = new();


        private void Start()
        {
            // Starts the continuous spawning routine.
            StartCoroutine(SpawnRoutine());
        }

        /// <summary>
        /// Repeatedly attempts to spawn money at the configured interval.
        /// </summary>
        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);

                TrySpawnMoney();
            }
        }

        /// <summary>
        /// Spawns a money pickup if the maximum count has not been reached.
        /// </summary>
        void TrySpawnMoney()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                return;
            }

            spawnedMoney.RemoveAll(money => money == null);

            if (spawnedMoney.Count >= maxMoney)
                return;

            GameObject prefab = GetRandomMoney();

            if (prefab == null)
                return;

            Vector3 spawnPosition = GetRandomPointInCircle();

            GameObject money = Instantiate(prefab, spawnPosition, Quaternion.identity);

            spawnedMoney.Add(money);
        }

        /// <summary>
        /// Selects a money prefab based on the configured spawn chances, returns null when no money should be spawned.
        /// </summary>
        GameObject GetRandomMoney()
        {
            int roll = Random.Range(0, 100);


            if (roll < 50)
            {
                return null;
            }

            if (roll < 70)
            {
                return money1k;
            }

            if (roll < 85)
            {
                return money5k;
            }

            if (roll < 95)
            {
                return money10k;
            }

            return money50k;
        }

        /// <summary>
        /// Returns a random position within the playable arena.
        /// </summary>
        Vector3 GetRandomPointInCircle()
        {
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            return new Vector3(randomPoint.x, moneySpawnHeight, randomPoint.y);
        }

        /// <summary>
        /// Draws the money spawn radius inside the Scene view.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
