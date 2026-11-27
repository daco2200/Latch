// BulletPattern.cs
using System.Collections;
using UnityEngine;

public abstract class BulletPattern : ScriptableObject
{
    public float cooldown = 2.5f;     // base time between uses

    public abstract IEnumerator Execute(PatternContext ctx);

    protected static Vector2 Rotate(Vector2 v, float degrees) =>
        Quaternion.Euler(0f, 0f, degrees) * v;
}