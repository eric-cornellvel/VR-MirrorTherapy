using System.Collections.Generic;
using UnityEngine;
using bid = OVRSkeleton.BoneId;

/// <summary>
/// Class for mapping Oculus hand tracking to RocketBox avatar hand
/// Attach this script to RocketBox avatar gameobject.
/// </summary>
public class MixamoOVRHandTracking : MonoBehaviour
{
    public OVRSkeleton OVRSkeleton_L;
    public OVRSkeleton OVRSkeleton_R;
    public Transform anchorOffsetL;
    public Transform anchorOffsetR;
    public Transform anchorOffsetML; //seems not needed


    private AvatarHand handL;
    private AvatarHand handR;
    private bool isLeft = false;

    void Start()
    {
        //TODO-save default bone transform so that we can reset when switch back to controller
        handL = new AvatarHand(AvatarHand.HandType.LeftHand, anchorOffsetL.rotation);
        handR = new AvatarHand(AvatarHand.HandType.RightHand, anchorOffsetR.rotation);

    }

    void LateUpdate()
    {
        if (VRRig.Instance.isCalibrated)
        {
            OVRPlugin.Controller control = OVRPlugin.GetActiveController();
            if (control == OVRPlugin.Controller.Hands || control == OVRPlugin.Controller.LHand || control == OVRPlugin.Controller.RHand || control == OVRPlugin.Controller.None)
            {
                if (VRRig.Instance.curMode == VRRig.MirrorMode.None)
                {
                    handL.UpdateHand(OVRSkeleton_L, anchorOffsetL.rotation);
                    handR.UpdateHand(OVRSkeleton_R, anchorOffsetR.rotation);
                }
                else if (VRRig.Instance.curMode == VRRig.MirrorMode.MirrorLeft)
                {
                    handL.UpdateHand(OVRSkeleton_L, anchorOffsetL.rotation);
                    handR.UpdateHand(OVRSkeleton_L, anchorOffsetR.rotation);
                }
                else if (VRRig.Instance.curMode == VRRig.MirrorMode.MirrorRight)
                {
                    handL.UpdateHand(OVRSkeleton_R, anchorOffsetL.rotation);
                    handR.UpdateHand(OVRSkeleton_R, anchorOffsetR.rotation);
                }
            }
        }
        
    }

    public void SaveFingerPose()
    {
        handL.SaveRestPose();
        handR.SaveRestPose();
    }

    public void ResetFingerPose()
    {
        handL.resetPose();
        handR.resetPose();
    }

    /// <summary>
    /// Class for avatar's hand, including 5 fingers and 3 bones on each finger.
    /// </summary>
    class AvatarHand
    {
        public enum HandType
        {
            LeftHand,
            RightHand
        }

        public HandType handType;
        public FingerBone[,] fingerBones = new FingerBone[5,3];
        public Quaternion[,] restPoseBones = new Quaternion[5, 3];

        private string[] fingerName_mixamo = { "Thumb", "Index", "Middle", "Ring", "Pinky" };

        /// <summary>
        /// Auto detect finger bones, set offset.
        /// </summary>
        /// <param name="type">Left or right hand</param>
        /// <param name="offset">Offset the finger rotation to make it works correctly. For right hand, it needs an offset of Vector3(0.0f, 0.0f, 180.0f).</param>
        public AvatarHand(HandType type, Quaternion offset)
        {
            handType = type;

            for (int fingerIndex = 0; fingerIndex <= fingerBones.GetUpperBound(0); fingerIndex++)
            {
                for(int fingerBoneIndex = 0; fingerBoneIndex <= fingerBones.GetUpperBound(1); fingerBoneIndex++)
                {
                    if (VRRig.Instance.avatarType == VRRig.AvatarType.RockectBox)
                    {
                        string name = "Finger" + fingerIndex + ((fingerBoneIndex == 0) ? "" : fingerBoneIndex.ToString());
                        string namePrefix = "Bip01 " + ((type == HandType.LeftHand) ? "L " : "R ");
                        Transform transform = GameObject.Find(namePrefix + name).transform;

                        fingerBones[fingerIndex, fingerBoneIndex] = new FingerBone(name, transform, offset, handType);
                    }else if(VRRig.Instance.avatarType == VRRig.AvatarType.Mixamo)
                    {
                        //mixamorig:LeftHandMiddle1
                        string name = fingerName_mixamo[fingerIndex] + (fingerBoneIndex + 1);
                        string namePrefix = "mixamorig:" + ((type == HandType.LeftHand) ? "LeftHand" : "RightHand");
                        Transform transform = GameObject.Find(namePrefix + name).transform;

                        fingerBones[fingerIndex, fingerBoneIndex] = new FingerBone(name, transform, offset, handType);
                    }
                }
            }
        }

        public void SaveRestPose()
        {
            for (int fingerIndex = 0; fingerIndex <= fingerBones.GetUpperBound(0); fingerIndex++)
            {
                for (int fingerBoneIndex = 0; fingerBoneIndex <= fingerBones.GetUpperBound(1); fingerBoneIndex++)
                {
                    restPoseBones[fingerIndex, fingerBoneIndex] = fingerBones[fingerIndex, fingerBoneIndex].transform.rotation;
                }
            }
        }

        public void resetPose()
        {
            for (int fingerIndex = 0; fingerIndex <= fingerBones.GetUpperBound(0); fingerIndex++)
            {
                for (int fingerBoneIndex = 0; fingerBoneIndex <= fingerBones.GetUpperBound(1); fingerBoneIndex++)
                {
                    fingerBones[fingerIndex, fingerBoneIndex].transform.rotation = restPoseBones[fingerIndex, fingerBoneIndex];
                }
            }
        }

        /// <summary>
        /// Update the fingers of this hand, so they rotate as the OVR skeleton rotates. Call this in your program's LateUpdate()
        /// </summary>
        /// <param name="s"> OVRSkeleton, provided by Oculus Integration SDK. </param>
        public void UpdateHand(OVRSkeleton s, Quaternion offset)
        {
            if (s == null || s.Bones.Count == 0)
            {
                return;
            }
            foreach (FingerBone f in fingerBones)
            {
                //f.UpdateFinger(s); // if we want to change the offset for fingers, we can use UpdateFinger(s, newOffset).
                f.UpdateFinger(s, offset); // if we want to change the offset for fingers, we can use UpdateFinger(s, newOffset).
            }
        }

        override public string ToString()
        {
            string s = "";
            s += $"_fingerBones.length: {fingerBones.Length}";
            foreach (FingerBone f in fingerBones)
            {
                s += f.ToString();
                s += "\n";
            }
            return s;
        }

        public class FingerBone
        {
            public string name;
            public Transform transform;
            public Quaternion offset;
            public HandType handType;

            public Dictionary<string, bid> boneMapping_rocketbox = new Dictionary<string, bid>
            {
                { "Finger0", bid.Hand_Thumb1 },
                { "Finger01", bid.Hand_Thumb2 },
                { "Finger02", bid.Hand_Thumb3 },

                { "Finger1", bid.Hand_Index1 },
                { "Finger11", bid.Hand_Index2 },
                { "Finger12", bid.Hand_Index3 },

                { "Finger2", bid.Hand_Middle1 },
                { "Finger21", bid.Hand_Middle2 },
                { "Finger22", bid.Hand_Middle3 },

                { "Finger3", bid.Hand_Ring1 },
                { "Finger31", bid.Hand_Ring2 },
                { "Finger32", bid.Hand_Ring3 },

                { "Finger4", bid.Hand_Pinky1 },
                { "Finger41", bid.Hand_Pinky2 },
                { "Finger42", bid.Hand_Pinky3 },
            };

            public Dictionary<string, bid> boneMapping_mixamo = new Dictionary<string, bid>
            {
                { "Thumb1", bid.Hand_Thumb1 },
                { "Thumb2", bid.Hand_Thumb2 },
                { "Thumb3", bid.Hand_Thumb3 },

                { "Index1", bid.Hand_Index1 },
                { "Index2", bid.Hand_Index2 },
                { "Index3", bid.Hand_Index3 },

                { "Middle1", bid.Hand_Middle1 },
                { "Middle2", bid.Hand_Middle2 },
                { "Middle3", bid.Hand_Middle3 },

                { "Ring1", bid.Hand_Ring1 },
                { "Ring2", bid.Hand_Ring2 },
                { "Ring3", bid.Hand_Ring3 },

                { "Pinky1", bid.Hand_Pinky1 },
                { "Pinky2", bid.Hand_Pinky2 },
                { "Pinky3", bid.Hand_Pinky3 },
            };

            public FingerBone(string n, Transform t, Quaternion o, HandType ht)
            {
                name = n;
                transform = t;
                offset = o;
                handType = ht;
            }

            public void UpdateFinger(OVRSkeleton s)
            {
                if (s == null || s.Bones.Count == 0)
                {
                    return;
                }

                if (VRRig.Instance.avatarType == VRRig.AvatarType.RockectBox)
                {
                    Transform tracking = s.Bones[(int)boneMapping_rocketbox[name]].Transform;
                    SetProperties(transform, tracking);
                }
                else if (VRRig.Instance.avatarType == VRRig.AvatarType.Mixamo)
                {
                    Transform tracking = s.Bones[(int)boneMapping_mixamo[name]].Transform;
                    SetProperties(transform, tracking);
                }

                
            }

            public void UpdateFinger(OVRSkeleton s, Quaternion newOffset)
            {
                offset = newOffset;
                UpdateFinger(s);
            }

            private void SetProperties(Transform d1, Transform d2)
            {
                if (d1 == null || d2 == null)
                {
                    return;
                }

                //d1.position = d2.position; // Uncomment this if you want elastic fingers like alien ;)

                if (VRRig.Instance.curMode == VRRig.MirrorMode.None)
                {
                    d1.transform.rotation = d2.rotation;
                    d1.transform.rotation *= offset;
                }
                else if(VRRig.Instance.curMode == VRRig.MirrorMode.MirrorLeft)
                {
                    //testing mirror left
                    if (handType == HandType.RightHand)
                    {
                        Quaternion destRot = d2.rotation;
                        destRot.x = -destRot.x;
                        destRot.w = -destRot.w;
                        d1.transform.rotation = destRot;
                        Vector3 flipoffset = new Vector3(180.0f, 0.0f, 0.0f); //flip upside down
                        d1.transform.rotation *= Quaternion.Euler(flipoffset);
                        d1.transform.rotation *= offset;

                    }
                    else
                    {
                        d1.transform.rotation = d2.rotation;
                        d1.transform.rotation *= offset;
                    }
                }
                else if (VRRig.Instance.curMode == VRRig.MirrorMode.MirrorRight)
                {
                    //testing mirror right
                    if (handType == HandType.LeftHand)
                    {
                        Quaternion destRot = d2.rotation;
                        destRot.x = -destRot.x;
                        destRot.w = -destRot.w;
                        d1.transform.rotation = destRot;
                        Vector3 flipoffset = new Vector3(180.0f, 0.0f, 0.0f); //flip upside down
                        d1.transform.rotation *= Quaternion.Euler(flipoffset);
                        d1.transform.rotation *= offset;

                    }
                    else
                    {
                        d1.transform.rotation = d2.rotation;
                        d1.transform.rotation *= offset;
                    }
                }


            }

            override public string ToString()
            {
                if (transform == null)
                {
                    return name + ": transform null";
                }

                return $"{name}: Position: {transform.position}; Rotation: {transform.rotation}";
            }
        }
    }
}
