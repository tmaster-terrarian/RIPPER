using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Utils
{
    /**
     * <summary>Clamps the given value between a range defined by the given minimum Vector3 and maximum Vector3 values. Returns the given value if each axis is within min and max.</summary>
     * <param name="value">The Vector3 to restrict inside the min-to-max range.</param>
     * <param name="min">The minimum Vector3 to compare against.</param>
     * <param name="min">The maximum Vector3 to compare against.</param><returns>The Vector3 result between min and max values.</returns>
     */
    public static Vector3 ClampVector(Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y), Mathf.Clamp(value.z, min.z, max.z));
    }

    /**
     * <summary>Clamps the given value between a range defined by the given minimum Vector2 and maximum Vector2 values. Returns the given value if each axis is within min and max.</summary>
     * <param name="value">The Vector2 to restrict inside the min-to-max range.</param>
     * <param name="min">The minimum Vector2 to compare against.</param>
     * <param name="min">The maximum Vector2 to compare against.</param><returns>The Vector2 result between min and max values.</returns>
     */
    public static Vector2 ClampVector(Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
    }

    public static float Approach(float value, float target, float rate)
    {
        if (value < target)
            return Mathf.Min((value + rate), target);
        else
            return Mathf.Max((value - rate), target);
    }

    public static int Approach(int value, int target, int rate)
    {
        if (value < target)
            return Mathf.Min((value + rate), target);
        else
            return Mathf.Max((value - rate), target);
    }
}
