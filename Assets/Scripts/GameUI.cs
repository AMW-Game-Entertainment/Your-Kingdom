using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using GameConstants;

public class GameUI : MonoBehaviour
{
    // Fields on UI
    [SerializeField]
    private TextMeshProUGUI dayText;
    [SerializeField]
    private TextMeshProUGUI cityText;
    [SerializeField]
    private TextMeshProUGUI cashText;
    [SerializeField]
    private TextMeshProUGUI populationText;
    [SerializeField]
    private TextMeshProUGUI foodText;
    [SerializeField]
    private TextMeshProUGUI jobsText;
    [SerializeField]
    private TextMeshProUGUI deathText;
    [SerializeField]
    private TextMeshProUGUI notificationText;
    [SerializeField]
    private Image notificationBackground;
    [SerializeField]
    public GameObject menu;

    private float lastCheckNotificationTimer;
    [Header("Timers")]
    public float notificationTimer;

    // Declare Singleton this class
    public static GameUI instance;
    [Header("Current City")]
    // External instances
    public City cityInstance;

    private void Awake()
    {
        instance = this;
        // Setup the city name by the scene name
        cityText.text = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        lastCheckNotificationTimer -= Time.deltaTime;
        if (lastCheckNotificationTimer <= 0.0f)
        {
            DeleteNotification();
        }
    }

    /**
     * Update the city resources
     * @return void
     */
    public void UpdateCityData()
    {
        cashText.text = string.Format("Cash: ${0} (+${1})", cityInstance.Cash, cityInstance.CashPerTurn);
        populationText.text = string.Format("Population: {0}/{1} (+{2})", (int)cityInstance.CurrentPopulation, (int)cityInstance.MaxPopulation, (int)cityInstance.PopulationPerTurn);
        foodText.text = string.Format("Food: {0} (+{1})", (int)cityInstance.Food, (int)cityInstance.FoodPerTurn);
        jobsText.text = string.Format("Jobs: {0}/{1} (+{2})", cityInstance.CurrentJobs, cityInstance.MaxJobs, cityInstance.JobsPerTurn);
        deathText.text = string.Format("Death: {0}", (int)cityInstance.DeathPopulation);
        // Update the day along
        dayText.text = string.Format("Day {0}", cityInstance.Day);
    }

    /**
     * Update user total cash left
     * @return void
     */
    public void UpdateCash()
    {
        cashText.text = string.Format("Cash: ${0} (+${1})", cityInstance.Cash, cityInstance.CashPerTurn);
    }

    /**
     * Remove notification
     * @return void
     */
    public void DeleteNotification()
    {
        notificationBackground.gameObject.SetActive(false);
    }

    /**
     * Add new notification
     * @return void
     */
    public void AddNotification(NOTIFICATION_TYPES type, string message)
    {
        notificationBackground.gameObject.SetActive(true);
        notificationText.text = message;

        lastCheckNotificationTimer = notificationTimer;

        switch (type)
        {
            case NOTIFICATION_TYPES.ERROR:
                notificationText.color = Color.white;
                notificationBackground.color = Color.red;
                break;
            case NOTIFICATION_TYPES.WARNING:
                notificationText.color = Color.white;
                notificationBackground.color = Color.yellow;
                break;
            case NOTIFICATION_TYPES.INFO:
                notificationText.color = Color.white;
                notificationBackground.color = Color.blue;
                break;
            case NOTIFICATION_TYPES.SUCCESS:
                notificationText.color = Color.white;
                notificationBackground.color = Color.green;
                break;
            default:
                notificationText.color = Color.white;
                notificationBackground.color = Color.cyan;
                break;

        }
    }

    /**
     * On menu panel
     * @return void
     */
    public void OnMenuPanel()
    {
        if (GameManager.instance.HasSelectedBuilding)
        {
            return;
        }

        GameManager.instance.isMenuActive = !GameManager.instance.isMenuActive;

        menu.SetActive(GameManager.instance.isMenuActive);

        Time.timeScale = GameManager.instance.isMenuActive ? 0.0f : 1.0f;
    }

    /**
     * On restart button
     * @return void
     */
    public void OnQuitButton()
    {
        Application.Quit();
    }

    /**
     * On restart button
     * @return void
     */
    public void OnRestartButton()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Atlantice");
    }

    /**
     * On Menu Button
     * @return void
     */
    public void OnMenuButton()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
    }
}
