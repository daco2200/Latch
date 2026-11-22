// RuleState.cs
public static class RuleState
{
    public static bool InvertControls;
    public static bool WeaponJammed;

    public static void Clear()
    {
        InvertControls = false;
        WeaponJammed = false;
    }
}