using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace Assets.Scripts
{
    public class PrinterTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
    {
        private TrackableBehaviour _trackableBehaviour;

        private bool _isInfoScreenShowing = false;
        private Rect _position = new Rect(50, 50, 200, 100);

        void Start()
        {
            _trackableBehaviour = GetComponent<TrackableBehaviour>();
            _trackableBehaviour?.RegisterTrackableEventHandler(this);
        }

        public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            _isInfoScreenShowing =
                newStatus == TrackableBehaviour.Status.DETECTED
                || newStatus == TrackableBehaviour.Status.TRACKED;
        }

        void OnGUI()
        {
            if (_isInfoScreenShowing)
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 300;
                GUI.Label(_position, _trackableBehaviour.TrackableName, style);

                //if (GUI.Button(_position, "Hello"))
                //{
                //    // do something on button click	
                //}
            }
        }
    }
}
