using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScrollButtons : MonoBehaviour
    {
        private const float scroll_amount = 0.005f;
        private bool scrollActive;
        private bool isUP;
        private void FixedUpdate()
        {
            if (!scrollActive) return;
            
            if (isUP)
            {
                SetScroll.Instance.scrollUp(scroll_amount);
            }
            else
            {
                SetScroll.Instance.scrollDown(scroll_amount);
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