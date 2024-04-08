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
		public class QuasiTileInflation
        {
            /// <summary>
            /// Subdivides / inflates an icosahedral face based on its connector type.
            /// Follows the rules provided in https://www.researchgate.net/publication/269776178_Substitution_rules_for_icosahedral_quasicrystals.
            /// Returns a list of sub-tiles.
            /// </summary>
			public static List<QuasiTile> aGetInflatedFace(IcosehedralFace sFace)
			{
                //sFace.Preview();
                Vector3 vecFaceNormal   = Vector3.Cross(sFace.vecLongAxis, sFace.vecShortAxis);
                LocalFrame oFrame       = new LocalFrame(sFace.vecCentre, vecFaceNormal, sFace.vecLongAxis);


                if (sFace.eConnector == IcosehedralFace.EConnector.LINE)
                {
                    List<QuasiTile> aInflatedTiles = new List<QuasiTile>();
                    aInflatedTiles.AddRange(aGetInflatedBlackLine(sFace.vecPt1, sFace.vecPt2, oFrame.vecGetLocalZ(), 0f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine(sFace.vecPt1, sFace.vecPt4, oFrame.vecGetLocalZ(), 1f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine(sFace.vecPt3, sFace.vecPt2, oFrame.vecGetLocalZ(), 1f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine(sFace.vecPt3, sFace.vecPt4, oFrame.vecGetLocalZ(), 0f * MathF.PI / 5f));
                    return aInflatedTiles;
                }
                else if (sFace.eConnector == IcosehedralFace.EConnector.TRIANGLE)
                {
                    List<QuasiTile> aInflatedTiles = new List<QuasiTile>();
                    aInflatedTiles.AddRange(aGetInflatedPurpleLine(sFace.vecPt3, sFace.vecPt4, oFrame.vecGetLocalZ(), 1f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedPurpleLine(sFace.vecPt3, sFace.vecPt2, oFrame.vecGetLocalZ(), 2f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine( sFace.vecPt1, sFace.vecPt4, oFrame.vecGetLocalZ(), 1f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine( sFace.vecPt1, sFace.vecPt2, oFrame.vecGetLocalZ(), 0f * MathF.PI / 5f));
                    return aInflatedTiles;
                }
                else if (sFace.eConnector == IcosehedralFace.EConnector.ARROW)
                {
                    List<QuasiTile> aInflatedTiles = new List<QuasiTile>();
                    aInflatedTiles.AddRange(aGetInflatedPurpleLine(sFace.vecPt3, sFace.vecPt4, oFrame.vecGetLocalZ(), 1f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedPurpleLine(sFace.vecPt1, sFace.vecPt4, oFrame.vecGetLocalZ(), 2f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine( sFace.vecPt3, sFace.vecPt2, oFrame.vecGetLocalZ(), 1f * MathF.PI / 5f));
                    aInflatedTiles.AddRange(aGetInflatedBlackLine( sFace.vecPt1, sFace.vecPt2, oFrame.vecGetLocalZ(), 0f * MathF.PI / 5f));
                    return aInflatedTiles;
                }
                else
                {
                    throw new Exception("Unknown face connector type. Face cannot get inflated.");
                }
            }

            protected static List<QuasiTile> aGetInflatedBlackLine(Vector3 vecStart, Vector3 vecEnd, Vector3 vecFaceNormal, float fCustomAngle)
            {
                //target line arrangement
                m_oTargetLength         = (vecEnd - vecStart).Length();
                Vector3 vecTargetLocalZ = (vecEnd - vecStart).Normalize();
                Vector3 vecTargetLocalX = vecFaceNormal;
                m_oTargetFrame          = new LocalFrame(vecStart, vecTargetLocalZ, vecTargetLocalX);


                //construct sub-tiles
                List<QuasiTile> aInflatedTiles = new List<QuasiTile>();
                QuasiTile oSubTile_000  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_001  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_002  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_003  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_004  = new QuasiTile_01(new LocalFrame());

                oSubTile_001.AttachToOtherQuasiTile(0, oSubTile_000, 1);
                oSubTile_002.AttachToOtherQuasiTile(0, oSubTile_001, 1);
                oSubTile_003.AttachToOtherQuasiTile(0, oSubTile_002, 1);
                oSubTile_004.AttachToOtherQuasiTile(1, oSubTile_000, 0);

                QuasiTile oSubTile_Mid  = new QuasiTile_03(new LocalFrame());
                oSubTile_Mid.AttachToOtherQuasiTile(17, oSubTile_001, 4, true);

                QuasiTile oSubTile_005  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_006  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_007  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_008  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_009  = new QuasiTile_01(new LocalFrame());

                oSubTile_005.AttachToOtherQuasiTile(2, oSubTile_Mid, 0);
                oSubTile_006.AttachToOtherQuasiTile(2, oSubTile_Mid, 1);
                oSubTile_007.AttachToOtherQuasiTile(2, oSubTile_Mid, 2);
                oSubTile_008.AttachToOtherQuasiTile(2, oSubTile_Mid, 3);
                oSubTile_009.AttachToOtherQuasiTile(2, oSubTile_Mid, 4);

                aInflatedTiles.Add(oSubTile_000);
                aInflatedTiles.Add(oSubTile_001);
                aInflatedTiles.Add(oSubTile_002);
                aInflatedTiles.Add(oSubTile_003);
                aInflatedTiles.Add(oSubTile_004);
                aInflatedTiles.Add(oSubTile_Mid);
                aInflatedTiles.Add(oSubTile_005);
                aInflatedTiles.Add(oSubTile_006);
                aInflatedTiles.Add(oSubTile_007);
                aInflatedTiles.Add(oSubTile_008);
                aInflatedTiles.Add(oSubTile_009);


                //current line arrangement
                Vector3 vecS                = oSubTile_000.aGetFaces()[2].vecPt3;
                Vector3 vecE                = oSubTile_009.aGetFaces()[4].vecPt1;
                m_oCurrentLength            = (vecE - vecS).Length();
                Vector3 vecCurrentLocalZ    = (vecE - vecS).Normalize();
                m_oCurrentFrame             = new LocalFrame(vecS, vecCurrentLocalZ);
                Vector3 vecRef1             = oSubTile_Mid.aGetFaces()[^1].vecPt2;
                Vector3 vecRef2             = oSubTile_Mid.aGetFaces()[^1].vecPt1;
                Vector3 vecCross            = Vector3.Cross(vecCurrentLocalZ, (vecRef1 - vecRef2).Normalize());
                Vector3 vecCurrentLocalX    = Vector3.Cross(vecCross, vecCurrentLocalZ);
                vecCurrentLocalX            = VecOperations.vecRotateAroundAxis(vecCurrentLocalX, (fCustomAngle) + MathF.PI / 10f, vecCurrentLocalZ);
                m_oCurrentFrame             = new LocalFrame(vecS, vecCurrentLocalZ, vecCurrentLocalX);


                //transform onto target line
                foreach (QuasiTile oTile in aInflatedTiles)
                {
                    oTile.ApplyTrafo(vecTrafo);
                }
                return aInflatedTiles;
            }

            protected static List<QuasiTile> aGetInflatedPurpleLine(Vector3 vecStart, Vector3 vecEnd, Vector3 vecFaceNormal, float fCustomAngle)
            {
                //target line arrangement
                m_oTargetLength         = (vecEnd - vecStart).Length();
                Vector3 vecTargetLocalZ = (vecEnd - vecStart).Normalize();
                Vector3 vecTargetLocalX = vecFaceNormal;
                m_oTargetFrame          = new LocalFrame(vecStart, vecTargetLocalZ, vecTargetLocalX);


                //construct sub-tiles
                List<QuasiTile> aInflatedTiles = new List<QuasiTile>();
                QuasiTile oSubTile_000  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_001  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_002  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_003  = new QuasiTile_01(new LocalFrame());
                QuasiTile oSubTile_004  = new QuasiTile_01(new LocalFrame());

                oSubTile_001.AttachToOtherQuasiTile(0, oSubTile_000, 1);
                oSubTile_002.AttachToOtherQuasiTile(0, oSubTile_001, 1);
                oSubTile_003.AttachToOtherQuasiTile(0, oSubTile_002, 1);
                oSubTile_004.AttachToOtherQuasiTile(1, oSubTile_000, 0);

                QuasiTile oSubTile_Mid  = new QuasiTile_04(new LocalFrame());
                oSubTile_Mid.AttachToOtherQuasiTile(17, oSubTile_001, 4, true);

                aInflatedTiles.Add(oSubTile_000);
                aInflatedTiles.Add(oSubTile_001);
                aInflatedTiles.Add(oSubTile_002);
                aInflatedTiles.Add(oSubTile_003);
                aInflatedTiles.Add(oSubTile_004);
                aInflatedTiles.Add(oSubTile_Mid);


                //current line arrangement
                Vector3 vecS                = oSubTile_000.aGetFaces()[2].vecPt3;
                Vector3 vecE                = oSubTile_Mid.aGetFaces()[4].vecPt1;
                m_oCurrentLength            = (vecE - vecS).Length();
                Vector3 vecCurrentLocalZ    = (vecE - vecS).Normalize();
                m_oCurrentFrame             = new LocalFrame(vecS, vecCurrentLocalZ);
                Vector3 vecRef1             = oSubTile_Mid.aGetFaces()[2].vecPt2;
                Vector3 vecRef2             = oSubTile_Mid.aGetFaces()[2].vecPt1;
                Vector3 vecCross            = Vector3.Cross(vecCurrentLocalZ, (vecRef1 - vecRef2).Normalize());
                Vector3 vecCurrentLocalX    = Vector3.Cross(vecCross, vecCurrentLocalZ);
                vecCurrentLocalX            = VecOperations.vecRotateAroundAxis(vecCurrentLocalX, (fCustomAngle + MathF.PI / 10f), vecCurrentLocalZ);
                m_oCurrentFrame             = new LocalFrame(vecS, vecCurrentLocalZ, vecCurrentLocalX);


                //transform onto target line
                foreach (QuasiTile oTile in aInflatedTiles)
                {
                    oTile.ApplyTrafo(vecTrafo);
                }
                return aInflatedTiles;
            }

            //temps
            protected static LocalFrame    m_oTargetFrame;
            protected static float         m_oTargetLength;
            protected static LocalFrame    m_oCurrentFrame;
            protected static float         m_oCurrentLength;
            protected static Vector3 vecTrafo(Vector3 vecPt)
            {
                Vector3 vecRel      = VecOperations.vecExpressPointInFrame(m_oCurrentFrame, vecPt);
                vecRel              *= (m_oTargetLength / m_oCurrentLength);
                Vector3 vecNewPt    = VecOperations.vecTranslatePointOntoFrame(m_oTargetFrame, vecRel);
                return vecNewPt;
            }
		}
	}
}