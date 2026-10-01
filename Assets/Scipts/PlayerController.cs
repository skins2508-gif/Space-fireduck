using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    private float score = 0f;
    public GameObject bounceEffectPrefab;
    public float scoreMultiplier = 10f;
    public float fireScorePerObstacle = 100f;
    public float invincibleDuration = 3f;
    public float thrustForce;
    public float normalThrustForce = 50f;
    public float fireThrustForce = 100f;
    public float screenPadding = 0.8f;
    public Sprite idleChicken;
    public Sprite flyChicken;
    public Sprite deadChicken;
    public Sprite fireChicken;
    public Sprite buldakNoodle;
    private SpriteRenderer sr;
    Rigidbody2D rb;
    public UnityEngine.UI.Slider slowGaugeSlider;
    public UIDocument uiDocument;
    private Label scoreText;
    private UnityEngine.UIElements.Button restartButton;
    private bool isDead = false;
    private bool isFireChicken = false;
    private bool isInvincible = false;
    private float invincibleEndTime = 0f;
    private Sprite currentCharacterSprite;
    public GameObject explosionEffect;
    private Label highScoreText;
    private int highScore;
    [Header("Slow Mode")]
    public float slowTimeScale = 0.3f;
    public float maxSlowGauge = 100f;
    public float slowGauge = 100f;
   
    public float slowDrainPerSecond = 10f;   // 1초에 10 감소
    public float slowRecoverPerSecond = 20f; // 1초에 20 회복
    void Start()
        {
        // Always begin with a completely filled slow gauge.
        slowGauge = maxSlowGauge;
        thrustForce = normalThrustForce;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
   
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<UnityEngine.UIElements.Button>("RestartButton");

        if (restartButton != null)
        {
            restartButton.style.display = DisplayStyle.None;
            restartButton.clicked += ReloadScene;
        }

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScoreText != null)
        {
            highScoreText.text = "High Score : " + highScore;
            highScoreText.style.display = DisplayStyle.None;
        }

        currentCharacterSprite = idleChicken;

        // The slow gauge starts full, so initialise the UI before the first frame.
        // This also prevents the slider from briefly appearing empty when the scene loads.
        UpdateSlowGaugeUI();
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (isInvincible && Time.time >= invincibleEndTime)
        {
            isInvincible = false;
            isFireChicken = false;
            thrustForce = normalThrustForce;
        }

        scoreText.text = "Score: " + score;

        // 좌클릭 이동
        if (Mouse.current.leftButton.isPressed)
        {
            sr.sprite = GetFlyingSprite();

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            transform.up = direction;
            rb.AddForce(direction * thrustForce);
        }
        else
        {
            sr.sprite = GetIdleSprite();
        }

        // 우클릭 중 슬로우 모드 사용
        if (Mouse.current.rightButton.isPressed)
        {
            if (slowGauge > 0f)
            {
                Time.timeScale = slowTimeScale;
                slowGauge -= slowDrainPerSecond * Time.unscaledDeltaTime;
            }

            if (slowGauge <= 0f)
            {
                slowGauge = 0f;
                Time.timeScale = 1f;
            }
        }
        else
        {
            Time.timeScale = 1f;

            slowGauge += slowRecoverPerSecond * Time.unscaledDeltaTime;
        }

        slowGauge = Mathf.Clamp(slowGauge, 0f, maxSlowGauge);

        UpdateSlowGaugeUI();

        if (score > highScore)
        {
            highScore = Mathf.FloorToInt(score);
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }




    void OnCollisionEnter2D(Collision2D collision)
        {
        SideFood sideFood = collision.gameObject.GetComponentInParent<SideFood>();
        if (sideFood != null)
        {
            EatSideFood(sideFood, collision);
            return;
        }

        FireSauce fireSauce = collision.gameObject.GetComponentInParent<FireSauce>();
        if (fireSauce != null)
        {
            EatFireSauce(collision.gameObject);
            return;
        }

        ColdMeteor coldMeteor = collision.gameObject.GetComponentInParent<ColdMeteor>();
        if (coldMeteor != null)
        {
            if (isInvincible)
            {
                Destroy(collision.gameObject);
                return;
            }

            GameOver(collision);
            return;
        }

        Obstacles obstacle = collision.gameObject.GetComponentInParent<Obstacles>();
        if (obstacle == null)
            return;

        score += fireScorePerObstacle;
        scoreText.text = "Score: " + score;

        if (score > highScore)
        {
            highScore = Mathf.FloorToInt(score);
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            highScoreText.text = "High Score : " + highScore;
        }

        ObstacleSpawner obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
        if (obstacleSpawner != null)
            obstacleSpawner.OnFireMeteorEaten();

        Destroy(collision.gameObject);

    }

    void UpdateSlowGaugeUI()
    {
        if (slowGaugeSlider == null)
            return;

        slowGaugeSlider.minValue = 0f;
        slowGaugeSlider.maxValue = Mathf.Max(0.01f, maxSlowGauge);
        slowGaugeSlider.value = Mathf.Clamp(slowGauge, 0f, slowGaugeSlider.maxValue);

        // Keep every visual part on the same full-width rect. This fixes legacy
        // slider layouts whose Fill Area was wider than the Slider itself.
        RectTransform sliderRect = slowGaugeSlider.GetComponent<RectTransform>();
        if (sliderRect != null)
            sliderRect.sizeDelta = new Vector2(240f, 18f);

        RectTransform fillRect = slowGaugeSlider.fillRect;
        if (fillRect != null)
        {
            // Slider geometry can leave a thin edge at zero. Disable Fill itself
            // so a completely spent gauge has no red pixels remaining.
            fillRect.gameObject.SetActive(slowGauge > 0.001f);

            if (fillRect.parent is RectTransform fillArea)
            {
                fillArea.anchorMin = Vector2.zero;
                fillArea.anchorMax = Vector2.one;
                fillArea.anchoredPosition = Vector2.zero;
                fillArea.sizeDelta = Vector2.zero;
            }
        }

        RectTransform handleRect = slowGaugeSlider.handleRect;
        if (handleRect != null && handleRect.parent is RectTransform handleArea)
        {
            handleArea.anchorMin = Vector2.zero;
            handleArea.anchorMax = Vector2.one;
            handleArea.anchoredPosition = Vector2.zero;
            handleArea.sizeDelta = Vector2.zero;
        }
    }

    void LateUpdate()
    {
        if (isDead)
            return;

        ClampPlayerToCamera();
    }

    void EatFireSauce(GameObject sauceObject)
    {
        isFireChicken = true;
        isInvincible = true;
        invincibleEndTime = Time.time + invincibleDuration;
        currentCharacterSprite = fireChicken;
        thrustForce = fireThrustForce;
        if (fireChicken != null)
            sr.sprite = fireChicken;
        
        Destroy(sauceObject);
    }

    void EatSideFood(SideFood sideFood, Collision2D collision)
    {
        if (isInvincible)
        {
            Destroy(collision.gameObject);
            return;
        }
        Sprite deathSprite = null;

        if (sideFood.foodType == SideFoodType.BuldakCup && buldakNoodle != null)
        {
            currentCharacterSprite = buldakNoodle;
            deathSprite = buldakNoodle;
        }
        else if (sideFood.foodType == SideFoodType.ChickenRadish && deadChicken != null)
        {
            currentCharacterSprite = deadChicken;
            deathSprite = deadChicken;
        }

        
        sr.sprite = currentCharacterSprite;

        Destroy(collision.gameObject);
        GameOver(collision, deathSprite);
    }

    Sprite GetFlyingSprite()
    {
        if (isFireChicken && fireChicken != null)
            return fireChicken;

        if (flyChicken != null)
            return flyChicken;

        return GetIdleSprite();
    }

    Sprite GetIdleSprite()
    {
        if (isFireChicken && fireChicken != null)
            return fireChicken;

        if (currentCharacterSprite != null)
            return currentCharacterSprite;

        return idleChicken;
    }

    void GameOver(Collision2D collision, Sprite deathSprite = null)
    {
        isDead = true;
        sr.sprite = deathSprite != null ? deathSprite : deadChicken;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Vector2 contactPoint = collision.GetContact(0).point;

        if (bounceEffectPrefab != null)
            Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, transform.rotation);

        if (restartButton != null)
            restartButton.style.display = DisplayStyle.Flex;
        if (highScoreText != null)
        {
            highScoreText.text = "High Score : " + highScore;
            highScoreText.style.display = DisplayStyle.Flex;
        }

        Destroy(gameObject, 20f);
    }
    

    void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }

    void ClampPlayerToCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        Vector3 cameraPosition = mainCamera.transform.position;
        float halfHeight = Mathf.Max(0f, mainCamera.orthographicSize - screenPadding);
        float halfWidth = Mathf.Max(0f, mainCamera.orthographicSize * mainCamera.aspect - screenPadding);
        Vector3 position = transform.position;

        float clampedX = Mathf.Clamp(position.x, cameraPosition.x - halfWidth, cameraPosition.x + halfWidth);
        float clampedY = Mathf.Clamp(position.y, cameraPosition.y - halfHeight, cameraPosition.y + halfHeight);

        if (Mathf.Approximately(position.x, clampedX) && Mathf.Approximately(position.y, clampedY))
            return;

        transform.position = new Vector3(clampedX, clampedY, position.z);

        if (rb == null)
            return;

        Vector2 velocity = rb.linearVelocity;
        if (!Mathf.Approximately(position.x, clampedX))
            velocity.x = 0f;
        if (!Mathf.Approximately(position.y, clampedY))
            velocity.y = 0f;

        rb.linearVelocity = velocity;
    }

}
