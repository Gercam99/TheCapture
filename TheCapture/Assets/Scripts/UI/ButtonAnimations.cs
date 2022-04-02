using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace CTF.UI
{
    public class ButtonAnimations : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Ease ease = Ease.InSine;
        [SerializeField] private Vector3 newScale = new Vector3(1.1f,1.1f,1.1f);
        [SerializeField] private float speed = .1f;

        private Vector3 startScale;

        private void Awake() => startScale = transform.localScale;
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(newScale,speed).SetEase(ease);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(startScale,speed).SetEase(ease);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            transform.localScale = startScale;
        }
    }
}
