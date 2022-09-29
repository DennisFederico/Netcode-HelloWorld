using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace HelloWorld {
    public class HelloWorldManager : MonoBehaviour {
        void OnGUI() {
            GUILayout.BeginArea(new Rect(10,10,300,300));
            if (!NetworkManager.Singleton.IsClient && ! NetworkManager.Singleton.IsServer) {
                StartButtons();                
            } else {
                StatusLabels();
                SubmitNewPosition();
            }
            GUILayout.EndArea();
        }

        void StartButtons() {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        void StatusLabels() {
            var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            GUILayout.Label($"Transport: { NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name }");
            GUILayout.Label($"Mode: { mode }");
        }

        static void SubmitNewPosition() {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Move")) {
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient) {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                    }
                } else {
                    NetworkObject netObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    netObject.GetComponent<HelloWorldPlayer>().Move();
                }
            }
        }
    }
}