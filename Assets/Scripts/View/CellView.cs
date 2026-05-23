using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CellView : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer BackgroundImage { get; private set; }
        [field: SerializeField] public TMP_Text MinesNearCountText {get; private set;}
        [field: SerializeField] public Sprite ClosedSprite { get; private set; }
        [field: SerializeField] public Sprite OpenedSprite { get; private set; }
        [field: SerializeField] public Sprite MineSprite { get; private set; }
    }
}
