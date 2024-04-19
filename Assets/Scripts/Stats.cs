using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName="ScriptableObjects")]
public class Stats : ScriptableObject
{
    [Header("Input Configuration")]
    public KeyCode[] actionKeys = {KeyCode.X};
    public KeyCode[] runKeys = {KeyCode.Z};

    [Header("Movement")]
    public float walkSpeed = 6;
    public float runSpeed = 8;

    [Header("Multiplier Deltas")]
    public float runSpeedMultDelta = 0.05f;
    public float dragSpeedMultDelta = 0.05f;
    public float killRangeMultDelta = 0.05f;
    public float enduranceMultDelta = -0.05f;

    [Header("Boost Progress Counters")]
    public int runSpeedProgress = 0;
    public int dragSpeedProgress = 0;
    public int killRangeProgress = 0;
    public int enduranceProgress = 0;

    [Header("Multipliers")]
    public float runSpeedMult = 1f;
    public float dragSpeedMult = 1f;
    public float killRangeMult = 1f;
    public float enduranceMult = 1f;

    public LayerMask groundLayer;
}
