using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeenGames.Utils.AStarPathFinder;
using FFACETools;

namespace Flipper
{
    public static class Navigation
    {
        /// <summary>
        /// Contains all Navigation Meshes.
        /// </summary>
        private static Dictionary<int,byte[,]> _grids { get; set; }

        /// <summary>
        /// Contains all Hotspots.
        /// </summary>
        private static Dictionary<int,List<Hotspot>> _hotspots { get; set; }

        private static Dictionary<int,List<Blackspot>> _blackspots { get; set; }

        #region Internal Properties

        private static readonly int _offset = 2000;
        private static PathFinderFast worker;

        #endregion


        #region Public Methods

        public static bool IsPositionSafe(int zoneId, int x, int z)
        {
            return false;
        }

        #endregion



        #region Load Records
        private static bool LoadRecord(Zone zone, NavigationRecordType type)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + @"\assets\" + Convert.ToInt32(zone) + type;
            int zoneId = (int) zone;
            string line;

            // create a new mesh
            if (type == NavigationRecordType.Mesh)
            {
                if (!_grids.ContainsKey(zoneId))
                {
                    _grids.Add(zoneId, new byte[PathFinderHelper.RoundToNearestPowerOfTwo(4000), PathFinderHelper.RoundToNearestPowerOfTwo(4000)]);
                }
                for (int i = 0; i < 4096; i++)
                {
                    for (int j = 0; j < 4096; j++)
                    {
                        _grids[zoneId][i, j] = PathFinderHelper.BLOCKED_TILE;
                    }
                }
            }
            if (File.Exists(file))
            {
                StreamReader stream = new StreamReader(file);
                while ((line = stream.ReadLine()) != null)
                {
                    string[] token = line.Split(',');
                    switch (type)
                    {
                        case NavigationRecordType.Mesh:
                        {
                            int x = int.Parse(token[2], CultureInfo.InvariantCulture) + _offset;
                            int z = int.Parse(token[4], CultureInfo.InvariantCulture) + _offset;
                            _grids[zoneId][x,z] = PathFinderHelper.EMPTY_TILE;
                            break;
                        }
                        case NavigationRecordType.Blackspot:
                        {
                            _blackspots[zoneId].Clear();
                            int x = int.Parse(token[1], CultureInfo.InvariantCulture);
                            int z = int.Parse(token[3], CultureInfo.InvariantCulture);
                            int radius = int.Parse(token[4]);
                            _blackspots[zoneId].Add(new Blackspot { X = x, Z = z, Radius = radius });
                            break;
                        }
                        case NavigationRecordType.Hotspot:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                }
            }

            return true;
        }
        #endregion
    }

    public enum NavigationRecordType
    {
        Mesh,
        Blackspot,
        Hotspot
    }


    public class Blackspot
    {
        public int X;
        public int Z;
        public int Radius;
    }
}
