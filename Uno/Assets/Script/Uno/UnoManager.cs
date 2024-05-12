using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UnoManager : MonoBehaviour
{
    public static UnoManager Instance;
    public EnumHolder.GameState gameState;
    public List<CardList> cardLists = new List<CardList>();
    public List<CardList> copyCardList = new List<CardList>();
    public int randomColor;
    int randomNumber;

    public Color[] arrowColor;
    public SpriteRenderer arrowSpriteRenderer;
    public int rotateValue = -1;

    [Header("Game Finish")]
    [SerializeField] ParticleSystem conffeti;
    [SerializeField] GameObject[] winnerImg;
    [SerializeField] GameObject restartBtn;
    [SerializeField] GameObject smoke;
    public string[] playerName;
    public bool isStartGame = false;
    public enum Colors
    {
        Red,
        Yellow,
        Blue,
        Green
    }
    public Colors colors;
    public string cardNumberStr;
    public int colorIndex = 0;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        //StartCoroutine(SmokeEffect());
        //StartCoroutine(PlayerSmokeEffect());
        //StartCoroutine(UnoCardSpawn.Instance.DeactiveAllCard());

    }
    public void CopyCardList()
    {
         //cardLists = copyCardList;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < copyCardList[i].cards.Count; j++)
            {
                cardLists[i].cards.Add(copyCardList[i].cards[j]);
            }
        }
    }
    public void RotateCenterArrow()
    {
        arrowSpriteRenderer.gameObject.SetActive(true);
        rotateValue = UnoRules.Instance.tourValue;
        LeanTween.cancel(arrowSpriteRenderer.gameObject);
        LeanTween.rotateAround(arrowSpriteRenderer.gameObject, new Vector3(0, 0, 360), 360 * rotateValue, 3).setRepeat(-1);
        arrowSpriteRenderer.transform.localScale = new Vector3(rotateValue, 1, 1);
    }
    // Update is called once per frame
    Sprite cardSprite;
    public Sprite SelectSpriteToCard()
    {

        againCardSelect:
        int samecount = 0;
        randomColor = Random.Range(0, 100);

        if (randomColor >= 0 && randomColor <= 23)
            randomColor = 0;
        else if (randomColor > 23 && randomColor <= 46)
            randomColor = 1;
        else if (randomColor > 46 && randomColor <= 69)
            randomColor = 2;
        else if (randomColor > 69 && randomColor <= 92)
            randomColor = 3;
        else if (randomColor >92)
            randomColor = 4;

        randomNumber = Random.Range(0, cardLists[randomColor].cards.Count);

       


        if (cardLists[randomColor].cards.Count >= 1)
            cardSprite = cardLists[randomColor].cards[randomNumber];

        else
        {
            for (int i = 0; i < cardLists.Count; i++)
            {
                if (cardLists[i].cards.Count >= 1)
                    samecount++;
            }
        }

        if (samecount >= 1)
            goto againCardSelect;


        ArrowColorChange();
        return cardSprite;
    }
    public void ArrowColorChange()
    {
        if (colorIndex <= 3)
            arrowSpriteRenderer.color = arrowColor[colorIndex];
    }

    public void GetColor(int index, string number)
    {


        cardNumberStr = number;
        colorIndex = index;
        if (colorIndex <= 3)
            arrowSpriteRenderer.color = arrowColor[colorIndex];
    }
    public string cardNumber()
    {
        string number="";
        if (cardLists[randomColor].cards.Count >= 1)
        {
             number = cardLists[randomColor].cards[randomNumber].name;
            cardLists[randomColor].cards.RemoveAt(randomNumber);
           
        }
        else
        {
            CopyCardList();
            SelectSpriteToCard();
           
            number = cardLists[randomColor].cards[randomNumber].name;
            cardLists[randomColor].cards.RemoveAt(randomNumber);
            Debug.Log("KARTLARR KOPYALANDI");
        }
        return number;
    }
    public void GameFinish(int winnerPlayerIndex)
    {
        if (isStartGame)
        {
            conffeti.Play();
            winnerImg[winnerPlayerIndex].SetActive(true);
            restartBtn.SetActive(true);
            gameState = EnumHolder.GameState.Stopped;
            UnoSoundManager.Instance.winnerSoundPlay();
            StartCoroutine(SmokeEffect());
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(PlayerSmokeEffect(i));
            }

            StartCoroutine(UnoCardSpawn.Instance.DeactiveAllCard(winnerPlayerIndex));
        }

    }
    IEnumerator SmokeEffect()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Instantiate(smoke, transform.position, Quaternion.identity);

        }
    }
    IEnumerator PlayerSmokeEffect(int i)
    {
        yield return new WaitForSeconds(1f);

        for (int j = 0; j < UnoCardSpawn.Instance.unoPlayers[i].transform.childCount; j++)
        {
            yield return new WaitForSeconds(0.2f);
            UnoCardSpawn.Instance.unoPlayers[i].transform.GetChild(j).gameObject.SetActive(false);
            Vector2 cardPosScene = Camera.main.ScreenToWorldPoint(UnoCardSpawn.Instance.unoPlayers[i].transform.GetChild(j).position);
            Instantiate(smoke, cardPosScene, Quaternion.identity);

        }



    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
[System.Serializable]
public class CardList
{
    public List<Sprite> cards = new List<Sprite>();

}