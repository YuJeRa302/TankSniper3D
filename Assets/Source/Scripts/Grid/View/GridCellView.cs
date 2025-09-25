using UniRx;
using UnityEngine;

namespace Assets.Source.Scripts.Grid
{
    public class GridCellView : MonoBehaviour
    {
        public static readonly IMessageBroker Message = new MessageBroker();

        private readonly Color _defaultColor = new Color32(197, 197, 197, 255);
        private readonly Color _selectColor = Color.green;
        private readonly Color _blockedColor = Color.red;

        [SerializeField] private Renderer _cellRenderer;

        private GridTankView _currentItem;

        public bool IsOccupied { get; private set; } = false;

        public void SetOccupied(GridTankView gridItemView)
        {
            IsOccupied = true;
            _currentItem = gridItemView;
        }

        public void SetFree()
        {
            IsOccupied = false;
            _currentItem = null;
            Deselect();
        }

        public void Select(GridTankView newItem)
        {
            if (this != newItem.OriginalCell)
            {
                if (IsOccupied == true)
                {
                    if (newItem.Level == _currentItem.Level)
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

        public bool TryToMerge(GridTankView newItem)
        {
            if (_currentItem != newItem)
            {
                if (newItem.Level == _currentItem.Level)
                {
                    MergeTank(newItem);
                    return true;
                }
            }

            return false;
        }

        public void Deselect()
        {
            Highlighting(false, _defaultColor);
        }

        private void MergeTank(GridTankView newItem)
        {
            newItem.OriginalCell.SetFree();
            Deselect();
            Destroy(newItem.gameObject);
            Destroy(_currentItem.gameObject);
            Message.Publish(new M_ItemMerged(_currentItem.Level, this));
        }

        private void Highlighting(bool isOn, Color color)
        {
            _cellRenderer.gameObject.SetActive(isOn);
            _cellRenderer.material.color = color;
        }
    }
}