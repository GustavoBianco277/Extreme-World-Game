using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ReceitaCraft
{
    public GameObject Item;
    public int Quantidade;
}
[System.Serializable]
public class Construction
{
    public bool Construct, Hammer, AjustTerrain, OneBuild;
    public float Altura;
    public GameObject ObjFantasma;
    public LayerMask layer;
}
public class ItemCraft : MonoBehaviour
{
    private Slot[] Slots;
    public List<ReceitaCraft> Receita = new List<ReceitaCraft>();
    [HideInInspector]
    public static AudioSource SomItemFake;
    
    public GameObject Craft;
    public GameObject Previw;
    public int cont;
    public bool Metal;
    public Construction Construction;

    //privates
    private RectTransform Rect;
    private bool Pronto, Suficiente;
    private Inv inventario;
    private SlotScalerItem menu;
    private List<Slot> slotCraft = new List<Slot>();
    private List<Slot> slotInventario = new List<Slot>();
    void Start()
    {
        SomItemFake = GetComponent<AudioSource>();
        Slots = FindObjectsOfType<Slot>();
        inventario = FindObjectOfType<Inv>();
        menu = FindObjectOfType<SlotScalerItem>();

        foreach (Slot slot in Slots)
        {
            if (slot.Numero < 0)
                slotCraft.Add(slot);

            if (slot.Numero > 0 && slot.Numero < 55)
                slotInventario.Add(slot);
        }
        Rect = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (!menu.InvAberto && Pronto)
        {
            LimparCraft(InvOff:true);
            Pronto = false;
        }
        if (menu.InvAberto)
            Pronto = true;
    }
    public void ItemSelected()
    {
        List<int> ids = new List<int>();
        List<int> notIds = new List<int>();
        itemObject item = Previw.transform.GetComponent<itemObject>();

        if (item.ItemCraft != transform.gameObject)
        {
            Previw.transform.GetChild(0).GetComponent<Image>().enabled = true;
            Previw.transform.GetChild(0).GetComponent<Image>().sprite = transform.GetChild(1).GetComponent<Image>().sprite;

            if (cont > 1)
                Previw.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cont.ToString();

            else
                Previw.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Empty;

            if (!Construction.Construct)
            {
                item.ItemModel = Craft;
                item.id = Craft.GetComponent<item>().Id;
            }

            item.ItemCraft = this;
            item.cont = cont;
            if (Construction.Hammer)
            {
                FindObjectOfType<BuildHammer>().ActiveHammer(false);
                item.Craft();
            }
        }
        // Verifica se tem item suficiente no craft
        for (int c = 0; c < Receita.Count; c++) 
        {
            foreach (Slot slot in slotCraft)
            {
                if (slot.Id == Receita[c].Item.GetComponent<item>().Id)
                {
                    if (slot.count >= Receita[c].Quantidade)
                        ids.Add(slot.Id);
                    else
                        notIds.Add(slot.Id);
                }
            }
        }
        LimparCraft(ids, notIds);
        if (Suficiente)
        {
            VerificaItens();
        }
    }
    public bool CraftItem()
    {
        bool PodeCraft = true;

        // Ve se tem espaço no inventario
        if (!Construction.Construct)
        {
            foreach (Slot slot in slotInventario)
            {
                if (slot.Id == Craft.GetComponent<item>().Id && slot.count + cont <= 64)
                {
                    PodeCraft = true;
                    break;
                }
                else if (slot.Id == -1)
                {
                    PodeCraft = true;
                    break;
                }
                else
                    PodeCraft = false;
            }
        }
        // Agrupa Itens
        inventario.Agrupar(slotCraft, -6, -1);

        // ve se tem todos os itens nescessarios
        for (int c = 0; c < Receita.Count; c++)
        {
            if (PodeCraft)
            {
                foreach (Slot slot in slotCraft)
                {
                    if (slot.Id == Receita[c].Item.GetComponent<item>().Id && slot.count >= 0)
                    {
                        if (slot.count >= Receita[c].Quantidade)
                        {
                            PodeCraft = true;
                            break;
                        }
                        else
                        {
                            PodeCraft = false;
                            if (!slot.ItemFake)
                                ItemFake(slot, Receita[c].Item, slot.Id, Receita[c].Quantidade);
                            break;
                        }
                    }
                    else
                        PodeCraft = false;
                }
            }
            else 
            {
                PodeCraft = false;
                break; 
            }
        }

        if (PodeCraft)
        {
            for (int c = 0; c < Receita.Count; c++)
            {
                foreach (Slot slot in slotCraft)
                {
                    if (slot.Id == Receita[c].Item.GetComponent<item>().Id)
                    {
                        inventario.remItem(slot.Id, Receita[c].Quantidade, true);
                        break;
                    }
                }
            }
        }
        // Verifica se Precisa Add ItemFake
        if (!Construction.Construct)
        {
            for (int c = 0; c < Receita.Count; c++)
            {
                int id = Receita[c].Item.GetComponent<item>().Id;
                int disponivel = slotCraft.Count;
                bool ItensSuficiente = false;
                foreach (Slot slot in slotCraft)
                {
                    if (slot.Numero < disponivel && slot.Id == -1)
                        disponivel = slot.Numero;

                    if (slot.Id == Receita[c].Item.GetComponent<item>().Id && slot.count >= 0 && PodeCraft)
                    {
                        if (slot.count < Receita[c].Quantidade)
                        {
                            if (!slot.ItemFake)
                            {
                                ItemFake(slot, Receita[c].Item, slot.Id, Receita[c].Quantidade);
                            }
                        }
                    }
                    if (slot.Id == Receita[c].Item.GetComponent<item>().Id && slot.count >= Receita[c].Quantidade || slot.Id == Receita[c].Item.GetComponent<item>().Id && slot.countFake != 0)
                        ItensSuficiente = true;
                }

                if (!ItensSuficiente)
                {
                    foreach (Slot slotcraft in slotCraft)
                    {
                        if (slotcraft.Numero == disponivel)
                        {
                            inventario.remItem(id, 0, true, false, 0, Receita[c].Quantidade);
                            if (inventario.ItemNaoExiste)
                            {
                                ItemFake(slotcraft, Receita[c].Item, id, Receita[c].Quantidade, true);
                                break;
                            }
                        }
                    }
                }
            }
        }
        return PodeCraft;
    }
    public void ItemFake (Slot slot, GameObject item, int id, int Quantidade,bool Ignore = false)
    {
        GameObject Obj = Instantiate(item, slot.transform.position, Quaternion.identity, slot.transform);
        Obj.GetComponent<Image>().color = new Color(1, 0.4f, 0.4f, 1);
        Obj.GetComponent<item>().ItemFake = true;
        Obj.transform.parent.GetComponent<Slot>().ItemFake = true;
        slot.Id = id;

        if (Ignore)
            slot.countFake = Quantidade;
        else
            slot.countFake = Quantidade - slot.count;

        SomItemFake.Play();
    }
    public void LimparCraft(List<int> ids = null, List<int> notIds = null, bool InvOff = false)
    {
        if (InvOff)
        {
            Previw.transform.GetChild(0).GetComponent<Image>().enabled = false;
            Previw.GetComponent<itemObject>().ItemCraft = null;
            Previw.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Empty;
        }
        Suficiente = true;
        foreach (Slot slot in slotCraft)
        {
            if (ids != null)
            {
                if (ids.Count > 0)
                {
                    bool Next = false;
                    foreach (int id in ids)
                    {
                        if (slot.Id == id)
                        {
                            Next = true;
                            break;
                        }
                    }
                    if (Next)
                        continue;
                }
            }

            // verifica se a itens no inventario
            if (notIds != null)
            {
                if (notIds.Count > 0 && slot.Id != -1)
                {
                    foreach (Slot slt in slotInventario)
                    {
                        foreach (int id in notIds)
                        {
                            if (slt.Id == id)
                            {
                                int quantidade = 0;
                                for (int c = 0; c < Receita.Count; c++)
                                {
                                    if (Receita[c].Item.GetComponent<item>().Id == id)
                                        quantidade = Receita[c].Quantidade;
                                }
                                if (slot.count < quantidade)
                                {
                                    Suficiente = false;
                                    break;
                                }
                            }
                            //else
                            //{
                            //    Suficiente = true;
                            //    break;
                            //}
                        }
                    }
                }
            }
            if (Suficiente)
            {
                int disponivel = Slots.Length;
                if (slot.count > 0)
                {
                    foreach (Slot sloti in Slots)
                    {
                        if (sloti.Numero < disponivel && sloti.Numero > 0 && sloti.Numero < 55 && sloti.Id == -1)
                            disponivel = sloti.Numero;
                    }
                    foreach (Slot slotInv in Slots)
                    {
                        // remove os itens do craft
                        if (slotInv.Numero == disponivel)
                        {
                            inventario.addItem(slot.Id, slot.count);
                            inventario.remItem(slot.Id, slot.count, true, Notfake: true);
                            break;
                        }
                    }
                }
            }
            inventario.remItem(-2, 64, true, Notfake: true);
        }
    }
    public void VerificaItens()
    {
        for (int c = 0; c < Receita.Count; c++)
        {
            int SlotDisponivel = 0;
            int id = Receita[c].Item.GetComponent<item>().Id;
            bool ItensSuficiente = false;
            foreach (Slot slot in slotCraft)
            {
                if (slot.Numero < SlotDisponivel && slot.Id == -1)
                    SlotDisponivel = slot.Numero;

                if (slot.Id == id && slot.count >= Receita[c].Quantidade)
                {
                    ItensSuficiente = true;
                    if (slot.ItemFake)
                    {
                        if (slot.transform.childCount > 1)
                        {
                            Destroy(slot.transform.GetChild(1).gameObject);
                        }
                        slot.ItemFake = false;
                        slot.countFake = 0;
                    }
                }
            }
            if (!ItensSuficiente)
            {
                foreach (Slot slotcraft in slotCraft)
                {
                    if (slotcraft.Numero == SlotDisponivel)
                    {
                        int index = inventario.remItem(id, 64, false, false, 0, Receita[c].Quantidade);
                        if (inventario.ItemNaoExiste)
                        {
                            ItemFake(slotcraft, Receita[c].Item, id, Receita[c].Quantidade, true);
                            break;
                        }
                        else
                            inventario.addItem(id, index, true);
                        if (inventario.ItemInsuficiente)
                        {
                            ItemFake(slotcraft, Receita[c].Item, id, Receita[c].Quantidade);
                            break;
                        }
                        break;
                    }
                }
            }
        }
    }
    public void OnPointerEnter(PointerEventData evenData)
    {
        Rect.sizeDelta = new Vector2(Rect.sizeDelta.x + 5, Rect.sizeDelta.y + 5);
        print("em cima");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        print("sai");
        Rect.sizeDelta = new Vector2(Rect.sizeDelta.x - 5, Rect.sizeDelta.y - 5);
    }
}
