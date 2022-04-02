using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace CTF.Managers.Animations
{
    public class AnimationMainMenuManager : Singleton<AnimationMainMenuManager>
    {
        [SerializeField] private MMFeedbacks LoginFeedback;
        [SerializeField] private MMFeedbacks CreateRoomFeedback;
        [SerializeField] private MMFeedbacks InsideRoomFeedback;
        [SerializeField] private MMFeedbacks ShowRoomsFeedback;
        public void SelectionMenuAnimation()
        {
            LoginFeedback.PlayFeedbacks();
        }

        public void CreateRoomAnimation()
        {
            CreateRoomFeedback.PlayFeedbacks();
        }

        public void InsideRoomAnimation()
        {
            InsideRoomFeedback.PlayFeedbacks();
        }

        public void ShowRoomsAnimation()
        {
            ShowRoomsFeedback.PlayFeedbacks();
        }
    }
}
