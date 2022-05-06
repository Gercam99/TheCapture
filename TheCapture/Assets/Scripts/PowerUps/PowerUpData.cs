using UnityEngine;

namespace PowerUps
{
    [CreateAssetMenu(fileName = "PowerUpData", menuName = "PowerUps", order = 0)]
    public class PowerUpData : ScriptableObject
    {
        public string DisplayName;
        public Sprite Icon;
        public int Damage;
        public bool Freeze;
        public float AreaOfDamage;
    }
}