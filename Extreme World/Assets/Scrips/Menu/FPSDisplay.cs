using UnityEngine;
using System.Collections;
using Photon.Pun;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;

	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = Color.white;
		float msec = deltaTime * 1000.0f;
		float ping = PhotonNetwork.GetPing();
		float fps = 1.0f / deltaTime;
		string text;
		if (PhotonNetwork.IsConnected)
			text = string.Format("Ping {0:0.}     FPS ({1:0.})", ping, fps);
		else
			text = string.Format("{0:0.0} ms    FPS ({1:0.})", msec, fps);
		GUI.Label(rect, text, style);
	}
}