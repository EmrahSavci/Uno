using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnoCardSpawn : MonoBehaviour
{
    public static UnoCardSpawn Instance;
    public Button drawCardFromCenterBtn;
    public GameObject player2DrawCardBtn;
    public List<UnoPlayerCardList> unoPlayers = new List<UnoPlayerCardList>();


    [Header("Card Spawn In UI")]
    public GameObject card;
    public Transform CardFirstPos;
    public Transform playerCardName;
    public HorizontalLayoutGroup layoutGroup;
    public Transform[] player;
    public GameObject[] drawCardArrow;
    public CardsInPlayerHand[] cardsInPlayerHands;


    [Header("Card Spawn in Scene")]
    public GameObject card2;
    public Transform parent;

    public int PlayerIndex = 0;
    public int playersCount = 3;
    [SerializeField] List<GameObject> centerCard = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        drawCardFromCenterBtn.interactable = false;
        StartCoroutine(DealingCard());
    }
    public IEnumerator DealingCard()
    {
        WaitForSeconds delay = new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(1f);
        for (int i = 0; i < playersCount+1; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                SentCard(i, false);
                yield return delay;
            }
        }

        SelectFirstStartCard();

    }
    void SelectFirstStartCard()
    {
        GameObject _card = Instantiate(card, transform.position, Quaternion.Euler(0, 0, 0), playerCardName);


        _card.transform.SetSiblingIndex(playerCardName.transform.childCount - 5);
        againSelectColor:
        _card.GetComponent<Image>().sprite = UnoManager.Instance.SelectSpriteToCard();
        int randomColor = UnoManager.Instance.randomColor;
        if (randomColor == 4)
            goto againSelectColor;
        else
        {
            _card.GetComponent<UnoCard>().SelectColor(randomColor);
            _card.GetComponent<UnoCard>().numberStr = UnoManager.Instance.cardNumber();
            if (_card.GetComponent<UnoCard>().numberStr == "+2")
                goto againSelectColor;
        }

        centerCard.Add(_card);
        UnoManager.Instance.GetColor(_card.GetComponent<UnoCard>().colorIndex, _card.GetComponent<UnoCard>().numberStr);
        

        ShowRightCard(false);
        PlayerCardListAdd();
        // unoPlayers[0].effect.SetActive(true);
        unoPlayers[0].blurEffect.SetActive(true);
        UnoManager.Instance.RotateCenterArrow();
        UnoManager.Instance.isStartGame = true;
    }
    void PlayerCardListAdd()
    {
        for (int i = 0; i < playersCount; i++)
        {
            unoPlayers[i].OrderCard(false);
        }
    }
    // Update is called once per frame
    public int totalCardCount = 0;
    public int randomColor = 0;
    public void SentCard(int playerIndex, bool isDrawCard)
    {
        if (totalCardCount >=55)
        {
            totalCardCount = 0;
            UnoManager.Instance.CopyCardList();
        }

        if (UnoManager.Instance.gameState == EnumHolder.GameState.Stopped)
            return;
        GameObject _card = Instantiate(card, CardFirstPos.position, player[playerIndex].rotation, transform);
        LeanTween.rotateAround(_card, new Vector3(0, 0, 360), 360, 0.2f).setRepeat(3);
        UnoSoundManager.Instance.cardDrawSoundPlay();
        centerCard.Add(_card);
        LeanTween.move(_card, player[playerIndex].position, 0.3f).setOnComplete(() =>
          {
              LeanTween.cancel(_card);
              if (totalCardCount <56)
              {
                  _card.transform.parent = player[playerIndex];
                  _card.transform.eulerAngles = player[playerIndex].transform.eulerAngles;
                  _card.GetComponent<Button>().onClick.AddListener(() => SentToCenterTheCard(_card));
                  _card.GetComponent<Image>().sprite = UnoManager.Instance.SelectSpriteToCard();
                  randomColor = UnoManager.Instance.randomColor;
                  _card.GetComponent<UnoCard>().SelectColor(randomColor);
                  _card.GetComponent<UnoCard>().numberStr = UnoManager.Instance.cardNumber();
                  _card.GetComponent<UnoCard>().playerIndex = playerIndex;

                  cardsInPlayerHands[playerIndex].unoCards.Add(_card.GetComponent<UnoCard>());

                  unoPlayers[playerIndex].SetLayoutSpacing();

                  DrawCardArrowClose(false);
                  if (isDrawCard)
                  {

                      ShowRightCard(isDrawCard);

                  }
                  unoPlayers[playerIndex].OrderCard(true);
                  totalCardCount++;
                  //if (totalCardCount >= 56)
                  //{
                  //    totalCardCount = 0;
                  //    Debug.Log("KARTLARR BÝTTÝ");
                  //    UnoManager.Instance.CopyCardList();
                  //}
              }
                  
          });
        

    }
    public void DrawCardArrowClose(bool _enable)
    {
        for (int i = 0; i < drawCardArrow.Length; i++)
        {
            drawCardArrow[i].SetActive(false);
            unoPlayers[i].passBtn.SetActive(false);
        }
    }

    public void GetNewCard()
    {
        drawCardArrow[1].SetActive(false);
        player2DrawCardBtn.SetActive(false);
        SentCard(PlayerIndex, true);
        drawCardFromCenterBtn.interactable = false;
       

    }
    public void DrawPenaltyCard(int playerIndex)
    {
        SentCard(playerIndex, false);
    }
    public void SpawnCard()
    {
        GameObject _card = Instantiate(card2, parent.position, Quaternion.identity, parent);
        _card.GetComponent<SpriteRenderer>().sprite = UnoManager.Instance.SelectSpriteToCard();
        int value = 1;
        float scale = 0;
        if (parent.transform.childCount > 1)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                parent.GetChild(i).transform.localPosition = new Vector3(0, scale * value, 0);
                // value *= -1;
                scale += 0.05f;
            }
        }

    }

    public bool isCanClickNewCard = true;
    public void SentToCenterTheCard(GameObject card)
    {
        if (card.GetComponent<UnoCard>().playerIndex != PlayerIndex || !isCanClickNewCard || UnoManager.Instance.gameState == EnumHolder.GameState.Stopped || !UnoManager.Instance.isStartGame)
            return;


        isCanClickNewCard = false;
        card.transform.parent = playerCardName;
        card.transform.SetAsLastSibling();
        
        for (int i = 0; i < 4; i++)
        {
            unoPlayers[i].passBtn.SetActive(false);
        }
        LeanTween.moveLocal(card, Vector3.zero, 0.5f).setOnComplete(() =>
            {
                card.transform.eulerAngles = new Vector3(0, 0, Random.Range(-30, 30));
                UnoManager.Instance.GetColor(card.GetComponent<UnoCard>().colorIndex, card.GetComponent<UnoCard>().numberStr);
                randomColor = UnoManager.Instance.colorIndex;
                cardsInPlayerHands[PlayerIndex].unoCards.Remove(card.GetComponent<UnoCard>());
                unoPlayers[PlayerIndex].OrderCard(false);
                unoPlayers[PlayerIndex].SetLayoutSpacing();
                Rules(card.GetComponent<UnoCard>());

                card.transform.SetSiblingIndex(playerCardName.transform.childCount - 5);


            });
        UnoSoundManager.Instance.SendCardToCenterPlay();

    }
    void Rules(UnoCard card)
    {
        if (card.numberStr == "ColorChange" || card.numberStr == "+4")
        {
            UnoRules.Instance.OpenPanel(card.numberStr);
        }
        else if (card.numberStr == "turn")
            UnoRules.Instance.TurnTour();

        

        if (card.numberStr == "+2")
        {
            TwoDrawCardControl();

        }
       else if (card.numberStr != "ColorChange" && card.numberStr != "+4")
            PassNextPlayer(UnoRules.Instance.tourValue);
        if (card.numberStr == "next")
        {

            PassNextPlayer(UnoRules.Instance.tourValue);
        }
    }
    int twoCardCount = 2;
    void TwoDrawCardControl()
    {
        UnoRules.Instance.isPenalty = true;
        PassNextPlayer(UnoRules.Instance.tourValue);
        if (!unoPlayers[PlayerIndex].IsThereTwoDrawCard())
        {
            StartCoroutine(UnoRules.Instance.DrawTwoCard(twoCardCount, PlayerIndex));


            twoCardCount = 2;
        }
        else
            twoCardCount += 2;
    }
    public int rightCardCount = 0;
    public void ShowRightCard(bool passNextPlayer)
    {
        Debug.Log("PENALTY: " + UnoRules.Instance.isPenalty);
        DrawCardArrowClose(false);
        rightCardCount = 0;
        for (int i = 0; i < player[PlayerIndex].childCount; i++)
        {
            player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = false;
        }
        for (int i = 0; i < player[PlayerIndex].childCount; i++)
        {
            if (UnoManager.Instance.cardNumberStr == "+2" && player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "+2" && UnoRules.Instance.isPenalty)
            {
                rightCardCount++;
                player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
               // UnoRules.Instance.isPenalty = false;
                Debug.Log("+2 kart bulundu");
            }
            else if (UnoManager.Instance.cardNumberStr == "+2" && player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "+2" && !UnoRules.Instance.isPenalty)
            {
                rightCardCount++;
                player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                //UnoRules.Instance.isPenalty = true;
                Debug.Log("+2 kart bulundu 2");
            }
             if ((UnoManager.Instance.cardNumberStr != "+4" && UnoManager.Instance.cardNumberStr != "+2") && (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == UnoManager.Instance.cardNumberStr ||
                 UnoManager.Instance.colorIndex == player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex)
                  )
            {
                rightCardCount++;
                player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                Debug.Log("diðer kartlar bulundu  bulundu");
            }
            else if(!UnoRules.Instance.isPenalty && (!unoPlayers[PlayerIndex].IsThereTwoDrawCard() && !unoPlayers[PlayerIndex].IsThereFourDrawCard())  &&(player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == UnoManager.Instance.cardNumberStr ||
                 UnoManager.Instance.colorIndex == player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex) )
            {
                rightCardCount++;
                player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                Debug.Log("diðer kartlar bulundu  bulundu 2");
            }
           else  if (!UnoRules.Instance.isPenalty && UnoManager.Instance.cardNumberStr == "+4" && (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == UnoManager.Instance.cardNumberStr ||
                UnoManager.Instance.colorIndex == player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex))
            {
                rightCardCount++;
                player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                Debug.Log("diðer kartlar bulundu  bulundu 3");
            }
           else if (!UnoRules.Instance.isPenalty && UnoManager.Instance.cardNumberStr == "+2" && (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == UnoManager.Instance.cardNumberStr ||
               UnoManager.Instance.colorIndex == player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex))
            {
                rightCardCount++;
                player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                Debug.Log("diðer kartlar bulundu  bulundu 4");
            }
            if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex == 4 && UnoManager.Instance.cardNumberStr == "+4"  && !UnoRules.Instance.isPenalty)
            {
                if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "+4")
                {
                    rightCardCount++;
                    player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                    Debug.Log("+4 kart bulundu ");
                }
                
            }
            else if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex == 4 && UnoManager.Instance.cardNumberStr == "+2" && !UnoRules.Instance.isPenalty)
            {
                if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "+4")
                {
                    rightCardCount++;
                    player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                    Debug.Log("+4 kart bulundu 2");
                }
                else if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "ColorChange")
                {

                    rightCardCount++;
                    player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                    Debug.Log("color change kart bulundu 2 ");

                }
            }
             if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex == 4 && UnoManager.Instance.cardNumberStr != "+2" && !UnoRules.Instance.isPenalty)
            {
                if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "+4")
                {
                    rightCardCount++;
                    player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                    Debug.Log("+4 kart bulundu 2");
                }
                else if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "ColorChange")
                {

                    rightCardCount++;
                    player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                    Debug.Log("color change kart bulundu 3");

                }
            }
            else if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().colorIndex == 4  && UnoManager.Instance.cardNumberStr != "+2" && UnoManager.Instance.cardNumberStr == "+4" && !UnoRules.Instance.isPenalty)
            {

                if (player[PlayerIndex].GetChild(i).gameObject.GetComponent<UnoCard>().numberStr == "ColorChange")
                {

                    rightCardCount++;
                    player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                    Debug.Log("color change kart bulundu 4");

                }
            }


        }
        //if (randomColor != 4 && rightCardCount >= 1)
        //    unoPlayers[PlayerIndex].passBtn.SetActive(true);
        if (rightCardCount == 0 && !passNextPlayer)
            DrawCard();
        else if (passNextPlayer && rightCardCount == 0)
            PassNextPlayer(UnoRules.Instance.tourValue);

        UnoManager.Instance.ArrowColorChange();

    }
    void DrawCard()
    {
        drawCardFromCenterBtn.interactable = true;
        drawCardArrow[PlayerIndex].SetActive(true);
        for (int i = 0; i < player[PlayerIndex].childCount; i++)
        {
            player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = false;
        }
    }
    public void PassNextPlayer(int changeValue)
    {
        if (UnoManager.Instance.gameState == EnumHolder.GameState.Stopped)
            return;

        unoPlayers[PlayerIndex].OrderCard(true);
        DrawCardArrowClose(false);
        for (int i = 0; i < player[PlayerIndex].childCount; i++)
        {
            player[PlayerIndex].GetChild(i).gameObject.GetComponent<Button>().interactable = true;
        }
        unoPlayers[PlayerIndex].effect.SetActive(false);
        unoPlayers[PlayerIndex].blurEffect.SetActive(false);
        PlayerIndex += changeValue;
        if (PlayerIndex > playersCount)
            PlayerIndex = 0;
        else if (PlayerIndex < 0)
            PlayerIndex = playersCount;

       // drawCardArrow[PlayerIndex].SetActive(true);
        unoPlayers[PlayerIndex].blurEffect.SetActive(true);
        ShowRightCard(false);
        unoPlayers[PlayerIndex].OrderCard(true);
        isCanClickNewCard = true;
    }
    public void PassTour()
    {
        PassNextPlayer(UnoRules.Instance.tourValue);
        for (int i = 0; i < 4; i++)
        {
            unoPlayers[i].passBtn.SetActive(false);
        }
    }
    public IEnumerator DeactiveAllCard(int winnerPlayerIndex)
    {
        yield return new WaitForSeconds(1f);
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        for (int i = centerCard.Count - 1; i >= 0; i--)
        {
            yield return delay;
            centerCard[i].SetActive(false);
        }
        yield return new WaitForSeconds(4);
        QuitGame.Instance.WinnerPlayerNameSling(UnoManager.Instance.playerName[winnerPlayerIndex]);
    }
}
[System.Serializable]
public class CardsInPlayerHand
{
    public List<UnoCard> unoCards = new List<UnoCard>();
}