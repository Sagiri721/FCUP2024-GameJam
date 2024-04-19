using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName="EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    public float wanderSpeed = 6f;
    public float chaseSpeed = 10f;
    public float tweenRate = 6f;

    [Header("AI")]
    [Range(0, 360)]
    public float checkAngle = 60;
    public float checkDistance = 4f;

    public bool hasRestTime = true;
    public float restTime = 1f;

    public float killRadius = 2f;

    public LayerMask visionMask, collisionLayer;

    public float rotTime = 10f;
}