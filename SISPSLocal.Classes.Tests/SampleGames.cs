// <copyright file="SampleGames.cs" company="Rolls-Royce plc">
//   Copyright (c) 2017 Rolls-Royce plc
// </copyright>

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

        public const string ScholarsMate = BaseSTR
            + @"1. e4 e5 {standard opening} 2. Bc4 (2. d4) Nc6 3. Qh5 Nf6 4. Qxf7 1-0";
    }
}