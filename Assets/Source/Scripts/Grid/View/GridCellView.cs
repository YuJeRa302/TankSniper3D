using Assets.Source.Scripts.Sound;
using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class GridCellView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly float _effectLifeTime = 1f;
        private readonly int _maxTankLevelMerge = 8;
        private readonly Color _defaultColor = new Color32(197, 197, 197, 255);
        private readonly Color _selectColor = Color.green;
        private readonly Color _blockedColor = Color.red;

        [SerializeField] private Renderer _cellRenderer;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private ParticleSystem _mergeEffect;

        private AudioPlayer _audioPlayer;
        private GridTankView _currentTank;
        private MaterialPropertyBlock _propertyBlock;

        public int Id { get; private set; }
        public bool IsOccupied { get; private set; } = false;

        public void Initialize(int id, AudioPlayer audioPlayer)
        {
            Id = id;
            _audioPlayer = audioPlayer;
        }

        public void SetMaterialColor(Color color)
        {
            _propertyBlock = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", color);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetOccupied(GridTankView gridTankView)
        {
            IsOccupied = true;
            _currentTank = gridTankView;
        }

        public void SetFree()
        {
            IsOccupied = false;
            _currentTank = null;
            Deselect();
        }

        public void Select(GridTankView newTank)
        {
            if (this != newTank.OriginalCell)
            {
                if (IsOccupied == true)
                {
                    if (newTank.Level == _currentTank.Level)
                        Highlighting(true, _selectColor);
                    else
                        Highlighting(true, _blockedColor);
                }
                else
                {
                    Highlighting(true, _selectColor);
                }
            }
        }

        public bool TryToMerge(GridTankView newTank)
        {
            if (_currentTank.Level == _maxTankLevelMerge)
                return false;

            if (_currentTank != newTank)
            {
                if (newTank.Level == _currentTank.Level)
                {
                    MergeTank(newTank);
                    return true;
                }
            }

            return false;
        }

        public void Deselect()
        {
            Highlighting(false, _defaultColor);
        }

        private void MergeTank(GridTankView newTank)
        {
            newTank.OriginalCell.SetFree();
            Deselect();

            var first = newTank.GridTankState;
            var second = _currentTank.GridTankState;
            Destroy(newTank.gameObject);
            Destroy(_currentTank.gameObject);

            Message.Publish(new M_ItemMerged(
                _currentTank.Level,
                this,
                first,
                second));

            _audioPlayer.PlayMergeTankAudioClip();
            CreateMergeEffect();
        }

        private void Highlighting(bool isOn, Color color)
        {
            _cellRenderer.gameObject.SetActive(isOn);
            _cellRenderer.material.color = color;
        }

        private void CreateMergeEffect()
        {
            var effect = Instantiate(_mergeEffect, transform.position, _mergeEffect.transform.rotation);
            Destroy(effect.gameObject, _effectLifeTime);
        }
    }
}