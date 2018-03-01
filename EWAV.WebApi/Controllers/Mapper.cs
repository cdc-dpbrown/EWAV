using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ewav.Web.Services;
using Newtonsoft.Json.Linq;

namespace EWAV.WebApi.Controllers
{
    public class Mapper
    {
        /// <summary>
        /// Method to convert Dictionary object into List of custom created dictionary object
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        internal List<DictionaryDTO> MapDictToList(Dictionary<string, string> dictionary)
        {
            List<DictionaryDTO> listDicDto = new List<DictionaryDTO>();
            foreach (var item in dictionary)
            {
                DictionaryDTO dtoDict = new DictionaryDTO();
                dtoDict.Key = new MyString();
                dtoDict.Key.VarName = item.Key;
                dtoDict.Value = new MyString();
                dtoDict.Value.VarName = item.Value;
                listDicDto.Add(dtoDict);
            }
            return listDicDto;
        }

        /// <summary>
        /// Method that intializes the InputVariable list.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, string> MapJSONToRegressionInputVariableList(JObject gadgetJSON)
        {
            Dictionary<string, string> inputVariableList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


            try
            {
                inputVariableList.Add(gadgetJSON["mainVariable"].ToString(), "dependvar");

                if (gadgetJSON["weightVariable"].ToString().Length > 0)
                {
                    inputVariableList.Add(gadgetJSON["weightVariable"].ToString(), "weightvar");
                }

                if (gadgetJSON["matchVariable"] != null &&
                    gadgetJSON["matchVariable"].ToString().Length > 0)
                {
                    inputVariableList.Add(gadgetJSON["matchVariable"].ToString(), "matchvar");
                }

                bool interceptVal = false;


                interceptVal = Convert.ToBoolean(gadgetJSON["intercept"].ToString());


                if (interceptVal == true)
                {
                    inputVariableList.Add("intercept", "true");
                }
                else
                {
                    inputVariableList.Add("intercept", "false");
                }

                if (gadgetJSON["includemissing"] != null &&
                    Convert.ToBoolean(gadgetJSON["includemissing"].ToString()) == true)
                {
                    inputVariableList.Add("includemissing", "true");
                }
                else
                {
                    inputVariableList.Add("includemissing", "false");
                }

                double p = 0.95;
                
                //if (cbxConf.SelectedIndex > -1 && !string.IsNullOrEmpty(cbxConf.SelectedItem.ToString().Trim()))
                //{
                    bool success = Double.TryParse(gadgetJSON["pvalue"].ToString().Replace("%", string.Empty), out p);
                    if (!success)
                    {
                        p = 0.95;
                    }
                //}

                inputVariableList.Add("p", p.ToString());

                if (gadgetJSON["covariates"]["covariate"] != null)
                {
                    foreach (var s in gadgetJSON["covariates"]["covariate"].Children())
                    {
                        string t = s.ToString();
                        if (!string.IsNullOrEmpty(t) && !inputVariableList.ContainsKey(t))
                        {
                            inputVariableList.Add(t, "unsorted");
                        }
                    }
                }
                if (gadgetJSON["covariates"]["interactionTerm"] != null)
                {
                    if (gadgetJSON["covariates"]["interactionTerm"].HasValues)
                    {
                        foreach (var s in gadgetJSON["covariates"]["interactionTerm"].Children())
                        {
                            string t = s.ToString();
                            if (!string.IsNullOrEmpty(t) && !inputVariableList.ContainsKey(t))
                            {
                                inputVariableList.Add(t, "term");
                            }
                        }
                    }
                    else
                    {
                        string t = gadgetJSON["covariates"]["interactionTerm"].ToString();
                        inputVariableList.Add(t, "term");
                    }
                }

                if (gadgetJSON["covariates"]["dummy"] != null)
                {
                    if (gadgetJSON["covariates"]["dummy"].HasValues)
                    {
                        foreach (var s in gadgetJSON["covariates"]["dummy"].Children())
                        {
                            string t = s.ToString();
                            if (!string.IsNullOrEmpty(t) && !inputVariableList.ContainsKey(t))
                            {
                                inputVariableList.Add(t, "discrete");
                            }
                        }
                    }
                    else
                    {
                        string t = gadgetJSON["covariates"]["dummy"].ToString();
                        inputVariableList.Add(t, "discrete");
                    }
                }
                

            }
            catch (ArgumentException)
            {
                throw;
            }

            return inputVariableList;
        }

        internal LinRegressionResults FormatLinRegressionResults(LinRegressionResults LinRegressionResults)
        {
            LinRegressionResults.RegressionSumOfSquares = Convert.ToDouble(LinRegressionResults.RegressionSumOfSquares.ToString("F4"));
            LinRegressionResults.RegressionMeanSquare = Convert.ToDouble(LinRegressionResults.RegressionMeanSquare.ToString("F4"));
            LinRegressionResults.RegressionF = Convert.ToDouble(LinRegressionResults.RegressionF.ToString("F4"));

            LinRegressionResults.ResidualsSumOfSquares = Convert.ToDouble(LinRegressionResults.ResidualsSumOfSquares.ToString("F4"));
            LinRegressionResults.ResidualsMeanSquare = Convert.ToDouble(LinRegressionResults.ResidualsMeanSquare.ToString("F4"));

            LinRegressionResults.TotalSumOfSquares = Convert.ToDouble(LinRegressionResults.TotalSumOfSquares.ToString("F4"));

            LinRegressionResults.CorrelationCoefficient = Convert.ToDouble(LinRegressionResults.CorrelationCoefficient.ToString("F2"));


            foreach (var item in LinRegressionResults.Variables)
            {
                item.Coefficient = Convert.ToDouble(item.Coefficient.ToString("F3"));
                item.StdError = Convert.ToDouble(item.StdError.ToString("F3"));
                item.Ftest = Convert.ToDouble(item.Ftest.ToString("F4"));
                item.P = Convert.ToDouble(item.P.ToString("F6"));
            }
            return LinRegressionResults;
        }

        internal LogRegressionResults FormatLogRegressionResults(LogRegressionResults LogRegressionResults)
        {
            LogRegressionResults.ScoreStatistic = Convert.ToDouble(LogRegressionResults.ScoreStatistic.ToString("F4"));
            LogRegressionResults.ScoreP = Convert.ToDouble(LogRegressionResults.ScoreP.ToString("F4"));
            LogRegressionResults.LRStatistic = Convert.ToDouble(LogRegressionResults.LRStatistic.ToString("F4"));

            LogRegressionResults.LRP = Convert.ToDouble(LogRegressionResults.LRP.ToString("F4"));
            LogRegressionResults.FinalLikelihood = Convert.ToDouble(LogRegressionResults.FinalLikelihood.ToString("F4"));


            foreach (var item in LogRegressionResults.Variables)
            {
                item.OddsRatio = Convert.ToDouble(item.OddsRatio.ToString("F4"));
                item.NinetyFivePercent = Convert.ToDouble(item.NinetyFivePercent.ToString("F4"));
                item.Ci = Convert.ToDouble(item.Ci.ToString("F4"));
                item.Coefficient = Convert.ToDouble(item.Coefficient.ToString("F4"));
                item.Se = Convert.ToDouble(item.Se.ToString("F4"));
                item.Z = Convert.ToDouble(item.Z.ToString("F4"));
                item.P = Convert.ToDouble(item.P.ToString("F4"));
            }
            return LogRegressionResults;
        }
    }
}