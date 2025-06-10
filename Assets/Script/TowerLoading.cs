using UnityEngine;
using System.Collections;

public class TowerLoading : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameObject objectToActivate2;
    public GameObject objectToDeactivate;
    public float delay = 3f;

    void Start()
    {
        StartCoroutine(SwitchObjectsAfterDelay());
    }

    IEnumerator SwitchObjectsAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(false);

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        if (objectToActivate2 != null)
            objectToActivate2.SetActive(true);

        GameManager.Instance.StartGame();
    }
}
