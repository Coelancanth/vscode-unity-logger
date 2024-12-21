using UnityEngine;
using System;

namespace Board.Core
{
    /// <summary>
    /// Represents a single tile in the grid
    /// </summary>
    public class Tile : MonoBehaviour
    {
        private ITileContent content;
        private Vector2Int gridPosition;
        private bool isSelected;

        // Events
        public event Action<ITileContent> OnContentChanged;
        public event Action<bool> OnSelectionChanged;
        public event Action<Vector2Int> OnPositionChanged;

        private void Awake()
        {
            ResetState();
        }

        /// <summary>
        /// Sets the content of the tile
        /// </summary>
        public void SetContent(ITileContent newContent)
        {
            // Remove old content
            if (content != null)
            {
                content.OnRemovedFromTile(this);
            }

            // Ensure content is never null by using EmptyTileContent
            content = newContent ?? EmptyTileContent.Instance;

            // Add new content
            content.OnAddedToTile(this);
            OnContentChanged?.Invoke(content);
        }
        
        public ITileContent GetContent()
        {
            // Ensure content is never null
            if (content == null)
            {
                SetContent(EmptyTileContent.Instance);
            }
            return content;
        }

        /// <summary>
        /// Called when the tile is added to the grid
        /// </summary>
        public void OnAddToGrid(Vector2Int position)
        {
            gridPosition = position;
            OnPositionChanged?.Invoke(position);
        }

        /// <summary>
        /// Called when the tile is removed from the grid
        /// </summary>
        public void OnRemoveFromGrid()
        {
            SetContent(EmptyTileContent.Instance);
            ResetState();
        }

        /// <summary>
        /// Sets the selection state of the tile
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (isSelected == selected) return;
            
            isSelected = selected;
            OnSelectionChanged?.Invoke(selected);
        }

        /// <summary>
        /// Resets the tile to its default state
        /// </summary>
        public void ResetState()
        {
            SetContent(EmptyTileContent.Instance);
            isSelected = false;
            gridPosition = Vector2Int.zero;
        }

        /// <summary>
        /// Gets the current grid position of the tile
        /// </summary>
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        /// <summary>
        /// Checks if the tile is currently selected
        /// </summary>
        public bool IsSelected()
        {
            return isSelected;
        }
    }
} 