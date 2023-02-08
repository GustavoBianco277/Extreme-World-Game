using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
public class item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public int Id, SlotCount, count;
	public Slot inSlot;
	public GameObject ItemObject;
	[HideInInspector]
	public Vector3 lastTransform;
	public TextMeshProUGUI ContText;
	public bool ItemFake = false, Select=false, Peguei = false, clonado = false, update = false;
	private Slot[] slots;
	private GameObject Object;
	public int disponivel;
	private SlotScalerItem SLT;
	public void OnPointerEnter (PointerEventData evenData)
	{
		if (!Select)
			Select = true;
	}
	public void OnPointerExit (PointerEventData eventData)
	{
		if (Select)
			Select = false;
	}
	void Start () 
	{
		lastTransform = transform.position;
		slots = FindObjectsOfType<Slot>();
		SLT = FindObjectOfType<SlotScalerItem>();
		disponivel = slots.Length;
	}
	void Update() 
	{
		if (SLT.InvAberto || update)
		{
			update = false;
			// passa o item para o inventario
			if (Select && Input.GetKey(KeyCode.LeftShift))
			{
				if (Input.GetMouseButtonDown(0))
				{
					// Passa o item da Hotbar para o inventario
					if (inSlot.Numero < 10 && !SLT.ChestAberto)
						PassaItem(true);

					// Passa o item para o bau
					else if (inSlot.Numero < 55 && SLT.ChestAberto)
						PassaItem(false, true);

					// Tira o item do Bau
					else if (inSlot.Numero >= 55 && SLT.ChestAberto)
						PassaItem(false, false, true);
				}
			}

			// Pega o item
			else if (Select && Input.GetMouseButtonDown(0) && !Peguei)
			{
				if (!ItemFake)
				{
					Transform i = transform.parent;
					if (inSlot.transform.parent.parent.name == "Craft")
						i.parent.parent.SetSiblingIndex(3);
					else
						i.parent.SetSiblingIndex(3);
					transform.parent.SetSiblingIndex(slots.Length);
				}
				if (inSlot != null)
					count = inSlot.count;
				Peguei = true;
			}

			// larga o item
			else if (Select && Input.GetMouseButtonDown(0) && Peguei)
			{
				if (!ItemFake)
				{
					int t = 0;
					for (int i = 0; i < slots.Length; i++)
					{
						if (slots[i].selected == true)
						{
							t = i;
							break;
						}
					}
					if (slots[t].ItemFake && slots[t].Id == Id)
					{
						if (slots[t].countFake - count <= 0)
						{
							if (slots[t].transform.childCount > 1)
							{
								Destroy(slots[t].transform.GetChild(1).gameObject);
								slots[t].ItemFake = false;
								slots[t].countFake = 0;
								slots[t].count += count;
								inSlot.Id = -1;
								inSlot.count = 0;
								Peguei = false;
								Destroy(this.gameObject);
							}
							else if (slots[t].transform.childCount == 1)
							{
								Destroy(slots[t].transform.GetChild(0).gameObject);
								transform.position = slots[t].transform.position;
								slots[t].ItemFake = false;
								slots[t].countFake = 0;
								slots[t].count += count;
								inSlot.Id = -1;
								inSlot.count = 0;
								inSlot = slots[t];
								lastTransform = transform.position;
								transform.SetParent(slots[t].transform);
								Peguei = false;
							}
						}
						else
						{
							slots[t].countFake -= count;
							slots[t].count += count;
							inSlot.Id = -1;
							inSlot.count = 0;
							Peguei = false;
							Destroy(this.gameObject);
						}
					}
					else if (t >= 0)
					{
						if (slots[t].Id == -1 || slots[t].Id == Id)
						{
							if (slots[t].Id == Id)
							{
								if (slots[t].transform.childCount > 1)
								{
									if (slots[t].transform.GetChild(0).GetComponent<item>().inSlot == inSlot)
									{
										slots[t].count += count;
										Destroy(this.gameObject);
										Peguei = false;
									}
								}
								else if (slots[t].count + inSlot.count <= 64)
								{
									if (inSlot.transform.childCount > 1)
									{
										slots[t].count += count;
										Destroy(this.gameObject);
										Peguei = false;
									}
									else
									{
										// Coloca o item na pilha
										if (slots[t] != inSlot)
										{
											inSlot.Id = -1;
											slots[t].count += count;
											inSlot.count = 0;
											Peguei = false;
											if (PhotonNetwork.IsConnected)
											{
												if (slots[t].Numero >= 55 || inSlot.Numero >= 55)
												{
													int IdChest = 0;
													foreach (Chest chest in FindObjectsOfType<Chest>())
													{
														if (chest.Opened)
														{
															IdChest = chest.View.ViewID;
															break;
														}
													}
													if (slots[t].Numero >= 55)
														servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slots[t].count, slots[t].Numero, true, true);

													if (inSlot.Numero >= 55)
														servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slots[t].count, inSlot.Numero, false, true);
												}
											}
											Destroy(this.gameObject);
										}
										else
										{
											Peguei = false;
											transform.position = lastTransform;
											count = 0;
										}
									}
								}
								else
								{
									int c = 64 - slots[t].count;
									if (inSlot.count != 64)
									{
										inSlot.count -= c;
										count = inSlot.count;
										slots[t].count += c;
									}
								}
							}
							else
							{
								bool clone = false;
								if (!clonado)
								{
									slots[t].count = inSlot.count;
									inSlot.count = 0;
									inSlot.Id = -1;
								}
								else
								{
									clonado = false;
									slots[t].count = count;
									count = 0;
									clone = true;
								}
								transform.position = slots[t].transform.position;
								lastTransform = transform.position;
								transform.SetParent(slots[t].transform);
								slots[t].Id = Id;
								Peguei = false;

								// Envia o item paro os outros Players
								if (PhotonNetwork.IsConnected)
								{
									if (slots[t].Numero >= 55 || inSlot.Numero >= 55)
									{
										int IdChest = 0;
										foreach (Chest chest in FindObjectsOfType<Chest>())
										{
											if (chest.Opened)
											{
												IdChest = chest.View.ViewID;
												break;
											}
										}
										if (slots[t].Numero >= 55)
											servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slots[t].count, slots[t].Numero, true, false);

										if (inSlot.Numero >= 55 && !clone)
											servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slots[t].count, inSlot.Numero, false, false);
									}
								}
								inSlot = slots[t];
							}
						}
						else
							transform.position = lastTransform;
					}
					else
						transform.position = lastTransform;
				}
			}

			// Divide os itens
			else if (Select && Input.GetMouseButton(1) && !Peguei && inSlot != null && inSlot.count > 1)
			{
				if (!ItemFake)
				{
					Transform i = transform.parent;
					if (inSlot.transform.parent.parent.name == "Craft")
						i.parent.parent.SetSiblingIndex(3);
					else
						i.parent.SetSiblingIndex(3);

					transform.parent.SetSiblingIndex(slots.Length);

					int cont;
					if (inSlot.count % 2 == 0)
						cont = inSlot.count / 2;
					else
						cont = (inSlot.count / 2) + 1;

					Object = Instantiate(this.gameObject, Input.mousePosition, Quaternion.identity);
					Object.GetComponent<item>().count = cont;
					inSlot.count /= 2;
					Object.transform.SetParent(inSlot.transform);
					Object.GetComponent<item>().clonado = true;
					Object.GetComponent<item>().Peguei = true;

					// Passa item para a pilha
					if (PhotonNetwork.IsConnected)
					{
						if (inSlot.Numero >= 55)
						{
							int IdChest = 0;
							foreach (Chest chest in FindObjectsOfType<Chest>())
							{
								if (chest.Opened)
								{
									IdChest = chest.View.ViewID;
									break;
								}
							}
							servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, inSlot.count, inSlot.Numero, true, true);
						}
					}
				}
			}

			// mantem o item na posiçao do mouse
			if (Peguei)
				transform.position = Input.mousePosition;

			// Count itens
			if (transform.parent.GetComponent<Slot>())
			{
				if (transform.parent.GetComponent<Slot>().countFake >= 1)
				{
					ContText.color = new Color(1, 0.4f, 0.4f, 1);
					ContText.text = "+" + transform.parent.GetComponent<Slot>().countFake.ToString();
				}

				else if (count > 1 && Peguei)
				{
					ContText.text = count.ToString();
					if (ContText.color != Color.white)
						ContText.color = Color.white;
				}

				else if (transform.parent.GetComponent<Slot>().count > 1 && !Peguei)
				{
					ContText.text = transform.parent.GetComponent<Slot>().count.ToString();
					if (ContText.color != Color.white)
						ContText.color = Color.white;
				}

				else if (transform.parent.GetComponent<Slot>().countFake == 0 && transform.parent.GetComponent<Slot>().count <= 1)
					ContText.text = string.Empty;
			}
		}
	}
	public bool PassaItem(bool Inventario=false, bool Chest=false, bool HotBar=false, bool verificar=false)
    {
		List<int> SlotCount = new List<int>();
		List<Slot> Slots = new List<Slot>();
		bool Completed = false;

		foreach(Slot slot in slots)
        {
			if (Inventario && slot.Numero > 9 && slot.Numero < 55)
				Slots.Add(slot);
			else if (Chest && slot.Numero >= 55)
				Slots.Add(slot);
			else if (HotBar && slot.Numero < 10 && slot.Numero > 0)
				Slots.Add(slot);
        }

		foreach (Slot slot in Slots)
		{
			if (slot.Numero < disponivel && slot.Id == -1 && inSlot != slot)
				disponivel = slot.Numero;

			if (slot.Id == Id && slot.count < 64 && inSlot != slot)
				SlotCount.Add(slot.count);
		}

		SlotCount.Sort();

		foreach (Slot slot in Slots)
		{
			// Se Tem espaço em um Slot
			if (SlotCount.Count > 0)
			{
                if (slot.Id == Id)
				{
                    for (int i = 0; i < SlotCount.Count; i++)
					{
						if (slot.count == SlotCount[i] && inSlot.count > 0)
						{
							int c = 64 - slot.count;
							if (inSlot.count + slot.count <= 64)
							{
								slot.count += inSlot.count;
								inSlot.count = 0;
								inSlot.Id = -1;

								// Passa item para a pilha
								if (PhotonNetwork.IsConnected)
								{
									if (slot.Numero >= 55 || inSlot.Numero >= 55)
									{
										int IdChest = 0;
										foreach (Chest chest in FindObjectsOfType<Chest>())
										{
											if (chest.Opened)
											{
												IdChest = chest.View.ViewID;
												break;
											}
										}
										if (slot.Numero >= 55)
											servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slot.count, slot.Numero, true, true);
										if (inSlot.Numero >= 55)
											servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slot.count, inSlot.Numero, false, true);
									}
								}
								Destroy(this.gameObject);
							}
							else
							{
								slot.count += c;
								inSlot.count -= c;
								transform.position = lastTransform;
							}
							Completed = true;
							break;
						}
					}
					if (Completed)
						break;
				}
			}

            // Verifica se ha espaço no Inventario
            else if (HotBar && SLT.ChestAberto)
            {
                Completed = PassaItem(true, verificar: true);
				if (Completed)
					break;
            }

            // Passa o item para um Slot vazio
            if (slot.Numero == disponivel && !verificar && !Completed && SlotCount.Count == 0)
			{
				inSlot.Id = -1;
				slot.count = inSlot.count;
				inSlot.count = 0;
				transform.position = slot.transform.position;
				lastTransform = transform.position;
				transform.SetParent(slot.transform);
				slot.Id = Id;
				disponivel = slots.Length;
				Completed = true;

				// Envia o item paro os outros Players
				if (!Inventario && PhotonNetwork.IsConnected)
				{
					int IdChest = 0;
					foreach (Chest chest in FindObjectsOfType<Chest>())
					{
						if (chest.Opened)
						{
							IdChest = chest.View.ViewID;
							break;
						}
					}
					if (!HotBar)
						servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slot.count, slot.Numero, true, false);
					if (inSlot.Numero >= 55)
						servidor.Server.RPC("ItemNetWork", RpcTarget.Others, IdChest, Id, slot.count, inSlot.Numero, false, false);
				}

				inSlot = slot;
				break;
			}
		}

		// se nao encontrou nenhum lugar para passar
		if (!Completed)
			print("Sem Espaço no inventario");
		return Completed;
	}
}