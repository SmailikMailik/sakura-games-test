using UnityEngine;

namespace Project.Runtime.Space
{
    /// <summary>
    /// Кеширует границы обзора камеры на плоскости XZ до изменения её проекции или положения.
    /// </summary>
    internal sealed class CameraWorldArea
    {
        private readonly Camera _camera;
        private readonly Plane _groundPlane = new(Vector3.up, Vector3.zero);
        private Matrix4x4 _cachedProjectionMatrix;
        private Matrix4x4 _cachedViewMatrix;
        private Rect _cachedBounds;
        private bool _hasBounds;

        internal Rect Bounds
        {
            get
            {
                var projectionMatrix = _camera.projectionMatrix;
                var viewMatrix = _camera.worldToCameraMatrix;
                if (_hasBounds && projectionMatrix == _cachedProjectionMatrix && viewMatrix == _cachedViewMatrix)
                    return _cachedBounds;

                _cachedProjectionMatrix = projectionMatrix;
                _cachedViewMatrix = viewMatrix;
                _cachedBounds = CalculateBounds();
                _hasBounds = true;
                return _cachedBounds;
            }
        }

        internal CameraWorldArea(Camera camera)
        {
            _camera = camera;
        }

        private Rect CalculateBounds()
        {
            var bottomLeft = ProjectToGround(Vector3.zero);
            var topRight = ProjectToGround(Vector3.one);

            return Rect.MinMaxRect(
                Mathf.Min(bottomLeft.x, topRight.x),
                Mathf.Min(bottomLeft.z, topRight.z),
                Mathf.Max(bottomLeft.x, topRight.x),
                Mathf.Max(bottomLeft.z, topRight.z));
        }

        private Vector3 ProjectToGround(Vector3 viewportPoint)
        {
            var ray = _camera.ViewportPointToRay(viewportPoint);
            _groundPlane.Raycast(ray, out var distance);
            return ray.GetPoint(distance);
        }
    }
}