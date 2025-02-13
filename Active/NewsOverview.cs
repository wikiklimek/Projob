using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class NewsOverview
    {
        private List<Media> _media = new List<Media>();
        private List<IReportable> _reportables = new List<IReportable>();
        bool _direction = false;

        public void ReverseDirection()
        {
            _direction = !_direction;
        }
        public int CountMedia { get { return _media.Count; } }
        public int CountReportables  { get { return _reportables.Count; } }
        public List<Media> GetItemsMedia()
        {
            return _media;
        }
        public List<IReportable> GetItemsIReportable()
        {
            return _reportables;
        }

        public void AddItemMedia(Media item)
        {
            this._media.Add(item);
        }
        public void AddItemIReportable(IReportable item)
        {
            this._reportables.Add(item);
        }
        public void ResetMedia()
        {
            _media.Clear();
        }
        public void ResetReportables()
        {
            _reportables.Clear();
        }

        public NewsIterator GetEnumerator()
        {
            return new NewsIterator(_media, _reportables);
        }

        public NewsGenerator GetNewsGenerator()
        {
            return new NewsGenerator(_media, _reportables);
        }

    }


}
