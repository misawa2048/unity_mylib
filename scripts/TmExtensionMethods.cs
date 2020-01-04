// https://unity3d.com/jp/learn/tutorials/topics/scripting/extension-methods
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using nn;

public static class TmExtensionMethods
{

    /// <summary>
    /// From Vector3(x,y,z) to Vector2(x,z) 
    /// </summary>
    public static UnityEngine.Vector2 ToVec2XZ(this UnityEngine.Vector3 v)
    {
        return new UnityEngine.Vector2(v.x, v.y);
    }

    /// <summary>
    /// From Vector2(x,y) To Vector3(x,0,y) 
    /// </summary>
    public static UnityEngine.Vector3 ToVec3X0Y(this UnityEngine.Vector2 v)
    {
        return new UnityEngine.Vector3(v.x, 0f, v.y);
    }

    /// <summary>
    /// Get random range value of Vector2
    /// </summary>
    public static UnityEngine.Vector2 Random(this UnityEngine.Vector2 min, UnityEngine.Vector2 max)
    {
        return new UnityEngine.Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
    }

    /// <summary>
    /// Get random range value of Vector3
    /// </summary>
    public static UnityEngine.Vector3 Random(this UnityEngine.Vector3 min, UnityEngine.Vector3 max)
    {
        return new UnityEngine.Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
    }

#if UNITY_EDITOR
#endif // 
#if UNITY_SWITCH || NN_PLUGIN_ENABLE // || UNITY_EDITOR
    public static UnityEngine.Vector2 F2V(this nn.util.Float2 f)
    {
        return new UnityEngine.Vector3(f.x, f.y);
    }
    public static UnityEngine.Vector3 F2V(this nn.util.Float3 f)
    {
        return new UnityEngine.Vector3(f.x, f.y, f.z);
    }
    public static UnityEngine.Quaternion F2Q(this nn.util.Float4 f)
    {
        return new UnityEngine.Quaternion(f.x, f.z, f.y, -f.w);
    }
#endif //#if UNITY_SWITCH || NN_PLUGIN_ENABLE // || UNITY_EDITOR 
}

