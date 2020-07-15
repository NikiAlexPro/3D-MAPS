
using System;
using System.Collections.Generic;

namespace Alpinechough.Srtm
{
	/// <summary>
	/// SRTM data interface.
	/// </summary>
	public interface ISrtmData
	{
	    /// <summary>
		/// Unloads all SRTM data cells.
		/// </summary>
		void Unload ();
		
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
		int? GetElevation (IGeographicalCoordinates coordinates);

		void GetListLoseElevation (IGeographicalCoordinates coordinates, List<string> loseSRTMdata);
	}
}

