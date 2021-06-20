using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPopup : MonoBehaviour 
{
    [SerializeField] RectTransform Background;
    [SerializeField] Image ItemImage;
    [SerializeField] Text Name;
    [SerializeField] Text Type;
    [SerializeField] Text Description;
    [SerializeField] Text Value;

	public void UpdateUI(InventoryItem item)
    {
        int height = Mathf.Max(200, 10 + item.Data.Height * 50);
        int width = 450 + 25 * (item.Data.Width - 1);

        Name.text = item.EffectiveName;

        Type.rectTransform.sizeDelta = new Vector2(Type.rectTransform.sizeDelta.x, 30f);

        if (!item.IsIdentified || item.IsBroken)
        {
            Type.text = "";
            Description.alignment = TextAnchor.MiddleCenter;
            Description.color = Color.red;
            Value.text = "";
            ItemImage.color = item.IsBroken ? Color.red : Color.green;
        }
        else
        {
            string typeText = "Type: " + item.Data.GetTypeDescription();
            if(item.Data is Weapon)
            {
                Weapon weap = item.Data as Weapon;
                typeText += "\nAttack: +" + weap.BaseDamage + "   Damage: " + weap.DamageRoll.Rolls + "d" + weap.DamageRoll.Sides;
                if (weap.BaseDamage > 0)
                    typeText += " + " + weap.BaseDamage;
                Type.rectTransform.sizeDelta = new Vector2(Type.rectTransform.sizeDelta.x, 60f);
                height += 30;
            }
            else if (item.Data is Armor)
            {
                Armor arm = item.Data as Armor;
                if (arm.EquipSlot != EquipSlot.Ring)
                {
                    typeText += "\nArmor: +" + arm.AC;
                    Type.rectTransform.sizeDelta = new Vector2(Type.rectTransform.sizeDelta.x, 60f);
                    height += 30;
                }
            }
            Type.text = typeText;

            if (item.Enchantment != null)
            {
                height += 50;
            }
            Description.color = Color.white;
            Description.alignment = TextAnchor.UpperLeft;
            Value.text = "Value: " + item.EffectiveValue;
            ItemImage.color = Color.white;
        }

        Description.text = item.EffectiveDescription;

        ItemImage.sprite = item.Data.sprite;
        ItemImage.SetNativeSize();

        float itemHeight = ItemImage.preferredHeight + 50f;
        height = (int)Mathf.Max(itemHeight, height);

        Background.sizeDelta = new Vector2(width, height);

        if(Input.mousePosition.x <= Screen.width / 2)
            Background.position = Input.mousePosition + new Vector3(Background.sizeDelta.x / 2f, -Background.sizeDelta.y / 2, 0);
        else
            Background.position = Input.mousePosition + new Vector3(-Background.sizeDelta.x / 2f, -Background.sizeDelta.y / 2, 0);
    }
}
