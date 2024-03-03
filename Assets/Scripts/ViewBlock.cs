using UnityEngine;

public class ViewBlock : MonoBehaviour 
{
    [SerializeField] private GameObject _waterCellPrefab;
    [SerializeField] private GameObject _fireCellPrefab;
    [SerializeField] private float _speed = 5f;

    private int _id;
    private int _type;
    private Vector2Int _pos;
    
    public int Id => _id;
    public int Type => _type;
    public Vector2Int Pos => _pos;

    private Vector3 _targetPos;

    public bool IsUpdated => _targetPos != transform.position;

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
        _targetPos = pos;
    }

    public void SetTargetPos(Vector3 pos){
        _targetPos = pos;
    }

    private void Update(){
        float distance = (_targetPos - transform.position).magnitude;
        float t = _speed * Time.deltaTime / distance;
        Vector3 pos = Vector3.Lerp(transform.position, _targetPos, t);
        transform.position = pos;
    }
}