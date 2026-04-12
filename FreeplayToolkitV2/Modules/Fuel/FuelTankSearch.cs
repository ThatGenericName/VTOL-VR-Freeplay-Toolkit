using VTOLAPI;

namespace FreeplayToolkitV2.Modules.Fuel;

public static class FuelTankSearch
{

    public static FuelTank playerFuelTank;


    public static void OnSceneLoaded(VTScenes scene)
    {
        if (scene != VTScenes.CustomMapBase)
        {
            playerFuelTank = null;
        }
        
        

        var playerGO = VTAPI.GetPlayersVehicleGameObject();
        if (playerGO != null)
        {
            var fuelTanks = playerGO.GetComponents<FuelTank>();
            if (fuelTanks != null)
            {
                Log($"Player craft has {fuelTanks.Length} FuelTanks");

                foreach(var ft in fuelTanks)
                {
                    if (ft.subFuelTanks.Count > 0)
                    {
                        playerFuelTank = ft;
                        return;
                    }
                }
            }
        }
    }
}