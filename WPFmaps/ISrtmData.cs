#region MIT License
// MIT License
// Copyright (c) 2012 Alpine Chough Software.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.	
#endregion

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

