using UnityEngine;
using UnityEngine.UI;

public class LifeBarElementUI : MonoBehaviour
{
    public RectTransform m_LifeBarRectTransform;
    public Image m_ForegroundLifeBarUI;

    public void Show(Vector3 WorldPosition, float LigePct)
    {
        Vector3 l_lifeBarViewportPosition = GameManager.GetGameManager().GetPlayer().m_Camera.WorldToViewportPoint(WorldPosition);
        if (l_lifeBarViewportPosition.z > 0.0f)
        {
            Vector2 l_PositionUI = new Vector2(l_lifeBarViewportPosition.x * 1920.0f, -(1.0f - l_lifeBarViewportPosition.y)*1080);
            m_LifeBarRectTransform.anchoredPosition = l_PositionUI;
            m_ForegroundLifeBarUI.fillAmount= LigePct;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }


    }
}
