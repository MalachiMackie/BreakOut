using GamePlay;
using Shared;
using UnityEngine;

namespace Managers
{
    public class BrickManager : Singleton<BrickManager>
    {
        public Color oneHealthColor;
        public Color twoHealthColor;
        public Color threeHealthColor;
        public Color fourOrMoreHealthColor;

        public bool randomizeBrickHealth;

        private void Start()
        {
            if (!randomizeBrickHealth)
            {
                return;
            }

            var allBricks = FindObjectsOfType<Brick>();
            foreach (var brick in allBricks)
            {
                brick.SetHealth(Random.Range(1, 5));
            }
        }
    }
}
