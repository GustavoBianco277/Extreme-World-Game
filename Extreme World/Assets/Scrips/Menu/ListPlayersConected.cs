using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ListPlayersConected : MonoBehaviour
{
    public GameObject ListPlayerStats, Player;
    public KeyCode KeyActive = KeyCode.Tab;
    public List<Transform> StatusList = new List<Transform>();

    void Update()
    {
        if (Input.GetKeyDown(KeyActive) && PhotonNetwork.IsConnected)
        {
            ListPlayerStats.SetActive(true);
            servidor.Server.RPC("ActivePlayerStats", RpcTarget.All, true);
        }

        else if (Input.GetKeyUp(KeyActive) && PhotonNetwork.IsConnected)
        {
            ListPlayerStats.SetActive(false);
            servidor.Server.RPC("ActivePlayerStats", RpcTarget.All, false);
        }

    }
    public void PlayersStatus(int Ping, int Id, string Name)
    {
        print("Cheguei");
        bool complete = false;
        if (StatusList.Count > 0) 
        { 
            foreach(Transform T in StatusList)
            {
                if (T.GetChild(0).GetComponent<TextMeshProUGUI>().text == Id.ToString())
                {
                    T.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Ping: " + Ping;
                    complete = true;
                    break;
                }
            }
        }
        if (!complete)
        {
            GameObject S = Instantiate(Player);
            S.transform.SetParent(ListPlayerStats.transform);
            S.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Id.ToString();
            S.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Name;
            S.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Ping: " + Ping;
            StatusList.Add(S.transform);
        }
    }
}
