using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoRules : MonoBehaviour
{
    public static UnoRules Instance;

    public Sprite[] arrowTurnSprite;
    public SpriteRenderer arrowSpriteRenderer;

    public List<GameObject> selectCardColorPanel = new List<GameObject>();
    public GameObject dontTouchPanel;
    public int tourValue = 1;
    private void Awake()
    {
        Instance = this;
    }
    int penalytPlayer = 0;
    public bool isPenalty = false;
    public IEnumerator DrawTwoCard(int drawCardCount,int playerIndex)
    {
        penalytPlayer = playerIndex;
        UnoSoundManager.Instance.smileSoundPlay();
       UnoCardSpawn.Instance.isCanClickNewCard = false;
        
        for (int i = 0; i < drawCardCount; i++)
        {
            yield return new WaitForSeconds(0.3f);
            UnoCardSpawn.Instance.DrawPenaltyCard(penalytPlayer);
            
        }
        isPenalty = false;
        
        UnoCardSpawn.Instance.PassNextPlayer(tourValue);
        
    }
    void DrawCardFromPlace()
    {
        UnoCardSpawn.Instance.DrawPenaltyCard(penalytPlayer);
    }
    public void TurnTour()
    {
        tourValue *= -1;
        UnoManager.Instance.RotateCenterArrow();
        arrowSpriteRenderer.sprite = arrowTurnSprite[UnoManager.Instance.colorIndex];
        arrowSpriteRenderer.gameObject.SetActive(true);
        arrowSpriteRenderer.transform.localScale = new Vector3(tourValue, 1, 1);
        LeanTween.rotateAround(arrowSpriteRenderer.gameObject, new Vector3(0, 0, 360), 360 * tourValue, 1).setRepeat(1).setOnComplete(() =>
          {
              arrowSpriteRenderer.gameObject.SetActive(false);

          });
        
    }
    public string cardName = "";
    int FourCardCount = 4;
    public void SelectColor(int colorIndex)
    {
        UnoManager.Instance.GetColor(colorIndex, UnoManager.Instance.cardNumberStr);
        UnoCardSpawn.Instance.PassNextPlayer(tourValue);
        // UnoManager.Instance.ArrowColorChange();
        if (cardName == "+4")
        {
            isPenalty = true;
            if (!UnoCardSpawn.Instance.unoPlayers[UnoCardSpawn.Instance.PlayerIndex].IsThereFourDrawCard())
            {   
                StartCoroutine(DrawTwoCard(FourCardCount, UnoCardSpawn.Instance.PlayerIndex));

                
                FourCardCount = 4;
            }
            else
                FourCardCount += 4;
                
        }
       
        else
            UnoCardSpawn.Instance.ShowRightCard(false);

        dontTouchPanel.SetActive(false);
        //else
        //    UnoCardSpawn.Instance.PassNextPlayer(tourValue);


    }
    public void OpenPanel(string _cardName)
    {   if (UnoManager.Instance.gameState == EnumHolder.GameState.Stopped)
            return;
        dontTouchPanel.SetActive(true);
        selectCardColorPanel[UnoCardSpawn.Instance.PlayerIndex].SetActive(true);
        cardName = _cardName;
    }
}
