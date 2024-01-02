using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;


namespace Lightbug.CharacterControllerPro.Demo
{
    public class Move : MonoBehaviour
   {

      public CharacterActor characterActor = null;
      public static float rotationSpeed = 1.0f;
      public static float verticalSpeed = 150.0f;

      public Transform vrHeadset;

        // Start is called before the first frame update
        void Start()
       {
        
       }
    // Update is called once per frame
       void FixedUpdate()
       {
        float r = rotationSpeed * Input.GetAxis("MovementX");
        float v = verticalSpeed * Input.GetAxis("MovementY");
      //   characterActor.PlanarVelocity = -v*characterActor.transform.right;
        characterActor.PlanarVelocity = v*characterActor.transform.forward;
        characterActor.RotateYaw(r);
        
       }
        

        void Update()
        {

            if (vrHeadset != null)
            {
                // Get the rotation of the VR headset
                Quaternion vrRotation = vrHeadset.rotation;

                // Apply the VR rotation to the player (this assumes the script is attached to the player GameObject)
                transform.rotation = vrRotation;
            }


        }
        

   }
}
