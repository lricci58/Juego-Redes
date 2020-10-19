using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private float updateHealthSpeed = 0.5f;

    void Awake()
    {
        GetComponentInParent<UnitScript>().OnHealthChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float healtPct)
    {
        // suaviza el descenso de la barra de vida
        StartCoroutine(UpdateHealthBar(healtPct));
    }

    private IEnumerator UpdateHealthBar(float healtPct)
    {
        float originalHealth = healthImage.fillAmount;
        // para medir el tiempo de actualizacion
        float elapsed = 0f;

        while (elapsed < updateHealthSpeed)
        {
            elapsed += Time.deltaTime;
            // suaviza el movimiento entre los valores inicial y final
            healthImage.fillAmount = Mathf.Lerp(originalHealth, healtPct, elapsed / updateHealthSpeed);
            yield return null;
        }

        healthImage.fillAmount = healtPct;
    }
}
