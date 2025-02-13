using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using NetworkSourceSimulator;

namespace ProjOb
{
    public class NewsGenerator
    {
        private List<Media> _media;
        private List<IReportable> _reportables;
        private NewsIterator _iterator;
        public NewsGenerator(List<Media> media, List<IReportable> reportables) 
        {
            this._media = media;
            this._reportables = reportables;
            _iterator = new NewsIterator(_media, _reportables);
        }
        public string? GenerateNextNews()
        {
            (IReportable reportable, Media media) = _iterator.Current;
            bool move = _iterator.MoveNext();
            return move == true ? reportable.Reported(media) : null;
        }
    }

    

    
}
