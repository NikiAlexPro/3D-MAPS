
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO.Compression;
using System.Windows;
using WPFmaps;
using System.Threading;
//using System.IO.Compression.FileSystem;


namespace Alpinechough.Srtm
{
	/// <summary>
	/// SRTM Data.
	/// </summary>

	public class SrtmData : ISrtmData
	{
		#region Lifecycle
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Alpinechough.Srtm.SrtmData"/> class.
		/// </summary>

		public SrtmData (string dataDirectory)
		{
			if (!Directory.Exists (dataDirectory))
				throw new DirectoryNotFoundException (dataDirectory);
			
			DataDirectory = dataDirectory;
			DataCells = new List<SrtmDataCell> ();
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the data directory.
		/// </summary>
		/// <value>
		/// The data directory.
		/// </value>
		private string DataDirectory { get; set; }
		
		/// <summary>
		/// Gets or sets the SRTM data cells.
		/// </summary>
		/// <value>
		/// The SRTM data cells.
		/// </value>
		private List<SrtmDataCell> DataCells { get; set; }
		
		#endregion
		
		#region Public methods
		
		/// <summary>
		/// Unloads all SRTM data cells.
		/// </summary>
		public void Unload ()
		{
			DataCells.Clear ();
		}
		
		/// <summary>
		/// Gets the elevation.
		/// </summary>

		public int? GetElevation (IGeographicalCoordinates coordinates)
		{
			int cellLatitude = (int)Math.Floor (Math.Abs (coordinates.Latitude));
			if (coordinates.Latitude < 0)
			{
				cellLatitude *= -1;
				cellLatitude -= 1; // because negative so in bottom tile
			}
			
			int cellLongitude = (int)Math.Floor (Math.Abs (coordinates.Longitude));
			if (coordinates.Longitude < 0)
			{
				cellLongitude *= -1;
				cellLongitude -= 1; // because negative so in left tile
			}

			SrtmDataCell dataCell = DataCells.Where (dc => dc.Latitude == cellLatitude && dc.Longitude == cellLongitude).FirstOrDefault ();
			if (dataCell != null)
				return dataCell.GetElevation (coordinates);

			string filename = string.Format("{0}{1:D2}{2}{3:D3}.hgt",
				cellLatitude < 0 ? "S" : "N",
				Math.Abs(cellLatitude),
				cellLongitude < 0 ? "W" : "E",
				Math.Abs(cellLongitude));


			string filePath = Path.Combine (DataDirectory, filename);
			dataCell = new SrtmDataCell (filePath);
			DataCells.Add (dataCell);
			return dataCell.GetElevation (coordinates);
		}

		public void GetListLoseElevation(IGeographicalCoordinates coordinates, List<string> loseSRTMdata)
		{
			int cellLatitude = (int)Math.Floor(Math.Abs(coordinates.Latitude));
			if (coordinates.Latitude < 0)
			{
				cellLatitude *= -1;
				cellLatitude -= 1; // because negative so in bottom tile
			}

			int cellLongitude = (int)Math.Floor(Math.Abs(coordinates.Longitude));
			if (coordinates.Longitude < 0)
			{
				cellLongitude *= -1;
				cellLongitude -= 1; // because negative so in left tile
			}

			string filename = string.Format("{0}{1:D2}{2}{3:D3}.hgt",
				cellLatitude < 0 ? "S" : "N",
				Math.Abs(cellLatitude),
				cellLongitude < 0 ? "W" : "E",
				Math.Abs(cellLongitude));


			string filePath = Path.Combine(DataDirectory, filename);

			if (!File.Exists(filePath))
			{
				if(!loseSRTMdata.Contains(filename))
					loseSRTMdata.Add(filename);
			}
		}
		#endregion

		

	}
}

