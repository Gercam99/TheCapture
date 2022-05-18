using System;
using System.Web.UI.Design;
using UnityEngine;
using UnityEngine.UI;

namespace CTF.Managers
{
    public class HUDManager : MonoBehaviour
    {
        public const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 1f;
        public PlayerController.PlayerController PlayerController;


        public Image HealthFill;
        public Image EffectFill;
        public float ShrinkTimer;

        private bool _completeEffectFill;


        private void Start()
        {
            SetHealth(PlayerController.HealthSystem.GetHealthNormalized());
            
            PlayerController.HealthSystem.OnDamage += HealthSystem_OnDamage;
            PlayerController.HealthSystem.OnHeal += HealthSystem_OnHeal;
        }

        private void Update()
        {
            if (_completeEffectFill != false) return;
            ShrinkTimer -= Time.deltaTime;
            if (!(ShrinkTimer < 0)) return;
            if (HealthFill.fillAmount < EffectFill.fillAmount)
            {
                float shrinkSpeed = 1f;
                EffectFill.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
            else
            {
                _completeEffectFill = true;
            }

        }

        private void HealthSystem_OnHeal()
        {
            SetHealth(PlayerController.HealthSystem.GetHealthNormalized());
            EffectFill.fillAmount = HealthFill.fillAmount;
        }

        private void HealthSystem_OnDamage()
        {
            _completeEffectFill = false;
            ShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
            SetHealth(PlayerController.HealthSystem.GetHealthNormalized());
        }


        private void SetHealth(float healthNormalized)
        {
            HealthFill.fillAmount = healthNormalized;
        }
    }
}