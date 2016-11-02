using UnityEngine;
using System.Collections;

public class LevelMatrixController : MonoBehaviour
{
    public int ColumnCount = 5;
    public int RowCount = 5;
    public GameObject ItemPrefab = null;

    void Start()
    {    
        CreateMatrix();
    }

    void CreateMatrix()
    {
        Vector3 size = ItemPrefab.renderer.bounds.size;
        float x = -((size.x * ColumnCount) / 2f);
        float y = (size.y * RowCount) / 2f;
        float w = size.x / 2f;
        float h = size.y / 2f;
        float z = ItemPrefab.transform.position.z;
        int levelNumber = 1;
        int openedMaxLevelNumber = SettingsHelper.SharedInstance().GetOpenedMaxLevelNumber() + 1;

        for (int row = 0; row < RowCount; row++)
        {
            float xx = x;
            
            for (int col = 0; col < ColumnCount; col++)
            {
                GameObject item = Instantiate(ItemPrefab, new Vector3(xx + w, y - h, z), Quaternion.identity) as GameObject;
                LevelCellController controller = item.GetComponent<LevelCellController>();
                controller.levelNumber = levelNumber;
                controller.levelEnabled = controller.levelNumber <= openedMaxLevelNumber;
                controller.Refresh();
                controller.SetButtonOnClick(OnClick);
                xx += size.x;
                levelNumber++;
            }
            
            y -= size.y;
        }
    }

    void OnClick(GameObject sender)
    {
        LevelCellController controller = sender.GetComponent<LevelCellController>();

        if (controller.levelEnabled)
        {
            GameController.Scene(controller.levelNumber);
        }
    }
}