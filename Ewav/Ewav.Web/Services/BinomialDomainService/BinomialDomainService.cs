/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       BinomialDomainService.cs
 *  Namespace:  Ewav.Web.Services    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace Ewav.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // TODO: Create methods containing your application logic.

    [EnableClientAccess()]
    public class BinomialDomainService : DomainService
    {
        private StatisticsRepository.Strat2x2 strat2x2;

        public BinomialDomainService()
        {
            strat2x2 = new StatisticsRepository.Strat2x2();

        }

        public BinomialStatCalcDTO GenerateBinomial(string txtNumerator, string txtObserved, string txtExpected)
        {
            BinomialStatCalcDTO bDto = new BinomialStatCalcDTO();
            if (!string.IsNullOrEmpty(txtNumerator) && !string.IsNullOrEmpty(txtObserved) && !string.IsNullOrEmpty(txtExpected))
            {
                int numerator = 0;
                int observed = 0;
                double expected = 0;
                double lessThan = 0;
                double lessThanEqual = 0;
                double equal = 0;
                double greaterThanEqual = 0;
                double greaterThan = 0;
                double pValue = 0;
                int lcl = 0;
                int ucl = 0;
                bool parseResult1 = int.TryParse(txtNumerator, out numerator);
                bool parseResult2 = int.TryParse(txtObserved, out observed);
                bool parseResult3 = double.TryParse(txtExpected, out expected);

                if (parseResult1 && parseResult2 && parseResult3 && numerator < observed)
                {
                    //bDto.lblLessThanTxt = "< " + numerator;
                    //bDto.lblLessThanEqualTxt = "<= " + numerator;
                    //bDto.lblEqualTxt = "= " + numerator;
                    //bDto.lblGreaterThanEqualTxt = ">= " + numerator;
                    //bDto.lblGreaterThanTxt = "> " + numerator;

                    strat2x2.binpdf(observed, numerator, (expected / 100.0), ref lessThan, ref lessThanEqual, ref equal, ref greaterThanEqual, ref greaterThan, ref pValue, ref lcl, ref ucl);

                    bDto.LessThanTxt = lessThan.ToString("F7");
                    bDto.LessThanEqualTxt = lessThanEqual.ToString("F7");
                    bDto.EqualTxt = equal.ToString("F7");
                    bDto.GreaterThanEqualTxt = greaterThanEqual.ToString("F7");
                    bDto.GreaterThanTxt = greaterThan.ToString("F7");
                    bDto.PValueTxt = pValue.ToString("F7");
                    bDto.NinefiveCiTxt = lcl + " - " + ucl;
                }
                else
                {
                    bDto.LessThanTxt = "";
                    bDto.LessThanEqualTxt = "";
                    bDto.EqualTxt = "";
                    bDto.GreaterThanEqualTxt = "";
                    bDto.GreaterThanTxt = "";
                    bDto.PValueTxt = "";
                    bDto.NinefiveCiTxt = "";
                }
            }
            return bDto;

        }
    }

    public class BinomialStatCalcDTO
    {

        public string lessThanTxt;
        public string LessThanTxt
        {
            get { return lessThanTxt; }
            set { lessThanTxt = value; }
        }

        private string lessThanEqualTxt;

        public string LessThanEqualTxt
        {
            get { return lessThanEqualTxt; }
            set { lessThanEqualTxt = value; }
        }

        private string equalTxt;

        public string EqualTxt
        {
            get { return equalTxt; }
            set { equalTxt = value; }
        }

        private string greaterThanEqualTxt;

        public string GreaterThanEqualTxt
        {
            get { return greaterThanEqualTxt; }
            set { greaterThanEqualTxt = value; }
        }

        private string greaterThanTxt;

        public string GreaterThanTxt
        {
            get { return greaterThanTxt; }
            set { greaterThanTxt = value; }
        }

        private string pValueTxt;

        public string PValueTxt
        {
            get { return pValueTxt; }
            set { pValueTxt = value; }
        }

        private string ninefiveCiTxt;

        public string NinefiveCiTxt
        {
            get { return ninefiveCiTxt; }
            set { ninefiveCiTxt = value; }
        }

    }
}