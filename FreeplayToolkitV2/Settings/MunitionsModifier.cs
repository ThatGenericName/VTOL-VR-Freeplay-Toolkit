using ModLoader.Framework.Settings;

namespace FreeplayToolkitV2.Settings;

public class MunitionsModifier : BaseSettings
{
    public float ReloadTime = 5f;
    public int ReloadCount = 5;
    public bool HasReloads = false;
    public bool InfiniteAmmo = false;
    public float ReloadTimePrev = 0.0f;
    public bool InfAmmoPrev = false;
    public bool PatchActive = false;

    public void ComputeProperties()
    {
        HasReloads = ReloadCount > 0;
    }
    
    public void PrintoutCurrentSettings()
    {
        Log($"Reload Time: {ReloadTime}s");
        Log($"Infinite Ammo: {InfiniteAmmo}");
        Log($"Reload Count: {ReloadCount}");
    }
}