using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatronLoader : MonoBehaviour
{
    public List<Patron> patrons = new List<Patron>();
    public static PatronLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public List<Patron> LoadPatrons()
    {
        return patrons;
    }
}
