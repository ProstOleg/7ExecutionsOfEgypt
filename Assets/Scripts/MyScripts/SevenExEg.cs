﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ePhase
{
    //start,  // начало игры
    idle,   // карта не выбрана
    select,  // есть выбранная карта
    //end     // конец игры
}



public class SevenExEg : MonoBehaviour
{
    static public SevenExEg S;

    [Header("Установи")]
    public TextAsset deckXML;

    [Header("Загружается")]
    public Deck deck;
    public ePhase phase = ePhase.idle;
    public Card SelectCard;
    public string VoidName ="";
    public Vector2 VoidPlace;
    public int score;
    public Text UIHints;

    private Vector3 SelectPlace = new Vector3(19, 8, 0);

    void Awake()
    {
        S = this;
        score = 0;
        //UIHints = GetComponent<Text>
    }

    void Start()
    {
        deck = GetComponent<Deck>();    // Получить компонент Deck.cs
        deck.InitDeck(deckXML.text);    // Создаем стартовую колоду из deckXML.xml 
        Deck.Shuffle(ref deck.table, ref deck.well);   // Стартовую колоду раскладываем на стол, 4 карты в колодец
        
    }

    void Update()
    {
        // TODO: передалть в Action
        if (deck.well.Count==0)
        {
            if (SelectCard.rank == 7 || SelectCard.name == VoidName)
            {
                string txt = "";
                if (score >= 31)
                    txt = "You win!";
                else
                    txt = "You Lose!";
                Result.ShowRes(txt);
            }
        }
    }

    

    public void CardClicked(Card card)
    {
        if (card.state == eCardState.onplace || card.state == eCardState.select)
            return;

        // TODO: По возможности переделать в switch
        if (phase == ePhase.idle)
        {
            if (VoidName == "")
            {
                float Vx = card.transform.localPosition.x;
                float Vy = card.transform.localPosition.y;
                VoidName = FindSuit(Vy)+FindRank(Vx);
                VoidPlace = new Vector2(Vx, Vy);
                print("выбрана "+VoidName+" позиция");
            }

            card.transform.localPosition = SelectPlace;   
            phase = ePhase.select;

            SelectCard = card;
            SelectCard.state = eCardState.select;
            score++;
        }
        else
        {
            
            if(card.state == eCardState.well)
            {
                if (SelectCard.rank == 7 || SelectCard.name == VoidName)
                {
                    deck.RemoveWell(card);
                    card.transform.localPosition = SelectPlace;   
                    CardGoToVoid(SelectCard.name);

                    card.state = eCardState.select;
                    SelectCard = card;
                    score++;
                }
                return;
            }

            if (CheckPlace(card))
            {
                ChangeCards(card);
                score++;
                print("Позиции совпадают");
            }
            else
            {
                print("Позиции разные");
            }
        }
    }

    public bool CheckPlace(Card card)
    {
        // TODO: По возможности переделать в switch
        // Если метоположение совпадает тогда меняем, иначе нифига
        if (SelectCard.suit == "C" && card.transform.localPosition.y == 0)
        {
            print("Проверяем Трефы");
            return Checkrank(card.transform.localPosition.x);
        }
        if (SelectCard.suit == "D" && card.transform.localPosition.y == 3)
        {
            print("Проверяем Бубны");
            return Checkrank(card.transform.localPosition.x);
        }
        if (SelectCard.suit == "H" && card.transform.localPosition.y == 6)
        {
            print("Проверяем Черви");
            return Checkrank(card.transform.localPosition.x);
        }
        if (SelectCard.suit == "S" && card.transform.localPosition.y == 9)
        {
            print("Проверяем Пики");
            return Checkrank(card.transform.localPosition.x);
        }

        return false;

    }

    private bool Checkrank(float card_x)
    {
        float need_x = (14 - SelectCard.rank) * 2;
        //print(card_x);
        if (card_x == need_x)
            return true; 
        return false;
    }

    private string FindRank(float card_x)
    {
        int rank = (int )(14 - (card_x / 2f));
        return rank.ToString();
    }

    private string FindSuit(float card_y)
    {
        // TODO: По возможности переделать в switch
        if (card_y == 0)
            return "C";
        if (card_y == 3)
            return "D";
        if (card_y == 6)
            return "H";
        if (card_y == 9)
            return "S";
        return "";
    }

    public void ChangeCards(Card card)
    {
        SelectCard.transform.localPosition = card.transform.localPosition;
        SelectCard.state = eCardState.onplace;
        card.transform.localPosition = SelectPlace;
        //TODO:
        card.state = eCardState.select;
        SelectCard = card;  
    }

    private void CardGoToVoid(string CardName)
    {
        
        if (CardName != VoidName)  //Если не изначально пустующее место, то ранг=7
        {
            float v_y= -1;
            float v_x = v_x = 14;
            switch (SelectCard.suit)
            {
                case "C":
                    {
                        v_y = 0;
                        break;
                    }

                case "D":
                    { 
                        v_y = 3;
                        break; 
                    }
                case "H":
                    {
                        v_y = 6;
                        break;
                    }
                case "S":
                    {
                        v_y = 9;
                        break;
                    }
                default:
                    {
                        Debug.LogError("Не верная масть");
                        break;
                    }
            }
            SelectCard.transform.localPosition = new Vector2(v_x, v_y);
            print("колодец " + CardName);
        }
        else
        {
            SelectCard.transform.localPosition = VoidPlace;
            print("Изначальное место");
        }
        SelectCard.state = eCardState.onplace;
    }
}
