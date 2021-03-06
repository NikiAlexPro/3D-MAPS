
using System;
using System.IO;
using System.Windows;

namespace Alpinechough.Srtm
{
	/// <summary>
	/// SRTM data cell.
	/// </summary>
	
	public class SrtmDataCell
	{
		#region Lifecycle

		/// <summary>
		/// Initializes a new instance of the <see cref="Alpinechough.Srtm.SrtmDataCell"/> class.
		/// </summary>
		
		public SrtmDataCell (string filepath)
		{
			if (!File.Exists (filepath))
				throw new FileNotFoundException ("File not found.", filepath);
			
			if (string.Compare (".hgt", Path.GetExtension (filepath), StringComparison.CurrentCultureIgnoreCase) != 0)
				throw new ArgumentException ("Invalid extension.", filepath);
			
			string filename = Path.GetFileNameWithoutExtension (filepath).ToLower ();
			string[] fileCoordinate = filename.Split (new [] { 'e', 'w' });
			if (fileCoordinate.Length != 2)
				throw new ArgumentException ("Invalid filename.", filepath);
			
			fileCoordinate [0] = fileCoordinate [0].TrimStart (new [] { 'n', 's' });
			
			Latitude = int.Parse (fileCoordinate [0]);
			if (filename.Contains ("s"))
				Latitude *= -1;
			
			Longitude = int.Parse (fileCoordinate [1]);
			if (filename.Contains ("w"))
				Longitude *= -1;
			
			
			HgtData = File.ReadAllBytes(filepath);

			
			switch (HgtData.Length) {
			case 1201*1201*2: // SRTM-3
				PointsPerCell = 1201;
				break;
			case 3601*3601*2: // SRTM-1
				PointsPerCell = 3601;
				break;
				//case 0:
					//break;
			default:
				throw new ArgumentException ("Invalid file size.", filepath);
			}
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the hgt data.
		/// </summary>
		/// <value>
		/// The hgt data.
		/// </value>
		private byte[] HgtData { get; set; }
		
		/// <summary>
		/// Gets or sets the points per cell.
		/// </summary>
		/// <value>
		/// The points per cell.
		/// </value>
		private int PointsPerCell { get; set; }
		
		/// <summary>
		/// Gets or sets the latitude of the srtm data file.
		/// </summary>
		/// <value>
		/// The latitude.
		/// </value>
		public int Latitude { get; private set; }
		
		/// <summary>
		/// Gets or sets the longitude of the srtm data file.
		/// </summary>
		/// <value>
		/// The longitude.
		/// </value>
		public int Longitude { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the elevation.
		/// </summary>
		/// <returns>
		/// The height. Null, if elevation is not available.
		/// </returns>
		/// <param name='coordinates'>
		/// Coordinates.
		/// </param>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		/// 

		
		public int? GetElevation (IGeographicalCoordinates coordinates)
		{
			int localLat = (int)((coordinates.Latitude - Latitude) * PointsPerCell);
			int localLon = (int)(((coordinates.Longitude - Longitude)) * PointsPerCell);
			int bytesPos = ((PointsPerCell - localLat - 1) * PointsPerCell * 2) + localLon * 2;

			//if (PointsPerCell == 0)
				//return 0;

			if (bytesPos < 0 || bytesPos > PointsPerCell * PointsPerCell * 2)
				throw new ArgumentOutOfRangeException ("Coordinates out of range.", "coordinates");
			
			if ((HgtData [bytesPos] == 0x80) && (HgtData [bytesPos + 1] == 0x00))
				return 0; //return null
			
			// Motorola "big-endian" order with the most significant byte first

			int? result = (HgtData[bytesPos]) << 8 | HgtData[bytesPos + 1];// ìîäèôèêàöèÿ 
			if (result == null)
			{
				return 0;
			}
			else
				return result; // return (HgtData[bytesPos]) << 8 | HgtData[bytesPos + 1]; áåç if
		}
	
		#endregion
	}
}

