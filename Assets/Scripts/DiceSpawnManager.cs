using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpawnManager : NetworkBehaviour
{
    [SerializeField]
    Transform Transform;

    [SerializeField]
    GameObject Dice_prefab;

    Queue<GameObject> DiceList;
    int diceCount = 10;

    public static DiceSpawnManager instance;

    private DiceSpawnManager()
    {
        DiceList = new Queue<GameObject>();
    }

    private void Awake()
    {
        instance = new DiceSpawnManager();
    }
    float timeCooltimes = 5.0f;
    float time = 5.0f;

    public Queue<GameObject> GetDiceList()
    {
        return DiceList;
    }

    [Server]
    public void OnEnableToServer()
    {
        for (int i = 0; i < diceCount; i++)
        {
            GameObject gameObject = Instantiate(Dice_prefab, this.gameObject.transform);
            gameObject.gameObject.SetActive(false);
            NetworkServer.Spawn(gameObject);
            Enqueues(gameObject);
        }
    }

    // Update is called once per frame
    
    void Update()
    {
        OnUpdateInServer();
    }

    [Command]
    void OnUpdateInServer()
    {
        if (isLocalPlayer == true)
            return;

        if (time > 0)
            time -= Time.deltaTime;
        else
        {
            if (DiceList.Count > 0)
            {
                Dequeues();

                time = timeCooltimes;
            }
        }
    }

    [Command]
    private void Dequeues()
    {
        GameObject gameObject = DiceList.Dequeue();
        gameObject.transform.position = Transform.position;
        gameObject.SetActive(true);
    }

    [Server]
    public void Enqueues(GameObject gameObject)
    {
        DiceList.Enqueue(gameObject);
    }
}
