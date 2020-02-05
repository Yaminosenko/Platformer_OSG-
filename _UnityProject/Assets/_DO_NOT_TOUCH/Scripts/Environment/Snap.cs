using UnityEngine;

public class Snap : MonoBehaviour
{
    public static Vector3 SnapToGrid(Vector3 pos, float xValue = .5f, float yValue = .5f, bool constantZ = true, float zValue = .5f)
    {
        pos.x = SnapToGrid(pos.x, xValue);
        pos.y = SnapToGrid(pos.y, yValue);

        pos.z = constantZ ? 0 : SnapToGrid(pos.z, zValue);

        return pos;
    }

    private static float SnapToGrid(float v, float size)
    {
        v += 0.5f * size * Mathf.Sign(v);
        return v - v % size;
    }
}
