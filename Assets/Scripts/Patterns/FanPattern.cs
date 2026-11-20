// FanPattern.cs: several bullets spread over an arc centered on the player
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Latch/Patterns/Fan")]
public class FanPattern : BulletPattern
{
    public int bullets = 3;
    public float arc = 40f;

    public override IEnumerator Execute(PatternContext ctx)
    {
        Vector2 aim = ctx.ToTarget;
        for (int i = 0; i < bullets; i++)
        {
            float t = bullets > 1 ? (float)i / (bullets - 1) : 0.5f;
            ctx.Fire(ctx.Position, Rotate(aim, Mathf.Lerp(-arc * 0.5f, arc * 0.5f, t)));
        }
        yield break;
    }
}