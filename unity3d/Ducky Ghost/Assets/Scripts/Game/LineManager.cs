using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class LineManager : MonoBehaviour
{
    public GameObject LinePrefab = null;
    public float Width = 0.1f;
    public float BlinkDuration = 0.06f;
    public int BlinkCount = 3;
    public float LineMargin = 0.1f;
    private static LineManager helper = null;

    public static LineManager SharedInstance()
    {
        return helper;
    }

    void Awake()
    {
        helper = this;
    }

    public void DrawLine(Vector3 position, List<LineVector> vectors)
    {
        for (int i = vectors.Count - 1; i >= 0; i--)
        {
            LineVector vector = vectors[i];
            GameObject line = Instantiate(LinePrefab) as GameObject;
            Transform tfm = line.transform;
            Vector3 size;
            
            switch (vector.direction)
            {
                case Direction.left:
                    tfm.localScale = new Vector3(vector.distance, Width);
                    size = line.renderer.bounds.size;
                    tfm.position = new Vector3(position.x - size.x / 2f, position.y);
                    position.x = tfm.position.x - size.x / 2f;
                    break;
                case Direction.right:
                    tfm.localScale = new Vector3(vector.distance, Width);
                    size = line.renderer.bounds.size;
                    tfm.position = new Vector3(position.x + size.x / 2f, position.y);
                    position.x = tfm.position.x + size.x / 2f;
                    break;
                case Direction.up:
                    tfm.localScale = new Vector3(Width, vector.distance);
                    size = line.renderer.bounds.size;
                    tfm.position = new Vector3(position.x, position.y + size.y / 2f);
                    position.y = tfm.position.y + size.y / 2f;
                    break;
                case Direction.down:
                    tfm.localScale = new Vector3(Width, vector.distance);
                    size = line.renderer.bounds.size;
                    tfm.position = new Vector3(position.x, position.y - size.y / 2f);
                    position.y = position.y - size.y;
                    break;
            }
            
            GameController.SharedInstance().IncreaseProgress();            
            Material mat = line.GetComponent<MeshRenderer>().material;

            Sequence seq = DOTween.Sequence();
            seq.SetLoops(BlinkCount);
            seq.OnComplete(() => 
            {
                OnBlinkComplete(line); 
            });
            seq.Append(mat.DOFade(0f, BlinkDuration));
            seq.Append(mat.DOFade(255f, BlinkDuration));
            seq.Play();
        }
    }
    
    void OnBlinkComplete(GameObject item)
    {
        Destroy(item);
        GameController.SharedInstance().DecreaseProgress();
        
        if (!GameController.SharedInstance().isProgressing)
        {
            if (!GameController.SharedInstance().CheckFinish())
            {
                if (!MoveManager.SharedInstance().MoveBlocks())
                {
                    MatrixController.SharedInstance().CheckRefreshMatrix();
                }
            }
        }
    }
}