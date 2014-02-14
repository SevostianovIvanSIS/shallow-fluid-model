﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Models;
using Engine.Polyhedra;
using Engine.Utilities;
using UnityEngine;

namespace Assets.Rendering.ParticleMap
{
    public class ParticleNeighbourhoodTracker
    {
        private readonly KDTree _vertexTree;
        private readonly Vector3[] _vertices;
        private readonly int[][] _indicesOfNeighbours;
        private readonly Vector3[][] _neighbours;

        private int[] _indicesOfNearestVertex;

        public ParticleNeighbourhoodTracker(IPolyhedron polyhedron, int particleCount)
        {
            _vertices = GetVertexPositions(polyhedron);

            _vertexTree = KDTree.MakeFromPoints(_vertices);
            _indicesOfNeighbours = VertexIndexedTableFactory.Neighbours(polyhedron);
            _neighbours = BuildVertexNeighbourTable(_indicesOfNeighbours, _vertices);

            _indicesOfNearestVertex = new int[particleCount];
        }

        private static Vector3[][] BuildVertexNeighbourTable(int[][] neighbourIndices, Vector3[] vertices)
        {
            var neighbours = new Vector3[vertices.Length][];
            for (int i = 0; i < vertices.Length; i++)
            {
                neighbours[i] = neighbourIndices[i].Select(j => vertices[j]).ToArray();
            }

            return neighbours;
        }

        private static Vector3[] GetVertexPositions(IPolyhedron surface)
        {
            return surface.Vertices.Select(vertex => GraphicsUtilities.Vector3(vertex.Position)).ToArray();
        }

        public int[] GetIndicesOfVerticesNearest(Vector3[] particlePositions)
        {
            for (int i = 0; i < particlePositions.Length; i++)
            {
                _indicesOfNearestVertex[i] = GetIndexOfNearest(i, particlePositions[i]);
            }

            return _indicesOfNearestVertex;
        }

        private int GetIndexOfNearest(int particleIndex, Vector3 particlePosition)
        {
            var indexOfPreviousClosestVertex = _indicesOfNearestVertex[particleIndex];

            int indexOfNewClosestVertex;
            if (CheckIfAnyNeighbourIsCloser(particlePosition, indexOfPreviousClosestVertex, out indexOfNewClosestVertex))
            {
                int dummyOutVariable;
                if (CheckIfAnyNeighbourIsCloser(particlePosition, indexOfNewClosestVertex, out dummyOutVariable))
                {
                    indexOfNewClosestVertex = _vertexTree.FindNearest(particlePosition);                    
                }
            }

            return indexOfNewClosestVertex;
        }

        private bool CheckIfAnyNeighbourIsCloser(Vector3 particlePosition, int indexOfPreviousClosestVertex, out int indexOfNewClosestVertex)
        {
            var nearestVertex = _vertices[indexOfPreviousClosestVertex];
            var neighbours = _neighbours[indexOfPreviousClosestVertex];
            var indicesOfNeighbours = _indicesOfNeighbours[indexOfPreviousClosestVertex];

            var aNeighbourIsCloser = false;
            indexOfNewClosestVertex = indexOfPreviousClosestVertex;
            var currentSqrDistance = (particlePosition - nearestVertex).sqrMagnitude;
            for (int j = 0; j < neighbours.Length; j++)
            {
                var sqrDistanceToNeighbour = (particlePosition - neighbours[j]).sqrMagnitude;

                if (sqrDistanceToNeighbour < currentSqrDistance)
                {
                    aNeighbourIsCloser = true;
                    indexOfNewClosestVertex = indicesOfNeighbours[j];                    
                    currentSqrDistance = sqrDistanceToNeighbour;
                }
            }

            return aNeighbourIsCloser;
        }

    }
}
