using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SevenExEg
{
    public class Deck : MonoBehaviour
    {
        [Header("Установи")]
        public bool startFaceUp = false;
        // Масти 
        public Sprite suitClub;
        public Sprite suitDiamond;
        public Sprite suitHeart;
        public Sprite suitSpade;

        public Sprite[] faceSprites;
        public Sprite[] rankSprites;

        public Sprite cardBack;
        public Sprite cardFront;

        // Шаблоны
        public GameObject prefabCard;
        public GameObject prefabSprite;

        //("Загружается")]
        [HideInInspector] public PT_XMLReader xmlr;
        [HideInInspector] public List<string> cardNames;
        [HideInInspector] public List<Decorator> decorators;
        [HideInInspector] public List<CardDefinition> cardDefs;
        [HideInInspector] public Transform deckAnchor;
        [HideInInspector] public Dictionary<string, Sprite> dictSuits;
        [HideInInspector] public List<Card> table;
        [HideInInspector] public List<Card> well;

        #region Переменные необходимые для ограничения стартовый колоды (Deck)
        private int StartRank = 6; // начальный ранг,
        private int LastRank = 14; // конечный ранг, 

        #endregion

        //  Инициализация доски(расклада)
        public void InitDeck(string deckXMLText)
        {
            // Создать точку привязки для всех игровых объектов Card в иерархии
            if (GameObject.Find("_Deck") == null)
            {
                GameObject anchorGO = new GameObject("_Deck");
                anchorGO.transform.position = new Vector3(-10, -6, 0);
                deckAnchor = anchorGO.transform;
            }

            // Инициализировать словарь со спрайтамми значков мастей
            dictSuits = new Dictionary<string, Sprite>()
        {
            { "C", suitClub },
            { "D", suitDiamond },
            { "H", suitHeart },
            { "S", suitSpade }
        };

            ReadDeck(deckXMLText);
            MakeCards();
        }

        // Чтение данных их XML файла 
        public void ReadDeck(string deckXMLText)
        {
            xmlr = new PT_XMLReader();
            xmlr.Parse(deckXMLText);


            // Прочитать элементы <decorator> для всех карт
            decorators = new List<Decorator>();
            PT_XMLHashList xDecos = xmlr.xml["xml"][0]["decorator"];
            Decorator deco;
            for (int i = 0; i < xDecos.Count; i++)
            {
                // Для каждого элемента <decorator> в XML
                deco = new Decorator(); // Создать экземпляр Decorator
                                        // Скопировать атрибуты из <decorator> в Decorator
                deco.type = xDecos[i].att("type");
                // deco.flip получит значение true, если атрбут flip содержит текст "1"
                deco.flip = (xDecos[i].att("flip") == "1");
                // Получить значения float из строковых атрибутов
                deco.scale = float.Parse(xDecos[i].att("scale"), CultureInfo.InvariantCulture);
                // Vector3 loc инициализируется как [0,0,0]
                // поэтому нам остается только изменить его
                deco.loc.x = float.Parse(xDecos[i].att("x"), CultureInfo.InvariantCulture);
                deco.loc.y = float.Parse(xDecos[i].att("y"), CultureInfo.InvariantCulture);
                deco.loc.z = float.Parse(xDecos[i].att("z"), CultureInfo.InvariantCulture);
                // Добавить deco в список decorators
                decorators.Add(deco);
            }

            // Прочитать координаты для значков, определяющих достоинство карты
            cardDefs = new List<CardDefinition>(); // Инициализировать список карт
                                                   // Извлечь список PT_XMLHashList всех элементов <card> из XML-файла
            PT_XMLHashList xCardDefs = xmlr.xml["xml"][0]["card"];
            for (int i = 0; i < xCardDefs.Count; i++)
            {
                // Для каждого экземпляра <card>
                // Создать экземпляр CardDefinition
                CardDefinition cDef = new CardDefinition();
                // Получить значения атрибута и добавить их в cDef
                cDef.rank = int.Parse(xCardDefs[i].att("rank"));
                // Извлечь список PT_XMLHashList всех элементов <pip>
                // внтури этого элемента <card>
                PT_XMLHashList xPips = xCardDefs[i]["pip"];
                if (xPips != null)
                {
                    for (int j = 0; j < xPips.Count; j++)
                    {
                        // Обойти все элементы <pip>
                        deco = new Decorator();
                        // Элементы <pip> в <card> обрабатываются классом Decorator
                        deco.type = "pip";
                        deco.flip = (xPips[j].att("flip") == "1");
                        deco.loc.x = float.Parse(xPips[j].att("x"), CultureInfo.InvariantCulture);
                        deco.loc.y = float.Parse(xPips[j].att("y"), CultureInfo.InvariantCulture);
                        deco.loc.z = float.Parse(xPips[j].att("z"), CultureInfo.InvariantCulture);
                        if (xPips[j].HasAtt("scale"))
                        {
                            deco.scale = float.Parse(xPips[j].att("scale"), CultureInfo.InvariantCulture);
                        }
                        cDef.pips.Add(deco);
                    }
                }
                // Карты с картинками (Валет, Дама и Король) имеют атрибут face
                if (xCardDefs[i].HasAtt("face"))
                {
                    cDef.face = xCardDefs[i].att("face");
                }
                cardDefs.Add(cDef);
            }

        }

        // Создает игровые объекты карт
        public void MakeCards()
        {

            // cardNames имена всех карт в колоде 
            cardNames = new List<string>();
            string[] letters = new string[] { "C", "D", "H", "S" };
            foreach (string s in letters)       // Добавляем для каждой масти
            {
                for (int i = StartRank; i < LastRank; i++)
                    cardNames.Add(s + (i + 1));
            }

            // Создать список со вссеми картами
            table = new List<Card>();

            // Для каждой созданного имени карты
            for (int i = 0; i < cardNames.Count; i++)
            {
                // Создать карту-объект
                table.Add(MakeCard(i));
            }
        }

        public void RemoveWell(Card card)
        {
            table.Add(card);
            well.Remove(card);
            card.state = eCardState.deck;
        }

        // Статическая функция создающую изначальную раскладку пасьянса
        static public void Shuffle(ref List<Card> resCards, ref List<Card> wellCards)
        {
            // TODO:(Oleg) добавить анимацию
            // Колоды Разденная на карты на столе resCards и wellCards
            List<Card> tempCards = new List<Card>();
            int id = 0;
            while (resCards.Count > 4)
            {
                id = Random.Range(0, resCards.Count);
                tempCards.Add(resCards[id]);
                resCards.RemoveAt(id);
            }

            while (resCards.Count > 0)
            {
                wellCards.Add(resCards[0]);
                resCards.RemoveAt(0);
            }
            // Обновляем раcположение колоды
            int StartRank = 6;
            int LastRank = 14;
            LastRank--;
            int deltarank = (LastRank - StartRank);
            // TODO: (Oleg) Переделать
            int deltax = 2;     // Изначальные значения 3
            int deltay = 3;     // Изначальные значения 4
            float sc = 0.7f;       // Изначальные значения 0.7f

            Card cgo;
            for (int i = 0; i < tempCards.Count; i++)
            {
                cgo = tempCards[i];
                cgo.transform.localPosition = new Vector3((i % deltarank) * deltax, i / deltarank * deltay, 0);
                cgo.transform.localScale = new Vector3(sc, sc, sc);
            }

            // Задаем расположение колодцам
            for (int i = 0; i < wellCards.Count; i++)
            {
                cgo = wellCards[i];
                cgo.transform.localPosition = new Vector3(18 + (i % 2) * deltax, (i / 2) * deltay, 0);
                cgo.transform.localScale = new Vector3(sc, sc, sc);
                wellCards[i].state = eCardState.well;
            }
            resCards = tempCards;
        }

        public CardDefinition GetCardDefinitionByRank(int rnk)
        {
            // Поиск во всех определениях CardDefinition
            foreach (CardDefinition cd in cardDefs)
            {
                // Если достоинство совпдает, вернуть это определение
                if (cd.rank == rnk)
                {
                    return (cd);
                }
            }
            return (null);
        }

        //TODO: (Oleg) переделать в нормальный стиль
        #region Cкрытые переменные используються вспомогательными методами
        // 
        private Sprite _tSp = null;
        private GameObject _tGO = null;
        private SpriteRenderer _tSR = null;
        #endregion

        #region Приватные функции
        // Создать карту 
        private Card MakeCard(int cNum)
        {
            // Создать новый игровой объект с картой
            GameObject cgo = Instantiate(prefabCard) as GameObject;
            // Настроить transform.parent новой карты в соответсвии с точкой привязки.
            cgo.transform.parent = deckAnchor;
            Card card = cgo.GetComponent<Card>(); // Получить компонент Card

            // Настроить основные параметры карты
            card.name = cardNames[cNum];                    // Имя карты
            card.suit = card.name[0].ToString();            // Масть
            card.rank = int.Parse(card.name.Substring(1));  // Ранг
            if (card.suit == "D" || card.suit == "H")       // Цвет карты, зависит от масти
            {
                card.colS = "Red";
                card.color = Color.red;
            }


            AddDecorators(card);  // Добавляем спрайты украшения

            card.def = GetCardDefinitionByRank(card.rank);
            AddPips(card);        // Добвляем значок для численных карт
            AddFace(card);

            //AddBack(card);

            return card;
        }

        private void AddDecorators(Card card)
        {
            // Добавить оформление
            foreach (Decorator deco in decorators)
            {
                // Масть
                if (deco.type == "suit")
                {
                    // Создать экземпляр игорового объекта спрайта
                    _tGO = Instantiate(prefabSprite) as GameObject;
                    // Получить ссылку на компонент SpriteRenderer
                    _tSR = _tGO.GetComponent<SpriteRenderer>();
                    // Установить спрайт масти
                    _tSR.sprite = dictSuits[card.suit];
                }
                else
                {
                    _tGO = Instantiate(prefabSprite) as GameObject;
                    _tSR = _tGO.GetComponent<SpriteRenderer>();
                    // Получить спрайт для отображения достоинства
                    _tSp = rankSprites[card.rank];
                    // Установить спрайт достоинства в SpriteRenderer
                    _tSR.sprite = _tSp;
                    // Установить цвет соответствующий масти
                    _tSR.color = card.color;
                }
                // Поместить спрайты под картой
                _tSR.sortingOrder = 1;
                // Сделать спрайт дочерним по отношению к карте
                _tGO.transform.SetParent(card.transform);
                // Установить localPosition, как определено в DeckXML
                _tGO.transform.localPosition = deco.loc;
                // Перевернуть значок, если необходимо
                if (deco.flip)
                {
                    // Эйлеров поворт на 180 относительно оси Z-axis
                    _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                // Установить масштаб, чтобы уменьшить размер спрайта
                if (deco.scale != 1)
                {
                    _tGO.transform.localScale = Vector3.one * deco.scale;
                }
                // Дать имя этому игровому объекту для наглядности
                _tGO.name = deco.type;
                // Добавить этот игровой объект с оформлением в список card.decoGOs
                card.decoGOs.Add(_tGO);
            }
        }

        //TODO:(Oleg) Добвляет Значки номерной карты
        private void AddPips(Card card)
        {
            foreach (Decorator pip in card.def.pips)
            {
                //...Создать игровой объекть спрайта
                _tGO = Instantiate(prefabSprite) as GameObject;
                // Назначить родителем игровой объект карты
                _tGO.transform.SetParent(card.transform);
                // Установить localPosition, как определено в XML - файле
                _tGO.transform.localPosition = pip.loc;
                // Перевернуть, если необходимо
                if (pip.flip)
                {
                    _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                // Масшатабировать, если необходимо (только для туза)
                if (pip.scale != 1)
                {
                    _tGO.transform.localScale = Vector3.one * pip.scale;
                }
                // Дать имя игровому объекту
                _tGO.name = "pip";
                // Получить ссылку на компонент SpriteRenderer
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                // Установить спрайт масти
                _tSR.sprite = dictSuits[card.suit];
                // Установить sortingOrder, чтобы значок отображался над Card_Front
                _tSR.sortingOrder = 1;
                // Добавить этот игровой объект в список значков
                card.pipGos.Add(_tGO);
            }
        }

        //TODO: (Oleg) Добавить картинку для туза, короля,дамы, вальта
        private void AddFace(Card card)
        {
            if (card.def.face == null)
            {
                return; // Выйти если это не карта с картинкой
            }

            _tGO = Instantiate(prefabSprite) as GameObject;
            _tSR = _tGO.GetComponent<SpriteRenderer>();
            // Сгенерировать имя и передать его в GetFace()
            _tSp = GetFace(card.def.face + card.suit);
            _tSR.sprite = _tSp; // Установить этот спрайт в _tSR
            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = Vector3.zero;
            _tGO.name = "face";
            _tSR.sortingOrder = 1;
        }

        // Находит спрайт с картинкой для карты
        private Sprite GetFace(string faceS)
        {
            foreach (Sprite _tSP in faceSprites)
            {
                // Если найден спрайт с требуемым именем...
                if (_tSP.name == faceS)
                {
                    // ...вернуть его
                    return (_tSP);
                }
            }
            // Если ничего не найдено, вернуть null
            return (null);

        }

        //Добавить рубашку, по стандарту все рубашкой вверх
        private void AddBack(Card card)
        {
            // Card_Back будет покрывать все остальное на карте
            _tGO = Instantiate(prefabSprite) as GameObject;
            _tSR = _tGO.GetComponent<SpriteRenderer>();
            _tSR.sprite = cardBack;
            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = Vector3.zero;
            // Большее значение sortingOrder, чем у других спрайтов
            _tSR.sortingOrder = 2;
            _tGO.name = "back";
            card.back = _tGO;

            // По умолчанию картинкой вверх
            card.faceUP = startFaceUp; // Использовать свойство faceUp карты
        }

        #endregion

    }
}
