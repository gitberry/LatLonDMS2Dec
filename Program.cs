using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        // Switch memory
        static bool showHelpAtEnd = false;
        static bool showDebug = false;

        // Format specifiers
        const string OutputFormatTEXT = "/Text";
        const string OutputFormatJSON = "/JSON";
        public static string echoBefore = "";
        public static string echoAfter = "";
        public static bool LONLATOutput = false;
        //todo add KML
        static string DefaultOutputFormat = OutputFormatTEXT;
        static string DesignatedOutputFormat = DefaultOutputFormat;
        public const string outputFormatting = "0.000000"; // If 6 decimal places is good enough for google it'll be good enough for this...                                                           
        public const string JSONCoordinateFormatString = "{{ \"type\": \"Point\", \"coordinates\": [ {0} ] }}"; // notes from https://macwright.com/2015/03/23/geojson-second-bite.html

        // error constants
        const string NotProperCoordinateResultString = "Not proper coordinate input.";
        const double ErrorCoordOutput = 666;
        static bool ErrorOccurred = false; //super kludgy...

        public class SingleCoordinate
        {
            public double Lat = ErrorCoordOutput; // default to error
            public double Lon = ErrorCoordOutput;
            public string ExceptionMessage = "";
        }

        static void Main(string[] args)
        {
            if (showDebug) { Console.WriteLine("Debug Start"); }
            var result = NotProperCoordinateResultString;

            result = ProcessAllInput(GetRealData());

            if (showDebug) { Console.WriteLine("===== DEBUG output start ====="); }
            Console.WriteLine(result);
            if (showDebug) { Console.WriteLine("===== DEBUG output end ====="); }

            if (showDebug) { Console.WriteLine("Debug End"); }
            if (showHelpAtEnd) { ShowHelp(); }
        }

        private static string GetRealData()
        {
            // we're getting the commandline info raw - because of quotes and such that don't populate the arrays quite right..
            var theRawData = System.Environment.CommandLine; if (showDebug) { Console.WriteLine(theRawData); }
            string xcodeBase = Assembly.GetExecutingAssembly().CodeBase; if (showDebug) { Console.WriteLine(xcodeBase); }
            string theRealData = theRawData.Replace(xcodeBase.Replace("file:///", "").Replace("/", "\\"), ""); if (showDebug) { Console.WriteLine(theRealData); }
            return theRealData;
        }

        private static string ProcessAllInput(string theRealData)
        {
            var result = NotProperCoordinateResultString;
            //global variables that should be reset after each call (it multiple tests blow up without this althouuuugh - shouldn't matter for single command line calls - best to fix...
            echoBefore = ""; echoAfter = ""; LONLATOutput = false; 

            // massaging the data - making Apple image EXIF geocordinates fix our expected input
            theRealData = massageInputs(theRealData); if (showDebug) { Console.WriteLine(theRealData); }

            var theRealArgs = theRealData.Split(" "); if (theRealData == "") theRealArgs = new string[0];

            if (theRealArgs.Length == 0) ShowHelp();
            if (theRealArgs.Length > 0)
            {
                if (theRealArgs[0] == "/H") ShowHelp(); //quick end..
                // scan through arguments - pick out the switches and act accordingly
                List<string> actualArgs = new List<string>();
                foreach (string arg in theRealArgs)
                {
                    var thisArg = arg.ToUpper().Replace("\"", ""); // feels cludgy??? breaking single purpose rule somehow?
                    // some special cases
                    if (thisArg.Contains("/ECHOBEFORE")) { echoBefore = thisArg.Replace("/ECHOBEFORE", ""); }
                    else
                        if (thisArg.Contains("/ECHOAFTER")) { echoAfter = thisArg.Replace("/ECHOAFTER", ""); }
                    else
                    {
                        switch (thisArg)
                        {
                            case OutputFormatTEXT: DesignatedOutputFormat = OutputFormatTEXT; break;
                            case OutputFormatJSON: DesignatedOutputFormat = OutputFormatJSON; break;
                            case "/LONLATOUTPUT": LONLATOutput = true; break;
                            case "/DEBUG": showDebug = true; Console.WriteLine("DEBUG ON"); break;
                            case "/TESTS": RunTests(); break; // order sensitive - can put debug on prior - but not after...
                            case "/H": showHelpAtEnd = true; break;
                            case "/HELP": showHelpAtEnd = true; break;
                            case "/HLP": showHelpAtEnd = true; break;
                            default:
                                actualArgs.Add(thisArg); // if it didn't get caught as a switch - then we use it as an argument
                                break;
                        }
                    }
                }
                result = ProcessCoordinateInput(actualArgs);
                if (ErrorOccurred) { result += $" Using Input: [{theRealData}]"; }
            }
            return result;
        }

        private static string ProcessCoordinateInput(List<string> actualArgs)
        {
            var result = NotProperCoordinateResultString;
            switch (actualArgs.Count)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    break; //errors and default indicates this
                case 4: // our plain solution
                    result = OneValueOutput(DMS2Decimal(actualArgs[0], actualArgs[1], actualArgs[2], actualArgs[3]));
                    break;
                default:
                    // input from apple EXIF metadata
                    result = CoordOutput(DMS2Decimal(actualArgs));
                    break;
            }
            ErrorOccurred = (ErrorOccurred || (result == NotProperCoordinateResultString));
            return result;
        }

        public static double DMS2Decimal(string D, string M, string S, string direction)
        {
            //a much more flexible input scheme from original here: https://stackoverflow.com/questions/3249700/convert-degrees-minutes-seconds-to-decimal-coordinates
            // I actually wanted to restrict input format - so I did the following
            // - but I LOVE the groups feature of the regex of the original by Alobidat (although I have never coded and tested their solution I loved the simple concept when reading the code!)
            double result = ErrorCoordOutput;

            var degrees = double.Parse("0" + D);
            var minutes = double.Parse("0" + M);
            var seconds = double.Parse("0" + S);
            direction = direction.ToUpper(); // just in case - still making assumptions about the value uggg...

            var dec = (Math.Abs(degrees) + minutes / 60d + seconds / 3600d) * (direction == "S" || direction == "W" ? -1 : 1);
            var absDec = Math.Abs(dec);

            if ((((direction == "W" || direction == "E") && degrees <= 180 & absDec <= 180) || (degrees <= 90 && absDec <= 90)) && minutes < 60 && seconds < 60)
            {                
                result = dec; // happy path!
            }

            return result;
        }

        public static SingleCoordinate DMS2Decimal(string[] args)
        {
            return DMS2Decimal(args.ToList());
        }

        public static SingleCoordinate DMS2Decimal(List<string> args)
        {
            var result = new SingleCoordinate();
            try
            {
                result.Lat = DMS2Decimal(args[0], args[1], args[2], args[3]);
                result.Lon = DMS2Decimal(args[4], args[5], args[6], args[7]);
            }
            catch (Exception ex) { result.ExceptionMessage = ex.Message; } // gentle fail - _should_ have 666 in one of the values..
            if (result.ExceptionMessage != "") { Console.WriteLine($"Error:[{result.ExceptionMessage}]"); }
            return result;
        }

        private static string OneValueOutput(double givenSingleValue)
        {
            string result = NotProperCoordinateResultString;
            if (givenSingleValue != ErrorCoordOutput)
            {
                switch (DesignatedOutputFormat)
                {
                    case OutputFormatJSON:
                        // it's not possible to make any JSON out of a single coordinate
                        // SO.... just to test output we fake it with a 0 for other
                        // alway assume Longitude 
                        result = SingleCoordinateJSON(givenSingleValue, 0);
                        break;
                    default: //
                        result = givenSingleValue.ToString(outputFormatting);
                        break;
                }
            }
            return result;
        }

        private static string CoordOutput(SingleCoordinate givenCoordinate)
        {
            string result = NotProperCoordinateResultString;
            if (!(givenCoordinate.Lat == ErrorCoordOutput || givenCoordinate.Lon == ErrorCoordOutput))
            {
                switch (DesignatedOutputFormat)
                {
                    case OutputFormatJSON:
                        result = SingleCoordinateJSON(givenCoordinate);
                        break;
                    default: 
                        if (LONLATOutput)
                        {
                            result = $"{echoBefore}{TwoDoubleFormattedOutput(givenCoordinate.Lon, givenCoordinate.Lat)}{echoAfter}"; // NOTE: Lon,Lat as per spec!!
                        }
                        else
                        {
                            result = $"{echoBefore}{TwoDoubleFormattedOutput(givenCoordinate.Lat, givenCoordinate.Lon)}{echoAfter}"; // NOTE: Lon,Lat as per spec!!
                        }
                        break;
                }
            }
            return result;
        }

        private static string SingleCoordinateJSON(SingleCoordinate givenCoordinate)
        {
            return SingleCoordinateJSON(givenCoordinate.Lat, givenCoordinate.Lon);
        }

        private static string SingleCoordinateJSON(double givenLat, double givenLon)
        {
            return string.Format(JSONCoordinateFormatString, TwoDoubleFormattedOutput(givenLon, givenLat)); // NOTE: Lon,Lat as per spec!!
        }

        private static string TwoDoubleFormattedOutput(double givenDouble1, double givenDouble2) // so we can flip our Lat's and Lon's according to specs and keep decimal formatting going through 1 place - HERE!!
        {
            return $"{givenDouble1.ToString(outputFormatting)},{givenDouble2.ToString(outputFormatting)}";  // no spaces is important automation feature..
        }

        private static void ShowHelp()
        {
            Console.WriteLine(
                "This can be a simple utility - or with extra parameters become a very handy                                       \n" +
                "tool for outputing consistently formatted GEOGRAPHIC data.                                                        \n" +
                "                                                                                                                  \n" +
                "LatLonDMS2Dec Usage:                                                                                              \n" +
                "Basic: 																										   \n" +
                "4 parameters: degrees, minutes, seconds, direction                                 							   \n" +
                " ie 44 30 45.0 N   ==>   44.512500																				   \n" +
                "    110 23 00.1 W  ==> -110.383361																				   \n" +
                "																												   \n" +
                "8 parameters: degrees, minutes, seconds, N/S degrees, minutes, seconds, E/W									   \n" +
                "ie 44 30 45.0 N 110 23 00.1 W  ==> 44.512500, -110.383361														   \n" +
                " 																												   \n" +
                "(designed for batch processing of apple EXIF extracted data)                                                      \n" +
                "example Apple EXIF extract: 53 deg 13' 24.91\" N, 105 deg 43' 3.13\" W 											   \n" +
                "NOTE: quotes and extras in the EXIF make the arguments in the app a little wonky so the app strips out the cruft  \n" +
                "																												   \n" +
                "Extra switches																									   \n" +
                "/H = puts a help at the end of the app run (if it's 1st parameter- thats all it will do)						   \n" +
                "/DEBUG = puts in debug and shows a lot of stuff																   \n" +
                "/TEST = will run a number of tests and quit																	   \n" +
                "/JSON - will output in GeoJSON																					   \n" +
                "/LONLATOUTPUT - will put the coordinates in Longitude then Latitude order (KML likes that)                        \n" +
                "* it is order sensitive, so if you put the /H after /TEST it won't show help									   \n" +
                "  similarlyl if you specify /JSON after /TEST it won't put the output of tests in JSON							   \n" +
                "   (which is as it should be - you aren't *usually* testing accuracy and JSON formatting)						   \n" +
                "Special Switches                                                                                                  \n" +
                "/ECHOBEFORE[WHATEVER] - WILL PUT THAT [WHATEVER] BEFORE YOUR TEXT OUTPUT                                          \n" +
                "/ECHOAFTER[WHATEVER] - WILL PUT THAT [WHATEVER] AFTER YOU TEXT OUTPUT                                             \n" +
                "(for hardcore batch functionality -hint use for creating batch files etc..)                                       \n" +
                "");
            Environment.Exit(1);
        }

        private static string massageInputs(string givenString)
        {
            //to match our input needs - Apple EXIF specific formatting "massaged" from this:
            //53 deg 13' 23.47" N, 105 deg 43' 27.14" W
            // to this:
            //53 13 23.47 N, 105 43 27.14 W
            var result = givenString.Replace("\"", "").ToUpper().Replace("DEG", "").Replace("'", "").Replace(",", "").Trim();
            var prevResult = "";
            while (result != prevResult)
            {
                prevResult = result;
                result = result.Replace("  ", " ");
            }
            return result;
        }

        public static string RIGHT(string value, int length) // WTF MS! - not a standard Right string function in c# after all these years???
        {
            // dutifully borrowed from here: https://stackoverflow.com/questions/16782786/right-function-in-c
            if (String.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= length ? value : value.Substring(value.Length - length);
        }

        #region Testing // to avoid uncessary testing infrastructure for a simple little app...

        public static void RunTests()
        {
            (int, int) result = (0, 0);
            Console.WriteLine("Starting Tests\n");
            //todo: add a variety of test values from around the world Mate! 
            result.Item1 += 1; result.Item2 += TestHarness("44 30 45.0 N", "44.512500");   // 4 params (some cool place 44°30'45.0"N 110°23'00.1"W ==> 44.512503, -110.383361 )
            result.Item1 += 1; result.Item2 += TestHarness("110 23 00.1 W", "-110.383361"); // 4 params (some cool place 44°30'45.0"N 110°23'00.1"W ==> 44.512503, -110.383361 )
            // it gotta ask myself - why 4 params?  Does this serve a useful function after code functional?  I think not - likely to excise to make simpler
            result.Item1 += 1; result.Item2 += TestHarness("53 deg 13' 24.91\" N, 105 deg 43' 3.13\" W", "53.223586,-105.717536");  //ground truth'd in Little Red River Park..
            result.Item1 += 1; result.Item2 += TestHarness("53 deg 13' 23.47\" N, 105 deg 43' 27.14\" W", "53.223186,-105.724206"); //ground truth'd in Little Red River Park..                        
            result.Item1 += 1; result.Item2 += TestHarness("53 deg 13' 23.47\" N, 105 deg 43' 27.14\" W /LONLATOUTPUT", "-105.724206,53.223186"); ///LONLATOUTPUT test
            result.Item1 += 1; result.Item2 += TestHarness("53 deg 13' 23.47\" N, 105 deg 43' 27.14\" /ECHOBEFOREFFOORR W", "FFOORR53.223186,-105.724206"); //ECHOBEFOR test            
            result.Item1 += 1; result.Item2 += TestHarness("53 deg 13' 23.47\" N, 105 deg 43' 27.14\" /ECHOAFTERAAFFTTEERR W", "53.223186,-105.724206AAFFTTEERR");//ECHOAFTER test            
            result.Item1 += 1; result.Item2 += TestHarness("53 deg 13' 23.47\" N, 105 deg 43' 27.14\" W /ECHOBEFOREFFOORR /ECHOAFTERAAFFTTEERR", "FFOORR53.223186,-105.724206AAFFTTEERR"); //ECHOBEFOR and ECHOAFTER test

            Console.WriteLine($"Finished Tests. Result: {result.Item2} out of {result.Item1} passed.");
            Environment.Exit(result.Item1 == result.Item2 ? 1 : 0);
        }

        private static int TestHarness(string givenString, string expectedResult)
        {
            Console.Write($"TestHarness ==>> {givenString} ==>>");
            var result = ProcessAllInput(givenString); // not true recursion but...
            Console.WriteLine($" [{(result == expectedResult ? "SUCCESS" : "FAIL")} : {expectedResult} == {result}]");
            return result == expectedResult ? 1 : 0;
        }

        #endregion

    }
}
