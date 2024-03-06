using System.Linq;
using UnityEngine;

namespace ElementsGame.View
{
    public class ViewBlock : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _waterCellPrefab;
        [SerializeField] private SpriteRenderer _fireCellPrefab;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _destroyTime = 1.5f;
        [SerializeField] private float _sizeMultiplier = 1.2f;

        private int _id;
        private int _type;
        private Vector2Int _pos;
        private float _destroyEndTime = float.MinValue;
        private bool _isDestroying = false;

        public int Id => _id;
        public int Type => _type;
        public Vector2Int Pos => _pos;

        private Vector3 _targetPos;
        private SpriteRenderer _sRenderer;
        private Animator _animator;

        public bool IsUpdated => _isDestroying || _targetPos != transform.position;


        public void Init(int type, Vector2Int pos, float cellSize)
        {
            _type = type;
            _pos = pos;

            AdjustTypeView(cellSize);
        }

        private void AdjustTypeView(float cellSize)
        {

            SpriteRenderer prefab = null;
            if (_type == 1)
            {
                prefab = _waterCellPrefab;
            }
            else if (_type == 2)
            {
                prefab = _fireCellPrefab;
            }

            _sRenderer = Instantiate(prefab, transform);
            _animator = _sRenderer.GetComponent<Animator>();

            string name = _animator.GetCurrentAnimatorClipInfo(0).First().clip.name;
            _animator.Play(name, 0, Random.Range(0, 0.5f));

            float width = _sRenderer.sprite.bounds.size.x;
            float height = _sRenderer.sprite.bounds.size.y;
            float viewSize = Mathf.Max(width, height);
            float scaleFactor = cellSize / viewSize;
            _sRenderer.transform.localScale = Vector3.one * scaleFactor * _sizeMultiplier;
        }

        public void SetPos(Vector3 pos, int sortingOrder)
        {
            transform.position = pos;
            SetTargetPos(pos, sortingOrder);
        }

        public void SetTargetPos(Vector3 pos, int sortingOrder)
        {
            _targetPos = pos;
            _sRenderer.sortingOrder = sortingOrder;
        }

        public void DestroyBlock()
        {
            _isDestroying = true;
            _animator.SetTrigger("destroy");
            _destroyEndTime = Time.time + _animator.GetCurrentAnimatorClipInfo(0).Length * 1.5f; ;
        }

        private void Update()
        {
            if (_isDestroying && Time.time > _destroyEndTime)
            {
                _isDestroying = false;
                gameObject.SetActive(false);
                _animator.enabled = false;
            }

            float distance = (_targetPos - transform.position).magnitude;
            float t = _speed * Time.deltaTime / distance;
            Vector3 pos = Vector3.Lerp(transform.position, _targetPos, t);
            transform.position = pos;
        }
    }
}
