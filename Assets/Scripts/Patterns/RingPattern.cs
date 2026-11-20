// RingPattern.cs: a circle of bullets (good for the elite later)
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Latch/Patterns/Ring")]
public class RingPattern : BulletPattern
{
    public int bullets = 12;

    public override IEnumerator Execute(PatternContext ctx)
    {
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < bullets; i++)
            ctx.Fire(ctx.Position, Rotate(Vector2.down, offset + 360f * i / bullets));
        yield break;
    }
}