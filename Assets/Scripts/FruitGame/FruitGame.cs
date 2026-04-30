using UnityEngine;

public class FruitGame : MonoBehaviour
{

    public GameObject[] fruitPerfabs;                                                        //과일 프리맵 배열 선언 
    public float[] fruitSize = { 0.5f, -0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f  };         //과일 크기 선언 

    public GameObject currentFruit;                                                           //
    public int currentFruitType;

    public float fruitStartHeigt = 6.0f;
    public float gameWidth = 6.0f;
    public bool isGameOver = false;
    public Camera mainCamera;

    public float fruitTimer;                                           //잰 시간 설정을 위한 타이머




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;                   //메인 카메라 참조 가져오기
        SpawnnewFruit();                           //
        fruitTimer = -3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        if (fruitTimer >= 0)
        {
            fruitTimer -= Time.deltaTime;
        }

        if (fruitTimer < 0 && fruitTimer > -2)                  //타이머 시간이 0과 -2 사이에 있을 때 잰 함수를 호출하고 다른 시간대로 보낸다.
        {
            SpawnnewFruit();
            fruitTimer = -3.0f;                                 //타이머 시간을 -3으로 보낸다.
        }

        if (currentFruit != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 newPosition = currentFruit.transform.position;                       //과일 위치 업데이트

            newPosition.x = worldPosition.x;

            float halfFruitSize = fruitSize[currentFruitType] / 2f;

            if(newPosition.x < -gameWidth / 2 - halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 - halfFruitSize;
            }

            if (newPosition.x > gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = gameWidth / 2 + halfFruitSize;
            }

            currentFruit.transform.position = newPosition;                                   //과일 좌표 갱신
        }

        if (Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)                             //마우스 좌 클릭하면 과일을 떨어뜨린다.
        {
            DropFruit();
        }
    }

    //과이ㄹ 생ㅅㅓㅇ 함ㅅㅜ
    void SpawnnewFruit()
    {
        if(!isGameOver)
        {

            currentFruitType = Random.Range(0, 3);

            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 spawnPosition = new Vector3(worldPosition.x, fruitStartHeigt, 0);

            float halfFruitSize = fruitSize[currentFruitType] / 2f;

            //
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 - halfFruitSize);

            currentFruit = Instantiate(fruitPerfabs[currentFruitType], spawnPosition, Quaternion.identity);
            currentFruit.transform.localScale = new Vector3(fruitSize[currentFruitType], fruitSize[currentFruitType], 1);

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.gravityScale = 0.0f;
            }
        }
    }

    //과일을 떨어뜨리는 함수 구현
    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.0f;
            currentFruit = null;
            fruitTimer = 1.0f;
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if(fruitType < fruitPerfabs.Length -1)
        {
            GameObject newFruit = Instantiate(fruitPerfabs[fruitType + 1] , position, Quaternion.identity);
            newFruit.transform.localScale = new Vector3(fruitSize[fruitType + 1], fruitSize[fruitType + 1], 1.0f);

            //점수 추가 로직 등등
        }
    }
}
