using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenExEg
{

    /// <summary>
    /// Этот класс хранит иинформацию о достоинстве карты
    /// </summary>
    [System.Serializable]
    public class CardDefinition
    {
        public string face; // Спрайт - лицевая сторона карты
        public int rank; // Достоинства карты
        public List<Decorator> pips = new List<Decorator>(); // Список значков карты
    }
}
