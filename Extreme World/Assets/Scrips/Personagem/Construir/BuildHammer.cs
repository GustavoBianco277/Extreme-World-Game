using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHammer : MonoBehaviour
{
    [SerializeField] private GameObject Interface;
    public bool HammerOpen;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !Interface.active && !MenuPause.MenuOpen && !Status.Morreu && !ChatMsm.Opened)
        {
            ActiveHammer(true);
        }
        else if (Input.GetMouseButtonDown(1) && Interface.active)
        {
            ActiveHammer(false);
        }
    }
    public void ActiveHammer (bool Active)
    {
        Interface.SetActive(Active);
        Cursor.visible = Active;
        HammerOpen = true;
        FindObjectOfType<DerrubarArvores>().enabled = !Active;
        MouseLook.MouseEnable = !Active;

        if (Active)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}
