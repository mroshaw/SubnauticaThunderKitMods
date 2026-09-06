using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    public sealed class TechTypePickerEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text sourceText;
        [SerializeField] private Button selectButton;
        private TechType techType;
        private Action<TechType> selected;

        internal void Bind(
            TechType value,
            string displayName,
            string sourceName,
            bool isModded,
            Action<TechType> callback)
        {
            techType = value;
            selected = callback;
            nameText.text = displayName;
            sourceText.text = sourceName;
            sourceText.color = isModded
                ? new Color(1f, 0.68f, 0.05f, 1f)
                : new Color(0.42f, 0.84f, 1f, 1f);
            iconImage.sprite = SpriteManager.Get(techType);
            iconImage.enabled = iconImage.sprite != null;
            selectButton.onClick.RemoveListener(Select);
            selectButton.onClick.AddListener(Select);
        }

        private void Select()
        {
            if (selected != null)
            {
                selected(techType);
            }
        }
    }
}
