using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SevenExEg
{
    /// <summary>
    /// Где карта расположена
    /// </summary>
    public enum eCardState
    {
        deck,   // на столе
        select, // выбрана
        well,   // в колодце
        onplace // на своем месте
    }


    /// <summary>
    /// Этот класс определяет базовую карту
    /// </summary>
    public class Card : MonoBehaviour
    {
        [Header("Загружается")]
        // масть карты
        public string suit;
        // ранг карты(у нас используется только туз,король,дама,валет,10,9,8,7)
        public int rank;
        // цвет карты(изначально черный)
        public Color color = Color.black;
        //TODO:(Oleg) проверить надобность
        // то же самое, только в виде строковых данных 
        public string colS = "Black";
        // список объектов Decorator для карты
        public List<GameObject> decoGOs = new List<GameObject>();
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

        // Реакция при нажатии мышкой на карту
        public void OnMouseUpAsButton()
        {
            SevenExEg.Instance.CardClicked(this);
        }
    }
}
