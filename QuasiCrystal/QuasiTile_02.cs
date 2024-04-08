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
    using ShapeKernel;

	namespace AperiodicTiling
    {
        using static IcosehedralFace;

        public class QuasiTile_02 : QuasiTile
        {
            /// <summary>
            /// Second type of rhombic 3D quasi-tile.
            /// </summary>
            public QuasiTile_02(LocalFrame oFrame0c, float fFaceSide = 20f)
            {
                m_clr                       = Cp.clrLemongrass;

                IcosehedralFace sRefFace    = new IcosehedralFace(oFrame0c, EDef.LONG_AXIS, EConnector.LINE, fFaceSide);
                float fLongAxis             = (sRefFace.vecPt3 - sRefFace.vecPt1).Length();
               
                LocalFrame oFrame1b         = oFrame0c;
                LocalFrame oFrame1t         = LocalFrame.oGetTranslatedFrame(oFrame0c, fLongAxis * oFrame0c.vecGetLocalZ());
                oFrame1t                    = LocalFrame.oGetInvertFrame(oFrame1t, true, true);

                IcosehedralFace sFace1b     = new IcosehedralFace(oFrame1b, EDef.CENTRE, EConnector.LINE, fFaceSide);
                IcosehedralFace sFace1t     = new IcosehedralFace(oFrame1t, EDef.CENTRE, EConnector.LINE, fFaceSide);

                List<IcosehedralFace> aSideFaces    = new List<IcosehedralFace>();
                List<IcosehedralFace> aAngledFaces  = new List<IcosehedralFace>();
                {
                    Vector3 vecSide1b           = sFace1b.vecPt2;
                    Vector3 vecSide1t           = sFace1t.vecPt2;
                    Vector3 vecLong1s           = (vecSide1t - vecSide1b).Normalize();
                    Vector3 vecShort1s          = (sFace1b.vecPt3 - sFace1b.vecPt1).Normalize();
                    Vector3 vecNormal1s         = Vector3.Cross(vecLong1s, vecShort1s);
                    LocalFrame oFrame1s         = new LocalFrame(vecSide1b, vecNormal1s, vecLong1s);
                    IcosehedralFace sFace1s     = new IcosehedralFace(oFrame1s, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);
                    aSideFaces.Add(sFace1s);

                    {
                        Vector3 vecCentre2s         = 0.5f * (sFace1b.vecPt1 + sFace1s.vecPt2);
                        Vector3 vecLong2s           = (vecCentre2s - vecSide1b).Normalize();
                        Vector3 vecShort2s          = (vecCentre2s - sFace1b.vecPt1).Normalize();
                        Vector3 vecNormal2s         = Vector3.Cross(vecLong2s, vecShort2s);
                        LocalFrame oFrame2s         = new LocalFrame(vecSide1b, vecNormal2s, vecLong2s);
                        IcosehedralFace sFace2s     = new IcosehedralFace(oFrame2s, EDef.LONG_AXIS, EConnector.ARROW, fFaceSide);

                        Vector3 vecCentre3s         = 0.5f * (sFace1b.vecPt3 + sFace1s.vecPt4);
                        Vector3 vecLong3s           = (vecCentre3s - vecSide1b).Normalize();
                        Vector3 vecShort3s          = (vecCentre3s - sFace1b.vecPt3).Normalize();
                        Vector3 vecNormal3s         = Vector3.Cross(vecLong3s, vecShort3s);
                        LocalFrame oFrame3s         = new LocalFrame(vecSide1b, vecNormal3s, vecLong3s);
                        IcosehedralFace sFace3s     = new IcosehedralFace(oFrame3s, EDef.LONG_AXIS, EConnector.ARROW, fFaceSide);

                        aAngledFaces.Add(sFace2s);
                        aAngledFaces.Add(sFace3s);
                    }
                    {
                        Vector3 vecCentre2s         = 0.5f * (sFace1t.vecPt1 + sFace1s.vecPt4);
                        Vector3 vecLong2s           = (vecCentre2s - vecSide1t).Normalize();
                        Vector3 vecShort2s          = (vecCentre2s - sFace1t.vecPt1).Normalize();
                        Vector3 vecNormal2s         = Vector3.Cross(vecLong2s, vecShort2s);
                        LocalFrame oFrame2s         = new LocalFrame(vecSide1t, vecNormal2s, vecLong2s);
                        IcosehedralFace sFace2s     = new IcosehedralFace(oFrame2s, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);

                        Vector3 vecCentre3s         = 0.5f * (sFace1t.vecPt3 + sFace1s.vecPt2);
                        Vector3 vecLong3s           = (vecCentre3s - vecSide1t).Normalize();
                        Vector3 vecShort3s          = (vecCentre3s - sFace1t.vecPt3).Normalize();
                        Vector3 vecNormal3s         = Vector3.Cross(vecLong3s, vecShort3s);
                        LocalFrame oFrame3s         = new LocalFrame(vecSide1t, vecNormal3s, vecLong3s);
                        IcosehedralFace sFace3s     = new IcosehedralFace(oFrame3s, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);

                        aAngledFaces.Add(sFace2s);
                        aAngledFaces.Add(sFace3s);
                    }
                }

                {
                    Vector3 vecSide1b           = sFace1b.vecPt4;
                    Vector3 vecSide1t           = sFace1t.vecPt4;
                    Vector3 vecLong1s           = (vecSide1t - vecSide1b).Normalize();
                    Vector3 vecShort1s          = (sFace1b.vecPt3 - sFace1b.vecPt1).Normalize();
                    Vector3 vecNormal1s         = Vector3.Cross(vecLong1s, vecShort1s);
                    LocalFrame oFrame1s         = new LocalFrame(vecSide1b, vecNormal1s, vecLong1s);
                    IcosehedralFace sFace1s     = new IcosehedralFace(oFrame1s, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);
                    aSideFaces.Add(sFace1s);

                    {
                        Vector3 vecCentre2s         = 0.5f * (sFace1b.vecPt1 + sFace1s.vecPt2);
                        Vector3 vecLong2s           = (vecCentre2s - vecSide1b).Normalize();
                        Vector3 vecShort2s          = (vecCentre2s - sFace1b.vecPt1).Normalize();
                        Vector3 vecNormal2s         = Vector3.Cross(vecLong2s, vecShort2s);
                        LocalFrame oFrame2s         = new LocalFrame(vecSide1b, vecNormal2s, vecLong2s);
                        IcosehedralFace sFace2s     = new IcosehedralFace(oFrame2s, EDef.LONG_AXIS, EConnector.ARROW, fFaceSide);

                        Vector3 vecCentre3s         = 0.5f * (sFace1b.vecPt3 + sFace1s.vecPt4);
                        Vector3 vecLong3s           = (vecCentre3s - vecSide1b).Normalize();
                        Vector3 vecShort3s          = (vecCentre3s - sFace1b.vecPt3).Normalize();
                        Vector3 vecNormal3s         = Vector3.Cross(vecLong3s, vecShort3s);
                        LocalFrame oFrame3s         = new LocalFrame(vecSide1b, vecNormal3s, vecLong3s);
                        IcosehedralFace sFace3s     = new IcosehedralFace(oFrame3s, EDef.LONG_AXIS, EConnector.ARROW, fFaceSide);

                        aAngledFaces.Add(sFace2s);
                        aAngledFaces.Add(sFace3s);
                    }
                    {
                        Vector3 vecCentre2s         = 0.5f * (sFace1t.vecPt1 + sFace1s.vecPt4);
                        Vector3 vecLong2s           = (vecCentre2s - vecSide1t).Normalize();
                        Vector3 vecShort2s          = (vecCentre2s - sFace1t.vecPt1).Normalize();
                        Vector3 vecNormal2s         = Vector3.Cross(vecLong2s, vecShort2s);
                        LocalFrame oFrame2s         = new LocalFrame(vecSide1t, vecNormal2s, vecLong2s);
                        IcosehedralFace sFace2s     = new IcosehedralFace(oFrame2s, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);

                        Vector3 vecCentre3s         = 0.5f * (sFace1t.vecPt3 + sFace1s.vecPt2);
                        Vector3 vecLong3s           = (vecCentre3s - vecSide1t).Normalize();
                        Vector3 vecShort3s          = (vecCentre3s - sFace1t.vecPt3).Normalize();
                        Vector3 vecNormal3s         = Vector3.Cross(vecLong3s, vecShort3s);
                        LocalFrame oFrame3s         = new LocalFrame(vecSide1t, vecNormal3s, vecLong3s);
                        IcosehedralFace sFace3s     = new IcosehedralFace(oFrame3s, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);

                        aAngledFaces.Add(sFace2s);
                        aAngledFaces.Add(sFace3s);
                    }


                    //flip lower faces
                    for (int i = 0; i < aSideFaces.Count; i++)
                    {
                        aSideFaces[i].FlipAroundShortAxis();
                        aSideFaces[i].FlipAroundLongAxis();
                    }


                    //combine faces
                    m_aFaces = new List<IcosehedralFace>();
                    m_aFaces.Add(sFace1b);
                    m_aFaces.Add(sFace1t);
                    m_aFaces.AddRange(aSideFaces);
                    m_aFaces.AddRange(aAngledFaces);
                }
            }
        }
	}
}