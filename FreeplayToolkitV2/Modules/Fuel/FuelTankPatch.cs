using System;
using HarmonyLib;

namespace FreeplayToolkitV2.Modules.Fuel;

[HarmonyPatch(typeof(FuelTank), nameof(FuelTank.RequestFuel), new Type[] { typeof(double) })]
public static class FuelTankPatch
{   
    [HarmonyPrefix]
    public static bool RequestFuelPrefix(FuelTank __instance, ref double deltaFuel)
    {
        
        if (MultiplayerLock.IsMultiplayer)
        {
            return true;
        }
        
        if (FuelTankSearch.playerFuelTank == null)
        {
            return true;
        }
        if (__instance == FuelTankSearch.playerFuelTank)
        {
            deltaFuel = deltaFuel * Main.FuelModifier.FuelDrainModifier;
        }

        return true;
    }
}