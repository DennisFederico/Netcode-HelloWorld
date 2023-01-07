using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class FollowVirtualCamera : NetworkBehaviour {
   [SerializeField] private Transform cameraRoot;
   
   public override void OnNetworkSpawn() {
      var vCamera = FindObjectOfType<CinemachineVirtualCamera>();
      vCamera.Follow = cameraRoot;
   }
}