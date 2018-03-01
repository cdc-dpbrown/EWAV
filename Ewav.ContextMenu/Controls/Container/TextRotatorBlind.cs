/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       TextRotatorBlind.cs
 *  Namespace:  Ewav.ContextMenu    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    30/05/2012    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.ContextMenu
{
    public class TextRotatorBlind : TextRollerBlind
    {
        public TextRotatorBlind()
        {
            OpenType = RollerOpenType.RotateY;
        }
    }
}