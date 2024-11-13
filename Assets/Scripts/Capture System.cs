using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CaptureSystem : MonoBehaviour
{
    // This is Profile Image
    public RawImage ProfileImage;

    // Drag UI
    public RectTransform AreaLineRectTransform;

    // Rect to contain the drag size
    private Rect _selectArea;

    // Initial position variable when dragging
    private Vector2 _startPosition = Vector2.zero;

    /// <summary>
    /// Drag UI object
    /// </summary>
    public GameObject AreaLineObject => AreaLineRectTransform.gameObject;

    private void Start()
    {
        //선택 UI 옵젝의 앵커 포인터를 왼쪽 하단으로 변경함
        AreaLineRectTransform.pivot = new Vector2(0, 0);
        AreaLineRectTransform.anchorMin = new Vector2(0, 0);
        AreaLineRectTransform.anchorMax = new Vector2(0, 0);
    }

    private void Update()
    {
        Drag();
        Capture();
    }

    private async void Capture()
    {
        // Press the space bar to start capturing.
        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;


        
        // Check if the drag UI is active
        if (AreaLineObject.activeSelf)
        {
            // Disable drag UI
            AreaLineObject.SetActive(false);

            // Wait for the drag UI to disappear
            await Awaitable.WaitForSecondsAsync(0.01f);
            
            // Wait for the End of Frame
            await Awaitable.EndOfFrameAsync();
            
            // Calculate the capture area.
            Vector2 captureAreaMin = AreaLineRectTransform.anchoredPosition;
            Vector2 captureAreaMax = AreaLineRectTransform.anchoredPosition;

            // Set the cut size horizontally and vertically in the Select Box.
            Vector2Int cutSize = Vector2Int.zero;
            cutSize.x = (int)AreaLineRectTransform.sizeDelta.x;
            cutSize.y = (int)AreaLineRectTransform.sizeDelta.y;

            captureAreaMax.x += cutSize.x;
            captureAreaMax.y += cutSize.y;

            // Creating textures statically
            Texture2D texture = new Texture2D(cutSize.x, cutSize.y, TextureFormat.RGB24, false);

            // Adds pixels to the screen
            texture.ReadPixels(new Rect(captureAreaMin.x, captureAreaMin.y, captureAreaMax.x, captureAreaMax.y), 0, 0,
                false);
            
            // Apply
            texture.Apply();

            // Apply the texture to the profile image
            ProfileImage.texture = texture;

            // Set the size of the profile image to the size of the cut. (Optional)
            ProfileImage.rectTransform.sizeDelta = cutSize;

            // Show Drag UI
            AreaLineObject.SetActive(true);
        }
        else
            Debug.Log("Please cast drag");
    }

    /// <summary>
    /// Function to create a selection area by dragging
    /// </summary>
    private void Drag()
    {
        // Get Mouse Position
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Click Left Mouse Button
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AreaLineRectTransform.gameObject.SetActive(true);
            _startPosition = mousePosition;
        }

        // Dragging
        if (Mouse.current.leftButton.isPressed)
        {
            if (mousePosition.x > _startPosition.x)
            {
                // If the mouse moves to the right from where you first clicked
                _selectArea.xMin = _startPosition.x;
                _selectArea.xMax = mousePosition.x;
            }
            else
            {
                // If the mouse moves to the left from where you first clicked
                _selectArea.xMin = mousePosition.x;
                _selectArea.xMax = _startPosition.x;
            }

            if (mousePosition.y > _startPosition.y)
            {
                // When the mouse moves up from where you first clicked
                _selectArea.yMin = _startPosition.y;
                _selectArea.yMax = mousePosition.y;
            }
            else
            {
                // If the mouse moves down from where you first clicked
                _selectArea.yMin = mousePosition.y;
                _selectArea.yMax = _startPosition.y;
            }

            // Tip : If ReadPixels leaves the rendering area when dragged, the editor explodes.
            if (_selectArea.xMin < 0)
                _selectArea.xMin = 0;

            if (_selectArea.yMin < 0)
                _selectArea.yMin = 0;

            if (_selectArea.xMax > Screen.width)
                _selectArea.xMax = Screen.width;

            if (_selectArea.yMax > Screen.height)
                _selectArea.yMax = Screen.height;

            // Apply Select Area
            AreaLineRectTransform.offsetMin = _selectArea.min;
            AreaLineRectTransform.offsetMax = _selectArea.max;
        }
    }
}