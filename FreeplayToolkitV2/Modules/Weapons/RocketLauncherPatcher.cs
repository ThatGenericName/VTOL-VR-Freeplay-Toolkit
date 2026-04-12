using System;
using System.Collections;
using System.Collections.Generic;
using FreeplayToolkitV2.Settings;
using HarmonyLib;
using UnityEngine;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(RocketLauncher), nameof(RocketLauncher.FireRocket))]
public class RocketLauncherPatcher
{

    [HarmonyPrefix]
    public static bool Prefix(RocketLauncher __instance, ref Actor ___parentActor)
    {
        CurrentIDX = __instance.currentIdx;
        
        return true;
    }

    // I dunno if it's the version of harmony used or a new bug, but the harmony version used in VTOLVR suffers
    // from the bug where the __state parameter to pass data between prefixes and postfixes don't work.
    // Luckily, afaik everything I am patching runs synchronously without multiple instances in parallel
    // so I can just assume nothing else modifies shared states.
    private static int CurrentIDX;
    
    public static WaitForSeconds AmmoReloadWait => new(Main.MunitionsModifier.ReloadTime);
    
    private class ReloadCoroutineClass
    {
        public RocketLauncher instance;
        public int mIdx;
        
        public IEnumerator ReloadCoroutine()
        {
            yield return (object) AmmoReloadWait;
            instance.LoadRocket(mIdx);
            ConsumeFromMagazine(instance, 1);
        }

        public ReloadCoroutineClass(RocketLauncher instance, int midx)
        {
            this.instance = instance;
            mIdx = midx;
        }
    }
        
    [HarmonyPostfix]
    public static void Postfix(RocketLauncher __instance)
    {
        if (MultiplayerLock.IsMultiplayer)
        {
            return;
        }
        
        if (VTScenario.current == null || !__instance.parentActor.isPlayer)
        {
            return;
        }
        
        if (VTScenario.current.infiniteAmmo)
        {
            return;
        }

        if (Main.MunitionsModifier.InfiniteAmmo)
        {
            VTScenario.current.infAmmoReloadDelay = Main.MunitionsModifier.ReloadTime;
            __instance.StartCoroutine(__instance.InfReloadRoutine());
        }
        else if (Main.MunitionsModifier.HasReloads)
        {
            int launcherId = __instance.weaponEntityID;
            Log($"Checking Rocket Launcher Id {launcherId} for reloads");

            var magazine = GetMagazine(__instance);
            Log($"Rocket Launcher Id {launcherId} has {magazine} reloads remaining");
            if (magazine > 0)
            {
                var reloadCoroutineClass = new ReloadCoroutineClass(__instance, CurrentIDX);
                __instance.StartCoroutine(reloadCoroutineClass.ReloadCoroutine());
            }
        }
        
        
        return;
    }

    public static int GetMagazine(RocketLauncher instance)
    {
        if (!instance.parentActor.isPlayer)
        {
            return 0;
        }
        
        if (!MunitionsManager.RocketMagazineTracker.TryGetValue(instance, out int magazine))
        {
            magazine = instance.fireTransforms.Length * Main.MunitionsModifier.ReloadCount;
        }

        return magazine;
    }

    public static int ConsumeFromMagazine(RocketLauncher instance, int amount)
    {
        if (!MunitionsManager.RocketMagazineTracker.TryGetValue(instance, out int magazine))
        {
            return 0;
        }

        int realAmount = Math.Min(amount, magazine);
        MunitionsManager.RocketMagazineTracker[instance] -= realAmount;
        return realAmount;
    }
}