using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public Slider fillPercentSlider; // UI'deki Slider
    public Slider fillEnemySpeedSlider;

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0) // Menü sahnesindeysek çalıştır
        {
            // PlayerPrefs'te kayıtlı bir değer varsa onu al, yoksa varsayılanı (15) ata
            int savedValue = Mathf.RoundToInt(PlayerPrefs.GetFloat("FillPercent", 15f));
            fillPercentSlider.value = Mathf.Clamp(savedValue, 1f, 30f);

            // PlayerPrefs'ten kaydedilen değeri al ve slider'ı buna göre ayarla
            float savedEnemySpeed = PlayerPrefs.GetFloat("EnemySpeed", 1f); // Varsayılan değer 1f
            fillEnemySpeedSlider.value = Mathf.Clamp(savedEnemySpeed, 0.1f, 5f);

            // Slider her değiştiğinde değeri kaydet
            fillPercentSlider.onValueChanged.AddListener(UpdateFillPercent);
            fillEnemySpeedSlider.onValueChanged.AddListener(UpdateEnemySpeed);
        }
    }

    void UpdateFillPercent(float value)
    {
        // Değeri int'e yuvarla ve 1 ile 30 arasında sınırla
        int clampedValue = Mathf.RoundToInt(Mathf.Clamp(value, 1f, 30f));
        PlayerPrefs.SetInt("FillPercent", clampedValue); // Int olarak kaydet
        PlayerPrefs.Save();
        Debug.Log("Kaydedilen Fill Percent: " + clampedValue);
    }

    void UpdateEnemySpeed(float value)
    {
        // Enemy speed değerini kaydet
        PlayerPrefs.SetFloat("EnemySpeed", value); // Float olarak kaydet
        PlayerPrefs.Save();
        Debug.Log("Kaydedilen Enemy Speed: " + value);
    }
    
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
