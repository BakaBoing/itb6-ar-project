/*==============================================================================
Copyright (c) 2019 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Vuforia;

namespace Assets.Scripts
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    ///
    /// Changes made to this file could be overwritten when upgrading the Vuforia version.
    /// When implementing custom event handler behavior, consider inheriting from this class instead.
    /// </summary>
    public class PrinterTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
    {
        private static bool _isInfoScreenShowing = false;
        private GameObject _infoScreen = null;
        private Rect _position = new Rect(50, 50, 200, 100);

        //void OnGUI()
        //{
            //if (_isInfoScreenShowing)
            //{
            //    GUIStyle style = new GUIStyle();
            //    style.fontSize = 300;
            //    GUI.Label(_position, _trackableBehaviour.TrackableName, style);

            //    //if (GUI.Button(_position, "Hello"))
            //    //{
            //    //    // do something on button click	
            //    //}
            //}
        //}

        private void ShowInfoScreen()
        {
            if (_isInfoScreenShowing == false)
            {
                foreach (Component component in _trackableBehaviour.gameObject.GetComponentsInChildren<Component>())
                {
                    if (_infoScreen == null)
                    {
                        _infoScreen = component.gameObject.FindComponentInChildWithTag<Component>(Tags.PrinterInfo)?.gameObject;
                    }
                    else
                    {
                        break;
                    }
                }

                if (_infoScreen != null)
                {
                    PrinterInfo printerInfo = DataStoreQuery.GetPrinterInfo(_trackableBehaviour.TrackableName);
                    TMP_Text[] infoChilds = _infoScreen.GetComponentsInChildren<TMP_Text>();

                    foreach (TMP_Text info in infoChilds)
                    {
                        switch (info.name)
                        {
                            case Tags.PrinterName:
                                info.SetText(printerInfo.Name);
                                break;
                            case Tags.PaperFormats:
                                info.SetText(String.Join(", ", printerInfo.PaperFormats));
                                break;
                                case Tags.InstructionsText:
                                info.SetText(printerInfo.Instructions);
                                break;
                        }
                    }

                    _infoScreen.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _infoScreen.transform.localRotation = Quaternion.identity;
                    _infoScreen.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    _infoScreen.gameObject.SetActive(true);
                    _isInfoScreenShowing = true;
                }
            }
        }

        private void HideInfoScreen()
        {
            if (_infoScreen != null)
            {
                _infoScreen.SetActive(false);
                _isInfoScreenShowing = false;
            }
        }

        #region PROTECTED_MEMBER_VARIABLES

        protected TrackableBehaviour _trackableBehaviour;
        protected TrackableBehaviour.Status _previousStatus;
        protected TrackableBehaviour.Status _newStatus;

        #endregion // PROTECTED_MEMBER_VARIABLES

        #region UNITY_MONOBEHAVIOUR_METHODS

        protected virtual void Start()
        {
            _trackableBehaviour = GetComponent<TrackableBehaviour>();
            if (_trackableBehaviour)
                _trackableBehaviour.RegisterTrackableEventHandler(this);
        }

        protected virtual void OnDestroy()
        {
            if (_trackableBehaviour)
                _trackableBehaviour.UnregisterTrackableEventHandler(this);
        }

        #endregion // UNITY_MONOBEHAVIOUR_METHODS

        #region PUBLIC_METHODS

        /// <summary>
        ///     Implementation of the ITrackableEventHandler function called when the
        ///     tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
            TrackableBehaviour.Status previousStatus,
            TrackableBehaviour.Status newStatus)
        {
            _previousStatus = previousStatus;
            _newStatus = newStatus;

            Debug.Log("Trackable " + _trackableBehaviour.TrackableName +
                      " " + _trackableBehaviour.CurrentStatus +
                      " -- " + _trackableBehaviour.CurrentStatusInfo);

            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                     newStatus == TrackableBehaviour.Status.NO_POSE)
            {
                OnTrackingLost();
            }
            else
            {
                // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
                // Vuforia is starting, but tracking has not been lost or found yet
                // Call OnTrackingLost() to hide the augmentations
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS

        #region PROTECTED_METHODS

        protected virtual void OnTrackingFound()
        {
            if (_trackableBehaviour)
            {
                ShowInfoScreen();

                var rendererComponents = _trackableBehaviour.GetComponentsInChildren<Renderer>(true);
                var colliderComponents = _trackableBehaviour.GetComponentsInChildren<Collider>(true);
                var canvasComponents = _trackableBehaviour.GetComponentsInChildren<Canvas>(true);

                // Enable rendering:
                foreach (var component in rendererComponents)
                    component.enabled = true;

                // Enable colliders:
                foreach (var component in colliderComponents)
                    component.enabled = true;

                // Enable canvas':
                foreach (var component in canvasComponents)
                    component.enabled = true;
            }
        }


        protected virtual void OnTrackingLost()
        {
            if (_trackableBehaviour)
            {
                var rendererComponents = _trackableBehaviour.GetComponentsInChildren<Renderer>(true);
                var colliderComponents = _trackableBehaviour.GetComponentsInChildren<Collider>(true);
                var canvasComponents = _trackableBehaviour.GetComponentsInChildren<Canvas>(true);

                // Disable rendering:
                foreach (var component in rendererComponents)
                    component.enabled = false;

                // Disable colliders:
                foreach (var component in colliderComponents)
                    component.enabled = false;

                // Disable canvas':
                foreach (var component in canvasComponents)
                    component.enabled = false;
            }
        }

        #endregion // PROTECTED_METHODS
    }
}
