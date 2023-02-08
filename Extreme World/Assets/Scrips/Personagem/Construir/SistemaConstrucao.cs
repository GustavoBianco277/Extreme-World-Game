using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaConstrucao : MonoBehaviour
{
    public GameObject ObjetoFantasma;
    public LayerMask layer;
    public QueryTriggerInteraction query;
    public float MaxDistance = 7f, MinDistance = 3f, Regulation = 12f, Altura = 0.8f;
    public Vector2 PosXY;

    public bool AjustTerrain, OneBuild;
    void Start()
    {
        //ObjetoFantasma = Instantiate(ObjetoFantasma, transform.root.parent);
    }

    void Update()
    {
        if (AjustTerrain)
        {
            Ray r;
            r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit2;

            if (Physics.Raycast(r, out hit2, 5f, layer))
            {
                //set object position to hit point
                Vector3 pos = hit2.point;
                ObjetoFantasma.transform.position = pos;
                AlignGhostToSurface(ObjetoFantasma.transform, hit2.normal);
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, MaxDistance, layer, query))
        {
            PosXY[0] += Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal") / 5;
            PosXY[1] += Input.GetAxis("Mouse Y") + Input.GetAxis("Vertical") / 5;

            ObjetoFantasma.SetActive(true);
            ThisBuild Obj = ObjetoFantasma.GetComponent<ThisBuild>();
            if (Mathf.Abs(PosXY[0]) >= Regulation || Mathf.Abs(PosXY[1]) >= Regulation)
            {
                Obj.Colidiu = false;
                Obj.Outro = null;
                PosXY[0] = PosXY[1] = 0f;
                print("aqui");
            }

            if (!Obj.Colidiu && !MenuPause.MenuOpen && !ChatMsm.Opened && !Status.Morreu)
            {
                /* Vector3 pos = ObjetoFantasma.transform.position;

                 float distance = Vector3.Distance(pos, hit.point);
                 float move = Input.GetAxis("Mouse Y");

                 print(move);
                 if (distance > 2)
                     ObjetoFantasma.transform.position = hit.point;

                 if (hit.distance < MinDistance)
                 {

                     if (Obj.StopDown)
                     {
                         if (move > 0)
                             ObjetoFantasma.transform.position = new Vector3(hit.point.x, pos.y + Input.GetAxis("Mouse Y") / 9, hit.point.z);
                     }
                     else
                     {
                         if (move > 0 && distance <= 0.5f)
                             ObjetoFantasma.transform.position = new Vector3(hit.point.x, pos.y + Input.GetAxis("Mouse Y") / 9, hit.point.z);
                         else if (move < 0)
                             ObjetoFantasma.transform.position = new Vector3(hit.point.x, pos.y + Input.GetAxis("Mouse Y") / 9, hit.point.z);
                     }
                 }
                 else*/
                 ObjetoFantasma.transform.position = new Vector3(hit.point.x, hit.point.y - Altura, hit.point.z);
            }//new Vector3(hit.point.x, hit.point.y, hit.point.z);

            
            /*
            if (Input.GetKey(KeyCode.UpArrow) && ObjetoFantasma.transform.localScale.y < 5)
            {
                ObjetoFantasma.transform.localScale = new Vector3(3, ObjetoFantasma.transform.localScale.y + Time.deltaTime, 3);
            }
            else if (Input.GetKey(KeyCode.DownArrow) && ObjetoFantasma.transform.localScale.y > 3)
            {
                ObjetoFantasma.transform.localScale = new Vector3(3, ObjetoFantasma.transform.localScale.y - Time.deltaTime, 3);
            }*/
        }
        else
        {
            ObjetoFantasma.SetActive(false);
            print("noa");
        }
    }
    private void AlignGhostToSurface(Transform itemToAlign, Vector3 hitNormal)
    {
        if (itemToAlign == null) 
            return;

        if (Input.GetKeyDown("r"))
        {
            itemToAlign.eulerAngles = new Vector3(itemToAlign.rotation.x, itemToAlign.rotation.y + 20, itemToAlign.rotation.z);
        }
        itemToAlign.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal); //* Quaternion.Euler(new Vector3(0, 1, 0));
        
    }
}
