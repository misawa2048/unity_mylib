// https://unity3d.com/jp/learn/tutorials/topics/scripting/extension-methods
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using nn;

public static class TmExtensionMethods
{

#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE 
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
#endif //#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE
}

