using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3[] offsetPos = new Vector3[9]; //local position for each match number

    private GameObject[] matchs = new GameObject[32]; //array form maximum matchs numbers
    private GameObject[] mNumber = new GameObject[4]; //spawn points for match assembled number
    private GameObject[] mSymbols = new GameObject[2]; //spawn points for match assembled symbols

    private int numberOne, numberTwo, numberThree, numberFour;

    private string[] signs = new string[3];
    private string[] symbols = new string[2];
    private int result;

    int[] testingNo;
    string[] testingSymbols;

    private List<GameObject> matchToMove = new List<GameObject>();
    private List<GameObject> emptySpace = new List<GameObject>();
    private List<GameObject> noToSwap = new List<GameObject>();

    private GameObject[] checkNum = new GameObject[4];
    private GameObject[] checkSign = new GameObject[2];

    private GameObject[] tempNum = new GameObject[4];
    private GameObject[] tempS = new GameObject[2];

    private GameObject[] mRecord = new GameObject[4];
    private GameObject[] sRecord = new GameObject[2];
    private int mtoM, eS, canSwap;
    private bool sigleDig = false, isCorrect = true;
    
    private int _corrected;
    public int Corrected { get { return _corrected; } set { _corrected = value; } }
    private int _attempt;
    public int Attempt { get { return _attempt; } set { _attempt = value; } }

    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

        // Start is called before the first frame update
        void Start()
    {
        //assign offset for local positions
        //offset for numbers
        
        offsetPos[0] = new Vector3(0f, 5.6f, 0f);
        offsetPos[1] = new Vector3(2.7f, 2.7f, 0f);
        offsetPos[2] = new Vector3(2.7f, -2.7f, 0f);
        offsetPos[3] = new Vector3(0f, -5.6f, 0f);
        offsetPos[4] = new Vector3(-2.7f, -2.7f, 0f);
        offsetPos[5] = new Vector3(-2.7f, 2.7f, 0f);
        offsetPos[6] = new Vector3(0f, 0f, 0f);

        //offset for signs
        offsetPos[7] = new Vector3(0f, 1f, 0f);
        offsetPos[8] = new Vector3(0f, -1f, 0f);

        testingNo = new int[mNumber.Length];
        testingSymbols = new string[mSymbols.Length];

        signs[0] = "+";
        signs[1] = "-";
        signs[2] = "=";

        symbols[1] = signs[2];

        CreateGame();

        _corrected = 0;
        _attempt = 1;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Replay();
    }

    private IEnumerator CreatingGame()
    {
        FindObjectOfType<UpdateUI>().preparePanel.SetActive(true);

        for (int i = 0; i < mNumber.Length; i++)
        {
            if (mNumber[i] != null)
                Destroy(mNumber[i]);

            if (mRecord[i] != null)
                Destroy(mRecord[i]);
        }

        for(int i =0; i < mSymbols.Length; i++)
        {
            if (mSymbols[i] != null)
                Destroy(mSymbols[i]);

            if (mRecord[i] != null)
                Destroy(mRecord[i]);
        }
        
        CreateTargetPosition();
        PreMatchPos();
        yield return new WaitForSeconds(0.5f);
        CreatingList(mNumber, mSymbols);
       
        if (mtoM != 0 && eS != 0 && canSwap != 0)
        {
            int i = Random.Range(0, 2);
            if (i == 0)
                SwapEmpty();
            else
                SwapMatch();
        }

        else if (canSwap != 0 && mtoM == 0 || eS ==0)   
            SwapMatch();

        Record();
        FindObjectOfType<UpdateUI>().preparePanel.SetActive(false);
    }

    void CreateTargetPosition()
    {
        mNumber[0] = new GameObject("Number 1");
        mNumber[0].transform.position = new Vector3(-20f, 0f, 0f);
        mNumber[1] = new GameObject("Number 2");
        mNumber[1].transform.position = new Vector3(-4f, 0f, 0f);
        mNumber[2] = new GameObject("Number 3");
        mNumber[2].transform.position = new Vector3(12f, 0f, 0f);
        mNumber[3] = new GameObject("Number 4");
        mNumber[3].transform.position = new Vector3(20f, 0f, 0f);

        mSymbols[0] = new GameObject("Sign 1");
        mSymbols[0].transform.position = new Vector3(-12f, 0f, 0f);
        mSymbols[1] = new GameObject("Sign 2");
        mSymbols[1].transform.position = new Vector3(4f, 0f, 0f);
    }

    public void CreateGame()
    {
        Time.timeScale = 1;
        StartCoroutine("CreatingGame");
        _attempt = 1;

        if (isCorrect == true)
            _corrected++;
    }

    public void PreMatchPos()  //creating correct equation for later modification.
    {
        int one = Random.Range(0, 10);
        int two = Random.Range(1, 10);
        if (one == two)
            two = Random.Range(0, 10);

        int i = Random.Range(0, 2);
        if (one == 8 && two == 4 && signs[i] == "-")
            two = Random.Range(5, 10);       

        Calculation(signs[i], one, two);

        if(result >= 10)
        {
            sigleDig = false;
            numberThree = 1;
            numberFour = result-10;
            mNumber[3].SetActive(true);
        }
        else
        {
            sigleDig = true;
            numberThree = result;
            mNumber[3].SetActive(false);
        }

        symbols[0] = signs[i];

        SpawnNumber(0, numberOne);
        SpawnNumber(1, numberTwo);
        SpawnNumber(2, numberThree);
        SpawnNumber(3, numberFour);

        SpawnSigns(0, symbols[0]);
        SpawnSigns(1, symbols[1]);


    }

     void Calculation (string symbols, int number1, int number2)
    {
        if (symbols == "+")
        {
            result = number1 + number2;
            numberOne = number1;
            numberTwo = number2;
        }

        if (symbols == "-")
        {
            if (number1 >= number2)
            {
                result = number1 - number2;
                numberOne = number1;
                numberTwo = number2;
            }

            else
            {
                result = number2 - number1;
                numberOne = number2;
                numberTwo = number1;
            }
        }
    }

    public void SpawnNumber (int i, int num) //assign match location for each number (i = array index in number group, num = spawn number)
    {
        if (num == 0)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;

            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
        }

        if (num == 1)
        {
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;

            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
        }

        if (num == 2)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
          
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
        }

        if (num == 3)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
        }

        if (num == 4)
        {
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;

            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
        }

        if (num == 5)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
        }

        if (num == 6)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
        }

        if (num == 7)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
        }

        if (num == 8)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
        }

        if (num == 9)
        {
            matchs[0 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[0], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;
            matchs[1 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[1], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[2 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[2], Quaternion.Euler(0f, 0f, 180f), mNumber[i].transform) as GameObject;
            matchs[3 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[3], Quaternion.Euler(0f, 0f, 90f), mNumber[i].transform) as GameObject;
            matchs[5 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[5], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
            matchs[6 + i * 7] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mNumber[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mNumber[i].transform) as GameObject;

            matchs[4 + i * 7] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mNumber[i].transform.position + offsetPos[4], Quaternion.Euler(0f, 0f, 0f), mNumber[i].transform) as GameObject;
        }
    }

    public void SpawnSigns (int i, string sign) //assign symbols location.
    {
        if (sign == "+")
        {
            matchs[28] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mSymbols[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mSymbols[i].transform) as GameObject;
            matchs[29] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mSymbols[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, 0f), mSymbols[i].transform) as GameObject;
        }

        if (sign == "-")
        {
            matchs[28] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mSymbols[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, -90f), mSymbols[i].transform) as GameObject;
            matchs[29] = Instantiate(Resources.Load("Prefab/PosHolderPrefab"), mSymbols[i].transform.position + offsetPos[6], Quaternion.Euler(0f, 0f, 0f), mSymbols[i].transform) as GameObject;
        }

        if (sign == "=")
        {
            matchs[30] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mSymbols[i].transform.position + offsetPos[7], Quaternion.Euler(0f, 0f, -90f), mSymbols[i].transform) as GameObject;
            matchs[31] = Instantiate(Resources.Load("Prefab/MatchPrefab"), mSymbols[i].transform.position + offsetPos[8], Quaternion.Euler(0f, 0f, -90f), mSymbols[i].transform) as GameObject;
        }
    }

    public void TestMatchNumber (GameObject[] matchGrp) // transfrom match number gameobject group into int
    {
        testingNo = new int[mNumber.Length];
        int[] mChild = new int[mNumber.Length];
        int matchNo = -1;
        bool c1 = false, c2 = false, c3 = false, c4 = false ;

        for(int i = 0; i < mNumber.Length; i++)
        {
            c1 = false; c2 = false; c3 = false;  c4 = false;
            mChild[i] = 0;

            for (int j=0; j < 7; j++)
            {
                if (matchGrp[i].transform.GetChild(j).tag == "Match")
                {
                    mChild[i]++;

                    //assig and test key location
                    if (matchGrp[i].transform.GetChild(j).localPosition == offsetPos[1])
                        c1 = true;
                    if (matchGrp[i].transform.GetChild(j).localPosition == offsetPos[2])
                        c2 = true;
                    if (matchGrp[i].transform.GetChild(j).localPosition == offsetPos[5])
                        c3 = true;
                    if (matchGrp[i].transform.GetChild(j).localPosition == offsetPos[6])
                        c4 = true;
                }

                if (mChild[i] == 1)
                    matchNo = -1;

                if (mChild[i] == 2)
                {
                    if (c1 == true && c2 == true)
                        matchNo = 1;
                    else
                        matchNo = -1;
                }

                if (mChild[i] == 3)
                {
                    if (c1 == true && c2 == true && c3 == false && c4 == false)
                        matchNo = 7;
                    else
                        matchNo = -1;
                }

                if (mChild[i] == 4)
                {
                    if (c1 == true && c2 == true && c3 == true && c4 == true)
                        matchNo = 4;
                    else
                        matchNo = -1;
                }

                if (mChild[i] == 7)
                    matchNo = 8;

                if(mChild[i] == 5)
                {
                    if (c1==true && c2 ==true && c3 == false && c4 == true)
                        matchNo = 3;
                    else if (c1 == false && c2 == true && c3 == true && c4 == true)
                        matchNo = 5;
                    else if (c1 == true && c2 == false && c3 == false && c4 == true)
                        matchNo = 2;
                    else
                        matchNo = -1;
                }

                if (mChild[i] == 6)
                {
                    if (c1 == true && c2 == true && c3 == true && c4 == true)
                        matchNo = 9;
                    else if (c1 == false && c2 == true && c3 == true && c4 == true)
                        matchNo = 6;
                    else if (c1 == true && c2 == true && c3 == true && c4 == false)
                        matchNo = 0;
                    else
                        matchNo = -1;
                }
            }

            testingNo[i] = matchNo;          
        }      
    }

    public void TestingMatchSymbols(GameObject[] signGrp) //transfer match symbol game object group into string.
    {
        testingSymbols = new string[mSymbols.Length];
        int[] mChild = new int[mSymbols.Length];
        for (int i = 0; i < mSymbols.Length; i++)
        {
            mChild[i] = 0;
            for (int j = 0; j < 2; j++)
            {
                if (signGrp[i].transform.GetChild(j).tag == "Match")
                {
                    mChild[i]++;
                }

                if (mChild[i] == 1)
                {
                    if (signGrp[i].transform.GetChild(0).localEulerAngles.z != 0)
                        testingSymbols[i] = signs[1];
                    else
                        testingSymbols[i] = "error";
                }

                else if (mChild[i] == 2)
                {
                    if (signGrp[i].transform.GetChild(0).localEulerAngles == signGrp[i].transform.GetChild(1).localEulerAngles)
                        testingSymbols[i] = signs[2];
                    else
                        testingSymbols[i] = signs[0];
                }
            }
        }
    }

    void CreatingList(GameObject[] mNGrp, GameObject[] mSGrp) // creating list for possible movement for assemble question.
    {
        matchToMove = new List<GameObject>();
        emptySpace = new List<GameObject>();
        noToSwap = new List<GameObject>();
        int arrayNo = 0;

        TestMatchNumber(mNumber);
        if (mNumber[3].activeSelf == false)
            arrayNo = mNumber.Length - 1;
        else
            arrayNo = mNumber.Length;

            for (int i = 0; i < arrayNo; i++)
        {
                if (testingNo[i] == 0)
                    noToSwap.Add(mNGrp[i]);

                if (testingNo[i] == 2)
                    noToSwap.Add(mNGrp[i]);

                if (testingNo[i] == 3)
                    noToSwap.Add(mNGrp[i]);

                if (testingNo[i] == 5)
                    noToSwap.Add(mNGrp[i]);

                if (testingNo[i] == 6)
                    noToSwap.Add(mNGrp[i]);

                if (testingNo[i] == 9)
                    noToSwap.Add(mNGrp[i]);


            for (int j = 0; j < 7; j++)
            {
                if (testingNo[i] == 1)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[0])
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                }

                if (testingNo[i] == 3)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[5])
                    {
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }
                }

                if (testingNo[i] == 5)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[1])
                    {
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }

                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[4])
                    {
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }
                }

                if (testingNo[i] == 6)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[1])
                    {
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }

                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[4])
                    {
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }
                }

                if (testingNo[i] == 7)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[0])
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                }

                if (testingNo[i] == 8)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[1])
                    {
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }

                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[4])
                    {
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }

                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[6])
                    {
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }
                }

                if (testingNo[i] == 9)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[1])
                    {
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }

                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[4])
                    {
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }

                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[5])
                    {
                        matchToMove.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }
                }

                if (testingNo[i] == 0)
                {
                    if (mNGrp[i].transform.GetChild(j).localPosition == offsetPos[6])
                    {
                        emptySpace.Add(mNGrp[i].transform.GetChild(j).gameObject);
                    }
                }
            }

            mtoM = matchToMove.Count;
            eS = emptySpace.Count;
            canSwap = noToSwap.Count;
        }

        TestingMatchSymbols(mSymbols);
        for(int i = 0; i <2; i++)
        {
            for(int j = 0; j <2; j ++)
            {
                if (testingSymbols[i] == "+")
                {
                    if (mSGrp[i].transform.GetChild(j).localEulerAngles.z == 0)
                        matchToMove.Add(mSGrp[i].transform.GetChild(j).gameObject);
                }

                if (testingSymbols[i] == "-")
                {
                    if (mSGrp[i].transform.GetChild(j).localEulerAngles.z == 0)
                        emptySpace.Add(mSGrp[i].transform.GetChild(j).gameObject);
                }
            }
        }
    }

    void SwapEmpty() //swap match across equation
    {
        int match = Random.Range(0, mtoM);
        int empty = Random.Range(0, eS);
        Transform tarParent, oriParent;
        Vector3 tempPos, tempAngle;

        tempPos = matchToMove[match].transform.position;
        tempAngle = matchToMove[match].transform.localEulerAngles;
        matchToMove[match].transform.position = emptySpace[empty].transform.position;
        matchToMove[match].transform.localEulerAngles = emptySpace[empty].transform.localEulerAngles;
        emptySpace[empty].transform.position = tempPos;
        emptySpace[empty].transform.localEulerAngles = tempAngle;

        oriParent = matchToMove[match].transform.parent;
        tarParent = emptySpace[empty].transform.parent;
        matchToMove[match].transform.parent = null;
        emptySpace[empty].transform.parent = null;

        matchToMove[match].transform.parent = tarParent;
        emptySpace[empty].transform.parent = oriParent;
    }

    void SwapMatch() //swap match within number
    {
        int mNum = Random.Range(0, canSwap);
        int numToChange =0;
        if(noToSwap[mNum].name =="Number 1")
            numToChange = testingNo[0];

        if (noToSwap[mNum].name == "Number 2")
            numToChange = testingNo[1];

        if (noToSwap[mNum].name == "Number 3")
            numToChange = testingNo[2];

        if (noToSwap[mNum].name == "Number 4")
            numToChange = testingNo[3];
        
         if(numToChange == 0)
         {
             int method = Random.Range(0, 2);
             if (method == 0)
                 SixZero(noToSwap[mNum]);
             if (method == 1)
                 NineZero(noToSwap[mNum]);
         }

         if (numToChange == 2)
             TwoThree(noToSwap[mNum]);

         if(numToChange == 3)
         {
             int method = Random.Range(0, 2);
             if (method == 0)
                 TwoThree(noToSwap[mNum]);
             if (method == 1)
                 ThreeFive(noToSwap[mNum]);
         }

         if(numToChange == 5)
             ThreeFive(noToSwap[mNum]);

         if (numToChange == 6)
         {
             int method = Random.Range(0, 2);
             if (method == 0)
                 SixNine(noToSwap[mNum]);
             if (method == 1)
                 SixZero(noToSwap[mNum]);
         }

         if (numToChange == 9)
         {
             int method = Random.Range(0, 2);
             if (method == 0)
                 NineZero(noToSwap[mNum]);
             if (method == 1)
                 SixNine(noToSwap[mNum]);
         }
     }

    //below are comment swap position within number group.
    void SixNine (GameObject mNum)
     {
        Vector3 tempPos, tempAngle;
        List<GameObject> holder = new List<GameObject>();
        for (int i = 0; i < 7; i++)
        {
            if (mNum.transform.GetChild(i).localPosition == offsetPos[1])
                holder.Add(mNum.transform.GetChild(i).gameObject);
            if (mNum.transform.GetChild(i).localPosition == offsetPos[4])
                holder.Add(mNum.transform.GetChild(i).gameObject);
        }

        tempPos = holder[0].transform.position;
        tempAngle = holder[0].transform.localEulerAngles;
        
        holder[0].transform.position = holder[1].transform.position;
        holder[0].transform.localEulerAngles = holder[1].transform.localEulerAngles;
        holder[1].transform.position = tempPos;
        holder[1].transform.localEulerAngles = tempAngle;
    }
     void SixZero(GameObject mNum)
     {
         Vector3 tempPos, tempAngle;
        List<GameObject> holder = new List<GameObject>();
         for (int i = 0; i < 7; i++)
         {
             if (mNum.transform.GetChild(i).localPosition == offsetPos[1])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
             if (mNum.transform.GetChild(i).localPosition == offsetPos[6])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
         }
         
        tempPos = holder[0].transform.position;
        tempAngle = holder[0].transform.localEulerAngles;

        holder[0].transform.position = holder[1].transform.position;
        holder[0].transform.localEulerAngles = holder[1].transform.localEulerAngles;
        holder[1].transform.position = tempPos;
        holder[1].transform.localEulerAngles = tempAngle;
    }
     void NineZero(GameObject mNum)
     {
         Vector3 tempPos, tempAngle;
        List<GameObject> holder = new List<GameObject>();
         for (int i = 0; i < 7; i++)
         {
             if (mNum.transform.GetChild(i).localPosition == offsetPos[4])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
             if (mNum.transform.GetChild(i).localPosition == offsetPos[6])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
         }
         
        tempPos = holder[0].transform.position;
        tempAngle = holder[0].transform.localEulerAngles;

        holder[0].transform.position = holder[1].transform.position;
        holder[0].transform.localEulerAngles = holder[1].transform.localEulerAngles;
        holder[1].transform.position = tempPos;
        holder[1].transform.localEulerAngles = tempAngle;
    }
     void TwoThree(GameObject mNum)
     {
         Vector3 tempPos, tempAngle;
        List<GameObject> holder = new List<GameObject>();
         for (int i = 0; i < 7; i++)
         {
             if (mNum.transform.GetChild(i).localPosition == offsetPos[2])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
             if (mNum.transform.GetChild(i).localPosition == offsetPos[4])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
         }

        tempPos = holder[0].transform.position;
        tempAngle = holder[0].transform.localEulerAngles;

        holder[0].transform.position = holder[1].transform.position;
        holder[0].transform.localEulerAngles = holder[1].transform.localEulerAngles;
        holder[1].transform.position = tempPos;
        holder[1].transform.localEulerAngles = tempAngle;
    }
     void ThreeFive(GameObject mNum)
     {
         Vector3 tempPos, tempAngle;
        List<GameObject> holder = new List<GameObject>();
         for (int i = 0; i < 7; i++)
         {
             if (mNum.transform.GetChild(i).localPosition == offsetPos[1])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
             if (mNum.transform.GetChild(i).localPosition == offsetPos[5])
                 holder.Add(mNum.transform.GetChild(i).gameObject);
         }

        tempPos = holder[0].transform.position;
        tempAngle = holder[0].transform.localEulerAngles;

        holder[0].transform.position = holder[1].transform.position;
        holder[0].transform.localEulerAngles = holder[1].transform.localEulerAngles;
        holder[1].transform.position = tempPos;
        holder[1].transform.localEulerAngles = tempAngle;
    }

    public void CheckAnswer() //checking new equation
    {
        checkNum = new GameObject[4];
        checkSign = new GameObject[2];

        checkNum[0] = GameObject.Find("Number 1");
        checkNum[1] = GameObject.Find("Number 2");
        checkNum[2] = GameObject.Find("Number 3");
        checkNum[3] = GameObject.Find("Number 4");
        if (checkNum[3] == null)
            checkNum[3] = GameObject.Find("Number 3");

        checkSign[0] = GameObject.Find("Sign 1");
        checkSign[1] = GameObject.Find("Sign 2");

        TestingMatchSymbols(checkSign);
        for (int i = 0; i < symbols.Length; i++)
        {
            if (testingSymbols[i] == "error")
            {
                StartCoroutine("Wrong");
                isCorrect = false;
            }
        }

        TestMatchNumber(checkNum);
        for (int i = 0; i < checkNum.Length; i++)
        {
            if (testingNo[i] == -1)
            {
                StartCoroutine("Wrong");
                isCorrect = false;
            }
        }

        Calculation(testingSymbols[0], testingNo[0], testingNo[1]);

        if (result >= 10)
        {
            if (testingNo[2] == 1 && testingNo[3] == result - 10)
            {
                StartCoroutine("Correct");
                isCorrect = true;
            }
            else
            {
                StartCoroutine("Wrong");
                isCorrect = false;
            }
        }
        else
        {
            if (testingNo[2] == result)
            {
                StartCoroutine("Correct");
                isCorrect = true;
            }
            else
            {
                StartCoroutine("Wrong");
                isCorrect = false;
            }
        }

        Debug.Log("No1: " + testingNo[0]);
        Debug.Log(testingSymbols[0]);
        Debug.Log("No2: " + testingNo[1]);
        Debug.Log("No3: " + testingNo[2]);
        Debug.Log("No4: " + testingNo[3]);
    }

    private IEnumerator Correct()
    {
        FindObjectOfType<UpdateUI>().correctPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        FindObjectOfType<UpdateUI>().levelSelect.SetActive(true);
        Time.timeScale = 0;
    }

    private IEnumerator Wrong()
    {
        FindObjectOfType<UpdateUI>().incorrectPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        FindObjectOfType<UpdateUI>().levelSelect.SetActive(true);
        Time.timeScale = 0;
        
    }

    void Record()
    {
        mRecord = new GameObject[mNumber.Length];
        sRecord = new GameObject[mSymbols.Length];

        mRecord[0] = Instantiate(GameObject.Find("Number 1"), mNumber[0].transform.position, Quaternion.identity) as GameObject;
        mRecord[0].SetActive(false);

        mRecord[1] = Instantiate(GameObject.Find("Number 2"), mNumber[1].transform.position, Quaternion.identity) as GameObject;
        mRecord[1].SetActive(false);

        mRecord[2] = Instantiate(GameObject.Find("Number 3"), mNumber[2].transform.position, Quaternion.identity) as GameObject;
        mRecord[2].SetActive(false);

        if (mNumber[3].activeSelf == true)
            mRecord[3] = Instantiate(GameObject.Find("Number 4"), mNumber[3].transform.position, Quaternion.identity) as GameObject;
        else
        {
            mRecord[3] = Instantiate(GameObject.Find("Number 3"), mNumber[3].transform.position, Quaternion.identity) as GameObject;
            testingNo[3] = testingNo[2];
        }

        mRecord[3].SetActive(false);

        sRecord[0] = Instantiate(GameObject.Find("Sign 1"), mSymbols[0].transform.position, Quaternion.identity) as GameObject;
        sRecord[0].SetActive(false);

        sRecord[1] = Instantiate(GameObject.Find("Sign 2"), mSymbols[1].transform.position, Quaternion.identity) as GameObject;
        sRecord[1].SetActive(false);
    }
    public void Replay()
    {
        Time.timeScale = 1;
        
        for (int i = 0; i < mNumber.Length; i++)
        {
            Destroy(mNumber[i]);
        }

        for (int i = 0; i < mSymbols.Length; i++)
        {
            Destroy(mSymbols[i]);
        }
        
        CreateTargetPosition();

            for (int i = 0; i < mRecord.Length; i++)
        {
            while (mRecord[i].transform.childCount != 0) //using while loop to make sure all children object has been changed, had issue with using for loop only.
            {
                for (int j = 0; j < mRecord[i].transform.childCount; j++)
                {
                    mRecord[i].transform.GetChild(j).SetParent(mNumber[i].transform, true);
                }
            }

            Destroy(mRecord[i]);
        }

        for (int i = 0; i < mSymbols.Length; i++)
        {
            while (sRecord[i].transform.childCount != 0)
            {
                for (int j = 0; j < sRecord[i].transform.childCount; j++)
                {
                    sRecord[i].transform.GetChild(j).SetParent(mSymbols[i].transform, true);
                }
            }

            Destroy(sRecord[i].gameObject);
        }

        if (isCorrect == false)
            _attempt++;
        else
            _corrected++;

        if (sigleDig == true)
            mNumber[3].SetActive(false);
        
        FindObjectOfType<UpdateUI>().levelSelect.SetActive(false);
        FindObjectOfType<UpdateUI>().correctPanel.SetActive(false);
        FindObjectOfType<UpdateUI>().incorrectPanel.SetActive(false);
    }
}
