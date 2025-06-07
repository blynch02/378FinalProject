using TMPro;
using UnityEngine;

public class ResistanceDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI protectionText;
    [SerializeField] private TextMeshProUGUI bleedText;
    [SerializeField] private TextMeshProUGUI stunText;
    [SerializeField] private TextMeshProUGUI confusionText;
    [SerializeField] private TextMeshProUGUI fireText;
    [SerializeField] private TextMeshProUGUI poisonText;
    [SerializeField] private TextMeshProUGUI blindText;
    [SerializeField] private TextMeshProUGUI crippleText;

    public void DisplayResistances(Enemy enemy)
    {
        if (enemy == null)
        {
            protectionText.text = "Protection: %";
            bleedText.text = "Bleed: %";
            stunText.text = "Stun: %";
            confusionText.text = "Confusion: %";
            fireText.text = "Fire: %";
            poisonText.text = "Poison: %";
            blindText.text = "Blind: %";
            crippleText.text = "Cripple: %";
            return;
        }

        protectionText.text = $"Protection: {enemy.protectionRes}%";
        bleedText.text = $"Bleed: {enemy.bleedRes}%";
        stunText.text = $"Stun: {enemy.stunRes}%";
        confusionText.text = $"Confusion: {enemy.confusionRes}%";
        fireText.text = $"Fire: {enemy.fireRes}%";
        poisonText.text = $"Poison: {enemy.poisonRes}%";
        blindText.text = $"Blind: {enemy.blindRes}%";
        crippleText.text = $"Cripple: {enemy.crippleRes}%";
    }

}
