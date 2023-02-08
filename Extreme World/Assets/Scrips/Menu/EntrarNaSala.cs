using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EntrarNaSala : MonoBehaviour {
	public int PlayerCount;
	public int MaxPlayers;
	private Multiplayer Multiplayer;
    private void Start()
    {
		Multiplayer = FindObjectOfType<Multiplayer>();
    }
    public void entrarNaSala (TMPro.TextMeshProUGUI Texto)
	{
		if (PlayerCount == MaxPlayers)
		{
			Multiplayer.MaxPlayersError.SetActive(true);
			Multiplayer.MaxPlayersError.GetComponent<AnimateMessages>().reset = true;
		}
		else if (PhotonNetwork.LocalPlayer.NickName == string.Empty)
		{
			Multiplayer.NameError.SetActive(true);
			Multiplayer.NameError.GetComponent<AnimateMessages>().reset = true;
			Multiplayer.Name.ActivateInputField();
		}
		else
		{
			servidor Sv = FindObjectOfType<servidor>();
			Sv.TextNameRoom = Texto.text;
			Sv.Entrei = true;

			Sv.SceneName = "Game";
			SceneManager.LoadScene("Carregamento");

			if (Multiplayer.NameError.activeInHierarchy == true)
				Multiplayer.NameError.SetActive(false);
		}
	}
}
