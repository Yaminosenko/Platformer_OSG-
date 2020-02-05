using System;
using UnityEngine;

namespace FigmentGames
{
    public abstract class CameraConstraint2D : EnhancedMonoBehaviour
    {
        [Space(10)]
        [Header("CONSTRAINT BLEND")]
        [Tooltip("This curve represents how smoothly the constraint will be applied to the CameraController2D when set.")]
        [SerializeField] private AnimationCurve _blendInCurve = EnhancedMath.easeIn;
        public AnimationCurve blendInCurve { get { return _blendInCurve; } }

        [Tooltip("This curve represents how smoothly the CameraController2D will return to its normal behaviour when freed from this constraint.\n\nIf a new constraint is set in the meantime, this \"smooth out\" behaviour will be ignored.")]
        [SerializeField] private AnimationCurve _blendOutCurve = EnhancedMath.easeOut;
        public AnimationCurve blendOutCurve { get { return _blendOutCurve; } }

        private void OnEnable()
        {
            Camera2DEvents.OnCameraConstraint2DSet += ConstraintSet;
        }

        private void OnDisable()
        {
            Camera2DEvents.OnCameraConstraint2DSet -= ConstraintSet;
        }

        private void ConstraintSet(CameraController2D controller, CameraConstraint2D constraint)
        {
            if (constraint != this)
                return;

            ControllerAssigned(controller);
        }

        protected virtual void ControllerAssigned(CameraController2D controller) { }
        public virtual Vector3 GetConstraintPosition(CameraController2D controller) { return Vector3.zero; }
    }
}