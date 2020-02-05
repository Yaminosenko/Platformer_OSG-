using UnityEngine;
using UnityEditor;

namespace FigmentGames
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraPath2D))]
    public class CameraPath2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CameraPath2D path = target as CameraPath2D;

            if (path.curveMode == CameraPath2D.CurveMode.Flat)
            {
                path.curve.axis = BezierCurve.Axis.XY;
                path.curve.axisOffset = BezierCurve.AxisOffset.Value;
                path.curve.axisOffsetValue = 0;
            }
            else
            {
                path.curve.axis = BezierCurve.Axis.All;
            }

            path.curve.axisLock = true;
        }
    }
}