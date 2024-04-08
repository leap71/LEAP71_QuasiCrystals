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

        public class QuasiTile_03 : QuasiTile
        {
            /// <summary>
            /// Third type of rhombic 3D quasi-tile.
            /// </summary>
            public QuasiTile_03(LocalFrame oFrame0c, float fFaceSide = 20f)
            {
                m_clr                       = Cp.clrCrystal;

                float fRotAngle             = (360f / 5f) / 180f * MathF.PI;

                IcosehedralFace sRefFace    = new IcosehedralFace(oFrame0c, EDef.LONG_AXIS, EConnector.LINE, fFaceSide);
                float fLongAxis             = (sRefFace.vecPt3 - sRefFace.vecPt1).Length();
                float fShortAxis            = (sRefFace.vecPt4 - sRefFace.vecPt2).Length();
                float fDomePentagonSide     = fShortAxis;
                float fDomePentagonHeight   = fDomePentagonSide / (2f * MathF.Sqrt(5f - MathF.Sqrt(20f)));
                float fTiltAngleRad         = MathF.Asin(fDomePentagonHeight / (0.5f * fLongAxis));
                float fTiltAngleDeg         = 90f - (fTiltAngleRad / MathF.PI * 180f);
                float fTiltAngle            = (-fTiltAngleDeg) / 180f * MathF.PI;


                //lower dome
                //lower centre faces
                List<IcosehedralFace> aLowerCentreFaces = new List<IcosehedralFace>();
                for (int i = 0; i < 5; i++)
                {
                    LocalFrame oFrame1b     = LocalFrame.oGetRotatedFrame(oFrame0c, i * fRotAngle, oFrame0c.vecGetLocalZ());
                    oFrame1b                = LocalFrame.oGetRotatedFrame(oFrame1b, fTiltAngle, oFrame1b.vecGetLocalY());

                    IcosehedralFace sFace1b = new IcosehedralFace(oFrame1b, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);
                    aLowerCentreFaces.Add(sFace1b);
                }


                //lower side faces
                List<IcosehedralFace> aLowerSideFaces = new List<IcosehedralFace>();
                for (int i = 0; i < 5; i++)
                {
                    int iLowerIndex = i - 1;
                    if (iLowerIndex < 0)
                    {
                        iLowerIndex += 5;
                    }
                    int iUpperIndex = i;

                    IcosehedralFace sFace1b = aLowerCentreFaces[iLowerIndex];
                    IcosehedralFace sFace2b = aLowerCentreFaces[iUpperIndex];

                    Vector3 vecTip1b        = sFace1b.vecPt3;
                    Vector3 vecTip2b        = sFace2b.vecPt3;
                    Vector3 vecLong1s       = (vecTip2b - vecTip1b).Normalize();
                    Vector3 vecCentre1s     = vecTip1b + 0.5f * (vecTip2b - vecTip1b);
                    Vector3 vecShort1s      = (sFace1b.vecPt4 - vecCentre1s).Normalize();
                    Vector3 vecNormal1s     = Vector3.Cross(vecLong1s, vecShort1s);
                    LocalFrame oFrame1s     = new LocalFrame(vecTip1b, vecNormal1s, vecLong1s);

                    IcosehedralFace sFace1s = new IcosehedralFace(oFrame1s, EDef.LONG_AXIS, EConnector.ARROW, fFaceSide);
                    aLowerSideFaces.Add(sFace1s);
                }


                //mirror upper dome
                float fLowerZ       = VecOperations.vecExpressPointInFrame(oFrame0c, aLowerSideFaces[0].vecPt1).Z;
                float fUpperZ       = VecOperations.vecExpressPointInFrame(oFrame0c, aLowerSideFaces[0].vecPt2).Z;
                float fMaxZ         = fLowerZ + fUpperZ;
                LocalFrame oFrame1c = LocalFrame.oGetTranslatedFrame(oFrame0c, fMaxZ * oFrame0c.vecGetLocalZ());
                oFrame1c            = LocalFrame.oGetInvertFrame(oFrame1c, true, true);


                //upper centre faces
                List<IcosehedralFace> aUpperCentreFaces = new List<IcosehedralFace>();
                for (int i = 0; i < 5; i++)
                {
                    LocalFrame oFrame1t     = LocalFrame.oGetRotatedFrame(oFrame1c, i * fRotAngle, oFrame1c.vecGetLocalZ());
                    oFrame1t                = LocalFrame.oGetRotatedFrame(oFrame1t, fTiltAngle, oFrame1t.vecGetLocalY());

                    IcosehedralFace sFace1t = new IcosehedralFace(oFrame1t, EDef.LONG_AXIS, EConnector.LINE, fFaceSide);
                    aUpperCentreFaces.Add(sFace1t);
                }


                //upper side faces
                List<IcosehedralFace> aUpperSideFaces = new List<IcosehedralFace>();
                for (int i = 0; i < 5; i++)
                {
                    int iLowerIndex = i - 1;
                    if (iLowerIndex < 0)
                    {
                        iLowerIndex += 5;
                    }
                    int iUpperIndex = i;

                    IcosehedralFace sFace1b = aUpperCentreFaces[iLowerIndex];
                    IcosehedralFace sFace2b = aUpperCentreFaces[iUpperIndex];

                    Vector3 vecTip1b        = sFace1b.vecPt3;
                    Vector3 vecTip2b        = sFace2b.vecPt3;
                    Vector3 vecLong1s       = (vecTip2b - vecTip1b).Normalize();
                    Vector3 vecCentre1s     = vecTip1b + 0.5f * (vecTip2b - vecTip1b);
                    Vector3 vecShort1s      = (sFace1b.vecPt4 - vecCentre1s).Normalize();
                    Vector3 vecNormal1s     = Vector3.Cross(vecLong1s, vecShort1s);
                    LocalFrame oFrame1s     = new LocalFrame(vecTip1b, vecNormal1s, vecLong1s);

                    IcosehedralFace sFace1s = new IcosehedralFace(oFrame1s, EDef.LONG_AXIS, EConnector.LINE, fFaceSide);
                    aUpperSideFaces.Add(sFace1s);
                }


                //combine faces
                m_aFaces = new List<IcosehedralFace>();
                m_aFaces.AddRange(aLowerCentreFaces);
                m_aFaces.AddRange(aLowerSideFaces);
                m_aFaces.AddRange(aUpperSideFaces);
                m_aFaces.AddRange(aUpperCentreFaces);
            }
        }
	}
}