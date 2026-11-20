// CurtainPattern.cs: a wall of bullets across the arena with a gap you must reach
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Latch/Patterns/Curtain")]
public class CurtainPattern : BulletPattern
{
    public int columns = 9;
    public int gapWidth = 2;
    public float halfWidth = 8f;     // arena half-width, world units
    public float centerX = 0f;

    public override IEnumerator Execute(PatternContext ctx)
    {
        int gapStart = Random.Range(0, columns - gapWidth + 1);
        float y = ctx.Position.y;

        for (int i = 0; i < columns; i++)
        {
            if (i >= gapStart && i < gapStart + gapWidth) continue;
            float t = columns > 1 ? (float)i / (columns - 1) : 0.5f;
            float x = Mathf.Lerp(centerX - halfWidth, centerX + halfWidth, t);
            ctx.Fire(new Vector2(x, y), Vector2.down);
        }
        yield break;
    }
}