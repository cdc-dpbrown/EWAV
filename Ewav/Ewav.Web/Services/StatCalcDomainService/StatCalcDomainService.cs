/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       StatCalcDomainService.cs
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
    using System.Windows;
    using Ewav.DTO;
    using Ewav.BAL;
    using EpiDashboard;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class StatCalcDomainService : DomainService
    {
        StatCalc2x2 statCalcRecord = new StatCalc2x2();

        private Dictionary<int, double[]> strataVals;

        private Dictionary<int, bool[]> strataActive;

        //List<DictionaryDTO> strataActive, strataVals;
        public MySingleTableResults currentSingleTableResults;

        List<MyString> resultsList;

        StatCalcDTO statCalcDtoObject;

        #region  "These are here to force the proxy classes to be created.  "
        public void PortClassToClient777(DescriptiveStatistics ds)
        {
        }

        public void PortClassToClient4(GadgetParameters gp)
        {
        }

        public void PortClassToClient7(EwavFrequencyControlDto gp)
        {
        }

        public void PortClassToClient77(EwavColumn gp)
        {
        }

        //public void PortClassToClient(EwavColumnsMetaData wcmd) { }
        public void PortClassToClient79(EwavConnectionString aa)
        {

        }

        public void PortClassToClient1(DictionaryDTO e)
        {

        }
        #endregion

        public void InitializeStatCalc()
        {
            strataVals = new Dictionary<int, double[]>();
            strataVals.Add(1, new double[4]);
            strataVals.Add(2, new double[4]);
            strataVals.Add(3, new double[4]);
            strataVals.Add(4, new double[4]);
            strataVals.Add(5, new double[4]);
            strataVals.Add(6, new double[4]);
            strataVals.Add(7, new double[4]);
            strataVals.Add(8, new double[4]);
            strataVals.Add(9, new double[4]);

            strataActive = new Dictionary<int, bool[]>();
            strataActive.Add(1, new Boolean[4]);
            strataActive.Add(2, new Boolean[4]);
            strataActive.Add(3, new Boolean[4]);
            strataActive.Add(4, new Boolean[4]);
            strataActive.Add(5, new Boolean[4]);
            strataActive.Add(6, new Boolean[4]);
            strataActive.Add(7, new Boolean[4]);
            strataActive.Add(8, new Boolean[4]);
            strataActive.Add(9, new Boolean[4]);

            resultsList = new List<MyString>();
            statCalcDtoObject = new StatCalcDTO();
            currentSingleTableResults = new MySingleTableResults();

            //return "";
        }

        /// <summary>
        /// This GetFactory Generates the results and sends it back to client in STATCalcDTO format.
        /// </summary>
        /// <param name="ytVal"></param>
        /// <param name="ntVal"></param>
        /// <param name="tyVal"></param>
        /// <param name="tnVal"></param>
        /// <param name="yyVal"></param>
        /// <param name="ynVal"></param>
        /// <param name="nyVal"></param>
        /// <param name="nnVal"></param>
        /// <param name="strataActive"></param>
        /// <param name="strataVals"></param>
        /// <returns></returns>
        public StatCalcDTO GenerateSigTable(int ytVal, int ntVal, int tyVal, int tnVal, int yyVal, int ynVal, int nyVal, int nnVal, List<DictionaryDTO> strataActive,
            List<DictionaryDTO> strataVals)
        {
            InitializeStatCalc();
            ConvertToDictionaryWithBoolKey(strataActive);
            ConvertToDictionaryWithDoubleKey(strataVals);
            

            if ((ytVal > 0) && (ntVal > 0) && (tyVal > 0) && (tnVal > 0))
            {
                StatisticsRepository.cTable.SingleTableResults singleTableResults = new StatisticsRepository.cTable().SigTable((double)yyVal, (double)ynVal, (double)nyVal, (double)nnVal, 0.95);
                currentSingleTableResults.ChiSquareMantel2P = singleTableResults.ChiSquareMantel2P;
                currentSingleTableResults.ChiSquareMantelVal = singleTableResults.ChiSquareMantelVal;
                currentSingleTableResults.ChiSquareUncorrected2P = singleTableResults.ChiSquareUncorrected2P;
                currentSingleTableResults.ChiSquareUncorrectedVal = singleTableResults.ChiSquareUncorrectedVal;
                currentSingleTableResults.ChiSquareYates2P = singleTableResults.ChiSquareYates2P;
                currentSingleTableResults.ChiSquareYatesVal = singleTableResults.ChiSquareYatesVal;
                currentSingleTableResults.ErrorMessage = singleTableResults.ErrorMessage;
                currentSingleTableResults.FisherExact2P = singleTableResults.FisherExact2P;
                currentSingleTableResults.FisherExactP = singleTableResults.FisherExactP;
                currentSingleTableResults.MidP = singleTableResults.MidP;
                currentSingleTableResults.OddsRatioEstimate = singleTableResults.OddsRatioEstimate;
                currentSingleTableResults.OddsRatioLower = singleTableResults.OddsRatioLower;
                currentSingleTableResults.OddsRatioMLEEstimate = singleTableResults.OddsRatioMLEEstimate;
                currentSingleTableResults.OddsRatioMLEFisherLower = singleTableResults.OddsRatioMLEFisherLower;
                currentSingleTableResults.OddsRatioMLEFisherUpper = singleTableResults.OddsRatioMLEFisherUpper;
                currentSingleTableResults.OddsRatioMLEMidPLower = singleTableResults.OddsRatioMLEMidPLower;
                currentSingleTableResults.OddsRatioMLEMidPUpper = singleTableResults.OddsRatioMLEMidPUpper;
                currentSingleTableResults.OddsRatioUpper = singleTableResults.OddsRatioUpper;
                currentSingleTableResults.RiskDifferenceEstimate = singleTableResults.RiskDifferenceEstimate;
                currentSingleTableResults.RiskDifferenceLower = singleTableResults.RiskDifferenceLower;
                currentSingleTableResults.RiskDifferenceUpper = singleTableResults.RiskDifferenceUpper;
                currentSingleTableResults.RiskRatioEstimate = singleTableResults.RiskRatioEstimate;
                currentSingleTableResults.RiskRatioLower = singleTableResults.RiskRatioLower;
                currentSingleTableResults.RiskRatioUpper = singleTableResults.RiskRatioUpper;


                string oddsRatioEstimate = Epi.SharedStrings.UNDEFINED;
                string oddsRatioLower = Epi.SharedStrings.UNDEFINED;
                string oddsRatioUpper = Epi.SharedStrings.UNDEFINED;
                string riskRatioLower = Epi.SharedStrings.UNDEFINED;
                string riskRatioUpper = Epi.SharedStrings.UNDEFINED;
                string fisherExact2P = Epi.SharedStrings.UNDEFINED;

                if (singleTableResults.FisherExact2P != -1)
                {
                    fisherExact2P = ((double)singleTableResults.FisherExact2P).ToString("F10");
                }

                if (singleTableResults.OddsRatioEstimate != null)
                {
                    oddsRatioEstimate = ((double)singleTableResults.OddsRatioEstimate).ToString("F4");
                }

                if (singleTableResults.OddsRatioLower != null)
                {
                    oddsRatioLower = ((double)singleTableResults.OddsRatioLower).ToString("F4");
                }

                if (singleTableResults.OddsRatioUpper != null)
                {
                    oddsRatioUpper = ((double)singleTableResults.OddsRatioUpper).ToString("F4");
                }

                if (singleTableResults.RiskRatioLower != null)
                {
                    riskRatioLower = ((double)singleTableResults.RiskRatioLower).ToString("F4");
                }

                if (singleTableResults.RiskRatioUpper != null)
                {
                    riskRatioUpper = ((double)singleTableResults.RiskRatioUpper).ToString("F4");
                }

                statCalcRecord.ChiSqCorP = singleTableResults.ChiSquareYates2P.ToString("F10");
                statCalcRecord.ChiSqCorVal = singleTableResults.ChiSquareYatesVal.ToString("F4");
                statCalcRecord.ChiSqManP = singleTableResults.ChiSquareMantel2P.ToString("F10");
                statCalcRecord.ChiSqManVal = singleTableResults.ChiSquareMantelVal.ToString("F4");
                statCalcRecord.ChiSqUncP = singleTableResults.ChiSquareUncorrected2P.ToString("F10");
                statCalcRecord.ChiSqUncVal = singleTableResults.ChiSquareUncorrectedVal.ToString("F4");
                statCalcRecord.OddsRatioEstimate = oddsRatioEstimate; // singleTableResults.OddsRatioEstimate.ToString("F4");
                statCalcRecord.OddsRatioLower = oddsRatioLower; //singleTableResults.OddsRatioLower.ToString("F4");
                statCalcRecord.OddsRatioUpper = oddsRatioUpper; // singleTableResults.OddsRatioUpper.ToString("F4");
                statCalcRecord.MidPEstimate = singleTableResults.OddsRatioMLEEstimate.ToString("F4");
                statCalcRecord.MidPLower = singleTableResults.OddsRatioMLEMidPLower.ToString("F4");
                statCalcRecord.MidPUpper = singleTableResults.OddsRatioMLEMidPUpper.ToString("F4");
                statCalcRecord.FisherLower = singleTableResults.OddsRatioMLEFisherLower.ToString("F4");
                statCalcRecord.FisherUpper = singleTableResults.OddsRatioMLEFisherUpper.ToString("F4");
                statCalcRecord.RiskDifferenceEstimate = singleTableResults.RiskDifferenceEstimate.ToString("F4");
                statCalcRecord.RiskDifferenceLower = singleTableResults.RiskDifferenceLower.ToString("F4");
                statCalcRecord.RiskDifferenceUpper = singleTableResults.RiskDifferenceUpper.ToString("F4");
                statCalcRecord.RiskRatioEstimate = singleTableResults.RiskRatioEstimate.ToString("F4");
                statCalcRecord.RiskRatioLower = riskRatioLower; //singleTableResults.RiskRatioLower.ToString("F4");
                statCalcRecord.RiskRatioUpper = riskRatioUpper; //singleTableResults.RiskRatioUpper.ToString("F4");
                statCalcRecord.FisherExact = singleTableResults.FisherExactP.ToString("F10");
                statCalcRecord.MidPExact = singleTableResults.MidP.ToString("F10");
                statCalcRecord.FisherExact2P = fisherExact2P;

            }
            CalculateSummary();
            statCalcDtoObject.SingleResults = currentSingleTableResults;
            statCalcDtoObject.Results = resultsList;
            statCalcDtoObject.TextBoxesData = statCalcRecord;
            return statCalcDtoObject;
        }

        /// <summary>
        /// Helper methods that calculates the results.
        /// </summary>
        void CalculateSummary()
        {
            try
            {
                List<double> yyList = new List<double>();
                List<double> ynList = new List<double>();
                List<double> nyList = new List<double>();
                List<double> nnList = new List<double>();
                for (int x = 1; x <= 9; x++)
                {

                    if (!strataActive[x].Contains(false))
                    {
                        yyList.Add(strataVals[x][0]);
                        ynList.Add(strataVals[x][1]);
                        nyList.Add(strataVals[x][2]);
                        nnList.Add(strataVals[x][3]);
                    }
                }
                double[] yyArr = yyList.ToArray();
                double[] ynArr = ynList.ToArray();
                double[] nyArr = nyList.ToArray();
                double[] nnArr = nnList.ToArray();

                double yySum = yyList.Sum();
                double ynSum = ynList.Sum();
                double nySum = nyList.Sum();
                double nnSum = nnList.Sum();

                if (yyList.Count > 1)
                {
                    StatisticsRepository.Strat2x2 strat2x2 = new StatisticsRepository.Strat2x2();
                    StatisticsRepository.cTable.SingleTableResults singleTableResults = new StatisticsRepository.cTable().SigTable(yySum, ynSum, nySum, nnSum, 0.95);

                    double computedOddsRatio = (double)strat2x2.ComputeOddsRatio(yyArr, ynArr, nyArr, nnArr);
                    double computedOddsRatioMHLL = computedOddsRatio * Math.Exp(-(double)strat2x2.ZSElnOR(yyArr, ynArr, nyArr, nnArr));
                    double computedOddsRatioMHUL = computedOddsRatio * Math.Exp((double)strat2x2.ZSElnOR(yyArr, ynArr, nyArr, nnArr));
                    double computedRR = (double)strat2x2.ComputedRR(yyArr, ynArr, nyArr, nnArr);
                    double computedRRMHLL = computedRR * Math.Exp(-(double)strat2x2.ZSElnRR(yyArr, ynArr, nyArr, nnArr));
                    double computedRRMHUL = computedRR * Math.Exp((double)strat2x2.ZSElnRR(yyArr, ynArr, nyArr, nnArr));
                    //                double mleOR = (double)strat2x2.ucestimaten(yyArr, ynArr, nyArr, nnArr);
                    //                double ExactORLL = (double)strat2x2.exactorln(yyArr, ynArr, nyArr, nnArr);
                    //                double ExactORUL = (double)strat2x2.exactorun(yyArr, ynArr, nyArr, nnArr);
                    double mleOR = double.NaN;
                    double ExactORLL = double.NaN;
                    double ExactORUL = double.NaN;
                    if (ynSum == 0.0 || nySum == 0.0)
                    {
                        mleOR = double.PositiveInfinity;
                        ExactORLL = (double)strat2x2.exactorln(yyArr, ynArr, nyArr, nnArr);
                        ExactORUL = double.PositiveInfinity;
                    }
                    else if (yySum == 0.0 || nnSum == 0.0)
                    {
                        mleOR = 0.0;
                        ExactORLL = 0.0;
                        ExactORUL = (double)strat2x2.exactorun(yyArr, ynArr, nyArr, nnArr);
                    }
                    else
                    {
                        mleOR = (double)strat2x2.ucestimaten(yyArr, ynArr, nyArr, nnArr);
                        ExactORLL = (double)strat2x2.exactorln(yyArr, ynArr, nyArr, nnArr);
                        ExactORUL = (double)strat2x2.exactorun(yyArr, ynArr, nyArr, nnArr);
                    }
                    double uncorrectedChiSquare = strat2x2.ComputeUnChisq(yyArr, ynArr, nyArr, nnArr);
                    double corrChisq = strat2x2.ComputeCorrChisq(yyArr, ynArr, nyArr, nnArr);
                    double uncorrectedChiSquareP = (double)strat2x2.pForChisq(uncorrectedChiSquare);
                    double corrChisqP = (double)strat2x2.pForChisq(corrChisq);
                    string[] results = new string[24];

                    //results[0] = mleOR.ToString("F4");
                    //results[1] = ExactORLL.ToString("F4");
                    //results[2] = ExactORUL.ToString("F4");

                    //results[3] = computedRR.ToString("F4");
                    //results[4] = computedRRMHLL.ToString("F4");
                    //results[5] = computedRRMHUL.ToString("F4");
                    resultsList.Add(new MyString(mleOR.ToString("F4")));
                    resultsList.Add(new MyString(ExactORLL.ToString("F4")));
                    resultsList.Add(new MyString(ExactORUL.ToString("F4")));
                    resultsList.Add(new MyString(computedRR.ToString("F4")));
                    resultsList.Add(new MyString(computedRRMHLL.ToString("F4")));
                    resultsList.Add(new MyString(computedRRMHUL.ToString("F4")));

                    try
                    {
                        resultsList.Add(new MyString(singleTableResults.OddsRatioEstimate.Value.ToString("F4")));
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                    try
                    {
                        resultsList.Add(new MyString(singleTableResults.OddsRatioLower.Value.ToString("F4")));
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                    try
                    {
                        resultsList.Add(new MyString(singleTableResults.OddsRatioUpper.Value.ToString("F4")));
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                    resultsList.Add(new MyString(singleTableResults.OddsRatioMLEEstimate.ToString("F4")));
                    resultsList.Add(new MyString(singleTableResults.OddsRatioMLEMidPLower.ToString("F4")));
                    resultsList.Add(new MyString(singleTableResults.OddsRatioMLEMidPUpper.ToString("F4")));

                    resultsList.Add(new MyString(singleTableResults.OddsRatioMLEFisherLower.ToString("F4")));
                    resultsList.Add(new MyString(singleTableResults.OddsRatioMLEFisherUpper.ToString("F4")));
                    resultsList.Add(new MyString(singleTableResults.RiskRatioEstimate.ToString("F4")));
                    resultsList.Add(new MyString(singleTableResults.RiskRatioLower.Value.ToString("F4")));
                    resultsList.Add(new MyString(singleTableResults.RiskRatioUpper.Value.ToString("F4")));
                    resultsList.Add(new MyString(computedOddsRatio.ToString("F4")));
                    resultsList.Add(new MyString(computedOddsRatioMHLL.ToString("F4")));
                    resultsList.Add(new MyString(computedOddsRatioMHUL.ToString("F4")));
                    resultsList.Add(new MyString(uncorrectedChiSquare.ToString("F4")));
                    resultsList.Add(new MyString(uncorrectedChiSquareP.ToString("F10")));
                    resultsList.Add(new MyString(corrChisq.ToString("F4")));
                    resultsList.Add(new MyString(corrChisqP.ToString("F10")));

                    //e.Result = results;
                }
            }
            catch (Exception ex)
            {
                //
            }
        }

        /// <summary>
        /// Helper method that converts DictionaryDTO in dotnet Dictionary object
        /// </summary>
        /// <param name="dtoList"></param>
        public void ConvertToDictionaryWithBoolKey(List<DictionaryDTO> dtoList)
        {
            Dictionary<int, bool[]> dict = new Dictionary<int, bool[]>();

            Boolean[] boolNew = new Boolean[dtoList.Count];

            for (int i = 0; i < dtoList.Count; i++)
            {
                boolNew[i] = bool.Parse(dtoList[i].Value.VarName.ToString());
            }

            for (int i = 0; i < dtoList.Count; i++)
            {
                strataActive[int.Parse(dtoList[i].Key.VarName.Substring(0, 1))][int.Parse(dtoList[i].Key.VarName.Substring(1, 1))] = boolNew[i];
            }
        }

        /// Helper method that converts DictionaryDTO in dotnet Dictionary object
        public void ConvertToDictionaryWithDoubleKey(List<DictionaryDTO> dtoList)
        {
            double[] doubleNew = new double[dtoList.Count];

            for (int i = 0; i < dtoList.Count; i++)
            {
                doubleNew[i] = double.Parse(dtoList[i].Value.VarName.ToString());
            }

            for (int i = 0; i < dtoList.Count; i++)
            {
                strataVals[int.Parse(dtoList[i].Key.VarName.Substring(0, 1))][int.Parse(dtoList[i].Key.VarName.Substring(1, 1))] = doubleNew[i];
            }

        }
    }

    /// <summary>
    /// Class which consumes another Dto object and sends the data back to client.
    /// </summary>
    public class StatCalcDTO
    {
        public StatCalcDTO()
        {

        }
        private MySingleTableResults singleResults;

        public MySingleTableResults SingleResults
        {
            get { return singleResults; }
            set { singleResults = value; }
        }

        private List<MyString> results;

        public List<MyString> Results
        {
            get { return results; }
            set { results = value; }
        }

        private StatCalc2x2 textBoxesData;

        public StatCalc2x2 TextBoxesData
        {
            get { return textBoxesData; }
            set { textBoxesData = value; }
        }
    }
}