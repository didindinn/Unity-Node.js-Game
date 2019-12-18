using Project.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{

    public class PlayerManager : MonoBehaviour
    {

        [Header("Data")]
        [SerializeField]
        private float mSpeed = 4;

        [Header("Class References")]
        [SerializeField]
        private NetworkIdentity mNetworkIdentity;



        void Update()
        {
            if (mNetworkIdentity.IsControlling())
            {
                checkMovement();
            }
        }

        private void checkMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.position += new Vector3(horizontal, 0, vertical) * mSpeed * Time.deltaTime;
        }
    }

}