using UnityEngine;

public class ViewBlock : MonoBehaviour 
{
    [SerializeField] private GameObject _waterCellPrefab;
    [SerializeField] private GameObject _fireCellPrefab;

    private int _id;
    private int _type;
    private Vector2Int _pos;
    
    public int Id => _id;
    public int Type => _type;
    public Vector2Int Pos => _pos;

    public void Init(int type, Vector2Int pos)
    {
        _type = type;
        _pos = pos;

        // temp
        GameObject prefab = null;
        if(_type == 1){
            prefab = _waterCellPrefab;
        }
        else if (_type == 2){
            prefab = _fireCellPrefab;
        }
        Instantiate(prefab, transform);
    }

    public void SetPos(Vector3 pos){
        transform.position = pos;
    }
}