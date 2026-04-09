using ModLoader.Framework.Settings;

namespace FreeplayToolkitV2.Settings;

public class MunitionsModifier : BaseSettings
{
    public float ReloadTime = 15f;
    public int ReloadCount = 0;
    public bool HasReloads = false;
    public bool InfiniteAmmo = false;

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