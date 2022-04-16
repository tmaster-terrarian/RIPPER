using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Utils
{
    ///<summary>Convert a bool value to a 1 or a 0.</summary><param name="b">The bool to convert.</param><returns>A float of either 1 or 0.</returns>
    public static float BoolToFloat(bool b)
    {
        if(b == true)
        {
            return 1.0f;
        }
        else if(b == false)
        {
            return 0.0f;
        }
        else
        {
            Debug.Log(new System.NullReferenceException());
            return 0.0f;
        }
    }

    ///<summary>Convert a bool value to a 1 or a 0.</summary><param name="b">The bool to convert.</param><returns>An int of either 1 or 0.</returns>
    public static int BoolToInt(bool b)
    {
        if(b == true)
        {
            return 1;
        }
        else if(b == false)
        {
            return 0;
        }
        else
        {
            Debug.Log(new System.NullReferenceException());
            return 0;
        }
    }

    ///<summary>Convert a float value to true or false. Returns true if the float is equal to or greater than 1.</summary><param name="f">The float to convert.</param><returns>A bool of either true or false.</returns>
    public static bool FloatToBool(float f)
    {
        if(f >= 1.0f)
        {
            return true;
        }
        else if(f < 1.0f)
        {
            return false;
        }
        else
        {
            Debug.Log(new System.NullReferenceException());
            return false;
        }
    }

    ///<summary>Convert a float value to true or false. Returns true if the int is equal to or greater than 1.</summary><param name="i">The int to convert.</param><returns>A bool of either true or false.</returns>
    public static bool IntToBool(int i)
    {
        if(i >= 1)
        {
            return true;
        }
        else if(i < 1)
        {
            return false;
        }
        else
        {
            Debug.Log(new System.NullReferenceException());
            return false;
        }
    }

    ///<summary>Clamps the given value between a range defined by the given minimum Vector3 and maximum Vector3 values. Returns the given value if each axis is within min and max.</summary><param name="value">The Vector3 to restrict inside the min-to-max range.</param><param name="min">The minimum Vector3 to compare against.</param><param name="min">The maximum Vector3 to compare against.</param><returns>The Vector3 result between min and max values.</returns>
    public static Vector3 ClampVector3(Vector3 value, Vector3 min, Vector3 max)
    {
        if(value.x < min.x)
        {
            value.x = min.x;
            return value;
        }
        else if(value.y < min.y)
        {
            value.y = min.y;
            return value;
        }
        else if(value.z < min.z)
        {
            value.z = min.z;
            return value;
        }
        else if(value.x > max.x)
        {
            value.x = max.x;
            return value;
        }
        else if(value.y > max.y)
        {
            value.y = max.y;
            return value;
        }
        else if(value.z > max.z)
        {
            value.z = max.z;
            return value;
        }
        else
        {
            return value;
        }
    }

    ///<summary>Clamps the given value between a range defined by the given minimum Vector2 and maximum Vector2 values. Returns the given value if each axis is within min and max.</summary><param name="value">The Vector2 to restrict inside the min-to-max range.</param><param name="min">The minimum Vector2 to compare against.</param><param name="min">The maximum Vector2 to compare against.</param><returns>The Vector2 result between min and max values.</returns>
    public static Vector3 ClampVector2(Vector2 value, Vector2 min, Vector2 max)
    {
        if(value.x < min.x)
        {
            value.x = min.x;
            return value;
        }
        else if(value.y < min.y)
        {
            value.y = min.y;
            return value;
        }
        else if(value.x > max.x)
        {
            value.x = max.x;
            return value;
        }
        else if(value.y > max.y)
        {
            value.y = max.y;
            return value;
        }
        else
        {
            return value;
        }
    }

    ///<summary>Sets the float to absolute value. (If the float is negative => set to positive, else return the float)</summary><param name="value">The float.</param><returns>The absolute value of the float.</returns>
    public static float AbsoluteValue(float value)
    {
        if(value < 0)
        {
            value *= -1;
            return value;
        }
        else return value;
    }
    
    ///<summary>Sets the int to absolute value. (If the int is negative => set to positive, else return the int)</summary><param name="value">The int.</param><returns>The absolute value of the int.</returns>
    public static int AbsoluteValue(int value)
    {
        if(value < 0)
        {
            value *= -1;
            return value;
        }
        else return value;
    }

    
}
