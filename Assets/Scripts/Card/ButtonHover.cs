using UnityEngine;

public class ButtonHover : MonoBehaviour
{
    private GameObject[] children;

    void Start()
    {
        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        DisableChildren();

    }

    public void EnableChildren()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(true);
        }
    }

    public void DisableChildren()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(false);
        }
    }
}
