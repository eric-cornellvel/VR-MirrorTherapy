using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes one GameObject's transform to follow another GameObject's transform.
/// </summary>
public class TransformFollow : ObjectFollow
{
    /// <summary>
    /// The moment at which to follow.
    /// </summary>
    public enum FollowMoment
    {
        /// <summary>
        /// Follow in the FixedUpdate method.
        /// </summary>
        OnFixedUpdate,
        /// <summary>
        /// Follow in the Update method.
        /// </summary>
        OnUpdate,
        /// <summary>
        /// Follow in the LateUpdate method.
        /// </summary>
        OnLateUpdate,
        /// <summary>
        /// Follow in the OnPreRender method. (This script doesn't have to be attached to a camera).
        /// </summary>
        OnPreRender,
        /// <summary>
        /// Follow in the OnPreCull method. (This script doesn't have to be attached to a camera).
        /// </summary>
        OnPreCull
    }

    [Header("Follow Settings")]

    [Tooltip("The moment at which to follow.")]
    [SerializeField]
    private FollowMoment _moment = FollowMoment.OnPreRender;
    public FollowMoment moment
    {
        get
        {
            return _moment;
        }
        set
        {
            if (_moment == value)
            {
                return;
            }

            if (isActiveAndEnabled)
            {
                if (_moment == FollowMoment.OnPreRender && value != FollowMoment.OnPreRender)
                {
                    Camera.onPreRender -= OnCamPreRender;
                }
                if (_moment != FollowMoment.OnPreRender && value == FollowMoment.OnPreRender)
                {
                    Camera.onPreRender += OnCamPreRender;
                }

                if (_moment == FollowMoment.OnPreCull && value != FollowMoment.OnPreCull)
                {
                    Camera.onPreCull -= OnCamPreCull;
                }
                if (_moment != FollowMoment.OnPreCull && value == FollowMoment.OnPreCull)
                {
                    Camera.onPreCull += OnCamPreCull;
                }
            }

            _moment = value;
        }
    }

    public Transform transformToFollow;
    public Transform transformToChange;

    public bool isLocal;

    /// <summary>
    /// Follow `gameObjectToFollow` using the current settings.
    /// </summary>
    public override void Follow()
    {
        CacheTransforms();
        base.Follow();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (moment == FollowMoment.OnPreRender)
        {
            Camera.onPreRender += OnCamPreRender;
        }

        if (moment == FollowMoment.OnPreCull)
        {
            Camera.onPreCull += OnCamPreCull;
        }
    }

    protected virtual void OnDisable()
    {
        transformToFollow = null;
        transformToChange = null;
        Camera.onPreRender -= OnCamPreRender;
        Camera.onPreCull -= OnCamPreCull;
    }

    protected void FixedUpdate()
    {
        if (moment == FollowMoment.OnFixedUpdate)
        {
            Follow();
        }
    }

    protected void Update()
    {
        if (moment == FollowMoment.OnUpdate)
        {
            Follow();
        }
    }

    protected virtual void LateUpdate()
    {
        if (moment == FollowMoment.OnLateUpdate)
        {
            Follow();
        }
    }

    // The following `OnCam*` methods need to have the `Cam` addition to prevent a name clash with the `MonoBehaviour.On*` methods since those are used when this script is attached to a camera.
    protected virtual void OnCamPreRender(Camera cam)
    {
        if (cam.gameObject.transform == Utils.getHeadsetCamera().gameObject.transform)
        {
            Follow();
        }
    }

    protected virtual void OnCamPreCull(Camera cam)
    {
        if (cam.gameObject.transform == Utils.getHeadsetCamera().gameObject.transform)
        {
            Follow();
        }
    }

    protected override Vector3 GetPositionToFollow()
    {
        if (isLocal)
        {
            return transformToFollow.localPosition;
        }
        else
        {
            return transformToFollow.position;
        }
        
    }

    protected override void SetPositionOnGameObject(Vector3 newPosition)
    {
        transformToChange.position = newPosition;
    }

    protected override Quaternion GetRotationToFollow()
    {
        if (isLocal)
        {
            return transformToFollow.localRotation;
        }
        else
        {
            return transformToFollow.rotation;
        }    
    }

    protected override void SetRotationOnGameObject(Quaternion newRotation)
    {
        transformToChange.rotation = newRotation;
    }

    public virtual void CacheTransforms()
    {
        if (gameObjectToFollow == null || gameObjectToChange == null
            || (transformToFollow != null && transformToChange != null))
        {
            return;
        }

        transformToFollow = gameObjectToFollow.transform;
        transformToChange = gameObjectToChange.transform;
    }

}
