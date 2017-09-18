using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes.Tests
{
    public static class SampleGames
    {
        private const string BaseSTR = @"[Event ""Test Match Name""]
[Site ""Test Match Site""]
[Date ""1970.01.01""]
[Round ""1""]
[White ""White Player""]
[Black ""Black Player""]
[Result ""1/2-1/2""]

";
        public const string ScholarsMate = BaseSTR + @"1. e4 e5 2. Bc4 Nc6 3. Qh5 Nf6 4. Qxf7 1-0";

    }
}
