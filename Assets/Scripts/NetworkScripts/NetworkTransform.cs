using Project.Utility.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkTransform : MonoBehaviour
    {

        [SerializeField]
        [GreyOut]
        private Vector3 mOldPosition;

        private NetworkIdentity mNetworkIdentity;
        private Player mPlayer;

        private float mStillCounter = 0;

        // Start is called before the first frame update
        public void Start()
        {
            mNetworkIdentity = GetComponent<NetworkIdentity>();
            mOldPosition = transform.position;
            mPlayer = new Player();

            mPlayer.position = new Position();
            mPlayer.position.x = 0;
            mPlayer.position.y = 0;
            mPlayer.position.z = 0;

            if (!mNetworkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (mNetworkIdentity.IsControlling())
            {
                if(mOldPosition != transform.position)      //I could restructure this to be an event
                {
                    mOldPosition = transform.position;
                    mStillCounter = 0;
                    sendData();
                }
                else
                {
                    mStillCounter += Time.deltaTime;

                    if(mStillCounter >= 1)                  //I could restructure this to be an event
                    {
                        mStillCounter = 0;
                        sendData();
                    }
                }
            }
        }

        //Update player information
        private void sendData()
        {
            mPlayer.position.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
            mPlayer.position.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
            mPlayer.position.z = Mathf.Round(transform.position.z * 1000.0f) / 1000.0f;

            mNetworkIdentity.GetSocket().Emit("updatePosition", new JSONObject(JsonUtility.ToJson(mPlayer)));
        }


    }
}