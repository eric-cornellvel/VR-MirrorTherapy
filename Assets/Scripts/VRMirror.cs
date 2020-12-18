using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMirror : MonoBehaviour
{
    public Transform leftHand; 
    public Transform mirrorLeftHand;
    public Transform rightHand;
    public Transform mirrorRightHand;

    public Transform playerTransform;

    void Start()
    {

    }

    void Update()
    {
        MirrorFromTo(leftHand, mirrorLeftHand);
        MirrorFromTo(rightHand, mirrorRightHand);
    }

    void MirrorFromTo(Transform sourceTransform, Transform destTransform)
    {
        // Determine dest position
        Vector3 playerToSourceHand = sourceTransform.position - playerTransform.position;
        Vector3 playerToDestHand = ReflectRelativeVector(playerToSourceHand);
        destTransform.position = playerTransform.position + playerToDestHand;

        // Determine dest rotation
        
        Quaternion destRot = sourceTransform.rotation;

        //destRot.y = -destRot.y;
        //destRot.z = -destRot.z;
        destRot.x = -destRot.x;
        destRot.w = -destRot.w;
        destTransform.rotation = destRot;
        Vector3 offset = new Vector3(180.0f, 0.0f, 0.0f); //flip upside down
        destTransform.rotation *= Quaternion.Euler(offset);


        /*
        Quaternion destRot = sourceTransform.localRotation;

        //destRot.y = -destRot.y;
        //destRot.z = -destRot.z;
        destRot.x = -destRot.x;
        destRot.w = -destRot.w;
        destTransform.localRotation = destRot;
        */
    }

    Vector3 ReflectRelativeVector(Vector3 relativeVec)
    {
        // relativeVec
        //     Take the relative vector....
        // + Vector3.Dot(relativeVec, playerTransform.right)
        //     and for how far along the player's right direction it is 
        //     away from the player (may be negative),
        // * playerTransform.right
        //     move it that distance along the player's right...
        // * -2f
        //    negative two times (i.e., along the left direction 2x)

        //change from Vector3.right to playerTransform.right
        return relativeVec
        + Vector3.Dot(relativeVec, playerTransform.right)
            * playerTransform.right
            * -2f;
    }
}