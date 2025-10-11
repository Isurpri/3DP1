using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI m_AmmoText;
    public TextMeshProUGUI m_LifeText;
    public TextMeshProUGUI m_ShieldText;
    public TextMeshProUGUI m_ScoreText;

    private void Awake()
    {
        Instance = this;
    }

    public void UiVariables(float chargerAmmo, float totalAmmo, float life, float shield)
    {
        if (m_AmmoText != null)
        {
            m_AmmoText.text = $"{chargerAmmo} / {totalAmmo}";
        }
        if (m_LifeText != null)
        {
            m_LifeText.text = $"{life}";
        }
        if (m_ShieldText != null)
        {
            m_ShieldText.text = $"{shield}";
        }
    }
    public void SumScore(float score)
    {
        if (m_ScoreText != null)
        {
            m_ScoreText.text = $"{score}";
        }
    }
    
}
