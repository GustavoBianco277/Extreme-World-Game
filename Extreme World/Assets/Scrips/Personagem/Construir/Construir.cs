using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Construir : MonoBehaviour
{
	/*public Transform cuboPreview;
	public Transform paredePrefab;
	public Transform portaPrefab;
	private Transform prefabSelecionado;
	public Text madeirasTexto;
	public bool PodeConstruir;
	public Camera cam;
	public int madeiras;
	public Renderer rend;
	private float angulo = 90;
	private Vector3 currentpos;*/


	public bool BuildOn;
	public float Distance = 15, altura;
	public LayerMask layer;
	public QueryTriggerInteraction Query;
	private Camera cam;

	private void Start()
	{
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
	}

	void Update()
	{

		RaycastHit hit = new RaycastHit();
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit, Distance, layer, Query))
		{
			
			if (hit.collider.tag == "Terrain")
			{
				if (BuildOn)
				{
					transform.position = new Vector3(hit.point.x, hit.point.y + altura, hit.point.z);
				}
			}
		}







		/*
		madeirasTexto.text = "Madeiras: " + madeiras.ToString();
		rend.enabled = PodeConstruir;
		if (Input.GetKey("u"))
		{
			madeiras += 1;
		}
		RaycastHit hit;
		Vector3 euler = transform.rotation.eulerAngles;
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		if (Input.GetKeyDown(KeyCode.Alpha1) && madeiras > 0)
		{
			PodeConstruir = true;
			prefabSelecionado = paredePrefab;
			cuboPreview.transform.localScale = new Vector3(paredePrefab.transform.localScale.x, paredePrefab.transform.localScale.y,
				paredePrefab.transform.localScale.z);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) && madeiras > 0)
		{
			PodeConstruir = true;
			prefabSelecionado = portaPrefab;
			cuboPreview.transform.localScale = new Vector3(portaPrefab.transform.localScale.x, portaPrefab.transform.localScale.y,
				portaPrefab.transform.localScale.z);
		}
		if (Input.GetKeyDown(KeyCode.Alpha0) && madeiras > 1)
		{
			PodeConstruir = false;
		}
		if (madeiras == 0)
		{
			PodeConstruir = false;
		}
		if (PodeConstruir == true && madeiras > 0)
		{
			if (Physics.Raycast(ray, out hit, 15))
			{
				if (hit.distance > 2)
				{
					if (Input.GetKey(KeyCode.Q))
					{
						angulo += 45 *Time.deltaTime;
						// >>> angulo = angulo + 90;
					}
					if (Input.GetKey(KeyCode.E))
					{
						angulo -= 45 * Time.deltaTime;
						// >>> angulo = angulo - 90;
					}
					if (hit.collider.tag == "Terrain")
					{
						cuboPreview.gameObject.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 1, 0, 0.5f);
						transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * 15.0f);

						if (Input.GetMouseButtonDown(0))
						{
							if (prefabSelecionado == portaPrefab && madeiras > 1)
							{
								madeiras -= 2;
								Instantiate(prefabSelecionado, hit.point, Quaternion.Euler(0, euler.y, 0));
							}
							else if (prefabSelecionado == paredePrefab && madeiras > 0)
							{
								madeiras -= 1;
								Instantiate(prefabSelecionado, hit.point, Quaternion.Euler(0, euler.y, 0));
								print("instanciei");
							}
						}
					}
				}
			}
			else
			{
				cuboPreview.gameObject.GetComponent<Renderer>().sharedMaterial.color = new Color(1, 0, 0, 0.5f);
			}
		}
		else
		{
			cuboPreview.gameObject.GetComponent<Renderer>().sharedMaterial.color = new Color(1, 0, 0, 0.5f);
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, angulo,
		transform.rotation.z), Time.deltaTime * 35.0f);*/
	}
}