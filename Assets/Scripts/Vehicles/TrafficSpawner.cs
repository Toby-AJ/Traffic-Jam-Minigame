using System.Collections;
using TrafficJam.Core;
using UnityEngine;

namespace TrafficJam.Traffic
{
    public class TrafficSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject trafficPrefab;

        [Header("Arena")]
        [SerializeField]
        float spawnRadius = 8f;

        [Header("Timing")]
        [SerializeField]
        float spawnDelay = 3f;

        [Header("Particle")]
        [SerializeField]
        GameObject trafficWarningPrefab;
        [SerializeField]
        float warningTime = 1.5f;


        private void Start()
        {
            InvokeRepeating(
                nameof(SpawnTraffic),
                1f,
                spawnDelay
            );
        }

        /// <summary>
        /// Attempts to spawn a new traffic vehicle.
        /// A warning effect is created before the vehicle appears.
        /// </summary>
        void SpawnTraffic()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                return;
            }

            Vector3 start = GetEdgePoint();
            Vector3 end = -start;
            Vector2 curveOffset = Random.insideUnitCircle * 5f;

            GameObject warning = null;

            if (trafficWarningPrefab != null)
            {
                Quaternion rotation = Quaternion.LookRotation(end - start);

                warning = Instantiate(trafficWarningPrefab, start, rotation);
            }

            StartCoroutine(SpawnCarAfterWarning(start, end, warning));
        }

        /// <summary>
        /// Waits for the warning duration before spawning the vehicle.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="warning"></param>
        /// <returns></returns>
        private IEnumerator SpawnCarAfterWarning(Vector3 start, Vector3 end, GameObject warning)
        {
            yield return new WaitForSeconds(warningTime);

            // Remove warning effect
            if (warning != null)
            {
                Destroy(warning);
            }

            GameObject car = Instantiate(trafficPrefab, start, Quaternion.identity);

            TrafficVehicle vehicle = car.GetComponent<TrafficVehicle>();

            vehicle.SetupPath(start, end);
        }

        /// <summary>
        /// Returns a random point around the edge of the arena.
        /// </summary>
        private Vector3 GetEdgePoint()
        {
            Vector2 point = Random.insideUnitCircle.normalized * spawnRadius;
            return new Vector3(point.x, 0.2f, point.y);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StateChanged -= OnStateChanged;
        }

        /// <summary>
        /// Stops traffic spawning when the game finishes.
        /// </summary>
        /// <param name="state"></param>
        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Finished)
            {
                CancelInvoke(nameof(SpawnTraffic));
            }
        }

        /// <summary>
        /// Draws the traffic spawn radius inside the Scene view.
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
