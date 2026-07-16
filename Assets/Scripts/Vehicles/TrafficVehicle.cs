using TrafficJam.Core;
using TrafficJam.Players;
using UnityEngine;
using System.Collections;

namespace TrafficJam.Traffic
{
    public class TrafficVehicle : MonoBehaviour
    {
        Vector3 startPoint;
        Vector3 controlPoint;
        Vector3 endPoint;

        [Header("Movement")]
        [SerializeField]
        float speed = 5f;

        [Header("Curve")]
        [SerializeField]
        float curveAmount = 8f;

        [Header("Collision")]
        [SerializeField]
        int collisionPenalty = 10;
        [SerializeField]
        float collisionCooldown = 1f;

        float progress;
        bool canHit = true;

        /// <summary>
        /// Initialises the curved path the vehicle will follow.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetupPath(Vector3 start, Vector3 end)
        {
            startPoint = start;
            endPoint = end;

            Vector3 direction = (end - start).normalized;

            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            // Random left/right curve
            float side = Random.Range(-1f, 1f);

            controlPoint = Vector3.Lerp(start, end, 0.5f) + perpendicular * curveAmount * side;

            progress = 0f;

            transform.position = startPoint;
        }

        /// <summary>
        /// Moves the vehicle along its Bézier curve and rotates
        /// it to face the direction of travel.
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                Destroy(this);
                return;
            }

            progress += speed * Time.deltaTime;

            Vector3 position = CalculateCurve(startPoint, controlPoint, endPoint, progress);

            transform.position = position;

            Vector3 nextPosition = CalculateCurve(startPoint, controlPoint, endPoint, progress + 0.03f);

            Vector3 direction = nextPosition - position;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (progress >= 1f)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Applies a score penalty when colliding with a player.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!canHit)
                return;


            if (other.TryGetComponent<PlayerScore>(out var player))
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCrash(0.15f);
                }

                player.RemoveMoney(collisionPenalty);

                StartCoroutine(CollisionCooldown());
            }
        }

        /// <summary>
        /// Prevents multiple collisions from being registered in quick succession.
        /// </summary>
        IEnumerator CollisionCooldown()
        {
            canHit = false;

            yield return new WaitForSeconds(collisionCooldown);

            canHit = true;
        }

        /// <summary>
        /// Evaluates a point along a quadratic Bézier curve.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 CalculateCurve(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            float u = 1 - t;
            return u * u * a + 2 * u * t * b + t * t * c;
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
        /// Removes the traffic vehicle when gameplay ends.
        /// </summary>
        /// <param name="state"></param>
        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Finished)
            {
                Destroy(gameObject);
            }
        }
    }
}
