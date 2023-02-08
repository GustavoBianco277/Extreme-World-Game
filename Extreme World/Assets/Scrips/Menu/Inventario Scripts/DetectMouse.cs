using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class DetectMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool ItemCraft;
    public void OnPointerEnter(PointerEventData evenData)
    {
        if (ItemCraft)
            transform.GetComponent<Image>().color = new Color(1, 0, 0, 0.4f);
        else
        {
         if (transform.childCount >= 1)
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemCraft)
            transform.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
        else
        {
            if (transform.childCount >= 1)
                transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}