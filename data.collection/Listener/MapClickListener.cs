
using System;
using Carto.Ui;

namespace data.collection
{
    public class MapClickListener : MapEventListener
    {
        public EventHandler<EventArgs> MapClicked;
        public EventHandler<EventArgs> MapMoved;

        public override void OnMapClicked(MapClickInfo mapClickInfo)
        {
            MapClicked?.Invoke(mapClickInfo, EventArgs.Empty);
        }

        public override void OnMapMoved()
        {
            MapMoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
