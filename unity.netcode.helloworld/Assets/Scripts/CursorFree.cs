using UnityEngine;

public class CursorFree : MonoBehaviour {
    private void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}