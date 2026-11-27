// PatternContext.cs
using UnityEngine;

public class PatternContext
{
    public Transform source;          // the enemy
    public Transform target;          // the player
    public EnemyBulletPool pool;

    public Vector2 Position => (Vector2)source.position;
    public Vector2 ToTarget => target != null
        ? ((Vector2)target.position - Position).normalized
        : Vector2.down;

    public void Fire(Vector2 position, Vector2 direction) => pool.Fire(position, direction);
}