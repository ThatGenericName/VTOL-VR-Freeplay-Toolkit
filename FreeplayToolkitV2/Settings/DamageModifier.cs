using ModLoader.Framework.Settings;

namespace FreeplayToolkitV2.Settings;

public class DamageModifier : BaseSettings
{
    public float AllyDamageMultiplier = 1.0f;
    public float SelfDamageMultiplier = 1.0f;
    public float EnemyDamageMultiplier = 1.0f;

    public void PrintoutCurrentSettings()
    {
        Log($"Ally Damage Multiplier: {AllyDamageMultiplier}");
        Log($"Self Damage Multiplier: {SelfDamageMultiplier}");
        Log($"Enemy Damage Multiplier: {EnemyDamageMultiplier}");
    }
}