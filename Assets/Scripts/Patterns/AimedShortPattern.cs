// AimedShotPattern.cs: one bullet at the player with a small random deviation
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Latch/Patterns/Aimed Shot")]
public class AimedShotPattern : BulletPattern
{
    public float spread = 8f;

    public override IEnumerator Execute(PatternContext ctx)
    {
        ctx.Fire(ctx.Position, Rotate(ctx.ToTarget, Random.Range(-spread, spread)));
        yield break;
    }
}