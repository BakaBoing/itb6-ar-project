using UnityEngine;

public struct ManoMotionFrame
{
	public int width;
	public int height;
	public Texture2D texture;
	public Color32[] pixels;
	public DeviceOrientation orientation;
}