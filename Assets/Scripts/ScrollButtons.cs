using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScrollButtons : MonoBehaviour
    {
        private bool scrollActive;
        private bool isUP;
        private void FixedUpdate()
        {
            if (!scrollActive) return;

            if (isUP)
            {
                ScrollManager.Instance.ScrollUp();
            }
            else
            {
                ScrollManager.Instance.ScrollDown();
            }
        }

        // Used in UI
        // ReSharper disable once UnusedMember.Global
        public void onPressUP()
        {
            scrollActive = true;
            isUP = true;
        }

        // Used in UI
        // ReSharper disable once UnusedMember.Global
        public void onRelease()
        {
            scrollActive = false;
        }

        // Used in UI
        // ReSharper disable once UnusedMember.Global
        public void onPressDOWN()
        {
            scrollActive = true;
            isUP = false;
        }
    }
}