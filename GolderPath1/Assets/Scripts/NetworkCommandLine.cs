using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour {

    private NetworkManager netManager;

    // Start is called before the first frame update
    void Start() {
        netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor) return;

        var args = GetCommandLineArgs();

        if (args.TryGetValue("-mlapi", out string mlapiValue)) {
            switch (mlapiValue) {
                case "server":
                    netManager.StartServer();
                    break;
                case "host":
                    netManager.StartHost();
                    break;
                case "client":
                    netManager.StartClient();
                    break;
            }
        }
    }

    private Dictionary<string, string> GetCommandLineArgs() {

        Dictionary<string, string> argDict = new Dictionary<string, string>();
        
        var args = System.Environment.GetCommandLineArgs();

        string command = null;
        foreach (string arg in args) {
            if (arg.StartsWith("-")) {
                command = arg.ToLower();
            } else if (command != null) {
                argDict.Add(command, arg.ToLower());
                command = null;
            }
        }

        return argDict;
    }
}
