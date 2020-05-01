using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using Assets.Scripts;
using System.Reflection;

public class DynamicDataSetLoader : MonoBehaviour
{
    // specify these in Unity Inspector
    [SerializeField]
    private GameObject augmentationObject = null;
    [SerializeField]
    private string dataSetName = "";  //  Assets/StreamingAssets/Vuforia/DataSetName without ending

    // Use this for initialization
    void Start()
    {
        //AddTags();
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
    }

    void LoadDataSet()
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(dataSetName))
        {
            objectTracker.Stop();  // stop tracker so that we can add new dataset

            if (!objectTracker.ActivateDataSet(dataSet))
            {
                // Note: ImageTracker cannot have more than 100 total targets activated
                Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
            }

            if (!objectTracker.Start())
            {
                Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
            }

            int counter = 0;
            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in tbs)
            {
                if (tb.name == "New Game Object")
                {
                    // change generic name to include trackable name
                    tb.gameObject.name = ++counter + ":DynamicImageTarget-" + tb.TrackableName;

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<TurnOffBehaviour>();
                    tb.gameObject.AddComponent<PrinterTrackableEventHandler>();

                    if (augmentationObject != null)
                    {
                        // instantiate augmentation object and parent to trackable
                        GameObject augmentation = (GameObject)GameObject.Instantiate(augmentationObject);
                        augmentation.tag = Tags.PrinterInfo;
                        augmentation.transform.parent = tb.gameObject.transform;
                    }
                    else
                    {
                        Debug.Log("<color=yellow>Warning: No augmentation object specified for: " + tb.TrackableName + "</color>");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("<color=yellow>Failed to load dataset: '" + dataSetName + "'</color>");
        }
    }
}