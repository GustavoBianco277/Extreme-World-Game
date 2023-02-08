using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
	[Header("Mouse Configuracoes")]
	public static Transform player;
	public static Transform VehicleUsing;
	public static bool MouseEnable;
    public Transform cam;
	private Transform Head;
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2}
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 5F;
	public float sensitivityY = 5F;
	public Vector2 minMaxX = new Vector2(-360F, 360f);
	public Vector2 minMaxY = new Vector2(-60F, 60f);
	float rotationY = 0F;
	float rotationX = 0f;
	public bool InverterMouse;
	public static bool Veiculo;

     void Awake()
	{
		MouseEnable = true;
		cam = transform.GetChild(0);
	}
	void Update()
	{
		// configuraçoes do mouse
		if (player == null)
			return;

		transform.position = player.transform.position;
		if (Head == null)
			Head = player.Find("TargetHead");

		if (MouseEnable)
		{
			if (Cursor.lockState != CursorLockMode.Locked)
				Cursor.lockState = CursorLockMode.Locked;

			if (axes == RotationAxes.MouseXAndY)
			{
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				if (Veiculo)
				{
					rotationY = Mathf.Clamp(rotationY, minMaxY[0], minMaxY[1]);
					rotationX = Mathf.Clamp(rotationX, minMaxX[0], minMaxX[1]);
					//transform.rotation = player.transform.rotation;
					cam.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationX, 0);
					Head.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationX, 0);
				}
				else
				{
                    rotationY = Mathf.Clamp(rotationY, minMaxY[0], minMaxY[1]);
                    transform.eulerAngles = new Vector3(0, player.eulerAngles.y, 0);
					player.transform.localEulerAngles = new Vector3(-0, rotationX, 0);
				}

				if (InverterMouse)
				{
					cam.transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);
					Head.transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);
				}

				else
				{
					cam.transform.localEulerAngles = new Vector3(-rotationY, cam.transform.localEulerAngles.y, 0);
					Head.transform.localEulerAngles = new Vector3(-rotationY, cam.transform.localEulerAngles.y, 0);
				}

			}

			else if (axes == RotationAxes.MouseX)
			{
				player.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			}

			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp(rotationY, minMaxY[0], minMaxY[1]);
				transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
			}
		}
	}
}