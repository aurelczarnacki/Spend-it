using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerQueueManager : NetworkBehaviour
{
    public List<PlayerObjectController> playerList = new List<PlayerObjectController>();
    public Transform playerPrefab;
    public GameObject blockade;
    public GameObject endScreen;
    public RectTransform playerEndInfoPrefab;
    public RectTransform playerEndInfoContent;
    public PlayerObjectController currentPlayer { get; private set; }
    public TextMeshProUGUI informationBar;
    public int currentPlayerIndex = -1;
    public PlayerObjectController playerWithHighestPoints { get; private set; }

    public static PlayerQueueManager Instance { get; private set; }


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

    public void setPlayerWithHighiestPoints()
    {
        playerWithHighestPoints = playerList.OrderByDescending(player => player.player.points).FirstOrDefault();
    }

    public void EndTurn()
    {
        CmdEndTurn();
    }

    [Command(requiresAuthority = false)]
    void CmdEndTurn()
    {
/*        if (currentPlayer.isLocalPlayer)
        {*/
            RpcPlayerEndedTurn();
/*        }*/
    }

    [ClientRpc]
    void RpcPlayerEndedTurn()
    {
        setPlayerWithHighiestPoints();

        PatronManager.Instance.CheckPatronRequirements(currentPlayer.player);
        if (playerWithHighestPoints.player.points >= 15 && currentPlayerIndex == playerList.Count - 1)
        {
            EndGame();
        }
        else
        {
            blockade.SetActive(true);
            NextPlayer();
        }
    }
    private void Start()
    {
        List<PlayerObjectController> players = FindObjectsOfType<PlayerObjectController>().ToList();

        playerList = new List<PlayerObjectController>(players);
        PlayerObjectController owner = players.FirstOrDefault(obj => obj.isOwned);
        players.Remove(owner);
        players.Insert(0, owner);
        foreach (PlayerObjectController player in players)
        {
            Transform playerObject = Instantiate(playerPrefab);
            switch (players.IndexOf(player))
            {
                case 0:
                    playerObject.transform.position = new Vector3(0, -150, 0);
                    break;
                case 1:
                    playerObject.transform.position = new Vector3(0, 150, 0);
                    playerObject.transform.Rotate(0f, 0f, 180f);
                    break;
                case 2:
                    playerObject.transform.position = new Vector3(-150, 0, 0);
                    playerObject.transform.Rotate(0f, 0f, -90f);
                    break;
                case 3:
                    playerObject.transform.position = new Vector3(150, 0, 0);
                    playerObject.transform.Rotate(0f, 0f, 90f);
                    break;
            }
            Player playerPlayer = playerObject.GetComponent<Player>();
            player.player = playerPlayer;
        }
        if (playerList.Count > 0)
        {
            currentPlayerIndex = -1;
            NextPlayer();
        }
    }

    private void NextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerList.Count)
        {
            currentPlayerIndex = 0;
        }
        currentPlayer = playerList[currentPlayerIndex];
        informationBar.text = "Tura gracza: " + currentPlayer.PlayerName;
        if (currentPlayer.isLocalPlayer)
        {
            blockade.SetActive(false);
        }
        StartCoroutine(AnimateText());
    }


    // Zakoñcz grê
    private void EndGame()
    {
        endScreen.SetActive(true);
        foreach (PlayerObjectController player in playerList.OrderByDescending(player => player.player.points))
        {

            RectTransform playerEndInfo = Instantiate(playerEndInfoPrefab, playerEndInfoContent);
            var playerEndInfoComponent = playerEndInfo.GetComponent<PlayerEndInfo>();
            playerEndInfoComponent.playerName = player.PlayerName;
            playerEndInfoComponent.points = player.player.points.ToString();

        }
        informationBar.text = "Gra zakoñczona. Wygra³ " + playerWithHighestPoints.PlayerName;
        StartCoroutine(AnimateText());
        playerList.Clear();
        currentPlayer = null;
    }

    private IEnumerator AnimateText()
    {
        informationBar.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        informationBar.gameObject.SetActive(false);
    }


}