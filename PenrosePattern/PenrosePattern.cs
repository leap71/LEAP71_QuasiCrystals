//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2023 by LEAP 71
// https://leap71.com
//
// Computational Engineering will profoundly change our physical world in the
// years ahead. Thank you for being part of the journey.
//
// We have developed this library to be used widely, for both commercial and
// non-commercial projects alike. Therefore, have released it under a permissive
// open-source license.
// 
// The LEAP 71 ShapeKernel is based on the PicoGK compact computational geometry 
// framework. See https://picogk.org for more information.
//
// LEAP 71 licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, THE SOFTWARE IS
// PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED.
//
// See the License for the specific language governing permissions and
// limitations under the License.   
//


using System.Numerics;
using PicoGK;


namespace Leap71
{
    using ShapeKernel;

    namespace AperiodicTiling
    {
		public class PenrosePattern
		{
            protected List<ISubDTile>[] m_aTileGenerations;
            protected float             m_fInitialSide;
            protected uint              m_nGenerations;


            /// <summary>
            /// Penrose pattern with a default initial set of tiles.
            /// The each new generation sub-divides / inflates the previous generation tiles.
            /// Each tile generation is de-dublicated.
            /// Use maximum number of generations below 10.
            /// https://en.wikipedia.org/wiki/Penrose_tiling
            /// </summary>
            public PenrosePattern(uint nGenerations, float fInitialSide = 20f)
			{
                m_nGenerations              = nGenerations;
                m_aTileGenerations          = new List<ISubDTile>[(int)m_nGenerations];
                m_fInitialSide              = fInitialSide;
                m_aTileGenerations[0]       = aGetDefaultInitialTiles();

                //inflate generations
                for (int i = 1; i < m_nGenerations; i++)
                {
                    List<ISubDTile> aInflatedSubTiles = new List<ISubDTile>();
                    foreach (ISubDTile xTile in m_aTileGenerations[i - 1])
                    {
                        aInflatedSubTiles.AddRange(xTile.aGetInflatedSubTiles());
                    }

                    //remove dublicated tiles
                    aInflatedSubTiles       = aGetDeduplicatedSubTiles(aInflatedSubTiles);
                    m_aTileGenerations[i]   = aInflatedSubTiles;
                }
            }

            /// <summary>
            /// Removes dublicated tiles that occur during inflation.
            /// </summary>
            protected List<ISubDTile> aGetDeduplicatedSubTiles(List<ISubDTile> aSubTiles)
            {
                List<Vector3> aRefCentres               = new List<Vector3>();
                List<ISubDTile> aDedublicatedSubTiles   = new List<ISubDTile>();

                foreach (ISubDTile xSubTile in aSubTiles)
                {
                    List<Vector3> aVertices = xSubTile.aGetTileVertices();
                    Vector3 vecCentre       = new Vector3();
                    foreach (Vector3 vec in aVertices)
                    {
                        vecCentre += vec;
                    }
                    vecCentre               /= aVertices.Count;
                    Vector3 vecRoundedCentre = new Vector3( MathF.Round(vecCentre.X, 2),
                                                            MathF.Round(vecCentre.Y, 2),
                                                            MathF.Round(vecCentre.Z, 2));

                    if (aRefCentres.Contains(vecRoundedCentre) == false)
                    {
                        aRefCentres.Add(vecRoundedCentre);
                        aDedublicatedSubTiles.Add(xSubTile);
                    }
                }
                return aDedublicatedSubTiles;
            }

            /// <summary>
            /// Returns a default 5-symmetric set of tiles that can serve as
            /// starting condition to the penrose tile inflation.
            /// </summary>
            protected List<ISubDTile> aGetDefaultInitialTiles()
            {
                List<ISubDTile> aInitialTiles   = new List<ISubDTile>();
                float fTheta                    = (108f) / 180f * MathF.PI;
                uint nSymmetry                  = 5;

                for (int i = 0; i < nSymmetry; i++)
                {
                    float fAlpha            = (2 * MathF.PI) / (float)(nSymmetry) * i;
                    Vector3 vecA            = new Vector3();
                    Vector3 vecB            = VecOperations.vecRotateAroundZ(new Vector3(m_fInitialSide, 0, 0), fAlpha);
                    Vector3 vecC            = VecOperations.vecRotateAroundZ(vecA, fTheta, vecB);

                    RobinsonTriangle oTri   = new RobinsonTriangle(vecA, vecB, vecC);
                    ISubDTile oTile         = new LargeRhombicTile(oTri);
                    aInitialTiles.Add(oTile);
                }
                return aInitialTiles;
            }

            /// <summary>
            /// Visualizes the outline of each tile in a given generation.
            /// If the generation does not exist, an exception is thrown.
            /// </summary>
            public void PreviewGeneration(uint nGen)
            {
                try
                {
                    List<ISubDTile> aTiles        = m_aTileGenerations[nGen];
                    Library.Log($"Number of Tiles = {aTiles.Count}.");
                    foreach (ISubDTile xTile in aTiles)
                    {
                        List<Vector3> aVertices   = xTile.aGetTileVertices();
                        aVertices.Add(aVertices[0]);
                        Sh.PreviewLine(aVertices, Cp.clrBlack);
                    }
                }
                catch
                {
                    throw new Exception("Generation not found.");
                }
            }
        }
	}
}