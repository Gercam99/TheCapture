using System;
using UnityEngine;

namespace CTF.PlayerController
{
    public class PlayerMine : MonoBehaviour
    {
     
        public GameObject[] destroyObjects;
        public Component[] destroyComponents;

        private PlayerController _player;


        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private void Start()
        {
            if (_player.pv.IsMine) return;
            
            foreach (var objects in destroyObjects)
            {
                Destroy(objects);
            }
            foreach (var component in destroyComponents)
            {
                Destroy(component);
            }
        }
    }
}