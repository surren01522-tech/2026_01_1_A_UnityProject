using Unity.VisualScripting;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitType;                             //과일 index 설정 (0: 사과, 1:블루베리, 2:코코넛)
    public bool hasMerged = false;                    //과일이 합쳐졌는지 확인하는 플래그

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMerged)
            return;                                   //이미 합쳐진 과일은 무시

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();             //다른 과일과 충돌 했는지 확인

        if (otherFruit != null && !otherFruit.hasMerged && otherFruit.fruitType == fruitType)         //충돌한 것이 과일이고 타일빙 같으면 (합쳐지지 않았을 경우)
        {
            hasMerged = true;                                          //합쳐짐 표시
            otherFruit.hasMerged = true;                               //합쳐짐 표시

            Vector3 mergePosition = (transform.position + otherFruit.transform.position) / 2f;              //두 과일의 중간 위치 계산

            //
            FruitGame gameManager = FindAnyObjectByType<FruitGame>();

            if (gameManager != null)
            {
                gameManager.MergeFruits(fruitType, mergePosition);
            }

            //과일들 제거
            Destroy(otherFruit.gameObject);
            Destroy(gameObject);
        }
    }

}
