using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public class IKSolver
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Transform rigTargetFollowObj;
    public Vector3 anchorPositionOffset;
    public Vector3 anchorRotationOffset;


    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(anchorPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(anchorRotationOffset);

        /*
        if (rigTargetFollowObj != null)
        {
            rigTarget.position = rigTargetFollowObj.position;
            rigTarget.rotation = rigTargetFollowObj.rotation;
        }
        else
        {
            rigTarget.position = vrTarget.position;
            rigTarget.rotation = vrTarget.rotation;
        }
        */
    }
}

public class VRRig : MonoBehaviour
{
    public GameObject cameraRig;
    public OVRHand OVRHandScript;
    public IKSolver headIKSolver;
    public IKSolver leftHandIKSolver;
    public IKSolver rightHandIKSolver;
    public GameObject leftArmIKTarget;
    public GameObject rightArmIKTarget;
    public GameObject maleAvatar;
    public GameObject femaleAvatar;


    private Transform headConstraint;
    private Vector3 headBodyOffset;
    private Rig VRConstriant;

    public Transform root;
    public Transform pelvis;
    public Transform spine;
    public Transform chest;
    public Transform neck;
    public Transform head;
    public Transform leftShoulder;
    public Transform leftUpperArm;
    public Transform leftForearm;
    public Transform leftHand;
    public Transform rightShoulder;
    public Transform rightUpperArm;
    public Transform rightForearm;
    public Transform rightHand;
    public Transform leftThigh;
    public Transform leftCalf;
    public Transform leftFoot;
    public Transform leftToes;
    public Transform rightThigh;
    public Transform rightCalf;
    public Transform rightFoot;
    public Transform rightToes;


    public bool isCalibrated = false;
    public float playerHeightHmd = 1.70f;
    private float avatarHeight = 1.70f;

    public float armLengthMlp = 1.0f;
    public AnimationCurve stretchCurve = new AnimationCurve();

    //trciky way to start moving the camera rig
    private bool cameraRigMoved = false;
    private Vector3 oldHeadPos;
    private float sizeF = 1.0f;

    public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Controller controller = OVRInput.Controller.All;

    public static VRRig Instance; //singleton variable
    public enum MirrorMode { None, MirrorLeft, MirrorRight };
    public enum AvatarType { RockectBox, Mixamo };
    public enum AvatarGender { Male, Female };
    public MirrorMode curMode = MirrorMode.None;
    public AvatarType avatarType = AvatarType.Mixamo;
    public AvatarGender avatarGender = AvatarGender.Male;

    private void Awake()
    {
        if (Instance == null) Instance = this; //store singleton
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null; //delete singelton
    }
    
    public void changeMirror(MirrorMode mode)
    {
        if(mode == MirrorMode.None) //none
        {
            leftHandIKSolver.vrTarget = Utils.getLeftController().transform;
            leftHandIKSolver.rigTarget = Utils.getLeftController().transform.Find("LeftHandIKTarget");

            rightHandIKSolver.vrTarget = Utils.getRightController().transform;
            rightHandIKSolver.rigTarget = Utils.getRightController().transform.Find("RightHandIKTarget");

        }
        else if (mode == MirrorMode.MirrorLeft) //mirror left
        {
            leftHandIKSolver.vrTarget = Utils.getLeftController().transform;
            leftHandIKSolver.rigTarget = Utils.getLeftController().transform.Find("LeftHandIKTarget");

            rightHandIKSolver.vrTarget = Utils.getCameraRig().transform.Find("TrackingSpace/MirrorLeft");
            rightHandIKSolver.rigTarget = Utils.getCameraRig().transform.Find("TrackingSpace/MirrorLeft/MirrorLeftIKTarget");
        }
        else if(mode == MirrorMode.MirrorRight) // mirror right
        {
            leftHandIKSolver.vrTarget = Utils.getCameraRig().transform.Find("TrackingSpace/MirrorRight");
            leftHandIKSolver.rigTarget = Utils.getCameraRig().transform.Find("TrackingSpace/MirrorRight/MirrorRightIKTarget");

            rightHandIKSolver.vrTarget = Utils.getRightController().transform;
            rightHandIKSolver.rigTarget = Utils.getRightController().transform.Find("RightHandIKTarget");
        }

        curMode = mode;
        leftArmIKTarget.GetComponent<TransformFollow>().gameObjectToFollow = leftHandIKSolver.rigTarget.gameObject;
        leftArmIKTarget.GetComponent<TransformFollow>().transformToFollow = leftHandIKSolver.rigTarget;
        rightArmIKTarget.GetComponent<TransformFollow>().gameObjectToFollow = rightHandIKSolver.rigTarget.gameObject;
        rightArmIKTarget.GetComponent<TransformFollow>().transformToFollow = rightHandIKSolver.rigTarget;
    }

    /// <summary>
    /// Calibrates only the avatar scale.
    /// </summary>
    private void CalibrateScale(float scaleMlp = 1f)
    {
        float sizeF = (headIKSolver.rigTargetFollowObj.position.y - root.position.y) / (head.position.y - root.position.y);
        root.localScale *= sizeF * scaleMlp;
    }

    /// <summary>
    /// Calibrates head IK target to specified anchor position and rotation offset independent of avatar bone orientations.
    /// </summary>
    public void CalibrateHead()
    {
        if (headIKSolver.rigTargetFollowObj == null) headIKSolver.rigTargetFollowObj = new GameObject("Head IK Target").transform;

        Vector3 forward = Quaternion.Inverse(head.rotation) * root.forward;
        Vector3 up = Quaternion.Inverse(head.rotation) * root.up;
        Quaternion headSpace = Quaternion.LookRotation(forward, up);

        Vector3 anchorPos = head.position + head.rotation * headSpace * headIKSolver.anchorPositionOffset;
        Quaternion anchorRot = head.rotation * headSpace * Quaternion.Euler(headIKSolver.anchorRotationOffset);
        Quaternion anchorRotInverse = Quaternion.Inverse(anchorRot);

        headIKSolver.rigTargetFollowObj.parent = headIKSolver.vrTarget;
        headIKSolver.rigTargetFollowObj.localPosition = anchorRotInverse * (head.position - anchorPos);
        headIKSolver.rigTargetFollowObj.localRotation = anchorRotInverse * head.rotation;
    }

    /// <summary>
    /// Calibrates hand IK targets to specified anchor position and rotation offsets independent of avatar bone orientations.
    /// </summary>
    public void CalibrateHands()
    {
        if (leftHandIKSolver.rigTargetFollowObj == null) leftHandIKSolver.rigTargetFollowObj = new GameObject("Left Hand IK Target").transform;
        if (rightHandIKSolver.rigTargetFollowObj == null) rightHandIKSolver.rigTargetFollowObj = new GameObject("Right Hand IK Target").transform;

        CalibrateHand(leftHand, leftForearm, leftHandIKSolver.rigTargetFollowObj, leftHandIKSolver.vrTarget, leftHandIKSolver.anchorPositionOffset, leftHandIKSolver.anchorRotationOffset, true);
        CalibrateHand(rightHand, rightForearm, rightHandIKSolver.rigTargetFollowObj, rightHandIKSolver.vrTarget, rightHandIKSolver.anchorPositionOffset, rightHandIKSolver.anchorRotationOffset, false);
    }

    private void CalibrateHand(Transform hand, Transform forearm, Transform target, Transform anchor, Vector3 positionOffset, Vector3 rotationOffset, bool isLeft)
    {
        if (isLeft)
        {
            positionOffset.x = -positionOffset.x;
            rotationOffset.y = -rotationOffset.y;
            rotationOffset.z = -rotationOffset.z;
        }

        Vector3 forward = GuessWristToPalmAxis(hand, forearm);
        Vector3 up = GuessPalmToThumbAxis(hand, forearm);
        Quaternion handSpace = Quaternion.LookRotation(forward, up);
        Vector3 anchorPos = hand.position + hand.rotation * handSpace * positionOffset;
        Quaternion anchorRot = hand.rotation * handSpace * Quaternion.Euler(rotationOffset);
        Quaternion anchorRotInverse = Quaternion.Inverse(anchorRot);

        target.parent = anchor;
        target.localPosition = anchorRotInverse * (hand.position - anchorPos);
        target.localRotation = anchorRotInverse * hand.rotation;
    }

    public Vector3 GuessWristToPalmAxis(Transform hand, Transform forearm)
    {
        Vector3 toForearm = forearm.position - hand.position;
        Vector3 axis = Utils.ToVector3(Utils.GetAxisToDirection(hand, toForearm));
        if (Vector3.Dot(toForearm, hand.rotation * axis) > 0f) axis = -axis;
        return axis;
    }

    public Vector3 GuessPalmToThumbAxis(Transform hand, Transform forearm)
    {
        if (hand.childCount == 0)
        {
            Debug.LogWarning("Hand " + hand.name + " does not have any fingers, The script can not guess the hand bone's orientation. Please assign 'Wrist To Palm Axis' and 'Palm To Thumb Axis' manually for both arms in settings.", hand);
            return Vector3.zero;
        }

        float closestSqrMag = Mathf.Infinity;
        int thumbIndex = 0;

        for (int i = 0; i < hand.childCount; i++)
        {
            float sqrMag = Vector3.SqrMagnitude(hand.GetChild(i).position - hand.position);
            if (sqrMag < closestSqrMag)
            {
                closestSqrMag = sqrMag;
                thumbIndex = i;
            }
        }

        Vector3 handNormal = Vector3.Cross(hand.position - forearm.position, hand.GetChild(thumbIndex).position - hand.position);
        Vector3 toThumb = Vector3.Cross(handNormal, hand.position - forearm.position);
        Vector3 axis = Utils.ToVector3(Utils.GetAxisToDirection(hand, toThumb));
        if (Vector3.Dot(toThumb, hand.rotation * axis) < 0f) axis = -axis;
        return axis;
    }



    IEnumerator calibrateIKAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        CalibrateHead();
        CalibrateHands();
        CalibrateScale();
        
        //Adjust foot position after scale
        float floorOffset = leftToes.position.y;
        root.position = new Vector3(root.position.x, root.position.y - floorOffset, root.position.z);

        // Adjusting arm length   
        float avatarArmLength = (leftUpperArm.position - leftHand.position).magnitude;
        float playerArmLength = (leftUpperArm.position - leftHandIKSolver.rigTargetFollowObj.position).magnitude;
        armLengthMlp = playerArmLength / avatarArmLength;

        headBodyOffset = transform.position - headConstraint.position;
        isCalibrated = true;
    }

    IEnumerator calibrateCameraRigAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        //move the camerarig to align with scaled avatar
        Vector3 toNewHead = head.position - headConstraint.position; //should get the new head position
        Vector3 deltaPositionOffset = headIKSolver.anchorPositionOffset * (sizeF - 1.0f);
        Vector3 displacement = toNewHead - deltaPositionOffset;
        cameraRig.transform.position += displacement;
        headIKSolver.anchorPositionOffset *= sizeF;

        isCalibrated = true;
    }


    private void calibrateCameraRig()
    {
        //move the camerarig to align with scaled avatar
        Vector3 toNewHead = head.position - headConstraint.position; //should get the new head position
        Vector3 deltaPositionOffset = headIKSolver.anchorPositionOffset * (sizeF - 1.0f);
        Vector3 displacement = toNewHead - deltaPositionOffset;
        cameraRig.transform.position += displacement;
        headIKSolver.anchorPositionOffset *= sizeF;

        isCalibrated = true;
    }

    private void calibrateRootPosition()
    {
        //move the camerarig to align with scaled avatar
        Vector3 toNewHead = head.position - headConstraint.position; //should get the new head position
        Vector3 deltaPositionOffset = headIKSolver.anchorPositionOffset * (sizeF - 1.0f);
        Vector3 displacement = toNewHead - deltaPositionOffset;
        root.transform.position -= displacement;
        headIKSolver.anchorPositionOffset *= sizeF;

        isCalibrated = true;
    }

    public void Calibration()
    {
        //CalibrateHead();
        //CalibrateHands();
        //CalibrateScale();

        oldHeadPos = head.position;
        sizeF = (headIKSolver.rigTarget.position.y - root.position.y) / (head.position.y - root.position.y);
        root.localScale *= sizeF;

        //TODO- test whether need to move the ovrcamerarig
        //float floorOffset = leftToes.position.y;
        //root.position = new Vector3(transform.position.x, transform.position.y - floorOffset, transform.position.z);

        // Adjusting arm length   
        float avatarArmLength = (leftUpperArm.position - leftHand.position).magnitude;
        //float playerArmLength = (leftUpperArm.position - leftHandIKSolver.rigTargetFollowObj.position).magnitude;
        float playerArmLength = (leftUpperArm.position - leftHandIKSolver.rigTarget.position).magnitude;
        armLengthMlp = playerArmLength / avatarArmLength;

        //headBodyOffset = root.position - headConstraint.position;
        headBodyOffset = root.position - head.position;

        //testing
        VRConstriant.weight = 1.0f;

        //save rest finger pose
        if (avatarGender == VRRig.AvatarGender.Male)
        {
            maleAvatar.GetComponent<MixamoOVRHandTracking>().SaveFingerPose();
        }
        else if (avatarGender == VRRig.AvatarGender.Female)
        {
            femaleAvatar.GetComponent<MixamoOVRHandTracking>().SaveFingerPose();
        }

        //Hide Controller
        InputManager.Instance.HideControlelr();

        //TODO- fix the issue on the Quest. temp solution: disable follow head
        StartCoroutine(calibrateCameraRigAfterTime(0.1f));

    }

    //need to called in the LateUpdate (after Animation Rigging)
    private void Stretching()
    {
        // Adjusting arm length
        /*
        float armLength = (avatarUpperArm.position - avatarHand.position).magnitude;
        Vector3 elbowAdd = Vector3.zero;
        Vector3 handAdd = Vector3.zero;

        if (armLengthMlp != 1f)
        {
            armLength *= armLengthMlp;
            elbowAdd = (avatarForearm.position - avatarUpperArm.position) * (armLengthMlp - 1f);
            handAdd = (avatarHand.position - avatarForearm.position) * (armLengthMlp - 1f);
            avatarForearm.position += elbowAdd;
            avatarHand.position += elbowAdd + handAdd;
        }

        // Stretching
        float distanceToTarget = Vector3.Distance(avatarUpperArm.position, leftHand.rigTarget.position);
        float stretchF = distanceToTarget / armLength;

        float m = stretchCurve.Evaluate(stretchF);
        m *= 1.0f; //position weight

        elbowAdd = (avatarForearm.position - avatarUpperArm.position) * m;
        handAdd = (avatarHand.position - avatarForearm.position) * m;

        avatarForearm.position += elbowAdd;
        avatarHand.position += elbowAdd + handAdd;
        */
    }


    public void LoadAvatarBones(AvatarGender gender)
    {
        avatarGender = gender;
        Animator animator = null;
        if (avatarGender == AvatarGender.Male)
        {
            if (!maleAvatar.activeSelf)
            {
                maleAvatar.SetActive(true);
            }
            if (femaleAvatar.activeSelf)
            {
                femaleAvatar.SetActive(false);
            }
            animator = maleAvatar.GetComponentInChildren<Animator>();
            root = maleAvatar.transform;
            VRConstriant = maleAvatar.transform.Find("VRConstraints").GetComponent<Rig>();
            headConstraint = maleAvatar.transform.Find("VRConstraints/HeadConstraint");
        }else if (avatarGender == AvatarGender.Female)
        {
            if (!femaleAvatar.activeSelf)
            {
                femaleAvatar.SetActive(true);
            }
            if (maleAvatar.activeSelf)
            {
                maleAvatar.SetActive(false);
            }
            animator = femaleAvatar.GetComponentInChildren<Animator>();
            root = femaleAvatar.transform;
            VRConstriant = femaleAvatar.transform.Find("VRConstraints").GetComponent<Rig>();
            headConstraint = femaleAvatar.transform.Find("VRConstraints/HeadConstraint");
        }

        
        if (animator == null || !animator.isHuman)
        {
            Debug.LogWarning("Needs a Humanoid Animator to auto-detect biped references. Please assign references manually.");
            return;
        }
        
        pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        leftForearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        rightForearm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        leftThigh = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        leftCalf = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        leftToes = animator.GetBoneTransform(HumanBodyBones.LeftToes);
        rightThigh = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        rightCalf = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        rightToes = animator.GetBoneTransform(HumanBodyBones.RightToes);

        headIKSolver.rigTarget = headConstraint;
        //headBodyOffset = root.position - headConstraint.position;
        headBodyOffset = root.position - head.position;

    }

    // Start is called before the first frame update
    void Start()
    {
        //TODO-support male and female avatar
        LoadAvatarBones(avatarGender);

        //StartCoroutine(calibrateIKAfterTime(10));
    }

    private void Update()
    {
        if (OVRHandScript != null)
        {
            /*
            if (OVRHandScript.GetFingerIsPinching(OVRHand.HandFinger.Index) &&
            OVRHandScript.GetFingerIsPinching(OVRHand.HandFinger.Thumb))
            {
                if (!isCalibrated)
                {

                    Debug.Log("Start Calibration");
                    Calibration();

                    //TODO-somehow Quest has error
                    StartCoroutine(calibrateCameraRigAfterTime(0.1f));
                }
            }
            */

            /*
            if (OVRHandScript.GetFingerIsPinching(OVRHand.HandFinger.Middle) &&
           OVRHandScript.GetFingerIsPinching(OVRHand.HandFinger.Thumb))
            {
                //TODO-quest might need to move avatar instead of camera rig
                calibrateRootPosition();
            }
            */

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Calibration();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            calibrateCameraRig();
        }
    }

    //testing to run in Update or LateUpdate
    private void LateUpdate()
    {
        /*
        if(head.position != oldHeadPos && !cameraRigMoved && isCalibrated)
        {
            Debug.Log("test:" + head.position + ", " + oldHeadPos);
            calibrateCameraRig();
            cameraRigMoved = true;
        }
        */

        Vector3 movement = headConstraint.position + headBodyOffset;
        if (isCalibrated)
        {
            movement = new Vector3(movement.x, transform.position.y, movement.z);
            //TODO- fix quest error
            if(avatarGender == AvatarGender.Male)
            {
                maleAvatar.transform.position = movement;
            }else if(avatarGender == AvatarGender.Female)
            {
                femaleAvatar.transform.position = movement;
            }
            //Stretching();
        }
        else
        {
            //transform.position = movement;
        }
        
        //transform.forward = Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized;

        //spine.forward = Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized;

        headIKSolver.Map();
        //leftHandIKSolver.Map();
        //rightHandIKSolver.Map();

    }
}
