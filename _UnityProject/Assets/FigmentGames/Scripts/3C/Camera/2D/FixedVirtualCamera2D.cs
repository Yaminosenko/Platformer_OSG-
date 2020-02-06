using UnityEngine;

namespace FigmentGames
{
    public partial class FixedVirtualCamera2D : VirtualCamera
    {
        [Tooltip("The distance the camera is placed at.")]
        [SerializeField] [MinValue(0)] private float cameraDistance = 32f;
        [Tooltip("The frustum width the camera must match, taking care of its FOV and aspect ratio.")]
        [SerializeField] [MinValue(0)] private float frustumWidth = 40f;
        [Tooltip("The frustum height the camera must match, taking care of its FOV and aspect ratio.")]
        [SerializeField] [MinValue(0)] private float frustumHeight = 20f;

        FixedVirtualCamera2D()
        {
            priority = 1;
        }

        public override Vector3 GetControllerAnchor(CameraController2D controller)
        {
            float controllerAnchorZPos = 0f;

            switch (distanceCalculation)
            {
                case DistanceCalculation.Simple:
                    controllerAnchorZPos = -cameraDistance;
                    break;

                case DistanceCalculation.FrustumWidth:
                    controllerAnchorZPos = -EnhancedMath.GetDistanceFromFrustumHeight(FOV, frustumWidth / controller.camera.aspect);
                    break;

                case DistanceCalculation.FrustumHeight:
                    controllerAnchorZPos = -EnhancedMath.GetDistanceFromFrustumHeight(FOV, frustumHeight);
                    break;
            }

            return position.ZValue(controllerAnchorZPos);
        }
    }
}