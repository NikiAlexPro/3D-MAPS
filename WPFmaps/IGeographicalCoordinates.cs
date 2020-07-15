
using System;

namespace Alpinechough.Srtm
{
	/// <summary>
	/// Geographical coordinates interface.
	/// </summary>
	public interface IGeographicalCoordinates
	{
		/// <summary>
		/// Gets or sets the latitude.
		/// </summary>
		/// <value>
		/// The latitude.
		/// </value>
		double Latitude { get; set; }
		
		/// <summary>
		/// Gets or sets the longitude.
		/// </summary>
		/// <value>
		/// The longitude.
		/// </value>
		double Longitude { get; set; }
	}
}

