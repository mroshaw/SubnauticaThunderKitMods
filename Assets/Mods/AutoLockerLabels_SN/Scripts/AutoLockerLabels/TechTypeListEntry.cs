using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    public sealed class TechTypeListEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text sourceText;
        [SerializeField] private Button removeButton;
        private TechType techType;
        private Action<TechType> removed;

        internal void Bind(
            TechType value,
            string sourceName,
            bool isModded,
            Action<TechType> callback)
        {
            techType = value;
            removed = callback;
            string localizedName = Language.main == null ? string.Empty : Language.main.Get(techType);
            nameText.text = string.IsNullOrWhiteSpace(localizedName)
                ? techType.ToString()
                : $"{localizedName}  ({techType})";
            iconImage.sprite = SpriteManager.Get(techType);
            iconImage.enabled = iconImage.sprite != null;
            sourceText.text = sourceName;
            sourceText.color = isModded
                ? new Color(1f, 0.68f, 0.05f, 1f)
                : new Color(0.42f, 0.84f, 1f, 1f);
            removeButton.onClick.RemoveListener(Remove);
            removeButton.onClick.AddListener(Remove);
        }

        private void Remove()
        {
            if (removed != null)
            {
                removed(techType);
            }
        }
    }
}
