using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

namespace CTF.Managers
{
    public enum TypeGame
    {
        onevsone = 0,
        twovstwo = 1,
        tenvsten = 2,

}
    public class TypeGameManager : MonoBehaviour
    {
        public TypeGame typeGame;

        [SerializeField] private Toggle oneToggle;
        [SerializeField] private Toggle twoToggle;
        [SerializeField] private Toggle tenToggle;

        public GameObject panelTypeGame;
        private void Start()
        {
            if(panelTypeGame== null)            OneVsOneSelected();

        }
        
        public void OneVsOneSelected()
        {
            typeGame = TypeGame.onevsone;
            MainMenuManager.Instance.OnSelectTypeGame((int)typeGame);
            
            if(panelTypeGame!= null) panelTypeGame.SetActive(false);
        }

        public void TwoVsTwoSelected()
        {
            typeGame = TypeGame.twovstwo;
            MainMenuManager.Instance.OnSelectTypeGame((int)typeGame);
            
            if(panelTypeGame!= null) panelTypeGame.SetActive(false);
        }

        public void TenVsTenSelected()
        {
            typeGame = TypeGame.tenvsten;
            MainMenuManager.Instance.OnSelectTypeGame((int)typeGame);
            
            if(panelTypeGame!= null) panelTypeGame.SetActive(false);
        }
        


    }
}

