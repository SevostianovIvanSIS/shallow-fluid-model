﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Polyhedra;
using Engine.Utilities;

namespace Engine.Simulation
{
    /// <summary>
    /// A set of static utility methods for use by simulation classes.
    /// </summary>
    public static class SimulationUtilities
    {
        /// <summary>
        /// Constructs a table of the distances from each face's center to it's neighbour's centers. 
        /// Neighbours are listed in the same order as given by surface.NeighboursOf.
        /// </summary>
        public static double[][] BuildDistancesTable(IPolyhedron surface)
        {
            var edgeLengths = new double[surface.Faces.Count][];
            foreach (var face in surface.Faces)
            {
                var distances =
                    surface.
                    NeighboursOf(face).
                    Select(neighbour => VectorUtilities.GeodesicDistance(face.SphericalCenter(), neighbour.SphericalCenter())).
                    ToArray();
                edgeLengths[surface.IndexOf(face)] = distances;
            }

            //TODO: Work out how to turn this into spherical area.

            return edgeLengths;
        }

        /// <summary>
        /// Constructs a table of the lengths of edges surrounding each face. 
        /// Edges are listed in the same order as the opposing faces are given by surface.NeighboursOf.
        /// </summary>
        public static double[][] BuildEdgeLengthsTable(IPolyhedron surface)
        {
            var edgeLengths = new double[surface.Faces.Count][];
            foreach (var face in surface.Faces)
            {
                var lengths = surface.EdgesOf(face).Select(edge => edge.Length()).ToArray();
                edgeLengths[surface.IndexOf(face)] = lengths;
            }

            return edgeLengths;
        }

        /// <summary>
        /// Constructs a table of the areas of the faces.
        /// </summary>
        public static double[] BuildAreasTable(IPolyhedron surface)
        {
            var areas = new double[surface.Faces.Count];
            foreach (var face in surface.Faces)
            {
                areas[surface.IndexOf(face)] = face.Area();
            }

            return areas;
        }

        /// <summary>
        /// Constructs a table of the neighbours of each face. 
        /// Neighbours are listed in the same order as given by surface.NeighboursOf.
        /// </summary>
        public static int[][] BuildNeighboursTable(IPolyhedron surface)
        {
            var neighbours = new int[surface.Faces.Count][];
            foreach (var face in surface.Faces)
            {
                var indicesOfNeighbours = surface.NeighboursOf(face).Select(neighbour => surface.IndexOf(neighbour)).ToArray();
                neighbours[surface.IndexOf(face)] = indicesOfNeighbours;
            }

            return neighbours;
        }
    }
}
