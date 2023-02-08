using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Itm
{
    public int Id;
    public int count;
}
public class Inv : MonoBehaviour
{
    public int disponivel;
    public List<GameObject> itensObjs = new List<GameObject>();
    public bool ItemNaoExiste = false, ItemInsuficiente = false;

    public bool addItem (int id, int c, bool Craft=false, bool Chest=false, int Numero=0)
    {
        ItemNaoExiste = false;
        bool comprete = false;
        Slot[] slots = FindObjectsOfType<Slot>();
        disponivel = slots.Length;
        for (int i = 0; i < slots.Length; i++)
        {
            if (Craft) 
            {
                if (slots[i].Numero < disponivel && slots[i].Id == -1 && slots[i].Numero < 0)
                    disponivel = slots[i].Numero;         
            }
            else if (Chest)
                disponivel = Numero;
            else
            {
                if (slots[i].Numero < disponivel && slots[i].Id == -1 && slots[i].Numero > 0 && slots[i].Numero < 55)
                    disponivel = slots[i].Numero;
                if (slots[i].Id == id && slots[i].count + c <= 64 && slots[i].Numero > 0 && slots[i].Numero < 55)
                {
                    slots[i].count += c;
                    comprete = true;
                    slots[i].transform.GetChild(0).GetComponent<item>().update = true;
                    break;
                }
            } 
        }
        if (comprete == false)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Id == -1 && slots[i].Numero == disponivel)
                {
                    GameObject itemObj = null;
                    foreach (GameObject item in itensObjs)
                    {
                        if (item.GetComponent<item>().Id == id)
                            itemObj = item;
                    }
                    
                    slots[i].Id = id;
                    slots[i].count = c;
                    GameObject obj = Instantiate(itemObj, slots[i].transform.position, Quaternion.identity,slots[i].transform);
                    obj.GetComponent<item>().inSlot = slots[i];
                    obj.GetComponent<item>().Id = id;
                    obj.GetComponent<item>().update = true;
                    comprete = true;
                    break;
                }
                else if (Chest && slots[i].Numero == disponivel && slots[i].Id != -1)
                    slots[i].count = c;
            }
        }
        if (!comprete)
            print("Nao Ha Espaço no inventario");
        else
            FindObjectOfType<SlotScalerItem>().AjusteItens();

        return comprete;
    }
    public int remItem(int id, int c, bool Craft=false, bool Chest=false, int Numero=0, int Quantidade = -1, bool Notfake = false)
    {
        ItemNaoExiste = false;
        ItemInsuficiente = false;
        bool comprete = false;
        int SlotCount = 0, Index = 0;
        Slot[] Slots = FindObjectsOfType<Slot>();
        List<Slot> slots = new List<Slot>();
        // procura os slots correspondentes

        if (Craft)
        {
            foreach (Slot slot in Slots)
            {
                if (slot.Numero < 0)
                    slots.Add(slot);
            }
        }
        else if (Chest)
        {
            foreach (Slot slot in Slots)
            {
                if (slot.Numero >= 55)
                    slots.Add(slot);
            }
        }
        else
        {
            foreach (Slot slot in Slots)
            {
                if (slot.Numero >= 0 && slot.Numero < 55)
                    slots.Add(slot);
            }
        }
        // Procura o slot que tem mais itens

        if (!Craft)
        {
            foreach (Slot slot in slots)
            {
                if (slot.Id == id)
                {
                    if (slot.count > SlotCount)
                        SlotCount = slot.count;
                }
            }
        }
        for (int i = 0; i < slots.Count; i++)
        {
            // Remove os itens fake
            if (slots[i].ItemFake && slots[i].count == 0 && Notfake)
            {
                if (slots[i].transform.childCount > 0)
                {
                    if (slots[i].transform.childCount == 1)
                        slots[i].Id = -1;
                    Destroy(slots[i].transform.GetChild(slots[i].transform.childCount - 1).gameObject);
                    slots[i].countFake = 0;
                    slots[i].ItemFake = false;
                }
            }
            if (id == -2)
                comprete = true;
            if (slots[i].Id == id && !comprete && !Chest)
            {
                if (!Craft)
                {
                    if (slots[i].count == SlotCount)
                    {
                        if (Quantidade > 0)
                        {
                            if (SlotCount < Quantidade)
                                ItemInsuficiente = true;
                        }
                    }
                    else
                        continue;
                }

                // Remove o item do inventario

                Index = slots[i].count;
                slots[i].count -= c;
                if (slots[i].count <= 0)
                {
                    slots[i].Id = -1;
                    slots[i].count = 0;
                    if (slots[i].transform.childCount > 0)
                        Destroy(slots[i].transform.GetChild(0).gameObject);
                }
                comprete = true;
                break;
            } 
            if (Chest && slots[i].Numero == Numero)
            {
                slots[i].Id = -1;
                slots[i].count = 0;
                if (slots[i].transform.childCount >= 1)
                    Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
        if (comprete == false)
            ItemNaoExiste = true;

        return Index;
    }
    public void Clear(int NumI, int NumF)
    {
        foreach(Slot slot in FindObjectsOfType<Slot>())
        {
            if (slot.Numero >= NumI && slot.Numero <= NumF && !slot.ItemFake)
            {
                slot.Id = -1;
                slot.count = 0;
                if (slot.transform.childCount >= 1)
                    Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
    }
    public void Agrupar(List<Slot> Slots, int NumI, int NumF)
    {
        List<Itm> ListItens = new List<Itm>();
        int cont;
        bool Agrup = false;
        foreach (Slot slot in Slots)
        {
            bool Completo = false;
            if (slot.Id != -1 && !slot.ItemFake)
            {
                foreach(Itm item in ListItens)
                {
                    if (item.Id == slot.Id && item.count < 64)
                    {
                        if (item.count + slot.count <= 64)
                            item.count += slot.count;
                        else
                        {
                            cont = 64 - item.count;
                            item.count = 64;
                            slot.count -= cont;
                            Itm s = new Itm();
                            s.Id = slot.Id;
                            s.count = slot.count;
                            ListItens.Add(s);
                        }
                        Completo = true;
                        break;
                    }
                    else
                        Completo = false;
                }
                if (Completo)
                    Agrup = true;
                else
                {
                    Itm s = new Itm();
                    s.Id = slot.Id;
                    s.count = slot.count;
                    ListItens.Add(s);
                }
            }
        }
        if (Agrup)
        {
            Clear(NumI, NumF);
            foreach (Itm item in ListItens)
            {
                addItem(item.Id, item.count, true);
            }
        }
    }
}    