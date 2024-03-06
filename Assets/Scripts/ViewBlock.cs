using UnityEngine;

public class ViewBlock : MonoBehaviour 
{
    [SerializeField] private SpriteRenderer _waterCellPrefab;
    [SerializeField] private SpriteRenderer _fireCellPrefab;
    [SerializeField] private GameObject _destroySign;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _destroyTime = 1.5f;

    private int _id;
    private int _type;
    private Vector2Int _pos;
    private float _destroyEndTime = float.MinValue;
    private bool _isDestroying = false;
    
    public int Id => _id;
    public int Type => _type;
    public Vector2Int Pos => _pos;

    private Vector3 _targetPos;

    public bool IsUpdated => _isDestroying || _targetPos != transform.position;

    public void Init(int type, Vector2Int pos, float cellSize)
    {
        _type = type;
        _pos = pos;
        _destroySign.SetActive(false);

        AdjustTypeView(cellSize);
    }

    private void AdjustTypeView(float cellSize){

        SpriteRenderer prefab = null;
        if (_type == 1)
        {
            prefab = _waterCellPrefab;
        }
        else if (_type == 2)
        {
            prefab = _fireCellPrefab;
        }

        SpriteRenderer sRenderer = Instantiate(prefab, transform);

        float width = sRenderer.size.x;
        float height = sRenderer.size.y;
        float viewSize = Mathf.Max(width, height);
        float scaleFactor = cellSize / viewSize;
        sRenderer.transform.localScale = Vector3.one * scaleFactor;
    }

    public void SetPos(Vector3 pos){
        transform.position = pos;
        _targetPos = pos;
    }

    public void SetTargetPos(Vector3 pos){
        _targetPos = pos;
    }

    public void TurnOff()
    {
        _destroyEndTime = Time.time + _destroyTime;
        _isDestroying = true;
        _destroySign.SetActive(true);
    }

    private void Update(){
        if(_isDestroying && Time.time > _destroyEndTime)
        {
            _isDestroying = false;
            gameObject.SetActive(false);
        }

        float distance = (_targetPos - transform.position).magnitude;
        float t = _speed * Time.deltaTime / distance;
        Vector3 pos = Vector3.Lerp(transform.position, _targetPos, t);
        transform.position = pos;
    }
}