using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PartyTargetButtons : MonoBehaviour
{
    [SerializeField] private List<Button> allyButtons;
    private Character activeCharacter;

    private Action postSelectionCallback;

    public void Show(Character activeChar, List<GameObject> party, Action onSelection = null)
    {
        activeCharacter = activeChar;
        postSelectionCallback = onSelection;

        for (int i = 0; i < allyButtons.Count; i++)
        {
            if (i < party.Count)
            {
                GameObject ally = party[i];
                Character allyChar = ally.GetComponent<Character>();
                if (allyChar != null && !allyChar.isdead)
                {
                    Button btn = allyButtons[i];
                    btn.gameObject.SetActive(true);
                    
                    TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
                    label.text = ally.name;

                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        activeCharacter.SetAllyTarget(ally);
                        HighlightSelected(btn);
                        postSelectionCallback?.Invoke();
                        Hide();
                    });
                }
                else
                {
                    allyButtons[i].gameObject.SetActive(false);
                }
            }
            else
            {
                allyButtons[i].gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(true);
    }


    private void HighlightSelected(Button selected)
    {
        foreach (var btn in allyButtons)
        {
            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            var img = btn.image;

            if (btn == selected)
            {
                txt.text = "Selected";
                img.color = Color.cyan;
            }
            else
            {
                txt.text = btn.name;
                img.color = Color.white;
            }
        }
    }

    public void Hide()
    {
        foreach (var btn in allyButtons)
        {
            btn.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
