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
        public class IcosehedralFace
        {
            public enum EConnector  { ARROW, TRIANGLE, LINE };
            public enum EDef        { CENTRE, SHORT_AXIS, LONG_AXIS };

            public static readonly float    m_fPsi = MathF.Acos(1f / MathF.Sqrt(5f));   // 1.10715     rad
                                                                                        // 63.43502229 deg
            public Vector3                  vecPt1;
            public Vector3                  vecPt2;
            public Vector3                  vecPt3;
            public Vector3                  vecPt4;
            public Vector3                  vecLongAxis;
            public Vector3                  vecShortAxis;
            public Vector3                  vecCentre;
            public EConnector               eConnector;


            /// <summary>
            /// Rhombic face, known and named after its appearance in regular icosahedrons.
            /// The long axis and the short axis follow the golden ratio.
            /// The shape can be sized by the specified absolute side length.
            /// </summary>
            public IcosehedralFace(LocalFrame oFrame, EDef eDef, EConnector eConnector, float fSide = 20f)
            {
                this.eConnector         = eConnector;

                Vector3 vecPointer      = fSide * Vector3.UnitX;
                Vector3 vecPointer_01   = VecOperations.vecRotateAroundAxis(vecPointer, -0.5f * m_fPsi, Vector3.UnitZ);
                Vector3 vecPointer_02   = VecOperations.vecRotateAroundAxis(vecPointer, +0.5f * m_fPsi, Vector3.UnitZ);

                //vertices
                vecPt1                  = new Vector3();
                vecPt2                  = vecPt1 + vecPointer_01;
                vecPt3                  = vecPt2 + vecPointer_02;
                vecPt4                  = vecPt3 - vecPointer_01;

                //transform onto frame
                if (eDef == EDef.CENTRE)
                {
                    vecCentre = vecPt1 + 0.5f * (vecPt3 - vecPt1);
                    vecPt1 -= vecCentre;
                    vecPt2 -= vecCentre;
                    vecPt3 -= vecCentre;
                    vecPt4 -= vecCentre;

                    vecPt1 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt1);
                    vecPt2 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt2);
                    vecPt3 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt3);
                    vecPt4 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt4);
                }
                else if (eDef == EDef.LONG_AXIS)
                {
                    Vector3 vecShift = vecPt1;
                    vecPt1 -= vecShift;
                    vecPt2 -= vecShift;
                    vecPt3 -= vecShift;
                    vecPt4 -= vecShift;

                    vecPt1 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt1);
                    vecPt2 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt2);
                    vecPt3 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt3);
                    vecPt4 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt4);
                }
                else if (eDef == EDef.SHORT_AXIS)
                {
                    vecCentre = vecPt1 + 0.5f * (vecPt3 - vecPt1);
                    vecPt1 -= vecCentre;
                    vecPt2 -= vecCentre;
                    vecPt3 -= vecCentre;
                    vecPt4 -= vecCentre;

                    vecPt1 = VecOperations.vecRotateAroundZ(vecPt1, -90f / 180f * MathF.PI);
                    vecPt2 = VecOperations.vecRotateAroundZ(vecPt2, -90f / 180f * MathF.PI);
                    vecPt3 = VecOperations.vecRotateAroundZ(vecPt3, -90f / 180f * MathF.PI);
                    vecPt4 = VecOperations.vecRotateAroundZ(vecPt4, -90f / 180f * MathF.PI);

                    Vector3 vecShift = vecPt2;
                    vecPt1 -= vecShift;
                    vecPt2 -= vecShift;
                    vecPt3 -= vecShift;
                    vecPt4 -= vecShift;

                    vecPt1 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt1);
                    vecPt2 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt2);
                    vecPt3 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt3);
                    vecPt4 = VecOperations.vecTranslatePointOntoFrame(oFrame, vecPt4);
                }

                vecCentre           = vecPt1 + 0.5f * (vecPt3 - vecPt1);
                vecLongAxis         = (vecPt3 - vecPt1).Normalize();
                vecShortAxis        = (vecPt4 - vecPt2).Normalize();
            }

            /// <summary>
            /// Flips the two vertices that make up the long axis.
            /// </summary>
            public void FlipAroundShortAxis()
            {
                Vector3 vecTemp1 = vecPt1;
                Vector3 vecTemp2 = vecPt2;
                Vector3 vecTemp3 = vecPt3;
                Vector3 vecTemp4 = vecPt4;

                vecPt1 = vecTemp3;
                vecPt2 = vecTemp2;
                vecPt3 = vecTemp1;
                vecPt4 = vecTemp4;
            }

            /// <summary>
            /// Flips the two vertices that make up the short axis.
            /// </summary>
            public void FlipAroundLongAxis()
            {
                Vector3 vecTemp1 = vecPt1;
                Vector3 vecTemp2 = vecPt2;
                Vector3 vecTemp3 = vecPt3;
                Vector3 vecTemp4 = vecPt4;

                vecPt1 = vecTemp1;
                vecPt2 = vecTemp4;
                vecPt3 = vecTemp3;
                vecPt4 = vecTemp2;
            }

            /// <summary>
            /// Visualizes the tile boundary, long and short axis.
            /// The connector type can be optionally displayed, too.
            /// </summary>
            public void Preview(bool bShowConnector)
            {
                List<Vector3> aBoundary     = new List<Vector3>() { vecPt1, vecPt2, vecPt3, vecPt4, vecPt1 };
                List<Vector3> aLongAxis     = new List<Vector3>() { vecPt1, vecPt3 };
                List<Vector3> aShortAxis    = new List<Vector3>() { vecPt2, vecPt4 };
                Sh.PreviewLine(aBoundary,   Cp.clrBlack);
                Sh.PreviewLine(aLongAxis,   Cp.clrGreen);
                Sh.PreviewLine(aShortAxis,  Cp.clrRed);


                //preview connector
                if (bShowConnector == true)
                {
                    Lattice latConnector    = new Lattice();
                    if (eConnector == EConnector.LINE)
                    {
                        Vector3 vecStart    = vecPt1 + 0.2f * (vecPt3 - vecPt1);
                        Vector3 vecEnd      = vecPt1 + 0.8f * (vecPt3 - vecPt1);
                        latConnector.AddBeam(vecStart, 0.5f, vecEnd, 0.5f);
                        Sh.PreviewLattice(latConnector, Cp.clrBlue);
                    }
                    else if (eConnector == EConnector.ARROW)
                    {
                        Vector3 vecStart    = vecPt2 + 0.2f * (vecPt4 - vecPt2);
                        Vector3 vecEnd      = vecPt2 + 0.8f * (vecPt4 - vecPt2);
                        latConnector.AddBeam(vecStart, 0.5f, vecEnd, 0.5f);

                        Vector3 vecArrow1   = vecPt2 + 0.4f * (vecPt4 - vecPt2) + 0.2f * (vecPt3 - vecPt1);
                        Vector3 vecArrow2   = vecPt2 + 0.4f * (vecPt4 - vecPt2) - 0.2f * (vecPt3 - vecPt1);
                        latConnector.AddBeam(vecStart, 0.5f, vecArrow1, 0.5f);
                        latConnector.AddBeam(vecStart, 0.5f, vecArrow2, 0.5f);
                        Sh.PreviewLattice(latConnector, Cp.clrRed);
                    }
                    else if (eConnector == EConnector.TRIANGLE)
                    {
                        Vector3 vecStart    = vecPt2 + 0.2f * (vecPt4 - vecPt2);
                        Vector3 vecEnd      = vecPt2 + 0.8f * (vecPt4 - vecPt2);
                        latConnector.AddBeam(vecStart, 0.5f, vecEnd, 0.5f);

                        Vector3 vecTri      = vecPt2 + 0.5f * (vecPt4 - vecPt2) + 0.25f * (vecPt3 - vecPt1);
                        latConnector.AddBeam(vecStart, 0.5f, vecTri, 0.5f);
                        latConnector.AddBeam(vecEnd,   0.5f, vecTri, 0.5f);
                        Sh.PreviewLattice(latConnector, Cp.clrBillie);
                    }
                }
            }
        }
	}
}