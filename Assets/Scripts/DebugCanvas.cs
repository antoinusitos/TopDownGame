using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{
    public RectTransform        myBiomeRectTransform = null;
    public Transform            myBiomeCellPrefab = null;
    public GridLayoutGroup      myBiomeGridLayoutGroup = null;

    public RectTransform        myMapRectTransform = null;
    public Transform            myMapCellPrefab = null;
    public GridLayoutGroup      myMapGridLayoutGroup = null;

    private Transform           myTransform = null;
    private int                 myWorldSideSize = 0;
    private int                 myBiomeSideSize = 0;

    public void Init(int aWorldSideSize, int aBiomeSideSize)
    {
        myTransform = transform;
        myWorldSideSize = aWorldSideSize;
        myBiomeSideSize = aBiomeSideSize;

        myMapGridLayoutGroup.cellSize = Vector2.one * (myMapRectTransform.rect.width / aWorldSideSize);

        for (int i = 0; i < aWorldSideSize; i++)
        {
            for (int j = 0; j < aWorldSideSize; j++)
            {
                Instantiate(myMapCellPrefab, myMapGridLayoutGroup.transform);
            }
        }

        myBiomeGridLayoutGroup.cellSize = Vector2.one * (myBiomeRectTransform.rect.width / aBiomeSideSize);

        for(int i = 0; i < aBiomeSideSize; i++)
        {
            for (int j = 0; j < aBiomeSideSize; j++)
            {
                Instantiate(myBiomeCellPrefab, myBiomeGridLayoutGroup.transform);
            }
        }
    }

    public void HighlightMap(int aX, int aY)
    {
        myMapRectTransform.GetChild(aY * myWorldSideSize + aX).GetComponent<Image>().color = Color.white;
    }

    public void SelectMap(int aX, int aY)
    {
        myMapRectTransform.GetChild(aY * myWorldSideSize + aX).GetComponent<Image>().color = Color.green;
    }

    public void HighlightBiome(int aX, int aY)
    {
        myBiomeRectTransform.GetChild(aY * myBiomeSideSize + aX).GetComponent<Image>().color = Color.white;
    }

    public void ClearHighlightBiome()
    {
        for(int i = 0; i < myBiomeRectTransform.childCount; i++)
        {
            myBiomeRectTransform.GetChild(i).GetComponent<Image>().color = Color.grey;
        }

    }
}
