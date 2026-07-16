using TrafficJam.Core;
using TrafficJam.Money;
using UnityEngine;

namespace TrafficJam.Vehicles
{
    [RequireComponent(typeof(VehicleController))]
    public class CPUVehicleController : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField, Min(0.05f)]
        private float decisionInterval = 0.1f;

        [Header("Obstacle Detection")]
        [SerializeField, Min(0f)]
        private float detectionDistance = 5f;

        [SerializeField, Min(0f)]
        private float detectionRadius = 0.75f;

        [SerializeField, Min(0f)]
        private float avoidanceDistance = 4f;

        [SerializeField, Min(0f)]
        private float avoidanceForwardDistance = 3f;

        [SerializeField]
        private LayerMask obstacleLayers;

        private VehicleController vehicle;

        private bool isAvoiding;
        private int avoidanceDirection;


        private void Awake()
        {
            vehicle = GetComponent<VehicleController>();
        }

        private void Start()
        {
            // Begins updating AI decisions at a fixed interval.
            InvokeRepeating(nameof(UpdateAI), 0f, decisionInterval);
        }

        /// <summary>
        /// Chooses the current target and determines whether obstacle avoidance is required.
        /// </summary>
        private void UpdateAI()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                vehicle.Stop();
                return;
            }

            MoneyNote target = FindBestMoney();

            if (target == null)
            {
                vehicle.Stop();
                return;
            }

            if (TryGetObstacle(out RaycastHit obstacle))
            {
                StartAvoiding(obstacle);

                Vector3 avoidanceTarget = GetAvoidanceTarget();

                vehicle.SetTargetPosition(avoidanceTarget);

                return;
            }

            if (isAvoiding)
            {
                if (IsPathClear())
                {
                    isAvoiding = false;
                }
                else
                {
                    vehicle.SetTargetPosition(GetAvoidanceTarget());

                    return;
                }
            }

            vehicle.SetTargetPosition(target.transform.position);
        }

        /// <summary>
        /// Checks for obstacles directly in front of the vehicle.
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool TryGetObstacle(out RaycastHit hit)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            return Physics.SphereCast(origin, detectionRadius, transform.forward, out hit, detectionDistance, obstacleLayers, QueryTriggerInteraction.Collide);
        }

        /// <summary>
        /// Determines which side of an obstacle should be used when steering around it.
        /// </summary>
        /// <param name="obstacle"></param>
        private void StartAvoiding(RaycastHit obstacle)
        {
            if (isAvoiding)
                return;

            isAvoiding = true;

            Vector3 toObstacle = obstacle.collider.bounds.center - transform.position;

            float obstacleSide = Vector3.Dot(toObstacle, transform.right);

            // Obstacle on right, go left.
            // Obstacle on left, go right.
            avoidanceDirection = obstacleSide >= 0f ? -1 : 1;
        }

        /// <summary>
        /// Calculates a temporary steering target while avoiding an obstacle.
        /// </summary>
        private Vector3 GetAvoidanceTarget()
        {
            Vector3 side = transform.right * avoidanceDirection * avoidanceDistance;

            Vector3 forward = transform.forward * avoidanceForwardDistance;

            return transform.position + side + forward;
        }

        /// <summary>
        /// Determines whether the path ahead is clear.
        /// </summary>
        private bool IsPathClear()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            bool blocked = Physics.SphereCast(origin, detectionRadius, transform.forward, out _, detectionDistance, obstacleLayers, QueryTriggerInteraction.Collide);

            return !blocked;
        }

        /// <summary>
        /// Finds the most desirable money pickup by balancing its value against its distance.
        /// </summary>
        private MoneyNote FindBestMoney()
        {
            MoneyNote[] money = FindObjectsByType<MoneyNote>(FindObjectsSortMode.None);

            MoneyNote best = null;
            float bestScore = float.MinValue;

            foreach (MoneyNote note in money)
            {
                float distance = Vector3.Distance(transform.position, note.transform.position);

                distance = Mathf.Max(distance, 0.1f);

                float score = note.Value / distance;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = note;
                }
            }

            return best;
        }

        /// <summary>
        /// Draws obstacle detection and avoidance gizmos for debugging AI behaviour.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;

            Gizmos.color = Color.red;

            Gizmos.DrawLine(origin, origin + transform.forward * detectionDistance);

            Gizmos.DrawWireSphere(origin + transform.forward * detectionDistance, detectionRadius);

            if (!isAvoiding)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(GetAvoidanceTarget(), 0.25f);
            Gizmos.DrawLine(transform.position, GetAvoidanceTarget());
        }
    }
}