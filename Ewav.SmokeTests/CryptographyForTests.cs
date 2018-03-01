using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ewav.SmokeTests
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ewav.Security;

    public class CryptographyForTests : Cryptography
    {

        public CryptographyForTests(string KeyForConnectionStringPassphrase, string KeyForConnectionStringSalt, string KeyForConnectionStringVector)
        {
            passPhrase = KeyForConnectionStringPassphrase;  
            saltValue = KeyForConnectionStringSalt;   
            initVector = KeyForConnectionStringVector;     
        }

    }




}
