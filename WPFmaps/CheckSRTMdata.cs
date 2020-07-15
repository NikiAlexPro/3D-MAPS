using Alpinechough.Srtm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WPFmaps
{
    static class CheckSRTMdata
    {
        public static List<string> FindLoseSRTMdata(double latitude, double longitude, double latitudeEnd, double longitudeEnd)
        {
            List<string> losingSRTMdata = new List<string>();
            SrtmData srtmData = new SrtmData("SrtmDataFiles");
            //Вызывать метод для каждой точки!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            double tempLon = longitude;
            double tempLat = latitude;

            double differenceLat = Math.Abs(latitude - latitudeEnd);
            double differenceLon = Math.Abs(longitude - longitudeEnd);

            int Distance(double startLatitude, double startLongitude, double endLatitude, double endLongitude)
            {
                double R = 6371;
                double d = (Math.Sin((Math.PI * startLatitude) / 180) * Math.Sin((Math.PI * endLatitude) / 180)) + ((Math.Cos((Math.PI * startLatitude) / 180) * Math.Cos((Math.PI * endLatitude) / 180)) * (Math.Cos((Math.PI * (startLongitude - endLongitude)) / 180)));
                double L = Math.Acos(d) * R;
                return (int)(L);
            }

            int width = Distance(latitude, longitude, latitude, longitudeEnd);
            int height = Distance(latitude, longitude, latitudeEnd, longitude);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    srtmData.GetListLoseElevation(new GeographicalCoordinates(latitude, longitude), losingSRTMdata);
                    //Вызывать метод с координатами и списком.
                    longitude += ((differenceLon) / (width));
                }

                latitude -= ((differenceLat) / (height)); 
                longitude = tempLon;
            }


            return losingSRTMdata;
        }
    }
}
