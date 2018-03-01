
namespace Ewav.SmokeTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ewav.BAL;
    using Ewav.Security;
    using Ewav.DTO;


    public class EntityManagerForTests : EntityManager
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager" /> class.
        /// </summary>
        /// <param name="metaDatabaseType">Type of the metadatabase.</param>
        /// <param name="metaDataConnectionString">The metadata connection string.</param>
        /// <param name="metaDataViewName">Name of the metadata view.</param>
        public EntityManagerForTests(string metaDatabaseType, string metaDataConnectionString, string metaDataViewName,
            string KeyForConnectionStringPassphrase, string KeyForConnectionStringSalt, string KeyForConnectionStringVector)    
        {
            this.metaDataViewName = metaDataViewName;

            CryptographyForTests cy = new CryptographyForTests(KeyForConnectionStringPassphrase, KeyForConnectionStringSalt,
                 KeyForConnectionStringVector);               
            this.metaDatabaseTypeEnum = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum),   metaDatabaseType);
            this.metaDataConnectionString = cy.Decrypt( metaDataConnectionString);  
        }

    }
}
