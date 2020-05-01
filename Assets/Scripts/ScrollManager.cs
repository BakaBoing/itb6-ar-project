using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScrollManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _canvas;
        [SerializeField]
        private float _scrollAmount = 0.01f;

        private static ScrollManager _scrollManager;
        public static ScrollManager Instance
        {
            get
            {
                if (!_scrollManager)
                {
                    _scrollManager = FindObjectOfType(typeof(ScrollManager)) as ScrollManager;
                }
                return _scrollManager;
            }
        }

        private List<Scrollbar> _scrollbars = new List<Scrollbar>();
        private float _currentScroll = 1f;

        private void Start()
        {
            SetScrolling(_currentScroll);
        }

        public static void AddScrollbar(Scrollbar scrollbar)
        {
            if (Instance)
            {
                Instance._scrollbars.Add(scrollbar);
                scrollbar.value = Instance._currentScroll;
            }
        }

        public static void RemoveScrollbar(Scrollbar scrollbar)
        {
            Instance?._scrollbars.Remove(scrollbar);
        }

        private void FixedUpdate()
        {
            if (_scrollbars?.Count > 0)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    ScrollUp();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SetScrolling(0.5f);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    ScrollDown();
                }

                if (ManomotionManager.Instance?.Hand_infos?[0].hand_info.gesture_info.mano_gesture_trigger == ManoGestureTrigger.CLICK)
                {
                    ScrollDown();
                }
                if (ManomotionManager.Instance?.Hand_infos?[0].hand_info.gesture_info.mano_gesture_trigger == ManoGestureTrigger.PICK)
                {
                    ScrollUp();
                }
            }
        }

        public void SetScrolling(float f)
        {
            f = Math.Max(0, Math.Min(1, f));
            _currentScroll = f;
            foreach (var scrollbar in _scrollbars)
            {
                scrollbar.value = f;
            }
        }

        public void ScrollUp()
        {
            SetScrolling(_currentScroll + _scrollAmount);
        }

        public void ScrollDown()
        {
            SetScrolling(_currentScroll - _scrollAmount);
        }

        private void Update()
        {
            _canvas?.SetActive(_scrollbars?.Count > 0);
        }
    }
}