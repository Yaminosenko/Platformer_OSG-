using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FigmentGames
{
    [RequireComponent(typeof(Camera))]
    public partial class CameraController2D : EnhancedMonoBehaviour
    {
        [SerializeField, HideInInspector] private Camera _camera;
        public new Camera camera
        {
            get
            {
                if (!_camera)
                    _camera = GetComponent<Camera>();

                return _camera;
            }
        }

        [Space(10)]
        [Header("BEHAVIOUR")]
        [Tooltip(
            "The list of all the know virtual cameras. The controller automatically follows the virtual camera with the highest priority." +
            "\n\nN.B: At runtime, virtual cameras should be set and removed with the CameraController2D methods AddVirtualCamera and RemoveVirtualCamera.")]
        [SerializeField] private List<VirtualCamera> _virtualCameras = new List<VirtualCamera>();
        public List<VirtualCamera> virtualCameras { get { return _virtualCameras; } }
        [Space]
        [Tooltip("The current constraint applied to the controller.")]
        [SerializeField] private CameraConstraint2D constraint;

        // Cache
        private VirtualCamera currentVirtualCamera;

        [SerializeField, HideInInspector] private Vector3 _virtualTargetPoint;
        public Vector3 virtualTargetPoint { get { return _virtualTargetPoint; } private set { _virtualTargetPoint = value; } }
        private float targetFOV;
        private Coroutine blendCoroutine;

        [SerializeField, HideInInspector] private Vector3 constraintTargetPoint;
        private bool blendOutConstraint;
        private Coroutine constraintBlendCoroutine;

        public Vector2 frustum { get { return EnhancedMath.GetCameraFrustumAtDistance(camera, Mathf.Abs(position.z)); } }

        #region UNITY

        private void Awake()
        {
            GetCurrentVirtualCamera();
            CancelVirtualTargetPointBlend();

            if (currentVirtualCamera)
                return;

            virtualTargetPoint = position;
            targetFOV = camera.fieldOfView;


        }

        private void LateUpdate()
        {
            UpdateTargetPoints();
            SnapToTargetPoint();
        }

        #endregion

        #region VIRTUAL CAMERA

        public void AddVirtualCamera(VirtualCamera virtualCamera)
        {
            // Camera is already in the list
            if (virtualCameras.Contains(virtualCamera))
                return;

            virtualCameras.Add(virtualCamera);
            GetCurrentVirtualCamera();

            Camera2DEvents.OnVirtualCameraAdded?.Invoke(this, virtualCamera);
        }

        public void RemoveVirtualCamera(VirtualCamera virtualCamera)
        {
            // Camera is not in the list
            if (!virtualCameras.Contains(virtualCamera))
                return;

            virtualCameras.Remove(virtualCamera);

            // It was the last camera from the list
            if (virtualCameras.Count == 0)
                CancelVirtualTargetPointBlend();

            GetCurrentVirtualCamera();

            Camera2DEvents.OnVirtualCameraRemoved?.Invoke(this, virtualCamera);
        }

        private void GetCurrentVirtualCamera()
        {
            int count = virtualCameras.Count;

            if (count == 0)
            {
                currentVirtualCamera = null;
                return;
            }

            // Get virtual camera by priority
            int index = -1;
            int priority = -1;
            for (int i = 0; i < count; i++)
            {
                // Missing VirtualCamera
                if (!virtualCameras[i])
                    continue;

                if (virtualCameras[i].priority > priority)
                {
                    priority = virtualCameras[i].priority;
                    index = i;
                }
            }

            // No camera found in the list
            if (index == -1)
            {
                currentVirtualCamera = null;
                return;
            }

            // Assign new virtual camera and blend towards its position
            VirtualCamera newVirtualCamera = virtualCameras[index];

            if (currentVirtualCamera != newVirtualCamera)
            {
                currentVirtualCamera = newVirtualCamera;
                BlendVirtualTargetPoint();

                Camera2DEvents.OnVirtualCameraSet?.Invoke(this, currentVirtualCamera);
            }
        }


        private void BlendVirtualTargetPoint()
        {
            this.StopAndStartCoroutine(ref blendCoroutine, CoBlendVirtualTargetPoint());
        }

        private IEnumerator CoBlendVirtualTargetPoint()
        {
            Vector3 blendStartPosition = virtualTargetPoint;
            float duration = currentVirtualCamera.blendCurve.GetDuration();

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = currentVirtualCamera.blendCurve.Evaluate(t);

                virtualTargetPoint = Vector3.Lerp(blendStartPosition, currentVirtualCamera.GetControllerAnchor(this), lerp);
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, currentVirtualCamera.FOV, lerp);

                yield return null;
            }

            CancelVirtualTargetPointBlend();
        }

        public void CancelVirtualTargetPointBlend()
        {
            this.DestroyCoroutine(ref blendCoroutine);

            if (!currentVirtualCamera)
                return;

            virtualTargetPoint = currentVirtualCamera.GetControllerAnchor(this);
            camera.fieldOfView = currentVirtualCamera.FOV;

            SnapToTargetPoint();
        }

        #endregion

        #region CONSTRAINT

        public void SetConstraint(CameraConstraint2D constraint)
        {
            if (!constraint)
                return;

            // Constraint already set
            if (!blendOutConstraint && this.constraint == constraint)
                return;

            this.constraint = constraint;

            BlendInConstraintTargetPoint();

            Camera2DEvents.OnCameraConstraint2DSet?.Invoke(this, constraint);
        }

        public void RemoveConstraint(CameraConstraint2D constraint)
        {
            if (!constraint)
                return;

            if (this.constraint == constraint)
                RemoveConstraint();
        }

        public void RemoveConstraint()
        {
            BlendOutConstraintTargetPoint();

            Camera2DEvents.OnCameraConstraint2DRemoved?.Invoke(this, constraint);
        }


        private void BlendInConstraintTargetPoint()
        {
            this.StopAndStartCoroutine(ref constraintBlendCoroutine, CoBlendInConstraintTargetPoint());
        }

        private IEnumerator CoBlendInConstraintTargetPoint()
        {
            float duration = constraint.blendInCurve.GetDuration();

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = constraint.blendInCurve.Evaluate(t);

                constraintTargetPoint = Vector3.Lerp(virtualTargetPoint, constraint.GetConstraintPosition(this), lerp);

                yield return null;
            }

            CancelConstraintTargetPointBlendIn();
        }

        public void CancelConstraintTargetPointBlendIn()
        {
            this.DestroyCoroutine(ref constraintBlendCoroutine);

            constraintTargetPoint = constraint.GetConstraintPosition(this);

            SnapToTargetPoint();
        }


        private void BlendOutConstraintTargetPoint()
        {
            this.StopAndStartCoroutine(ref constraintBlendCoroutine, CoBlendOutConstraintTargetPoint());
        }

        private IEnumerator CoBlendOutConstraintTargetPoint()
        {
            float duration = constraint.blendOutCurve.GetDuration();
            blendOutConstraint = true;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = constraint.blendOutCurve.Evaluate(t);

                constraintTargetPoint = Vector3.Lerp(virtualTargetPoint, constraint.GetConstraintPosition(this), lerp);

                yield return null;
            }

            CancelConstraintTargetPointBlendOut();
        }

        public void CancelConstraintTargetPointBlendOut()
        {
            this.DestroyCoroutine(ref constraintBlendCoroutine);

            constraintTargetPoint = virtualTargetPoint;
            blendOutConstraint = false;
            constraint = null;

            SnapToTargetPoint();
        }


        #endregion

        #region BEHAVIOUR

        private void UpdateTargetPoints()
        {
            if (blendCoroutine == null && currentVirtualCamera)
                virtualTargetPoint = currentVirtualCamera.GetControllerAnchor(this);

            if (!constraint)
                constraintTargetPoint = virtualTargetPoint;
            else if (constraintBlendCoroutine == null)
                constraintTargetPoint = constraint.GetConstraintPosition(this);
        }

        private void SnapToTargetPoint()
        {
            position = constraintTargetPoint;
        }

        #endregion
    }
}