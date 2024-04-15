using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class Utils : MonoBehaviour
{
    public static bool GetKeyDownAll(KeyCode[] keys){ return keys.Count(k => Input.GetKeyDown(k)) > 0; }
    public static bool GetKeyAll(KeyCode[] keys){ return keys.Count(k => Input.GetKey(k)) > 0; }
    public static bool GetKeyUpAll(KeyCode[] keys){ return keys.Count(k => Input.GetKeyUp(k)) > 0; }
}
