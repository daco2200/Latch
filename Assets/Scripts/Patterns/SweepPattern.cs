// SweepPattern.cs: a line of bullets that rotates from one side to the other
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Latch/Patterns/Sweep")]
public class SweepPattern : BulletPattern
{
    public int bullets = 10;
    public float stepTime = 0.08f;
    public float arc = 80f;          // total, centered on straight down

    public override IEnumerator Execute(PatternContext ctx)
    {
        bool leftToRight = Random.value < 0.5f;
        for (int i = 0; i < bullets; i++)
        {
            float t = bullets > 1 ? (float)i / (bullets - 1) : 0.5f;
            if (!leftToRight) t = 1f - t;
            ctx.Fire(ctx.Position, Rotate(Vector2.down, Mathf.Lerp(-arc * 0.5f, arc * 0.5f, t)));
            yield return new WaitForSeconds(stepTime);
        }
    }
}