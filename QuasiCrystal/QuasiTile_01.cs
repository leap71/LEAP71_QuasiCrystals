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


namespace Leap71
{
    using ShapeKernel;

	namespace AperiodicTiling
    {
        using static IcosehedralFace;

        public class QuasiTile_01 : QuasiTile
        {
            /// <summary>
            /// First type of rhombic 3D quasi-tile.
            /// </summary>
            public QuasiTile_01(LocalFrame oFrame0c, float fFaceSide = 20f)
            {
                m_clr                       = Cp.clrRed;

                float fRotAngle             = (360f / 3f) / 180f * MathF.PI;

                IcosehedralFace sRefFace    = new IcosehedralFace(oFrame0c, EDef.LONG_AXIS, EConnector.LINE, fFaceSide);
                float fLongAxis             = (sRefFace.vecPt3 - sRefFace.vecPt1).Length();
                float fShortAxis            = (sRefFace.vecPt4 - sRefFace.vecPt2).Length();
                float fDomeTriangleSide     = fShortAxis;
                float fDomeTriangleHeight   = fDomeTriangleSide / (2f * MathF.Sqrt(3f));
                float fTiltAngleRad         = MathF.Asin(fDomeTriangleHeight / (0.5f * fLongAxis));
                float fTiltAngleDeg         = 90f - (fTiltAngleRad / MathF.PI * 180f);
                float fTiltAngle            = (-fTiltAngleDeg) / 180f * MathF.PI;


                //lower dome
                //lower centre faces
                List<IcosehedralFace> aLowerCentreFaces = new List<IcosehedralFace>();
                for (int i = 0; i < 3; i++)
                {
                    LocalFrame oFrame1b     = LocalFrame.oGetRotatedFrame(oFrame0c, i * fRotAngle, oFrame0c.vecGetLocalZ());
                    oFrame1b                = LocalFrame.oGetRotatedFrame(oFrame1b, fTiltAngle, oFrame1b.vecGetLocalY());

                    IcosehedralFace sFace1b = new IcosehedralFace(oFrame1b, EDef.LONG_AXIS, EConnector.TRIANGLE, fFaceSide);
                    aLowerCentreFaces.Add(sFace1b);
                }


                //mirror upper dome
                float fLowerZ       = VecOperations.vecExpressPointInFrame(oFrame0c, aLowerCentreFaces[0].vecPt2).Z;
                float fUpperZ       = VecOperations.vecExpressPointInFrame(oFrame0c, aLowerCentreFaces[0].vecPt3).Z;
                float fMaxZ         = fLowerZ + fUpperZ;
                LocalFrame oFrame1c = LocalFrame.oGetTranslatedFrame(oFrame0c, fMaxZ * oFrame0c.vecGetLocalZ());
                oFrame1c            = LocalFrame.oGetInvertFrame(oFrame1c, true, true);


                //upper centre faces
                List<IcosehedralFace> aUpperCentreFaces = new List<IcosehedralFace>();
                for (int i = 0; i < 3; i++)
                {
                    LocalFrame oFrame1t     = LocalFrame.oGetRotatedFrame(oFrame1c, i * fRotAngle, oFrame1c.vecGetLocalZ());
                    oFrame1t                = LocalFrame.oGetRotatedFrame(oFrame1t, fTiltAngle, oFrame1t.vecGetLocalY());

                    IcosehedralFace sFace1t = new IcosehedralFace(oFrame1t, EDef.LONG_AXIS, EConnector.LINE, fFaceSide);
                    aUpperCentreFaces.Add(sFace1t);
                }


                //flip lower faces
                for (int i = 0; i < 3; i++)
                {
                    aLowerCentreFaces[i].FlipAroundShortAxis();
                    aLowerCentreFaces[i].FlipAroundLongAxis();
                }


                //combine faces
                m_aFaces = new List<IcosehedralFace>();
                m_aFaces.AddRange(aLowerCentreFaces);
                m_aFaces.AddRange(aUpperCentreFaces);
            }
        }
	}
}