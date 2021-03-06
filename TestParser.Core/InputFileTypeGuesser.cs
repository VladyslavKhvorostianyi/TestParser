﻿using System;
using System.IO;
using BassUtils;

namespace TestParser.Core
{
    /// <summary>
    /// Try and figure out the type of data contained in a file.
    /// </summary>
    public static class InputFileTypeGuesser
    {
        /// <summary>
        /// Guesses the type of the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>Input file type.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file ' + filename + ' does not exist.</exception>
        public static InputFileType GuessFileType(string filename)
        {
            filename.ThrowIfFileDoesNotExist("filename");

            string extension = Path.GetExtension(filename).Substring(1);
            if (extension.Equals("trx", StringComparison.InvariantCultureIgnoreCase))
                return InputFileType.Trx;

            // NUnit by default creates files ending in ".xml" but that is not guaranteed.
            // Try and validate against the 2.x schema file taken from https://github.com/nunit/nunit/wiki/XML-Formats
            // UPDATE: I have seen NUnit results files that do not pass that schema...
            //var validator = new XsdValidator();
            //using (var s = Assembly.GetExecutingAssembly().GetResourceStream("nunit_schema_25.xsd"))
            //{
            //    validator.AddSchema(s);
            //    if (validator.IsValid(filename))
            //        return TestResultFileType.NUnit2;
            //}

            // Still no luck? Open the file and look for some stuff.
            string text = File.ReadAllText(filename);
            if (text.Contains("xmlns=\"http://microsoft.com/schemas/VisualStudio/TeamTest/2010\""))
                return InputFileType.Trx;
            if (text.Contains("nunit-version=\"2.") && text.Contains("test-results"))
                return InputFileType.NUnit2;
            if (text.Contains("<sourceFile name=") && text.Contains("<line number=") && text.Contains("coveringTests="))
                return InputFileType.NCrunchCoverage;

            throw new Exception("Could not guess type of data in " + filename);
        }
    }
}
