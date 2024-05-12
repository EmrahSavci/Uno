using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnoPlayerCardList : MonoBehaviour
{
    public List<UnoCard> cardsInHand = new List<UnoCard>();

    public List<UnoCard> copyCardList = new List<UnoCard>();

    public GameObject lastcardText;
    public GameObject lastcardpenaltyText;
    public bool isUno = false;
    public int playerIndex = 0;

    public GameObject effect;
    public GameObject blurEffect;

    public float[] gridLayourSpacingValue;
    HorizontalLayoutGroup layoutGroup;
    public GameObject passBtn;
    public GameObject arrow;
    void Start()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        StartCoroutine(ArrowControl());
        StartCoroutine(CardOrderColor());
    }
    public void UnoBtn()
    {
        if(transform.childCount==2 && playerIndex==UnoCardSpawn.Instance.PlayerIndex)
        {
            lastcardText.SetActive(true);
            isUno = true;
            Invoke("DeactiveText", 2);
        }
    }
    IEnumerator ArrowControl()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(10);
        while (true)
        {
            if (UnoCardSpawn.Instance.PlayerIndex == playerIndex && UnoCardSpawn.Instance.rightCardCount == 0)
                arrow.SetActive(true);
            else
                arrow.SetActive(false);
           
            yield return delay;
        }
    }
    IEnumerator CardOrderColor()
    {
        WaitForSeconds delay = new WaitForSeconds(10f);
        yield return new WaitForSeconds(1);
        while (true)
        {
            OrderCard();
            yield return delay;
        }
    }
    public void LastCardPenalty(bool enable)
    {
        lastcardpenaltyText.SetActive(enable);
        Invoke("DeactiveText", 2);
    }
    void DeactiveText()
    {
        if (lastcardText.activeSelf)
            lastcardText.SetActive(false);
        else if (lastcardpenaltyText.activeSelf)
            lastcardpenaltyText.SetActive(false);
    }
    public void OrderCard(bool orderlist)
    {   
        cardsInHand.Clear();
        copyCardList.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            cardsInHand.Add(transform.GetChild(i).gameObject.GetComponent<UnoCard>());
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < cardsInHand.Count; j++)
            {
                if (cardsInHand[j].colorIndex == i)
                    copyCardList.Add(cardsInHand[j]);
            }
        }
        for (int i = 0; i < copyCardList.Count; i++)
        {
            GameObject card = copyCardList[i].gameObject;
            card.transform.parent = transform;
            card.transform.SetSiblingIndex(i);
        }
        if(transform.childCount==1 && !isUno && !orderlist)
        {
            LastCardPenalty(true);
            for (int i = 0; i < 2; i++)
            {
                UnoCardSpawn.Instance.SentCard(playerIndex, false);
            }
           
        }
        if (transform.childCount >= 2)
            isUno = false;
        if (transform.childCount <= 0)
            UnoManager.Instance.GameFinish(playerIndex);

        passBtn.SetActive(false);
       // effect.SetActive(effectEnable);
    }
    public void OrderCard()
    {
        cardsInHand.Clear();
        copyCardList.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            cardsInHand.Add(transform.GetChild(i).gameObject.GetComponent<UnoCard>());
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < cardsInHand.Count; j++)
            {
                if (cardsInHand[j].colorIndex == i)
                    copyCardList.Add(cardsInHand[j]);
            }
        }
        for (int i = 0; i < copyCardList.Count; i++)
        {
            GameObject card = copyCardList[i].gameObject;
            card.transform.parent = transform;
            card.transform.SetSiblingIndex(i);
        }
      

        passBtn.SetActive(false);
        // effect.SetActive(effectEnable);
    }
    public void SetLayoutSpacing()
    {
        layoutGroup.spacing = GetLayoutSpacing(transform.childCount);
    }
    float GetLayoutSpacing(int childCount)
    {
        float value=layoutGroup.spacing;
        if (childCount<21)
        {
            value=gridLayourSpacingValue[childCount];
        }
        else if(childCount>=21)
        {
            value= layoutGroup.spacing;
            value += -1;
            
        }
        return value;
    }
    public bool IsThereFourDrawCard()
    {
        bool isThere = false;
        for (int j = 0; j < cardsInHand.Count; j++)
        {
            if (cardsInHand[j].colorIndex == 4 && cardsInHand[j].numberStr == "+4")
            {
                isThere = true;
                break;
            }
                
            else
                isThere = false;
        }

        return isThere;
    }
    public bool IsThereTwoDrawCard()
    {
        bool isThere = false;
        for (int j = 0; j < cardsInHand.Count; j++)
        {
            if (cardsInHand[j].numberStr == "+2")
            {
                isThere = true;
                break;
            }

            else
                isThere = false;
        }

        return isThere;
    }
}
