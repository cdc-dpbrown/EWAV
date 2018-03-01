/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       CustomValidation.cs
 *  Namespace:  Ewav.ExtensionMethods    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.ComponentModel.DataAnnotations;


namespace Ewav.ExtensionMethods
{
    public class CustomValidation
    {
        #region Private Members
        private readonly string message;
        #endregion

        #region Properties
        public bool ShowErrorMessage { get; set; }

        public object ValidationError
        {
            get
            {
                return null;
            }
            set
            {
                if (ShowErrorMessage)
                {
                    throw new ValidationException(message);
                }
            }
        }
        #endregion

        #region Constructor
        public CustomValidation(string message)
        {
            this.message = message;
        }
        #endregion
    }
}