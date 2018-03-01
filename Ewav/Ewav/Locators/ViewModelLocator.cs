/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ViewModelLocator.cs
 *  Namespace:  Ewav.Locators     
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
// Toolkit namespace
using Ewav.ViewModels;
using Ewav.Services;
using Ewav.ViewModels.Gadgets;

namespace Ewav.Locators 
{
    /// <summary>
    /// This class creates ViewModels on demand for Views, supplying a
    /// ServiceAgent to the ViewModel if required.
    /// <para>
    /// Place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:AppMenuViewModelLocator xmlns:vm="clr-namespace:Ewav.Locators"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// Use the <strong>mvvmlocator</strong> or <strong>mvvmlocatornosa</strong>
    /// code snippets to add ViewModels to this locator.
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        // TODO: Use mvvmlocator or mvvmlocatornosa code snippets
        // to add ViewModels to the locator.  
        public AppMenuViewModel AppMenuViewModel
        {
            get
            {
                IDatasourceServiceAgent serviceAgent = new DatasourceServiceAgent();
                return new AppMenuViewModel(serviceAgent);
            }
        }








        public FrequencyViewModel FrequencyViewModel
        {
            get
            {
                IFrequencyControlServiceAgent freqserviceAgent = new FrequencyControlServiceAgent();
                return new FrequencyViewModel(freqserviceAgent);
            }
        }

        public MeansViewModel MeansViewModel
        {
            get
            {
                IMeansControlServiceAgent meanserviceAgent = new MeansControlServiceAgent();
                return new MeansViewModel(meanserviceAgent);
            }
        }

        public LogisticRegressionViewModel LogisticRegressionViewModel
        {
            get
            {
                ILogisticRegressionServiceAgent logisticRegressionServiceAgent = new LogisticRegressionServiceAgent();
                return new LogisticRegressionViewModel(logisticRegressionServiceAgent);
            }
        }

        public TwoxTwoViewModel TwoxTwoViewModel
        {
            get
            {
                ITwoxTwoControlServiceAgent twoxTwoViewAgent = new TwoxTwoControlServiceAgent();
                return new TwoxTwoViewModel(twoxTwoViewAgent);
            }
        }

        public MxNViewModel MxNViewModel
        {
            get
            {
                MxNControlServiceAgent  mxnViewAgent = new MxNControlServiceAgent();
                return new MxNViewModel(mxnViewAgent);
            }
        }

        public LinearRegressionViewModel LinearRegressionViewModel
        {
            get
            {
                ILinearRegressionServiceAgent linearRegressionServiceAgent = new LinearRegressionServiceAgent();
                return new LinearRegressionViewModel(linearRegressionServiceAgent);
            }
        }

        public AberrationViewModel AberrationViewModel
        {
            get
            {
                IAberrationControlServiceAgent aberrationControlServiceAgent  = new AberrationControlServiceAgent();
                return new AberrationViewModel(aberrationControlServiceAgent);
            }
        }

        public EpiCurveViewModel EpiCurveViewModel
        {
            get
            {
                IEpiCurveServiceAgent epiCurveServiceAgent = new EpiCurveServiceAgent();
                return new EpiCurveViewModel(epiCurveServiceAgent);
            }
        }

        //public BarViewModel BarViewModel  
        //{
        //    get
        //    {
        //        IBarServiceAgent  barServiceAgent = new BarServiceAgent();
        //        return new BarViewModel(barServiceAgent);        

        //    }
        //}


        public ScatterViewModel ScatterViewModel
        {
            get
            {
                IScatterControlServiceAgent scratterServiceAgent = new ScatterControlServiceAgent();
                return new ScatterViewModel(scratterServiceAgent);
            }
        }

        public XYChartViewModel XYChartViewModel
        {
            get
            {
                IXYChartServiceAgent xychartServiceAgent = new XYChartServiceAgent();
                return new XYChartViewModel(xychartServiceAgent);
            }
        }

        public StatCalcViewModel StatCalcViewModel
        {
            get
            {
                IStatCalcServiceAgent statcalcServiceAgent = new StatCalcServiceAgent();
                return new StatCalcViewModel(statcalcServiceAgent);
            }
        }

        public BinomialStatCalcViewModel BinomialStatCalcViewModel
        {
            get
            {
                IBinomialServiceAgent statcalcServiceAgent = new BinomialServiceAgent();
                return new BinomialStatCalcViewModel(statcalcServiceAgent);
            }
        }

        public CanvasViewModel CanvasViewModel
        {
            get
            {
                ICanvasServiceAgent canvasServiceAgent = new CanvasServiceAgent();
                return new CanvasViewModel(canvasServiceAgent);
            }
        }

        public LineListViewModel LineListViewModel
        {
            get
            {
                ILineListControlServiceAgent llServiceAgent = new LineListControlServiceAgent();
                return new LineListViewModel(llServiceAgent);
            }
        }

        public MapControlViewModel MapControlViewModel   
        {
            get
            {
                IMapControlControlServiceAgent mcServiceAgent = new MapControlServiceAgent();
                return new MapControlViewModel(mcServiceAgent);
            }
        }

        public CombinedFrequencyViewModel CombinedFrequencyViewModel
        {
            get
            {
                ICombinedFrequencyServiceAgent cfServiceAgent = new CombinedFrequencyServiceAgent();
                return new CombinedFrequencyViewModel(cfServiceAgent);
            }
        }

    }

}