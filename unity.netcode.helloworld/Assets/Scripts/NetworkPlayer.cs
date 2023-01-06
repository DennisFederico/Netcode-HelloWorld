using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NetworkPlayer : NetworkBehaviour {
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private Transform spawnPrefab;
    [SerializeField] private Transform spawn;

    private NetworkVariable<int> _randomInt = new NetworkVariable<int>(0, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);
    private NetworkVariable<MyDataType> _mydata = new NetworkVariable<MyDataType>(new MyDataType {
        _int = 0,
        _bool = true,
        _message = "Player"
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    
    public override void OnNetworkSpawn() {
        _randomInt.OnValueChanged += (oldVal, newVal) => {
            Debug.Log($"{OwnerClientId} - valueChanged: {oldVal} to {newVal}");
        };

        _mydata.OnValueChanged += (oldVal, newVal) => {
            Debug.Log($"My local client ID: {NetworkManager.Singleton.LocalClientId}");
            Debug.Log(OwnerClientId == NetworkManager.Singleton.LocalClientId
                ? $"{OwnerClientId} - valueChanged: {oldVal._int}:{newVal._int} to {oldVal._bool}:{newVal._bool} - {newVal._message}"
                : $"NOT MY DATA {OwnerClientId} - valueChanged: {oldVal._int}:{newVal._int} to {oldVal._bool}:{newVal._bool} - {newVal._message}");
        };
    }

    public struct MyDataType : INetworkSerializable {

        public int _int;
        public bool _bool;
        public FixedString32Bytes _message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _message);
        }
    }
    
    void Update() {

        //Debug.Log($"{OwnerClientId} - randomNumber: {_randomInt.Value}");
        
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.R)) {
            TestServerRpc();
        }
        
        if (Input.GetKeyDown(KeyCode.C)) {
            TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> {1}}});
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            if (spawn) {
                //This keeps the object alive (at server) but removes from network (for clients)
                if (spawn.GetComponent<NetworkObject>().IsSpawned) {
                    spawn.GetComponent<NetworkObject>().Despawn(false);
                    spawn.gameObject.SetActive(false);
                } else {
                    spawn.transform.position += (Vector3.forward + Vector3.right) * 2f;
                    spawn.GetComponent<NetworkObject>().Spawn(true);
                    spawn.gameObject.SetActive(true); //<- The active state is not sync to clients
                }
                //Destroy(spawn.gameObject);
            } else {
                spawn = Instantiate(spawnPrefab);
                spawn.GetComponent<NetworkObject>().Spawn(true);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T)) {
            //_randomInt.Value = Random.Range(0, 100);
            _mydata.Value = new MyDataType {
                _int = Random.Range(0,100), 
                _bool = !_mydata.Value._bool,
                _message = $"Player {OwnerClientId}"
            };
        }

        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;

        transform.position += direction * (speed * Time.deltaTime);
    }

    [ServerRpc]
    private void TestServerRpc() {
        //ServerRpcParams params as argument?
        Debug.Log($"RPC Called by {OwnerClientId} - Run on Server");
    }

    [ClientRpc]
    private void TestClientRpc(ClientRpcParams parameter) {
        Debug.Log($"Client RPC test Owner:{OwnerClientId} Client:{NetworkManager.Singleton.LocalClientId} Param:{parameter.Send.TargetClientIds}");
    }
}