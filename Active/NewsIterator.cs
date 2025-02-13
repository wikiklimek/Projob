using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class NewsIterator
    {
        private List<Media> _media;
        private List<IReportable> _reportables;
        private int _positionMedia;
        private int _positionReportables;
        private bool _reverse = false;
        public NewsIterator(List<Media> media, List<IReportable> reportables)
        {
            this._media = media;
            this._reportables = reportables;
            if (_reverse)
            {
                this._positionMedia = media.Count;
                this._positionReportables = reportables.Count;
            }
        }
        public (IReportable, Media) Current
        {
            get
            {
                return (_reportables[_positionReportables], _media[_positionMedia]);
            }
        }

        public (int, int) Key()
        {
            return (_positionReportables, _positionMedia);
        }

        public void Reset()
        {
            this._positionMedia = this._reverse ? this._media.Count - 1 : 0;
            this._positionReportables = this._reverse ? this._reportables.Count - 1 : 0;
        }

        public bool MoveNext()
        {
            int updatedPositionMedia = this._positionMedia + (this._reverse ? -1 : 1);
            int updatedPositionReportables = this._positionReportables + (this._reverse ? -1 : 1);

            if (updatedPositionMedia >= 0 && updatedPositionMedia < _media.Count)
            {
                _positionMedia = updatedPositionMedia;
                return true;
            }
            else if (updatedPositionReportables >= 0 && updatedPositionReportables < _reportables.Count)
            {
                _positionMedia = this._reverse ? this._media.Count - 1 : 0;
                _positionReportables = updatedPositionReportables;
                return true;
            }
            else return false;
        }
    }
}
