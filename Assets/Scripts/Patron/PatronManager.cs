using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PatronManager : NetworkBehaviour
{
    public Transform patronPrefab;

    public Transform container;

    public static PatronManager Instance { get; private set; }

    private List<Patron> patrons = new List<Patron>();
    public List<Patron> availablePatrons = new List<Patron>();
    [SyncVar] List<int> indexes = new List<int>();
    int patronCount = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        patrons = PatronLoader.Instance.LoadPatrons();

        if (isServer)
        {
            GenerateRandom();
        }

        SpawnPatrons(container, patrons);
    }

    [Server]
    private void GenerateRandom()
    {
        int rand;
        while(indexes.Count < patronCount)
        {
            rand = Random.Range(0, patrons.Count - 1);

            if (!indexes.Contains(rand))
            {
                indexes.Add(rand);
            }           
        }
    }


    private void SpawnPatrons(Transform container, List<Patron> patrons)
    {
        for (int i = 0; i < patronCount; i++)
        {
            Patron patron = patrons[indexes[i]];
            patron.Initialize();

            Transform patronObject = Instantiate(patronPrefab, container);
            Vector3 patronPosition = new Vector3(patronObject.position.x + (25 * i), patronObject.position.y, 0);
            patronObject.position = patronPosition;

            PatronDisplay patronDisplay = patronObject.GetComponentInChildren<PatronDisplay>();
            patronDisplay.patron = patron;
            availablePatrons.Add(patron);
        }
    }
    public bool CheckPatronRequirements(Player player)
    {
        List<Patron> patronsToRemove = new List<Patron>();

        foreach (Patron patron in availablePatrons)
        {
            bool requirementsMet = true;

            foreach (var requirement in patron.requirements)
            {
                CardColor color = requirement.Key;
                int requiredAmount = requirement.Value;

                if (player.pasiveCoinsInventory[color] < requiredAmount)
                {
                    requirementsMet = false;
                    break;
                }
            }

            if (requirementsMet)
            {
                player.AddPoints(3);
                patronsToRemove.Add(patron);
                player.cardInventory.AddPatron(patron);

            }
        }

        foreach (Patron patron in patronsToRemove)
        {
            availablePatrons.Remove(patron);
            foreach(PatronDisplay patronDisplay in container.GetComponentsInChildren<PatronDisplay>())
            {
                if(patronDisplay.patron == patron)
                {
                    Destroy(patronDisplay.transform.parent.parent.gameObject);
                }
            }
        }

        return true;
    }
}
