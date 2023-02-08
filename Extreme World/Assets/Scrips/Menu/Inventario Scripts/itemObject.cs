using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemObject : MonoBehaviour
{
    public GameObject ItemModel;
    [HideInInspector]
    public ItemCraft ItemCraft;
    public AudioSource CraftMetalSound, CraftSound;
    public int id;
    public int idCraft;
    public int cont;
    private Inv inventario;
    private SistemaConstrucao SisConstrucao;
    private SlotScalerItem SSI;

    private void Start()
    {
        inventario = FindObjectOfType<Inv>();
        SisConstrucao = FindObjectOfType<SistemaConstrucao>();
        SSI = FindObjectOfType<SlotScalerItem>();
    }
    public void Craft()
    {
        bool PodeCraft;

        PodeCraft = ItemCraft.CraftItem();

        if (PodeCraft)
        {
            if (ItemCraft.Construction.Construct)
            {
                SisConstrucao.enabled = true;
                Destroy(SisConstrucao.ObjetoFantasma);
                SisConstrucao.ObjetoFantasma = Instantiate(ItemCraft.Construction.ObjFantasma);
                SisConstrucao.layer = ItemCraft.Construction.layer;
                SisConstrucao.Altura = ItemCraft.Construction.Altura;
                SisConstrucao.AjustTerrain = ItemCraft.Construction.AjustTerrain;
                SisConstrucao.OneBuild = ItemCraft.Construction.OneBuild;
                SSI.CloseInventario();
            }
            else
            {
                inventario.addItem(id, cont);
                if (ItemCraft.Metal)
                    CraftMetalSound.Play();
                else
                    CraftSound.Play();
            }
        }
        else
            global::ItemCraft.SomItemFake.Play();
    }
}
