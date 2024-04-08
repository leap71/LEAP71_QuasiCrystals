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
		public class RobinsonTriangle
		{
			public      Vector3 m_vecA;
            public      Vector3 m_vecB;
            public      Vector3 m_vecC;
            protected   Vector3 m_vecCentre;

            /// <summary>
            /// Triangle element that forms one of the two rhombic penrose tiles.
            /// Nomenclature: A and C form the base.
            /// Nomenclature: B froms the tip.
            /// </summary>
            public RobinsonTriangle(Vector3 vecA, Vector3 vecB, Vector3 vecC)
			{
                m_vecA          = vecA;
                m_vecB          = vecB;
                m_vecC          = vecC;
                m_vecCentre     = 0.5f * (m_vecA + m_vecC);
            }

            /// <summary>
            /// Retruns a new triangle that shares the same base, but the tip is flipped.
            /// </summary>
            public RobinsonTriangle oGetFlippedTriangle()
            {
                Vector3 vecNewB          = m_vecB + 2f * (m_vecCentre - m_vecB);
                RobinsonTriangle oNewTri = new RobinsonTriangle(m_vecA, vecNewB, m_vecC);
                return oNewTri;
            }
        }
	}
}