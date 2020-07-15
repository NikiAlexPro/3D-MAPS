using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Microsoft.Maps.MapControl.WPF;

namespace WPFmaps
{
    public class TerrainModel
    {
        public Point3D pointOffset { get; set; }

        public TerrainTexture Texture { get; set; }

        public GeometryModel3D CreateModel(List<Point3D> point3Ds, int height, int width)
        {
            double x = height / 2;
            double y = width / 2;
            double z = 0;

            this.pointOffset = new Point3D(x, y, z);

            //Creation 3D model
            var mb = new MeshBuilder(false, false);
            mb.AddRectangularMesh(point3Ds, width);
            var mesh = mb.ToMesh();

            var material = Materials.Green;

            TerrainModel r = new TerrainModel();
            if (this.Texture != null)
            {
                this.Texture.Calculate(model: this, mesh);
                material = this.Texture.Material;
                mesh.TextureCoordinates = this.Texture.TextureCoordinates;
            }

            return new GeometryModel3D(mesh, material);
        }
    }
}
