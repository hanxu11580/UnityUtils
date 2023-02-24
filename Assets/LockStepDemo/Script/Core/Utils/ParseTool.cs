﻿using UnityEngine;
using System.Collections;
using System;

public class ParseTool : MonoBehaviour 
{

	public static Vector2 String2Vector2(string value)
    {
        try
        {
            string[] values = value.Split('|');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);

            return new Vector2(x, y);
        }
        catch (Exception e)
        {
            throw new Exception("ParseVector2: Don't convert value to Vector2 value:" + value + "\n" + e.ToString()); // throw  
        }
    }

    public static Vector3 String2Vector3(string value)
    {
        try
        {
            string[] values = value.Split('|');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);

            return new Vector3(x, y, z);
        }
        catch (Exception e)
        {
            throw new Exception("ParseVector3: Don't convert value to Vector3 value:" + value + "\n" + e.ToString()); // throw  
        }
    }

    public static Color String2Color(string value)
    {
        try
        {
            string[] values = value.Split('|');
            float r = float.Parse(values[0]);
            float g = float.Parse(values[1]);
            float b = float.Parse(values[2]);
            float a = 1;

            if (values.Length > 3)
            {
                a = float.Parse(values[3]);
            }

            return new Color(r, g, b, a);
        }
        catch (Exception e)
        {
            throw new Exception("ParseColor: Don't convert value to Color value:" + value + "\n" + e.ToString()); // throw  
        }
    }

    static string[] c_NullStringArray = new string[0];

    public static string[] String2StringArray(string value)
    {
        if (value != null
                && value != ""
                && value != "null"
                && value != "Null"
                && value != "NULL"
                && value != "None")
        {
            return value.Split('|');
        }
        else
        {
            return c_NullStringArray;
        }

    }

    public static int[] String2IntArray(string value)
    {
        int[] intArray = null;
        if (!string.IsNullOrEmpty(value))
        {
            string[] strs = value.Split('|');
            intArray = Array.ConvertAll(strs, s => int.Parse(s));
            return intArray;
        }
        else
        {
            return new int[0];
        }
    }
}
