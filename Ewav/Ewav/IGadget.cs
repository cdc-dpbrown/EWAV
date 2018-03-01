/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       IGadget.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */

using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace Ewav
{
    public interface IGadget
    {
        //event GadgetClosingHandler GadgetClosing;
        //event GadgetProcessingFinishedHandler GadgetProcessingFinished;

        StringBuilder HtmlBuilder { get; set; }
        void RefreshResults();
        XNode Serialize(XDocument doc);
        void CreateFromXml(XElement element);
        string ToHTML(bool ForDash = false, string htmlFileName = "", int count = 0);
        bool IsProcessing { get; set; }
        void SetGadgetToProcessingState();
        void SetGadgetToFinishedState();
        void UpdateVariableNames();
        //void UpdateStatusMessage(string statusMessage);
        string CustomOutputHeading { get; set; }
        string CustomOutputDescription { get; set; }
        string CustomOutputCaption { get; set; }
        void CloseGadget();
        void CloseGadgetOnClick();
    }
}