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
        using static QuasiTile;

        public class QuasiCrystal
        {
            protected List<QuasiTile>[] m_aTileGenerations;
            protected float             m_fInitialTileSideLength;
            protected float             m_fInitialTileRotationAngle;
            protected uint              m_nGenerations;


            /// <summary>
            /// Quasicrystal with an initial set of quasi tiles from which
            /// new generation of quasi tiles are inflated.
            /// Each tile generation is de-dublicated.
            /// </summary>
            public QuasiCrystal(uint nGenerations, List<QuasiTile> aInitialTiles)
			{
                m_nGenerations              = nGenerations;
                m_aTileGenerations          = new List<QuasiTile>[(int)m_nGenerations];
                m_aTileGenerations[0]       = aInitialTiles;

                //grow generations
                for (int i = 1; i < m_nGenerations; i++)
                {
                    List<QuasiTile> aInflatedSubTiles   = new List<QuasiTile>();
                    foreach (QuasiTile oTile in m_aTileGenerations[i - 1])
                    {
                        List<IcosehedralFace> aFaces    = oTile.aGetFaces();
                        foreach (IcosehedralFace sFace in aFaces)
                        {
                            aInflatedSubTiles.AddRange(QuasiTileInflation.aGetInflatedFace(sFace));
                        }
                    }

                    //remove dublicated tiles
                    aInflatedSubTiles       = aGetDeduplicatedSubTiles(aInflatedSubTiles);
                    m_aTileGenerations[i]   = aInflatedSubTiles;
                }
            }

            /// <summary>
            /// Quasicrystal with an initial icosahedral face from which
            /// new generation of quasi tiles are inflated.
            /// Each tile generation is de-dublicated.
            /// </summary>
            public QuasiCrystal(uint nGenerations, IcosehedralFace sInitialFace)
			{
                m_nGenerations              = nGenerations;
                m_aTileGenerations          = new List<QuasiTile>[(int)m_nGenerations];
                m_aTileGenerations[0]       = QuasiTileInflation.aGetInflatedFace(sInitialFace);

                //grow generations
                for (int i = 1; i < m_nGenerations; i++)
                {
                    List<QuasiTile> aInflatedSubTiles   = new List<QuasiTile>();
                    foreach (QuasiTile oTile in m_aTileGenerations[i - 1])
                    {
                        List<IcosehedralFace> aFaces    = oTile.aGetFaces();
                        foreach (IcosehedralFace sFace in aFaces)
                        {
                            aInflatedSubTiles.AddRange(QuasiTileInflation.aGetInflatedFace(sFace));
                        }
                    }

                    //remove dublicated tiles
                    aInflatedSubTiles       = aGetDeduplicatedSubTiles(aInflatedSubTiles);
                    m_aTileGenerations[i]   = aInflatedSubTiles;
                }
            }

            /// <summary>
            /// Removes dublicated tiles that occur during infaltion.
            /// </summary>
            protected List<QuasiTile> aGetDeduplicatedSubTiles(List<QuasiTile> aSubTiles)
            {
                List<Vector3> aRefCentres               = new List<Vector3>();
                List<QuasiTile> aDedublicatedSubTiles   = new List<QuasiTile>();

                foreach (QuasiTile oSubTile in aSubTiles)
                {
                    Vector3 vecRoundedCentre = oSubTile.vecGetRoundedCentre();

                    if (aRefCentres.Contains(vecRoundedCentre) == false)
                    {
                        aRefCentres.Add(vecRoundedCentre);
                        aDedublicatedSubTiles.Add(oSubTile);
                    }
                }
                return aDedublicatedSubTiles;
            }

            /// <summary>
            /// Returns a hard-coded initial set of tiles that can serve as
            /// starting condition to the quasicrystal tile growth.
            /// </summary>
            public static List<QuasiTile> aGetFirstGenerationTiles()
            {
                List<QuasiTile> aTiles = new List<QuasiTile>();

                //first row
                QuasiTile oTile_000 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_001 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_002 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_003 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_004 = new QuasiTile_01(new LocalFrame());

                oTile_001.AttachToOtherQuasiTile(0, oTile_000, 1);
                oTile_002.AttachToOtherQuasiTile(0, oTile_001, 1);
                oTile_003.AttachToOtherQuasiTile(0, oTile_002, 1);
                oTile_004.AttachToOtherQuasiTile(1, oTile_000, 0);

                //second row
                QuasiTile oTile_005 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_006 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_007 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_008 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_009 = new QuasiTile_01(new LocalFrame());

                oTile_005.AttachToOtherQuasiTile(0, oTile_000, 2);
                oTile_006.AttachToOtherQuasiTile(0, oTile_001, 2);
                oTile_007.AttachToOtherQuasiTile(0, oTile_002, 2);
                oTile_008.AttachToOtherQuasiTile(0, oTile_003, 2);
                oTile_009.AttachToOtherQuasiTile(0, oTile_004, 2);

                //third row
                QuasiTile oTile_010 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_011 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_012 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_013 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_014 = new QuasiTile_01(new LocalFrame());

                oTile_010.AttachToOtherQuasiTile(0, oTile_005, 1);
                oTile_011.AttachToOtherQuasiTile(0, oTile_006, 1);
                oTile_012.AttachToOtherQuasiTile(0, oTile_007, 1);
                oTile_013.AttachToOtherQuasiTile(0, oTile_008, 1);
                oTile_014.AttachToOtherQuasiTile(0, oTile_009, 1);

                //fourth row
                QuasiTile oTile_015 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_016 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_017 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_018 = new QuasiTile_01(new LocalFrame());
                QuasiTile oTile_019 = new QuasiTile_01(new LocalFrame());

                oTile_015.AttachToOtherQuasiTile(0, oTile_010, 2);
                oTile_016.AttachToOtherQuasiTile(0, oTile_011, 2);
                oTile_017.AttachToOtherQuasiTile(0, oTile_012, 2);
                oTile_018.AttachToOtherQuasiTile(0, oTile_013, 2);
                oTile_019.AttachToOtherQuasiTile(0, oTile_014, 2);

                aTiles.Add(oTile_000);
                aTiles.Add(oTile_001);
                aTiles.Add(oTile_002);
                aTiles.Add(oTile_003);
                aTiles.Add(oTile_004);
                aTiles.Add(oTile_005);
                aTiles.Add(oTile_006);
                aTiles.Add(oTile_007);
                aTiles.Add(oTile_008);
                aTiles.Add(oTile_009);
                aTiles.Add(oTile_010);
                aTiles.Add(oTile_011);
                aTiles.Add(oTile_012);
                aTiles.Add(oTile_013);
                aTiles.Add(oTile_014);
                aTiles.Add(oTile_015);
                aTiles.Add(oTile_016);
                aTiles.Add(oTile_017);
                aTiles.Add(oTile_018);
                aTiles.Add(oTile_019);
                return aTiles;
            }

            /// <summary>
            /// Returns a hard-coded secondary set of tiles.
            /// </summary>
            public static List<QuasiTile> aGetSecondGenerationTiles()
            {
                List<QuasiTile> aFirstGenerationTiles = aGetFirstGenerationTiles();
                List<QuasiTile> aTiles = new List<QuasiTile>();

                QuasiTile oTile_100 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_101 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_102 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_103 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_104 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_105 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_106 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_107 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_108 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_109 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_110 = new QuasiTile_04(new LocalFrame());
                QuasiTile oTile_111 = new QuasiTile_04(new LocalFrame());

                oTile_100.AttachToOtherQuasiTile(1, aFirstGenerationTiles[0], 4);
                oTile_101.AttachToOtherQuasiTile(1, aFirstGenerationTiles[0], 5);
                oTile_102.AttachToOtherQuasiTile(1, aFirstGenerationTiles[1], 5);
                oTile_103.AttachToOtherQuasiTile(1, aFirstGenerationTiles[2], 5);
                oTile_104.AttachToOtherQuasiTile(1, aFirstGenerationTiles[3], 5);
                oTile_105.AttachToOtherQuasiTile(1, aFirstGenerationTiles[4], 5);
                oTile_106.AttachToOtherQuasiTile(1, aFirstGenerationTiles[15], 5);
                oTile_107.AttachToOtherQuasiTile(1, aFirstGenerationTiles[16], 5);
                oTile_108.AttachToOtherQuasiTile(1, aFirstGenerationTiles[17], 5);
                oTile_109.AttachToOtherQuasiTile(1, aFirstGenerationTiles[18], 5);
                oTile_110.AttachToOtherQuasiTile(1, aFirstGenerationTiles[19], 5);
                oTile_111.AttachToOtherQuasiTile(1, aFirstGenerationTiles[19], 3);

                aTiles.Add(oTile_100);
                aTiles.Add(oTile_101);
                aTiles.Add(oTile_102);
                aTiles.Add(oTile_103);
                aTiles.Add(oTile_104);
                aTiles.Add(oTile_105);
                aTiles.Add(oTile_106);
                aTiles.Add(oTile_107);
                aTiles.Add(oTile_108);
                aTiles.Add(oTile_109);
                aTiles.Add(oTile_110);
                aTiles.Add(oTile_111);
                return aTiles;
            }

            /// <summary>
            /// Visualizes the quasi-tile of a given generation.
            /// If the generation does not exist, an exception is thrown.
            /// </summary>
            public void PreviewGeneration(uint nGen, EPreviewFace ePreviewFace)
            {
                try
                {
                    List<QuasiTile> aTiles        = m_aTileGenerations[nGen];
                    Library.Log($"Number of Tiles = {aTiles.Count}.");
                    foreach (QuasiTile oTile in aTiles)
                    {
                        oTile.Preview(ePreviewFace);
                    }
                }
                catch
                {
                    throw new Exception("Generation not found.");
                }
            }

            /// <summary>
            /// Returns a list of quasi-tile of a given generation.
            /// If the generation does not exist, an exception is thrown.
            /// </summary>
            public List<QuasiTile> aGetTileGeneration(uint nGen)
            {
                try
                {
                    List<QuasiTile> aTiles = m_aTileGenerations[nGen];
                    return aTiles;
                }
                catch
                {
                    throw new Exception("Generation not found.");
                }
            }

            /// <summary>
            /// Constructs a lattice wireframe from the quasi tiles of a given generation.
            /// If the generation does not exist, an exception is thrown.
            /// The global, constant beam thickness can be specified.
            /// </summary>
            public Voxels voxGetWireframe(  uint    nGen,
                                            float   fBeamR)
            {
                try
                {
                    List<QuasiTile> aQuasiTiles = m_aTileGenerations[nGen];
                    Lattice oLattice            = new Lattice();

                    foreach (QuasiTile oTile in aQuasiTiles)
                    {
                        foreach (IcosehedralFace sFace in oTile.aGetFaces())
                        {
                            Vector3 vecPt1 = sFace.vecPt1;
                            Vector3 vecPt2 = sFace.vecPt2;
                            Vector3 vecPt3 = sFace.vecPt3;
                            Vector3 vecPt4 = sFace.vecPt4;

                            oLattice.AddBeam(vecPt1, fBeamR, vecPt2, fBeamR);
                            oLattice.AddBeam(vecPt2, fBeamR, vecPt3, fBeamR);
                            oLattice.AddBeam(vecPt3, fBeamR, vecPt4, fBeamR);
                            oLattice.AddBeam(vecPt4, fBeamR, vecPt1, fBeamR);
                        }
                    }
                    Voxels voxLattice = new Voxels(oLattice);
                    return voxLattice;
                }
                catch
                {
                    throw new Exception("Generation not found.");
                }
            }
        }
	}
}