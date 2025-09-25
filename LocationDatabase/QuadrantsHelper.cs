using Core.Enums;
using LocationCore;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Location
{
	public static class QuadrantsHelper
	{
		private const double PI_OVER_180 = Math.PI / 180;
		private const double EARTHS_RADIUS = 6371;

        public static LevelQuadrantPair[] GetLevelQuadrantPairsForLatLng(LatLng latLng, int nLevels)
        {
			int level = 0;
			return GetQuadrantsForLatLng(latLng, nLevels).Select(q => new LevelQuadrantPair(level++, q)).ToArray();

        }	
		public static long[] GetQuadrantsForLatLng(LatLng latLng, int nLevels/*19*/)
		{

			long[] quads = new long[nLevels];
            double latFrom = 90;
            double latTo = -90;
            double lngFrom = -180;
            double lngTo = 180;
            long lastQuadN = 0;
			int level = 0;
			double lat = latLng.Lat;
			double lng = latLng.Lng;
			while (level < nLevels)
			{
                Quarter(ref latFrom, ref latTo,
                    ref lngFrom, ref lngTo, ref lastQuadN, latLng);
                quads[level] = lastQuadN;
				level++;
			}
			return quads;
		}

		public static LevelQuadrantPair[] GetQuadrantsForLatLngAndRadiusf(LatLng latLng, double radiusKm, int nLevels)
		{//at 84 degrees radius  radius about one tenth at equater. equator.

			int startLevel = GetStartLevel(radiusKm, latLng.Lat);
            double latFrom = 90;
            double latTo = -90;
            double lngFrom = -180;
            double lngTo = 180;
            long lastQuadN = 0;
			int level = 0;
			while (level < nLevels)
			{
				if (level >= startLevel)
				{
					break;
				};
                Quarter(ref latFrom, ref latTo,
					ref lngFrom, ref lngTo, ref lastQuadN, latLng);
				level++;
			}
			return GetAllChildQuadrants(latLng, lastQuadN, latFrom,
				latTo, lngFrom,
				lngTo, level,
				level + 3, level + 6,
				radiusKm, nLevels);
		}
		private static void Quarter(ref double latFrom, ref double latTo, 
			ref double lngFrom, ref double lngTo, ref long lastQuadN, LatLng latLng) {
            double midLat = (latTo + latFrom) / 2;
            double midLng = (lngTo + lngFrom) / 2;
            int bit0;
            int bit1;
            /*We go north to south, West to East.
             * [   0  ]  [  1   ] 
			 * [ 2    ] [ 3    ]
			 * 
			 * */
            if (latLng.Lng < midLng)
            {
                bit1 = 0;
                lngTo = midLng;
            }
            else
            {
                bit1 = 1;
                lngFrom = midLng;
            }
            if (latLng.Lat > midLat)
            {
                bit0 = 0;
                latTo = midLat;
            }
            else
            {
                bit0 = 1;
                latFrom = midLat;
            }
            lastQuadN = (lastQuadN * 4) + (2 * bit0) + bit1;
        }
		private static bool WithinRange(double distance, double radiusKm)
		{
			return (distance < 0 ? -distance : distance) < radiusKm;
		}
		public static LevelQuadrantPair[] GetAllChildQuadrants(LatLng latLng, long lastQuadN, double latFrom,
			double latTo, double lngFrom, double lngTo, int level,
			int minLevelPickFrom, int maxLevel, double radiusKm, int nLevels)
		{
			var middleLat = (latTo + latFrom) / 2;
			var middleLng = (lngTo + lngFrom) / 2;
			//01
			//23
			LatLngRange[] newQuadrants = new LatLngRange[] {
					new LatLngRange( latFrom: latFrom, latTo: middleLat, lngFrom: lngFrom, lngTo: middleLng),
					new LatLngRange(latFrom: latFrom, latTo: middleLat, lngFrom: middleLng, lngTo: lngTo),
					new LatLngRange(latFrom: middleLat, latTo: latTo, lngFrom: lngFrom, lngTo: middleLng),
					new LatLngRange(latFrom: middleLat, latTo: latTo, lngFrom: middleLng, lngTo: lngTo)
				};
			List<LevelQuadrantPair> children = new List<LevelQuadrantPair>();
			var lastQuadNTimes4 = (lastQuadN * 4);
			var newLevel = level + 1;
			for (var i = 0; i < 4; i++)
			{
				var newQuadrant = newQuadrants[i];
				var newQuadN = lastQuadNTimes4 + i;
				var toPush = level >= minLevelPickFrom;
				double distance;
				if (toPush)
				{
					distance = GetDistanceFromLatLngs(newQuadrant.middlePoint, latLng);
					if (WithinRange(distance, radiusKm))
                    {
                        children.Add(new LevelQuadrantPair(level, newQuadN));
						continue;
                    }
				}
				if (level < maxLevel && level < nLevels)
				{
					LevelQuadrantPair[] childChildren = GetAllChildQuadrants(latLng, newQuadN, 
							newQuadrant.LatFrom, newQuadrant.LatTo,
							newQuadrant.LngFrom, newQuadrant.LngTo, newLevel, 
							minLevelPickFrom, maxLevel, radiusKm, nLevels);
					children.AddRange(childChildren);

				}
			}
			return children.ToArray();
		}
		public static double GetDistanceFromLatLngs(LatLng latLng1, LatLng latLng2)
		{
			var R = 6371; // Radius of the earth in km
			double dLat = Deg2rad(latLng2.Lat - latLng1.Lat);  // deg2rad below
			double dLon = Deg2rad(latLng2.Lng - latLng1.Lng);
			double a =
					Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					Math.Cos(Deg2rad(latLng1.Lat)) * Math.Cos(Deg2rad(latLng2.Lat)) *
					Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			double d = R * c; // Distance in km
			return d;
		}
		private static double Deg2rad(double degrees)
		{
			return degrees * PI_OVER_180;
		}

        private static int GetStartLevel(double radiusKm, double lat)
        {
            var circleDiameterKm = radiusKm * 2;
            int level = 0;
            double stepKm = 6371 * Math.Cos((Math.PI * lat) / 180)/2;
            while (true)
            {
				if (stepKm/*(nextStep x 2)*/ < circleDiameterKm) {
					return level;
				}
				double nextStep = stepKm / 2;
                level++;
                stepKm = nextStep;
            }
        }
        /*
		private static int GetSmallestQuadLevelToBeBoundByCircleOfSizeRadius(double radiusKm)
		{
			var halfCircumferanceOfEarth = EARTHS_RADIUS  * Math.PI;
			double currentStepSize = halfCircumferanceOfEarth;
			int level = 0;
			while(currentStepSize > radiusKm)
			{
				currentStepSize /= 2;
				level++;
            }
			return level;
		}*/
    }
}
