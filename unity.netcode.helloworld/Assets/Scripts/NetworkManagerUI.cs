using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
    [SerializeField] private Button startServerBtn;
    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startClientBtn;


    private void Awake() {
        startServerBtn.onClick.AddListener(() =>  NetworkManager.Singleton.StartServer());
        startHostBtn.onClick.AddListener(() =>  NetworkManager.Singleton.StartHost());
        startClientBtn.onClick.AddListener(() =>  NetworkManager.Singleton.StartClient());
    }
}