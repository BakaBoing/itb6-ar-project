using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using Assets.Scripts;

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
                Debug.Log($"{tb.name}: {tb.TrackableName}");
                if (tb.name == "New Game Object")
                {
                    // change generic name to include trackable name
                    tb.gameObject.name = ++counter + ":DynamicImageTarget-" + tb.TrackableName;

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    tb.gameObject.AddComponent<TurnOffBehaviour>();
                    tb.gameObject.AddComponent<PrinterTrackableEventHandler>();

                    if (augmentationObject != null)
                    {
                        // instantiate augmentation object and parent to trackable
                        //GameObject augmentation = (GameObject)GameObject.Instantiate(augmentationObject);
                        //augmentation.transform.parent = tb.gameObject.transform;
                        //augmentation.transform.localPosition = new Vector3(0f, 0f, 0f);
                        //augmentation.transform.localRotation = Quaternion.identity;
                        //augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                        //augmentation.gameObject.SetActive(true);
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