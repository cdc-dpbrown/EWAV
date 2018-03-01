/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       UserListCollection.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Ewav
{
    public class UserListCollection : ObservableCollection<UserListInfo>
    {
        public UserListCollection(List<UserListInfo> userList) : base()
        {
            for (int i = 0; i < userList.Count; i++)
            {
                Add(new UserListInfo(userList[i].IsSelected,
                    userList[i].FirstName,
                    userList[i].LastName,
                    userList[i].UserId));
            }
        }
    }
}