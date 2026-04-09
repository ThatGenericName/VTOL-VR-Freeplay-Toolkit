using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FreeplayToolkitV2.Settings;
using HarmonyLib;
using UnityEngine;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(Gun), nameof(Gun.FireBullet))]
public class GunPatcher
{

    [HarmonyPrefix]
    public static bool Prefix(Gun __instance)
    {
        Log("FireBullet patch running");
        Log($"Starting Ammo: {StartAmmo}");
        StartAmmo = __instance.currentAmmo;
        return true;
    }

    private static int StartAmmo = 0;
    
    public static WaitForSeconds AmmoReloadWait => new(Main.MunitionsModifier.ReloadTime);
    
    private class ReloadCoroutineClass
    {
        public Gun instance;
        
        public IEnumerator ReloadCoroutine()
        {
            Log("Started Gun Reload Coroutine");
            Log($"Waiting for {Main.MunitionsModifier.ReloadTime} seconds");
            yield return AmmoReloadWait;
            Log("Gun Reload Wait Finished");
            // calculate how many bullets we can actually reload
            if (MunitionsManager.GunMagazineTracker.TryGetValue(instance, out var magazine))
            {
                int emptySpace = instance.maxAmmo - instance.currentAmmo;
                int refill = ConsumeFromMagazine(instance, emptySpace);
                instance.currentAmmo += refill;
                
            }
            
            ActiveCoroutines.Remove(instance);
        }

        public ReloadCoroutineClass(Gun instance)
        {
            this.instance = instance;
        }
    }

    private static Dictionary<Gun, ReloadCoroutineClass> ActiveCoroutines = new();
    
    [HarmonyPostfix]
    public static void Postfix(Gun __instance)
    {

        if (VTScenario.current == null || !__instance.actor.isPlayer)
        {
            return;
        }

        // if current ammo is greater or equal, no ammo was fired.
        if (__instance.currentAmmo >= StartAmmo)
        {
            Log("No ammo was fired");
            return;
        }

        if (!__instance.isLocal)
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
            int gunId = __instance.weaponEntityID;
            Log($"Checking Gun Id {gunId} for reloads");
            var magazine = GetMagazine(__instance);
            Log($"Gun Id {gunId} has {magazine} rounds remaining!");
            if (magazine > 0)
            {
                if (!ActiveCoroutines.ContainsKey(__instance))
                {
                    var reloadCoroutineClass = new ReloadCoroutineClass(__instance);
                    ActiveCoroutines.Add(__instance, reloadCoroutineClass);
                    Log($"Creating gun reload coroutine");
                    __instance.StartCoroutine(reloadCoroutineClass.ReloadCoroutine());
                }
                else
                {
                    Log("Gun reload coroutine already in progress");
                }
            }
            else
            {
                Log($"Gun has no more reloads!");
            }
            
        }
        return;
    }

    public static int GetMagazine(Gun __instance)
    {
        if (!__instance.actor.isPlayer)
        {
            return 0;
        }
        
        if (!MunitionsManager.GunMagazineTracker.TryGetValue(__instance, out int magazine))
        {
            magazine = Main.MunitionsModifier.ReloadCount * __instance.maxAmmo;
            MunitionsManager.GunMagazineTracker.Add(__instance, magazine);
        }

        return magazine;
    }

    public static int ConsumeFromMagazine(Gun instance, int amount)
    {
        if (!MunitionsManager.GunMagazineTracker.TryGetValue(instance, out int magazine))
        {
            return 0;
        }

        int realAmount = Math.Min(amount, magazine);
        MunitionsManager.GunMagazineTracker[instance] -= realAmount;
        return realAmount;
    }
}