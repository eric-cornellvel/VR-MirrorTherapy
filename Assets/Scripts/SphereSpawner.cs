using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{

    public Transform holdPosAnchorMale;
    public Transform holdPosAnchorFemale;
    public GameObject smallSphere;
    public GameObject medSphere;
    public GameObject largeSphere;
    private int[] sizeArray = {0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2};
	private int[] smallPosArray = { 0, 1, 2, 3, 4, 5 };
	private int[] mediumPosArray = { 0, 1, 2, 3, 4, 5 };
	private int[] largePosArray = { 0, 1, 2, 3, 4, 5 };
    public int sphereIndex = 0;
	private int smallIndex = 0;
	private int mediumIndex = 0;
	private int largeIndex = 0;



	private void Shuffle(int[] a)
	{
		// Loops through array
		for (int i = a.Length - 1; i > 0; i--)
		{
			// Randomize a number between 0 and i (so that the range decreases each time)
			int rnd = Random.Range(0, i);

			// Save the value of the current i, otherwise it'll overright when we swap the values
			int temp = a[i];

			// Swap the new and old values
			a[i] = a[rnd];
			a[rnd] = temp;
		}

        // Print
        string str = "array: ";
        for (int i = 0; i < a.Length; i++)
        {
            str += a[i].ToString() + ", ";
           
        }
        Debug.Log(str);
    }

    Vector3 get_pos(int pos)
    {
        float fowardDis = 1.0f;
        Transform holdPosAnchor = null;
        if (VRRig.Instance.avatarGender == VRRig.AvatarGender.Male)
        {
            holdPosAnchor = holdPosAnchorMale;
        }else if (VRRig.Instance.avatarGender == VRRig.AvatarGender.Female)
        {
            holdPosAnchor = holdPosAnchorFemale;
        }

        Vector3 forward = holdPosAnchor.forward;
        if (pos == 0)
        {
            Vector3 ret = holdPosAnchor.position + forward * fowardDis;
            ret.y -= 0.1f;
            return ret;
        }
        else if (pos == 1)
        {
            Vector3 ret = holdPosAnchor.position + forward * fowardDis;
            return ret;
        }
        else if (pos == 2)
        {
            Vector3 ret = holdPosAnchor.position + forward * fowardDis;
            ret.y += 0.2f;
            return ret;
        }
        else if (pos == 3)
        {
            Vector3 ret = holdPosAnchor.position + forward * fowardDis;
            ret.y += 0.3f;
            return ret;
        }
        else if (pos == 4)
        {
            Vector3 ret = holdPosAnchor.position + forward * fowardDis;
            ret.y += 0.4f;
            return ret;
        }
        else
        {
            Vector3 ret = holdPosAnchor.position + forward * fowardDis;
            ret.y += 0.5f;
            return ret;
        }
    }

    IEnumerator spawnSphereAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SpawnSphere();
    }

    public void SpawnSphereAfter(float time)
    {
        if (sphereIndex <= sizeArray.Length)
        {
            StartCoroutine(spawnSphereAfterTime(time));
        }
        else
        {
            //stop recording
            InputManager.Instance.StopRecording();

        }
    }

    public void SpawnSphere()
    {
        if (sphereIndex <= sizeArray.Length)
        {
            int size = sizeArray[sphereIndex];
            Vector3 pos;
            if (size == 0)
            {
                pos = get_pos(smallPosArray[smallIndex]);
                Instantiate(smallSphere, pos, Quaternion.identity);
                smallIndex++;
            }
            else if (size == 1)
            {
                pos = get_pos(mediumPosArray[mediumIndex]);
                Instantiate(medSphere, pos, Quaternion.identity);
                mediumIndex++;
            }
            else if (size == 2)
            {
                pos = get_pos(largePosArray[largeIndex]);
                Instantiate(largeSphere, pos, Quaternion.identity);
                largeIndex++;
            }
            sphereIndex++;
        }
        else
        {
            //stop recording
            //stop recording
            InputManager.Instance.StopRecording();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
		Shuffle(sizeArray);
		Shuffle(smallPosArray);
		Shuffle(mediumPosArray);
		Shuffle(largePosArray);

	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sphereIndex < sizeArray.Length)
            {
                SpawnSphere();             
            }
        }
    }
}
