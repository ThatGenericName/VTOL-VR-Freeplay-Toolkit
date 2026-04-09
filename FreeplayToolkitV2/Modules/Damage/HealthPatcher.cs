using HarmonyLib;

namespace FreeplayToolkitV2.Modules.Damage;


[HarmonyPatch(typeof(Health), nameof(Health.Damage))]
public class HealthPatcher
{
    [HarmonyPrefix]
    public static bool Prefix(Health __instance, ref float damage)
    {
        
        Log("Running HP Patcher");
        
        if (__instance.actor == null)
        {
            return true;
        }

        // check if it's a player, enemy or a ally.

        if (__instance.actor.isPlayer)
        {
            Log("Damage is on Player");
            float newDamage = damage * Main.DamageModifier.SelfDamageMultiplier;
            Log($"Damage: {damage} -> {newDamage}");
            damage = newDamage;
        }
        else
        {
            // not player, enemy or ally?
            if (__instance.actor.team == Teams.Allied)
            {
                Log("Damage is on Allied");
                float newDamage = damage * Main.DamageModifier.AllyDamageMultiplier;
                Log($"Damage: {damage} -> {newDamage}");
                damage = newDamage;
            }
            else
            {
                Log("Damage is on Enemy");
                float newDamage = damage * Main.DamageModifier.EnemyDamageMultiplier;
                Log($"Damage: {damage} -> {newDamage}");
                damage = newDamage;
            }
        }

        return true;
    }
}