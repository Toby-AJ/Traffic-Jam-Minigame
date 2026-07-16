using UnityEngine;

namespace TrafficJam.Vehicles
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class VehicleTargetVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        Transform vehicle;
        [SerializeField]
        Transform targetMarker;

        [Header("Settings")]
        [SerializeField]
        float heightOffset = 0.05f;

        LineRenderer _lineRenderer;
        Vector3 _targetPosition;
        bool _isVisible;
        Vector3 _defaultLocalPosition;

        private void Awake()
        {
            // Caches the LineRenderer and stores the marker's default position.

            _lineRenderer = GetComponent<LineRenderer>();

            _lineRenderer.positionCount = 2;

            if (targetMarker != null)
                _defaultLocalPosition = targetMarker.localPosition;

            SetVisible(false);
        }

        /// <summary>
        /// Updates the line renderer and marker position each frame.
        /// </summary>
        private void LateUpdate()
        {
            if (!_isVisible)
                return;

            Vector3 start = vehicle.position;
            Vector3 end = _targetPosition;

            start.y += heightOffset;
            end.y += heightOffset;

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);

            targetMarker.position = end;
        }

        /// <summary>
        /// Displays the target marker at the specified position.
        /// </summary>
        /// <param name="position"></param>
        public void SetTarget(Vector3 position)
        {
            _targetPosition = position;

            SetVisible(true);
        }

        /// <summary>
        /// Hides the target marker and line.
        /// </summary>
        public void Hide()
        {
            SetVisible(false);
        }

        /// <summary>
        /// Enables or disables the target visual.
        /// </summary>
        /// <param name="visible"></param>
        void SetVisible(bool visible)
        {
            _isVisible = visible;

            _lineRenderer.enabled = visible;

            if (targetMarker != null)
                targetMarker.gameObject.SetActive(visible);
        }

        /// <summary>
        /// Restores the target marker to its default state.
        /// </summary>
        public void ResetTarget()
        {
            _targetPosition = vehicle.position;

            if (targetMarker != null)
                targetMarker.localPosition = _defaultLocalPosition;

            Hide();
        }
    }
}
