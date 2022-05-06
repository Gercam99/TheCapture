using PowerUps;
using UnityEngine;

namespace CTF.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        public PowerUpData PowerUpData;
        
        
        public bool HasPowerUp() => PowerUpData;
    }
}