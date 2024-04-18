using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName="ScriptableObjects")]
public class Stats : ScriptableObject
{
    [Header("Input Configuration")]
    public KeyCode[] jumpKeys = {KeyCode.UpArrow, KeyCode.W};
    public KeyCode[] runKeys = {KeyCode.LeftShift};

    [Header("Movement")]
    public float walkSpeed = 6;
    public float runSpeed = 8;

    public LayerMask groundLayer;
}
