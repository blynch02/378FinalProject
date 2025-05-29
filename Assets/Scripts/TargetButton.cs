using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetButton : MonoBehaviour
{
    public Character owner;
    public GameObject enemyRef;
    public TextMeshProUGUI label;

    private Button button;
    private static TargetButton currentlySelected;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (currentlySelected == this)
        {
            SetSelected(false);
            currentlySelected = null;
            owner.SetTarget(null);
        }
        else
        {
            if (currentlySelected != null)
                currentlySelected.SetSelected(false);

            currentlySelected = this;
            SetSelected(true);
            owner.SetTarget(enemyRef);
        }
    }

    public void SetSelected(bool selected)
    {
        if (label != null)
            label.text = selected ? "Selected" : "Attack";

        if (button != null)
            button.image.color = selected ? Color.green : Color.white;
    }

    private void OnDisable()
    {
        if (currentlySelected == this)
        {
            SetSelected(false);
            currentlySelected = null;
        }
    }
}
