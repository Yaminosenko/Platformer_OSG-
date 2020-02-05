using System;

namespace FigmentGames
{
    public class Camera2DEvents
    {
        public static Action<CameraController2D, VirtualCamera> OnVirtualCameraAdded;
        public static Action<CameraController2D, VirtualCamera> OnVirtualCameraRemoved;
        public static Action<CameraController2D, VirtualCamera> OnVirtualCameraSet;

        public static Action<CameraController2D, CameraConstraint2D> OnCameraConstraint2DSet;
        public static Action<CameraController2D, CameraConstraint2D> OnCameraConstraint2DRemoved;
    }
}