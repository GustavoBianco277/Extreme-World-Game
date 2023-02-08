using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlotScalerItem : MonoBehaviour
{
    public KeyCode Inv = KeyCode.I;
    public GameObject Inventario, Craft, Chest, AjusteCraft;
    public Slot slot;
    public DerrubarArvores Mira2;
    public bool InvAberto=false, ChestAberto;
    [HideInInspector]
    public int IdViewAnimal, IDTree;
    private float LastTamanho, ChildCount;
    private Movimentacao Controlador;
    private MenuPause menu;
    private bool Morte, activeCanvas;
    private void Start()
    {
        Inventario.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 5000, 0);
        Craft.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 5000, 0);
        Chest.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 5000, 0);
        menu = GetComponent<MenuPause>();
    }
    void Update()
    {
        // Desativar Canvas
        if (Input.GetKeyDown(KeyCode.F1) && !activeCanvas)
        {
            transform.GetComponent<Canvas>().enabled = false;
            FindObjectOfType<DerrubarArvores>().enabled = false;
            activeCanvas = true;
        }
        else if (Input.GetKeyDown(KeyCode.F1) && activeCanvas)
        {
            transform.GetComponent<Canvas>().enabled = true;
            FindObjectOfType<DerrubarArvores>().enabled= true;
            activeCanvas = false;
        }
        // ==========================================================

        if (Controlador == null)
        {
            if (MouseLook.player != null)
                Controlador = MouseLook.player.GetComponent<Movimentacao>();
            return;
        }

        if (Input.GetKeyDown(Inv) && !Morte && !ChestAberto && !InvAberto && !Controlador.Dirigindo && !ChatMsm.Opened && !MenuPause.MenuOpen && !GunsControl.GunsMode)
            OpenInventario();
        
        else if (Input.GetKeyDown(Inv) && InvAberto && !ChestAberto)
            CloseInventario();

        if (slot.Tamanho != LastTamanho)
            AjusteItens(); 
    }

    public void AjusteItens()
    {
        if (AjusteCraft.transform.parent.childCount >= 1)
            ChildCount = AjusteCraft.transform.parent.GetChild(1).childCount;

        for (int x = 0; x < ChildCount; x++)
        {
            Transform child = AjusteCraft.transform.parent.GetChild(1).GetChild(x);
            child.GetComponent<RectTransform>().sizeDelta = new Vector2(AjusteCraft.GetComponent<RectTransform>().rect.width, AjusteCraft.GetComponent<RectTransform>().rect.height);
        }

        item[] itens = FindObjectsOfType<item>();
        foreach (item item in itens)
        {
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(slot.Tamanho, slot.Tamanho);
        }
        LastTamanho = slot.Tamanho;
    }

    public void ControlComponent(bool Active, bool Morto=false)
    {
        GunsControl GC = FindObjectOfType<GunsControl>();
        if (!Morto)
            menu.PartJogo.Actived = Active;
        MouseLook.MouseEnable = Active;
        Mira2.enabled = Active;
        Controlador.enabled = Active;

        if (GC.GunSelected != null)
        {
            if (GC.GunSelected.GetComponent<Gun>())
                GC.GunSelected.GetComponent<Gun>().enabled = Active;
            else 
                GC.GunSelected.GetComponent<RPG_Gun>().enabled = Active;
        }


        if (Active)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            print("acabei");
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Mira2.ShowDestroy.gameObject.SetActive(false);
            FindObjectOfType<Linguagem>().Jogo.Interagir.gameObject.SetActive(false);
        }
        Morte = Morto;
    }

    public void CloseInventario()
    {
        Chest.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 5000, 0);
        Inventario.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 5000, 0);
        Craft.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 5000, 0);

        ControlComponent(true);
        InvAberto = false;
    }

    public void OpenInventario(bool ChestBool=false)
    {
        if (ChestBool)
        {
            Chest.transform.GetComponent<RectTransform>().position = Chest.transform.parent.Find("AJusteChest").position;
            Inventario.transform.GetComponent<RectTransform>().position = Inventario.transform.parent.Find("AjusteInvChest").position;
            ChestAberto = true;
        }

        else
        {
            Inventario.transform.GetComponent<RectTransform>().position = Inventario.transform.parent.Find("AjusteInventario").position;
            Craft.transform.GetComponent<RectTransform>().position = Craft.transform.parent.Find("AjusteCraft").position;
        }
        ControlComponent(false);
        InvAberto = true;
    }
}
