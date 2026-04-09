using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(HUDWeaponInfo), nameof(HUDWeaponInfo.LateUpdate))]
public class HUDAmmoDisplayPatcher
{

    [HarmonyPostfix]
    public static void Postfix(HUDWeaponInfo __instance)
    {
        
        if (!(__instance.weapon != null && __instance.weapon.itemActivated))
        {
            return;
        }

        int currentCount = 0;
        int spareCount = 0;

        var firstCombEquip = __instance.wm.combinedEquips[0];
        
        if (firstCombEquip is RocketLauncher)
        {
            // Log("Player Equipped Rocket launcher");
            foreach (var rl in __instance.wm.combinedEquips.Cast<RocketLauncher>())
            {
                spareCount += RocketLauncherPatcher.GetMagazine(rl);
                currentCount += rl.GetCount();
            }
        }
        else if (firstCombEquip is HPEquipMissileLauncher)
        {
            // Log("Player Equipped Missile Launcher");
            foreach (var mlhp in __instance.wm.combinedEquips.Cast<HPEquipMissileLauncher>())
            {
                spareCount += MissileLauncherPatcher.GetMagazine(mlhp.ml);
                currentCount += mlhp.GetCount();
            }
        }
        else if (firstCombEquip is HPEquipGun)
        {
            // Log("Player Equipped Gun");
            foreach (var ghp in __instance.wm.combinedEquips.Cast<HPEquipGun>())
            {
                spareCount += GunPatcher.GetMagazine(ghp.gun);
                currentCount += ghp.GetCount();
            }
        }
        else
        {
            // Log($"Player switched to unknown weapon type {firstCombEquip.GetType()}");
            return;
        }
        
        if (spareCount == 0)
        {
            return;
        }

        __instance.ammoCountText.text = $"{currentCount} ({spareCount})";
    }
}