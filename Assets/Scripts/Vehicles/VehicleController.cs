using TrafficJam.Core;
using UnityEngine;

namespace TrafficJam.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)]
        float maxSpeed = 8f;
        [SerializeField, Min(0f)]
        float acceleration = 20f;
        [SerializeField, Min(0f)]
        float deceleration = 25f;
        [SerializeField, Min(0f)]
        float rotationSpeed = 180f;
        [SerializeField, Min(0f)]
        float stoppingDistance = 0.15f;

        [Header("Arena Bounds")]
        [SerializeField]
        Transform arenaCenter;

        [SerializeField]
        float arenaRadius = 15f;

        [Header("Engine Audio")]
        [SerializeField]
        AudioSource engineAudio;

        [SerializeField]
        float engineStartSpeed = 0.1f;

        Rigidbody _rigidbody;
        Vector3 _targetPosition;
        float _currentSpeed;
        bool _hasTarget;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _targetPosition = transform.position;
        }

        /// <summary>
        /// Updates vehicle movement and engine audio.
        /// </summary>
        private void FixedUpdate()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                StopImmediately();
                UpdateEngineAudio();
                return;
            }

            MoveVehicle();

            UpdateEngineAudio();
        }

        /// <summary>
        /// Assigns a new movement target for the vehicle.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void SetTargetPosition(Vector3 targetPosition)
        {
            targetPosition.y = _rigidbody.position.y;

            _targetPosition = targetPosition;
            _hasTarget = true;
        }

        /// <summary>
        /// Clears the current movement target and begins slowing down.
        /// </summary>
        public void Stop()
        {
            _hasTarget = false;
            _targetPosition = _rigidbody.position;
        }

        /// <summary>
        /// Rotates and moves the vehicle towards its current target.
        /// </summary>
        void MoveVehicle()
        {
            Vector3 position = _rigidbody.position;

            // No target, just coast straight ahead while slowing down.
            if (!_hasTarget)
            {
                Decelerate();
                return;
            }

            Vector3 toTarget = _targetPosition - position;
            toTarget.y = 0f;

            float distance = toTarget.magnitude;

            if (distance <= stoppingDistance)
            {
                Stop();
                return;
            }

            Vector3 targetDirection = toTarget.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            Quaternion nextRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            _rigidbody.MoveRotation(nextRotation);

            _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);

            Vector3 forward = nextRotation * Vector3.forward;

            Vector3 nextPosition = position + forward * (_currentSpeed * Time.fixedDeltaTime);

            nextPosition = ClampToArena(nextPosition);

            _rigidbody.MovePosition(nextPosition);
        }

        /// <summary>
        /// Restricts movement to the playable arena.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ClampToArena(Vector3 position)
        {
            Vector3 offset = position - arenaCenter.position;

            offset.y = 0f;

            // Clamp the vehicle to the edge of the circular arena so it can never drive outside the playable area.
            if (offset.magnitude > arenaRadius)
            {
                offset = offset.normalized * arenaRadius;
                position = arenaCenter.position + offset;
            }

            return position;
        }

        /// <summary>
        /// Gradually slows the vehicle until it comes to a stop.
        /// </summary>
        void Decelerate()
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, deceleration * Time.fixedDeltaTime);

            if (_currentSpeed <= 0f)
                return;

            Vector3 forwardMovement = transform.forward * (_currentSpeed * Time.fixedDeltaTime);

            _rigidbody.MovePosition(_rigidbody.position + forwardMovement);
        }

        /// <summary>
        /// Instantly stops all vehicle movement, used when gameplay is not active.
        /// </summary>
        public void StopImmediately()
        {
            _hasTarget = false;
            _currentSpeed = 0f;

            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Starts and stops the engine sound depending on whether the vehicle is currently moving.
        /// </summary>
        void UpdateEngineAudio()
        {
            if (engineAudio == null)
                return;


            bool moving = _currentSpeed > engineStartSpeed;


            if (moving && !engineAudio.isPlaying)
            {
                engineAudio.Play();
            }
            else if (!moving && engineAudio.isPlaying)
            {
                engineAudio.Stop();
            }
        }

        /// <summary>
        /// Draws the arena boundary in the Scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (arenaCenter == null)
                return;

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(arenaCenter.position, arenaRadius);
        }
    }
}
