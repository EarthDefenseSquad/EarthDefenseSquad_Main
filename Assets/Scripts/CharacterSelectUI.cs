using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterInfo
{
    public string characterName;
    public string description;
    public Sprite characterSprite;
}

public class CharacterSelectUI : MonoBehaviour
{
    public CharacterInfo[] characters;

    public Text infoText;
    public Image infoImage;

    // 버튼에서 호출할 함수, index는 버튼 번호
    public void OnCharacterSelected(int index)
    {
        if (index < 0 || index >= characters.Length) return;

        CharacterInfo selected = characters[index];
        infoText.text = $"{selected.characterName}\n{selected.description}";
        infoImage.sprite = selected.characterSprite;
    }
}
