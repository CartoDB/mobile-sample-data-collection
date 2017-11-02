using System;
using Carto.Core;
using Carto.Layers;
using Carto.Services;
using Carto.Ui;

namespace data.collection
{
    public static class Extensions
    {
        public static void ConfigureNamedVectorLayers(this MapView map, string username, string mapname, Action<VectorTileLayer> complete)
        {
            System.Threading.Tasks.Task.Run(delegate
            {
                CartoMapsService service = new CartoMapsService();
                service.Username = username;

                // Use VectorLayers
                service.DefaultVectorLayerMode = true;

                VectorTileLayer layer;

                LayerVector layers = service.BuildNamedMap(mapname, new StringVariantMap());
                // Only grab the second layer in the list. 
                // The first is the base map and the last is a text layer
                layer = (VectorTileLayer)layers[1];

                complete(layer);
            });
        }
    }
}
