using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

namespace Project.Networking {
    public class NetworkClient : SocketIOComponent
    {

        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefab;

        public static string ClientID { get; private set; }

        private Dictionary<string, NetworkIdentity> serverObjects;
        public override void Start()
        {
            SetInitialReferences();
            SetFrameRate();
            base.Start();   //This calls the Start method off of the base class.
            SetupEvents();
        }

        public override void Update()
        {
            base.Update();
        }

        private void SetFrameRate()
        {
            Debug.Log("Set frame rate to 30");
            Application.targetFrameRate = 30;
        }

        private void SetupEvents()
        {
            On("open", (Event) =>
            {
                Debug.Log("Connection made to the server");
            });

            On("register", (Event) =>
            {
                ClientID = Event.data["id"].ToString();
                Debug.LogFormat("Our Client's ID ({0})", ClientID);
            });

            On("spawn", (Event) =>
            {
                Debug.Log("Player spawned");
                string id = Event.data["id"].ToString();
                Debug.Log(id);

                GameObject go = Instantiate(playerPrefab, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);

                Debug.Log(ni);
                serverObjects.Add(id, ni);
            });

            On("disconnected", (Event) =>
            {
                Debug.Log("Player disconnected");
                string id = Event.data["id"].ToString();
                Debug.Log(id);

                GameObject go = serverObjects[id].gameObject;
                Debug.Log(go);
                Destroy(go);        //Remove from game
                serverObjects.Remove(id);       //Remove from memory
            });

            On("updatePosition", (Event) => {
                Debug.Log("Position updated");
                string id = Event.data["id"].ToString();
                float x = Event.data["position"]["x"].f;
                float y = Event.data["position"]["y"].f;
                float z = Event.data["position"]["z"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, z);
            });
        }

        private void SetInitialReferences()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

    };

    [Serializable]
    public class Player
    {
        public string id;
        public Position position;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }

};