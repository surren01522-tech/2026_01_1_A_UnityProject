using UnityEngine;

public class DraggableRank : MonoBehaviour
{

    public int rankLevel = 1;                           //계급장 레벨 (0은 빈칸)
    public float dragSpeed = 30f;                       //드래그 시 이동 속도
    public float snapBackSpeed = 20f;                   //원 위치로 돌아가는 속도

    public bool isDragging = false;                     //현재 드리그 중인지 확인하는 변수 
    public Vector3 originalPosition;                    //원래 위치
    public GridCell currentCell;                        //현재 위치한 칸 

    public Camera mainCamera;                           //메인 카메라
    public Vector3 dragOffset;                          //드래그 시 오프셋 (보정값)
    public SpriteRenderer spriteRenderer;               //계급장 이미지 렌더러 
    public RankGameManager gameManager;


    private void Awake()
    {
        //필요한 컴포넌트 참조 가져오기
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindAnyObjectByType<RankGameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
        }
        else if(transform.position != originalPosition && currentCell != null)  //드래그가 끝낫는데 원래 위치로 돌아가야 하는 경우
        {
            transform.position = Vector3.Lerp(transform.position , originalPosition, snapBackSpeed * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        StopDragging();
    }

    void StartDragging() //드래그 시작
    {
        isDragging = true;                                          //드래그 상태로 설정
        dragOffset = transform.position - GetMouseWorldPosition();  //마우스와 계급장 위치 차이 계산
        spriteRenderer.sortingOrder = 10;                           //드래그 시작 시 계급장을 앞으로 보내기
    }

    void StopDragging()     //드래그 종료
    {
        isDragging = false;
        spriteRenderer.sortingLayerID = 1;
        GridCell targetCell = gameManager.FindClosestCell(transform.position);      //가장 가까운 칸 찾기 

        if (targetCell != null )
        {
            if(targetCell.currentRank == null)          //빈칸인 경우 -> 이동
            {
                MoveToCell(targetCell);
            }
            else if(targetCell.currentRank != this && targetCell.currentRank.rankLevel == rankLevel)    //같은 랭크일 경우 마지
            {
                MergeWithCell(targetCell);
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }
        else
        {
            ReturnToOriginalPosition();                 //유효한 칸이 없으면 위치로 복귀
        }
    }

    public void MoveToCell(GridCell targetCell)         //특정 칸으로 이동
    {
        if (currentCell != null)
        {
            currentCell.currentRank = null;             //기존 칸에서 제거
        }

        currentCell = targetCell;                       //새 칸으로 이동
        targetCell.currentRank = this;

        originalPosition = new Vector3(targetCell.transform.position.x, targetCell.transform.position.y, 0f);
        transform.position = originalPosition;

    }

    public void ReturnToOriginalPosition()              //원래 위치로 돌아가는 함수
    {
        transform.position = originalPosition;
    }

    public void MergeWithCell(GridCell targetCell)
    {
        if(targetCell.currentRank == null || targetCell.currentRank.rankLevel != rankLevel) //다른 레벨이거나 비어있다면
        {
            ReturnToOriginalPosition();         //원래 위치로 돌아가기 
            return;
        }

        if (currentCell != null)
        {
            currentCell.currentRank = null;         //기존 칸에서 제거
        }

        //합치기 실행 MergeRanke 함수를 통해서 실행 
        gameManager.MergeRanks(this, targetCell.currentRank);
    }

    public Vector3 GetMouseWorldPosition()                          //마우스 월드 좌표 구하기 
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void SetRankLevel(int level)
    {
        rankLevel = level;

        if(gameManager != null && gameManager.rankSprites.Length > level - 1)
        {
            spriteRenderer.sprite = gameManager.rankSprites[level - 1];                 //레벨에 맞는 스프라이트로 변경

        }
    }
}
