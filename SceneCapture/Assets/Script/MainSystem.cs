using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Camera))]
public class MainSystem : MonoBehaviour
{
    //캡쳐를 할것인지 말것인지 정하는 트리거 변수
    bool Captuer;

    //프로필 이미지
    public RawImage ProfileImage;

    //드래그 ui
    public RectTransform AreaLine;

    //드래그 사이즈를 담을 Rect
    Rect selectArea = new Rect();

    //드래그를 시전시 초기 위치 변수
    Vector2 startpos = Vector2.zero;

    private void Start()
    {
        init();
    }

    private void init()
    {
        //선택 UI 옵젝의 앵커 포인터를 왼쪽 하단으로 변경함
        AreaLine.pivot = new Vector2(0, 0);
        AreaLine.anchorMin = new Vector2(0, 0);
        AreaLine.anchorMax = new Vector2(0, 0);
    }

    private void Update()
    {
        #region 캡쳐
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartCaptuer());
        }
        #endregion

        DragSystem();
    }

    private void OnPreRender()
    {
        //캡쳐 신호가 온다면..?
        if (Captuer)
        {
                Vector2 captuerAreaMin = AreaLine.anchoredPosition;
                Vector2 captuerAreaMax = AreaLine.anchoredPosition;
                #region 컷 영역
                //컷 사이즈를 Sellect Box의 가로 세로로 잡아준다.
                Vector2Int cutSize = Vector2Int.zero;
                cutSize.x = (int)AreaLine.sizeDelta.x;
                cutSize.y = (int)AreaLine.sizeDelta.y;
                #endregion
                captuerAreaMax.x += cutSize.x;
                captuerAreaMax.y += cutSize.y;

                //정적으로 텍스쳐를 만듬
                Texture2D texture = new Texture2D(cutSize.x, cutSize.y, TextureFormat.RGB24, false);

                //화면의 픽셀을 넣어줌
                texture.ReadPixels(new Rect(captuerAreaMin.x, captuerAreaMin.y, captuerAreaMax.x, captuerAreaMax.y), 0, 0, false);
                texture.Apply();

                //얻은 텍스쳐를 입힘.
                ProfileImage.texture = texture;
                //이건.. 드래그 된 사이즈를 반영하는 파트인데.. 의미는 크게 없다.
                ProfileImage.rectTransform.sizeDelta = cutSize;

                //드래그 ui 표시
                AreaLine.gameObject.SetActive(true);
            //캡쳐 끝
            Captuer = false;
        }
    }

    IEnumerator StartCaptuer()
    {
        //코루틴을 사용하는 이유는 화면을 캡쳐할 때 AreaLine이 영역도 같이 캡쳐 되어버린다.
        //왜냐면 OnPreRender는 렌더링을 모두 끝낸 후 실행 되기 때문. 그러므로 드래그를 했는지
        //않했는지 체크 한 후, 드래그를 했다면 AreaLine을 비활성화 한 후 캡쳐를 시도한다.
        if (AreaLine.gameObject.activeSelf == true)
        {
            AreaLine.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.01f);
            Captuer = true;
        }
        else
        {
            print("드래그를 시전 해주세요");
        }

    }

    private void DragSystem()
    {
        #region 처음 클릭한 좌표를 받아옴
        if (Input.GetMouseButtonDown(0))
        {
            AreaLine.gameObject.SetActive(true);
            startpos = Input.mousePosition;
        }
        #endregion

        #region 드래그 중이라면..?
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x > startpos.x)
            {
                //처음 클릭한 곳에서 마우스가 오른쪽으로 이동할 경우
                selectArea.xMin = startpos.x;
                selectArea.xMax = Input.mousePosition.x;
            }
            else
            {
                //처음 클릭한 곳에서 마우스가 왼쪽으로 이동할 경우
                selectArea.xMin = Input.mousePosition.x;
                selectArea.xMax = startpos.x;
            }

            if (Input.mousePosition.y > startpos.y)
            {
                //처음 클릭한 곳에서 마우스가 위로 이동할 경우
                selectArea.yMin = startpos.y;
                selectArea.yMax = Input.mousePosition.y;
            }
            else
            {

                //처음 클릭한 곳에서 마우스가 아래로 이동할 경우
                selectArea.yMin = Input.mousePosition.y;
                selectArea.yMax = startpos.y;
            }

            #region Safe Area
            //ReadPixels매서드는 카메라가 렌더링 되는 부분을 캡쳐하는 함수이다.
            //만일 드래그 했을 때 렌더링 영역을 나가면 해당 프로그램(유니티 에디터)는 터진다.
            if (selectArea.xMin < 0)
            {
                selectArea.xMin = 0;
            }

            if (selectArea.yMin < 0)
            {
                selectArea.yMin = 0;
            }

            if (selectArea.xMax > Screen.width)
            {
                selectArea.xMax = Screen.width;
            }

            if (selectArea.yMax > Screen.height)
            {
                selectArea.yMax = Screen.height;
            }
            #endregion

            //사이즈 반영
            AreaLine.offsetMin = selectArea.min;
            AreaLine.offsetMax = selectArea.max;
        }
        #endregion
    }

}
