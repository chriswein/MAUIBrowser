using System.Diagnostics;
using System.Text;

namespace MAUI_App.Utilities
{
    internal class HistoryItem
    {
        public HistoryItem? Previous { get; set; }
        public HistoryItem? Next { get; set; }
        public string Url { get; init; }

        public HistoryItem(string url)
        {
            Url = url;
        }
        public void Append(HistoryItem newItem)
        {
            if (this.Next != null) // So the gc can dispose of it.
            {
                this.Next.Previous = null;
                this.Next = null;
            }
            // Establish the forward link
            this.Next = newItem;

            // Establish the backward link
            newItem.Previous = this;
        }

        public bool HasNext() => Next != null;

        public bool IsFirst() => Previous == null;
    }
    internal class HistoryManager
    {
        private HistoryItem? _current;
        private int _ignoreNextAdded = 0;
        private enum Direction
        {
            LEFT, RIGHT
        }
        public void AddNewUrl(string url)
        {
            try { var u = new Uri(url);  }
            catch { return; /* Not a valid url */ }
            if (url.Equals("about:blank")) return;

            if (_ignoreNextAdded > 0)
            {
                _ignoreNextAdded--;
                return;
            }
            if (_current == null)
            {
                _current = new HistoryItem(url);
            }
            else
            {
                var newItem = new HistoryItem(url);
                _current.Append(newItem);
                _current = newItem; // Move current pointer forward
            }
        }

        private string GetNextByDirection(Direction direction)
        {
            if (_current is null) return "";
            var move = direction == Direction.LEFT ? _current.Previous : _current.Next;
            if (move is not null)
            {
                _current = move;
                _ignoreNextAdded += 1;
                return _current.Url;
            }

            return "";
        }

        public string GetPrevious()
        {
            return GetNextByDirection(Direction.LEFT);
        }

        public string GetNext()
        {
            return GetNextByDirection(Direction.RIGHT);
        }
    }
}
