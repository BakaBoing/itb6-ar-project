using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BoundingBoxUI : MonoBehaviour
{
    public enum BoundingBoxType
    {
        Inner,
        Outer,
    }

    public BoundingBoxType myBoundingBoxType;
    public LineRenderer bound_line_renderer;

    [SerializeField]
    private TextMesh top_left, width, height;

    private ManoUtils mano_utils;
    private float textDepthModifier = 4;
    private float textAdjustment = 0.01f;
    private float backgroundDepth = 8;

    private void Start()
    {
        mano_utils = ManoUtils.Instance;
        bound_line_renderer.positionCount = 4;
    }

    private float normalizedTopLeftX;
    private float normalizedTopLeftY;
    private float normalizedBBWidth;
    private float normalizedHeight;

    private Vector3 normalizedTopLeft;
    private Vector3 normalizedTopRight;
    private Vector3 normalizedBotRight;
    private Vector3 normalizedBotLeft;
    private Vector3 normalizedTextHeightPosition;
    private Vector3 normalizedTextWidth;

    public void UpdateInfo(BoundingBox bounding_box)
    {
        if (!mano_utils)
            mano_utils = ManoUtils.Instance;
        if (!bound_line_renderer)
        {
            Debug.Log("bound_line_renderer: null");
            return;
        }

        switch (myBoundingBoxType)
        {
            case BoundingBoxType.Outer:
                normalizedTopLeftX = bounding_box.top_left.x;
                normalizedTopLeftY = bounding_box.top_left.y;
                normalizedBBWidth = bounding_box.width;
                normalizedHeight = bounding_box.height;

                normalizedTopLeft = new Vector3(normalizedTopLeftX, normalizedTopLeftY);
                normalizedTopRight = new Vector3(normalizedTopLeftX + normalizedBBWidth, normalizedTopLeftY);
                normalizedBotRight = new Vector3(normalizedTopLeftX + normalizedBBWidth, normalizedTopLeftY - normalizedHeight);
                normalizedBotLeft = new Vector3(normalizedTopLeftX, normalizedTopLeftY - normalizedHeight);

                bound_line_renderer.SetPosition(0, ManoUtils.Instance.CalculateWorldPosition(normalizedTopLeft, backgroundDepth));
                bound_line_renderer.SetPosition(1, ManoUtils.Instance.CalculateWorldPosition(normalizedTopRight, backgroundDepth));
                bound_line_renderer.SetPosition(2, ManoUtils.Instance.CalculateWorldPosition(normalizedBotRight, backgroundDepth));
                bound_line_renderer.SetPosition(3, ManoUtils.Instance.CalculateWorldPosition(normalizedBotLeft, backgroundDepth));

                normalizedTopLeft.y += textAdjustment * 3;
                top_left.gameObject.transform.position = ManoUtils.Instance.CalculateWorldPosition(normalizedTopLeft, backgroundDepth / textDepthModifier);
                top_left.text = "Top Left: " + "X: " + normalizedTopLeftX.ToString("F2") + " Y: " + normalizedTopLeftY.ToString("F2");

                normalizedTextHeightPosition = new Vector3(normalizedTopLeftX + normalizedBBWidth + textAdjustment, (normalizedTopLeftY - normalizedHeight / 2f));
                height.transform.position = ManoUtils.Instance.CalculateWorldPosition(normalizedTextHeightPosition, backgroundDepth / textDepthModifier);
                height.GetComponent<TextMesh>().text = "Height: " + normalizedHeight.ToString("F2");

                normalizedTextWidth = new Vector3(normalizedTopLeftX + normalizedBBWidth / 2f, (normalizedTopLeftY - normalizedHeight) - textAdjustment);
                width.transform.position = ManoUtils.Instance.CalculateWorldPosition(normalizedTextWidth, backgroundDepth / textDepthModifier);
                width.GetComponent<TextMesh>().text = "Width: " + normalizedBBWidth.ToString("F2");
                break;
            default:
                break;
        }
    }
}
