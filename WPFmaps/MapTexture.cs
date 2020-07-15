using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace WPFmaps
{
    class MapTexture : TerrainTexture
    {
        public MapTexture(string source)
        {
            this.Material = MaterialHelper.CreateImageMaterial(source, 1);
        }
        /// <summary>
		/// MAPTexture Calculate on coordinates.
		/// </summary>
        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {

            var texcoords = new PointCollection();
            foreach (var p in mesh.Positions)
            {
                double x = p.X + model.pointOffset.X;
                double y = p.Y + model.pointOffset.Y;
                texcoords.Add(new Point(y, x));
            }

            this.TextureCoordinates = texcoords;
        }
    }
}
