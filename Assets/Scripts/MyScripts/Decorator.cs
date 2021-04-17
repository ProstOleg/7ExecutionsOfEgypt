using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenExEg
{
    /// <summary>
    /// Этот класс хранит информацию из DeckXML о каждом значке на карте
    /// </summary>
    [System.Serializable]
    public class Decorator
    {
        public string type; // Значок с достоинством карты
        public Vector3 loc; //Метоположение спрайта
        public bool flip = false; // Признак переворота спрайта по вертикали
        public float scale = 1f; // Масштаб спрайта
    }
}
