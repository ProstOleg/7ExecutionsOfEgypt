using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    deck,   // на столе
    select, // выбрана
    well,   // в кеолодце
    onplace // на своем месте
}

// Этот класс определяет базовую карту
public class Card : MonoBehaviour
{
    [Header("Загружается")]
    public string suit; // масть карты
    public int rank;    // ранг карты
                        // (у нас используется только туз,король,дама,валет,10,9,8,7)
    public Color color = Color.black; // цвет карты(изначально черный)
    public string colS = "Black"; // то же самое, только в виде строковых данных
    public List<GameObject> decoGOs = new List<GameObject>(); // список объектов Decorator для карты
    public List<GameObject> pipGos = new List<GameObject>(); // список объектов Pip для карты
    public GameObject back; // объект рубашки карты
    public CardDefinition def; // Извлекается из DeckXML.xml
    public SpriteRenderer[] spriteRenderers; // массив спрайтов карты

    public eCardState state = eCardState.deck;
    

   

    // Если массив спрайтов пуст, то этот метод определяет его
    public void PopulateSpriteRenderers()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }


    // Переворачивает карту 
    public bool faceUP
    {
        get
        {
            return (!back.activeSelf);
        }
        set
        {
            back.SetActive(!value);
        }
    }

    public void SetSortOrder(int sOrd)
    {

        PopulateSpriteRenderers();

        // Выполнить обход всех элементов в списке spriteRenderers
        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == this.gameObject)
            {
                // Если компонент принадлежит текущему игровому объекту, это фон
                tSR.sortingOrder = sOrd; // Установить порядковый номер для сортировки в sOrd
                continue; // И перейти к следующей итерации цикла
            }

            // Каждый дочерний игровой объект имеет имя
            // Установить порядковый номер для сортировки, в зависимости от имени
            switch (tSR.gameObject.name)
            {
                case "back": // если имя "back"
                    // Установить наибольший порядковый номер
                    // для отображения поверх других спрайтов
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "face":    // если имя "face"
                default:  // или же другое
                    // Установить промежуточный порядковый номер 
                    // для отображения поверх фона
                    tSR.sortingOrder = sOrd + 1;
                    break;
            }
        }
    }

    public void setSortingLayerName(string tSLN)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }

    // Реакция при нажатии мышкой на карту, виртуальная
    public void OnMouseUpAsButton()
    {
        SevenExEg.S.CardClicked(this);
    }
}

//Этот класс хранит информацию из DeckXML о каждом значке на карте
[System.Serializable] 
public class Decorator
{
    public string type; // Значок с достоинством карты
    public Vector3 loc; //Метоположение спрайта
    public bool flip = false; // Признак переворота спрайта по вертикали
    public float scale = 1f; // Масштаб спрайта
}

// Этот класс хранит иинформацию о достоинстве карты
[System.Serializable]
public class CardDefinition
{
    public string face; // Спрайт - лицевая сторона карты
    public int rank; // Достоинства карты
    public List<Decorator> pips = new List<Decorator>(); // Список значков карты
}
