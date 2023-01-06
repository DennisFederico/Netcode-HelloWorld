using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkVariableTest : NetworkBehaviour {
    private NetworkVariable<float> serverNetworkVariable = new NetworkVariable<float>();
    private NetworkVariable<float> clientNetworkVariable = new NetworkVariable<float>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    private float last_t = 0.0f;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            serverNetworkVariable.Value = 0.0f;
            Debug.Log($"Server's var initialized to: {serverNetworkVariable.Value}");
        } else if (IsClient) {
            clientNetworkVariable.Value = 0.0f;
            Debug.Log($"Client's var initialized to: {clientNetworkVariable.Value}");
        }
    }

    void Update() {
        var t_now = Time.time;

        if (IsServer) {
            serverNetworkVariable.Value = serverNetworkVariable.Value + 0.1f;

            if (t_now - last_t > 0.5f) {
                last_t = t_now;
                Debug.Log($"Server set its var to: {serverNetworkVariable.Value}, client var at: {(clientNetworkVariable != null ? clientNetworkVariable.Value : "not init yet!")}");
            }
        }

        if (IsClient) {
            clientNetworkVariable.Value = clientNetworkVariable.Value + 0.1f;
            if (t_now - last_t > 0.5f) {
                last_t = t_now;
                Debug.Log($"Client set its var to: {clientNetworkVariable.Value}, server var at: {serverNetworkVariable.Value}");
            }
        }
    }
}
