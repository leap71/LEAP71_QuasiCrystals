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


namespace Leap71
{
    using AperiodicTiling;

    namespace QuasiCrystalExamples
    {
        class PenrosePatternShowCase
        {
            /// <summary>
            /// Generates a penrose pattern by tile inflation.
            /// </summary>
            public static void Task()
            {
                //Step 1: Choose number of subdivisions ("generations") between 1 and 10
                uint nGenerations = 5;

                //Step 2: Generate
                PenrosePattern oPattern = new PenrosePattern(nGenerations);
               
                //Step 3: Show last generation
                oPattern.PreviewGeneration(nGenerations - 1);
            }
        }
    }
}