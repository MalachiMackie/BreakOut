using System;
using System.Linq;
using Shared;
using UnityEngine;


public class LevelDetails : MonoBehaviour
{
    [Serializable]
    public class PowerUpAmount
    {
        public PowerUpType type;
        public BrickPowerUpApplies applies = BrickPowerUpApplies.BrickDestroy;
        public int count;
    }

    public PowerUpAmount[] powerUps;

    private void Awake()
    {
        if (!powerUps.Any())
        {
            return;
        }
        var maxPowerUps = powerUps.GroupBy(x => x.type).Select(x => x.Count()).Max();
        Helpers.AssertIsTrueOrQuit(maxPowerUps <= 1, "level details can only specify a powerup once");
        Helpers.AssertIsTrueOrQuit(powerUps.All(x => x.type != PowerUpType.None),
            $"{nameof(PowerUpType)}.{nameof(PowerUpType.None)} is not valid");
        Helpers.AssertIsTrueOrQuit(powerUps.All(x => x.applies != BrickPowerUpApplies.None),
            $"{nameof(BrickPowerUpApplies)}.{nameof(BrickPowerUpApplies.None)} is not valid");
    }
}
