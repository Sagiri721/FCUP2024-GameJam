using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName="ScriptableObjects")]
public class Stats : ScriptableObject
{
    // Player instance stuff
    [Header("Input Configuration")]
    public KeyCode[] jumpKeys = {KeyCode.UpArrow, KeyCode.W};

    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Movement")]
    public float walkSpeed = 6;
    public float runSpeed = 8;

    [Header("Jumping")]
    public float jumpPower = 10;
    public float jumpTime = 0.2f;

    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;

    [Header("Gravity")]
    public float airStall = 0.5f;
    public float gravityPull = 0.5f;
    public float gravityClamp = 20;
    public float jumpApexModifier = 0.7f;

    public LayerMask groundLayer;
}
