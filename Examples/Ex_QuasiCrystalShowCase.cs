//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this PicoGK Example Code.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using PicoGK;
using System.Numerics;


namespace Leap71
{
    using ShapeKernel;
    using AperiodicTiling;
    using static AperiodicTiling.IcosehedralFace;

    namespace QuasiCrystalExamples
    {
        class QuasiCrystalShowCase
        {
            /// <summary>
            /// Generates the four elementary quasi-tiles consisting of icosahedral faces.
            /// </summary>
            public static void IntroduceQuasiTilesTask()
            {
                LocalFrame oFrame_01 = new LocalFrame(new Vector3(+100, 0, 0));
                LocalFrame oFrame_02 = new LocalFrame(new Vector3(+ 40, 0, 0));
                LocalFrame oFrame_03 = new LocalFrame(new Vector3(- 30, 0, 0));
                LocalFrame oFrame_04 = new LocalFrame(new Vector3(-100, 0, 0));

                QuasiTile oTile_01 = new QuasiTile_01(oFrame_01, 20);
                QuasiTile oTile_02 = new QuasiTile_02(oFrame_02, 20);
                QuasiTile oTile_03 = new QuasiTile_03(oFrame_03, 20);
                QuasiTile oTile_04 = new QuasiTile_04(oFrame_04, 20);

                oTile_01.Preview(QuasiTile.EPreviewFace.CONNECTOR);
                oTile_02.Preview(QuasiTile.EPreviewFace.CONNECTOR);
                oTile_03.Preview(QuasiTile.EPreviewFace.CONNECTOR);
                oTile_04.Preview(QuasiTile.EPreviewFace.CONNECTOR);
            }

            /// <summary>
            /// Generates a quasi-crystal from a single icosahedral face by tile inflation.
            /// </summary>
            public static void CrystalFromFaceTask()
            {
                //Step 1: Choose isosahedral face by connector type
                IcosehedralFace sInitialFace    = new IcosehedralFace(new LocalFrame(), EDef.CENTRE, EConnector.LINE, 200);
                //IcosehedralFace sInitialFace    = new IcosehedralFace(new LocalFrame(), EDef.CENTRE, EConnector.TRIANGLE, 200);
                //IcosehedralFace sInitialFace    = new IcosehedralFace(new LocalFrame(), EDef.CENTRE, EConnector.ARROW, 200);
                sInitialFace.Preview(true);

                //Step 2: Generate
                uint nGenerations               = 2;    //choose 1 or 2, not higher!
                QuasiCrystal oCrystal           = new QuasiCrystal(nGenerations, sInitialFace);

                //Step 3: Show last generation
                oCrystal.PreviewGeneration(nGenerations - 1, QuasiTile.EPreviewFace.NONE);
            }

            /// <summary>
            /// Generates a quasi-crystal from a list of quasi-tiles by tile inflation.
            /// </summary>
            public static void CrystalFromTileTask()
            {
                //Step 1a: Choose elementary quasi-tile between 1 and 4 as a single list item
                QuasiTile oInitialTile          = new QuasiTile_04(new LocalFrame(), 50);
                List<QuasiTile> aInitialTiles   = new List<QuasiTile>() { oInitialTile };

                //Step 1b: Choose preset, hard-coded list of quasi-tiles
                //List<QuasiTile> aInitialTiles   = QuasiCrystal.aGetFirstGenerationTiles();

                //Step 2: Generate
                uint nGenerations               = 2;    //choose 1 or 3, not higher!
                QuasiCrystal oCrystal           = new QuasiCrystal(nGenerations, aInitialTiles);

                //Step 3: Show last generation
                oCrystal.PreviewGeneration(nGenerations - 1, QuasiTile.EPreviewFace.AXIS);
            }

            /// <summary>
            /// Generates a lattice-based wireframe from a quasi-crystal.
            /// </summary>
            public static void WireframeFromCrystalTask()
            {
                uint nGenerations               = 2;    //choose 1 or 2, not higher!

                QuasiTile oInitialTile          = new QuasiTile_02(new LocalFrame(), 50);
                List<QuasiTile> aInitialTiles   = new List<QuasiTile>() { oInitialTile };
                QuasiCrystal oCrystal           = new QuasiCrystal(nGenerations, aInitialTiles);

                float fBeamRadius               = 1f;
                Voxels voxCrystalWireframe      = oCrystal.voxGetWireframe(nGenerations - 1, fBeamRadius);
                Sh.PreviewVoxels(voxCrystalWireframe, Cp.clrBlue);
            }
        }
    }
}