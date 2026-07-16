using TrafficJam.Core;
using UnityEngine;

namespace TrafficJam.Vehicles
{
    [RequireComponent(typeof(VehicleController))]
    public sealed class HumanVehicleInput : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        Camera gameplayCamera;
        [SerializeField]
        LayerMask playgroundLayer;

        [Header("Visual")]
        [SerializeField]
        VehicleTargetVisual targetVisual;

        VehicleController _vehicle;

        private void Awake()
        {
            _vehicle = GetComponent<VehicleController>();

            if (gameplayCamera == null)
                gameplayCamera = Camera.main;
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                return;
            }

            HandlePointerInput();
        }

        /// <summary>
        /// Converts mouse input into vehicle movement targets.
        /// </summary>
        void HandlePointerInput()
        {
            if (Input.GetMouseButtonUp(0))
            {
                _vehicle.Stop();

                if (targetVisual != null)
                    targetVisual.ResetTarget();

                return;
            }

            if (!Input.GetMouseButton(0))
                return;

            Ray ray = gameplayCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, playgroundLayer))
            {
                return;
            }

            _vehicle.SetTargetPosition(hit.point);

            if (targetVisual != null)
                targetVisual.SetTarget(hit.point);
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
        /// Hides the movement target when gameplay ends.
        /// </summary>
        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Finished)
            {
                if (targetVisual != null)
                    targetVisual.Hide();
            }
        }
    }
}
