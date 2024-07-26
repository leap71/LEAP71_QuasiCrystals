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
        public abstract class QuasiTile
        {
            public enum EPreviewFace          { NONE, AXIS, CONNECTOR };
            protected   ColorFloat            m_clr;
            protected   List<IcosehedralFace> m_aFaces;
            protected   Vector3?              m_vecRoundedCentre;

            /// <summary>
            /// Retruns a list of the icosahedral faces of the tile.
            /// </summary>
            public List<IcosehedralFace> aGetFaces()
            {
                return m_aFaces;
            }

            /// <summary>
            /// Applies the specified coordinate trafo to each vertex of each face of the tile.
            /// These transformations are used to place sub-tiles correctly during the inflation process.
            /// </summary>
            public void ApplyTrafo(BaseShape.fnVertexTransformation oTrafoFunc)
            {
                foreach (IcosehedralFace sFace in m_aFaces)
                {
                    sFace.vecPt1 = oTrafoFunc(sFace.vecPt1);
                    sFace.vecPt2 = oTrafoFunc(sFace.vecPt2);
                    sFace.vecPt3 = oTrafoFunc(sFace.vecPt3);
                    sFace.vecPt4 = oTrafoFunc(sFace.vecPt4);

                    sFace.vecCentre     = sFace.vecPt1 + 0.5f * (sFace.vecPt3 - sFace.vecPt1);
                    sFace.vecLongAxis   = (sFace.vecPt3 - sFace.vecPt1).Normalize();
                    sFace.vecShortAxis  = (sFace.vecPt4 - sFace.vecPt2).Normalize();
                }

                m_vecRoundedCentre = null;
                vecGetRoundedCentre();
            }

            /// <summary>
            /// Calculates the centre of the tile from all unique vertices.
            /// The centre coordinates are rounded to four digits to make comparisons more robust.
            /// </summary>
            public Vector3 vecGetRoundedCentre()
            {
                if (m_vecRoundedCentre == null)
                {
                    List<Vector3> aUniqueVertices = new List<Vector3>();
                    foreach (IcosehedralFace sFace in m_aFaces)
                    {
                        List<Vector3> aFaceVertices = new List<Vector3>() { sFace.vecPt1,
                                                                            sFace.vecPt2,
                                                                            sFace.vecPt3,
                                                                            sFace.vecPt4 };
                        foreach (Vector3 vec in aFaceVertices)
                        {
                            Vector3 vecRounded = new Vector3(   MathF.Round(vec.X, 4),
                                                                MathF.Round(vec.Y, 4),
                                                                MathF.Round(vec.Z, 4));
                            if (aUniqueVertices.Contains(vecRounded) == false)
                            {
                                aUniqueVertices.Add(vecRounded);
                            }
                        }
                    }

                    Vector3 vecCentre = new Vector3();
                    foreach (Vector3 vecRounded in aUniqueVertices)
                    {
                        vecCentre += vecRounded;
                    }
                    vecCentre /= aUniqueVertices.Count;

                    m_vecRoundedCentre       = new Vector3( MathF.Round(vecCentre.X, 4),
                                                            MathF.Round(vecCentre.Y, 4),
                                                            MathF.Round(vecCentre.Z, 4));
                }
                return (Vector3)m_vecRoundedCentre;
            }

            /// <summary>
            /// Returns the number of faces that make up the tile.
            /// </summary>
            public uint nGetNumberOfFaces()
            {
                return (uint)m_aFaces.Count;
            }

            /// <summary>
            /// Visualizes the tile as a mesh with optional previews for each face.
            /// </summary>
            public void Preview(EPreviewFace ePreviewFace)
            {
                //create mesh
                Mesh mshTile            = new Mesh();
                Vector3 vecCentre       = vecGetRoundedCentre();
                foreach (IcosehedralFace sFace in m_aFaces)
                {
                    Vector3 vecNormal   = Vector3.Cross((sFace.vecPt3 - sFace.vecPt1).Normalize(), (sFace.vecPt4 - sFace.vecPt2).Normalize());
                    if (VecOperations.bCheckAlignment(vecNormal, sFace.vecCentre - vecCentre))
                    {
                        MeshUtility.AddQuad(ref mshTile, sFace.vecPt1, sFace.vecPt2, sFace.vecPt3, sFace.vecPt4);
                    }
                    else
                    {
                        MeshUtility.AddQuad(ref mshTile, sFace.vecPt1, sFace.vecPt4, sFace.vecPt3, sFace.vecPt2);
                    }

                    //face preview
                    if (ePreviewFace == EPreviewFace.AXIS)
                    {
                        sFace.Preview(false);
                    }
                    else if (ePreviewFace == EPreviewFace.CONNECTOR)
                    {
                        sFace.Preview(true);
                    }
                }
                Sh.PreviewMesh(mshTile, m_clr, 0.9f);
            }

            /// <summary>
            /// Retruns a connector frame sitting on the centre of the specified face.
            /// The frame's local z points outwards of the quasi tile.
            /// The frame's local x points along the long axis in accordance with the connector type.
            /// </summary>
            public LocalFrame oGetConnectorFrame(int iFaceIndex, int iIndex = 0)
            {
                IcosehedralFace sFace   = m_aFaces[iFaceIndex];

                Vector3 vecPos          = sFace.vecCentre;
                Vector3 vecLocalX       = (sFace.vecPt3 - sFace.vecPt1).Normalize();
                Vector3 vecLocalY       = (sFace.vecPt4 - sFace.vecPt2).Normalize();
                Vector3 vecLocalZ       = Vector3.Cross(vecLocalX, vecLocalY);
                vecLocalZ               = VecOperations.vecFlipForAlignment(vecLocalZ, vecPos - vecGetRoundedCentre());

                if (iIndex != 0 && sFace.eConnector == IcosehedralFace.EConnector.LINE)
                {
                    vecLocalX *= -1f;
                }

                LocalFrame oConnectorFrame = new LocalFrame(vecPos, vecLocalZ, vecLocalX);
                return oConnectorFrame;
            }

            /// <summary>
            /// Command to rotate and position this tile with this face onto the specified other tile's other face.
            /// If the connector types of the specified faces do not match, a custom exception will be thrown.
            /// If the specified faces do not exist, custom exceptions will be thrown.
            /// If the connector type is LINE, the switch toggle can change the refernce direction.
            /// </summary>
            public void AttachToOtherQuasiTile(int iThisFaceIndex, QuasiTile oOtherTile, int iOtherFaceIndex, bool bSwitch = false)
            {
                //check if the specified faces exist
                uint nThisFaces     = nGetNumberOfFaces();
                uint nOtherFaces    = oOtherTile.nGetNumberOfFaces();
                if (iThisFaceIndex >= nThisFaces)
                {
                    throw new ThisFaceNotFoundException("This face index exceeds number of faces on this quasi tile.");
                }
                if (iOtherFaceIndex >= nOtherFaces)
                {
                    throw new OtherFaceNotFoundException("Other face index exceeds number of faces on other quasi tile.");
                }

                //check if the specified faces have a compatible connector type
                if (m_aFaces[iThisFaceIndex].eConnector != oOtherTile.m_aFaces[iOtherFaceIndex].eConnector)
                {
                    throw new ConnectorMismatchException("Connector types do not match.");
                }

                int iSwitch = 0;
                if (bSwitch == true)
                {
                    iSwitch = 1;
                }
                LocalFrame oOtherConnectorFrame  = oOtherTile.oGetConnectorFrame(iOtherFaceIndex, iSwitch);
                LocalFrame oThisCurrentFaceFrame = LocalFrame.oGetInvertFrame(oGetConnectorFrame(iThisFaceIndex), true, false);

                //update all coordinates of this quasi tile
                foreach (IcosehedralFace sFace in m_aFaces)
                {
                    sFace.vecPt1 = VecOperations.vecExpressPointInFrame(oThisCurrentFaceFrame, sFace.vecPt1);
                    sFace.vecPt2 = VecOperations.vecExpressPointInFrame(oThisCurrentFaceFrame, sFace.vecPt2);
                    sFace.vecPt3 = VecOperations.vecExpressPointInFrame(oThisCurrentFaceFrame, sFace.vecPt3);
                    sFace.vecPt4 = VecOperations.vecExpressPointInFrame(oThisCurrentFaceFrame, sFace.vecPt4);

                    sFace.vecPt1 = VecOperations.vecTranslatePointOntoFrame(oOtherConnectorFrame, sFace.vecPt1);
                    sFace.vecPt2 = VecOperations.vecTranslatePointOntoFrame(oOtherConnectorFrame, sFace.vecPt2);
                    sFace.vecPt3 = VecOperations.vecTranslatePointOntoFrame(oOtherConnectorFrame, sFace.vecPt3);
                    sFace.vecPt4 = VecOperations.vecTranslatePointOntoFrame(oOtherConnectorFrame, sFace.vecPt4);

                    sFace.vecCentre     = sFace.vecPt1 + 0.5f * (sFace.vecPt3 - sFace.vecPt1);
                    sFace.vecLongAxis   = (sFace.vecPt3 - sFace.vecPt1).Normalize();
                    sFace.vecShortAxis  = (sFace.vecPt4 - sFace.vecPt2).Normalize();
                }

                m_vecRoundedCentre = null;
                vecGetRoundedCentre();
            }


            //custom exceptions
            public class ThisFaceNotFoundException : Exception
            {
                public ThisFaceNotFoundException(string strMessage) : base(strMessage) { }
            }

            public class OtherFaceNotFoundException : Exception
            {
                public OtherFaceNotFoundException(string strMessage) : base(strMessage) { }
            }

            public class ConnectorMismatchException : Exception
            {
                public ConnectorMismatchException(string strMessage) : base(strMessage) { }
            }
        }
	}
}