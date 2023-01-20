using System;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Debug Start");
            if (args.Length == 0) ShowHelp();
            if (args.Length > 0)
            {
                if (args[0] == "TESTS") RunTests();
                if (args[0] == "/H") ShowHelp();

                if (args.Length > 3) Console.WriteLine($"[{DMS2DecimalEXIFfromApple(args)}]");
                else ShowHelp();
                }
            Console.Write("Debug End");

            //Console.WriteLine($"[{LatLonUtils.DMS2Decimal(args[0])}]");
            //Console.WriteLine($"[{LatLonUtils.DMS2DecimalEXIFfromApple(args[0])}]");
            //Console.WriteLine($"[{DMS2DecimalEXIFfromApple(args[0])}]");

        }

        private static void ShowHelp()
        {
            Console.WriteLine(
                "This can be a simple utility - or with extra parameters become a very handy                                       \n" +
                "tool for outputing consistently formatted GEOGRAPHIC data.                                                        \n" +
                "                                                                                                                  \n" +
                "LatLonDMS2Dec Usage:                                                                                              \n" +
                "provide 3 parameters of Degrees, Minutes, Seconds - receive Degrees in Decimal format                             \n" +
                "provide more than 3 parameters and the following occurs                                                           \n" +
                "(designed for batch processing of apple EXIF extracted data)                                                      \n" +
                "example Apple EXIF extract: 53 deg 13' 24.91 N, 105 deg 43' 3.13 W                                                \n" +
                "4 parameters: the first, 3rd and 4th will be treated as degrees, minutes, seconds                                 \n" +
                " ie 53 deg 13' 24.91  => returns                                                                                  \n" +
                "5 parameters: the first, 3rd and 4th will be treated as D,M,S and the 5th discarded                               \n" +
                " ie 53 deg 13' 24.91 N  => returns                                                                                \n" +
                "6 parameters: the first will be echoed and the 2nd, 4th and 5th used for D,M,S and the 6th discarded              \n" +
                " ie IMG2345.JPG 53 deg 13' 24.91 N  => returns  IMG2345.JPG ??                                                    \n" +
                "7-10 parameters: the first will be echoed and the 2nd, 4th and 5th used for D,M,S and remaining discarded         \n" +
                " ie IMG2345.JPG 53 deg 13' 24.91 N, 105 deg 43' 3.13  => returns IMG2345.JPG ??                                   \n" +
                "11 parameters: the first will be echoed and parameters 2, 4, 5 and 7, 9, 10 used for DMS, DMS                     \n" +
                " ie IMG2345.JPG 53 deg 13' 24.91 N, 105 deg 43' 3.13 W => returns  IMG2345.JPG ??, ??                             \n" +
                "    NOTE - apple outputs in N,W order - so this option does that too...                                           \n" +
                "12-15 parameters: the first parameter is a switch and it's values are as follows:                                 \n" +
                " LonLat - will look in the 7th and 12th parameters if N, E or W put param 2 first then coordinates Lon first order\n" +
                "  and if 12 is W, add - sign to Lon value                                                                         \n" +
                "  and if parameters 13,14 or 15 exist - they will be passed into the output                                       \n" +
                " JSONLonLat does the same as above - but in more formal JSON format as follows:                                   \n" +
                "  {\"ID\":\"{Param2Value}\", \"Lat\":\"{LatValue}\", \"Lon\":\"{LatValue}\",                                      \n" +
                "   \"ImgURL\":\"{Param13Value}\",\"LinkURL\":\"{Param14Value},\"Desc\":\"{Param15Value}\"}                        \n" +
                "Note: if you are formatting values - base64 encode 13, 14, 15 to avoid issues                                        \n" +
                "");
            Environment.Exit(1);
        }

        private static void RunTests()
        {
            Console.WriteLine("Starting Tests\n");
            int ExitValue = 0;
            //todo: add a variety of test values from around the world Mate! 
            ExitValue += TestHarness("44 30 45.0", "44.512503"); // 3 params (some cool place 44°30'45.0"N 110°23'00.1"W ==> 44.512503, -110.383361 )
            ExitValue += TestHarness("110 23 00.1", "-110.383361"); // 3 params (some cool place 44°30'45.0"N 110°23'00.1"W ==> 44.512503, -110.383361 )

            //ExitValue += TestHarnessA("53 deg 13' 24.91 N, 105 deg 43' 3.13 W "); // 3 parameters - plain jane OK

            //ExitValue += TestHarnessA("53 deg 13' 24.91 N, 105 deg 43' 3.13 W "); // 4 parameters


            //ExitValue += TestHarnessA("53 deg 13' 24.91 N, 105 deg 43' 3.13 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 25.13 N, 105 deg 43' 3.85 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 24.06 N, 105 deg 43' 6.73 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 21.95 N, 105 deg 43' 6.35 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 21.23 N, 105 deg 43' 8.43 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 21.63 N, 105 deg 43' 11.89 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 23.53 N, 105 deg 43' 14.31 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 22.32 N, 105 deg 43' 16.48 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 20.19 N, 105 deg 43' 18.73 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 18.94 N, 105 deg 43' 22.14 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 18.58 N, 105 deg 43' 25.49 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 18.64 N, 105 deg 43' 25.49 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 19.26 N, 105 deg 43' 29.09 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 17.95 N, 105 deg 43' 25.85 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 18.09 N, 105 deg 43' 22.17 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 15.96 N, 105 deg 43' 23.68 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.54 N, 105 deg 43' 26.15 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.54 N, 105 deg 43' 26.15 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 13.68 N, 105 deg 43' 23.49 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.19 N, 105 deg 43' 19.89 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.37 N, 105 deg 43' 16.54 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.41 N, 105 deg 43' 12.69 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.38 N, 105 deg 43' 9.09 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.56 N, 105 deg 43' 5.41 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 13.25 N, 105 deg 43' 3.08 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 13.23 N, 105 deg 43' 3.16 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 11.69 N, 105 deg 43' 2.09 W "); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 11.00 N, 105 deg 42' 59.04 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 11.04 N, 105 deg 42' 59.01 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 10.86 N, 105 deg 42' 55.50 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 10.66 N, 105 deg 42' 51.87 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 10.78 N, 105 deg 42' 45.17 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 10.77 N, 105 deg 42' 45.14 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.93 N, 105 deg 42' 46.30 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 14.93 N, 105 deg 42' 46.41 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 16.91 N, 105 deg 42' 48.91 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 18.58 N, 105 deg 42' 50.94 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 18.64 N, 105 deg 42' 54.76 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 20.55 N, 105 deg 42' 56.49 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 21.67 N, 105 deg 42' 56.79 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 22.13 N, 105 deg 43' 20.11 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 22.13 N, 105 deg 43' 20.11 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 23.90 N, 105 deg 43' 22.33 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 23.88 N, 105 deg 43' 22.28 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 23.47 N, 105 deg 43' 27.14 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 20.45 N, 105 deg 43' 26.45 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 20.51 N, 105 deg 43' 26.51 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 20.64 N, 105 deg 43' 22.77 W"); //.Trim().Split(" "));
            //ExitValue += TestHarnessA("53 deg 13' 21.78 N, 105 deg 43' 20.35 W"); //.Trim().Split(" "));
            Console.WriteLine("Finished Tests.");
            Environment.Exit(ExitValue);
        }

        // Test Harness has only 1 possibility - 3 params are a valid coordinate or not...
        private static int TestHarness(string givenString, string expectedResult)
        {
            Console.Write($"TestHarness ==>> {givenString} ==>>");
            var input = givenString.Trim().Split(" ");
            var result = DMS2Decimal(input[0], input[1], input[2]);
            Console.WriteLine($"{expectedResult} == {result}");
            return result == expectedResult ? 1 : 0; // TestHarnessA2(givenString.Trim().Split(" "));

        }


        // A series test harnesses are for Apple formatted EXIF
        private static int TestHarnessA(string givenString)
        {
            Console.Write($"TestHarnessA ==>> {givenString} ==>>");
            return TestHarnessA2(givenString.Trim().Split(" "));
        }

        private static int TestHarnessA2(string[] args)
        {
            var cnt = 0;
            foreach (string xstring in args)
            {
                cnt += 1;
                Console.Write($"{cnt}|{xstring};");
            }
            Console.WriteLine($"  Count:{cnt}");
            return 0;
        }

        //    }

        //public class LatLonUtils

        //{
        //original from here: https://stackoverflow.com/questions/3249700/convert-degrees-minutes-seconds-to-decimal-coordinates
        //static Regex reg = new Regex(@"^((?<D>\d{1,2}(\.\d+)?)(?<W>[SN])|(?<D>\d{2})(?<M>\d{2}(\.\d+)?)(?<W>[SN])|(?<D>\d{2})(?<M>\d{2})(?<S>\d{2}(\.\d+)?)(?<W>[SN])|(?<D>\d{1,3}(\.\d+)?)(?<W>[WE])|(?<D>\d{3})(?<M>\d{2}(\.\d+)?)(?<W>[WE])|(?<D>\d{3})(?<M>\d{2})(?<S>\d{2}(\.\d+)?)(?<W>[WE]))$");

        //public static double DMS2Decimal(string dms)
        //{
        //    double result = double.NaN;

        //    var match = reg.Match(dms);

        //    if (match.Success)
        //    {
        //        var degrees = double.Parse("0" + match.Groups["D"]);
        //        var minutes = double.Parse("0" + match.Groups["M"]);
        //        var seconds = double.Parse("0" + match.Groups["S"]);
        //        var direction = match.Groups["W"].ToString();
        //        var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d) * (direction == "S" || direction == "W" ? -1 : 1);
        //        var absDec = Math.Abs(dec);

        //        if ((((direction == "W" || direction == "E") && degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
        //        {
        //            result = dec;
        //        }

        //    }

        //    return result;
        //}

        static string NotAnAppleEXIFCoordinateStringResultValue = "Not an Apple EXIF coordinate string.";

        public static string DMS2DecimalEXIFfromApple(string[] DMS)
        {
            //original from here: https://stackoverflow.com/questions/3249700/convert-degrees-minutes-seconds-to-decimal-coordinates
            // modified to match Apple's EXIF format
            // a feature of Apple's format is that it has a double quote for seconds - SO... what should be 5 parameters ends up being 4...
            string result = NotAnAppleEXIFCoordinateStringResultValue; //"Not an Apple EXIF coordinate string.";  //string.Empty;

            //var givenArray = dms.Split(' ');
            result += $"length={DMS.Length}for[{string.Join("|",DMS)}]";
            if (DMS.Length >= 4)
            {
                result = "we have more than 4";
                // if " didn't mess things up:
                //if (Righty(DMS[2], 1) == "'" && Righty(DMS[3], 1) == "\"" && DMS[1].ToLower() == "deg" && (DMS[4].ToUpper() == "N" || DMS[4].ToUpper() == "W" || DMS[4].ToUpper() == "E"))
                if (Righty(DMS[2], 1) == "'" && DMS[1].ToLower() == "deg" && (Righty(DMS[3],1).ToUpper() == "N" || Righty(DMS[3], 1).ToUpper() == "W" || Righty(DMS[3], 1).ToUpper() == "E"))
                {
                    result = DMS2Decimal(DMS[0], SanitizeAppleM(DMS[2]), SanitizeAppleS(DMS[3]), ExtractAppleDirection(DMS[3]));
                    ////return $"do something with {DMS}";
                    //var degrees = double.Parse("0" + DMS[0]);
                    //var minutes = double.Parse("0" + DMS[2].Replace("'", "")); ;
                    //var seconds = double.Parse("0" + DMS[3].Substring(0, 5));
                    //var direction = Righty(DMS[3], 1).ToUpper();
                    //var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d) * (direction == "S" || direction == "W" ? -1 : 1);
                    //var absDec = Math.Abs(dec);
                    //
                    //    if ((((direction == "W" || direction == "E") && degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
                    //    {
                    //        result = $"{dec}"; // all is good - let's have it -otherwise it's formatted wonky...
                    //    }
                }
            }
            //var match = reg.Match(dms);

            //if (match.Success)
            //{
            //    var degrees = double.Parse("0" + match.Groups["D"]);
            //    var minutes = double.Parse("0" + match.Groups["M"]);
            //    var seconds = double.Parse("0" + match.Groups["S"]);
            //    var direction = match.Groups["W"].ToString();
            //    var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d) * (direction == "S" || direction == "W" ? -1 : 1);
            //    var absDec = Math.Abs(dec);

            //    if ((((direction == "W" || direction == "E") && degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
            //    {
            //        result = dec;
            //    }

            //}

            return result;
        }

        public static string SanitizeAppleM(string M)
        {
            return M.Replace("'", "");
        }

        public static string SanitizeAppleS(string S)
        {
            return S.Substring(0, Math.Min(S.Length, 5));
        }

        public static string ExtractAppleDirection(string S)
        {
            //if (S.Length < 3) return "W"; 
            //else
            //{
            //    return Righty(S, 1).ToUpper();
            //}
            return Righty(S, 1).ToUpper();  // ASSUME - makes an ASS out of U and ME!!!
        }

        static string NotProperCoordinateResultString = "Not proper coordinate information.";

        public static string DMS2Decimal(string D, string M, string S)
        {
            // pure math - no error checking for Direction
            string result = NotProperCoordinateResultString; // "Not proper coordinate information.";  //string.Empty;           
            var degrees = double.Parse("0" + D);
            var minutes = double.Parse("0" + M);
            var seconds = double.Parse("0" + S);
            //direction = direction.ToUpper(); // just in case - still making assumptions about the value uggg...

            var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d); // * (direction == "S" || direction == "W" ? -1 : 1);
            var absDec = Math.Abs(dec);

            // basic error checking - won't really know if you're a Lat or Lon - 
            if ((( degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
            {
                result = $"{dec}"; // all is good - let's have it -otherwise it's wonky input...
                //todo:  line up testing harnes so that 3 works?? or not perhaps?
            }

            return result;
        }
        public static string DMS2Decimal(string D, string M, string S, string direction)
        {
            //original from here: https://stackoverflow.com/questions/3249700/convert-degrees-minutes-seconds-to-decimal-coordinates
            string result = NotProperCoordinateResultString; // "Not proper coordinate information.";  //string.Empty;           

            var degrees = double.Parse("0" + D);
            var minutes = double.Parse("0" + M);
            var seconds = double.Parse("0" + S);
            direction = direction.ToUpper(); // just in case - still making assumptions about the value uggg...

            var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d) * (direction == "S" || direction == "W" ? -1 : 1);
            var absDec = Math.Abs(dec);

            if ((((direction == "W" || direction == "E") && degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
            {
                result = $"{dec}"; // all is good - let's have it -otherwise it's wonky input...
                //todo:  line up testing harnes so that 3 works?? or not perhaps?
            }
                
            return result;
        }



        public static string Righty(string value, int length) // WTF MS! - not a standard Right string function in c# after all these years???
        {
            // dutifully stolen from here: https://stackoverflow.com/questions/16782786/right-function-in-c
            if (String.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= length ? value : value.Substring(value.Length - length);
        }
        //    }

    }
}
