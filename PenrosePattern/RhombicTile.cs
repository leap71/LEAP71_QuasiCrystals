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


namespace Leap71
{
	namespace AperiodicTiling
    {
        /// <summary>
        /// Interface for recursive tiles that are used in sub-division / inflation patterns.
        /// </summary>
        public interface ISubDTile
        {
            /// <summary>
            /// Returns the list of next-generation, inflated tiles from a parent tile.
            /// Subdivision and vertex naming after https://www.researchgate.net/publication/343969683_Penrose_tiling_for_visual_secret_sharing.
            /// </summary>
            public List<ISubDTile>  aGetInflatedSubTiles();

            /// <summary>
            /// Returns the vertices of  the tile's planar shape.
            /// </summary>
            public List<Vector3>    aGetTileVertices();
        }

		public abstract class RhombicTile : ISubDTile
        {
            public static readonly float    m_fPsi      = (MathF.Sqrt(5f) - 1f) / 2f;   //1.618f;
            protected RobinsonTriangle      m_oTri_01;
            protected RobinsonTriangle      m_oTri_02;


            /// <summary>
            /// Generates a rhombic tile from a robinson triangle.
            /// </summary>
            public RhombicTile(RobinsonTriangle oTri)
			{
                m_oTri_01   = oTri;
                m_oTri_02   = oTri.oGetFlippedTriangle();
            }

            public abstract List<ISubDTile> aGetInflatedSubTiles();

            public List<Vector3> aGetTileVertices()
            {
                return new List<Vector3>() {    m_oTri_01.m_vecA,
                                                m_oTri_01.m_vecB,
                                                m_oTri_01.m_vecC,
                                                m_oTri_02.m_vecB };
            }
        }

        public class SmallRhombicTile : RhombicTile
        {
            /// <summary>
            /// Generates a rhombic tile from a robinson triangle.
            /// This is the smaller / "skinny" tile used within a penrose pattern.
            /// </summary>
            public SmallRhombicTile(RobinsonTriangle oTri) : base(oTri) { }

            public override List<ISubDTile> aGetInflatedSubTiles()
            {
                Vector3 vecD            = m_oTri_01.m_vecB + m_fPsi * (m_oTri_01.m_vecA - m_oTri_01.m_vecB);
                ISubDTile oSubTile1     = new SmallRhombicTile(new RobinsonTriangle(vecD, m_oTri_01.m_vecC, m_oTri_01.m_vecA));
                ISubDTile oSubTile2     = new LargeRhombicTile(new RobinsonTriangle(m_oTri_01.m_vecC, vecD, m_oTri_01.m_vecB));

                vecD                    = m_oTri_02.m_vecB + m_fPsi * (m_oTri_02.m_vecA - m_oTri_02.m_vecB);
                ISubDTile oSubTile11    = new SmallRhombicTile(new RobinsonTriangle(vecD, m_oTri_02.m_vecC, m_oTri_02.m_vecA));
                ISubDTile oSubTile22    = new LargeRhombicTile(new RobinsonTriangle(m_oTri_02.m_vecC, vecD, m_oTri_02.m_vecB));

                List<ISubDTile> aNewSubDTiles = new List<ISubDTile>() { oSubTile1,
                                                                        oSubTile2,
                                                                        oSubTile11,
                                                                        oSubTile22 };
                return aNewSubDTiles;
            }
        }

        public class LargeRhombicTile : RhombicTile
        {
            /// <summary>
            /// Generates a rhombic tile from a robinson triangle.
            /// This is the larger / "fat" tile used within a penrose pattern.
            /// </summary>
            public LargeRhombicTile(RobinsonTriangle oTri) : base(oTri) { }

            public override List<ISubDTile> aGetInflatedSubTiles()
            {
                Vector3 vecD            = m_oTri_01.m_vecA + m_fPsi * (m_oTri_01.m_vecB - m_oTri_01.m_vecA);
                Vector3 vecE            = m_oTri_01.m_vecA + m_fPsi * (m_oTri_01.m_vecC - m_oTri_01.m_vecA);
                ISubDTile oSubTile1     = new LargeRhombicTile(new RobinsonTriangle(vecE,               vecD, m_oTri_01.m_vecA));
                ISubDTile oSubTile2     = new SmallRhombicTile(new RobinsonTriangle(vecD,               vecE, m_oTri_01.m_vecB));
                ISubDTile oSubTile3     = new LargeRhombicTile(new RobinsonTriangle(m_oTri_01.m_vecC,   vecE, m_oTri_01.m_vecB));

                vecD                    = m_oTri_02.m_vecA + m_fPsi * (m_oTri_02.m_vecB - m_oTri_02.m_vecA);
                vecE                    = m_oTri_02.m_vecA + m_fPsi * (m_oTri_02.m_vecC - m_oTri_02.m_vecA);
                ISubDTile oSubTile11    = new LargeRhombicTile(new RobinsonTriangle(vecE,              vecD, m_oTri_02.m_vecA));
                ISubDTile oSubTile22    = new SmallRhombicTile(new RobinsonTriangle(vecD,              vecE, m_oTri_02.m_vecB));
                ISubDTile oSubTile33    = new LargeRhombicTile(new RobinsonTriangle(m_oTri_02.m_vecC,  vecE, m_oTri_02.m_vecB));

                List<ISubDTile> aNewSubDTiles = new List<ISubDTile>() { oSubTile1,
                                                                        oSubTile2,
                                                                        oSubTile3,
                                                                        oSubTile11,
                                                                        oSubTile22,
                                                                        oSubTile33 };
                return aNewSubDTiles;
            }
        }
	}
}