using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCollider : MonoBehaviour
{

    private Transform[,] fingerBonesLeft = new Transform[5,4];
    private Transform[,] fingerBonesRight = new Transform[5, 4];
    private string[] fingerName_mixamo = { "Thumb", "Index", "Middle", "Ring", "Pinky" };


    private struct FingerBone
    {
        public readonly float Radius;
        public readonly float Height;
        public FingerBone(float radius, float height)
        {
            Radius = radius;
            Height = height;
        }

        public Vector3 GetCenter(bool isLeftHand)
        {
            //return new Vector3(((isLeftHand) ? -1 : 1) * Height / 2.0f, 0, 0);
            return new Vector3(0, Height / 2.0f, 0);
        }
    };

    private readonly FingerBone Phalanges = new FingerBone(0.01f, 0.03f);
    private readonly FingerBone Metacarpals = new FingerBone(0.01f, 0.05f);



    private void CreateCollider(Transform transform)
    {
        
        if (!transform.gameObject.GetComponent(typeof(CapsuleCollider)) && !transform.gameObject.GetComponent(typeof(SphereCollider)) && transform.name.Contains("Hand"))
        {
            if (transform.name.Contains("Thumb") || transform.name.Contains("Index") || transform.name.Contains("Middle") || transform.name.Contains("Ring") || transform.name.Contains("Pinky"))
            {
                CapsuleCollider collider = transform.gameObject.AddComponent<CapsuleCollider>();
                if (!transform.name.EndsWith("1"))
                {
                    collider.radius = Phalanges.Radius;
                    collider.height = Phalanges.Height;
                    collider.center = Phalanges.GetCenter(transform.name.Contains("Left"));
                    collider.direction = 1; //y
                }
                else
                {
                    collider.radius = Metacarpals.Radius;
                    collider.height = Metacarpals.Height;
                    collider.center = Metacarpals.GetCenter(transform.name.Contains("Left"));
                    collider.direction = 1;
                }

                var capsuleRigidBody = transform.gameObject.AddComponent<Rigidbody>();
                capsuleRigidBody.mass = 1.0f;
                capsuleRigidBody.isKinematic = true;
                capsuleRigidBody.useGravity = false;
                capsuleRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            else
            {
                Debug.Log("error: check fingerbone init: " + transform.name);
            }
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int fingerIndex = 0; fingerIndex <= fingerBonesLeft.GetUpperBound(0); fingerIndex++)
        {
            for (int fingerBoneIndex = 0; fingerBoneIndex <= fingerBonesLeft.GetUpperBound(1); fingerBoneIndex++)
            {
                //mixamorig:LeftHandMiddle1
                string name = fingerName_mixamo[fingerIndex] + (fingerBoneIndex + 1);
                string namePrefixLeft = "mixamorig:LeftHand";
                string namePrefixRight = "mixamorig:RightHand";
                //Transform transformLeft = GameObject.Find(namePrefixLeft + name).transform;
                Transform transformLeft = this.transform.FindDeepChild(namePrefixLeft + name).transform;
                fingerBonesLeft[fingerIndex, fingerBoneIndex] = transformLeft;
                //Transform transformRight = GameObject.Find(namePrefixRight + name).transform;
                Transform transformRight = this.transform.FindDeepChild(namePrefixRight + name).transform;
                fingerBonesRight[fingerIndex, fingerBoneIndex] = transformRight;

            }
        }

        for (int fingerIndex = 0; fingerIndex <= fingerBonesLeft.GetUpperBound(0); fingerIndex++)
        {
            for (int fingerBoneIndex = 0; fingerBoneIndex <= fingerBonesLeft.GetUpperBound(1); fingerBoneIndex++)
            {
                Transform boneLeft = fingerBonesLeft[fingerIndex, fingerBoneIndex];
                if(!boneLeft.name.Contains("4"))
                {
                    CreateCollider(boneLeft);
                }

                Transform boneRight = fingerBonesRight[fingerIndex, fingerBoneIndex];
                if (!boneRight.name.Contains("4"))
                {
                    CreateCollider(boneRight);
                }
            }
        }

        //create sphere collider for palm
        Transform lefthand = this.transform.FindDeepChild("mixamorig:LeftHand").transform;
        Transform righthand = this.transform.FindDeepChild("mixamorig:RightHand").transform;

        BoxCollider colliderLeft = lefthand.gameObject.AddComponent<BoxCollider>();
        colliderLeft.size = new Vector3(0.08f, 0.09f, 0.04f);
        colliderLeft.center = new Vector3(0, 0.03f, 0f);
        
        Rigidbody capsuleRigidBodyLeft = lefthand.gameObject.AddComponent<Rigidbody>();
        capsuleRigidBodyLeft.mass = 1.0f;
        capsuleRigidBodyLeft.isKinematic = true;
        capsuleRigidBodyLeft.useGravity = false;
        capsuleRigidBodyLeft.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        BoxCollider colliderRight = righthand.gameObject.AddComponent<BoxCollider>();
        colliderRight.size = new Vector3(0.08f, 0.09f, 0.04f);
        colliderRight.center = new Vector3(0, 0.03f, 0f);

        Rigidbody capsuleRigidBodyRight = righthand.gameObject.AddComponent<Rigidbody>();
        capsuleRigidBodyRight.mass = 1.0f;
        capsuleRigidBodyRight.isKinematic = true;
        capsuleRigidBodyRight.useGravity = false;
        capsuleRigidBodyRight.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
