using UnityEngine;

/// <summary>
/// Camera controller.
/// - Adjusts the main camera to fit the entire board. 
/// - Calculates size and position based on rows, columns, cell size, and spacing.
/// - Applies extra padding on X and Y to avoid cutting edges.
/// </summary>
public class CameraController
{
    private Camera _camera;
    private float _paddingX;
    private float _paddingY;

    /// <summary>
    /// Constructor: receives the camera and padding values.
    /// </summary>
    public CameraController(Camera camera, float paddingX = 1f, float paddingY = 1f)
    {
        _camera = camera;
        _paddingX = paddingX;
        _paddingY = paddingY;
    }

    /// <summary> 
    /// Fits the camera to the board:
    /// - Calculates board width and height from rows, columns, cell size, and spacing.
    /// - Applies padding on both axes.
    /// - Adjusts orthographicSize so the entire board is visible.
    /// - Centers the camera.
    /// </summary>
    public void FitCameraToBoard(int rows, int columns, float cellSize, float spacing)
    {
        float boardWidth = columns * (cellSize + spacing);
        float boardHeight = rows * (cellSize + spacing); 

        float paddedWidth = boardWidth + _paddingX * 2f;
        float paddedHeight = boardHeight + _paddingY * 2f;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetSize = Mathf.Max(paddedHeight / 2f, paddedWidth / 2f / screenRatio);

        float centerX = 0;
        float centerY = 0;

        _camera.orthographicSize = targetSize;
        _camera.transform.position = new Vector3(centerX, centerY, -10f);
    }
}