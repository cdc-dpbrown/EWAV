/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Mapper.cs
 *  Namespace:  Ewav.BAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Ewav.DTO;

namespace Ewav.BAL
{
    public static class Mapper
    {
  

        public static List<DTO.EwavFrequencyControlDto> FrequencyOutputList(System.Data.DataTable dt, EwavGadgetParameters ewavGadgetParameters)
        {
            List<EwavFrequencyControlDto> fdsList = new List<EwavFrequencyControlDto>();

            for (int x = 0; x < dt.Rows.Count; x++)
            {
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    Dictionary<string, double> dic = GetConfLimit((double)dt.Rows[x][i],
                        (double)dt.Compute(string.Format("SUM([{0}])", dt.Columns[i].ColumnName), ""));

                    EwavFrequencyControlDto frequencyItem = new EwavFrequencyControlDto
                    {
                        FrequencyColumn = dt.Rows[x][i].ToString(),
                        FreqVariable = dt.Rows[x][ewavGadgetParameters.MainVariableName].ToString(),
                        Perc95ClLowerColumn = dic["lower"].ToString(), //dtDataSources.Rows[x]["freq"].ToString(),  // dtDataSources.Rows[x][ewavGadgetParameters.MainVariableName].ToString(),
                        Perc95ClUpperColumn = dic["upper"].ToString(), //dtDataSources.Rows[x]["freq"].ToString(), //dtDataSources.Rows[x][ewavGadgetParameters.MainVariableName].ToString(),
                        PercentColumn = dt.Rows[x][i].ToString(), //dtDataSources.Rows[x][ewavGadgetParameters.MainVariableName].ToString(),
                        NameOfDtoList = dt.TableName.ToString()
                    };
                    if (frequencyItem.NameOfDtoList.Trim().IndexOf("=") > 0)
                    {
                        switch (frequencyItem.NameOfDtoList.ToString().Substring(frequencyItem.NameOfDtoList.Trim().IndexOf("=") + 1))
                        {
                            case " 0":
                                frequencyItem.NameOfDtoList = frequencyItem.NameOfDtoList.Replace(frequencyItem.NameOfDtoList.ToString().Substring(frequencyItem.NameOfDtoList.Trim().IndexOf("=") + 1), " No");
                                break;
                            case " false":
                                frequencyItem.NameOfDtoList = frequencyItem.NameOfDtoList.Replace(frequencyItem.NameOfDtoList.ToString().Substring(frequencyItem.NameOfDtoList.Trim().IndexOf("=") + 1), " No");
                                break;
                            case " 1":
                                frequencyItem.NameOfDtoList = frequencyItem.NameOfDtoList.Replace(frequencyItem.NameOfDtoList.ToString().Substring(frequencyItem.NameOfDtoList.Trim().IndexOf("=") + 1), " Yes");
                                break;
                            case " true":
                                frequencyItem.NameOfDtoList = frequencyItem.NameOfDtoList.Replace(frequencyItem.NameOfDtoList.ToString().Substring(frequencyItem.NameOfDtoList.Trim().IndexOf("=") + 1), " Yes");
                                break;
                            case " ":
                                frequencyItem.NameOfDtoList = string.Format("{0}Missing", frequencyItem.NameOfDtoList);
                                break;
                            default:
                                break;
                        }
                    }

                    fdsList.Add(frequencyItem);
                }
            }

            return fdsList;
        }

        public static Dictionary<string, double> GetConfLimit(double frequency, double count)
        {
            StatisticsRepository.cFreq freq = new StatisticsRepository.cFreq();
            double lower = 0;
            double upper = 0;

            if (frequency == count)
            {
                lower = 1;
                upper = 1;
            }
            else
            {
                if (count > 300)
                {
                    freq.FLEISS(frequency, count, 1.96, ref lower, ref upper);
                }
                else
                {
                    freq.ExactCI(frequency, count, 95.0, ref lower, ref upper);
                }
            }

            Dictionary<string, double> cl = new Dictionary<string, double>();
            cl.Add("lower", lower);
            cl.Add("upper", upper);

            //ConfLimit cl = new ConfLimit();
            //cl.Lower = lower;
            //cl.Upper = upper;
            //cl.Value = value;
            return cl;
        }
    }
}